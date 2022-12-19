using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ExileCore.Shared.Helpers;

namespace ExpeditionIcons;

public static class Extensions
{
    public static bool DistanceLessThanOrEqual(this Vector2 v, Vector2 other, float distance)
    {
        return v.DistanceSquared(other) < distance * distance;
    }
}

public class PathPlanner
{
    private readonly Dictionary<object, double> _lootValueTable = new(ReferenceEqualityComparer.Instance);
    private readonly PlannerSettings _settings;

    public PathPlanner(PlannerSettings settings)
    {
        _settings = settings;
    }

    public double GetScore(List<Vector2> state, ExpeditionEnvironment environment)
    {
        var relics = new HashSet<IExpeditionRelic>();
        var lootList = new HashSet<IExpeditionLoot>();
        var score = 0.0;
        foreach (var explosionPoint in state)
        {
            foreach (var (_, relic) in environment.Relics.Where(x => x.Item1.Distance(explosionPoint) <= environment.ExplosionRadius))
            {
                relics.Add(relic);
            }

            foreach (var (_, loot) in environment.Loot
                         .Where(x => x.Item1.DistanceLessThanOrEqual(explosionPoint, environment.ExplosionRadius))
                         .Where(x => lootList.Add(x.Item2)))
            {
                var (multiplier, sum) = relics.Select(x => x.GetScoreMultiplier(loot)).Aggregate((mult: 1.0, sum: 0.0), (a, b) => (a.mult * b.Item1, a.sum + b.Item2));
                score += _lootValueTable[loot] * multiplier * (1 + sum);
            }
        }

        return score;
    }

    private static Vector2 GetNextPosition(Vector2 position, float radius)
    {
        var length = Random.Shared.Next(2) == 0 ? radius : GetWeightedLength(radius);
        var angle = Random.Shared.NextSingle() * MathF.PI * 2;
        var (sin, cos) = MathF.SinCos(angle);
        return position + new Vector2(cos * length, sin * length);
    }

    private static float GetWeightedLength(float radius)
    {
        return Math.Max(Random.Shared.NextSingle(), Random.Shared.NextSingle()) * radius;
    }

    private static List<Vector2> MutatePath(Vector2 startingPoint, float radius, List<Vector2> originalPath)
    {
        var mutateTimes = Random.Shared.Next(1, 4);
        List<Vector2> newPath = null;
        for (var mutation = 0; mutation < mutateTimes; mutation++)
        {
            var changeIndex = Random.Shared.Next(originalPath.Count);
            Vector2 changedPoint;
            var previousPoint = changeIndex == 0 ? startingPoint : originalPath[changeIndex - 1];
            var changingPoint = originalPath[changeIndex];
            if (Random.Shared.Next(2) == 0)
            {
                changedPoint = GetNextPosition(previousPoint, radius);
            }
            else
            {
                var allowedMoveRadius = Math.Max(radius - previousPoint.Distance(changingPoint), radius / 5);
                do
                {
                    changedPoint = GetNextPosition(changingPoint, allowedMoveRadius);
                } while (!previousPoint.DistanceLessThanOrEqual(changedPoint, radius));
            }

            newPath = originalPath.ToList();
            newPath[changeIndex] = changedPoint;
            for (var i = changeIndex + 1; i < newPath.Count; i++)
            {
                var maxLength = GetWeightedLength(radius);
                var difference = newPath[i] - newPath[i - 1];
                var diffLength = difference.Length();
                if (diffLength > maxLength)
                {
                    newPath[i] = newPath[i - 1] + difference * (maxLength / diffLength);
                }
                else
                {
                    break;
                }
            }

            originalPath = newPath;
        }

        return newPath;
    }

    private static List<Vector2> MergePaths(List<Vector2> path1, List<Vector2> path2)
    {
        if (path1.Count != path2.Count)
        {
            throw new Exception("Paths have different lengths");
        }

        var factor = Random.Shared.NextSingle();
        return path1.Zip(path2).Select(x => x.First * factor + x.Second * (1 - factor)).ToList();
    }

    private static T RandomBiasedElement<T>(List<T> elements)
    {
        return elements[Math.Min(Random.Shared.Next(elements.Count), Random.Shared.Next(elements.Count))];
    }

    public IEnumerable<PathState> GetBestPathSeries(ExpeditionEnvironment environment)
    {
        var bestPath = Enumerable.Repeat(Vector2.Zero, environment.MaxExplosions).ToList();
        var batch = Enumerable.Range(0, _settings.PathGenerationSize * 2).Select(_ => BuildPath(environment)).ToList();
        while (true)
        {
            var batchWithValues = batch
                .Select(x => (GetScore(x, environment), x))
                .OrderByDescending(x => x.Item1)
                .Take(_settings.PathGenerationSize)
                .ToList();
            var mixedAndMutated = batchWithValues
                .Concat(batchWithValues)
                .Select(i => i.x)
                //.Select(x => Random.Shared.NextDouble() > _settings.PathMixChance ? x : MergePaths(x, RandomBiasedElement(batch)))
                .Select(x => Random.Shared.NextDouble() > _settings.PathMutateChance ? x : MutatePath(environment.StartingPoint, environment.ExplosionRange, x));
            var newPaths = Enumerable.Range(0, (int)(_settings.PathGenerationSize * _settings.NewRandomPathInjectionRate)).Select(_ => BuildPath(environment));
            var newBatch = mixedAndMutated.Append(bestPath).Concat(newPaths).ToList();
            if (batchWithValues[0].Item1 > GetScore(bestPath, environment))
            {
                bestPath = batchWithValues[0].x;
            }

            yield return new PathState(bestPath, GetScore(bestPath, environment));
            batch = newBatch;
        }
    }

    private List<Vector2> BuildPath(ExpeditionEnvironment environment)
    {
        _lootValueTable.Clear();
        foreach (var (_, loot) in environment.Loot)
        {
            _lootValueTable[loot] = loot switch
            {
                RunicMonster => _settings.RunicMonsterWeight,
                ArtifactChest => _settings.ArtifactChestWeight,
                OtherChest => _settings.OtherChestWeight,
                NormalMonster => _settings.NormalMonsterWeight,
            };
            _lootValueTable.TrimExcess();
        }

        var path = new List<Vector2>(environment.MaxExplosions);
        if (Random.Shared.Next(2) != 0 && environment.Relics.Any())
        {
            var relic = environment.Relics[Random.Shared.Next(environment.Relics.Count)];
            var current = environment.StartingPoint;
            do
            {
                var diff = relic.Item1 - current;
                if (diff.Length() < environment.ExplosionRange)
                {
                    path.Add(relic.Item1);
                }
                else
                {
                    current += diff * (environment.ExplosionRange / diff.Length());
                    path.Add(current);
                }
            } while (!current.DistanceLessThanOrEqual(relic.Item1, environment.ExplosionRadius) &&
                     path.Count < environment.MaxExplosions);
        }

        var point = path.LastOrDefault(environment.StartingPoint);
        while (path.Count < environment.MaxExplosions)
        {
            path.Add(point = GetNextPosition(point, environment.ExplosionRange));
        }

        return path;
    }
}

public record PathState(List<Vector2> Points, double Score);

public record ExpeditionEnvironment(List<(Vector2, IExpeditionRelic)> Relics, List<(Vector2, IExpeditionLoot)> Loot, float ExplosionRange, float ExplosionRadius,
    int MaxExplosions, Vector2 StartingPoint);

public interface IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot);
}

public record DoubledMonstersRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is RunicMonster)
        {
            return (2, 0);
        }

        return (1, 0);
    }
}

public class IncreasedMonsterLootRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IMonster)
        {
            return (1, 0.4);
        }

        return (1, 0);
    }
}

public class IncreasedMonsterArtifactsRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IMonster)
        {
            return (1, 0.4);
        }

        return (1, 0);
    }
}

public class IncreasedChestLootRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IChest)
        {
            return (1, 0.4);
        }

        return (1, 0);
    }
}

public class IncreasedChestArtifactsRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is ArtifactChest)
        {
            return (1, 0.4);
        }

        return (1, 0);
    }
}

public class OtherMonsterRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IMonster)
        {
            return (1, 0.15);
        }

        return (1, 0);
    }
}

public class OtherChestRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IChest)
        {
            return (1, 0.15);
        }

        return (1, 0);
    }
}

public class OtherGoodMonsterRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IMonster)
        {
            return (1, 0.25);
        }

        return (1, 0);
    }
}

public class OtherGoodChestRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IChest)
        {
            return (1, 0.25);
        }

        return (1, 0);
    }
}

public class LogbookMonsterRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IMonster)
        {
            return (1.5, 0);
        }

        return (1, 0);
    }
}

public class LogbookChestRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IChest)
        {
            return (1.5, 0);
        }

        return (1, 0);
    }
}

public class FracturedMonsterRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IMonster)
        {
            return (1.3, 0);
        }

        return (1, 0);
    }
}

public class FracturedChestRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IChest)
        {
            return (1.3, 0);
        }

        return (1, 0);
    }
}

public class PackSizeMonsterRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        if (loot is IMonster)
        {
            return (1.25, 0);
        }

        return (1, 0);
    }
}

public record WarningRelic : IExpeditionRelic
{
    public (double, double) GetScoreMultiplier(IExpeditionLoot loot)
    {
        return (0, 0);
    }
}

public interface IExpeditionLoot
{
}

public interface IChest : IExpeditionLoot
{
}

public interface IMonster : IExpeditionLoot
{
}

public class RunicMonster : IMonster
{
}

public class NormalMonster : IMonster
{
}

public class ArtifactChest : IChest
{
}

public class OtherChest : IChest
{
}
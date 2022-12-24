using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Nodes;
using GameOffsets.Native;
using ImGuiNET;
using SharpDX;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace ExpeditionIcons;

public class ExpeditionIcons : BaseSettingsPlugin<ExpeditionIconsSettings>
{
    private const string MarkerPath = "Metadata/MiscellaneousObjects/Expedition/ExpeditionMarker";
    private const string ExplosivePath = "Metadata/MiscellaneousObjects/Expedition/ExpeditionExplosive";
    private const string RelicPath = "Metadata/MiscellaneousObjects/Expedition/ExpeditionRelic";

    private const string TextureName = "Icons.png";
    private const double CameraAngle = 38.7 * Math.PI / 180;
    private static readonly float CameraAngleCos = (float)Math.Cos(CameraAngle);
    private static readonly float CameraAngleSin = (float)Math.Sin(CameraAngle);

    private const float GridToWorldMultiplier = 250 / 23f;

    //TODO
    private const int ExplosiveBaseRange = 87;
    private const int ExplosiveBaseRadius = 30;

    private readonly ConcurrentDictionary<string, List<ExpeditionMarkerIconDescription>> _relicModIconMapping = new();
    private readonly ConcurrentDictionary<string, ExpeditionMarkerIconDescription> _metadataIconMapping = new();
    private readonly Dictionary<uint, EntityCacheItem> _cachedEntities = new Dictionary<uint, EntityCacheItem>();
    private readonly ConcurrentDictionary<string, ExpeditionEntityType> _entityTypeCache = new();
    private double _mapScale;
    private Vector2 _mapCenter;
    private bool _largeMapOpen;
    private Vector2 _playerGridPos;
    private float _playerZ;
    private List<Vector2> _explosives2DPositions;
    private float _explosiveRadius;
    private float _explosiveRange;
    private PathPlannerRunner _plannerRunner;
    private Vector2? _detonatorPos;
    private bool _zoneCleared;
    private int[][] _pathfindingData;
    private Vector2i _areaDimensions;

    private Camera Camera => GameController.Game.IngameState.Camera;

    private Vector2? DetonatorPos => _detonatorPos ??= RealDetonatorPos;

    private Vector2? RealDetonatorPos => DetonatorEntity is { } e
        ? e.GridPosNum
        : null;

    private Entity DetonatorEntity =>
        GameController.EntityListWrapper.ValidEntitiesByType[EntityType.IngameIcon]
            .FirstOrDefault(x => x.Path == "Metadata/MiscellaneousObjects/Expedition/ExpeditionDetonator");

    public override bool Initialise()
    {
        Graphics.InitImage(TextureName);
        Settings._iconsImageId = Graphics.GetTextureId(TextureName);
        Settings.PlannerSettings.StartSearch.OnPressed += StartSearch;
        Settings.PlannerSettings.StopSearch.OnPressed += StopSearch;
        Settings.PlannerSettings.ClearSearch.OnPressed += ClearSearch;
        RegisterHotkey(Settings.PlannerSettings.StartSearchHotkey);
        RegisterHotkey(Settings.PlannerSettings.StopSearchHotkey);
        RegisterHotkey(Settings.PlannerSettings.ClearSearchHotkey);
        return base.Initialise();
    }

    private static void RegisterHotkey(HotkeyNode hotkey)
    {
        Input.RegisterKey(hotkey);
        hotkey.OnValueChanged += () => { Input.RegisterKey(hotkey); };
    }

    private void StopSearch()
    {
        if (_plannerRunner is { } run)
        {
            run.Stop();
            Settings.PlannerSettings.SearchState = SearchState.Stopped;
        }
        else
        {
            Settings.PlannerSettings.SearchState = SearchState.Empty;
        }
    }

    private void StartSearch()
    {
        _plannerRunner?.Stop();
        var plannerRunner = new PathPlannerRunner();
        plannerRunner.Start(Settings.PlannerSettings, BuildEnvironment());
        _plannerRunner = plannerRunner;
        Settings.PlannerSettings.SearchState = SearchState.Searching;
    }

    private void ClearSearch()
    {
        if (_plannerRunner is { } run)
        {
            run.Stop();
            _plannerRunner = null;
        }
    }

    public override void AreaChange(AreaInstance area)
    {
        _plannerRunner?.Stop();
        _plannerRunner = null;
        _detonatorPos = null;
        _cachedEntities.Clear();
        _zoneCleared = false;
        _pathfindingData = GameController.IngameState.Data.RawPathfindingData;
        _areaDimensions = GameController.IngameState.Data.AreaDimensions;
    }

    private ExpeditionEntityType GetEntityType(string path)
    {
        return _entityTypeCache.GetOrAdd(path, p => p switch
        {
            RelicPath => ExpeditionEntityType.Relic,
            MarkerPath => ExpeditionEntityType.Marker,
            _ when p.StartsWith("Metadata/Terrain/Leagues/Expedition/Tiles/ExpeditionChamber") => ExpeditionEntityType.Cave,
            _ when p.StartsWith("Metadata/Terrain/Leagues/Expedition/Tiles/ExpeditionBossDispenser") => ExpeditionEntityType.Boss,
            _ => ExpeditionEntityType.None,
        });
    }

    private Vector3 ExpandWithTerrainHeight(Vector2 gridPosition)
    {
        return new Vector3(gridPosition.GridToWorld(), GameController.IngameState.Data.GetTerrainHeightAt(gridPosition));
    }

    private void DrawCirclesInWorld(List<Vector3> positions, float radius, Color color)
    {
        const int segments = 90;
        const int segmentSpan = 360 / segments;
        var playerPos = GameController.Player.GetComponent<Positioned>().WorldPosNum;
        foreach (var position in positions
                     .Where(x => playerPos.Distance(new Vector2(x.X, x.Y)) < 80 * GridToWorldMultiplier + radius))
        {
            foreach (var segmentId in Enumerable.Range(0, segments))
            {
                (Vector2, Vector2) GetVector(int i)
                {
                    var (sin, cos) = MathF.SinCos(MathF.PI / 180 * i);
                    var offset = new Vector2(cos, sin) * radius;
                    var xy = position.Xy() + offset;
                    var screen = Camera.WorldToScreen(ExpandWithTerrainHeight(xy.WorldToGrid()));
                    return (xy, screen);
                }

                var segmentOrigin = segmentId * segmentSpan;
                var (w1, c1) = GetVector(segmentOrigin);
                var (w2, c2) = GetVector(segmentOrigin + segmentSpan);
                if (Settings.ExplosivesSettings.EnableExplosiveRadiusMerging)
                {
                    if (positions
                        .Where(x => x != position)
                        .Select(x => new Vector2(x.X, x.Y))
                        .Any(x => Vector2.Distance(w1, x) < radius &&
                                  Vector2.Distance(w2, x) < radius))
                    {
                        continue;
                    }
                }

                Graphics.DrawLine(c1, c2, 1, color);
            }
        }
    }

    public override Job Tick()
    {
        Settings._iconsImageId = Graphics.GetTextureId(TextureName);
        Settings.PlannerSettings.SearchState = _plannerRunner switch
        {
            { IsRunning: true } => SearchState.Searching,
            { IsRunning: false, CurrentBestPath.Count: > 0 } => SearchState.Stopped,
            _ => SearchState.Empty
        };
        var detonatorPos = DetonatorPos;
        _playerGridPos = GameController.Player.GetComponent<Positioned>().WorldPosNum.WorldToGrid();
        if (detonatorPos is { } dp && _playerGridPos.Distance(dp) < 90)
        {
            _zoneCleared = DetonatorEntity?.IsTargetable != true;
            if (_zoneCleared)
            {
                ClearSearch();
                return null;
            }
        }

        var ingameUi = GameController.Game.IngameState.IngameUi;
        var map = ingameUi.Map;
        var largeMap = map.LargeMap.AsObject<SubMap>();
        _largeMapOpen = largeMap.IsVisible;
        _mapScale = GameController.IngameState.Camera.Height / 677f * largeMap.Zoom;
        _mapCenter = largeMap.GetClientRect().TopLeft.ToVector2Num() + largeMap.ShiftNum + largeMap.DefaultShiftNum;
        _playerZ = GameController.Player.GetComponent<Render>().Z;

        _explosiveRadius = Settings.ExplosivesSettings.CalculateRadiusAutomatically
            //ReSharper disable once PossibleLossOfFraction
            //rounding here is extremely important to get right, this is taken from the game's code
            ? ExplosiveBaseRadius * (100 + GameController.IngameState.Data.MapStats?.GetValueOrDefault(GameStat.MapExpeditionExplosionRadiusPct) ?? 0) / 100 * GridToWorldMultiplier
            : Settings.ExplosivesSettings.ExplosiveRadius.Value;
        //ReSharper disable once PossibleLossOfFraction
        //rounding here is extremely important to get right, this is taken from the game's code
        _explosiveRange = ExplosiveBaseRange * (100 + GameController.IngameState.Data.MapStats?.GetValueOrDefault(GameStat.MapExpeditionMaximumPlacementDistancePct) ?? 0) / 100 *
                          GridToWorldMultiplier;

        foreach (var entity in new[] { EntityType.IngameIcon, EntityType.Terrain }
                     .SelectMany(x => GameController.EntityListWrapper.ValidEntitiesByType[x]))
        {
            if (GetEntityType(entity.Path) != ExpeditionEntityType.None)
            {
                if (_cachedEntities.TryGetValue(entity.Id, out var oldValue))
                {
                    _cachedEntities[entity.Id] = oldValue.Merge(BuildCacheItem(entity));
                }
                else
                {
                    _cachedEntities[entity.Id] = BuildCacheItem(entity);
                }
            }
        }

        return null;
    }

    private ExpeditionEnvironment BuildEnvironment()
    {
        if (DetonatorPos is not { } detonatorPos)
        {
            throw new Exception("Unable to plan a path: detonator position is unknown");
        }

        var loot = new List<(Vector2, IExpeditionLoot)>();
        var relics = new List<(Vector2, IExpeditionRelic)>();
        foreach (var e in _cachedEntities.Values)
        {
            switch (GetEntityType(e.Path))
            {
                case ExpeditionEntityType.Marker:
                {
                    var animatedMetaData = e.BaseAnimatedEntityMetadata;
                    if (animatedMetaData != null)
                    {
                        if (animatedMetaData.Contains("elitemarker"))
                        {
                            loot.Add((e.GridPos, new RunicMonster()));
                        }
                        else
                        {
                            var iconDescription = _metadataIconMapping.GetOrAdd(animatedMetaData,
                                a => Icons.LogbookChestIcons.FirstOrDefault(icon =>
                                    icon.BaseEntityMetadataSubstrings.Any(a.Contains)));
                            if (iconDescription != null)
                            {
                                loot.Add((e.GridPos, new OtherChest()));
                            }
                        }
                    }

                    continue;
                }
                case ExpeditionEntityType.Relic:
                {
                    var mods = e.Mods;
                    if (mods == null)
                    {
                        continue;
                    }

                    if (e.MinimapIconHide != false) continue;
                    if (!mods.Any(x => x.Contains("ExpeditionRelicModifier"))) continue;

                    if (ContainsWarnMods(mods))
                    {
                        relics.Add((e.GridPos, new WarningRelic()));
                        continue;
                    }

                    var iconDescriptions = mods.SelectMany(mod =>
                        _relicModIconMapping.GetOrAdd(mod, s =>
                            Icons.ExpeditionRelicIcons.Where(icon =>
                                icon.BaseEntityMetadataSubstrings.Any(s.Contains)).ToList())).Distinct();
                    var allSubStrings = iconDescriptions.SelectMany(d => d.BaseEntityMetadataSubstrings).ToList();
                    var fittingMods = mods
                        .SelectMany(mod => allSubStrings.Where(mod.Contains))
                        .Distinct()
                        .Select(Icons.GetRelicType);
                    relics.AddRange(fittingMods.Select(expeditionRelic => (e.GridPos, expeditionRelic)));

                    break;
                }
                case ExpeditionEntityType.Cave:
                {
                    //Shits given about performance: some? a few?
                    for (int i = 0; i < Settings.PlannerSettings.LogbookCaveRunicMonsterMultiplier; i++)
                    {
                        loot.Add((e.GridPos, new RunicMonster()));
                    }

                    for (int i = 0; i < Settings.PlannerSettings.LogbookCaveArtifactChestMultiplier; i++)
                    {
                        loot.Add((e.GridPos, new ArtifactChest()));
                    }

                    break;
                }
                case ExpeditionEntityType.Boss:
                {
                    //Shits given about performance: some? a few?
                    for (int i = 0; i < Settings.PlannerSettings.LogbookBossRunicMonsterMultiplier; i++)
                    {
                        loot.Add((e.GridPos, new RunicMonster()));
                    }

                    break;
                }
            }
        }

        return new ExpeditionEnvironment(
            relics.FindAll(x => x.Item2 != null),
            loot.FindAll(x => x.Item2 != null),
            _explosiveRange / GridToWorldMultiplier,
            _explosiveRadius / GridToWorldMultiplier,
            GameController.IngameState.IngameUi.ExpeditionDetonatorElement.RemainingExplosives,
            detonatorPos,
            IsValidPlacement);
    }

    private bool IsValidPlacement(Vector2 x)
    {
        return x.X >= 0 && x.Y >= 0 &&
               x.X < _areaDimensions.X &&
               x.Y < _areaDimensions.Y &&
               _pathfindingData[(int)x.Y][(int)x.X] > 3;
    }

    public override void Render()
    {
        if (Settings.PlannerSettings.ClearSearchHotkey.PressedOnce())
        {
            ClearSearch();
        }

        if (Settings.PlannerSettings.StopSearchHotkey.PressedOnce())
        {
            StopSearch();
        }

        if (_zoneCleared)
        {
            return;
        }

        if (Settings.PlannerSettings.StartSearchHotkey.PressedOnce())
        {
            StartSearch();
        }

        var explosives3D = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.IngameIcon]
            .Where(x => x.Path == ExplosivePath)
            .Select(x => x.PosNum)
            .ToList();
        _explosives2DPositions = explosives3D.Select(x => new Vector2(x.X, x.Y)).ToList();
        if (Settings.ExplosivesSettings.ShowExplosives)
        {
            DrawCirclesInWorld(
                positions: explosives3D,
                radius: _explosiveRadius,
                color: Settings.ExplosivesSettings.ExplosiveColor.Value);
        }

        foreach (var e in _cachedEntities.Values)
        {
            switch (GetEntityType(e.Path))
            {
                case ExpeditionEntityType.Marker:
                {
                    var animatedMetaData = e.BaseAnimatedEntityMetadata;
                    if (animatedMetaData != null)
                    {
                        if (animatedMetaData.Contains("elitemarker"))
                        {
                            var mapSettings = Settings.IconMapping.GetValueOrDefault(IconPickerIndex.EliteMonstersIndicator, new IconDisplaySettings());
                            if (mapSettings.ShowOnMap)
                            {
                                DrawIconOnMap(e, mapSettings.Icon ?? ExpeditionIconsSettings.DefaultEliteMonsterIcon, mapSettings.Tint, Vector2.Zero);
                            }

                            if (mapSettings.ShowInWorld)
                            {
                                DrawIconInWorld(e, mapSettings.Icon ?? ExpeditionIconsSettings.DefaultEliteMonsterIcon, mapSettings.Tint, Vector2.Zero);
                            }
                        }
                        else
                        {
                            var iconDescription = _metadataIconMapping.GetOrAdd(animatedMetaData,
                                a =>
                                    Icons.LogbookChestIcons.FirstOrDefault(icon =>
                                    icon.BaseEntityMetadataSubstrings.Any(a.Contains)));
                            if (iconDescription != null)
                            {
                                var settings = Settings.IconMapping.GetValueOrDefault(iconDescription.IconPickerIndex, new IconDisplaySettings());
                                var icon = settings.Icon ?? iconDescription.DefaultIcon;
                                if (settings.ShowOnMap)
                                {
                                    DrawIconOnMap(e, icon, settings.Tint, Vector2.Zero);
                                }

                                if (settings.ShowInWorld)
                                {
                                    DrawIconInWorld(e, icon, settings.Tint, Vector2.Zero);
                                }
                            }
                        }
                    }

                    continue;
                }
                case ExpeditionEntityType.Relic:
                {
                    var mods = e.Mods;
                    if (e.Mods == null) continue;
                    if (e.MinimapIconHide != false) continue;
                    if (!mods.Any(x => x.Contains("ExpeditionRelicModifier"))) continue;

                    if (ContainsWarnMods(mods))
                    {
                        var mapSettings = Settings.IconMapping.GetValueOrDefault(IconPickerIndex.BadModsIndicator, new IconDisplaySettings());
                        if (mapSettings.ShowOnMap)
                        {
                            DrawIconOnMap(e, mapSettings.Icon ?? ExpeditionIconsSettings.DefaultBadModsIcon, mapSettings.Tint, Vector2.Zero);
                        }

                        if (mapSettings.ShowInWorld)
                        {
                            DrawIconInWorld(e, mapSettings.Icon ?? ExpeditionIconsSettings.DefaultBadModsIcon, mapSettings.Tint, -Vector2.UnitY);
                        }

                        continue;
                    }

                    if (Settings.DrawGoodModsInWorld || Settings.DrawGoodModsOnMap)
                    {
                        var worldIcons = new HashSet<(MapIconsIndex, Color?)>();
                        var mapIcons = new HashSet<(MapIconsIndex, Color?)>();
                        var iconDescriptions = mods.SelectMany(mod =>
                            _relicModIconMapping.GetOrAdd(mod, s =>
                                Icons.ExpeditionRelicIcons.Where(icon =>
                                    icon.BaseEntityMetadataSubstrings.Any(s.Contains)).ToList())).Distinct();
                        foreach (var iconDescription in iconDescriptions)
                        {
                            var settings = Settings.IconMapping.GetValueOrDefault(iconDescription.IconPickerIndex, new IconDisplaySettings());
                            var icon = settings.Icon ?? iconDescription.DefaultIcon;
                            if (settings.ShowOnMap)
                            {
                                mapIcons.Add((icon, settings.Tint));
                            }

                            if (settings.ShowInWorld)
                            {
                                worldIcons.Add((icon, settings.Tint));
                            }
                        }

                        var offset = new Vector2(-worldIcons.Count * 0.5f + 0.5f, 0);
                        foreach (var (icon, tint) in worldIcons)
                        {
                            if (Settings.DrawGoodModsInWorld)
                            {
                                DrawIconInWorld(e, icon, tint, offset);
                            }

                            offset += Vector2.UnitX;
                        }

                        offset = new Vector2(-mapIcons.Count * 0.5f + 0.5f, 0);
                        foreach (var (icon, tint) in mapIcons)
                        {
                            if (Settings.DrawGoodModsOnMap)
                            {
                                DrawIconOnMap(e, icon, tint, offset);
                            }

                            offset += Vector2.UnitX;
                        }
                    }

                    break;
                }
            }
        }

        if (_plannerRunner?.CurrentBestPath is { Count: > 0 } path)
        {
            var firstPoint = DetonatorPos ?? _playerGridPos;
            var prevPoint = firstPoint;
            for (var i = 0; i < path.Count; i++)
            {
                var point = path[i];
                if (_largeMapOpen)
                {
                    Graphics.DrawLine(GetMapScreenPosition(prevPoint), GetMapScreenPosition(point), 1, Settings.PlannerSettings.MapLineColor);
                }

                var worldPos = GetWorldScreenPosition(point);
                Graphics.DrawLine(GetWorldScreenPosition(prevPoint), worldPos, 1, Settings.PlannerSettings.WorldLineColor);
                var text = $"#{i}";
                Graphics.DrawBox(worldPos, worldPos + Graphics.MeasureText(text), Color.Black);
                Graphics.DrawText(text, worldPos, Color.White);
                prevPoint = point;
            }

            DrawCirclesInWorld(
                positions: path.Select(ExpandWithTerrainHeight).ToList(),
                radius: _explosiveRadius,
                color: Settings.PlannerSettings.ExplosiveColor.Value);
        }
    }

    private bool ContainsWarnMods(List<string> mods)
    {
        return Settings.WarnPhysImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmunePhysicalDamage")) ||
               Settings.WarnFireImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneFireDamage")) ||
               Settings.WarnColdImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneColdDamage")) ||
               Settings.WarnLightningImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneLightningDamage")) ||
               Settings.WarnChaosImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneChaosDamage")) ||
               Settings.WarnCritImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierCannotBeCrit")) ||
               Settings.WarnIgniteImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneStatusAilments")) ||
               Settings.WarnArmorPen.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierIgnoreArmour")) ||
               Settings.WarnNoEvade.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierHitsCannotBeEvaded")) ||
               Settings.WarnNoLeech.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierCannotBeLeechedFrom")) ||
               Settings.WarnNoFlask.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierGrantNoFlaskCharges")) ||
               Settings.WarnPetrify.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierElitesPetrifyOnHit")) ||
               Settings.WarnCurseImmune.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneToCurses")) ||
               Settings.WarnCull.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierCullingStrikeTwentyPercent")) ||
               Settings.WarnMonsterBlock.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierAttackBlockSpellBlockMaxBlockChance")) ||
               Settings.WarnMonsterResist.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierResistancesAndMaxResistances")) ||
               Settings.WarnMonsterRegen.Value && mods.Any(x => x.Contains("ExpeditionRelicModifierElitesRegenerateLifeEveryFourSeconds"));
    }

    private void DrawIconOnMap(EntityCacheItem entity, MapIconsIndex icon, Color? color, Vector2 offset)
    {
        if (_largeMapOpen)
        {
            var halfsize = Settings.MapIconSize / 2.0f;
            var point = GetEntityPosOnMapScreen(entity) + offset * halfsize * 2;
            var entityPos = entity.Pos;
            var entityPos2 = new Vector2(entityPos.X, entityPos.Y);

            DrawIcon(icon, color, point, entityPos2,
                Settings.ExplosivesSettings.HideCapturedEntitiesOnMap,
                Settings.ExplosivesSettings.MarkCapturedEntitiesOnMap,
                Settings.ExplosivesSettings.CapturedEntityMapFrameColor,
                Settings.PlannerSettings.CapturedEntityMapFrameColor,
                Settings.ExplosivesSettings.CapturedEntityMapFrameThickness,
                Settings.MapIconSize);
        }
    }

    private void DrawIconInWorld(EntityCacheItem entity, MapIconsIndex icon, Color? color, Vector2 offset)
    {
        var halfsize = Settings.WorldIconSize / 2.0f;
        var entityPos = entity.Pos;
        var entityPos2 = new Vector2(entityPos.X, entityPos.Y);
        var point = Camera.WorldToScreen(entityPos) + offset * halfsize * 2;
        DrawIcon(icon, color, point, entityPos2,
            Settings.ExplosivesSettings.HideCapturedEntitiesInWorld,
            Settings.ExplosivesSettings.MarkCapturedEntitiesInWorld,
            Settings.ExplosivesSettings.CapturedEntityWorldFrameColor,
            Settings.PlannerSettings.CapturedEntityWorldFrameColor,
            Settings.ExplosivesSettings.CapturedEntityWorldFrameThickness,
            Settings.WorldIconSize);
    }

    private void DrawIcon(
        MapIconsIndex icon,
        Color? color,
        Vector2 displayPosition,
        Vector2 worldPosition,
        bool hideCaptured,
        bool markCaptured,
        Color capturedFrameColor,
        Color plannerCapturedFrameColor,
        int frameThickness,
        float iconSize)
    {
        var halfsize = iconSize / 2.0f;
        var rect = new RectangleF(displayPosition.X, displayPosition.Y, 0, 0);
        rect.Inflate(halfsize, halfsize);
        var calculateExplosiveFrameDisplay = hideCaptured || markCaptured;
        var isInExplosiveRadius = calculateExplosiveFrameDisplay &&
                                  _explosives2DPositions.Any(x => Vector2.Distance(x, worldPosition) < _explosiveRadius);
        var gridPosition = worldPosition.WorldToGrid();
        var isInPlannedExplosiveRadius = calculateExplosiveFrameDisplay &&
                                         _plannerRunner?.CurrentBestPath is { Count: > 0 } path &&
                                         path.Any(x => Vector2.Distance(x, gridPosition) < _explosiveRadius / GridToWorldMultiplier);

        if (markCaptured)
        {
            var plannedRect = rect;
            if (isInExplosiveRadius)
            {
                Graphics.DrawFrame(rect, capturedFrameColor, frameThickness);
                plannedRect.Inflate(frameThickness, frameThickness);
            }

            if (isInPlannedExplosiveRadius)
            {
                Graphics.DrawFrame(plannedRect, plannerCapturedFrameColor, frameThickness);
            }
        }

        if (!isInExplosiveRadius || !hideCaptured)
        {
            Graphics.DrawImage(TextureName, rect, SpriteHelper.GetUV(icon), color ?? Color.White);
        }
    }

    private Vector2 GetMapScreenPosition(Vector2 gridPos)
    {
        return _mapCenter + TranslateGridDeltaToMapDelta(gridPos - _playerGridPos, GameController.IngameState.Data.GetTerrainHeightAt(gridPos) - _playerZ);
    }

    private Vector2 GetWorldScreenPosition(Vector2 gridPos)
    {
        return Camera.WorldToScreen(ExpandWithTerrainHeight(gridPos));
    }

    private Vector2 GetEntityPosOnMapScreen(EntityCacheItem entity)
    {
        var point = _mapCenter + TranslateGridDeltaToMapDelta(entity.GridPos - _playerGridPos, (entity.RenderZ ?? 0) - _playerZ);
        return point;
    }

    private Vector2 TranslateGridDeltaToMapDelta(Vector2 delta, float deltaZ)
    {
        deltaZ /= GridToWorldMultiplier; //z is normally "world" units, translate to grid
        return (float)_mapScale * new Vector2((delta.X - delta.Y) * CameraAngleCos, (deltaZ - (delta.X + delta.Y)) * CameraAngleSin);
    }

    private enum ExpeditionEntityType
    {
        None,
        Relic,
        Marker,
        Cave,
        Boss,
    }

    private record EntityCacheItem(string Path, Lazy<string> BaseAnimatedEntityMetadataCache, List<string> Mods, Vector3 Pos, Vector2 GridPos, float? RenderZ,
        float? RenderSize,
        bool? MinimapIconHide)
    {
        public string BaseAnimatedEntityMetadata => BaseAnimatedEntityMetadataCache.Value;

        public EntityCacheItem Merge(EntityCacheItem other)
        {
            return new EntityCacheItem(
                Path ?? other.Path,
                BaseAnimatedEntityMetadata == null ? other.BaseAnimatedEntityMetadataCache : BaseAnimatedEntityMetadataCache,
                Mods ?? other.Mods,
                Pos,
                GridPos,
                RenderZ ?? other.RenderZ,
                RenderSize ?? other.RenderSize,
                MinimapIconHide ?? MinimapIconHide);
        }
    }


    public override void EntityAdded(Entity entity)
    {
        if (entity.Type is EntityType.IngameIcon or EntityType.Terrain && GetEntityType(entity.Path) != ExpeditionEntityType.None)
        {
            _cachedEntities[entity.Id] = BuildCacheItem(entity);
        }
    }

    private static EntityCacheItem BuildCacheItem(Entity entity)
    {
        return new EntityCacheItem(
            entity.Path,
            new Lazy<string>(() => entity.GetComponent<Animated>()?.BaseAnimatedObjectEntity?.Metadata, LazyThreadSafetyMode.None),
            entity.GetComponent<ObjectMagicProperties>()?.Mods,
            entity.PosNum,
            entity.PosNum.WorldToGrid(),
            entity.GetComponent<Render>()?.Z,
            entity.GetComponent<Render>()?.BoundsNum is { } b ? Math.Min(b.X, b.Y) : null,
            entity.GetComponent<MinimapIcon>()?.IsHide);
    }
}
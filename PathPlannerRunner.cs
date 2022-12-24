using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ExileCore;

namespace ExpeditionIcons;

public class PathPlannerRunner
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    public bool IsRunning => _task is { IsCompleted: false };
    public (List<Vector2> Path, double Score)[] BestValues;
    public List<Vector2> CurrentBestPath => BestValues?.MaxBy(x => x.Score).Path;
    public double CurrentBestScore => BestValues?.Max(x => x.Score) ?? 0;

    private Task _task;

    //for debugging
    private int _generation;
    private double _lastGenerationTime;

    public void Start(PlannerSettings settings, ExpeditionEnvironment environment)
    {
        var threadCount = settings.SearchThreads.Value;
        BestValues = new (List<Vector2>, double)[threadCount];
        var tasks = new List<Task>();
        for (int i = 0; i < threadCount; i++)
        {
            var ii = i;
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    var p = new PathPlanner(settings);
                    var sw = Stopwatch.StartNew();
                    var iterationSw = Stopwatch.StartNew();
                    foreach (var bestPath in p.GetBestPathSeries(environment))
                    {
                        _lastGenerationTime = iterationSw.Elapsed.TotalMilliseconds;
                        iterationSw.Restart();
                        BestValues[ii] = (bestPath.Points, bestPath.Score);
                        _generation++;
                        if (sw.Elapsed.TotalSeconds >= settings.MaximumGenerationTimeSeconds.Value ||
                            _cts.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugWindow.LogError($"Expedition search thread failed: {ex}");
                }
            }));
        }

        _task = Task.WhenAll(tasks);
    }

    public void Stop() => _cts.Cancel();
}
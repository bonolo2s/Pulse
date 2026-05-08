using System.Diagnostics;

namespace Pulse.Lambda.Services;

public class LatencyTracker
{
    public async Task<long> MeasureAsync(string url, HttpClient httpClient)
    {
        var stopwatch = Stopwatch.StartNew();

        await httpClient.GetAsync(url);

        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }
}
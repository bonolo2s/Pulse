using System.Diagnostics;

namespace Pulse.Lambda.Services;

public class LatencyTracker
{
    public async Task<long> MeasureAsync(string url, HttpClient httpClient)
    {
        var stopwatch = Stopwatch.StartNew(); // dont i have latency in this Req on its own.

        await httpClient.GetAsync(url); // the must be somelatency here.

        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }
}
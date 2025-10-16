using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace AsyncReloaded;

public class Bench4TaskContinueWithBenchmarks
{
    private int _counter = 0;

    [Benchmark(Baseline = true)]
    public Task TaskCompletionSource()
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        tcs.SetResult();
        return tcs.Task;
    }
    
    [Benchmark]
    public Task ContinueWith_Instance()
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var result = tcs.Task.ContinueWith(t => Count());
        tcs.SetResult();
        return result;
    }
    
    [Benchmark]
    public Task ContinueWith_Static()
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var result = tcs.Task.ContinueWith(static (t, state) => ((Bench4TaskContinueWithBenchmarks)state!).Count(), this);
        tcs.SetResult();
        return result;
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Count() => _counter++;
}
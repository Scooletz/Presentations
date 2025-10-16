using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace AsyncReloaded;

public class Bench3CancellationTokenBenchmarks
{
    private readonly CancellationTokenSource _cts = new();
    private int _counter = 0;
    
    [Benchmark]
    public void Cancel_Instance()
    {
        // ReSharper disable ConvertClosureToMethodGroup
        var registration = _cts.Token.Register(() => this.Count());
        // ReSharper restore ConvertClosureToMethodGroup
        registration.Dispose();
    }
    
    [Benchmark]
    public void Cancel_MethodGroup()
    {
        var registration = _cts.Token.Register(Count);
        registration.Dispose();
    }
    
    [Benchmark]
    public void Cancel_StaticState()
    {
        var registration = _cts.Token.Register(static state=>((Bench3CancellationTokenBenchmarks)state!).Count(), this);
        registration.Dispose();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Count() => _counter++;
}
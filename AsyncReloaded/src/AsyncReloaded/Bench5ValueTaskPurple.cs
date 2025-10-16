using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace AsyncReloaded;

[Config(typeof(MyConfig))]
public class Bench5ValueTaskPurple
{
    private const int Diff = 1;
    private static readonly Task<int> Completed = Task.FromResult(Diff);

    [Benchmark]
    public async Task<int> FromTask_Regular()
    {
        return await TaskAsyncLike() + Diff;
    }

    [Benchmark]
    public async ValueTask<int> FromTask_ValueTask()
    {
        return await TaskAsyncLike() + Diff;
    }
    
    [Benchmark]
    public async ValueTask<int> FromValueTask_ValueTask()
    {
        return await ValueTaskAsyncLike() + Diff;
    }
    
    [Benchmark]
    public ValueTask<int> FromValueTask_Purple()
    {
        var vt = ValueTaskAsyncLike();
        if (vt.IsCompletedSuccessfully)
            return new ValueTask<int>(vt.Result + Diff);

        return SlowPathAsync(vt);

        static async ValueTask<int> SlowPathAsync(ValueTask<int> vt)
        {
            return (await vt) + Diff;
        }
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Task<int> TaskAsyncLike() => Completed;
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ValueTask<int> ValueTaskAsyncLike() => new(Diff);
}
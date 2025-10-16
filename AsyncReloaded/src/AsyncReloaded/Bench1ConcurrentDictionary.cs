using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace AsyncReloaded;

public class Bench1ConcurrentDictionary
{
    private readonly ConcurrentDictionary<int, int> _dict = new();
    private readonly int _seed = Random.Shared.Next();

    private const int Key = 13;
    
    [Benchmark]
    public int AddOrUpdate_Instance()
    {
        // ReSharper disable ConvertClosureToMethodGroup
        return _dict.AddOrUpdate(Key, 
            addValueFactory: key => Build(key),
            updateValueFactory: (key, prev) => Update(key, prev));
        // ReSharper restore ConvertClosureToMethodGroup
    }
    
    [Benchmark]
    public int AddOrUpdate_MethodGroup()
    {
        return _dict.AddOrUpdate(Key, 
            addValueFactory: Build, 
            updateValueFactory: Update);
    }

    [Benchmark]
    public int AddOrUpdate_StaticState()
    {
        return _dict.AddOrUpdate(Key, 
            addValueFactory: static (key,state) => state.Build(key), 
            updateValueFactory: static (key, prev, state) => state.Update(key, prev), this); 
    }
    
    private int Build(int key) => key ^ _seed;

    private int Update(int key, int prev) => key ^ prev ^ _seed;
}
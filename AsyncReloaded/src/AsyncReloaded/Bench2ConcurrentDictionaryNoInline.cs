using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace AsyncReloaded;

public class Bench2ConcurrentDictionaryNoInline
{
    private readonly ConcurrentDictionary<int, int> _dict = new();
    private readonly int _seed = Random.Shared.Next();

    private const int Key = 13;
    
    [Benchmark]
    public int AddOrUpdate_MethodGroup_No_Inline()
    {
        return AddOrUpdateMethodGroupNoInline(_dict, Build, Update);
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        static int AddOrUpdateMethodGroupNoInline(ConcurrentDictionary<int, int> dict, Func<int, int> addValueFactory, Func<int, int, int> updateValueFactory)
        {
            return dict.AddOrUpdate(Key, 
                addValueFactory: addValueFactory, 
                updateValueFactory: updateValueFactory);
        }
    }
    
    private int Build(int key) => key ^ _seed;

    private int Update(int key, int prev) => key ^ prev ^ _seed;
}
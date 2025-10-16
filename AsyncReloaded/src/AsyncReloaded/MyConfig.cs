using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace AsyncReloaded;

class MyConfig : ManualConfig
{
    public MyConfig()
    {
        AddJob(
            Job.ShortRun
                .WithRuntime(CoreRuntime.Core90)
                .WithId("9.0"));
        
        AddJob(
            Job.ShortRun
                .WithRuntime(CoreRuntime.Core10_0)
                .WithId("10.0"));
        
        AddExporter(DefaultConfig.Instance.GetExporters().ToArray());
        AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
        AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());

        AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()), new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig()));
    }
}

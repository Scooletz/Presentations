using AsyncReloaded;
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(Bench1ConcurrentDictionary).Assembly).Run(args);
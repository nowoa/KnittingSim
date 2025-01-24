using System;
using UnityEngine.Profiling;

public class ProfileSample : IDisposable
{
    public ProfileSample(string sampleName)
    {
        Profiler.BeginSample($"Knit/{sampleName}");
    }
    public void Dispose()
    {
        Profiler.EndSample();
    }
}

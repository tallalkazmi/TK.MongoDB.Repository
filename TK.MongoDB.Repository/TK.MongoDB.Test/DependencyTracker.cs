using System;
using TK.MongoDB.Interfaces;

namespace TK.MongoDB.Test
{
    public class DependencyTracker : IDependencyTracker
    {
        public void Dependency(string name, string description, bool success, TimeSpan duration)
        {
            Console.WriteLine($"{name}-{description}-{success}-{duration}");
        }
    }
}

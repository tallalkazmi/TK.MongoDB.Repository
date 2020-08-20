using System;

namespace TK.MongoDB.Interfaces
{
    /// <summary>
    /// Dependency Tracking interface
    /// </summary>
    public interface IDependencyTracker
    {
        /// <summary>
        /// Tracks a dependency
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="description">Description e.g. query</param>
        /// <param name="success">Result</param>
        /// <param name="duration">Duration</param>
        void Dependency(string name, string description, bool success, TimeSpan duration);
    }
}

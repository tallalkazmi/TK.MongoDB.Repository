using System.Threading.Tasks;
using TK.MongoDB.Data;
using TK.MongoDB.Interfaces;
using TK.MongoDB.Models;

namespace TK.MongoDB.Test
{
    public class OverrideRepository<T> : Repository<T> where T : BaseEntity
    {
        public OverrideRepository()
        {
        }

        public OverrideRepository(IDependencyTracker dependencyTracker) : base(dependencyTracker)
        {
        }

        public override Task<bool> UpdateAsync(T instance)
        {
            return base.UpdateAsync(instance);
        }
    }
}

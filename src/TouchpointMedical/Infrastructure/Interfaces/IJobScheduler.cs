using System;
using System.Threading.Tasks;

namespace TouchpointMedical.Infrastructure
{
    public interface IJobScheduler
    {
        Task ScheduleDebouncedProcessing(string entityId, TimeSpan delay);
    }
}

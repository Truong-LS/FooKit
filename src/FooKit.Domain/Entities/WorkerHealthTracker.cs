using System;

namespace FooKit.Domain.Entities
{
    public class WorkerHealthTracker
    {
        public bool IsWorkerRunning { get; set; }
        public DateTime? LastAffiliateSyncTime { get; set; }
    }
}

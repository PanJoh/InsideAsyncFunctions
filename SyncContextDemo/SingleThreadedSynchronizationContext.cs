using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SyncContextDemo
{
    class SingleThreadedSynchronizationContext : SynchronizationContext
    {
        private class WorkItem
        {
            public SendOrPostCallback Action { get; set; }
            public object State { get; set; }
        }

        BlockingCollection<WorkItem> workItems = new BlockingCollection<WorkItem>();

        public void Run()
        {
            while (workItems.TryTake(out var workItem))
            {
                workItem.Action(workItem.State);
            }
        }

        
        public override void Post(SendOrPostCallback cb, object state)
        {
            workItems.Add(new WorkItem {Action = cb, State = state});
        }

        public void Terminate()
        {
            workItems.CompleteAdding();
        }
    }
}

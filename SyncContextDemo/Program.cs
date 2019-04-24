using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SyncContextDemo;

namespace ThreadingDemo
{
    class Program
    {

        static async Task Run()
        {
            Dictionary<int, int> threadUsage = new Dictionary<int, int>();
            for (var counter = 0; counter < 2000; counter++)
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;
                if (threadUsage.ContainsKey(threadId))
                {
                    threadUsage[threadId]++;
                }
                else
                {
                    threadUsage[threadId] = 1;
                }

                await Task.Yield();
            }

            foreach (var threadId in threadUsage.Keys)
            {
                Console.WriteLine($"Thread: {threadId}, Usage: {threadUsage[threadId]}");
            }
        }

        static void Main(string[] args)
        {
            var syncContext = new SingleThreadedSynchronizationContext();
            var oldSyncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(syncContext);

            Run()
                .ContinueWith((action) => syncContext.Terminate(), TaskContinuationOptions.ExecuteSynchronously);

            syncContext.Run();

            SynchronizationContext.SetSynchronizationContext(oldSyncContext);
        }
    }
}

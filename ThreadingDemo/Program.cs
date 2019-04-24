using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingDemo
{
    public class Program
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
            Run().Wait();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncStateMachine
{
    class RunStateMachine : IAsyncStateMachine
    {

        public const int Cont0 = 0;
        public const int Cont1 = 1;
        public const int Cont2 = 2;
        public const int Cont3 = 3;

        public int state;
        public AsyncTaskMethodBuilder methodBuilder;

        private Dictionary<int, int> threadUsage;
        private int counter;
        private int threadId;
        private YieldAwaitable.YieldAwaiter yieldAwaiter;

        public void MoveNext()
        {
            switch (state)
            {
                case Cont0:
                    threadUsage = new Dictionary<int, int>();
                    counter = 0;
                    state = Cont1;
                    goto case Cont1;

// Head of for loop
                case Cont1:
                    if (counter >= 2000)
                    {
                        state = Cont3;
                        goto case Cont3;
                    }

// Loop Body 
                    threadId = Thread.CurrentThread.ManagedThreadId;
                    if (threadUsage.ContainsKey(threadId))
                    {
                        threadUsage[threadId]++;
                    }
                    else
                    {
                        threadUsage[threadId] = 1;
                    }

                    state = Cont2;
                    var yieldAwaitable = Task.Yield();
                    yieldAwaiter = yieldAwaitable.GetAwaiter();
                    RunStateMachine stm = this;
                    methodBuilder.AwaitUnsafeOnCompleted(ref yieldAwaiter,  ref stm);
                    break;

// Continuation after await and tail of loop Body
                case Cont2:
                    counter++;
                    state = Cont1;
                    goto case 1;

// Code after the for loop
                case Cont3:
                    foreach (var threadId in threadUsage.Keys)
                    {
                        Console.WriteLine($"Thread: {threadId}, Usage: {threadUsage[threadId]}");
                    }

                    state = -2;
                    methodBuilder.SetResult();
                    break;
            }
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {

        static Task Run()
        {
            var stm = new RunStateMachine {methodBuilder = AsyncTaskMethodBuilder.Create(), state = 0};
            var methodBuilder = stm.methodBuilder;
            methodBuilder.Start(ref stm);
            return stm.methodBuilder.Task;
        }

        static void Main(string[] args)
        {
            Run().Wait();
        }
    }
}

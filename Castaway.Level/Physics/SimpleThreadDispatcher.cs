using System;
using System.Diagnostics;
using System.Threading;
using BepuUtilities;
using BepuUtilities.Memory;

// TODO Possibly replace?
// Copied from https://github.com/bepu/bepuphysics2/blob/master/Demos/SimpleThreadDispatcher.cs
namespace Castaway.Level.Physics
{
    public class SimpleThreadDispatcher : IThreadDispatcher, IDisposable
    {
        BufferPool[] bufferPools;
        int completedWorkerCounter;

        volatile bool disposed;
        AutoResetEvent finished;
        int threadCount;

        volatile Action<int> workerBody;
        int workerIndex;

        Worker[] workers;

        public SimpleThreadDispatcher(int threadCount)
        {
            this.threadCount = threadCount;
            workers = new Worker[threadCount - 1];
            for (int i = 0; i < workers.Length; ++i)
            {
                workers[i] = new Worker {Thread = new Thread(WorkerLoop), Signal = new AutoResetEvent(false)};
                workers[i].Thread.IsBackground = true;
                workers[i].Thread.Start(workers[i].Signal);
            }

            finished = new AutoResetEvent(false);
            bufferPools = new BufferPool[threadCount];
            for (int i = 0; i < bufferPools.Length; ++i)
            {
                bufferPools[i] = new BufferPool();
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                SignalThreads();
                for (int i = 0; i < bufferPools.Length; ++i)
                {
                    bufferPools[i].Clear();
                }

                foreach (var worker in workers)
                {
                    worker.Thread.Join();
                    worker.Signal.Dispose();
                }
            }
        }

        public int ThreadCount => threadCount;

        public void DispatchWorkers(Action<int> workerBody)
        {
            Debug.Assert(this.workerBody == null);
            workerIndex =
                1; //Just make the inline thread worker 0. While the other threads might start executing first, the user should never rely on the dispatch order.
            completedWorkerCounter = 0;
            this.workerBody = workerBody;
            SignalThreads();
            //Calling thread does work. No reason to spin up another worker and block this one!
            DispatchThread(0);
            finished.WaitOne();
            this.workerBody = null;
        }

        public BufferPool GetThreadMemoryPool(int workerIndex)
        {
            return bufferPools[workerIndex];
        }

        void DispatchThread(int workerIndex)
        {
            Debug.Assert(workerBody != null);
            workerBody(workerIndex);

            if (Interlocked.Increment(ref completedWorkerCounter) == threadCount)
            {
                finished.Set();
            }
        }

        void WorkerLoop(object untypedSignal)
        {
            var signal = (AutoResetEvent) untypedSignal;
            while (true)
            {
                signal.WaitOne();
                if (disposed)
                    return;
                DispatchThread(Interlocked.Increment(ref workerIndex) - 1);
            }
        }

        void SignalThreads()
        {
            for (int i = 0; i < workers.Length; ++i)
            {
                workers[i].Signal.Set();
            }
        }

        struct Worker
        {
            public Thread Thread;
            public AutoResetEvent Signal;
        }
    }
}
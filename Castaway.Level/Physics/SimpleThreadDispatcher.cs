using System;
using System.Diagnostics;
using System.Threading;
using BepuUtilities;
using BepuUtilities.Memory;

// TODO Possibly replace?
// Copied from https://github.com/bepu/bepuphysics2/blob/master/Demos/SimpleThreadDispatcher.cs
namespace Castaway.Level.Physics;

public class SimpleThreadDispatcher : IThreadDispatcher, IDisposable
{
	private readonly BufferPool[] _bufferPools;
	private readonly AutoResetEvent _finished;

	private readonly Worker[] _workers;
	private int _completedWorkerCounter;

	private volatile bool _disposed;

	private volatile Action<int>? _workerBody;
	private int _workerIndex;

	public SimpleThreadDispatcher(int threadCount)
	{
		ThreadCount = threadCount;
		_workers = new Worker[threadCount - 1];
		for (var i = 0; i < _workers.Length; ++i)
		{
			_workers[i] = new Worker { Thread = new Thread(WorkerLoop), Signal = new AutoResetEvent(false) };
			_workers[i].Thread.IsBackground = true;
			_workers[i].Thread.Start(_workers[i].Signal);
		}

		_finished = new AutoResetEvent(false);
		_bufferPools = new BufferPool[threadCount];
		for (var i = 0; i < _bufferPools.Length; ++i) _bufferPools[i] = new BufferPool();
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			SignalThreads();
			for (var i = 0; i < _bufferPools.Length; ++i) _bufferPools[i].Clear();

			foreach (var worker in _workers)
			{
				worker.Thread.Join();
				worker.Signal.Dispose();
			}
		}
	}

	public int ThreadCount { get; }

	public void DispatchWorkers(Action<int> workerBody)
	{
		Debug.Assert(_workerBody == null);
		_workerIndex =
			1; //Just make the inline thread worker 0. While the other threads might start executing first, the user should never rely on the dispatch order.
		_completedWorkerCounter = 0;
		this._workerBody = workerBody;
		SignalThreads();
		//Calling thread does work. No reason to spin up another worker and block this one!
		DispatchThread(0);
		_finished.WaitOne();
		this._workerBody = null;
	}

	public BufferPool GetThreadMemoryPool(int workerIndex)
	{
		return _bufferPools[workerIndex];
	}

	private void DispatchThread(int workerIndex)
	{
		Debug.Assert(_workerBody != null);
		_workerBody(workerIndex);

		if (Interlocked.Increment(ref _completedWorkerCounter) == ThreadCount) _finished.Set();
	}

	private void WorkerLoop(object untypedSignal)
	{
		var signal = (AutoResetEvent)untypedSignal;
		while (true)
		{
			signal.WaitOne();
			if (_disposed)
				return;
			DispatchThread(Interlocked.Increment(ref _workerIndex) - 1);
		}
	}

	private void SignalThreads()
	{
		for (var i = 0; i < _workers.Length; ++i) _workers[i].Signal.Set();
	}

	private struct Worker
	{
		public Thread Thread;
		public AutoResetEvent Signal;
	}
}
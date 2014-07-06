using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using C5;

namespace CronScheduling
{
	/// <summary>
	/// Thread-safe priority queue based on interval heap.
	/// </summary>
	/// <typeparam name="T">Element type.</typeparam>
	internal sealed class PriorityQueue<T> : IProducerConsumerCollection<T>
	{
		private readonly IntervalHeap<T> _heap;
		private readonly object _lock = new object();

		public PriorityQueue(IComparer<T> comparer)
		{
			_heap = new IntervalHeap<T>(comparer);
		}

		public PriorityQueue() : this(Comparer<T>.Default)
		{
		}

		public void Add(T item)
		{
			lock (_lock)
			{
				_heap.Add(item);
			}
		}

		public void Enqueue(T item)
		{
			lock (_lock)
			{
				_heap.Add(item);
			}
		}

		public bool TryDequeue(out T result)
		{
			lock (_lock)
			{
				if (_heap.Count > 0)
				{
					result = _heap.DeleteMin();
					return true;
				}
			}
			result = default(T);
			return false;
		}

		public bool TryPeek(out T result)
		{
			lock (_lock)
			{
				if (_heap.Count > 0)
				{
					result = _heap.FindMin();
					return true;
				}
			}
			result = default(T);
			return false;
		}

		public IEnumerator<T> GetEnumerator()
		{
			var array = ToArray();
			return ((IEnumerable<T>) array).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			CopyTo((T[]) array, index);
		}

		public int Count
		{
			get
			{
				lock (_lock)
				{
					return _heap.Count;
				}
			}
		}

		public object SyncRoot { get { return _lock; } }

		public bool IsSynchronized { get { return true; } }

		public void CopyTo(T[] array, int index)
		{
			lock (_lock)
			{
				_heap.CopyTo(array, index);
			}
		}

		public bool TryAdd(T item)
		{
			Enqueue(item);
			return true;
		}

		public bool TryTake(out T item)
		{
			return TryDequeue(out item);
		}

		public T[] ToArray()
		{
			lock (_lock)
			{
				return _heap.ToArray();
			}
		}
	}
}

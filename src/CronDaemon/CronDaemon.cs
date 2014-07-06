using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NCrontab;

namespace CronScheduling
{
	/// <summary>
	/// Cron scheduling daemon.
	/// </summary>
	public sealed class CronDaemon<T> : IDisposable
	{
		// TODO consider to be parameter for cron daemon
		private static readonly TimeSpan ThrottlingInterval = TimeSpan.FromMilliseconds(10);

		private struct CronItem
		{
			public readonly T Job;
			public readonly Func<DateTime, DateTime?> Schedule;
			public readonly DateTime NextOccurrence;

			public CronItem(T job, Func<DateTime, DateTime?> schedule, DateTime nextOccurrence)
			{
				Job = job;
				Schedule = schedule;
				NextOccurrence = nextOccurrence;
			}
		}

		private readonly PriorityQueue<CronItem> _queue;
		private readonly Action<T> _execute;
		private readonly Func<T, T> _fork;
		private readonly object _lock = new object();
		private Timer _timer;

		/// <summary>
		/// Initializes new instance of <see cref="CronDaemon{T}"/>.
		/// </summary>
		/// <param name="execute">The job handler.</param>
		/// <param name="fork">The function to fork job instance on every recurrence.</param>
		public CronDaemon(Action<T> execute, Func<T,T> fork)
		{
			_execute = execute;
			_fork = fork;
			_queue = new PriorityQueue<CronItem>(new CronComparer());
			_timer = new Timer(TimerCallback);
		}

		public void Dispose()
		{
			_timer.Dispose();
			_timer = null;
		}

		/// <summary>
		/// Adds specified job to <see cref="CronDaemon{T}"/> queue with given cron expression and maximum number of repetitions.
		/// </summary>
		/// <param name="job">The job definition.</param>
		/// <param name="cronExpression">Specifies cron expression.</param>
		/// <param name="repeatCount">Specifies maximum number of job recurrence.</param>
		public void Add(T job, string cronExpression, int repeatCount)
		{
			var now = SystemTime.Now;
			var crontab = CrontabSchedule.Parse(cronExpression);

			var syncLock = new object();
			Func<DateTime, DateTime?> schedule = time =>
			{
				// TODO cancel cron scheduling
				lock (syncLock)
				{
					repeatCount--;
					// 0 means once like in java quartz
					if (repeatCount >= 0)
					{
						return crontab.GetNextOccurrence(time);
					}
					return null;
				}
			};

			var nextOccurrence = crontab.GetNextOccurrence(now);

			Push(new CronItem(job, schedule, nextOccurrence));
		}

		private void Push(CronItem item)
		{
			Task.Delay(ThrottlingInterval)
				.ContinueWith(task =>
				{
					_queue.Add(item);
					TimerCallback(null);
				});
		}

		private void TimerCallback(object state)
		{
			CronItem item;
			if (!Take(out item)) return;

			lock (_lock)
			{
				var nextOccurrence = item.Schedule(item.NextOccurrence);
				if (nextOccurrence != null)
				{
					// TODO compute job data expressions
					var job = _fork(item.Job);
					Push(new CronItem(job, item.Schedule, nextOccurrence.Value));
				}
			}

			_execute(item.Job);
		}

		private bool Take(out CronItem result)
		{
			result = default(CronItem);

			lock (_lock)
			{
				if (_timer == null)
					return false;

				CronItem item;
				if (!_queue.TryTake(out item))
					return false;

				var now = SystemTime.Now;
				var waitSpan = item.NextOccurrence - now;
				var dueTime = (long) waitSpan.TotalMilliseconds;
				if (dueTime > 0)
				{
					_queue.Add(item);
					_timer.Change(dueTime, Timeout.Infinite);
					return false;
				}

				// stop timer
				_timer.Change(Timeout.Infinite, Timeout.Infinite);

				result = item;
			}

			return true;
		}

		private class CronComparer : IComparer<CronItem>
		{
			public int Compare(CronItem x, CronItem y)
			{
				return x.NextOccurrence.CompareTo(y.NextOccurrence);
			}
		}
	}
}

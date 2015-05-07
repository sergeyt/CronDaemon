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
		private readonly Action<T> _execute;
		private readonly Func<T, T> _fork;
		private readonly List<CancellationTokenSource> _cancellations = new List<CancellationTokenSource>();

		/// <summary>
		/// Initializes new instance of <see cref="CronDaemon{T}"/>.
		/// </summary>
		/// <param name="execute">The job handler.</param>
		/// <param name="fork">The function to fork job instance on every recurrence.</param>
		public CronDaemon(Action<T> execute, Func<T,T> fork = null)
		{
			if (execute == null) throw new ArgumentNullException("execute");

			_execute = execute;
			_fork = fork ?? DefaultFork;
		}

		private static T DefaultFork(T item)
		{
			var cloneable = item as ICloneable;
			if (cloneable != null) return (T) cloneable.Clone();
			return item;
		}

		public void Dispose()
		{
			foreach (var cancellation in _cancellations)
			{
				cancellation.Cancel(false);
			}
			_cancellations.Clear();
		}

		/// <summary>
		/// Adds specified job to <see cref="CronDaemon{T}"/> queue with given cron expression and maximum number of repetitions.
		/// </summary>
		/// <param name="job">The job definition.</param>
		/// <param name="cronExpression">Specifies cron expression.</param>
		/// <param name="repeatCount">Specifies maximum number of job recurrence.</param>
		public void Add(T job, string cronExpression, int repeatCount)
		{
			if (repeatCount < 0) throw new ArgumentOutOfRangeException("repeatCount");
			if (repeatCount == 0) repeatCount = 1;

			var crontab = CrontabSchedule.Parse(cronExpression);

			var cancellation = new CancellationTokenSource();

			Func<DateTime, DateTime?> schedule = time =>
			{
				if (cancellation.IsCancellationRequested) return null;

				repeatCount--;

				// 0 means once like in java quartz
				if (repeatCount >= 0)
				{
					return crontab.GetNextOccurrence(time);
				}

				return null;
			};

			Action run = async () =>
			{
				while (true)
				{
					var now = SystemTime.Now;
					var nextOccurrence = schedule(now);
					if (nextOccurrence == null) break;

					var delay = nextOccurrence.Value - now;
					await Task.Delay(delay, cancellation.Token);

					if (cancellation.IsCancellationRequested) break;

					_execute(_fork(job));	
				}
			};

			Task.Run(run, cancellation.Token);

			_cancellations.Add(cancellation);
		}
	}

	public static class CronDaemon
	{
		public static CronDaemon<T> Start<T>(Action<T> execute, Func<T, T> fork = null)
		{
			if (execute == null) throw new ArgumentNullException("execute");

			return new CronDaemon<T>(execute, fork);
		}
	}
}

using System;

namespace CronScheduling
{
	// http://ayende.com/Blog/archive/2008/07/07/Dealing-with-time-in-tests.aspx

	/// <summary>
	/// For testing.
	/// </summary>
	internal static class SystemTime
	{
		public static Func<DateTime> NowFn;

		public static DateTime Now
		{
			get { return NowFn == null ? DateTime.UtcNow : NowFn(); }
		}

		internal static IDisposable WithOneMinuteTick()
		{
			var now = DateTime.UtcNow;
			now = now.Subtract(TimeSpan.FromSeconds(now.Second).Add(TimeSpan.FromMilliseconds(now.Millisecond)));

			NowFn = () =>
			{
				var t = now;
				now = now.Add(TimeSpan.FromMinutes(1));
				return t;
			};

			return new DisposableAction(() => NowFn = null);
		}
	}
}

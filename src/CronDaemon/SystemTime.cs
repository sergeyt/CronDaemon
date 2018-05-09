using System;

namespace CronScheduling
{
	// http://ayende.com/Blog/archive/2008/07/07/Dealing-with-time-in-tests.aspx

	/// <summary>
	/// For testing.
	/// </summary>
	internal static class SystemTime
	{
		public static Func<DateTime> NowFn = () => DateTime.UtcNow;

		public static DateTime Now
		{
			get { return NowFn(); }
		}
	}
}

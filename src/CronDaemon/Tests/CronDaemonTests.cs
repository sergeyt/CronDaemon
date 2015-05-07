#if NUNIT

using System;
using System.Threading;

using NUnit.Framework;

namespace CronScheduling.Tests
{
	[TestFixture]
	public class CronDaemonTests
	{
		[Test]
		public void CtorFails()
		{
			Assert.That(() => new CronDaemon<int>(null), Throws.InstanceOf<ArgumentNullException>());
		}

		public static IDisposable WithOneMinuteClock()
		{
			var now = DateTime.UtcNow;
			now = now.Subtract(TimeSpan.FromSeconds(now.Second)).Subtract(TimeSpan.FromSeconds(1));
			var timeIndex = 0;

			SystemTime.NowFn = () =>
			{
				if (timeIndex == 0)
				{
					++timeIndex;
					return now;
				}

				var t = now;
				now = now.Add(TimeSpan.FromMinutes(1));
				return t;
			};

			return new DisposableAction(() => SystemTime.NowFn = null);
		}

		[Test]
		public void RunForever()
		{
			using (WithOneMinuteClock())
			{
				var count = 0;
				var done = new ManualResetEvent(false);
				var crond = CronDaemon.Start<int>(
					i =>
					{
						count++;
						if (count >= 3)
						{
							done.Set();
						}
					},
					i => i
					);

				using (crond)
				{
					crond.Add(1, Cron.Minutely(), int.MaxValue);

					Assert.IsTrue(done.WaitOne(TimeSpan.FromSeconds(3)));
				}
			}
		}

		[TestCase(0, Result = 1)]
		[TestCase(1, Result = 1)]
		public int RunTimes(int times)
		{
			using (WithOneMinuteClock())
			{
				var count = 0;
				var done = new ManualResetEvent(false);
				var crond = CronDaemon.Start<int>(
					i =>
					{
						count++;
					},
					i => i
					);

				using (crond)
				{
					crond.Add(1, Cron.Minutely(), times);

					done.WaitOne(TimeSpan.FromSeconds(3));
				}

				return count;
			}
		}
	}
}
#endif

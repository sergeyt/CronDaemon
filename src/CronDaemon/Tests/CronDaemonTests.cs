#if NUNIT
using System;
using NUnit.Framework;

namespace CronScheduling.Tests
{
	[TestFixture]
	public class CronDaemonTests
	{
		[Test]
		public void CtorFails()
		{
			Assert.That(() => new CronDaemon<int>(null, null), Throws.InstanceOf<ArgumentNullException>());
			Assert.That(() => new CronDaemon<int>(_ => { }, null), Throws.InstanceOf<ArgumentNullException>());
		}
	}
}
#endif

#if NUNIT

using System;

namespace CronScheduling.Tests
{
	internal sealed class DisposableAction : IDisposable
	{
		private readonly Action _action;

		public DisposableAction(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			_action();
		}
	}
}

#endif

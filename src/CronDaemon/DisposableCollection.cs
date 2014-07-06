using System;
using System.Collections.Generic;
using System.Linq;

namespace CronScheduling
{
	internal sealed class DisposableCollection : IDisposable
	{
		private readonly List<IDisposable> _list;

		public DisposableCollection(IEnumerable<IDisposable> disposables)
		{
			_list = disposables.ToList();
		}
		
		public static DisposableCollection FromDisposables(params IDisposable[] disposables)
		{
			return new DisposableCollection(disposables);
		}

		public static DisposableCollection FromActions(params Action[] actions)
		{
			return new DisposableCollection(from a in actions select new DisposableAction(a));
		}

		public void Dispose()
		{
			_list.ForEach(d => d.Dispose());
			_list.Clear();
		}
	}

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

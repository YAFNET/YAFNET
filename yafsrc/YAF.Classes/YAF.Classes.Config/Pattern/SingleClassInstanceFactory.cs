using System;
using System.Collections.Generic;
using System.Text;

namespace YAF.Classes.Pattern
{
	public class SingleClassInstanceFactory
	{
		private readonly Dictionary<int, object> _contextClasses = new Dictionary<int, object>();

		public T GetInstance<T>() where T : class
		{
			int objNameHash = typeof(T).ToString().GetHashCode();

			if (!_contextClasses.ContainsKey(objNameHash))
			{
				_contextClasses[objNameHash] = (T)Activator.CreateInstance(typeof(T), true);
			}
			return (T)_contextClasses[objNameHash];
		}

		public void SetInstance<T>( T instance ) where T : class
		{
			int objNameHash = typeof(T).ToString().GetHashCode();
			_contextClasses[objNameHash] = instance;
		}
	}
}

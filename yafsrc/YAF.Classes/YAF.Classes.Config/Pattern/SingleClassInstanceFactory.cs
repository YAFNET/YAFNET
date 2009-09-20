/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
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
			int objNameHash = typeof( T ).ToString().GetHashCode();

			if ( !_contextClasses.ContainsKey( objNameHash ) )
			{
				_contextClasses[objNameHash] = (T)Activator.CreateInstance( typeof( T ), true );
			}
			return (T)_contextClasses[objNameHash];
		}

		public void SetInstance<T>( T instance ) where T : class
		{
			int objNameHash = typeof( T ).ToString().GetHashCode();
			_contextClasses[objNameHash] = instance;
		}
	}
}

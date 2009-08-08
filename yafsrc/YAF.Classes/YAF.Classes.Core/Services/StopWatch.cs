/* Yet Another Forum.net
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

namespace YAF.Classes.Core
{
	public class YafStopWatch
	{
		private readonly System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();

		public System.Diagnostics.Stopwatch Watch
		{
			get
			{
				return _stopWatch;
			}
		}

		public double Duration
		{
			get
			{
				double duration = (double) _stopWatch.ElapsedMilliseconds/1000.0;
				return duration;
			}
		}

		public void Start()
		{
			_stopWatch.Start();
		}

		public void Stop()
		{
			_stopWatch.Stop();
		}

		public void Reset()
		{
			_stopWatch.Reset();
		}
	}
}

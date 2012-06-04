/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.Utils.Extensions
{
	using System;

	using YAF.Types;

	/// <summary>
	/// The event extensions.
	/// </summary>
	public static class EventExtensions
	{
		#region Public Methods

		/// <summary>
		/// Only invokes an event handler if it's non-null.
		/// </summary>
		/// <param name="eventHandler">
		/// The event handler.
		/// </param>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		/// <typeparam name="TEventArgs">
		/// </typeparam>
		public static void SafeInvoke<TEventArgs>(
			[CanBeNull] this EventHandler<TEventArgs> eventHandler, [NotNull] object sender, [CanBeNull] TEventArgs args)
			where TEventArgs : EventArgs
		{
			if (eventHandler != null)
			{
				eventHandler(sender, args);
			}
		}

		#endregion
	}
}
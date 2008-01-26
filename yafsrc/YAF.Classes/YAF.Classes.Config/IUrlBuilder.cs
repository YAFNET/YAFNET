/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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

namespace YAF.Classes
{
	/// <summary>
	/// Defines interface for UrlBuilder class.
	/// </summary>
	public interface IUrlBuilder
	{
		/// <summary>
		/// Builds URL for calling page with URL argument as and parameter.
		/// </summary>
		/// <param name="url">URL to use as a parameter.</param>
		/// <returns>URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.</returns>
		string BuildUrl(string url);
	}
}

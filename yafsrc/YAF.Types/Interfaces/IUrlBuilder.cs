/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Types.Interfaces
{
  /// <summary>
  /// Defines interface for <see cref="IUrlBuilder"/> class.
  /// </summary>
  public interface IUrlBuilder
  {
    /// <summary>
    /// Builds path for calling page with URL argument as the parameter.
    /// </summary>
    /// <param name="url">
    /// URL to use as a parameter.
    /// </param>
    /// <returns>
    /// URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.
    /// </returns>
    string BuildUrl(string url);

    /// <summary>
    /// Builds a "Full URL" (server + path) for calling page with URL argument as parameter.
    /// </summary>
    /// <param name="url">
    /// URL to use as a parameter.
    /// </param>
    /// <returns>
    /// URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.
    /// </returns>
    string BuildUrlFull(string url);
  }
}
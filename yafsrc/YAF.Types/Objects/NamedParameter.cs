/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Types
{
  using YAF.Types.Interfaces;

  /// <summary>
  /// The named parameter.
  /// </summary>
  public class NamedParameter : IServiceLocationParameter
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedParameter"/> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public NamedParameter([NotNull] string name, [NotNull] object value)
    {
      this.Name = name;
      this.Value = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets Value.
    /// </summary>
    public object Value { get; set; }

    #endregion
  }
}
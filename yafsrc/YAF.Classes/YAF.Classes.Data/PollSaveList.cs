/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes.Data
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// Class to hold polls and questions data to save
  /// </summary>
  public class PollSaveList
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PollSaveList"/> class.
    /// </summary>
    public PollSaveList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PollSaveList"/> class.
    /// </summary>
    /// <param name="pollQuestion">
    /// The poll question.
    /// </param>
    /// <param name="pollChoices">
    /// The poll choices.
    /// </param>
    /// <param name="pollCloses">
    /// The poll closes.
    /// </param>
    public PollSaveList(string pollQuestion, string[] pollChoices, DateTime? pollCloses)
    {
      this.Question = pollQuestion;
      this.Choice = pollChoices;
      this.Closes = pollCloses;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or Sets value for Question text
    /// </summary>
    public string[] Choice { get; set; }

    /// <summary>
    ///   Gets or Sets value indicatiing when a poll (question) closes
    /// </summary>
    public DateTime? Closes { get; set; }

    /// <summary>
    ///   Gets or Sets value for Question text
    /// </summary>
    public string Question { get; set; }

    #endregion
  }
}
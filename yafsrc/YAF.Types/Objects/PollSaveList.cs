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
namespace YAF.Types.Objects
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// Class to hold polls and questions data to save
  /// </summary>
  [Serializable]
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
    public PollSaveList(string pollQuestion, string[,] pollChoices, DateTime? pollCloses, 
        int userId, int? topicId, int? forumId, int? categoryId, int? boardId,
        string questionObjectPath, string questionMimeType, bool isBound, bool isClosedBound, bool allowMultipleChoices, bool showVoters, bool allowSkipVote)
    {
        this.Question = pollQuestion;
        this.Choice = pollChoices;
        this.Closes = pollCloses;
        this.UserId = userId;
        this.TopicId = topicId;
        this.ForumId = forumId;
        this.CategoryId = categoryId;
        this.BoardId = boardId;
        this.QuestionObjectPath = questionObjectPath;
        this.QuestionMimeType = questionMimeType;
        this.IsBound = isBound;
        this.IsClosedBound = isClosedBound;
        this.AllowSkipVote = allowSkipVote;
        this.AllowMultipleChoices = allowMultipleChoices;
        this.ShowVoters = showVoters;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or Sets value for Question text
    /// </summary>
    public string[,] Choice { get; set; }

    /// <summary>
    ///   Gets or Sets value indicatiing when a poll (question) closes
    /// </summary>
    public DateTime? Closes { get; set; }

    /// <summary>
    ///   Gets or Sets value for Question text
    /// </summary>
    public string Question { get; set; }

    /// <summary>
    ///   Gets or Sets value for UserId 
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    ///   Gets or Sets value for TopicId 
    /// </summary>
    public int? TopicId { get; set; }

    /// <summary>
    ///   Gets or Sets value for ForumId 
    /// </summary>
    public int? ForumId { get; set; }

    /// <summary>
    ///   Gets or Sets value for CategoryId 
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    ///   Gets or Sets value for BoardId 
    /// </summary>
    public int? BoardId { get; set; }

    /// <summary>
    ///   Gets or Sets value for ObjectPath 
    /// </summary>
    public string QuestionObjectPath { get; set; }

    /// <summary>
    ///   Gets or Sets value for MimeType 
    /// </summary>
    public string QuestionMimeType { get; set; }

    /// <summary>
    ///   Gets or Sets value for IsBound. The polls in a group are bounded by voting. 
    /// </summary>
    public bool IsBound { get; set; }

    /// <summary>
    ///   Gets or Sets value for IsClosedBound. Users can't see results if the poll is not expired. 
    /// </summary>
    public bool IsClosedBound { get; set; }

    /// <summary>
    ///   Gets or Sets value for AllowMultipleChoices. Users can vote for many choices. 
    /// </summary>
    public bool AllowMultipleChoices { get; set; }

    /// <summary>
    ///   Gets or Sets value for ShowVoters. Users can see who voted. 
    /// </summary>
    public bool ShowVoters { get; set; }

    /// <summary>
    ///   Gets or Sets value for AllowSkipVote. Users can be allowed to skip voting. 
    /// </summary>
    public bool AllowSkipVote { get; set; }

    #endregion
  }
}
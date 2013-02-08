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

namespace YAF.DotNetNuke
{
    #region Using

    using System;
    using System.Linq;

    using global::DotNetNuke.Common.Utilities;

    using global::DotNetNuke.Entities.Modules;

    using global::DotNetNuke.Services.Search;

    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// YAF DNN Controller
    /// </summary>
    public class YafDnnController : ModuleSettingsBase, ISearchable
    {
        #region Constants and Fields

        /// <summary>
        /// The current Board id.
        /// </summary>
        private int boardId = 1;

        #endregion

        #region Implemented Interfaces

        #region ISearchable

        /// <summary>
        /// Included as a stub only so that the core knows this module Implements Entities.Modules.ISearchable
        /// </summary>
        /// <param name="modInfo">The mod info.</param>
        /// <returns>
        /// Search Items
        /// </returns>
        public SearchItemInfoCollection GetSearchItems(ModuleInfo modInfo)
        {
            // Get Used Board Id for the Module Instance
            try
            {
                if (this.Settings["forumboardid"].ToString().IsSet())
                {
                    this.boardId = this.Settings["forumboardid"].ToType<int>();
                }
            }
            catch (Exception)
            {
                this.boardId = 1;
            }

            var searchItemCollection = new SearchItemInfoCollection();

            // Get all Messages
            var yafMessages = Controller.Data.YafDnnGetMessages();

            // Get all Topics
            var yafTopics = Controller.Data.YafDnnGetTopics();

            // Get the forum name
            var forumName = modInfo.ModuleTitle.IsSet() ? modInfo.ModuleTitle : "YAF-Forum";

            foreach (Messages message in yafMessages)
            {
                // find the Topic of the message
                var curMessage = message;

                var curTopic =
                    yafTopics.Find(
                        topics => topics.TopicId.Equals(curMessage.TopicId) && topics.ForumId.Equals(this.boardId));

                if (curTopic == null)
                {
                    continue;
                }

                // Format message
                string sMessage = message.Message;

                if (sMessage.Contains(" "))
                {
                    string[] messageWords = sMessage.Split(' ');

                    var message1 = message;

                    foreach (var searchItem in from word in messageWords
                                               where !string.IsNullOrEmpty(word)
                                               select
                                                   new SearchItemInfo(
                                                   "{0} - {1}".FormatWith(forumName, curTopic.TopicName),
                                                   message1.Message,
                                                   Null.NullInteger,
                                                   message1.Posted,
                                                   modInfo.ModuleID,
                                                   "m{0}".FormatWith(message1.MessageId),
                                                   word,
                                                   "&g=posts&t={0}&m={1}#post{1}".FormatWith(
                                                       message1.TopicId, message1.MessageId),
                                                   Null.NullInteger))
                    {
                        searchItemCollection.Add(searchItem);
                    }
                }
                else
                {
                    var searchItem = new SearchItemInfo(
                        "{0} - {1}".FormatWith(forumName, curTopic.TopicName),
                        message.Message,
                        Null.NullInteger,
                        message.Posted,
                        modInfo.ModuleID,
                        "m{0}".FormatWith(message.MessageId),
                        sMessage,
                        "&g=posts&t={0}&m={1}#post{1}".FormatWith(message.TopicId, message.MessageId),
                        Null.NullInteger);

                    searchItemCollection.Add(searchItem);
                }
            }

            foreach (Topics topic in yafTopics)
            {
                if (!topic.ForumId.Equals(this.boardId))
                {
                    continue;
                }

                // Format Topic
                string sTopic = topic.TopicName;

                if (sTopic.Contains(" "))
                {
                    string[] topicWords = sTopic.Split(' ');

                    Topics topic1 = topic;

                    foreach (var searchItem in from topicWord in topicWords
                                               where !string.IsNullOrEmpty(topicWord)
                                               select
                                                   new SearchItemInfo(
                                                   "{0} - {1}".FormatWith(forumName, topic1.TopicName),
                                                   topic1.TopicName,
                                                   Null.NullInteger,
                                                   topic1.Posted,
                                                   modInfo.ModuleID,
                                                   "t{0}".FormatWith(topic1.TopicId),
                                                   topicWord,
                                                   "&g=posts&t={0}".FormatWith(topic1.TopicId),
                                                   Null.NullInteger))
                    {
                        searchItemCollection.Add(searchItem);
                    }
                }
                else
                {
                    var searchItem = new SearchItemInfo(
                        "{0} - {1}".FormatWith(forumName, topic.TopicName),
                        topic.TopicName,
                        Null.NullInteger,
                        topic.Posted,
                        modInfo.ModuleID,
                        "t{0}".FormatWith(topic.TopicId),
                        sTopic,
                        "&g=posts&t={0}".FormatWith(topic.TopicId),
                        Null.NullInteger);

                    searchItemCollection.Add(searchItem);
                }
            }

            return searchItemCollection;
        }

        #endregion

        #endregion
    }
}
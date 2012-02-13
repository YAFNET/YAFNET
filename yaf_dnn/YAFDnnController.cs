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
    using System.Collections.Generic;
    using System.Linq;

    using global::DotNetNuke.Common.Utilities;
    using global::DotNetNuke.Entities.Modules;
    using global::DotNetNuke.Services.Search;

    using YAF.Utils;

    #endregion

    /// <summary>
    /// YAF Dnn Controller
    /// </summary>
    public class YafDnnController : ModuleSettingsBase, ISearchable
    {
        #region Constants and Fields

        /// <summary>
        /// The i board id.
        /// </summary>
        private int iBoardId = 1;

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
                if (!string.IsNullOrEmpty(this.Settings["forumboardid"].ToString()))
                {
                    this.iBoardId = int.Parse(this.Settings["forumboardid"].ToString());
                }
            }
            catch (Exception)
            {
                this.iBoardId = 1;
            }

            var searchItemCollection = new SearchItemInfoCollection();

            // Get all Messages
            List<Messages> yafMessages = Controller.Data.YafDnnGetMessages();

            // Get all Topics
            List<Topics> yafTopics = Controller.Data.YafDnnGetTopics();

            foreach (Messages message in yafMessages)
            {
                // find the Topic of the message
                Messages curMessage = message;

                Topics curTopic = yafTopics.Find(topics => topics.TopicId.Equals(curMessage.TopicId));

                if (!curTopic.ForumId.Equals(this.iBoardId))
                {
                    continue;
                }

                // Format message
                string sMessage = message.Message;

                if (sMessage.Contains(" "))
                {
                    string[] sMessageC = sMessage.Split(' ');

                    Messages message1 = message;

                    foreach (var searchItem in
                        sMessageC.Select(
                            sNewMessage =>
                            new SearchItemInfo(
                                "{0} - {1}".FormatWith(modInfo.ModuleTitle, curTopic.TopicName),
                                message1.Message,
                                Null.NullInteger,
                                message1.Posted,
                                modInfo.ModuleID,
                                "m{0}".FormatWith(message1.MessageId),
                                sNewMessage,
                                "&g=posts&t={0}&m={1}#post{1}".FormatWith(message1.TopicId, message1.MessageId),
                                Null.NullInteger)))
                    {
                        searchItemCollection.Add(searchItem);
                    }
                }
                else
                {
                    var searchItem = new SearchItemInfo(
                        "{0} - {1}".FormatWith(modInfo.ModuleTitle, curTopic.TopicName),
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
                if (!topic.ForumId.Equals(this.iBoardId))
                {
                    continue;
                }

                // Format Topic
                string sTopic = topic.TopicName;

                if (sTopic.Contains(" "))
                {
                    string[] sTopicC = sTopic.Split(' ');

                    Topics topic1 = topic;
                    foreach (var searchItem in
                        sTopicC.Select(
                            sNewTopic =>
                            new SearchItemInfo(
                                "{0} - {1}".FormatWith(modInfo.ModuleTitle, topic1.TopicName),
                                topic1.TopicName,
                                Null.NullInteger,
                                topic1.Posted,
                                modInfo.ModuleID,
                                "t{0}".FormatWith(topic1.TopicId),
                                sNewTopic,
                                "&g=posts&t={0}".FormatWith(topic1.TopicId),
                                Null.NullInteger)))
                    {
                        searchItemCollection.Add(searchItem);
                    }
                }
                else
                {
                    var searchItem = new SearchItemInfo(
                        "{0} - {1}".FormatWith(modInfo.ModuleTitle, topic.TopicName),
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
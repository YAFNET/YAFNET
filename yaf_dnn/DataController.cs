/* Yet Another Forum.NET
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

namespace YAF.DotNetNuke
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Data;

    using global::DotNetNuke.Data;

    using YAF.Utils;

    #endregion

    /// <summary>
    /// DataController to Handling all SQL Stuff
    /// </summary>
    public class DataController
    {
        #region Public Methods

        /// <summary>
        /// Get all Messages From The Forum
        /// </summary>
        /// <returns>
        /// Message List
        /// </returns>
        public static List<Messages> YafDnnGetMessages()
        {
            List<Messages> messagesList = new List<Messages>();

            using (IDataReader dr = DataProvider.Instance().ExecuteReader("YafDnn_Messages"))
            {
                while (dr.Read())
                {
                    Messages message = new Messages
                        {
                            Message = Convert.ToString(dr["Message"]), 
                            MessageId = dr["MessageID"].ToType<int>(), 
                            TopicId = dr["TopicID"].ToType<int>(), 
                            Posted = Convert.ToDateTime(dr["Posted"])
                        };

                    messagesList.Add(message);
                }
            }

            return messagesList;
        }

        /// <summary>
        /// Get all Messages From The Forum
        /// </summary>
        /// <returns>
        /// Topics List
        /// </returns>
        public static List<Topics> YafDnnGetTopics()
        {
            List<Topics> topicsList = new List<Topics>();

            using (IDataReader dr = DataProvider.Instance().ExecuteReader("YafDnn_Topics"))
            {
                while (dr.Read())
                {
                    Topics topic = new Topics
                        {
                            TopicName = Convert.ToString(dr["Topic"]), 
                            TopicId = dr["TopicID"].ToType<int>(), 
                            ForumId = dr["ForumID"].ToType<int>(), 
                            Posted = Convert.ToDateTime(dr["Posted"])
                        };

                    topicsList.Add(topic);
                }
            }

            return topicsList;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using DotNetNuke.Data;

namespace YAF.DotNetNuke
{
    /// <summary>
    /// DataController to Handling all SQL Stuff
    /// </summary>
    public class DataController
    {
        /// <summary>
        /// Get all Messages From The Forum
        /// </summary>
        /// <returns></returns>
        public static List<Messages> YafDnnGetMessages()
        {
            List<Messages> messagesList = new List<Messages>();

            using (IDataReader dr = DataProvider.Instance().ExecuteReader("YafDnn_Messages"))
            {
                while (dr.Read())
                {
                    Messages message = new Messages
                    {
                        sMessage = Convert.ToString(dr["Message"]),
                        iMessageId = Convert.ToInt32(dr["MessageID"]),
                        iTopicId = Convert.ToInt32(dr["TopicID"]),
                        dtPosted = Convert.ToDateTime(dr["Posted"])
                    };

                    messagesList.Add(message);
                }
            }
            return messagesList;
        }
        /// <summary>
        /// Get all Messages From The Forum
        /// </summary>
        /// <returns></returns>
        public static List<Topics> YafDnnGetTopics()
        {
            List<Topics> topicsList = new List<Topics>();

            using (IDataReader dr = DataProvider.Instance().ExecuteReader("YafDnn_Topics"))
            {
                while (dr.Read())
                {
                    Topics topic = new Topics
                    {
                        sTopic = Convert.ToString(dr["Topic"]),
                        iTopicId = Convert.ToInt32(dr["TopicID"]),
                        iForumId = Convert.ToInt32(dr["ForumID"]),
                        dtPosted = Convert.ToDateTime(dr["Posted"])
                    };

                    topicsList.Add(topic);
                }
            }
            return topicsList;
        }
    }
}
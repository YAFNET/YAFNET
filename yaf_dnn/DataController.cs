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
                        Message = Convert.ToString(dr["Message"]),
                        MessageId = Convert.ToInt32(dr["MessageID"]),
                        TopicId = Convert.ToInt32(dr["TopicID"]),
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
                        TopicName = Convert.ToString(dr["Topic"]),
                        TopicId = Convert.ToInt32(dr["TopicID"]),
                        ForumId = Convert.ToInt32(dr["ForumID"]),
                        Posted = Convert.ToDateTime(dr["Posted"])
                    };

                    topicsList.Add(topic);
                }
            }
            return topicsList;
        }
    }
}
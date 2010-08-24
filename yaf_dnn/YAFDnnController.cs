#region Usings

using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search;
using System;


#endregion

namespace YAF.DotNetNuke
{
    /// <summary>
    /// Summary description for DotNetNukeModule.
    /// </summary>
    public class YafDnnController : ModuleSettingsBase, ISearchable
    {
        private int iBoardId = 1;

        /// <summary>
        /// Included as a stub only so that the core knows this module Implements Entities.Modules.ISearchable
        /// </summary>
        /// <param name="modInfo"></param>
        /// <returns></returns>
        public SearchItemInfoCollection GetSearchItems(ModuleInfo modInfo)
        {
            // Get Used Board Id for the Module Instance
            try
            {
                if (!string.IsNullOrEmpty(Settings["forumboardid"].ToString()))
                {
                    iBoardId = int.Parse(Settings["forumboardid"].ToString());
                }
            }
            catch (Exception)
            {
                iBoardId = 1;
            }


            SearchItemInfoCollection searchItemCollection = new SearchItemInfoCollection();

            // Get all Messages
            List<Messages> yafMessages = DataController.YafDnnGetMessages();

            // Get all Topics
            List<Topics> yafTopics = DataController.YafDnnGetTopics();

            foreach (Messages message in yafMessages)
            {
                //find the Topic of the message
                Messages curMessage = message;

                Topics curTopic = yafTopics.Find(topics => topics.iTopicId.Equals(curMessage.iTopicId));

                if (!curTopic.iForumId.Equals(iBoardId)) continue;

                // Format message
                string sMessage = message.sMessage;

                if (sMessage.Contains(" "))
                {
                    string[] sMessageC = sMessage.Split(' ');

                    foreach (string sNewMessage in sMessageC)
                    {
                        SearchItemInfo searchItem = new SearchItemInfo(
                            string.Format("{0} - {1}", modInfo.ModuleTitle, curTopic.sTopic),
                            message.sMessage,
                            Null.NullInteger,
                            message.dtPosted,
                            modInfo.ModuleID,
                            string.Format("m{0}", message.iMessageId),
                            sNewMessage,
                            string.Format("&g=posts&t={0}&m={1}#post{1}", message.iTopicId, message.iMessageId),
                            Null.NullInteger);

                        searchItemCollection.Add(searchItem);
                    }
                }
                else
                {
                    SearchItemInfo searchItem = new SearchItemInfo(
                        string.Format("{0} - {1}", modInfo.ModuleTitle, curTopic.sTopic),
                        message.sMessage,
                        Null.NullInteger,
                        message.dtPosted,
                        modInfo.ModuleID,
                        string.Format("m{0}", message.iMessageId),
                        sMessage,
                        string.Format("&g=posts&t={0}&m={1}#post{1}", message.iTopicId, message.iMessageId),
                        Null.NullInteger);

                    searchItemCollection.Add(searchItem);
                }
            }

            foreach (Topics topic in yafTopics)
            {
                if (!topic.iForumId.Equals(iBoardId)) continue;

                // Format Topic
                string sTopic = topic.sTopic;

                if (sTopic.Contains(" "))
                {
                    string[] sTopicC = sTopic.Split(' ');

                    foreach (string sNewTopic in sTopicC)
                    {
                        SearchItemInfo searchItem = new SearchItemInfo(
                            string.Format("{0} - {1}", modInfo.ModuleTitle,
                                          topic.sTopic),
                            topic.sTopic,
                            Null.NullInteger,
                            topic.dtPosted,
                            modInfo.ModuleID,
                            string.Format("t{0}", topic.iTopicId),
                            sNewTopic,
                            string.Format("&g=posts&t={0}", topic.iTopicId),
                            Null.NullInteger);

                        searchItemCollection.Add(searchItem);
                    }
                }
                else
                {
                    SearchItemInfo searchItem = new SearchItemInfo(
                        string.Format("{0} - {1}", modInfo.ModuleTitle,
                                      topic.sTopic),
                        topic.sTopic,
                        Null.NullInteger,
                        topic.dtPosted,
                        modInfo.ModuleID,
                        string.Format("t{0}", topic.iTopicId),
                        sTopic,
                        string.Format("&g=posts&t={0}", topic.iTopicId),
                        Null.NullInteger);

                    searchItemCollection.Add(searchItem);
                }
            }
            return searchItemCollection;
        }
    }
}
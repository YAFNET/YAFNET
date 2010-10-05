namespace YAF.DotNetNuke
{
  #region Using

  using System;
  using System.Collections.Generic;

  using global::DotNetNuke.Common.Utilities;
  using global::DotNetNuke.Entities.Modules;
  using global::DotNetNuke.Services.Search;

  #endregion

  /// <summary>
  /// Summary description for DotNetNukeModule.
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
    /// <param name="modInfo">
    /// </param>
    /// <returns>
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
      List<Messages> yafMessages = DataController.YafDnnGetMessages();

      // Get all Topics
      List<Topics> yafTopics = DataController.YafDnnGetTopics();

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

          foreach (string sNewMessage in sMessageC)
          {
            var searchItem = new SearchItemInfo(
              string.Format("{0} - {1}", modInfo.ModuleTitle, curTopic.TopicName), 
              message.Message, 
              Null.NullInteger, 
              message.Posted, 
              modInfo.ModuleID, 
              string.Format("m{0}", message.MessageId), 
              sNewMessage, 
              string.Format("&g=posts&t={0}&m={1}#post{1}", message.TopicId, message.MessageId), 
              Null.NullInteger);

            searchItemCollection.Add(searchItem);
          }
        }
        else
        {
          var searchItem = new SearchItemInfo(
            string.Format("{0} - {1}", modInfo.ModuleTitle, curTopic.TopicName), 
            message.Message, 
            Null.NullInteger, 
            message.Posted, 
            modInfo.ModuleID, 
            string.Format("m{0}", message.MessageId), 
            sMessage, 
            string.Format("&g=posts&t={0}&m={1}#post{1}", message.TopicId, message.MessageId), 
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

          foreach (string sNewTopic in sTopicC)
          {
            var searchItem = new SearchItemInfo(
              string.Format("{0} - {1}", modInfo.ModuleTitle, topic.TopicName), 
              topic.TopicName, 
              Null.NullInteger, 
              topic.Posted, 
              modInfo.ModuleID, 
              string.Format("t{0}", topic.TopicId), 
              sNewTopic, 
              string.Format("&g=posts&t={0}", topic.TopicId), 
              Null.NullInteger);

            searchItemCollection.Add(searchItem);
          }
        }
        else
        {
          var searchItem = new SearchItemInfo(
            string.Format("{0} - {1}", modInfo.ModuleTitle, topic.TopicName), 
            topic.TopicName, 
            Null.NullInteger, 
            topic.Posted, 
            modInfo.ModuleID, 
            string.Format("t{0}", topic.TopicId), 
            sTopic, 
            string.Format("&g=posts&t={0}", topic.TopicId), 
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
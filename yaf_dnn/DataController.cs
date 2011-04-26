/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

    using global::DotNetNuke.Common.Lists;
    using global::DotNetNuke.Common.Utilities;
    using global::DotNetNuke.Data;
    using global::DotNetNuke.Entities.Profile;

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

        /// <summary>
        /// Get The Latest DateTime where on of the DNN Profile Fields was updated
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>
        /// The DateTime when the dnn Profile was last updated.
        /// </returns>
        public static DateTime YafDnnGetLastUpdatedProfile(int userID)
        {
            DateTime lastUpdatedDate = new DateTime();

            using (IDataReader dr = DataProvider.Instance().ExecuteReader("YafDnn_LastUpdatedProfile", userID))
            {
                while (dr.Read())
                {
                    lastUpdatedDate = (DateTime)dr["LastUpdatedDate"];
                }
            }

            return lastUpdatedDate;
        }       

        /// <summary>
        /// Adds the Yaf Profile property definitions for a portal
        /// </summary>
        /// <param name="portalId">Id of the Portal</param>
        public static void AddYafProfileDefinitions(int portalId)
        {
            ListController objListController = new ListController();
            ListEntryInfoCollection dataTypes = objListController.GetListEntryInfoCollection("DataType");

            AddYafProfileDefinition(portalId, "YAF Profile", "Birthday", "DateTime", 0, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "Occupation", "Text", 0, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "Gender", "Integer", 0, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "Blog", "Text", 50, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "MSN", "Text", 50, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "YIM", "Text", 50, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "AIM", "Text", 50, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "ICQ", "Text", 50, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "Facebook", "Text", 400, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "Twitter", "Text", 400, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "Region", "Text", 50, dataTypes);
            AddYafProfileDefinition(portalId, "YAF Profile", "XMPP", "Text", 50, dataTypes);
        }

        /// <summary>
        /// Adds a single default property definition
        /// </summary>
        /// <param name="portalId">
        /// Id of the Portal
        /// </param>
        /// <param name="category">
        /// Category of the Property
        /// </param>
        /// <param name="name">
        /// Name of the Property
        /// </param>
        /// <param name="strType">
        /// The str Type.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="types">
        /// The types.
        /// </param>
        public static void AddYafProfileDefinition(int portalId, string category, string name, string strType, int length, ListEntryInfoCollection types)
        {
            ProfilePropertyDefinitionCollection profileProperties = ProfileController.GetPropertyDefinitionsByPortal(portalId);

            int lastViewOrder = profileProperties[profileProperties.Count - 1].ViewOrder;

            ListEntryInfo typeInfo = types.Item("DataType:{0}".FormatWith(strType));

            ProfilePropertyDefinition propertyDefinition = new ProfilePropertyDefinition
                {
                    DataType = typeInfo.EntryID,
                    DefaultValue = string.Empty,
                    ModuleDefId = Null.NullInteger,
                    PortalId = portalId,
                    PropertyCategory = category,
                    PropertyName = name,
                    Required = false,
                    Visible = true,
                    Length = length,
                    ViewOrder = ++lastViewOrder
                };

            ProfileController.AddPropertyDefinition(propertyDefinition);
        }

        #endregion
    }
}
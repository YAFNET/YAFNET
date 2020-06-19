/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Tasks
{
    #region Using

    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// Migrates all old Message Attachments
    /// </summary>
    public class MigrateAttachmentsTask : LongBackgroundTask
    {
        #region Properties

        /// <summary>
        ///   Gets TaskName.
        /// </summary>
        [NotNull]
        public static string TaskName { get; } = "MigrateAttachmentsTask";

        #endregion

        #region Public Methods

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            try
            {
                // attempt to run the migration code...
                var messages = this.GetRepository<Attachment>().GetMessageAttachments();

                if (!messages.Any())
                {
                    return;
                }

                messages.ForEach(
                    message =>
                    {
                        var attachments = this.GetRepository<Attachment>().Get(a => a.MessageID == message.ID);

                        var updatedMessage = new StringBuilder();

                        updatedMessage.Append(message.MessageText);

                        attachments.ForEach(
                            attach =>
                            {
                                updatedMessage.AppendFormat(" [ATTACH]{0}[/Attach] ", attach.ID);

                                // Rename filename
                                if (attach.FileData == null)
                                {
                                    var oldFilePath = this.Get<HttpRequestBase>().MapPath(
                                        $"{BoardFolders.Current.Uploads}/{attach.MessageID}.{attach.FileName}.yafupload");

                                    var newFilePath = this.Get<HttpRequestBase>().MapPath(
                                        $"{BoardFolders.Current.Uploads}/u{attach.UserID}.{attach.FileName}.yafupload");

                                    File.Move(oldFilePath, newFilePath);
                                }

                                attach.MessageID = 0;
                                this.GetRepository<Attachment>().Update(attach);
                            });

                        // Update Message
                        this.GetRepository<Message>().UpdateOnly(
                            () => new Message { MessageText = updatedMessage.ToString() },
                            m => m.ID == message.ID);
                    });
            }
            catch (Exception x)
            {
                this.Logger.Error(x, $"Error In {TaskName} Task");
            }
        }

        #endregion
    }
}
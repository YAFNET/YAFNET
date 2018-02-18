/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Shows a Message Post
    /// </summary>
    public class MessagePost : MessageBase
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether IsAlt.
        /// </summary>
        public virtual bool IsAlt
        {
            get
            {
                return this.ViewState["IsAlt"] != null && Convert.ToBoolean(this.ViewState["IsAlt"]);
            }

            set
            {
                this.ViewState["IsAlt"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets DisplayUserID.
        /// </summary>
        public virtual int? DisplayUserID
        {
            get
            {
                if (this.ViewState["DisplayUserID"] != null)
                {
                    return this.ViewState["DisplayUserID"].ToType<int>();
                }

                return null;
            }

            set
            {
                this.ViewState["DisplayUserID"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets MessageID.
        /// </summary>
        public virtual int? MessageID
        {
            get
            {
                if (this.ViewState["MessageID"] != null)
                {
                    return this.ViewState["MessageID"].ToType<int>();
                }

                return null;
            }

            set
            {
                this.ViewState["MessageID"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets the Words to highlight in this message
        /// </summary>
        [CanBeNull]
        public virtual IList<string> HighlightWords
        {
            get
            {
                return this.ViewState["HighlightWords"] as IList<string> ?? new List<string>();
            }

            set
            {
                this.ViewState["HighlightWords"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsModeratorChanged.
        /// </summary>
        public virtual bool IsModeratorChanged
        {
            get
            {
                return this.ViewState["IsModeratorChanged"] != null
                       && Convert.ToBoolean(this.ViewState["IsModeratorChanged"]);
            }

            set
            {
                this.ViewState["IsModeratorChanged"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Message.
        /// </summary>
        public virtual string Message
        {
            get
            {
                return this.ViewState["Message"] != null ? this.ViewState["Message"].ToString() : null;
            }

            set
            {
                this.ViewState["Message"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets MessageFlags.
        /// </summary>
        public virtual MessageFlags MessageFlags
        {
            get
            {
                return this.ViewState["MessageFlags"] as MessageFlags ?? new MessageFlags(0);
            }

            set
            {
                this.ViewState["MessageFlags"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Signature.
        /// </summary>
        public virtual string Signature
        {
            get
            {
                return this.ViewState["Signature"] != null ? this.ViewState["Signature"].ToString() : null;
            }

            set
            {
                this.ViewState["Signature"] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Highlight a Message
        /// </summary>
        /// <param name="message">The Message to Highlight</param>
        /// <param name="renderBbCode">if set to <c>true</c> Render Highlight as BB Code or as HTML Tags</param>
        /// <returns>
        /// The Message with the Span Tag and CSS Class "highlight" that Highlights it
        /// </returns>
        protected virtual string HighlightMessage([NotNull] string message, bool renderBbCode = false)
        {
            if (this.HighlightWords.Count > 0)
            {
                // highlight word list
                message = this.Get<IFormatMessage>().SurroundWordList(
                    message, this.HighlightWords, renderBbCode ? "[h]" : @"<span class=""highlight"">", renderBbCode ? "[/h]" : @"</span>");
            }

            return message;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            if (this.Signature.IsSet())
            {
                var sig = new MessageSignature
                    {
                        Signature = this.Signature,
                        DisplayUserId = this.DisplayUserID,
                        MessageId = this.MessageID
                    };

                this.Controls.Add(sig);
            }

            base.OnPreRender(e);
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            writer.BeginRender();
            writer.WriteBeginTag("div");

            writer.WriteAttribute("id", this.MessageID.HasValue ? this.MessageID.ToString() : this.ClientID);

            if (this.MessageID.HasValue)
            {
                writer.WriteAttribute("class", "selectionQuoteable");
            }

            writer.Write(HtmlTextWriter.TagRightChar);

            this.RenderMessage(writer);

            // render controls...
            base.Render(writer);

            writer.WriteEndTag("div");
            writer.EndRender();
        }

        /// <summary>
        /// The render deleted message.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="deleteText">The delete reason text.</param>
        protected virtual void RenderDeletedMessage([NotNull] HtmlTextWriter writer, string deleteText)
        {
            // if message was deleted then write that instead of real body
            if (!this.MessageFlags.IsDeleted)
            {
                return;
            }

            writer.Write(
                @"<div class=""alert alert-danger"" role=""alert""><strong>{1}</strong>{0}</div>",
                deleteText.IsSet()
                    ? @"&nbsp;<span class=""editedinfo"" title=""{1}"">{0}: {1}</span>".FormatWith(
                        this.GetText("EDIT_REASON"),
                        deleteText)
                    : string.Empty,
                this.IsModeratorChanged
                    ? this.GetText("POSTS", "MESSAGEDELETED_MOD")
                    : this.GetText("POSTS", "MESSAGEDELETED_USER"));
        }

        /// <summary>
        /// Renders the edited message.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="edited">The edited date time.</param>
        /// <param name="editReason">The edit reason text.</param>
        /// <param name="messageId">The message id.</param>
        protected virtual void RenderEditedMessage(
            [NotNull] HtmlTextWriter writer, [NotNull] DateTime edited, [NotNull] string editReason, int? messageId)
        {
            if (!this.Get<YafBoardSettings>().ShowEditedMessage)
            {
                return;
            }

            var editedDateTime = new DisplayDateTime { DateTime = edited }.RenderToString();

            // vzrus: TODO:  Guests doesn't have right to view change history
            // reason was specified ?!
            var editReasonText = "{0}: {1}".FormatWith(
                this.GetText("EDIT_REASON"),
                this.Get<HttpContextBase>().Server.HtmlDecode(editReason).IsSet()
                    ? this.Get<IFormatMessage>().RepairHtml(editReason, true)
                    : this.GetText("EDIT_REASON_NA"));

            // message has been edited
            // show, why the post was edited or deleted?
            var whoChanged = this.IsModeratorChanged
                                 ? this.GetText("POSTS", "EDITED_BY_MOD")
                                 : this.GetText("POSTS", "EDITED_BY_USER");

            writer.Write(
                @"<div class=""alert alert-secondary"" role=""alert"">
                      <a title=""{3}"" alt=""title=""{3}"" href=""{4}""><strong>{0}</strong> {1}</a>&nbsp;{2}&nbsp;|&nbsp;<em>{3}</em>
                      <button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close"">
                          <span aria-hidden=""true"">&times;</span>
                      </button></div>"
                    .FormatWith(
                        this.GetText("EDITED"),
                        whoChanged,
                        editedDateTime,
                        editReasonText,
                        this.PageContext.IsGuest
                            ? "#"
                            : YafBuildLink.GetLink(ForumPages.messagehistory, "m={0}", messageId.ToType<int>())));
        }

        /// <summary>
        /// Renders the answer message.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="messageId">The message identifier.</param>
        protected virtual void RenderAnswerMessage(
           [NotNull] HtmlTextWriter writer, int messageId)
        {
            writer.Write(
                @"<div class=""alert alert-success"" role=""alert"">
                      <a title=""{0}"" alt=""title=""{0}"" href=""{1}""><i class=""fa fa-check fa-fw""></i>{0}</a>
                      <button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close"">
                          <span aria-hidden=""true"">&times;</span>
                      </button></div>"
                    .FormatWith(
                        this.GetText("GO_TO_ANSWER"),
                        YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", messageId)));
        }

        /// <summary>
        /// The render message.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected virtual void RenderMessage([NotNull] HtmlTextWriter writer)
        {
            if (this.MessageFlags.IsDeleted)
            {
                // deleted message text...
                this.RenderDeletedMessage(writer, string.Empty);
            }
            else if (this.MessageFlags.NotFormatted)
            {
                writer.Write(this.Message);
            }
            else
            {
                var formattedMessage =
                    this.Get<IFormatMessage>().FormatMessage(
                        this.HighlightMessage(this.Message, true), this.MessageFlags);

                // tha_watcha : Since HTML message and BBCode can be mixed now, message should be always replace BBCode
                this.RenderModulesInBBCode(
                    writer, formattedMessage, this.MessageFlags, this.DisplayUserID, this.MessageID);
            }
        }

        #endregion
    }
}
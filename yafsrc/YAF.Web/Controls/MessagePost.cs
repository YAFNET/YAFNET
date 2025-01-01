﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
namespace YAF.Web.Controls;

/// <summary>
/// Shows a Message Post
/// </summary>
public class MessagePost : MessageBase
{
    /// <summary>
    ///   Gets or sets DisplayUserID.
    /// </summary>
    public virtual int? DisplayUserID
    {
        get => this.ViewState["DisplayUserID"]?.ToType<int>();

        set => this.ViewState["DisplayUserID"] = value;
    }

    /// <summary>
    ///   Gets or sets MessageID.
    /// </summary>
    public virtual int? MessageID
    {
        get => this.ViewState["MessageID"] != null ? this.ViewState["MessageID"].ToType<int?>() : 0;

        set => this.ViewState["MessageID"] = value;
    }

    /// <summary>
    ///   Gets or sets the Words to highlight in this message
    /// </summary>
    public virtual IList<string> HighlightWords
    {
        get => this.ViewState["HighlightWords"] as IList<string> ?? [];

        set => this.ViewState["HighlightWords"] = value;
    }

    /// <summary>
    ///   Gets or sets a value indicating whether IsModeratorChanged.
    /// </summary>
    public virtual bool IsModeratorChanged
    {
        get =>
            this.ViewState["IsModeratorChanged"] != null
            && Convert.ToBoolean(this.ViewState["IsModeratorChanged"]);

        set => this.ViewState["IsModeratorChanged"] = value;
    }

    /// <summary>
    ///   Gets or sets Message.
    /// </summary>
    public virtual string Message
    {
        get => this.ViewState["Message"]?.ToString();

        set => this.ViewState["Message"] = value;
    }

    /// <summary>
    ///   Gets or sets MessageFlags.
    /// </summary>
    public virtual MessageFlags MessageFlags
    {
        get => this.ViewState["MessageFlags"] as MessageFlags ?? new MessageFlags(0);

        set => this.ViewState["MessageFlags"] = value;
    }

    /// <summary>
    ///   Gets or sets Signature.
    /// </summary>
    public virtual string Signature
    {
        get => this.ViewState["Signature"]?.ToString();

        set => this.ViewState["Signature"] = value;
    }

    /// <summary>
    /// Highlight a Message
    /// </summary>
    /// <param name="message">The Message to Highlight</param>
    /// <param name="renderBbCode">if set to <c>true</c> Render Highlight as BB Code or as HTML Tags</param>
    /// <returns>
    /// The Message with the Span Tag and CSS Class "highlight" that Highlights it
    /// </returns>
    protected virtual string HighlightMessage(string message, bool renderBbCode = false)
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
    override protected void OnPreRender(EventArgs e)
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
    override protected void Render(HtmlTextWriter writer)
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
    protected virtual void RenderDeletedMessage(HtmlTextWriter writer, string deleteText)
    {
        // if message was deleted then write that instead of real body
        if (!this.MessageFlags.IsDeleted)
        {
            return;
        }

        writer.Write(
            @"<div class=""alert alert-danger"" role=""alert""><strong>{1}</strong>{0}</div>",
            deleteText.IsSet()
                ? $@"&nbsp;<span title=""{deleteText}"">{this.GetText("EDIT_REASON")}: {deleteText}</span>"
                : string.Empty,
            this.GetText("POSTS", this.IsModeratorChanged ? "MESSAGEDELETED_MOD" : "MESSAGEDELETED_USER"));
    }

    /// <summary>
    /// Renders the edited message.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="edited">The edited date time.</param>
    /// <param name="editReason">The edit reason text.</param>
    /// <param name="messageId">The message id.</param>
    protected virtual void RenderEditedMessage(
        HtmlTextWriter writer, DateTime edited, string editReason, int? messageId)
    {
        var editedDateTime = new DisplayDateTime { DateTime = edited }.RenderToString();

        // reason was specified ?!
        var editReasonText =
            $"{this.GetText("EDIT_REASON")}: {(this.Get<HttpContextBase>().Server.HtmlDecode(editReason).IsSet() ? this.Get<IFormatMessage>().RepairHtml(editReason, true) : this.GetText("EDIT_REASON_NA"))}";

        // message has been edited
        // show, why the post was edited or deleted?
        var whoChanged = this.IsModeratorChanged
                             ? this.GetText("POSTS", "EDITED_BY_MOD")
                             : this.GetText("POSTS", "EDITED_BY_USER");

        var messageHistoryButton =
            $@"<hr />
                   <p class=""mb-0""><a href=""{this.Get<LinkBuilder>().GetLink(ForumPages.MessageHistory, new { m = messageId.Value })}"" class=""btn btn-secondary btn-sm me-1"">
                         <i class=""fa fa-history fa-fw""></i>{this.GetText("MESSAGEHISTORY", "TITLE")}
                      </a></p>";

        writer.Write(
            @"<div class=""alert alert-secondary mt-1"" role=""alert"">
                      <i class=""fa fa-edit fa-fw text-secondary""></i>{0} {1}
                      &nbsp;{2}&nbsp;|&nbsp;<em>{3}</em> {4}</div>",
            this.GetText("EDITED"),
            whoChanged,
            editedDateTime,
            editReasonText,
            this.PageBoardContext.IsGuest ? string.Empty : messageHistoryButton);
    }

    /// <summary>
    /// Renders the answer message.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="messageId">The message identifier.</param>
    protected virtual void RenderAnswerMessage(
        HtmlTextWriter writer, int messageId)
    {
        writer.Write(
            @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                      <a title=""{0}"" href=""{1}""><i class=""fa fa-check fa-fw""></i>{0}</a>
                      <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close"">
                      </button></div>",
            this.GetText("GO_TO_ANSWER"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Posts, new { m = messageId, name = this.PageBoardContext.PageTopic.TopicName }));
    }

    /// <summary>
    /// The render message.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected virtual void RenderMessage(HtmlTextWriter writer)
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
            var formattedMessage = this.Get<IFormatMessage>().Format(
                this.MessageID.Value,
                this.HighlightMessage(this.Message, true),
                this.MessageFlags);

            // tha_watcha : Since HTML message and BBCode can be mixed now, message should be always replace BBCode
            this.RenderModulesInBBCode(
                writer, formattedMessage, this.MessageFlags, this.DisplayUserID, this.MessageID);
        }
    }
}
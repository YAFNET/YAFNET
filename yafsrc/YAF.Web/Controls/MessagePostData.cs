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
public class MessagePostData : MessagePost
{
    /// <summary>
    ///   The _row.
    /// </summary>
    private Message currentMessage;

    /// <summary>
    ///   Sets the DataRow.
    /// </summary>
    public PagedMessage DataRow
    {
        set => this.CurrentMessage = value != null ? new Message(value) : null;
    }

    /// <summary>
    /// Gets or sets the current message.
    /// </summary>
    /// <value>
    /// The current message.
    /// </value>
    public Message CurrentMessage
    {
        get => this.currentMessage;

        set
        {
            this.currentMessage = value ?? new Message();
            this.MessageFlags = this.currentMessage.MessageFlags;
        }
    }

    /// <summary>
    ///   Gets Edited.
    /// </summary>
    public DateTime Edited => this.CurrentMessage.Edited ?? this.CurrentMessage.Posted;

    /// <summary>
    ///   Gets Message.
    /// </summary>
    public override string Message => TruncateMessage(this.CurrentMessage.MessageText ?? string.Empty);

    /// <summary>
    ///   Gets Message Id.
    /// </summary>
    public override int? MessageID => this.CurrentMessage.ID;

    /// <summary>
    ///   Gets or sets a value indicating whether Show the Edit Message if needed.
    /// </summary>
    public bool ShowEditMessage { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether ShowAttachments.
    /// </summary>
    public bool ShowAttachments { get; set; } = true;

    /// <summary>
    ///   Gets or sets a value indicating whether ShowSignature.
    /// </summary>
    public bool ShowSignature { get; set; } = true;

    /// <summary>
    ///   Gets Signature.
    /// </summary>
    
    public override string Signature
    {
        get
        {
            if (this.ShowSignature && this.PageBoardContext.BoardSettings.AllowSignatures
                                   && this.CurrentMessage.Signature.IsSet()
                                   && this.CurrentMessage.Signature.ToLower() != "<p>&nbsp;</p>")
            {
                return this.CurrentMessage.Signature;
            }

            return null;
        }
    }

    /// <summary>
    /// Truncates the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>
    /// The truncate message.
    /// </returns>
    public static string TruncateMessage(string message)
    {
        var maxPostSize = Math.Max(BoardContext.Current.BoardSettings.MaxPostSize, 0);

        // 0 == unlimited
        return maxPostSize == 0 || message.Length <= maxPostSize ? message : message.Truncate(maxPostSize);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    override protected void OnPreRender(EventArgs e)
    {
        if (!this.MessageFlags.IsDeleted && !this.Get<IAspNetUsersHelper>().IsGuestUser(this.CurrentMessage.UserID))
        {
            this.DisplayUserID = this.CurrentMessage.UserID;
        }

        base.OnPreRender(e);
    }

    /// <summary>
    /// The render message.
    /// </summary>
    /// <param name="writer">The writer.</param>
    override protected void RenderMessage(HtmlTextWriter writer)
    {
        if (this.MessageFlags.IsDeleted)
        {
            this.IsModeratorChanged = this.CurrentMessage.IsModeratorChanged ?? false;

            var deleteText = this.Get<HttpContextBase>().Server.HtmlDecode(this.CurrentMessage.DeleteReason).IsSet()
                                 ? this.Get<IFormatMessage>().RepairHtml(this.CurrentMessage.DeleteReason, true)
                                 : this.GetText("EDIT_REASON_NA");

            // deleted message text...
            this.RenderDeletedMessage(writer, deleteText);
        }
        else if (this.MessageFlags.NotFormatted)
        {
            // just write out the message with no formatting...
            writer.Write(this.Message);
        }
        else if (this.CurrentMessage.Edited > this.CurrentMessage.Posted)
        {
            this.IsModeratorChanged = this.CurrentMessage.IsModeratorChanged ?? false;

            // handle a message that's been edited...
            var editedMessageDateTime = this.CurrentMessage.Posted;

            if (this.Edited > this.CurrentMessage.Posted)
            {
                editedMessageDateTime = this.Edited;
            }

            var formattedMessage = this.Get<IFormatMessage>().Format(
                this.CurrentMessage.ID,
                this.HighlightMessage(this.Message, true),
                this.MessageFlags,
                false,
                editedMessageDateTime);

            this.RenderModulesInBBCode(
                writer,
                formattedMessage,
                this.MessageFlags,
                this.DisplayUserID,
                this.MessageID);

            // Render Edit Message
            if (this.ShowEditMessage
                && this.Edited > this.CurrentMessage.Posted.AddSeconds(this.PageBoardContext.BoardSettings.EditTimeOut))
            {
                this.RenderEditedMessage(writer, this.Edited, this.CurrentMessage.EditReason, this.MessageID);
            }

            // Render Go to Answer Message
            if (this.CurrentMessage.AnswerMessageId.HasValue && this.CurrentMessage.Position.Equals(0))
            {
                this.RenderAnswerMessage(writer, this.CurrentMessage.AnswerMessageId.Value);
            }
        }
        else
        {
            var formattedMessage = this.Get<IFormatMessage>().Format(
                this.CurrentMessage.ID,
                this.HighlightMessage(this.Message, true),
                this.MessageFlags);

            this.RenderModulesInBBCode(
                writer,
                formattedMessage,
                this.MessageFlags,
                this.DisplayUserID,
                this.MessageID);

            // Render Go to Answer Message
            if (this.CurrentMessage.AnswerMessageId.HasValue && this.CurrentMessage.Position.Equals(0))
            {
                this.RenderAnswerMessage(writer, this.CurrentMessage.AnswerMessageId.Value);
            }
        }
    }
}
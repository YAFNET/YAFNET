/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Web.TagHelpers;

/// <summary>
/// The message post tag helper.
/// </summary>
[HtmlTargetElement("messagePostData")]
public class MessagePostDataTagHelper : MessagePostTagHelper
{
    /// <summary>
    /// Sets the data row.
    /// </summary>
    /// <value>The data row.</value>
    public PagedMessage PagedMessage { get; set; }

    /// <summary>
    ///   Gets Message.
    /// </summary>
    public override string Message => TruncateMessage(this.CurrentMessage.MessageText ?? string.Empty);

    /// <summary>
    ///   Gets Message Id.
    /// </summary>
    public override int? MessageId => this.CurrentMessage.ID;

    /// <summary>
    ///   Gets Signature.
    /// </summary>
    public override string Signature
    {
        get
        {
            if (this.ShowSignature && BoardContext.Current.BoardSettings.AllowSignatures
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
    /// The process.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (this.PagedMessage is not null)
        {
            this.CurrentMessage = new Message(this.PagedMessage);
            this.MessageFlags = this.CurrentMessage.MessageFlags;
        }

        output.TagName = HtmlTag.Div;

        output.Attributes.Add(HtmlAttribute.Id, this.CurrentMessage.ID.ToString());

        output.Attributes.Add(HtmlAttribute.Class, "selectionQuoteable");

        if (!this.MessageFlags.IsDeleted && !this.Get<IAspNetUsersHelper>().IsGuestUser(this.CurrentMessage.UserID))
        {
            this.DisplayUserId = this.CurrentMessage.UserID;
        }

        if (this.MessageFlags.IsDeleted)
        {
            // deleted message text...
            this.RenderDeletedMessage(output);
        }
        else if (this.MessageFlags.NotFormatted)
        {
            // just write out the message with no formatting...
            output.Content.Append(this.Message);
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

            var formattedMessage = await this.Get<IFormatMessage>().FormatMessageWithAllBBCodesAsync(
                this.HighlightMessage(this.Message, true), this.CurrentMessage.ID, this.DisplayUserId, false,
                editedMessageDateTime);

            output.Content.AppendHtml(formattedMessage);

            // Render Edit Message
            if (this.ShowEditMessage
                && this.Edited > this.CurrentMessage.Posted.AddSeconds(this.Get<BoardSettings>().EditTimeOut))
            {
                this.RenderEditedMessage(output, this.Edited, this.CurrentMessage.EditReason, this.MessageId);
            }

            // Render Go to Answer Message
            if (this.CurrentMessage.AnswerMessageId.HasValue && this.CurrentMessage.Position.Equals(0))
            {
                this.RenderAnswerMessage(output, this.CurrentMessage.AnswerMessageId.Value);
            }
        }
        else
        {
            var formattedMessage = await this.Get<IFormatMessage>().FormatMessageWithAllBBCodesAsync(
                this.HighlightMessage(this.Message, true), this.CurrentMessage.ID, this.DisplayUserId);

            output.Content.AppendHtml(formattedMessage);

            // Render Go to Answer Message
            if (this.CurrentMessage.AnswerMessageId.HasValue && this.CurrentMessage.Position.Equals(0))
            {
                this.RenderAnswerMessage(output, this.CurrentMessage.AnswerMessageId.Value);
            }
        }

        if (this.ShowSignature && this.Get<BoardSettings>().AllowSignatures && this.CurrentMessage.Signature.IsSet()
            && !this.CurrentMessage.Signature.Equals("<p>&nbsp;</p>", StringComparison.CurrentCultureIgnoreCase)
            && this.DisplayUserId.HasValue)
        {
            await this.RenderSignatureAsync(output);
        }
    }
}
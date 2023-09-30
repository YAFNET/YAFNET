/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using YAF.Types.Attributes;

/// <summary>
/// The message post tag helper.
/// </summary>
[HtmlTargetElement("message")]
public class MessagePostTagHelper : TagHelper, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    ///   The options.
    /// </summary>
    private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled;

    /// <summary>
    ///   The localization.
    /// </summary>
    private ILocalization localization;

    /// <summary>
    ///   The _row.
    /// </summary>
    private Message currentMessage;

    private int? messageId;

    private MessageFlags messageFlags;

    private IList<string> highlightWords;

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => this.localization ??= this.Get<ILocalization>();

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
    ///   Gets or sets a value indicating whether Show the Edit Message if needed.
    /// </summary>
    public bool ShowEditMessage { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether ShowAttachments.
    /// </summary>
    public bool ShowAttachments { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether show answer message link.
    /// </summary>
    public bool ShowAnswerMessageLink { get; set; } = true;

    /// <summary>
    ///   Gets or sets a value indicating whether ShowSignature.
    /// </summary>
    public bool ShowSignature { get; set; } = true;

    /// <summary>
    /// Gets or sets the display user id.
    /// </summary>
    public int? DisplayUserId { get; set; }

    /// <summary>
    ///   Gets or sets the Words to highlight in this message
    /// </summary>
    
    public virtual IList<string> HighlightWords
    {
        get => this.highlightWords ?? new List<string>();
        set => this.highlightWords = value;
    }

    /// <summary>
    ///   Gets or sets a value indicating whether IsModeratorChanged.
    /// </summary>
    public virtual bool IsModeratorChanged { get; set; }

    /// <summary>
    ///   Gets or sets Message.
    /// </summary>
    public virtual string Message { get; set; }

    /// <summary>
    ///   Gets or sets MessageID.
    /// </summary>
    public virtual int? MessageId
    {
        get => this.messageId ?? 0;
        set => this.messageId = value;
    }

    /// <summary>
    ///   Gets or sets MessageFlags.
    /// </summary>
    public virtual MessageFlags MessageFlags
    {
        get => this.messageFlags ?? new MessageFlags(0);
        set => this.messageFlags = value;
    }

    /// <summary>
    ///   Gets or sets Signature.
    /// </summary>
    public virtual string Signature { get; set; }

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Gets CustomBBCode.
    /// </summary>
    protected IDictionary<BBCode, Regex> CustomBBCode
    {
        get
        {
            return this.Get<IObjectStore>().GetOrSet(
                "CustomBBCodeRegExDictionary",
                () =>
                    {
                        var bbcodeTable = this.Get<IBBCodeService>().GetCustomBBCode();
                        return bbcodeTable
                            .Where(b => (b.UseModule ?? false) && b.ModuleClass.IsSet() && b.SearchRegex.IsSet())
                            .ToDictionary(codeRow => codeRow, codeRow => new Regex(codeRow.SearchRegex, Options));
                    });
        }
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
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        this.HighlightWords = new List<string>();

        if (this.Message.IsSet())
        {
            output.TagName = "div";

            this.RenderSimpleMessage(output);

            if (this.ShowSignature && this.Get<BoardSettings>().AllowSignatures &&
                this.Signature.IsSet() && this.Signature.ToLower() != "<p>&nbsp;</p>" &&
                this.DisplayUserId.HasValue)
            {
                this.RenderSignature(output);
            }
        }
        else
        {
            if (this.CurrentMessage is not null)
            {
                if (!this.MessageFlags.IsDeleted && !this.Get<IAspNetUsersHelper>().IsGuestUser(this.CurrentMessage.UserID))
                {
                    this.DisplayUserId = this.CurrentMessage.UserID;
                }
            }

            output.TagName = "div";

            output.Attributes.Add("id", this.CurrentMessage.ID.ToString());

            output.Attributes.Add("class", "selectionQuoteable");

            this.RenderMessage(output);

            if (this.ShowSignature && this.Get<BoardSettings>().AllowSignatures &&
                this.CurrentMessage.Signature.IsSet() && this.CurrentMessage.Signature.ToLower() != "<p>&nbsp;</p>" &&
                this.DisplayUserId.HasValue)
            {
                this.RenderSignature(output);
            }
        }

        return Task.CompletedTask;
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
                message,
                this.HighlightWords,
                renderBbCode ? "[h]" : @"<span class=""highlight"">",
                renderBbCode ? "[/h]" : @"</span>");
        }

        return message;
    }

    /// <summary>
    /// The render signature.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected virtual void RenderSignature(TagHelperOutput output)
    {
        var hr = new TagBuilder("hr") { TagRenderMode = TagRenderMode.SelfClosing };

        output.Content.AppendHtml(hr);

        var card = new TagBuilder("div");

        card.AddCssClass("card border-light card-message-signature");

        var cardBody = new TagBuilder("div");

        cardBody.AddCssClass("card-body py-0");

        // don't allow any HTML on signatures
        var signatureFlags = new MessageFlags { IsHtml = false };

        this.Signature = Core.Helpers.HtmlTagHelper.StripHtml(this.Signature);

        var signatureRendered = this.Get<IFormatMessage>().Format(0, this.Signature, signatureFlags);

        cardBody.InnerHtml.AppendHtml(
            this.RenderModulesInBBCode(
                signatureRendered,
                signatureFlags,
                this.DisplayUserId,
                this.CurrentMessage.ID));

        card.InnerHtml.AppendHtml(cardBody);

        output.Content.AppendHtml(card);
    }

    /// <summary>
    /// The render deleted message.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    /// <param name="deleteText">
    /// The delete reason text.
    /// </param>
    protected virtual void RenderDeletedMessage(TagHelperOutput output, string deleteText)
    {
        // if message was deleted then write that instead of real body
        if (!this.MessageFlags.IsDeleted)
        {
            return;
        }

        var alert = new TagBuilder("div");

        alert.AddCssClass("alert alert-danger");

        alert.MergeAttribute("role", "alert");

        var strong = new TagBuilder("strong");

        strong.InnerHtml.Append(
            this.GetText("POSTS", this.IsModeratorChanged ? "MESSAGEDELETED_MOD" : "MESSAGEDELETED_USER"));

        alert.InnerHtml.AppendHtml(strong);

        if (deleteText.IsSet())
        {
            var span = new TagBuilder("span");

            span.AddCssClass("ms-1");

            span.InnerHtml.AppendFormat("{0}: {1}", this.GetText("EDIT_REASON"), deleteText);

            alert.InnerHtml.AppendHtml(span);
        }

        output.Content.AppendHtml(alert);
    }

    /// <summary>
    /// Renders the edited message.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    /// <param name="edited">
    /// The edited date time.
    /// </param>
    /// <param name="editReason">
    /// The edit reason text.
    /// </param>
    /// <param name="msgId">
    /// The message id.
    /// </param>
    protected virtual void RenderEditedMessage(
        TagHelperOutput output,
        DateTime edited,
        string editReason,
        int? msgId)
    {
        if (!this.Get<BoardSettings>().ShowEditedMessage)
        {
            return;
        }

        var editedDateTime = this.Get<IHtmlHelper>().DisplayDateTime(DateTimeFormat.Both, edited);

        // reason was specified ?!
        var editReasonText =
            $"{this.GetText("EDIT_REASON")}: {(HttpUtility.HtmlDecode(editReason).IsSet() ? this.Get<IFormatMessage>().RepairHtml(editReason, true) : this.GetText("EDIT_REASON_NA"))}";

        // message has been edited
        // show, why the post was edited or deleted?
        var whoChanged = this.IsModeratorChanged
                             ? this.GetText("POSTS", "EDITED_BY_MOD")
                             : this.GetText("POSTS", "EDITED_BY_USER");

        var alert = new TagBuilder("div");

        alert.AddCssClass("alert alert-secondary mt-1");

        alert.MergeAttribute("role", "alert");

        var icon = new TagBuilder("i");

        icon.AddCssClass("fa fa-edit fa-fw text-secondary");

        alert.InnerHtml.AppendHtml(icon);

        alert.InnerHtml.AppendFormat("{0} {1}", this.GetText("EDITED"), whoChanged);

        alert.InnerHtml.AppendHtml(editedDateTime);

        alert.InnerHtml.Append(" | ");

        var em = new TagBuilder("em");

        em.AddCssClass("me-1");

        em.InnerHtml.Append(editReasonText);

        alert.InnerHtml.AppendHtml(em);

        if (!BoardContext.Current.IsGuest)
        {
            var hr = new TagBuilder("hr") { TagRenderMode = TagRenderMode.SelfClosing };

            alert.InnerHtml.AppendHtml(hr);

            var p = new TagBuilder("p");

            p.AddCssClass("mb-0");

            var link = new TagBuilder("a");

            link.AddCssClass("btn btn-secondary btn-sm me-1");

            link.MergeAttribute(
                "href",
                this.Get<LinkBuilder>().GetLink(ForumPages.MessageHistory, new { m = msgId.ToType<int>() }));

            var iconHistory = new TagBuilder("i");

            iconHistory.AddCssClass("fa fa-history fa-fw me-1");

            link.InnerHtml.AppendHtml(iconHistory);

            link.InnerHtml.Append(this.GetText("MESSAGEHISTORY", "TITLE"));

            p.InnerHtml.AppendHtml(link);

            alert.InnerHtml.AppendHtml(p);
        }

        output.Content.AppendHtml(alert);
    }

    /// <summary>
    /// Renders the answer message.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    /// <param name="messageId">
    /// The message identifier.
    /// </param>
    protected virtual void RenderAnswerMessage(TagHelperOutput output, int messageId)
    {
        var alert = new TagBuilder("div");

        alert.AddCssClass("alert alert-success alert-dismissible fade show");

        alert.MergeAttribute("role", "alert");

        var link = new TagBuilder("a");

        link.MergeAttribute("title", this.GetText("GO_TO_ANSWER"));
        link.MergeAttribute(
            "href",
            this.Get<LinkBuilder>().GetLink(
                ForumPages.Posts,
                new { m = messageId, name = BoardContext.Current.PageTopic.TopicName }));

        var icon = new TagBuilder("i");

        icon.AddCssClass("fa fa-check fa-fw me-1");

        link.InnerHtml.AppendHtml(icon);
        link.InnerHtml.Append(this.GetText("GO_TO_ANSWER"));

        alert.InnerHtml.AppendHtml(link);

        var button = new TagBuilder("button");

        button.AddCssClass("btn-close");
        button.MergeAttribute("type", "button");
        button.MergeAttribute("data-bs-dismiss", "alert");
        button.MergeAttribute("aria-label", "close");

        alert.InnerHtml.AppendHtml(button);

        output.Content.AppendHtml(alert);
    }

    /// <summary>
    /// The render message.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected virtual void RenderSimpleMessage(TagHelperOutput output)
    {
        var formattedMessage = this.Get<IFormatMessage>().Format(
            0,
            this.HighlightMessage(this.Message, true),
            this.MessageFlags);

        // tha_watcha : Since HTML message and BBCode can be mixed now, message should be always replace BBCode
        output.Content.AppendHtml(
            this.RenderModulesInBBCode(
                formattedMessage,
                this.MessageFlags,
                this.DisplayUserId,
                0));
    }

    /// <summary>
    /// The render message.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected virtual void RenderMessage(TagHelperOutput output)
    {
        if (this.MessageFlags.IsDeleted)
        {
            this.IsModeratorChanged = this.CurrentMessage.IsModeratorChanged ?? false;

            var deleteText = HttpUtility.HtmlDecode(this.CurrentMessage.DeleteReason).IsSet()
                                 ? this.Get<IFormatMessage>().RepairHtml(this.CurrentMessage.DeleteReason, true)
                                 : this.GetText("EDIT_REASON_NA");

            // deleted message text...
            this.RenderDeletedMessage(output, deleteText);
        }
        else if (this.MessageFlags.NotFormatted)
        {
            // just write out the message with no formatting...
            output.Content.Append(this.CurrentMessage.MessageText);
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
                this.HighlightMessage(this.CurrentMessage.MessageText, true),
                this.MessageFlags,
                false,
                editedMessageDateTime);

            output.Content.AppendHtml(
                this.RenderModulesInBBCode(
                    formattedMessage,
                    this.MessageFlags,
                    this.DisplayUserId,
                    this.CurrentMessage.ID));

            // Render Edit Message
            if (this.ShowEditMessage && this.Edited >
                this.CurrentMessage.Posted.AddSeconds(this.Get<BoardSettings>().EditTimeOut))
            {
                this.RenderEditedMessage(
                    output,
                    this.Edited,
                    this.CurrentMessage.EditReason,
                    this.CurrentMessage.ID);
            }

            // Render Go to Answer Message
            if (this.CurrentMessage.AnswerMessageId.HasValue && this.CurrentMessage.Position.Equals(0) &&
                this.ShowAnswerMessageLink)
            {
                this.RenderAnswerMessage(output, this.CurrentMessage.AnswerMessageId.Value);
            }
        }
        else
        {
            var formattedMessage = this.Get<IFormatMessage>().Format(
                this.CurrentMessage.ID,
                this.HighlightMessage(this.CurrentMessage.MessageText, true),
                this.MessageFlags);

            // tha_watcha : Since HTML message and BBCode can be mixed now, message should be always replace BBCode
            output.Content.AppendHtml(
                this.RenderModulesInBBCode(
                    formattedMessage,
                    this.MessageFlags,
                    this.DisplayUserId,
                    this.CurrentMessage.ID));

            // Render Go to Answer Message
            if (this.CurrentMessage.AnswerMessageId.HasValue && this.CurrentMessage.Position.Equals(0))
            {
                this.RenderAnswerMessage(output, this.CurrentMessage.AnswerMessageId.Value);
            }
        }
    }

    /// <summary>
    /// The render modules in bb code.
    /// </summary>
    /// <param name="message">
    /// The message
    /// </param>
    /// <param name="theseFlags">
    /// The these flags.
    /// </param>
    /// <param name="displayUserId">
    /// The display user id.
    /// </param>
    /// <param name="msgId">
    /// The Message Id.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    protected virtual string RenderModulesInBBCode(
        string message,
        MessageFlags theseFlags,
        int? displayUserId,
        int? msgId)
    {
        var workingMessage = message;

        // handle custom bbcodes row by row...
        this.CustomBBCode.ForEach(
            keyPair =>
                {
                    var codeRow = keyPair.Key;

                    Match match;

                    do
                    {
                        match = keyPair.Value.Match(workingMessage);

                        if (!match.Success)
                        {
                            continue;
                        }

                        var sb = new StringBuilder();

                        var paramDic = new Dictionary<string, string> { { "inner", match.Groups["inner"].Value } };

                        if (codeRow.Variables.IsSet() && codeRow.Variables.Split(';').Any())
                        {
                            var vars = codeRow.Variables.Split(';');

                            vars.Where(v => match.Groups[v] is not null).ForEach(
                                v => paramDic.Add(v, match.Groups[v].Value));
                        }

                        sb.Append(workingMessage[..match.Groups[0].Index]);

                        // create/render the control...
                        var module = Type.GetType(codeRow.ModuleClass, true, false);
                        var customModule = (BBCodeControl)Activator.CreateInstance(module);

                        // assign parameters...
                        customModule.CurrentMessageFlags = theseFlags;
                        customModule.DisplayUserID = displayUserId;
                        customModule.MessageID = msgId;
                        customModule.Parameters = paramDic;

                        // render this control...
                        customModule.Render(sb);

                        sb.Append(workingMessage[(match.Groups[0].Index + match.Groups[0].Length)..]);

                        workingMessage = sb.ToString();
                    }
                    while (match.Success);
                });

        return workingMessage;
    }
}
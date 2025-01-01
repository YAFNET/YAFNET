/* Yet Another Forum.NET
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

namespace YAF.Web.TagHelpers;

/// <summary>
/// The message post tag helper.
/// </summary>
[HtmlTargetElement("signature")]
public class SignaturePreviewTagHelper : TagHelper, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    ///   The options.
    /// </summary>
    private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled
                                        ;

    /// <summary>
    ///   The localization.
    /// </summary>
    private ILocalization localization;

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => this.localization ??= this.Get<ILocalization>();

    /// <summary>
    /// Gets or sets the display user id.
    /// </summary>
    public int? DisplayUserID { get; set; }

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
                            .ToDictionary(
                                codeRow => codeRow,
                                codeRow => new Regex(codeRow.SearchRegex, Options, TimeSpan.FromMilliseconds(100)));
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
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (this.Signature.IsSet())
        {
            this.RenderSignature(output);
        }
    }

    /// <summary>
    /// The render signature.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected virtual void RenderSignature(TagHelperOutput output)
    {
        output.TagName = HtmlTag.Div;

        output.Attributes.Add(HtmlAttribute.Class, "card card-message-signature mb-3");

        var cardBody = new TagBuilder(HtmlTag.Div);

        cardBody.AddCssClass("card-body");

        // don't allow any HTML on signatures
        var signatureFlags = new MessageFlags { IsHtml = false };

        var signatureRendered = this.Get<IFormatMessage>().Format(0, Core.Helpers.HtmlTagHelper.StripHtml(this.Signature), signatureFlags);

        cardBody.InnerHtml.AppendHtml(
            this.RenderModulesInBBCode(
                signatureRendered,
                signatureFlags,
                this.DisplayUserID,
                0));

        output.Content.AppendHtml(cardBody);
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
    /// <param name="messageId">
    /// The Message Id.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private string RenderModulesInBBCode(
        string message,
        MessageFlags theseFlags,
        int? displayUserId,
        int? messageId)
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

                        if (codeRow.Variables.IsSet() && codeRow.Variables.Split(';').Length != 0)
                        {
                            var vars = codeRow.Variables.Split(';');

                            vars.Where(v => match.Groups[v] != null).ForEach(
                                v => paramDic.Add(v, match.Groups[v].Value));
                        }

                        sb.Append(workingMessage[..match.Groups[0].Index]);

                        // create/render the control...
                        var module = Type.GetType(codeRow.ModuleClass, true, false);
                        var customModule = (BBCodeControl)Activator.CreateInstance(module);

                        // assign parameters...
                        customModule.CurrentMessageFlags = theseFlags;
                        customModule.DisplayUserID = displayUserId;
                        customModule.MessageID = messageId;
                        customModule.Parameters = paramDic;

                        // render this control...
                        customModule.RenderAsync(sb);

                        sb.Append(workingMessage[(match.Groups[0].Index + match.Groups[0].Length)..]);

                        workingMessage = sb.ToString();
                    }
                    while (match.Success);
                });

        return workingMessage;
    }
}
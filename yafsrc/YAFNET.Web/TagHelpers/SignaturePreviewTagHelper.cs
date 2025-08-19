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
    /// The process.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (this.Signature.IsSet())
        {
            await this.RenderSignatureAsync(output);
        }
    }

    /// <summary>
    /// The render signature.
    /// </summary>
    /// <param name="output">
    ///     The output.
    /// </param>
    async protected virtual Task RenderSignatureAsync(TagHelperOutput output)
    {
        output.TagName = HtmlTag.Div;

        output.Attributes.Add(HtmlAttribute.Class, "card card-message-signature mb-3");

        var cardBody = new TagBuilder(HtmlTag.Div);

        cardBody.AddCssClass("card-body");

        var signatureRendered = await this.Get<IFormatMessage>().FormatMessageWithAllBBCodesAsync(
            Core.Helpers.HtmlTagHelper.StripHtml(this.Signature), 0, this.DisplayUserID);

        cardBody.InnerHtml.AppendHtml(
            signatureRendered);

        output.Content.AppendHtml(cardBody);
    }
}
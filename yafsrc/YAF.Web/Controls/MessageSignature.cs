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
namespace YAF.Web.Controls;

/// <summary>
/// The message signature.
/// </summary>
public class MessageSignature : MessageBase
{
    /// <summary>
    /// Gets or sets the display user identifier.
    /// </summary>
    /// <value>
    /// The display user identifier.
    /// </value>
    public int? DisplayUserId { get; set; }

    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    /// <value>
    /// The message identifier.
    /// </value>
    public int? MessageId { get; set; }

    /// <summary>
    /// Gets or sets Signature.
    /// </summary>
    /// <value>
    /// The signature.
    /// </value>
    public string Signature { get; set; }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
        if (this.Signature.IsNotSet())
        {
            return;
        }

        writer.BeginRender();

        writer.Write("<hr />");

        writer.WriteBeginTag("div");
        writer.WriteAttribute("class", "card border-light card-message-signature");
        writer.Write(HtmlTextWriter.TagRightChar);

        writer.WriteBeginTag("div");
        writer.WriteAttribute("class", "card-body py-0");
        writer.Write(HtmlTextWriter.TagRightChar);

        this.RenderSignature(writer);

        base.Render(writer);

        writer.WriteEndTag("div");

        writer.WriteEndTag("div");

        writer.EndRender();
    }

    /// <summary>
    /// The render signature.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected void RenderSignature([NotNull] HtmlTextWriter writer)
    {
        if (!this.DisplayUserId.HasValue)
        {
            return;
        }

        // don't allow any HTML on signatures
        var signatureFlags = new MessageFlags { IsHtml = false };

        var signatureRendered = this.Get<IFormatMessage>().Format(0, this.Signature, signatureFlags);

        this.RenderModulesInBBCode(writer, signatureRendered, signatureFlags, this.DisplayUserId, this.MessageId);
    }
}
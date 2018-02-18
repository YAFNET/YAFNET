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

    using System.Web.UI;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The message signature.
    /// </summary>
    public class MessageSignature : MessageBase
    {
        #region Properties

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

        #endregion

        #region Methods

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
            writer.WriteAttribute("class", "yafsignature");
            writer.Write(HtmlTextWriter.TagRightChar);

            this.RenderSignature(writer);

            base.Render(writer);

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

            var signatureRendered = this.Get<IFormatMessage>().FormatMessage(this.Signature, signatureFlags);

            this.RenderModulesInBBCode(writer, signatureRendered, signatureFlags, this.DisplayUserId, this.MessageId);
        }

        #endregion
    }
}
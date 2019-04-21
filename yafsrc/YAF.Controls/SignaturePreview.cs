/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
    public class SignaturePreview : MessageBase
    {
        #region Properties

        /// <summary>
        ///   Gets or sets DisplayUserID.
        /// </summary>
        public int? DisplayUserID { get; set; }

        /// <summary>
        ///   Gets or sets MessageID.
        /// </summary>
        public int? MessageID { get; set; }

        /// <summary>
        ///   Gets or sets Signature.
        /// </summary>
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
            writer.BeginRender();

            if (this.Signature.IsSet())
            {
                this.RenderSignature(writer);
            }

            base.Render(writer);

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
            if (!this.DisplayUserID.HasValue)
            {
                return;
            }

            // don't allow any HTML on signatures
            var signatureFlags = new MessageFlags { IsHtml = false };

            var signatureRendered = this.Get<IFormatMessage>().FormatMessage(this.Signature, signatureFlags);

            this.RenderModulesInBBCode(writer, signatureRendered, signatureFlags, this.DisplayUserID, this.MessageID);
        }

        #endregion
    }
}
/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The message signature.
    /// </summary>
    public class MessageSignature : MessageBase
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
        ///   Gets or sets HtmlPrefix.
        /// </summary>
        public string HtmlPrefix { get; set; }

        /// <summary>
        ///   Gets or sets HtmlSuffix.
        /// </summary>
        public string HtmlSuffix { get; set; }

        /// <summary>
        ///   Gets or sets Signature.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether IsAlt.
        /// </summary>
        public bool IsAlt { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether Col Span is.
        /// </summary>
        public string ColSpan { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The get post class.
        /// </summary>
        /// <returns>
        /// Returns the post class.
        /// </returns>
        [NotNull]
        protected string GetPostClass()
        {
            return this.IsAlt ? "post_alt" : "post";
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

            if (!this.Get<ITheme>().ThemeFile.Contains("Mobile"))
            {
                writer.Write("</tr><tr class=\"{0}\">", this.GetPostClass());


                // Bad Hack
                if (this.ClientID.Contains("LastPosts"))
                {
                    writer.Write("<td></td>");
                }

                writer.WriteBeginTag("td");
                writer.WriteAttribute("id", this.ClientID);
                writer.WriteAttribute("class", "SignatureColumn");
                writer.WriteAttribute("colspan", "2");
                writer.Write(HtmlTextWriter.TagRightChar);
            }

            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", "yafsignature");
            writer.Write(HtmlTextWriter.TagRightChar);

            if (this.HtmlPrefix.IsSet())
            {
                writer.Write(this.HtmlPrefix);
            }

            if (this.Signature.IsSet())
            {
                this.RenderSignature(writer);
            }

            if (this.HtmlSuffix.IsSet())
            {
                writer.Write(this.HtmlSuffix);
            }

            base.Render(writer);


            writer.WriteEndTag("div");

            if (!this.Get<ITheme>().ThemeFile.Contains("Mobile"))
            {
                writer.WriteEndTag("td");
            } 


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

            string signatureRendered = this.Get<IFormatMessage>().FormatMessage(this.Signature, signatureFlags);

            this.RenderModulesInBBCode(writer, signatureRendered, signatureFlags, this.DisplayUserID, this.MessageID);
        }

        #endregion
    }
}
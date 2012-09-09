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
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Makes a very simple localized label
    /// </summary>
    public class HelpLabel : BaseControl, ILocalizationSupport
    {
        #region Constants and Fields

        /// <summary>
        /// The _enable bb code.
        /// </summary>
        protected bool _enableBBCode;

        /// <summary>
        /// The _suffix.
        /// </summary>
        protected string _suffix = string.Empty;

        /// <summary>
        /// The _localized page.
        /// </summary>
        protected string _localizedPage = string.Empty;

        /// <summary>
        /// The _localized tag.
        /// </summary>
        protected string _localizedTag = string.Empty;

        /// <summary>
        /// The _localized tag.
        /// </summary>
        protected string _localizedHelpTag = string.Empty;

        /// <summary>
        /// The _param 0.
        /// </summary>
        protected string _param0 = string.Empty;

        /// <summary>
        /// The _param 1.
        /// </summary>
        protected string _param1 = string.Empty;

        /// <summary>
        /// The _param 2.
        /// </summary>
        protected string _param2 = string.Empty;

         /// <summary>
        /// The _param 0.
        /// </summary>
        protected string _paramHelp0 = string.Empty;

        /// <summary>
        /// The _param 1.
        /// </summary>
        protected string _paramHelp1 = string.Empty;

        /// <summary>
        /// The _param 2.
        /// </summary>
        protected string _paramHelp2 = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpLabel"/> class.
        /// </summary>
        public HelpLabel()
            : base()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether EnableBBCode.
        /// </summary>
        public bool EnableBBCode
        {
            get
            {
                return this._enableBBCode;
            }

            set
            {
                this._enableBBCode = value;
            }
        }

        /// <summary>
        /// Gets or sets Suffix. e.g: ":" or "?"
        /// </summary>
        public string Suffix
        {
            get
            {
                return this._suffix;
            }

            set
            {
                this._suffix = value;
            }
        }

        /// <summary>
        /// Gets or sets LocalizedPage.
        /// </summary>
        public string LocalizedPage
        {
            get
            {
                return this._localizedPage;
            }

            set
            {
                this._localizedPage = value;
            }
        }

        /// <summary>
        /// Gets or sets LocalizedTag.
        /// </summary>
        public string LocalizedHelpTag
        {
            get
            {
                return string.IsNullOrEmpty(this._localizedHelpTag)
                           ? "{0}_HELP".FormatWith(this._localizedTag)
                           : this._localizedHelpTag;
            }

            set
            {
                this._localizedHelpTag = value;
            }
        }

        /// <summary>
        /// Gets or sets LocalizedTag.
        /// </summary>
        public string LocalizedTag
        {
            get
            {
                return this._localizedTag;
            }

            set
            {
                this._localizedTag = value;
            }
        }

        /// <summary>
        /// Gets or sets Param0.
        /// </summary>
        public string Param0
        {
            get
            {
                return this._param0;
            }

            set
            {
                this._param0 = value;
            }
        }

        /// <summary>
        /// Gets or sets Param1.
        /// </summary>
        public string Param1
        {
            get
            {
                return this._param1;
            }

            set
            {
                this._param1 = value;
            }
        }

        /// <summary>
        /// Gets or sets Param2.
        /// </summary>
        public string Param2
        {
            get
            {
                return this._param2;
            }

            set
            {
                this._param2 = value;
            }
        }

        /// <summary>
        /// Gets or sets ParamHelp0.
        /// </summary>
        public string ParamHelp0
        {
            get
            {
                return this._paramHelp0;
            }

            set
            {
                this._paramHelp0 = value;
            }
        }

        /// <summary>
        /// Gets or sets ParamHelp1.
        /// </summary>
        public string ParamHelp1
        {
            get
            {
                return this._paramHelp1;
            }

            set
            {
                this._paramHelp1 = value;
            }
        }

        /// <summary>
        /// Gets or sets ParamHelp2.
        /// </summary>
        public string ParamHelp2
        {
            get
            {
                return this._paramHelp2;
            }

            set
            {
                this._paramHelp2 = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the localized text string (if available)
        /// </summary>
        /// <param name="output">
        /// </param>
        protected override void Render(HtmlTextWriter output)
        {
            output.BeginRender();
            output.Write("<strong>");

            // Write Title
            output.Write(
                this.GetText(this.LocalizedPage, this.LocalizedTag).FormatWith(
                    this.Param0, this.Param1, this.Param2));

            // Append Suffix
            if (this.Suffix.IsSet())
            {
                output.Write(this.Suffix);
            }

            output.Write("</strong>");
            output.Write("<br />");

            output.Write("<em>");

            // Write Help Text
            output.Write(
                this.GetText(this.LocalizedPage, this.LocalizedHelpTag).FormatWith(
                    this.ParamHelp0, this.ParamHelp1, this.ParamHelp2));

            output.Write("</em>");

            output.EndRender();
        }

        #endregion
    }
}
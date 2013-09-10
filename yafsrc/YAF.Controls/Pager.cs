/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Pager Control.
    /// </summary>
    public class Pager : BaseControl, IPostBackEventHandler, IPager
    {
        #region Constants and Fields

        /// <summary>
        ///   The _goto page form.
        /// </summary>
        private readonly GotoPageForm _gotoPageForm = new GotoPageForm();

        /// <summary>
        ///   The _page label.
        /// </summary>
        private readonly Label _pageLabel = new Label();

        /// <summary>
        ///   The _ignore page index.
        /// </summary>
        private bool _ignorePageIndex;

        /// <summary>
        ///   The _use post back.
        /// </summary>
        private bool _usePostBack = true;

        #endregion

        #region Events

        /// <summary>
        ///   The page change.
        /// </summary>
        public event EventHandler PageChange;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.ViewState["Count"] != null ? (int)this.ViewState["Count"] : 0;
            }

            set
            {
                this.ViewState["Count"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets CurrentPageIndex.
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return (this.ViewState["CurrentPageIndex"] ?? 0).ToType<int>();
            }

            set
            {
                this.ViewState["CurrentPageIndex"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets LinkedPager.
        /// </summary>
        public string LinkedPager
        {
            get
            {
                return (string)this.ViewState["LinkedPager"];
            }

            set
            {
                this.ViewState["LinkedPager"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets PageSize.
        /// </summary>
        public int PageSize
        {
            get
            {
                return this.ViewState["PageSize"] != null ? (int)this.ViewState["PageSize"] : 20;
            }

            set
            {
                this.ViewState["PageSize"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether UsePostBack.
        /// </summary>
        public bool UsePostBack
        {
            get
            {
                return this._usePostBack;
            }

            set
            {
                this._usePostBack = value;
            }
        }

        /// <summary>
        ///   Gets the Current Linked Pager.
        /// </summary>
        [CanBeNull]
        protected Pager CurrentLinkedPager
        {
            get
            {
                if (this.LinkedPager != null)
                {
                    var linkedPager = (Pager)this.Parent.FindControl(this.LinkedPager);

                    if (linkedPager == null)
                    {
                        throw new Exception("Failed to link pager to '{0}'.".FormatWith(this.LinkedPager));
                    }

                    return linkedPager;
                }

                return null;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IPostBackEventHandler

        /// <summary>
        /// The raise post back event.
        /// </summary>
        /// <param name="eventArgument">
        /// The event argument.
        /// </param>
        public void RaisePostBackEvent([NotNull] string eventArgument)
        {
            if (this.LinkedPager != null)
            {
                // raise post back event on the linked pager...
                this.CurrentLinkedPager.RaisePostBackEvent(eventArgument);
            }
            else if (this.PageChange != null)
            {
                this.CurrentPageIndex = int.Parse(eventArgument) - 1;
                this._ignorePageIndex = true;
                this.PageChange(this, new EventArgs());
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The copy pager settings.
        /// </summary>
        /// <param name="toPager">
        /// The to pager.
        /// </param>
        protected void CopyPagerSettings([NotNull] Pager toPager)
        {
            toPager.Count = this.Count;
            toPager.CurrentPageIndex = this.CurrentPageIndex;
            toPager.PageSize = this.PageSize;
        }

        /// <summary>
        /// Gets the page URL.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        /// The get page url.
        /// </returns>
        protected string GetPageURL(int page)
        {
            string url;

            // create proper query string...
            var parser = new SimpleURLParameterParser(this.Get<HttpRequestBase>().QueryString.ToString());

            // get the current page
            var currentPage = (ForumPages)Enum.Parse(typeof(ForumPages), parser["g"], true);

            if (parser["m"] != null)
            {
                // must be converted to by topic...
                parser.Parameters.Remove("m");
                parser.Parameters.Add("t", YafContext.Current.PageTopicID.ToString());
            }

            if (page > 1)
            {
                string tmp = parser.CreateQueryString(new[] { "g", "p", "tabid", "find" });
                if (tmp.Length > 0)
                {
                    tmp += "&";
                }

                tmp += "p={0}";

                url = YafBuildLink.GetLink(currentPage, tmp, page);
            }
            else
            {
                url = YafBuildLink.GetLink(currentPage, parser.CreateQueryString(new[] { "g", "p", "tabid", "find" }));
            }

            return url;
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);

            if (!this._ignorePageIndex && this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p") != null)
            {
                // set a new page...
                this.CurrentPageIndex =
                    (int)Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p")) -
                    1;
            }

            this._pageLabel.ID = this.GetExtendedID("PageLabel");
            this._gotoPageForm.ID = this.GetExtendedID("GotoPageForm");

            this.Controls.Add(this._pageLabel);
            this.Controls.Add(this._gotoPageForm);

            // hook up events...
            this._gotoPageForm.GotoPageClick += this._gotoPageForm_GotoPageClick;
        }

        /// <summary>
        /// The on load.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnLoad([NotNull] EventArgs e)
        {
            base.OnLoad(e);

            // init the necessary js...
            this.PageContext.PageElements.RegisterJQuery();
            this.PageContext.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");

            this.PageContext.PageElements.RegisterCssBlock("PagerCss", "#simplemodal-overlay {background-color:#000;}");
            this._pageLabel.Attributes.Add("style", "cursor: pointer");

            const string GetBoxFunction = @"jQuery.fn.getBox = function() { return { 
                                                  left: $(this).offset().left, top: $(this).offset().top,
                                                  width: $(this).outerWidth(), height: $(this).outerHeight()
                                                };};
                  var gotoForumSuppressClick = false;  openGotoPageClick = function(e) {return false;};";

            string modalFunction =
                @"openGotoPageForm{2} = function(id) {{

var labelBox = jQuery('#' + id).getBox();
var modalBox = jQuery('#{0}').getBox();
var gotoForm = jQuery('#{0}');

var topOffset = labelBox.top+labelBox.height;
var leftOffset = labelBox.left;

if (jQuery('#' + id).parents('.ui-tabs').length > 0)
{{
   topOffset  = topOffset-jQuery('.ui-widget-content').offset().top;
   leftOffset = leftOffset-12;
}}

gotoForm.css({{position:'absolute',zindex:999,top:topOffset,left:leftOffset}});
gotoForm.fadeIn( 'slow', function() {{
	jQuery('#{0}').bind('click', openGotoPageClick);  
	jQuery(document).bind('click', function(e) {{
		jQuery('#{0}').hide();
		var fn = arguments.callee;
		jQuery(document).unbind('click', fn);
		jQuery('#{0}').unbind('click', openGotoPageClick);
	}});
  jQuery('#{1}').focus();

}});

}};
".FormatWith(this._gotoPageForm.ClientID, this._gotoPageForm.GotoTextBoxClientID, this.ClientID);

            // register...
            this.PageContext.PageElements.RegisterJsBlock(
               "getBoxJs{0}", GetBoxFunction);
            this.PageContext.PageElements.RegisterJsBlock(
                "OpenGotoPageFormJs{0}".FormatWith(this.ClientID), modalFunction);
            this.PageContext.PageElements.RegisterJsBlockStartup(
                @"LoadPagerForm_{0}".FormatWith(this.ClientID),
                @"Sys.Application.add_load(function() {{ jQuery('#{0}').click(function() {{ openGotoPageForm{1}('{0}'); }}); }});"
                    .FormatWith(this._pageLabel.ClientID, this.ClientID));
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter output)
        {
            if (this.LinkedPager != null)
            {
                // just copy the linked pager settings but still render in this function...
                this.CurrentLinkedPager.CopyPagerSettings(this);
            }

            if (this.PageCount() < 2)
            {
                return;
            }

            output.WriteLine(
                @"<div class=""yafpager"" title=""{0}"" id=""{1}"">".FormatWith(
                    this.Get<ILocalization>().TransPage.IsSet()
                        ? this.GetText("COMMON", "GOTOPAGE_HEADER")
                        : "Go to page...",
                    this.ClientID));

            this._pageLabel.CssClass = "pagecount";

            // have to be careful about localization because the pager is used in the admin pages...
            string pagesText = "Pages";
            if (this.Get<ILocalization>().TransPage.IsSet())
            {
                pagesText = this.GetText("COMMON", "PAGES");
            }

            this._pageLabel.Text = @"{0:N0} {1}".FormatWith(this.PageCount(), pagesText);

            // render this control...
            this._pageLabel.RenderControl(output);

            this.OutputLinks(output, this.UsePostBack);

            this._gotoPageForm.RenderControl(output);

            output.WriteLine("</div>");

            // base.Render( output );
        }

        /// <summary>
        /// Gets the link URL.
        /// </summary>
        /// <param name="pageNum">The page num.</param>
        /// <param name="postBack">The post back.</param>
        /// <returns>
        /// The get link url.
        /// </returns>
        private string GetLinkUrl(int pageNum, bool postBack)
        {
            return postBack
                       ? this.Page.ClientScript.GetPostBackClientHyperlink(this, pageNum.ToString())
                       : this.GetPageURL(pageNum);
        }

        /// <summary>
        /// The output links.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <param name="postBack">
        /// The post back.
        /// </param>
        private void OutputLinks([NotNull] HtmlTextWriter output, bool postBack)
        {
            int iStart = this.CurrentPageIndex - 2;
            int iEnd = this.CurrentPageIndex + 3;
            if (iStart < 0)
            {
                iStart = 0;
            }

            if (iEnd > this.PageCount())
            {
                iEnd = this.PageCount();
            }

            if (iStart > 0)
            {
                output.RenderAnchorBegin(
                    this.GetLinkUrl(1, postBack), "pagelinkfirst", this.GetText("COMMON", "GOTOFIRSTPAGE_TT"));

                output.WriteBeginTag("span");
                output.Write(HtmlTextWriter.TagRightChar);

                output.Write("&laquo;");
                output.WriteEndTag("span");
                output.WriteEndTag("a");
            }

            if (this.CurrentPageIndex > iStart)
            {
                output.RenderAnchorBegin(
                    this.GetLinkUrl(this.CurrentPageIndex, postBack),
                    "pagelink",
                    this.GetText("COMMON", "GOTOPREVPAGE_TT"));

                output.WriteBeginTag("span");
                output.Write(HtmlTextWriter.TagRightChar);

                output.Write("&lt;");
                output.WriteEndTag("span");
                output.WriteEndTag("a");
            }

            for (int i = iStart; i < iEnd; i++)
            {
                if (i == this.CurrentPageIndex)
                {
                    output.WriteBeginTag("span");
                    output.WriteAttribute("class", "pagecurrent");
                    output.Write(HtmlTextWriter.TagRightChar);
                    output.Write(i + 1);
                    output.WriteEndTag("span");
                }
                else
                {
                    string page = (i + 1).ToString();

                    output.RenderAnchorBegin(this.GetLinkUrl(i + 1, postBack), "pagelink", page);

                    output.WriteBeginTag("span");
                    output.Write(HtmlTextWriter.TagRightChar);

                    output.Write(page);
                    output.WriteEndTag("span");
                    output.WriteEndTag("a");
                }
            }

            if (this.CurrentPageIndex < (this.PageCount() - 1))
            {
                output.RenderAnchorBegin(
                    this.GetLinkUrl(this.CurrentPageIndex + 2, postBack),
                    "pagelink",
                    this.GetText("COMMON", "GOTONEXTPAGE_TT"));

                output.WriteBeginTag("span");
                output.Write(HtmlTextWriter.TagRightChar);

                output.Write("&gt;");
                output.WriteEndTag("span");
                output.WriteEndTag("a");
            }

            if (iEnd >= this.PageCount())
            {
                return;
            }

            output.RenderAnchorBegin(
                this.GetLinkUrl(this.PageCount(), postBack), "pagelinklast", this.GetText("COMMON", "GOTOLASTPAGE_TT"));

            output.WriteBeginTag("span");
            output.Write(HtmlTextWriter.TagRightChar);

            output.Write("&raquo;");
            output.WriteEndTag("span");
            output.WriteEndTag("a");
        }

        /// <summary>
        /// The _goto page form_ goto page click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void _gotoPageForm_GotoPageClick([NotNull] object sender, [NotNull] GotoPageForumEventArgs e)
        {
            int newPage = e.GotoPage - 1;

            if (newPage >= 0 && newPage < this.PageCount())
            {
                // set a new page index...
                this.CurrentPageIndex = newPage;
                this._ignorePageIndex = true;
            }

            if (this.LinkedPager != null)
            {
                // raise post back event on the linked pager...
                this.CurrentLinkedPager._gotoPageForm_GotoPageClick(sender, e);
            }
            else if (this.PageChange != null)
            {
                this.PageChange(this, new EventArgs());
            }
        }

        #endregion
    }
}
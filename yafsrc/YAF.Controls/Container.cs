using System;
using System.Data;
using System.ComponentModel;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace YAF.Controls
{
    /// <summary>
    /// Rendered DIV container
    /// </summary>
    public class Container : Control
    {
        private ITemplate _contentTemplate;
        private string _cssClass, _hideText, _showText, _title;
        private bool _roundedCorners;

        #region Private strings

        /// <summary>
        /// Summary description for ForumJump.
        /// </summary>
        private string ExpandLink()
        {
            string jsHref = String.Format("javascript:toggleContainer('{0}', '{1}', '{2}', '{3}');", this.ContentsClientID, this.LinkClientID, this.ShowText, this.HideText);
            string link = String.Format("<a id=\"{0}\" href=\"{1}\">{2}</a>", this.LinkClientID, jsHref, this.ShowText);
            return link;
        }

        /// <summary>
        /// Client ID for Contents DIV - Required for Javascript
        /// </summary>
        private string ContentsClientID
        {
            get { return this.ClientID + "_content"; }
        }

        /// <summary>
        /// Client ID for HTML Link - Required for Javascript
        /// </summary>
        private string LinkClientID
        {
            get { return this.ClientID + "_expandLink"; }
        }

        #endregion

        #region Render Methods

        /// <summary>
        /// Renders surrounding div
        /// </summary>
        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteBeginTag("div");
            writer.WriteAttribute("id", this.ClientID);
            if (! (String.IsNullOrEmpty(this.CSSClass))) // Make sure CSS Class is not empty before rendering attribute
                writer.WriteAttribute("class", this.CSSClass);
            writer.Write(">");
            writer.WriteLine();
            if (this.RoundedCorners)
                this.RenderRoundedCorners(writer); // Render additional DIVs for Sliding doors technique
            else
                this.RenderContents(writer); // Render contents div and Contents
            writer.WriteLine();
            writer.WriteEndTag("div");
            writer.WriteLine();
        }

        /// <summary>
        /// Renders surrounding DIV for Sliding Doors Rounded Corners technique
        /// </summary>
        protected void RenderRoundedCorners(HtmlTextWriter writer)
        {
            // Write Begining div
            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", "roundedHeader");
            writer.Write(">");
            writer.Write("<div class=\"rightCorner\"></div>");
            writer.WriteEndTag("div");

            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", "roundedContents");
            writer.Write(">");
            this.RenderContents(writer); // Render all Content Controls
            writer.WriteEndTag("div");
            // Write End div
            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", "roundedFooter");
            writer.Write(">");
            writer.Write("<div class=\"rightCorner\"></div>");
            writer.WriteEndTag("div");

        }

        /// <summary>
        /// Renders contents div and childcontrols
        /// </summary>
        protected void RenderContents(HtmlTextWriter writer)
        {
            // Expandable DIV
            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", "expandablePanel");
            writer.Write(">");
            writer.WriteLine();

            //Container DIV
            writer.WriteBeginTag("div");
            writer.WriteAttribute("id", this.ContentsClientID);
            writer.WriteAttribute("class", "contents");
            writer.Write(">");
            writer.WriteLine();

            if (!String.IsNullOrEmpty(this.Title))
            {
                writer.WriteFullBeginTag("h2");
                writer.Write(this.Title);
                writer.WriteEndTag("h2");
                writer.WriteLine();
            }

            // Render all Contents in the Contents Template
            PlaceHolder contentsPlaceHolder = new PlaceHolder();
            this.Contents.InstantiateIn(contentsPlaceHolder);
            contentsPlaceHolder.RenderControl(writer);

            writer.WriteEndTag("div");
            writer.WriteLine();

            // Footer DIV
            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", "footer");
            writer.Write(">");
            writer.WriteLine();
            writer.WriteLine(this.ExpandLink()); // Render Show/Hide
            writer.WriteLine();
            writer.WriteEndTag("div");
            writer.WriteLine();


        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Contents to render within the container
        /// </summary>
        [
          Browsable(false),
          PersistenceMode(PersistenceMode.InnerProperty),
          DefaultValue(typeof(ITemplate), ""),
          Description("Contents")
        ]
        public ITemplate Contents
        {
            get { return _contentTemplate; }
            set { _contentTemplate = value; }
        }

        /// <summary>
        /// Css Clas for surrounding DIV
        /// </summary>
        public string CSSClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }

        /// <summary>
        /// Hide Text required for expanding/collapsing container
        /// </summary>
        public string ShowText
        {
            get { return _showText; }
            set { _showText = value; }
        }

        /// <summary>
        /// Hide Text required for expanding/collapsing container
        /// </summary>
        public string HideText
        {
            get { return _hideText; }
            set { _hideText = value; }
        }

        /// <summary>
        /// If present renders a h2 html tag
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// If true, renders SLIDING doors technique additional DIV tags
        /// </summary>
        public bool RoundedCorners
        {
            get { return _roundedCorners; }
            set { _roundedCorners = value; }
        }

        #endregion

    }
}

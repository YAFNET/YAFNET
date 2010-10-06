///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// The MultiView Base Class
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    //   [ParseChildren(true)]
    public abstract class JQueryMultiViewControl : CompositeControl, IPostBackDataHandler
    {
        protected int newIndex = -1;
        private ViewCollection views;
        private int selectedIndex;

        [Browsable(false)]
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        /// <summary>
        /// Gets/Sets the ViewControl whether can post back the event to server
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets the ViewControl whether can post back the event to server")]
        [Bindable(true)]
        public bool AutoPostBack
        {
            get
            {
                Object obj = ViewState["AutoPostBack"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["AutoPostBack"] = value;
            }
        }

        /// <summary>
        /// Gets the View Colleciton of the jQueryMultiViewControl
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(false)]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(CollectionConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ViewCollection Views
        {
            get { return views ?? (views = new ViewCollection(this)); }
        }

        /// <summary>
        /// Gets/Sets the header style for views
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets the header style for views")]
        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        public string HeaderStyle
        {
            get
            {
                Object obj = ViewState["HeaderStyle"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["HeaderStyle"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the content style
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets the content style")]
        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        public string ContentStyle
        {
            get
            {
                Object obj = ViewState["ContentStyle"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["ContentStyle"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the view content style class
        /// </summary>
        [Description(" Gets/Sets the view content style class")]
        [Category("Appearance")]
        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [CssClassProperty]
        public string ContentCssClass
        {
            get
            {
                Object obj = ViewState["ContentCssClass"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["ContentCssClass"] = value;
            }
        }

        /// <summary>
        /// Ges/Sets the header style class
        /// </summary>
        [Category("Appearance")]
        [Description("Ges/Sets the header style class")]
        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [CssClassProperty]
        public string HeaderCssClass
        {
            get
            {
                Object obj = ViewState["HeaderCssClass"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["HeaderCssClass"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the active view control's index
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets the active view control's index")]
        [Browsable(true)]
        [Bindable(true)]
        [NotifyParentProperty(true)]
        public virtual int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                EnsureChildControls();
                OnActiveViewChanged();

                if (value != selectedIndex)
                {
                    View deactiveView = Views[SelectedIndex];
                    if (deactiveView != null)
                        deactiveView.OnDeactive();
                }

                View activeView = Views[value];
                if (activeView != null)
                    activeView.OnActive();
                selectedIndex = value;
            }
        }

        public void ApplyViewStyle()
        {
            foreach (View view in Views)
            {
               if (!string.IsNullOrEmpty(HeaderStyle))
                    if (string.IsNullOrEmpty(view.HeaderStyle))
                        view.HeaderStyle = HeaderStyle;

                if (!string.IsNullOrEmpty(ContentStyle))
                    view.Style.Value = ContentStyle;

                if (string.IsNullOrEmpty(view.CssClass))
                    view.CssClass = ContentCssClass;

                if (!string.IsNullOrEmpty(HeaderCssClass))
                    view.HeaderCssClass = HeaderCssClass;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (Views.Count > 0)
            {
                if (!Page.IsPostBack)
                    if (SelectedIndex == 0)
                        SelectedIndex = 0;
            }

            ApplyViewStyle();
            Page.RegisterRequiresPostBack(this);
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            ClientScriptManager.RegisterJQueryControl(this);
            base.OnPreRender(e);
        }

        protected string HiddenKey
        {
            get { return ClientID + "_selectedID"; }
        }

        protected virtual void OnActiveViewChanged()
        {
        }

        protected virtual bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            return true;
        }

        protected virtual void RaisePostDataChangedEvent()
        {
            EnsureChildControls();
            SelectedIndex = newIndex;
        }

        #region IPostBackDataHandler Members

        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            return this.LoadPostData(postDataKey, postCollection);
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.RaisePostDataChangedEvent();
        }

        #endregion
    }

}

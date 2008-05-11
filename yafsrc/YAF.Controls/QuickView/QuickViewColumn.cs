using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Security.Permissions;


namespace YAF.Controls.QuickView
{
    // CAS
    [AspNetHostingPermissionAttribute(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermissionAttribute(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    // attributes
    [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public abstract class QuickViewColumn : IStateManager
    {
        private string _header;
        private bool _visible;

        QuickView owner;
        StateBag viewstate;
        bool tracking_viewstate;
        bool design;

        protected QuickViewColumn()
        {
            viewstate = new StateBag();
        }

        [DefaultValue("")]
        public virtual string LocalizedHeader
        {
            get
            {
                return (_header);
            }
            set
            {
                _header = value;
            }
        }


        [DefaultValue(true)]
        public bool Visible
        {
            get
            {
                return (_visible);
            }
            set
            {
                _visible = value;
            }
        }

        public virtual void Initialize()
        {
            if (owner != null && owner.Site != null)
            {
                design = owner.Site.DesignMode;
            }
        }

        public virtual void InitializeControl(Control control)
        {
            throw new NotImplementedException("Method has not been implemented!");
        }


        public override string ToString()
        {
            return (String.Empty);
        }


        protected QuickView Owner
        {
            get
            {
                return (owner);
            }
        }

        internal void Set_Owner(QuickView value)
        {
            owner = value;
        }


        protected StateBag ViewState
        {
            get
            {
                return (viewstate);
            }
        }

        /* There are no events defined for QuickViewColumn, so no
         * idea what this method is supposed to do
         */
        protected virtual void OnColumnChanged()
        {
        }

        void IStateManager.LoadViewState(object savedState)
        {
            LoadViewState(savedState);
        }

        object IStateManager.SaveViewState()
        {
            return (SaveViewState());
        }

        void IStateManager.TrackViewState()
        {
            TrackViewState();
        }

        bool IStateManager.IsTrackingViewState
        {
            get
            {
                return (IsTrackingViewState);
            }
        }

        protected virtual void LoadViewState(object savedState)
        {
            object[] pieces = savedState as object[];
        }

        protected virtual object SaveViewState()
        {
            object[] res = new object[4];

            return (res);
        }

        protected virtual void TrackViewState()
        {
            tracking_viewstate = true;
        }

        protected bool IsTrackingViewState
        {
            get
            {
                return (tracking_viewstate);
            }
        }
    }
}

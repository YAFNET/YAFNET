using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;

namespace YAF.Controls
{
	public class PopupConfirm : System.Web.UI.Control
	{
		protected string _confirmText;
		protected string _targetID;
		protected ModalPopupExtender _popupControlExtender = new ModalPopupExtender();

		public PopupConfirm()	: base()
		{
		}

		public ModalPopupExtender ConfirmExtender
		{
			get { return _popupControlExtender; }
		}

		public void Show()
		{
			ConfirmExtender.Show();
		}

		public void Hide()
		{
			ConfirmExtender.Hide();
		}

        protected override void OnInit(EventArgs e)
        {
            this.BuildPopup();
            base.OnInit(e);
        }

        protected void BuildPopup()
		{
			Panel popupPanel = new Panel();
			popupPanel.ID = "PopUpPanel";

			System.Web.UI.WebControls.Button okButton = new Button();
			okButton.ID = "PopUpOKBtn";
			okButton.Text = "Ok";

			System.Web.UI.WebControls.Button cancelButton = new Button();
			cancelButton.ID = "PopUpCancelBtn";
			cancelButton.Text = "Cancel";

            okButton.Click += new EventHandler(okButton_Click);
			cancelButton.Click += new EventHandler(cancelButton_Click );

			System.Web.UI.HtmlControls.HtmlGenericControl div = new HtmlGenericControl( "div" );
			div.Attributes.Add( "class", "inner" );
			System.Web.UI.HtmlControls.HtmlGenericControl header = new HtmlGenericControl( "h2" );
			header.InnerText = this.Text;
			System.Web.UI.HtmlControls.HtmlGenericControl basediv = new HtmlGenericControl( "div" );
			div.Attributes.Add( "class", "base" );
			div.Controls.Add( header );
			popupPanel.Controls.Add( div );
			popupPanel.Controls.Add( basediv );

			basediv.Controls.Add( okButton );
			basediv.Controls.Add( cancelButton );

			popupPanel.Attributes.Add( "style", "display:none;" );
			popupPanel.CssClass = "confirm-dialog";

			this.Controls.Add( popupPanel );

			ConfirmExtender.TargetControlID = this.TargetID;
			ConfirmExtender.PopupControlID = popupPanel.ID;

			this.Controls.Add( ConfirmExtender );
		}

        void okButton_Click(object sender, EventArgs e)
        {
            if (Confirm != null) Confirm(this, e);
            Hide();
        }

		void cancelButton_Click( object sender, EventArgs e )
		{
            if (Cancel != null) Cancel(this, e);
            Hide();
        }

		public string Text
		{
			get { return _confirmText; }
			set { _confirmText = value; }
		}

		public string TargetID
		{
			get { return _targetID; }
			set { _targetID = value; }
		}

        public event EventHandler<EventArgs> Confirm;
		public event EventHandler<EventArgs> Cancel;
	}
}

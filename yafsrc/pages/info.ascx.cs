using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
  enum InfoMessage
  {
    Moderated = 1,
    Suspended = 2,
    RegistrationEmail = 3,
    AccessDenied = 4,
    Disabled = 5,
    Invalid = 6
  }
  /// <summary>
  /// Summary description for info.
  /// </summary>
  public partial class info : YAF.Classes.Base.ForumPage
  {

    public info()
      : base( "INFO" )
    {
      CheckSuspended = false;
    }

    protected void Page_Load( object sender, System.EventArgs e )
    {
      // Put user code to initialize the page here
      if ( !IsPostBack )
      {
        Continue.NavigateUrl = Request.QueryString ["url"];
        Continue.Text = GetText( "continue" );
        if ( Request.QueryString ["url"] != null )
          RefreshURL = Request.QueryString ["url"];
        else
          Continue.Visible = false;

        try
        {
          switch ( (InfoMessage)int.Parse( Request.QueryString ["i"] ) )
          {
            case InfoMessage.Moderated: /// Moderated
              Title.Text = GetText( "title_moderated" );
              Info.Text = GetText( "moderated" );
              break;
            case InfoMessage.Suspended: /// Suspended
              Title.Text = GetText( "title_suspended" );
              Info.Text = String.Format( GetText( "suspended" ), yaf_DateTime.FormatDateTime( PageContext.SuspendedUntil ) );
              break;
            case InfoMessage.RegistrationEmail: /// Registration email
              Title.Text = GetText( "title_registration" );
              Info.Text = GetText( "registration" );
              RefreshTime = 10;
              RefreshURL = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.login );
              break;
            case InfoMessage.AccessDenied: /// Access Denied
              Title.Text = GetText( "title_accessdenied" );
              Info.Text = GetText( "accessdenied" );
              RefreshTime = 10;
              RefreshURL = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum );
              break;
            case InfoMessage.Disabled: /// Disabled feature
              Title.Text = GetText( "TITLE_ACCESSDENIED" );
              Info.Text = GetText( "DISABLED" );
              RefreshTime = 10;
              RefreshURL = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum );
              break;
            case InfoMessage.Invalid: /// Invalid argument!
              Title.Text = GetText( "TITLE_INVALID" );
              Info.Text = GetText( "INVALID" );
              RefreshTime = 10;
              RefreshURL = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum );
              break;
          }
        }
        catch ( Exception )
        {
          Title.Text = GetText( "title_exception" );
          Info.Text = string.Format( "{1} <b>{0}</b>.", PageContext.PageUserName, GetText( "exception" ) );
          RefreshTime = 2;
          RefreshURL = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum );
        }
        Continue.NavigateUrl = RefreshURL;
        Continue.Visible = RefreshURL != null;
      }
    }

    #region Web Form Designer generated code
    override protected void OnInit( EventArgs e )
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit( e );
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }
    #endregion
  }
}

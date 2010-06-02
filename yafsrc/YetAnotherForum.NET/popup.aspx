<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="YAF.Classes.Core"%>
<%@ Register TagPrefix="YAF" Assembly="YAF" Namespace="YAF" %>
<script runat="server">
	public void Page_Error( object sender, System.EventArgs e )
	{
		Exception x = Server.GetLastError();
		YAF.Classes.Data.DB.eventlog_create(YafServices.InitializeDb.Initialized ? (int?)YafContext.Current.PageUserID : null , this, x );
		YAF.Classes.Core.CreateMail.CreateLogEmail( x );
	}
	public void Page_PreRender( object sender, System.EventArgs e )
	{
		// if this page isn't supposed to be a popup, redirect back to the main forum...
		if ( YafContext.Current != null && YafContext.Current.CurrentForumPage != null && YafContext.Current.CurrentForumPage.AllowAsPopup == false )
		{
			// redirect back to default.aspx page...
			Response.Redirect( String.Format( "{0}?{1}", YAF.Classes.Config.BaseScriptFile, Request.QueryString.ToString() ) );
		}
	}
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="YafHead" runat="server">
<meta id="YafMetaScriptingLanguage" http-equiv="Content-Script-Type" runat="server" name="scriptlanguage" content="text/javascript" />
<meta id="YafMetaStyles" http-equiv="Content-Style-Type" runat="server" name="styles" content="text/css" />
    <title></title>
</head>
<body class="PopupBody">
    <form id="form1" runat="server" enctype="multipart/form-data">
        <YAF:Forum runat="server" ID="forum" Popup="true"></YAF:Forum>
    </form>
</body>
</html>

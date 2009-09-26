<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" %>
<%@ Import Namespace="YAF.Classes.Core"%>
<script runat="server">
	public void Page_Error( object sender, System.EventArgs e )
	{
		Exception x = Server.GetLastError();
		YAF.Classes.Data.DB.eventlog_create(YafServices.InitializeDb.Initialized ? (int?)YafContext.Current.PageUserID : null , this, x );
		YAF.Classes.Core.CreateMail.CreateLogEmail( x );
	}		
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="YafHead" runat="server">
    <meta name="Description" content="Yet Another Forum.NET -- A bulletin board system written in ASP.NET" />
    <meta name="Keywords" content="Yet Another Forum.net, Forum, ASP.NET, BB, Bulletin Board, opensource" />
    <title></title>
</head>
<body style="margin:0; padding:5px">
    <img src="~/images/YAFLogo.jpg" runat="server" alt="YetAnotherForum" id="imgBanner" /><br/>    
    <form id="form1" runat="server" enctype="multipart/form-data">
        <YAF:Forum runat="server" ID="forum"></YAF:Forum>
    </form>
</body>
</html>

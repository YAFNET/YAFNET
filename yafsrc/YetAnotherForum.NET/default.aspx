<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server">
    public void Page_Error( object sender, System.EventArgs e )
    {
        Exception x = Server.GetLastError();
        YAF.Classes.Data.DB.eventlog_create( yafForum.PageUserID, this, x );
        YAF.Classes.Utils.General.LogToMail( x );
    }		
</script>
<html>
<head id="YafHead" runat="server">
    <meta name="Description" content="A bulletin board system written in ASP.NET" />
    <meta name="Keywords" content="Yet Another Forum.net, Forum, ASP.NET, BB, Bulletin Board, opensource" />
    <title>This title is overwritten</title>
</head>
<body>
    <img src="~/images/YAFLogo.jpg" runat="server" alt="YetAnotherForum" id="imgBanner" />
    <br />
    <form id="form1" runat="server" enctype="multipart/form-data">
        <YAF:Forum runat="server" ID="forum"></YAF:Forum>
    </form>
</body>
</html>

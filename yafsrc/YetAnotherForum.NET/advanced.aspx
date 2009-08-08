<%@ Page Language="C#" %>

<script runat="server">
    void Page_Load( object sender, System.EventArgs e )
    {
        yafForum.Header = yafHeader;
        yafForum.Footer = yafFooter;
    }
    public void Page_Error( object sender, System.EventArgs e )
    {
        Exception x = Server.GetLastError();
        YAF.Classes.Data.DB.eventlog_create( yafForum.PageUserID, this, x );
        YAF.Classes.Core.CreateMail.CreateLogEmail( x );
    }
</script>

<html>
<head id="YafHead" runat="server">
    <meta name="Description" content="A bulletin board system written in ASP.NET">
    <meta name="Keywords" content="Yet Another Forum.net, Forum, ASP.NET, BB, Bulletin Board, opensource">
    <!-- If you don't want the forum to set the page title, you can remove runat and id -->
    <title>This title is overwritten</title>
</head>
<body>
    <img src="~/images/YAFLogo.jpg" runat="server" alt="YetAnotherForum" id="imgBanner" />
    <br />
    <form id="Form1" runat="server" enctype="multipart/form-data">
        <table border="0" width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td colspan="3">
                    <YAF:Header runat="server" ID="yafHeader" />
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <YAF:Forum runat="server" ID="yafForum" />
                </td>
                <td width="10">&nbsp;</td>
                <td width="200" valign="top">
                    <br /><br /><br /><br /><br />                   
                    <YAF:MostActiveUsers ID="MostActiveList" runat="server" DisplayNumber="10" />
                    <br />
                    <YAF:ActiveDiscussions ID="ActiveDisccionsList" runat="server" DisplayNumber="10" />
                </td>                
            </tr>
            <tr>
                <td colspan="3">
                    <YAF:Footer runat="server" ID="yafFooter" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>

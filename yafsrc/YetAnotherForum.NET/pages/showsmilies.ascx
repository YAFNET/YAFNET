<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.showsmilies"CodeBehind="showsmilies.ascx.cs" %>
<%@ Import Namespace="YAF.Utils" %>
<script language="javascript" type="text/javascript">
<!--
    // set close timer so this window doesn't stay open forever
    var timeOutID = setTimeout(self.close, 20000);

    function insertsmiley(code, icon) {
        window.opener.insertsmiley(code, icon);
        window.opener.focus();
        // clear and set the close timer for another 20 seconds
        clearTimeout(timeOutID);
        timeOutID = setTimeout(self.close, 20000);
    }
-->
</script>
<asp:Repeater ID="List" runat="server">
    <HeaderTemplate>
        <table cellpadding="0" cellspacing="1" class="content" width="100%">
            <tr>
                <td class="header1" colspan="3" align="center">
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" />
            </tr>
            <tr>
                <td class="header2">
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER_CODE" />
                </td>
                <td class="header2" align="center">
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HEADER_SMILE" />
                </td>
                <td class="header2">
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEADER_MEANING" />
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td class="post">
                <%# DataBinder.Eval(Container.DataItem,"Code") %>
            </td>
            <td class="post" align="center">
                <asp:HyperLink ID="ClickSmiley" NavigateUrl='<%# GetSmileyScript( DataBinder.Eval(Container.DataItem,"Code","{0}"), DataBinder.Eval(Container.DataItem,"Icon","{0}")) %>'
                    ToolTip='<%# DataBinder.Eval(Container.DataItem,"Emoticon") %>' runat="server"><img alt="" src='<%# YafForumInfo.ForumClientFileRoot + YafBoardFolders.Current.Emoticons %>/<%# DataBinder.Eval(Container.DataItem,"Icon") %>'/></asp:HyperLink>
            </td>
            <td class="post">
                <%# DataBinder.Eval(Container.DataItem,"Emoticon") %>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        <tr>
            <td class="footer1" colspan="3" align="center">
                <a href="javascript:window.close();">
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="CLOSE_WINDOW" />
                </a>
            </td>
        </tr>
    </FooterTemplate>
</asp:Repeater>

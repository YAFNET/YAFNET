<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editboard" Codebehind="editboard.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <asp:UpdatePanel ID="UppdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
               <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITBOARD" />
            </td>
        </tr>
        <tr>
	      <td class="header2" height="30" colspan="2"></td>
		</tr>
        <tr>
            <td width="50%" class="postheader">
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_EDITBOARD" />
            </td>
            <td width="50%" class="post">
                <asp:TextBox ID="Name" runat="server" Width="350"></asp:TextBox></td>
        </tr>
              <tr>
            <td width="50%" class="postheader">
                <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="CULTURE" LocalizedPage="ADMIN_EDITBOARD" />
            </td>
            <td width="50%" class="post">
            <asp:DropDownList ID="Culture" runat="server" Width="350" CssClass="standardSelectMenu" />               
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="THREADED" LocalizedPage="ADMIN_EDITBOARD" />
            </td>
            <td class="post">
                <asp:CheckBox runat="server" ID="AllowThreaded" /></td>
        </tr>  
        <asp:PlaceHolder runat="server" ID="BoardMembershipAppNameHolder">                
        <tr>
            <td width="50%" class="postheader">
                <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="MEMBSHIP_APP_NAME" LocalizedPage="ADMIN_EDITBOARD" />
            </td>
            <td width="50%" class="post">
                <asp:TextBox ID="BoardMembershipAppName" runat="server"  Width="350"></asp:TextBox></td>
        </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="CreateNewAdminHolder">
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="ADMIN_USER" LocalizedPage="ADMIN_EDITBOARD" />
            </td>
            <td class="post">
                <asp:CheckBox runat="server" ID="CreateAdminUser" AutoPostBack="true" OnCheckedChanged="CreateAdminUser_CheckedChanged" /></td>
        </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="AdminInfo" Visible="false">
            <tr>
                <td colspan="2" class="header2">
                  <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_EDITBOARD" />
                </td>
            </tr>
            <tr>
                <td class="postheader">
                    <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="USER_NAME" LocalizedPage="ADMIN_EDITBOARD" />
                </td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserName" Width="350" /></td>
            </tr>
            <tr>
                <td class="postheader">
                    <YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="USER_MAIL" LocalizedPage="ADMIN_EDITBOARD" />
                </td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserEmail" Width="350" /></td>
            </tr>
            <tr>
                <td class="postheader">
                    <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="USER_PASS" LocalizedPage="ADMIN_EDITBOARD" />
                </td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserPass1" TextMode="password" Width="350" /></td>
            </tr>
            <tr>
                <td class="postheader">
                    <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="VERIFY_PASS" LocalizedPage="ADMIN_EDITBOARD" />
                </td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserPass2" TextMode="password" Width="350" /></td>
            </tr>
            <tr>
                <td class="postheader">
                    <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="SECURITY_QUESTION" LocalizedPage="ADMIN_EDITBOARD" />
                </td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserPasswordQuestion" Width="350" /></td>
            </tr>
            <tr>
                <td class="postheader">
                    <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="SECURITY_ANSWER" LocalizedPage="ADMIN_EDITBOARD" />
                </td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserPasswordAnswer" Width="350" /></td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td class="postfooter" align="center" colspan="2">
                <asp:Button ID="Save" runat="server" OnClick="Save_Click" CssClass="pbutton" />
                <asp:Button ID="Cancel" runat="server" OnClick="Cancel_Click" CssClass="pbutton" />
            </td>
        </tr>
    </table>
    </ContentTemplate>
    </asp:UpdatePanel>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

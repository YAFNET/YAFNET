<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bbcode" Codebehind="BBCode.ascx.cs" %>
<%@ Import Namespace="YAF.Core.BBCode" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">

		<asp:Repeater ID="bbCodeList" runat="server" OnItemCommand="bbCodeList_ItemCommand">
        <HeaderTemplate>
           	<table class="content" cellspacing="1" cellpadding="0" width="100%">
                <tr>
                    <td class="header1" colspan="4">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BBCODE" />
                    </td>
                </tr>
                <tr>
                    <td class="header2" width="1%">
                      &nbsp;
                    </td>
                    <td class="header2" width="40%">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_BBCODE" />
                    </td>
                    <td class="header2" width="40%">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_BBCODE" />
                    </td>                        
                    <td class="header2">
                        &nbsp;
                    </td>
                </tr>
        	 </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="post">
                    <asp:CheckBox ID="chkSelected" runat="server" />
                    <asp:HiddenField ID="hiddenBBCodeID" runat="server" Value='<%# Eval("BBCodeID") %>' />
                </td>
                <td class="post">
                    <strong><%# Eval("Name") %></strong></td>
                <td class="post">
                    <strong><%# this.Get<IBBCode>().LocalizeCustomBBCodeElement(Eval("Description").ToString())%></strong></td>                    
                <td class="post">
                    <asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval("BBCodeID") %>'
                        ID="Linkbutton1">
                         <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="EDIT" />
                    </asp:LinkButton>
                    |
                    <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete"
                        CommandArgument='<%# Eval("BBCodeID") %>' ID="Linkbutton2">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETE" />
                    </asp:LinkButton>
                </td>
            </tr>
        	 </ItemTemplate>
        <FooterTemplate>
            <tr>
                <td class="footer1" align="center" colspan="4">
                    <asp:Button runat="server" CommandName='add' ID="Linkbutton3" CssClass="pbutton" OnLoad="addLoad"></asp:Button>
                    |
                    <asp:Button runat="server" CommandName='import' ID="Linkbutton5" CssClass="pbutton" OnLoad="importLoad"></asp:Button>
                    |
                    <asp:Button runat="server" CommandName='export' ID="Linkbutton4" CssClass="pbutton" OnLoad="exportLoad"></asp:Button>
                </td>
            </tr>
             </table>
        	 </FooterTemplate>
    	 </asp:Repeater>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

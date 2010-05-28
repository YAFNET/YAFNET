<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bbcode" Codebehind="BBCode.ascx.cs" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
		
		
		
		<asp:Repeater ID="bbCodeList" runat="server" OnItemCommand="bbCodeList_ItemCommand">
        <HeaderTemplate>
           	<table class="content" cellspacing="1" cellpadding="0" width="100%">

                <tr>
                    <td class="header1" colspan="4">
                        <asp:Label ID="ExtensionTitle" runat="server">BBCode Extensions</asp:Label></td>
                </tr>
                <tr>
                    <td class="header2" width="1%">&nbsp;</td>
                    <td class="header2" width="40%">
                        Name</td>
                    <td class="header2" width="40%">
                        Description</td>                        
                    <td class="header2">
                        &nbsp;</td>
                </tr>
        	 </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="post">
                    <asp:CheckBox ID="chkSelected" runat="server" />
                    <asp:HiddenField ID="hiddenBBCodeID" runat="server" Value='<%# Eval("BBCodeID") %>' />
                </td>
                <td class="post">
                    <b><%# Eval("Name") %></b></td>
                <td class="post">
                    <b><%# YAF.Classes.UI.YafBBCode.LocalizeCustomBBCodeElement(Eval("Description").ToString()) %></b></td>                    
                <td class="post">
                    <asp:LinkButton runat="server" Text="Edit" CommandName="edit" CommandArgument='<%# Eval("BBCodeID") %>'
                        ID="Linkbutton1">
                    </asp:LinkButton>
                    |
                    <asp:LinkButton runat="server" Text="Delete" OnLoad="Delete_Load" CommandName="delete"
                        CommandArgument='<%# Eval("BBCodeID") %>' ID="Linkbutton2">
                    </asp:LinkButton>
                </td>
            </tr>
        	 </ItemTemplate>
        <FooterTemplate>
            <tr>
                <td class="footer1" colspan="4">
                    <asp:LinkButton runat="server" Text="Add" CommandName='add' ID="Linkbutton3"></asp:LinkButton>
                    |
                    <asp:LinkButton runat="server" Text="Import from XML" CommandName='import' ID="Linkbutton5"></asp:LinkButton>
                    |
                    <asp:LinkButton runat="server" Text="Export Selected to XML" CommandName='export' ID="Linkbutton4"></asp:LinkButton>
                </td>
            </tr>
             </table>
        	 </FooterTemplate>
    	 </asp:Repeater>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

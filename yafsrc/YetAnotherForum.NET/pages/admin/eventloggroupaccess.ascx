<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.eventloggroupaccess" Codebehind="eventloggroupaccess.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
                    
	<table class="content"  cellspacing="1" cellpadding="0" width="100%">
	     <tr class="header1">
	          <td class="header1" colspan="4">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE"  LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
              </td>
         </tr>
         <tr>
             <td class="header2">
                     <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />&nbsp;
                     <YAF:LocalizedLabel ID="GroupNameLabel" runat="server" LocalizedTag="GROUPNAME" LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />&nbsp;
                     <asp:Label ID="GroupName" runat="server"  />&nbsp;
             </td>
             <td class="header2">
                     <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="EVENT" LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />&nbsp;
             </td>
             <td class="header2">
                     <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="VIEWACCESS"  LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
            </td>
             <td class="header2">
                     <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETEACCESS"  LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
            </td>
        </tr>

	      <asp:Repeater ID="AccessList" OnItemDataBound="AccessList_OnItemDataBound" runat="server">
            <ItemTemplate>
                <tr>
                     <td class="post">
                        <strong>
                           <asp:Label ID="EventText" runat="server" /> 
                        </strong>
                      </td>
                     <td class="post">
                        <strong>
                           <asp:Label ID="EventTypeName" runat="server" /> 
                        </strong>
                    </td>
                      <td class="post">
                      <asp:CheckBox  ID="ViewAccess" runat="server"/>
                    </td>
                    <td class="post">
                      <asp:CheckBox  ID="DeleteAccess" runat="server"/>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
		<tr class="footer1">
			<td style="text-align: center" colspan="4">
				<asp:Button ID="Save" runat="server" OnClick="Save_Click" CssClass="pbutton" />
                <asp:Button ID="GrantAll" runat="server" OnClick="GrantAll_Click" CssClass="pbutton" />
                <asp:Button ID="RevokeAll" runat="server" OnClick="RevokeAll_Click" CssClass="pbutton" />
                <asp:Button ID="GrantAllDelete" runat="server" OnClick="GrantAllDelete_Click" CssClass="pbutton" />
                <asp:Button ID="RevokeAllDelete" runat="server" OnClick="RevokeAllDelete_Click" CssClass="pbutton" />
				<asp:Button ID="Cancel" runat="server" OnClick="Cancel_Click" CausesValidation="false" CssClass="pbutton" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

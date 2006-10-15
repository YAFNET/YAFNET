<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUsersGroups.ascx.cs" Inherits="yaf.controls.EditUsersGroups" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<asp:repeater id="UserGroups" runat="server">
	    <HeaderTemplate>
		    <tr>
			    <td class="header1" colspan="2">User Groups</td>
		    </tr>
		    <tr>
			    <td class="header2">Member</td>
			    <td class="header2">Group</td>
		    </tr>
	    </HeaderTemplate>
	    <ItemTemplate>
		    <tr>
			    <td class="post"><asp:checkbox runat="server" id="GroupMember" checked='<%# IsMember(DataBinder.Eval(Container.DataItem,"Member")) %>'/></td>
			    <td class="post"><asp:label id="GroupID" visible="false" runat="server" text='<%# DataBinder.Eval(Container.DataItem, "GroupID") %>'></asp:label>
				    <b><%# DataBinder.Eval(Container.DataItem, "Name") %></b>
			    </td>
		    </tr>
	    </ItemTemplate>
    </asp:repeater>

	<tr>
		<td class="postfooter" colspan="2" align="center">
			<asp:Button id="Save" runat="server" Text="Save" onclick="Save_Click" />
			<asp:Button id="Cancel" runat="server" Text="Cancel" onclick="Cancel_Click" />
		</td>
	</tr>
	</table>
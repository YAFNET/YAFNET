<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Control language="c#" Codebehind="editforum.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.editforum" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
	<TABLE class="content" cellSpacing="1" cellPadding="0" width="100%">
		<TR>
			<TD class="header1" colSpan="2">Edit Forum:
				<asp:label id="ForumNameTitle" runat="server"></asp:label></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Category:</B><BR>
				What category to put the forum under.</TD>
			<TD class="post">
				<asp:dropdownlist id="CategoryList" runat="server" DataValueField="CategoryID" DataTextField="Name"></asp:dropdownlist></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Parent Forum:</B><BR>
				Will make this forum a sub forum of another forum.</TD>
			<TD class="post">
				<asp:dropdownlist id="ParentList" runat="server"></asp:dropdownlist></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Name:</B><BR>
				The name of the forum.</TD>
			<TD class="post">
				<asp:textbox id="Name" runat="server" cssclass="edit"></asp:textbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Description:</B><BR>
				A description of the forum.</TD>
			<TD class="post">
				<asp:textbox id="Description" runat="server" cssclass="edit"></asp:textbox></TD>
		</TR>
		<tr>
			<td class="postheader"><b>Remote URL:</b><br/>
				Enter a url here, and instead of going to the forum you will be taken to this url instead.</td>
			<td class="post"><asp:textbox id="remoteurl" runat="server" cssclass="edit"/></td>
		</tr>
		<TR>
			<TD class="postheader"><B>SortOrder:</B><BR>
				Sort order under this category.</TD>
			<TD class="post">
				<asp:textbox id="SortOrder" runat="server"></asp:textbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Hide if no access:</B><BR>
				Means that the forum will be hidden when the user don't have read access to it.</TD>
			<TD class="post">
				<asp:checkbox id="HideNoAccess" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Locked:</B><BR>
				If the forum is locked, no one can post or reply in this forum.</TD>
			<TD class="post">
				<asp:checkbox id="Locked" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Is Test:</B><BR>
				If this is checked, posts in this forum will not count in the ladder system.</TD>
			<TD class="post">
				<asp:checkbox id="IsTest" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Moderated:</B><BR>
				If the forum is moderated, posts have to be approved by a moderator.</TD>
			<TD class="post">
				<asp:checkbox id="Moderated" runat="server"></asp:checkbox></TD>
		</TR>
		<TR id="NewGroupRow" runat="server">
			<TD class="postheader"><B>Initial Access Mask:</B><BR>
				The initial access mask for all forums.</TD>
			<TD class="post">
				<asp:dropdownlist id="AccessMaskID" runat="server" ondatabinding="BindData_AccessMaskID"></asp:dropdownlist></TD>
		</TR>
		<asp:repeater id="AccessList" runat="server">
			<HeaderTemplate>
				<tr>
					<td class="header1" colspan="2">Access</td>
				</tr>
				<tr class="header2">
					<td>Group</td>
					<td>Access Mask</td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="postheader">
						<asp:label id=GroupID visible=false runat="server" text='<%# DataBinder.Eval(Container.DataItem, "GroupID") %>'>
						</asp:label>
						<%# DataBinder.Eval(Container.DataItem, "GroupName") %>
					</td>
					<td class="post">
						<asp:dropdownlist runat="server" id="AccessMaskID" ondatabinding="BindData_AccessMaskID" onprerender="SetDropDownIndex" value='<%# DataBinder.Eval(Container.DataItem,"AccessMaskID") %>'/>
						...
					</td>
				</tr>
			</ItemTemplate>
		</asp:repeater>
		<TR>
			<TD class="postfooter" align="center" colSpan="2">
				<asp:button id="Save" runat="server" Text="Save"></asp:button>&nbsp;
				<asp:Button id="Cancel" runat="server" Text="Cancel"></asp:Button></TD>
		</TR>
	</TABLE>
</yaf:adminmenu>
<yaf:savescrollpos runat="server" id="Savescrollpos1" />

<%@ Control Language="c#" AutoEventWireup="True" Codebehind="avatar.ascx.cs" Inherits="YAF.Pages.avatar" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<p class="navlinks"><YAF:pager runat="server" id="pager"/></p>
		<asp:hyperLink id="goup" runat="server" /><br />
		<h2><%= GetText("TITLE") %></h2>
		<asp:datalist runat="server" id="directories" width="100%"
			RepeatColumns="5"
			Border="5"
			onitemdatabound="directories_bind"
			CssClass="content"
			AutoGenerateColumns="False" >
			<ItemStyle CssClass="postheader" Width="20%" />
			<ItemTemplate>
				<asp:hyperlink id="dname" runat="server" />
			</ItemTemplate>
		</asp:datalist>
		
		<asp:datalist runat="server" id="files" width="100%"
			RepeatColumns="5"
			Border="5"
			onitemdatabound="files_bind"
			CssClass="content"
			AllowPaging="True"
			PageSize="10"
			AutoGenerateColumns="False" >
			<ItemStyle CssClass="postheader" Width="20%" />
			<HeaderTemplate>
				<asp:literal id="fhead" runat="server" />
			</HeaderTemplate>
			<ItemTemplate>
				<asp:literal id="fname" runat="server" />
			</ItemTemplate>
		</asp:datalist>

<p class="navlinks"><YAF:pager runat="server" linkedpager="pager"/></p>

<asp:linkbutton runat="server" id="GoDir" visible="false"/>

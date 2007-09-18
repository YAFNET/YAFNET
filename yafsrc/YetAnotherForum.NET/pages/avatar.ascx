<%@ Control Language="c#" AutoEventWireup="True" CodeFile="avatar.ascx.cs" Inherits="YAF.Pages.avatar" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<p class="navlinks">
	<YAF:Pager runat="server" ID="pager" />
</p>
<asp:HyperLink ID="goup" runat="server" /><br />
<h2>
	<%= GetText("TITLE") %>
</h2>
<asp:DataList runat="server" ID="directories" Width="100%" RepeatColumns="5" Border="5"
	OnItemDataBound="directories_bind" CssClass="content" AutoGenerateColumns="False">
	<ItemStyle CssClass="postheader" Width="20%" />
	<ItemTemplate>
		<asp:HyperLink ID="dname" runat="server" />
	</ItemTemplate>
</asp:DataList>
<asp:DataList runat="server" ID="files" Width="100%" RepeatColumns="5" Border="5"
	OnItemDataBound="files_bind" CssClass="content" AllowPaging="True" PageSize="10"
	AutoGenerateColumns="False">
	<ItemStyle CssClass="postheader" Width="20%" />
	<HeaderTemplate>
		<asp:Literal ID="fhead" runat="server" />
	</HeaderTemplate>
	<ItemTemplate>
		<asp:Literal ID="fname" runat="server" />
	</ItemTemplate>
</asp:DataList>
<p class="navlinks">
	<YAF:Pager runat="server" LinkedPager="pager" />
</p>
<asp:LinkButton runat="server" ID="GoDir" Visible="false" />

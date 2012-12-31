<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.help.index" Codebehind="index.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:HelpMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
        <tr>
			<td class="header1">
                <YAF:LocalizedLabel ID="Title" runat="server" LocalizedTag="title" LocalizedPage="HELP_INDEX" />
			</td>
		</tr>
        <asp:Repeater runat="server" ID="HelpList">
			<ItemTemplate>
				<tr>
					<td class="header2">
						<%# Eval("HelpTitle") %>
					</td>
                </tr>
                <tr>
					<td class="post">
						<%# Eval("HelpContent") %>
					</td>
				</tr>
			</ItemTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="SearchHolder" runat="server">
        <tr>
            <td class="header2">
				 <asp:Label id="SubTitle" runat="server" />
			</td>
        </tr>
		<tr>
			<td class="post">
               <asp:Label ID="HelpContent" runat="server" />
			</td>
		</tr>
        <tr>
            <td class="header2">
				<YAF:LocalizedLabel ID="SearchHelpTitle" runat="server" LocalizedTag="searchhelptitle" />
			</td>
        </tr>
		<tr>
			<td class="post">
			    <YAF:LocalizedLabel ID="SearchFor" runat="server" LocalizedTag="searchfor" />&nbsp;
				<asp:TextBox runat="server" ID="search" />&nbsp;
				<asp:Button runat="server" ID="DoSearch" Text="Search" />
			</td>
		</tr>
        </asp:PlaceHolder>
	</table>
</YAF:HelpMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

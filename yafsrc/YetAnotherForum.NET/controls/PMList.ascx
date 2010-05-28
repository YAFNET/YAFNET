<%@ Import Namespace="YAF.Classes.Utils" %>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PMList"
	EnableTheming="true" Codebehind="PMList.ascx.cs" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />

<asp:GridView ID="MessagesView" runat="server" OnRowCreated="MessagesView_RowCreated"
	DataKeyNames="UserPMessageID" Width="99%" GridLines="None" CellSpacing="1" ShowFooter="true"
	AutoGenerateColumns="false" CssClass="content" EmptyDataText='<%#GetLocalizedText("NO_MESSAGES") %>'
	EmptyDataRowStyle-CssClass="post">
	<HeaderStyle CssClass="header2" />
	<RowStyle CssClass="post" />
	<AlternatingRowStyle CssClass="post_alt" />
	<FooterStyle CssClass="footer1" />
	<Columns>
		<asp:TemplateField>
			<HeaderTemplate>
				&nbsp;</HeaderTemplate>
			<ItemTemplate>
				<asp:CheckBox runat="server" ID="ItemCheck" /></ItemTemplate>
			<FooterTemplate>
				<YAF:ThemeButton runat="server" ID="MarkAsRead" CssClass="yafcssbigbutton leftItem"
					TextLocalizedTag="MARK_ALL_ASREAD" OnClick="MarkAsRead_Click" Visible="<%#this.View != PMView.Outbox %>" />
				<YAF:ThemeButton runat="server" ID="ArchiveSelected" CssClass="yafcssbigbutton leftItem"
					TextLocalizedTag="ARCHIVESELECTED" OnClick="ArchiveSelected_Click" Visible="<%#this.View == PMView.Inbox %>" />
				<YAF:ThemeButton runat="server" ID="DeleteSelected" CssClass="yafcssbigbutton leftItem"
					TextLocalizedTag="DELETESELECTED" OnLoad="DeleteSelected_Load" OnClick="DeleteSelected_Click" />
				<YAF:ThemeButton runat="server" ID="ArchiveAll" CssClass="yafcssbigbutton leftItem"
					TextLocalizedTag="ARCHIVEALL" OnLoad="ArchiveAll_Load" OnClick="ArchiveAll_Click" Visible="<%#this.View == PMView.Inbox %>" />
				<YAF:ThemeButton runat="server" ID="DeleteAll" CssClass="yafcssbigbutton leftItem"
					TextLocalizedTag="DELETEALL" OnLoad="DeleteAll_Load" OnClick="DeleteAll_Click" />
			</FooterTemplate>
			<HeaderStyle Width="50px" />
			<ItemStyle Width="50px" HorizontalAlign="Center" />
			<FooterStyle HorizontalAlign="Center" />
		</asp:TemplateField>
		<asp:TemplateField>
			<HeaderTemplate>
				&nbsp;</HeaderTemplate>
			<ItemTemplate>
				<img src="<%# GetImage(Container.DataItem) %>" alt="" />
			</ItemTemplate>
			<ItemStyle HorizontalAlign="Center" />
		</asp:TemplateField>
		<asp:TemplateField>
			<HeaderTemplate>
				<asp:Image runat="server" ID="SortFrom" AlternateText="Sort From" />
				<asp:LinkButton runat="server" ID="FromLink" OnClick="FromLink_Click" Text='<%#GetMessageUserHeader() %>' />
			</HeaderTemplate>
			<ItemTemplate>
				<YAF:UserLink ID="UserLink1" runat="server" UserID='<%# Convert.ToInt32(( View == PMView.Outbox ) ? Eval("ToUserID") : Eval("FromUserID" )) %>' />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<HeaderTemplate>
				<asp:Image runat="server" ID="SortSubject" AlternateText="Sort Subject" />
				<asp:LinkButton runat="server" ID="SubjectLink" OnClick="SubjectLink_Click" Text='<%#GetLocalizedText("SUBJECT") %>' />
			</HeaderTemplate>
			<HeaderStyle Width="60%" />
			<ItemTemplate>
				<a href='<%# GetMessageLink(Eval("UserPMessageID")) %>'>
					<%# HtmlEncode(Eval("Subject")) %>
				</a>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<HeaderTemplate>
				<asp:Image runat="server" ID="SortDate" AlternateText="Sort Date" />
				<asp:LinkButton runat="server" ID="DateLink" OnClick="DateLink_Click" Text='<%#GetLocalizedText("DATE") %>' />
			</HeaderTemplate>
			<ItemTemplate>
				<%# YafServices.DateTime.FormatDateTime((DateTime)Eval("Created"))%>
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</asp:GridView>
 <table class="content" cellspacing="1" cellpadding="0" width="99%">
            <tr class="postheader">
                <td class="post">
                  <asp:Label ID="PMInfoLink" runat="server" ></asp:Label>
                </td>
            </tr>
  </table>
<YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />

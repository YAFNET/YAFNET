<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PMList" EnableTheming="true" Codebehind="PMList.ascx.cs" EnableViewState="true" %>

<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Types.Constants" %>

<YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />

<YAF:Alert runat="server" ID="MobileInfo" Type="info" MobileOnly="True">
    <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
    <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
</YAF:Alert>
<div class="table-responsive">
<asp:GridView ID="MessagesView" runat="server" OnRowCreated="MessagesView_RowCreated"
              DataKeyNames="UserPMessageID" GridLines="None" ShowFooter="true"
	          AutoGenerateColumns="false" CssClass="table table-striped" 
              EmptyDataText='<%# this.GetLocalizedText("NO_MESSAGES", null) %>'>
	<Columns>
		<asp:TemplateField>
			<HeaderTemplate>
				&nbsp;</HeaderTemplate>
			<ItemTemplate>
				<asp:CheckBox runat="server" ID="ItemCheck" 
                              Text="&nbsp;"
                              CssClass="custom-control custom-checkbox"/></ItemTemplate>
			<FooterTemplate>
                <asp:UpdatePanel ID="upPanExport" runat="server">
                <ContentTemplate>
                    <div class="btn-group">
                  <YAF:ThemeButton runat="server" ID="MarkAsRead" Size="Small"
					TextLocalizedTag="MARK_ALL_ASREAD" OnClick="MarkAsRead_Click"
                                   Type="Secondary" Icon="eye"/>
				  <YAF:ThemeButton runat="server" ID="ArchiveSelected" Size="Small"
					TextLocalizedTag="ARCHIVESELECTED" OnClick="ArchiveSelected_Click"
                                   Type="Secondary" Icon="archive" />
                     <YAF:ThemeButton runat="server" ID="ExportSelected" Size="Small"
					TextLocalizedTag="EXPORTSELECTED" OnClick="ExportSelected_Click" OnLoad="ExportAll_Load"
                                      Type="Secondary" Icon="file-export" />
				  <YAF:ThemeButton runat="server" ID="DeleteSelected" 
                                   Size="Small"
                                   TextLocalizedTag="DELETESELECTED" 
                                   ReturnConfirmText='<%#this.GetText("CONFIRM_DELETE") %>'
                                   OnClick="DeleteSelected_Click"
                                   Type="Secondary" 
                                   Icon="trash" />
				  <YAF:ThemeButton runat="server" ID="ArchiveAll" 
                                   Size="Small"
                                   TextLocalizedTag="ARCHIVEALL" 
                                   ReturnConfirmText='<%#this.GetText("CONFIRM_ARCHIVEALL") %>'
                                   OnClick="ArchiveAll_Click"
                                   Type="Secondary" Icon="archive" />
                  <YAF:ThemeButton runat="server" ID="ExportAll" Size="Small"
					TextLocalizedTag="EXPORTALL" OnClick="ExportAll_Click" OnLoad="ExportAll_Load"
                                   Type="Secondary" Icon="file-export" />
				  <YAF:ThemeButton runat="server" ID="DeleteAll" 
                                   Size="Small"
                                   TextLocalizedTag="DELETEALL" 
                                   ReturnConfirmText='<%#this.GetText("CONFIRM_DELETEALL") %>'
                                   OnClick="DeleteAll_Click"
                                   Type="Secondary" 
                                   Icon="trash" />
                    </div>
                </ContentTemplate> 
                <Triggers>
                   <asp:PostBackTrigger ControlID="ExportSelected" />
                   <asp:PostBackTrigger ControlID="ExportAll" />
                </Triggers>
              </asp:UpdatePanel>
			</FooterTemplate>
			<HeaderStyle Width="20px" />
			<ItemStyle Width="20px" HorizontalAlign="Center" />
			<FooterStyle HorizontalAlign="Center" />
		</asp:TemplateField>
		<asp:TemplateField>
			<HeaderTemplate>
				&nbsp;</HeaderTemplate>
			<ItemTemplate>
				<%# this.GetIcon(Container.DataItem) %>
			</ItemTemplate>
            <HeaderStyle Width="40px" />
			<ItemStyle Width="40px" HorizontalAlign="Center" />
		</asp:TemplateField>
		<asp:TemplateField>
			<HeaderTemplate>
				<asp:Label runat="server" ID="SortFrom" />
				<asp:LinkButton runat="server" ID="FromLink" OnClick="FromLink_Click" Text='<%#
    this.GetMessageUserHeader() %>' />
			</HeaderTemplate>
            <HeaderStyle Width="7%" />
			<ItemTemplate>
				<YAF:UserLink ID="UserLink1" runat="server" UserID='<%# (this.View == PmView.Outbox ? this.Eval("ToUserID") : this.Eval("FromUserID" )).ToType<int>() %>' />
			</ItemTemplate>
            <ItemStyle Width="7%" HorizontalAlign="Center" />
		</asp:TemplateField>
		<asp:TemplateField>
			<HeaderTemplate>
				<asp:Label runat="server" ID="SortSubject" />
				<asp:LinkButton runat="server" ID="SubjectLink" OnClick="SubjectLink_Click" Text='<%#
    this.GetLocalizedText("SUBJECT", null) %>' />
			</HeaderTemplate>
			<HeaderStyle Width="60%" HorizontalAlign="Left" />
			<ItemTemplate>
				<a href='<%#
    this.GetMessageLink(this.Eval("UserPMessageID")) %>'>
					<%# this.HtmlEncode(this.Eval("Subject")) %>
				</a>
			</ItemTemplate>
            <ItemStyle HorizontalAlign="Left" />
		</asp:TemplateField>
		<asp:TemplateField>
			<HeaderTemplate>
				<asp:Label runat="server" ID="SortDate" />
				<asp:LinkButton runat="server" ID="DateLink" OnClick="DateLink_Click" Text='<%#
    this.GetLocalizedText("DATE", null) %>' />
			</HeaderTemplate>
            <HeaderStyle HorizontalAlign="Left" />
			<ItemTemplate>
                <YAF:DisplayDateTime ID="PostedDateTime" runat="server" DateTime='<%# Container.DataItemToField<DateTime>("Created") %>'></YAF:DisplayDateTime>
			</ItemTemplate>
            <ItemStyle HorizontalAlign="Left" />
		</asp:TemplateField>
	</Columns>
</asp:GridView>
</div>
<YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
<hr />
<asp:Label id="lblExportType" runat="server"></asp:Label>
<div class="custom-control custom-radio custom-control-inline">
    <asp:RadioButtonList runat="server" id="ExportType" 
                         RepeatLayout="UnorderedList"
                         CssClass="list-unstyled">
        <asp:ListItem Text="XML" Selected="True" Value="xml"></asp:ListItem>
        <asp:ListItem Text="CSV" Value="csv"></asp:ListItem>
        <asp:ListItem Text="Text" Value="txt"></asp:ListItem>
    </asp:RadioButtonList>
</div>
</div>

<div class="card-footer">
    <small class="text-muted"><asp:Label ID="PMInfoLink" runat="server" ></asp:Label></small>
</div>
<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.bannedemail" Codebehind="bannedemail.ascx.cs" %>

<%@ Import Namespace="YAF.Core"%>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDEMAIL" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3 card-info-outline">
                <div class="card-header card-info">
                     <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="SEARCH" LocalizedPage="TOOLBAR" />
                </div>
                <div class="card-block">
                    <h4>
                        <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="MASK" LocalizedPage="ADMIN_BANNEDIP" />
                    </h4>
                    <p>
                        <asp:TextBox ID="SearchInput" runat="server" Width="90%" CssClass="form-control"></asp:TextBox>
                    </p>
                </div>
                <div class="card-footer text-lg-center">
                    <YAF:ThemeButton ID="search" runat="server"  CssClass="btn btn-primary btn-sm"
                        TextLocalizedTag="BTNSEARCH" TextLocalizedPage="SEARCH" Icon="search"
                        OnClick="Search_Click">
                    </YAF:ThemeButton>
                </div>
            </div>
            <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
            <div class="card mb-3 card-outline-primary">
                <div class="card-header card-primary">
                    <i class="fa fa-hand-stop-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDEMAIL" />
                </div>
                <div class="card-block">

		<asp:Repeater ID="list" runat="server" OnItemCommand="List_ItemCommand">
		<HeaderTemplate>
				<div class="alert alert-info hidden-sm-up" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
                        </div>
                <div class="table-responsive">
				<table class="table">
                 <tr>
                        <thead>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MASK" LocalizedPage="ADMIN_BANNEDEMAIL" />
                    </th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="SINCE" LocalizedPage="ADMIN_BANNEDEMAIL" />
                    </th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="REASON" LocalizedPage="ADMIN_BANNEDEMAIL" />
                    </th>
					<th>&nbsp;
						</th>
                    </thead>
				</tr>
			</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td>
				<asp:HiddenField ID="fID" Value='<%# this.Eval("ID") %>' runat="server"/>
				<asp:Label ID="MaskBox" Text='<%# this.Eval("Mask") %>' runat="server"></asp:Label>
				</td>
				<td>
					<%# this.Get<IDateTime>().FormatDateTime(this.Eval("Since")) %>
				</td>
				<td>
					<%# this.Eval("Reason") %>
				</td>
				<td>
					    <span class="pull-right">
				<YAF:ThemeButton ID="ThemeButtonEdit" CssClass="btn btn-info btn-sm" CommandName='edit' CommandArgument='<%# this.Eval("ID") %>'
                    TextLocalizedTag="EDIT"
                    TitleLocalizedTag="EDIT" Icon="edit" runat="server"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" CssClass="btn btn-danger btn-sm" CommandName='delete' CommandArgument='<%# this.Eval("ID") %>'
                        TextLocalizedTag="DELETE"
                    TitleLocalizedTag="DELETE" Icon="trash" runat="server"></YAF:ThemeButton>

					    </span>
                    </td>
			</tr>
			</ItemTemplate>
		<FooterTemplate>
                </table></div>
                </div>
                <div class="card-footer text-lg-center">
					<asp:LinkButton runat="server" OnLoad="Import_Load" CommandName='import' CssClass="btn btn-primary"></asp:LinkButton>
                    &nbsp;
                    <asp:LinkButton runat="server" OnLoad="Add_Load" CommandName='add' CssClass="btn btn-info"></asp:LinkButton>
                    &nbsp;
					<asp:LinkButton runat="server" CommandName='export' ID="Linkbutton4" CssClass="btn btn-warning" OnLoad="ExportLoad"></asp:LinkButton>
                </div>
            </div>

			</FooterTemplate>
		</asp:Repeater>
	 <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
                            </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

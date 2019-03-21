<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.bannedname" Codebehind="bannedname.ascx.cs" %>

<%@ Import Namespace="YAF.Core"%>
<%@ Import Namespace="YAF.Types.Interfaces" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/BannedNameImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/BannedNameEdit.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDNAME" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                     <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="SEARCH" LocalizedPage="TOOLBAR" />
                </div>
                <div class="card-body">
                    <h4>
                        <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="MASK" LocalizedPage="ADMIN_BANNEDIP" />
                    </h4>
                    <p>
                        <asp:TextBox ID="SearchInput" runat="server" Width="90%" CssClass="form-control"></asp:TextBox>
                    </p>
                </div>
                <div class="card-footer text-lg-center">
                    <YAF:ThemeButton ID="search" runat="server"  Type="Primary" Size="Small"
                        TextLocalizedTag="BTNSEARCH" TextLocalizedPage="SEARCH" Icon="search"
                        OnClick="Search_Click">
                    </YAF:ThemeButton>
                </div>
            </div>
             <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-hand-paper fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDNAME" />
                </div>
                <div class="card-body">
    <asp:Repeater ID="list" runat="server" OnItemCommand="List_ItemCommand">
		<HeaderTemplate>
            <YAF:Alert runat="server" ID="Alert2" Type="info" MobileOnly="True">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server"
                                    LocalizedTag="TABLE_RESPONSIVE" 
                                    LocalizedPage="ADMIN_COMMON" />
                <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
            </YAF:Alert>
            <div class="table-responsive">
				<table class="table">
                    <tr>
                        <thead>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MASK" LocalizedPage="ADMIN_BANNEDNAME" />
                    </th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="SINCE" LocalizedPage="ADMIN_BANNEDNAME" />
                    </th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="REASON" LocalizedPage="ADMIN_BANNEDNAME" />
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
					    <span class="float-right">
				<YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" Size="Small" CommandName='edit' CommandArgument='<%# this.Eval("ID") %>'
                    TextLocalizedTag="EDIT"
                    TitleLocalizedTag="EDIT" Icon="edit" runat="server"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" Size="Small" CommandName='delete' CommandArgument='<%# this.Eval("ID") %>'
                    TextLocalizedTag="DELETE" ReturnConfirmText='<%# this.GetText("ADMIN_BANNEDIP", "MSG_DELETE") %>'
                    TitleLocalizedTag="DELETE" Icon="trash" runat="server"></YAF:ThemeButton>

					    </span>
                    </td>
			</tr>
			</ItemTemplate>
		<FooterTemplate>
                </table></div>
                </div>
                <div class="card-footer text-lg-center">
                    <YAF:ThemeButton runat="server" Icon="plus-square" Type="Primary"
                                     TextLocalizedTag="ADD_IP" TextLocalizedPage="ADMIN_BANNEDNAME" CommandName="add"></YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton runat="server" Icon="upload" DataTarget="ImportDialog" Type="Info"
                                     TextLocalizedTag="IMPORT_IPS" TextLocalizedPage="ADMIN_BANNEDNAME"></YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton runat="server" CommandName='export' ID="Linkbutton4" 
                                     Type="Warning" Icon="download" TextLocalizedPage="ADMIN_BANNEDIP" TextLocalizedTag="EXPORT"></YAF:ThemeButton>
                </div>
            </div>

			</FooterTemplate>
		</asp:Repeater>
	 <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
                            </div>
    </div>



<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />

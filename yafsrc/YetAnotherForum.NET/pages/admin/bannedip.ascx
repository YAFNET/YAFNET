<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.bannedip" Codebehind="bannedip.ascx.cs" %>

<%@ Import Namespace="YAF.Core"%>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/BannedIpImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/BannedIpEdit.ascx" %>



<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDIP" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                     <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SEARCH" LocalizedPage="TOOLBAR" />
                </div>
                <div class="card-body">
                     
                        <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="MASK" LocalizedPage="ADMIN_BANNEDIP" />
                     
                    <p>
                        <asp:TextBox ID="SearchInput" runat="server" Width="90%" CssClass="form-control"></asp:TextBox>
                    </p>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="search" runat="server"  Type="Primary" Size="Small"
                        TextLocalizedTag="BTNSEARCH" TextLocalizedPage="SEARCH" Icon="search"
                        OnClick="Search_Click">
                    </YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
<div class="row">
    <div class="col-xl-12">
            <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-hand-paper fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDIP" />
                </div>
                <div class="card-body">

		<asp:Repeater ID="list" runat="server" OnItemCommand="List_ItemCommand">
		<HeaderTemplate>
            <ul class="list-group">
			</HeaderTemplate>
		<ItemTemplate>
            <li class="list-group-item list-group-item-action">
                <div class="d-flex w-100 justify-content-between">
                    <asp:HiddenField ID="fID" Value='<%# this.Eval("ID") %>' runat="server"/>
                    <h5 class="mb-1 text-break">
                        <asp:HyperLink runat="server" ID="Mask" 
                                       Href='<%# string.Format(this.Get<YafBoardSettings>().IPInfoPageURL, IPHelper.GetIp4Address(this.Eval("Mask").ToString())) %>'
                                       ToolTip='<%#this.GetText("COMMON", "TT_IPDETAILS") %>'
                                       Target="_blank">
                            <%# this.HtmlEncode(IPHelper.GetIp4Address(this.Eval("Mask").ToString())) %>
                        </asp:HyperLink>
                    </h5>
                    <small class="d-none d-md-block">
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="SINCE" LocalizedPage="ADMIN_BANNEDIP" />
                        </span>
                        <%# this.Get<IDateTime>().FormatDateTime(this.Eval("Since")) %>
                        
                    </small>
                </div>
                <p class="mb-1">
                    <span class="font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="REASON" LocalizedPage="ADMIN_BANNEDIP" />
                    </span>
                    <%# this.Eval("Reason") %>
                </p>
                <p class="mb-1">
                    <span class="font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="BAN_BY" LocalizedPage="ADMIN_BANNEDIP" />
                    </span>
                    <YAF:UserLink ID="UserLink1" runat="server" 
                                  UserID='<%# this.Eval("UserID").ToString().IsNotSet() ? -1 : this.Eval("UserID").ToType<int>() %>' />
                </p>
                <small>
                    <YAF:ThemeButton ID="ThemeButtonEdit" 
                                     Type="Info" Size="Small" 
                                     CommandName='edit' CommandArgument='<%# this.Eval("ID") %>'
                                     TextLocalizedTag="EDIT"
                                     TitleLocalizedTag="EDIT" 
                                     Icon="edit" runat="server"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" 
                                     Type="Danger" Size="Small" 
                                     CommandName='delete' CommandArgument='<%# this.Eval("ID") %>'
                                     TextLocalizedTag="DELETE" 
                                     ReturnConfirmText='<%# this.GetText("ADMIN_BANNEDIP", "MSG_DELETE") %>'
                                     TitleLocalizedTag="DELETE" Icon="trash" runat="server"></YAF:ThemeButton>
                </small>
            </li>
			</ItemTemplate>
		<FooterTemplate>
                </ul>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton runat="server" 
                                     Icon="plus-square" 
                                     Type="Primary"
                                     CssClass="mt-1"
                                     TextLocalizedTag="ADD_IP" TextLocalizedPage="ADMIN_BANNEDIP" 
                                     CommandName="add"></YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton runat="server" 
                                     Icon="upload" 
                                     DataToggle="modal" 
                                     DataTarget="ImportDialog" 
                                     Type="Info"
                                     CssClass="mt-1"
                                     TextLocalizedTag="IMPORT_IPS" TextLocalizedPage="ADMIN_BANNEDIP"></YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton runat="server" 
                                     CommandName='export' 
                                     ID="Linkbutton4" 
                                     Type="Warning" 
                                     Icon="download" 
                                     CssClass="mt-1"
                                     TextLocalizedPage="ADMIN_BANNEDIP" TextLocalizedTag="EXPORT"></YAF:ThemeButton>
                </div>
            </div>

			</FooterTemplate>
		</asp:Repeater>
	 <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
            </div>
        </div>
    </div>
</div>


<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />
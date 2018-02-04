<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpforums"
    CodeBehind="nntpforums.ascx.cs" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/NntpForumEdit.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPFORUMS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-newspaper fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </div>
                <div class="card-body">
                    <asp:Repeater ID="RankList" OnItemCommand="RankListItemCommand" runat="server">
            <HeaderTemplate>
                <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                <table class="table">
                <tr>
                    <thead>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="Server" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="Group" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="Forum" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="Active" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </th>
                    <th>
                        &nbsp;
                    </th>
                    </thead>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# this.Eval( "Name") %>
                    </td>
                    <td>
                        <%# this.Eval( "GroupName") %>
                    </td>
                    <td>
                        <%# this.Eval( "ForumName") %>
                    </td>
                    <td>
                        <%# this.Eval( "Active") %>
                    </td>
                    <td>
					    <span class="float-right">
                        <YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" CssClass="btn-sm" runat="server"
                                         CommandName='edit' CommandArgument='<%# this.Eval("NntpForumID") %>'
                                         Icon="edit" TextLocalizedTag="EDIT">
					    </YAF:ThemeButton>&nbsp;
                        <YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm" runat="server"
                                         CommandName='delete' CommandArgument='<%# this.Eval("NntpForumID") %>'
                                         Icon="trash" TextLocalizedTag="DELETE"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_NNTPFORUMS", "DELETE_FORUM") %>'>
                        </YAF:ThemeButton>

					    </span>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></table></div></FooterTemplate>
        </asp:Repeater>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="NewForum" runat="server" Type="Primary" OnClick="NewForumClick"
                                     Icon="plus-square" TextLocalizedTag="NEW_FORUM" TextLocalizedPage="ADMIN_NNTPFORUMS" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

<modal:Edit ID="EditDialog" runat="server" />
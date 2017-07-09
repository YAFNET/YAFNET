<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpforums"
    CodeBehind="nntpforums.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPFORUMS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3 card-outline-primary">
                <div class="card-header card-primary">
                    <i class="fa fa-newspaper-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </div>
                <div class="card-block">
                    <asp:Repeater ID="RankList" OnItemCommand="RankList_ItemCommand" runat="server">
            <HeaderTemplate>
                <div class="alert alert-info hidden-sm-up" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
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
					    <span class="pull-right">
                        <asp:LinkButton ID="ThemeButtonEdit" CssClass="btn btn-info btn-sm" runat="server"
                            CommandName='edit' CommandArgument='<%# this.Eval("NntpForumID") %>'>
                            <i class="fa fa-edit fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="EDIT"
                            ></YAF:LocalizedLabel>
					    </asp:LinkButton>&nbsp;
                        <asp:LinkButton ID="ThemeButtonDelete" CssClass="btn btn-danger btn-sm" OnLoad="Delete_Load"   runat="server"
                                    CommandName='delete' CommandArgument='<%# this.Eval("NntpForumID") %>'>
                                    <i class="fa fa-trash fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DELETE"></YAF:LocalizedLabel>

                                </asp:LinkButton>

					    </span>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></table></div></FooterTemplate>
        </asp:Repeater>
                </div>
                <div class="card-footer text-center">
                    <asp:LinkButton ID="NewForum" runat="server" CssClass="btn btn-primary" OnClick="NewForum_Click" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

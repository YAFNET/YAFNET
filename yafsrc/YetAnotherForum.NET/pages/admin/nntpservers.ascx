<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpservers"
    CodeBehind="nntpservers.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPSERVERS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-newspaper-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPSERVERS" />
                </div>
                <div class="card-body">
                    <asp:Repeater ID="RankList" runat="server">
            <HeaderTemplate>
                <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                <table class="table">
                <thead>
                <tr>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_NNTPSERVERS" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ADRESS" LocalizedPage="ADMIN_NNTPSERVERS" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="USERNAME" LocalizedPage="ADMIN_NNTPSERVERS" />
                    </th>
                    <th>
                        &nbsp;
                    </th>
                </tr>
                    </thead>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# this.Eval("Name") %>
                    </td>
                    <td>
                        <%# this.Eval("Address") %>
                    </td>
                    <td>
                        <%# this.Eval("UserName") %>
                    </td>
                    <td>
					    <span class="pull-right">
                        <asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# this.Eval( "NntpServerID") %>' CssClass="btn btn-info btn-sm">
                            <i class="fa fa-edit fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="EDIT" LocalizedPage="ADMIN_NNTPFORUMS" />
                        </asp:LinkButton>
                        &nbsp;
                        <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# this.Eval( "NntpServerID") %>' CssClass="btn btn-danger btn-sm">
                            <i class="fa fa-trash fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DELETE" LocalizedPage="ADMIN_NNTPFORUMS" />
                        </asp:LinkButton>
					    </span>
                    </td>
                </tr>
            </ItemTemplate>
                        <FooterTemplate></table></div></FooterTemplate>
        </asp:Repeater>
                    </div>
                    <div class="card-footer text-center">
                    <asp:LinkButton ID="NewServer" runat="server" CssClass="btn btn-primary" OnClick="NewServer_Click" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

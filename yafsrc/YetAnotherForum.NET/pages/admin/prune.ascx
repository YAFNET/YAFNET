<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.prune" CodeBehind="prune.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_PRUNE" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card card-primary-outline">
                <div class="card-header card-primary">
                    <i class="fa fa-trash fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_PRUNE" />
                </div>
                <div class="card-block">
                    <asp:Label ID="lblPruneInfo" runat="server"></asp:Label>
            <h4>
                <YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="PRUNE_FORUM" LocalizedPage="ADMIN_PRUNE" />
            </h4>
            <p>
                <asp:DropDownList ID="forumlist" runat="server" CssClass="custom-select">
                </asp:DropDownList>
            </p>
            <hr />
            <h4>
                <YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="PRUNE_DAYS" LocalizedPage="ADMIN_PRUNE" />
            </h4>
            <p>
                <asp:TextBox ID="days" runat="server" CssClass="form-control DaysInput" TextMode="Number"></asp:TextBox>
            </p>
            <hr />
            <h4>
                <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="PRUNE_PERMANENT" LocalizedPage="ADMIN_PRUNE" />
            </h4>
            <p>
                <asp:CheckBox ID="permDeleteChkBox" runat="server" CssClass="form-control" />
            </p>
                </div>
                <div class="card-footer text-lg-center">
                <asp:LinkButton ID="commit" runat="server" class="btn btn-primary" OnClick="commit_Click" OnLoad="PruneButton_Load"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

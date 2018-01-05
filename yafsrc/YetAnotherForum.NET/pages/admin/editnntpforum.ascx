<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editnntpforum"
    CodeBehind="editnntpforum.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITNNTPFORUM" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-newspaper fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                </div>
                <div class="card-body">
                    <h4>
                        <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SERVER" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                    </h4>
                    <p>
                        <asp:DropDownList ID="NntpServerID" runat="server" CssClass="custom-select" />
                    </p>
                    <hr />
                    <h4>
                        <YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="GROUP" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                    </h4>
                    <p>
                        <asp:TextBox ID="GroupName" runat="server"  CssClass="form-control" />
                    </p>
                    <hr />
                    <h4>
                        <YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="FORUM" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                    </h4>
                    <p>
                        <asp:DropDownList ID="ForumID" runat="server"  CssClass="custom-select" />
                    </p>
                    <hr />
                    <h4>
                        <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="DATECUTOFF" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                    </h4>
                    <p>
                        <asp:TextBox  CssClass="form-control" ID="DateCutOff" runat="server" Enabled="true" TextMode="DateTime" />
                    </p>
                    <hr />
                    <h4>
                        <YAF:HelpLabel ID="LocalizedLabel5" runat="server" LocalizedTag="ACTIVE" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                    </h4>
                    <p>
                        <asp:CheckBox ID="Active" runat="server" Checked="true" CssClass="form-control" />
                    </p>
                </div>
                <div class="card-footer text-lg-center">
                    <asp:LinkButton ID="Save" runat="server" CssClass="btn btn-primary" OnClick="Save_Click" />&nbsp;
                    <asp:LinkButton ID="Cancel" runat="server" CssClass="btn btn-secondary" OnClick="Cancel_Click" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

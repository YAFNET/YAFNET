<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.bannedname_edit" Codebehind="bannedname_edit.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDNAME_EDIT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3 card-outline-primary">
                <div class="card-header card-primary">
                    <i class="fa fa-hand-stop-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDNAME_EDIT" />
                </div>
                <div class="card-block">
			<h4>
				<YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="MASK"  LocalizedPage="ADMIN_BANNEDNAME_EDIT" />
            </h4>
			<p>
				<asp:TextBox CssClass="form-control" ID="mask" runat="server" MaxLength="255"></asp:TextBox></p><hr />
			<h4>
				<YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="REASON" LocalizedPage="ADMIN_BANNEDNAME_EDIT" />
            </h4>
			<p>
				<asp:TextBox CssClass="form-control" ID="BanReason" runat="server" MaxLength="128"></asp:TextBox></p>
                </div>
                <div class="card-footer text-lg-center">
				    <asp:LinkButton ID="save" runat="server" OnClick="Save_Click" CssClass="btn btn-primary"></asp:LinkButton>
				    <asp:LinkButton ID="cancel" runat="server" OnClick="Cancel_Click" CssClass="btn btn-secondary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

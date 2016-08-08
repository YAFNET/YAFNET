<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.replacewords_edit" Codebehind="replacewords_edit.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_REPLACEWORDS_EDIT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card card-primary-outline">
                <div class="card-header card-primary">
                    <i class="fa fa-sticky-note fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_REPLACEWORDS_EDIT" />
                </div>
                <div class="card-block">
			<h4>
              <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="BAD" LocalizedPage="ADMIN_REPLACEWORDS_EDIT" />
            </h4>
			<p>
			  <asp:TextBox ID="badword" runat="server" CssClass="form-control"></asp:TextBox>
            </p>
            <hr />
			<h4>
			  <YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="GOOD" LocalizedPage="ADMIN_REPLACEWORDS_EDIT" />
            </h4>
			<p>
			  <asp:TextBox ID="goodword" runat="server" CssClass="form-control"></asp:TextBox>
            </p>
                </div>
                <div class="card-footer text-lg-center">
			  <asp:LinkButton ID="save" runat="server" CssClass="btn btn-primary"></asp:LinkButton>
			  <asp:LinkButton ID="cancel" runat="server" CssClass="btn btn-secondary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

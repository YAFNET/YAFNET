<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.spamwords_edit" Codebehind="spamwords_edit.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMWORDS_EDIT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card card-primary-outline">
                <div class="card-header card-primary">
                    <i class="fa fa-shield fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMWORDS_EDIT" />
                </div>
                <div class="card-block">
                    <h4>
                        <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SPAM" LocalizedPage="ADMIN_SPAMWORDS_EDIT" />
                    </h4>
			       <p>
                       <asp:TextBox ID="spamword" runat="server" CssClass="form-control"></asp:TextBox>
                   </p>
                </div>
                <div class="card-footer text-lg-center">
                    <asp:LinkButton ID="save" runat="server" CssClass="btn btn-primary"></asp:LinkButton>&nbsp;
                    <asp:LinkButton ID="cancel" runat="server" CssClass="btn btn-secondary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

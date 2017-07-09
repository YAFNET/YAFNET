<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.pm" Codebehind="pm.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_PM" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3 card-outline-primary">
                <div class="card-header card-primary">
                    <i class="fa fa-envelope-square fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_PM" />
                </div>
                <div class="card-block">
			<h4>
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="PM_NUMBER" LocalizedPage="ADMIN_PM" />
                &nbsp;<small><asp:Label runat="server" ID="Count" /></small>
            </h4>
            <hr />
			<h4>
                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="DELETE_READ" LocalizedPage="ADMIN_PM" />
                </h4>
			<p>
				<asp:TextBox runat="server" ID="Days1" CssClass="form-control DaysInput" TextMode="Number" />
           </p>
			<h4>
                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="DELETE_UNREAD" LocalizedPage="ADMIN_PM" />
            </h4>
			<p>
				<asp:TextBox runat="server" ID="Days2" CssClass="form-control DaysInput" TextMode="Number" />
            </p>
                </div>
                <div class="card-footer text-lg-center">
				    <asp:LinkButton ID="commit" CssClass="btn btn-primary" runat="server" OnLoad="DeleteButton_Load" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

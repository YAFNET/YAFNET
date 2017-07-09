<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.mail" Codebehind="mail.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_MAIL" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3 card-outline-primary">
                <div class="card-header card-primary">
                    <i class="fa fa-envelope-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_MAIL" />
			     </div>
                <div class="card-block">
                    <h4>
			  <strong><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="MAIL_TO" LocalizedPage="ADMIN_MAIL" /></strong>
            </h4>
			<p>
			  <asp:DropDownList ID="ToList" runat="server" DataValueField="GroupID" DataTextField="Name" CssClass="custom-select">
                </asp:DropDownList>
            </p>
            <hr />
			<h4>
			  <strong><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="MAIL_SUBJECT" LocalizedPage="ADMIN_MAIL" /></strong>
            </h4>
			<p>
			  <asp:TextBox ID="Subject" runat="server" CssClass="form-control"></asp:TextBox>
            </p>
            <hr />
			<h4>
			  <strong><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MAIL_MESSAGE" LocalizedPage="ADMIN_MAIL" /></strong>
            </h4>
			<p>
			  <asp:TextBox ID="Body" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="16"></asp:TextBox>
            </p>
		 </div>
                <div class="card-footer text-lg-center">
			  <asp:LinkButton ID="Send" runat="server" Text="Send" OnClick="Send_Click" CssClass="btn btn-primary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

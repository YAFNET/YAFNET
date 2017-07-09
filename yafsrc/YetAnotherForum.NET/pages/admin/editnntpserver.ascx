<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.editnntpserver" Codebehind="editnntpserver.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITNNTPSERVER" /></h1>
    </div>
</div>
<div class="row">
                <div class="col-xl-12">
                    <div class="card mb-3 card-outline-primary">
                        <div class="card-header card-primary">
                             <i class="fa fa-newspaper-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                        </div>
                        <div class="card-block">

			<h4>
                <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="NNTP_NAME" LocalizedPage="ADMIN_EDITNNTPSERVER" />
            </h4>
			<p>
				<asp:TextBox CssClass="form-control" ID="Name" runat="server" />
		</p><hr />
		
			<h4>
				<YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NNTP_ADRESS" LocalizedPage="ADMIN_EDITNNTPSERVER" />
            </h4>
			<p>
				<asp:TextBox CssClass="form-control" ID="Address" runat="server" />
		</p><hr />
		
			<h4>
				<YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="NNTP_PORT" LocalizedPage="ADMIN_EDITNNTPSERVER" />
            </h4>
			<p>
				<asp:TextBox  ID="Port" runat="server" CssClass="form-control" TextMode="Number" />
		</p><hr />
		
			<h4>
				<YAF:HelpLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NNTP_USERNAME" LocalizedPage="ADMIN_EDITNNTPSERVER" />
            </h4>
			<p>
				<asp:TextBox  CssClass="form-control" ID="UserName" runat="server" Enabled="true" />
		</p><hr />
		
			<h4>
				<YAF:HelpLabel ID="LocalizedLabel6" runat="server" LocalizedTag="NNTP_PASSWORD" LocalizedPage="ADMIN_EDITNNTPSERVER" />
            </h4>
			<p>
				<asp:TextBox  CssClass="form-control" ID="UserPass" runat="server" Enabled="true" />
		</p>
		 </div>
                        <div class="card-footer text-lg-center">
                            <asp:LinkButton ID="Save" runat="server" CssClass="btn btn-primary" OnClick="Save_Click"></asp:LinkButton>&nbsp;
				<asp:LinkButton ID="Cancel" runat="server" CssClass="btn btn-secondary" OnClick="Cancel_Click"></asp:LinkButton>
                        </div>
                    </div>
                </div>
    </div>      
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

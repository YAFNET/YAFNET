<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.pm" Codebehind="pm.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_PM" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-envelope-square fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_PM" />
                </div>
                <div class="card-body">
			 
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="PM_NUMBER" LocalizedPage="ADMIN_PM" />
                &nbsp;<small><asp:Label runat="server" ID="Count" /></small>
             
            <hr />
			 
                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="DELETE_READ" LocalizedPage="ADMIN_PM" />
                 
			<p>
				<asp:TextBox runat="server" ID="Days1" CssClass="form-control DaysInput" TextMode="Number" />
           </p>
			 
                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="DELETE_UNREAD" LocalizedPage="ADMIN_PM" />
             
			<p>
				<asp:TextBox runat="server" ID="Days2" CssClass="form-control DaysInput" TextMode="Number" />
            </p>
                </div>
                <div class="card-footer text-center">
				    <YAF:ThemeButton ID="commit" Type="Primary" runat="server"
                        Icon="trash" TextLocalizedTag="DELETE" TextLocalizedPage="COMMON" 
                        ReturnConfirmText='<%# this.GetText("ADMIN_PM", "CONFIRM_DELETE") %>'>
				    </YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>



<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.mail" Codebehind="mail.ascx.cs" %>


<YAF:PageLinks ID="PageLinks" runat="server" />

    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                    LocalizedTag="HEADER" 
                                    LocalizedPage="ADMIN_MAIL" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-envelope fa-fw text-secondary pr-1"></i>
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                        LocalizedTag="HEADER" LocalizedPage="ADMIN_MAIL" />
			     </div>
                <div class="card-body">
                    <div class="form-group">
                        <asp:Label runat="server" 
                                   CssClass="font-weight-bold"
                                   AssociatedControlID="ToList">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                LocalizedTag="MAIL_TO" LocalizedPage="ADMIN_MAIL" />
                        </asp:Label>
                        <asp:DropDownList ID="ToList" runat="server" 
                                          DataValueField="ID" 
                                          DataTextField="Name" 
                                          CssClass="custom-select">
                </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" 
                                   CssClass="font-weight-bold"
                                   AssociatedControlID="Subject">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server"
                                                LocalizedTag="MAIL_SUBJECT" LocalizedPage="ADMIN_MAIL" />
                        </asp:Label>
                        <asp:TextBox ID="Subject" runat="server" 
                                     CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" 
                                   CssClass="font-weight-bold"
                                   AssociatedControlID="Body">
                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MAIL_MESSAGE" LocalizedPage="ADMIN_MAIL" />
                        </asp:Label>
                        <asp:TextBox ID="Body" runat="server" 
                                     TextMode="MultiLine" 
                                     CssClass="form-control" Rows="16"></asp:TextBox>
                    </div>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Send" runat="server" 
                                     OnClick="SendClick" 
                                     Type="Primary"
                                     Icon="paper-plane"
                                     TextLocalizedTag="SEND_MAIL" TextLocalizedPage="ADMIN_MAIL"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_MAIL", "CONFIRM_SEND") %>'></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
<div class="row">
    <div class="col-xl-12">
        <div class="card">
            <div class="card-header">
                <i class="fa fa-envelope fa-fw text-secondary pr-1"></i>
                Mail testing
            </div>
            <div class="card-body">
                <div class="form-row">
                    <div class="form-group col-md-6">
                        <YAF:HelpLabel runat="server" 
                                       LocalizedTag="FROM_MAIL"
                                       AssociatedControlID="TestFromEmail">
                        </YAF:HelpLabel>
                        <asp:TextBox ID="TestFromEmail" runat="server"
                                     Placeholder="<%# YAF.App_GlobalResources.Install.FromEmail %>" 
                                     Type="Email"
                                     CssClass="form-control"/>
                    </div>
                    <div class="form-group col-md-6">
                        <YAF:HelpLabel runat="server"
                                       LocalizedTag="TO_MAIL"
                                       AssociatedControlID="TestToEmail">
                        </YAF:HelpLabel>
                        <asp:TextBox ID="TestToEmail" runat="server" 
                                     Placeholder="<%# YAF.App_GlobalResources.Install.ToEmail %>" 
                                     CssClass="form-control"/>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" 
                               CssClass="font-weight-bold"
                               AssociatedControlID="TestSubject">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server"
                                            LocalizedTag="MAIL_SUBJECT" LocalizedPage="ADMIN_MAIL" />
                    </asp:Label>
                    <asp:TextBox ID="TestSubject" runat="server" 
                                 CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" 
                               CssClass="font-weight-bold"
                               AssociatedControlID="TestBody">
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                            LocalizedTag="MAIL_MESSAGE" LocalizedPage="ADMIN_MAIL" />
                    </asp:Label>
                    <asp:TextBox ID="TestBody" runat="server" 
                                 TextMode="MultiLine" 
                                 CssClass="form-control" Rows="16"></asp:TextBox>
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="TestSmtp" runat="server" 
                                 TextLocalizedTag="SEND_MAIL" TextLocalizedPage="ADMIN_MAIL"
                                 Type="Info"
                                 Icon="paper-plane"
                                 OnClick="TestSmtpClick"/>
            </div>
        </div>
    </div>
</div>
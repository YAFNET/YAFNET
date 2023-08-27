<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.Mail" Codebehind="Mail.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconType="text-secondary"
                                    IconName="envelope"
                                    LocalizedPage="ADMIN_MAIL"></YAF:IconHeader>
                 </div>
                <div class="card-body">
                    <div class="mb-3">
                        <asp:Label runat="server"
                                   AssociatedControlID="ToList">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                                LocalizedTag="MAIL_TO" LocalizedPage="ADMIN_MAIL" />
                        </asp:Label>
                        <asp:DropDownList ID="ToList" runat="server"
                                          DataValueField="ID"
                                          DataTextField="Name"
                                          CssClass="select2-select">
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <asp:Label runat="server"
                                   AssociatedControlID="Subject">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server"
                                                LocalizedTag="MAIL_SUBJECT" LocalizedPage="ADMIN_MAIL" />
                        </asp:Label>
                        <asp:TextBox ID="Subject" runat="server"
                                     CssClass="form-control"
                                     required="required" />
                        <div class="invalid-feedback">
                            <YAF:LocalizedLabel runat="server"
                                                LocalizedTag="MSG_SUBJECT" />
                        </div>
                    </div>
                    <div class="mb-3">
                        <asp:Label runat="server"
                                   AssociatedControlID="Body">
                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server"
                                                LocalizedTag="MAIL_MESSAGE" LocalizedPage="ADMIN_MAIL" />
                        </asp:Label>
                        <asp:TextBox ID="Body" runat="server"
                                     TextMode="MultiLine"
                                     CssClass="form-control"
                                     Rows="16" />
                    </div>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Send" runat="server"
                                     OnClick="SendClick"
                                     CausesValidation="True"
                                     Type="Primary"
                                     Icon="paper-plane"
                                     TextLocalizedTag="SEND_MAIL" TextLocalizedPage="ADMIN_MAIL"
                                     ReturnConfirmTag="CONFIRM_SEND" />
                </div>
            </div>
        </div>
    </div>
<div class="row">
    <div class="col-xl-12">
        <div class="card">
            <div class="card-header">
                <i class="fa fa-envelope fa-fw text-secondary pe-1"></i>
                Mail testing
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="mb-3 col-md-6">
                        <YAF:HelpLabel runat="server"
                                       LocalizedTag="FROM_MAIL"
                                       AssociatedControlID="TestFromEmail">
                        </YAF:HelpLabel>
                        <asp:TextBox ID="TestFromEmail" runat="server"
                                     Placeholder='<%# this.GetText("FromEmail") %>'
                                     Type="Email"
                                     CssClass="form-control" />
                    </div>
                    <div class="mb-3 col-md-6">
                        <YAF:HelpLabel runat="server"
                                       LocalizedTag="TO_MAIL"
                                       AssociatedControlID="TestToEmail">
                        </YAF:HelpLabel>
                        <asp:TextBox ID="TestToEmail" runat="server"
                                     Placeholder='<%# this.GetText("ToEmail") %>'
                                     CssClass="form-control" />
                    </div>
                </div>
                <div class="mb-3">
                    <asp:Label runat="server"
                               AssociatedControlID="TestSubject">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server"
                                            LocalizedTag="MAIL_SUBJECT" LocalizedPage="ADMIN_MAIL" />
                    </asp:Label>
                    <asp:TextBox ID="TestSubject" runat="server"
                                 CssClass="form-control"  />
                </div>
                <div class="mb-3">
                    <asp:Label runat="server"
                               AssociatedControlID="TestBody">
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server"
                                            LocalizedTag="MAIL_MESSAGE" LocalizedPage="ADMIN_MAIL" />
                    </asp:Label>
                    <asp:TextBox ID="TestBody" runat="server"
                                 TextMode="MultiLine"
                                 CssClass="form-control"
                                 Rows="16" />
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="TestSmtp" runat="server"
                                 CausesValidation="True"
                                 TextLocalizedTag="SEND_MAIL" TextLocalizedPage="ADMIN_MAIL"
                                 Type="Info"
                                 Icon="paper-plane"
                                 OnClick="TestSmtpClick"/>
            </div>
        </div>
    </div>
</div>
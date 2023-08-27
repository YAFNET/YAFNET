<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.EmailTopic" Codebehind="EmailTopic.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                LocalizedTag="TITLE" />
        </h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconType="text-secondary"
                                IconName="paper-plane" />
            </div>
            <div class="card-body">
                <div class="mb-3">
                    <asp:Label runat="server" 
                               AssociatedControlID="EmailAddress">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                            LocalizedTag="to" />
                    </asp:Label>
                    <asp:TextBox ID="EmailAddress" runat="server" 
                                 CssClass="form-control"
                                 required="required" />
                    <div class="invalid-feedback">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NEED_EMAIL" />
                    </div>
                </div>
                <div class="mb-3">
                    <asp:Label runat="server" 
                               AssociatedControlID="Subject">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                            LocalizedTag="subject" />
                    </asp:Label>
                    <asp:TextBox ID="Subject" runat="server" 
                                 CssClass="form-control"
                                 required="required" />
                    <div class="invalid-feedback">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NEED_SUBJECT" />
                    </div>
                </div>
                <div class="mb-3">
                    <asp:Label runat="server" 
                               AssociatedControlID="Message">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                            LocalizedTag="message" />
                    </asp:Label>
                    <asp:TextBox ID="Message" runat="server" 
                                 CssClass="form-control" 
                                 TextMode="MultiLine" 
                                 Rows="12"
                                 required="required" />
                    <div class="invalid-feedback">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NEED_MESSAGE" />
                    </div>
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="SendEmail" runat="server" 
                                 OnClick="SendEmail_Click"
                                 CausesValidation="True"
                                 TextLocalizedTag="SEND"
                                 Icon="paper-plane"
                                 Type="Primary"/>
            </div>
        </div>
    </div>
</div>
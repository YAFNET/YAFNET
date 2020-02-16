<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.EmailTopic" Codebehind="EmailTopic.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-paper-plane fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="TITLE" />
            </div>
            <div class="card-body">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="EmailAddress">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                            LocalizedTag="to" />
                    </asp:Label>
                    <asp:TextBox ID="EmailAddress" runat="server" 
                                 CssClass="form-control">
                    </asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Subject">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                            LocalizedTag="subject" />
                    </asp:Label>
                    <asp:TextBox ID="Subject" runat="server" 
                                 CssClass="form-control">
                    </asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Message">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                            LocalizedTag="message" />
                    </asp:Label>
                    <asp:TextBox ID="Message" runat="server" 
                                 CssClass="form-control" 
                                 TextMode="MultiLine" 
                                 Rows="12">
                    </asp:TextBox>
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="SendEmail" runat="server" 
                                 OnClick="SendEmail_Click"
                                 TextLocalizedTag="SEND"
                                 Icon="paper-plane"
                                 Type="Primary"/>
            </div>
        </div>
    </div>
</div>
<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Approve" Codebehind="Approve.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                LocalizedTag="title" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="check-double"></YAF:IconHeader>
            </div>
            <div class="card-body text-center">
                <YAF:Alert runat="server" ID="Approved"
                           Type="success" 
                           Visible="False">
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                        LocalizedTag="email_verified" />
                </YAF:Alert>
                <YAF:Alert runat="server" ID="Error" 
                           Type="danger" 
                           Visible="False">
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                        LocalizedTag="email_verify_failed" />
                </YAF:Alert>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="key">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                            LocalizedTag="enter_key" />
                    </asp:Label>
                    <asp:TextBox ID="key" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="ValidateKey" runat="server" 
                                 OnClick="ValidateKey_Click" 
                                 Type="Primary"
                                 TextLocalizedTag="VALIDATE"
                                 Icon="check"/>
            </div>
        </div>
    </div>
</div>
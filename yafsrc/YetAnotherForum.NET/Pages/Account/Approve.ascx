<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Account.Approve" Codebehind="Approve.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                LocalizedTag="title" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card w-25 mx-auto">
            <div class="card-body">
                <h5 class="card-title">
                    <YAF:LocalizedLabel runat="server"
                                        LocalizedTag="TITLE"
                                        LocalizedPage="APPROVE" />
                </h5>
                <YAF:Alert runat="server" ID="Approved"
                           Type="success" 
                           Visible="False">
                    <YAF:Icon runat="server"
                              IconName="info-circle" />
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                        LocalizedTag="email_verified" />
                </YAF:Alert>
                <YAF:Alert runat="server" ID="ErrorAlert" 
                           Type="danger" 
                           Visible="False">
                    <YAF:Icon runat="server"
                              IconName="info-circle" />
                    <asp:Literal runat="server" ID="ErrorMessage"></asp:Literal>
                </YAF:Alert>
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="key">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                            LocalizedTag="enter_key" />
                    </asp:Label>
                    <asp:TextBox ID="key" runat="server" CssClass="form-control" />
                </div>
                <div class="mb-3">
                    <YAF:ThemeButton ID="ValidateKey" runat="server" 
                                     OnClick="ValidateKey_Click" 
                                     CssClass="btn-block"
                                     Type="Primary"
                                     TextLocalizedTag="VALIDATE"
                                     Icon="check"/>
                </div>
            </div>
        </div>
    </div>
</div>
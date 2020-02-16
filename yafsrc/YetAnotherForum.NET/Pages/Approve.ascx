<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Approve" Codebehind="Approve.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="title" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-check-double fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
            </div>
            <div class="card-body text-center">
                <asp:PlaceHolder runat="server" id="approved" Visible="false">
                    <YAF:Alert runat="server" Type="success">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="email_verified" />
                    </YAF:Alert>
                </asp:PlaceHolder>
                
                <asp:PlaceHolder id="error" runat="server" Visible="False">
                    <YAF:Alert runat="server" Type="danger">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="email_verify_failed" />
                    </YAF:Alert>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="key">
                            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="enter_key" />
                        </asp:Label>
                        <asp:TextBox ID="key" runat="server" CssClass="form-control" />
                    </div>
                </asp:PlaceHolder>
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
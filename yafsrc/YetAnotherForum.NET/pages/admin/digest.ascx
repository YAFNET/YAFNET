﻿<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.digest"
    CodeBehind="digest.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_DIGEST" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-envelope fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_DIGEST" />
                </div>
                <div class="card-body">
             
                <YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="DIGEST_ENABLED"
                    LocalizedPage="ADMIN_DIGEST" />
             
                <p>
                    <asp:Label ID="DigestEnabled" runat="server"></asp:Label></p>
            <hr />
             
                <YAF:HelpLabel ID="LocalizedLabel5" runat="server" LocalizedTag="DIGEST_LAST" LocalizedPage="ADMIN_DIGEST" />
             
                <p>
                    <asp:Label ID="LastDigestSendLabel" runat="server"></asp:Label></p>
            </div>
                <div class="card-footer text-center">
                <YAF:ThemeButton ID="Button2" runat="server" OnClick="ForceSendClick" Type="Primary"
                                 Icon="paper-plane" TextLocalizedTag="FORCE_SEND">
                </YAF:ThemeButton>
            </div>
        </div>
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-envelope fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_DIGEST" />
           </div>
                <div class="card-body">
        <asp:PlaceHolder ID="DigestHtmlPlaceHolder" runat="server" Visible="false">
                 
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DIGEST_GENERATE"
                        LocalizedPage="ADMIN_DIGEST" />
                 
                <p>
                    <asp:HtmlIframe id="DigestFrame" runat="server" style="width: 100%; height: 500px"></asp:HtmlIframe>
                </p>
        </asp:PlaceHolder>
</div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="GenerateDigest" runat="server" OnClick="GenerateDigestClick" Type="Primary"
                                     Icon="envelope" TextLocalizedTag="GENERATE_DIGEST">
                    </YAF:ThemeButton>
            </div>
        </div>
                <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-envelope fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_DIGEST" />
                </div>
                <div class="card-body">
             
                <YAF:HelpLabel ID="LocalizedLabel7" runat="server" LocalizedTag="DIGEST_EMAIL" LocalizedPage="ADMIN_DIGEST" />
             
            <p>
                <asp:TextBox ID="TextSendEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
            </p>
            <hr />
             
                <YAF:HelpLabel ID="LocalizedLabel8" runat="server" LocalizedTag="DIGEST_METHOD" LocalizedPage="ADMIN_DIGEST" />
             
            <p>
                <asp:DropDownList ID="SendMethod" runat="server" CssClass="custom-select">
                    <asp:ListItem Text="Direct" />
                    <asp:ListItem Text="Queued" Selected="True" />
                </asp:DropDownList>
            </p>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="TestSend" runat="server" OnClick="TestSendClick" Type="Primary"
                                     Icon="paper-plane" TextLocalizedTag="SEND_TEST">
                    </YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>



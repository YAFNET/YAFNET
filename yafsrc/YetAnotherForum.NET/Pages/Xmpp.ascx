<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Xmpp" Codebehind="Xmpp.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <YAF:Alert runat="server" ID="Alert">
            <div class="text-center">
                <asp:Label ID="NotifyLabel" runat="server"></asp:Label>
            </div>
        </YAF:Alert>
    </div>
</div>
<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.RulesAndPrivacy" Codebehind="RulesAndPrivacy.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="row">
    <div class="col-xl-12">
        <h1>
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                LocalizedTag="TITLE" />
        </h1>
    </div>
</div>
<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="user-secret"
                                LocalizedTag="TITLE"
                                LocalizedPage="RULES"/>
            </div>
            <div class="card-body">
                <YAF:LocalizedLabel runat="server" LocalizedTag="RULES_TEXT" EnableBBCode="true" ID="RulesText" />
            </div>
            <asp:Panel runat="server" ID="Footer" 
                       CssClass="card-footer text-center">
                <YAF:ThemeButton ID="Accept" runat="server" 
                                 TextLocalizedTag="ACCEPT" 
                                 Type="Success"
                                 CausesValidation="False"
                                 Icon="check"
                                 OnClick="Accept_Click" />
                <YAF:ThemeButton ID="Cancel" runat="server" 
                                 TextLocalizedTag="DECLINE" 
                                 CausesValidation="False"
                                 Type="Danger"
                                 Icon="hand-paper"
                                 OnClick="Cancel_Click" />
            </asp:Panel>
        </div>
    </div>
</div>
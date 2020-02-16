<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Rules" Codebehind="Rules.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" /></h1>
    </div>
</div>
<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-user-secret fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="TITLE" />
            </div>
            <div class="card-body">
                <YAF:LocalizedLabel runat="server" LocalizedTag="RULES_TEXT" EnableBBCode="true" ID="RulesText" />
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="Accept" runat="server" 
                                 TextLocalizedTag="ACCEPT" 
                                 Type="Success"
                                 Icon="check"
                                 OnClick="Accept_Click" />
                <YAF:ThemeButton ID="Cancel" runat="server" 
                                 TextLocalizedTag="DECLINE" 
                                 Type="Danger"
                                 Icon="hand-paper"
                                 OnClick="Cancel_Click" />
            </div>
        </div>
    </div>
</div>
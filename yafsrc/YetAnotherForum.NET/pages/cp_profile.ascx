<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cp_profile" Codebehind="cp_profile.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ProfileYourAccount" Src="../controls/ProfileYourAccount.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu ID="ProfileMenu1" runat="server" />
    </div>
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-user fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="ControlPanel" runat="server" LocalizedTag="YOUR_ACCOUNT" />
            </div>
            <div class="card-body">
                <YAF:ProfileYourAccount ID="YourAccount" runat="server" />
            </div>
        </div>
    </div>
</div>

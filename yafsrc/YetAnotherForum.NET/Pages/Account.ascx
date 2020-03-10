<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Account" Codebehind="Account.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ProfileYourAccount" Src="../controls/ProfileYourAccount.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ProfileTimeline" Src="../controls/ProfileTimeline.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
    <div class="col">
        <div class="row">
            <div class="col">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:Icon runat="server" 
                                  IconName="address-card"
                                  IconType="text-secondary"></YAF:Icon>
                        <YAF:LocalizedLabel ID="ControlPanel" runat="server" 
                                            LocalizedTag="YOUR_ACCOUNT" />
                    </div>
                    <div class="card-body">
                        <YAF:ProfileYourAccount ID="YourAccount" runat="server" />
                    </div>
                </div>
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="ActivityPlaceHolder">
            <YAF:ProfileTimeline runat="server"></YAF:ProfileTimeline>
        </asp:PlaceHolder>
    </div>
</div>

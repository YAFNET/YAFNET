<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayAd" EnableViewState="false" Codebehind="DisplayAd.ascx.cs" %>

<div class="row">
    <div class="col-xl-12">
        <div class="card text-bg-light mb-3">
            <div class="card-body">
                <div class="row">
                    <div class="col">
                        <span class="badge text-bg-warning">
                            <YAF:LocalizedLabel ID="SponserName" runat="server" 
                                                LocalizedTag="AD_USERNAME" />
                        </span>
                        <YAF:MessagePost ID="AdMessage" runat="server"></YAF:MessagePost>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
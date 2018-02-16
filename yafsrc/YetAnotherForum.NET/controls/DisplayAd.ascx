<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayAd"
    EnableViewState="false" Codebehind="DisplayAd.ascx.cs" %>

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <span class="float-left">
                    <a onclick="ScrollToTop();" class="postTopLink" href="javascript: void(0)">            
                        <i class="fa fa-angle-double-up fa-fw"></i>
                    </a>
                </span>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col">
                        <YAF:MessagePost ID="AdMessage" runat="server"></YAF:MessagePost>
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <span class="badge badge-warning"><YAF:LocalizedLabel ID="SponserName" runat="server" 
                                                                      LocalizedTag="AD_USERNAME" /></span>
            </div>
        </div>
    </div>
</div>
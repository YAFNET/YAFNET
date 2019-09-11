<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayAd"
    EnableViewState="false" Codebehind="DisplayAd.ascx.cs" %>
<%@ Import Namespace="YAF" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Web.Controls" %>

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-body">
                <div class="row">
                    <div class="col">
                        <span class="badge badge-warning">
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
<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
	Inherits="YAF.Controls.ForumIconLegend" Codebehind="ForumIconLegend.ascx.cs" %>
<%@ Import Namespace="YAF" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Web.Controls" %>


<ul class="list-group list-group-horizontal-sm mb-3">
    <li class="list-group-item d-flex align-items-center">
        <span class="fa-stack">        
        <i class="fas fa-comments fa-2x text-success"></i>&nbsp;
        </span>
        <span>
            <YAF:LocalizedLabel ID="NewPostsLabel" runat="server" 
                                LocalizedPage="ICONLEGEND"
                                LocalizedTag="New_Posts" />
        </span>
    </li>
    <li class="list-group-item d-flex align-items-center">
        <span class="fa-stack">        
        <i class="fas fa-comments fa-2x text-secondary"></i>&nbsp;
        </span>            
        <span>
            <YAF:LocalizedLabel ID="NoNewPostsLabel" runat="server" 
                                LocalizedPage="ICONLEGEND"
                                LocalizedTag="No_New_Posts" />
        </span>
    </li>
    <li class="list-group-item d-flex align-items-center">
        <span class="fa-stack small">
            <i class="fas fa-comments fa-stack-2x text-secondary"></i>
            <i class="fas fa-lock fa-stack-1x text-warning" style="position:absolute; bottom:0px !important;text-align:right;line-height: 1em;"></i>
        </span>&nbsp;
        <span class="align-bottom">
            <YAF:LocalizedLabel ID="ForumLockedLabel" runat="server" 
                                LocalizedPage="ICONLEGEND"
                                LocalizedTag="Forum_Locked" />
        </span>
    </li>
</ul>
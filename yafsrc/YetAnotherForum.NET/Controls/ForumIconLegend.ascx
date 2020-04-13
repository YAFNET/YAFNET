<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
	Inherits="YAF.Controls.ForumIconLegend" Codebehind="ForumIconLegend.ascx.cs" %>

<ul class="list-group list-group-flush">
    <li class="list-group-item d-flex align-items-center">
        <YAF:Icon runat="server" 
                  IconName="comments"
                  IconSize="fa-2x"
                  IconType="text-success" />
        <YAF:LocalizedLabel ID="NewPostsLabel" runat="server" 
                            LocalizedPage="ICONLEGEND"
                            LocalizedTag="New_Posts" />
    </li>
    <li class="list-group-item d-flex align-items-center">
        <YAF:Icon runat="server" 
                  IconName="comments"
                  IconSize="fa-2x"
                  IconType="text-secondary"/>
        <YAF:LocalizedLabel ID="NoNewPostsLabel" runat="server" 
                            LocalizedPage="ICONLEGEND"
                            LocalizedTag="No_New_Posts" />
    </li>
    <li class="list-group-item d-flex align-items-center">
        <YAF:Icon runat="server" 
                  IconName="comments"
                  IconType="text-secondary"
                  IconStackName="lock"
                  IconStackType="text-warning"
                  IconStackSize="fa-1x" />
        <YAF:LocalizedLabel ID="ForumLockedLabel" runat="server" 
                            LocalizedPage="ICONLEGEND"
                            LocalizedTag="Forum_Locked" />
    </li>
</ul>
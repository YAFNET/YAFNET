<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
	Inherits="YAF.Controls.ForumIconLegend" Codebehind="ForumIconLegend.ascx.cs" %>

<div class="row">
    <div class="col-auto">
        <i class="fa fa-comments fa-2x" style="color: green"></i>&nbsp;
        <span>
            <YAF:LocalizedLabel ID="NewPostsLabel" runat="server" 
                                LocalizedPage="ICONLEGEND"
                                LocalizedTag="New_Posts" />
        </span>
    </div>
    <div class="col-auto">
        <i class="fa fa-comments fa-2x"></i>&nbsp;
        <span>
            <YAF:LocalizedLabel ID="NoNewPostsLabel" runat="server" 
                                LocalizedPage="ICONLEGEND"
                                LocalizedTag="No_New_Posts" />
        </span>
    </div>
    <div class="col-auto">
        <span class="fa-stack fa-1x">
            <i class="fa fa-comments fa-stack-2x"></i>
            <i class="fa fa-lock fa-stack-1x fa-inverse" style="color: orange;"></i>
        </span>&nbsp;
        <span class="align-bottom">
            <YAF:LocalizedLabel ID="ForumLockedLabel" runat="server" 
                                LocalizedPage="ICONLEGEND"
                                LocalizedTag="Forum_Locked" />
        </span>
    </div>
</div>
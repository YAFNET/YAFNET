<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
	Inherits="YAF.Controls.ForumIconLegend" Codebehind="ForumIconLegend.ascx.cs" %>

<div class="container float-left">
	<div class="row">
	    <div class="col-auto">
	        <YAF:ThemeImage ID="ForumNewImage" runat="server" LocalizedTitlePage="ICONLEGEND"
				LocalizedTitleTag="New_Posts" ThemeTag="FORUM_NEW" />&nbsp;
		    <span><YAF:LocalizedLabel ID="NewPostsLabel" runat="server" LocalizedPage="ICONLEGEND"
				LocalizedTag="New_Posts" /></span>
        </div>
		<div class="col-auto">
		    <YAF:ThemeImage ID="ForumRegularImage" runat="server"
				ThemeTag="FORUM" LocalizedTitlePage="ICONLEGEND" LocalizedTitleTag="No_New_Posts" />&nbsp;
		    <span><YAF:LocalizedLabel ID="NoNewPostsLabel" runat="server" LocalizedPage="ICONLEGEND"
				LocalizedTag="No_New_Posts" /></span>
		</div>
		<div class="col-auto">
		    <YAF:ThemeImage ID="ForumLockedImage" runat="server"
				ThemeTag="FORUM_LOCKED" LocalizedTitlePage="ICONLEGEND" LocalizedTitleTag="Forum_Locked" />&nbsp;
		    <span><YAF:LocalizedLabel ID="ForumLockedLabel" runat="server" LocalizedPage="ICONLEGEND"
				LocalizedTag="Forum_Locked" /></span>
	    </div>
	</div>
</div>
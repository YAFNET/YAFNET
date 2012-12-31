<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="YAF.SampleWebApplication._Default" %>
<%@ Register TagPrefix="YAF" TagName="ForumActiveDiscussion" Src="forum/controls/ForumActiveDiscussion.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to ASP.NET and the YAF.NET Sample Application!
    </h2>
    <p>
        This Is a Sample Demo Application that shows the Integration of YAF.NET as a Control in to an existing ASP.NET Application.
        </p>
    <p>
        To learn more about YAF.NET visit <a href="http://www.yetanotherforum.net" title="YAF.NET Website">YAF.NET Website</a>.
    </p>
    
    <h3>YAF What's New</h3>
    <div class="yafWhatsNew">
        <YAF:ForumActiveDiscussion ID="ActiveDiscussions" runat="server" />
    </div>
</asp:Content>


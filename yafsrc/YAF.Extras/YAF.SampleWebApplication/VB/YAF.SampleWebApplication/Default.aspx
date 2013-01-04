<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="YAF.SampleWebApplication._Default" %>
<%@ Register TagPrefix="YAF" TagName="ForumActiveDiscussion" Src="forum/controls/ForumActiveDiscussion.ascx" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Welcome to ASP.NET and the YAF.NET Sample Application!</h2>
            </hgroup>
            <p>
                This Is a Sample Demo Application that shows the Integration of YAF.NET as a Control in to an existing ASP.NET Application
            </p>
            <p>
                 To learn more about YAF.NET visit <a href="http://www.yetanotherforum.net" title="YAF.NET Website">YAF.NET Website</a>.
            </p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>YAF What's New</h3>
    <div class="yafWhatsNew">
            <YAF:ForumActiveDiscussion ID="ActiveDiscussions" runat="server" />
        </div>
</asp:Content>

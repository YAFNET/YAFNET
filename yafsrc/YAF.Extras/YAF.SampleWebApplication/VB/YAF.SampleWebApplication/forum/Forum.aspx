<%@ Page Title="Forum" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Forum.aspx.vb" Inherits="YAF.SampleWebApplication.Forum" %>

<%@ Register TagPrefix="YAF" Assembly="YAF" Namespace="YAF" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Welcome to ASP.NET and the YAF.NET Sample Application!</h2>
            </hgroup>
           <YAF:Forum runat="server" ID="forum" BoardID="1" />
        </div>
    </section>
</asp:Content>
<%@ Page Title="Forum" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Forum.aspx.cs" Inherits="YAF.SampleWebApplication.Forum" %>
<%@ Register TagPrefix="YAF" Assembly="YAF" Namespace="YAF" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <YAF:Forum runat="server" ID="forum" BoardID="1" />
</asp:Content>

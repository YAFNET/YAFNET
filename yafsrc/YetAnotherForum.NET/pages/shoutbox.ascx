<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.shoutbox" Codebehind="shoutbox.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ShoutBox" Src="../controls/ShoutBox.ascx" %>

<YAF:ShoutBox ID="ShoutBox1" runat="server" />

<YAF:LocalizedLabel ID="MustBeLoggedIn" runat="server" Visible="false" LocalizedPage="SHOUTBOX" LocalizedTag="MUSTBELOGGEDIN"></YAF:LocalizedLabel>
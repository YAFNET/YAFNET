<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Controls.DisplayPostFooter" Codebehind="DisplayPostFooter.ascx.cs" %>
<%@ Import Namespace="YAF" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Web.Controls" %>

<YAF:ThemeButton ID="ReportPost" runat="server"
                 Visible="false"
                 Type="Link" 
                 TextLocalizedPage="POSTS" 
                 TextLocalizedTag="REPORTPOST"
                 Icon="exclamation-triangle"
                 IconColor="text-danger"
                 TitleLocalizedTag="REPORTPOST_TITLE"
                 CssClass="text-left" />
<YAF:ThemeButton ID="MarkAsAnswer" runat="server" 
                 Visible="false" 
                 Type="Link" 
                 TextLocalizedPage="POSTS" 
                 TextLocalizedTag="MARK_ANSWER" 
                 TitleLocalizedTag="MARK_ANSWER_TITLE"
                 Icon="check-square"
                 IconColor="text-success"
                 OnClick="MarkAsAnswerClick" />	
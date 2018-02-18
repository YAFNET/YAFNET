<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Controls.DisplayPostFooter" Codebehind="DisplayPostFooter.ascx.cs" %>

<YAF:ThemeButton ID="btnTogglePost" runat="server" 
                 Type="Link"
                 TextLocalizedPage="POSTS"
                 TextLocalizedTag="TOGGLEPOST"
                 Icon="eye"
                 Visible="false" />

<YAF:ThemeButton ID="ReportPost" runat="server"
                 Visible="false"
                 Type="Link" 
                 CssClass="btn-sm"
                 TextLocalizedPage="POSTS" 
                 TextLocalizedTag="REPORTPOST"
                 Icon="exclamation-triangle" 
                 TitleLocalizedTag="REPORTPOST_TITLE" />
<YAF:ThemeButton ID="MarkAsAnswer" runat="server" 
                 Visible="false" 
                 Type="Link" 
                 CssClass="btn-sm"
                 TextLocalizedPage="POSTS" 
                 TextLocalizedTag="MARK_ANSWER" 
                 TitleLocalizedTag="MARK_ANSWER_TITLE"
                 Icon="check-square" 
                 OnClick="MarkAsAnswerClick" />	
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="showsmilies.ascx.cs" Inherits="YAF.Pages.showsmilies" %>

<script language="javascript">
<!--
  function insertsmiley(code,icon)
  {
    window.opener.insertsmiley(code, icon);
    window.opener.focus();
  }
-->
</script>

<asp:Repeater runat="server" ID="List">
	<HeaderTemplate>
		<table width="100%" cellspacing="1" cellpadding="0" class="content">
			<tr>
				<td class="header1" colspan="3" align="center">
					<%# PageContext.Localization.GetText("TITLE") %></td>
			</tr>
			<tr>
				<td class="header2">
					<%# PageContext.Localization.GetText("HEADER_CODE")%></td>
				<td class="header2" align="center">
					<%# PageContext.Localization.GetText("HEADER_SMILE")%></td>
				<td class="header2">
					<%# PageContext.Localization.GetText("HEADER_MEANING")%></td>
			</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class="post">
				<%# DataBinder.Eval(Container.DataItem,"Code") %>
			</td>
			<td class="post" align="center">
				<asp:HyperLink ID="ClickSmiley" NavigateUrl='<%# GetSmileyScript( DataBinder.Eval(Container.DataItem,"Code","{0}"), DataBinder.Eval(Container.DataItem,"Icon","{0}")) %>'
					ToolTip='<%# DataBinder.Eval(Container.DataItem,"Emoticon") %>' runat="server"><img src='<%# YAF.Classes.Config.Root %>images/emoticons/<%# DataBinder.Eval(Container.DataItem,"Icon") %>'/></asp:HyperLink></td>
			<td class="post">
				<%# DataBinder.Eval(Container.DataItem,"Emoticon") %>
			</td>
		</tr>
	</ItemTemplate>
	<FooterTemplate>
		<tr>
			<td class="footer1" colspan="3" align="center">
				<a href="javascript:window.close();">
					<%# PageContext.Localization.GetText("CLOSE_WINDOW") %>
				</a>
			</td>
		</tr>
	</FooterTemplate>
</asp:Repeater>

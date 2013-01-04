<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.activeusers" Codebehind="activeusers.ascx.cs" %>
<%@ Import Namespace="YAF.Core"%>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator"></div>
<YAF:Pager runat="server" ID="Pager" OnPageChange="Pager_PageChange" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan='<%# this.PageContext.IsAdmin ? 8 : 7 %>'> 
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
		</td>
	</tr>
	<tr>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="username" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabelLatestActions" runat="server" LocalizedTag="latest_action" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="logged_in" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="last_active" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="active" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="browser" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="platform" />
		</td>
        <td id="Iptd_header1" class="header2" runat="server" visible='<%# this.PageContext.IsAdmin %>'>
			<strong>IP</strong>
		</td>
	</tr>
	<asp:Repeater ID="UserList" runat="server">
		<ItemTemplate>
			<tr>
				<td class="post">		
					<YAF:UserLink ID="NameLink"  runat="server" CrawlerName='<%# Convert.ToInt32(Eval("IsCrawler")) > 0 ? Eval("Browser").ToString() : String.Empty %>' UserID='<%# Convert.ToInt32(Eval("UserID")) %>' 				
					 Style='<%# Eval("Style").ToString() %>' />
				    <asp:PlaceHolder ID="HiddenPlaceHolder" runat="server" Visible='<%# Convert.ToBoolean(Eval("IsHidden"))%>' >
				    (<YAF:LocalizedLabel ID="Hidden" LocalizedTag="HIDDEN" runat="server" />)
				    </asp:PlaceHolder>				    
				</td>
				<td class="post">				
					<YAF:ActiveLocation ID="ActiveLocation2" UserID='<%# Convert.ToInt32((Eval("UserID") == DBNull.Value)? 0 : Eval("UserID")) %>' UserName='<%# Eval("UserName") %>' HasForumAccess='<%# Convert.ToBoolean(Eval("HasForumAccess")) %>' ForumPage='<%# Eval("ForumPage") %>' ForumID='<%# Convert.ToInt32((Eval("ForumID") == DBNull.Value)? 0 : Eval("ForumID")) %>' ForumName='<%# Eval("ForumName") %>' TopicID='<%# Convert.ToInt32((Eval("TopicID") == DBNull.Value)? 0 : Eval("TopicID")) %>' TopicName='<%# Eval("TopicName") %>' LastLinkOnly="false"  runat="server"></YAF:ActiveLocation>     
				</td>
				<td class="post">
					<%# this.Get<IDateTime>().FormatTime((DateTime)((System.Data.DataRowView)Container.DataItem)["Login"]) %>
				</td>				
				<td class="post">
					<%# this.Get<IDateTime>().FormatTime((DateTime)((System.Data.DataRowView)Container.DataItem)["LastActive"]) %>
				</td>
				<td class="post">
					<%# this.Get<ILocalization>().GetTextFormatted("minutes", ((System.Data.DataRowView)Container.DataItem)["Active"])%>
				</td>
				<td class="post">
					<%# Eval("Browser") %>
				</td>
				<td class="post">
					<%# Eval("Platform") %>
				</td>
                <td id="Iptd1" class="post" runat="server" visible='<%# this.PageContext.IsAdmin %>'>
					 <a id="Iplink1" href='<%# string.Format(this.PageContext.BoardSettings.IPInfoPageURL,IPHelper.GetIp4Address(Eval("IP").ToString())) %>'
                            title='<%# this.GetText("COMMON","TT_IPDETAILS") %>' target="_blank" runat="server">
                            <%# IPHelper.GetIp4Address(Eval("IP").ToString())%></a>
				</td>
			</tr>	
		</ItemTemplate>
		<FooterTemplate>
			<tr class="footer1">
		    <td colspan='<%# this.PageContext.IsAdmin ? 8 : 7 %>' align="center">            
            <YAF:ThemeButton ID="btnReturn" runat="server" CssClass="yafcssbigbutton rightItem"
                TextLocalizedPage="COMMON" TextLocalizedTag="OK" TitleLocalizedPage="COMMON" TitleLocalizedTag="OK" OnClick="btnReturn_Click" />                
            </td>
           </tr>
		</FooterTemplate>
	</asp:Repeater>
</table>
<YAF:Pager runat="server"  LinkedPager="Pager" OnPageChange="Pager_PageChange" />
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>

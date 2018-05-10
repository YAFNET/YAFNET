<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.activeusers" Codebehind="activeusers.ascx.cs" %>
<%@ Import Namespace="YAF.Core"%>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator"></div>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1"> 
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
		</td>
	</tr>
	<tr>
	    <td style="padding:0">
	<asp:Repeater ID="UserList" runat="server">
	    <HeaderTemplate>
	        <table style="width:100%"  cellspacing="1" cellpadding="0" class="tablesorter" id="ActiveUsers">
                <thead>
                    <tr>
                        <th class="header2">
			                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="username" />
                        </th>
		                <th class="header2">
			                <YAF:LocalizedLabel ID="LocalizedLabelLatestActions" runat="server" LocalizedTag="latest_action" />
		                </th>
		                <th class="header2">
			                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="logged_in" />
		                </th>
		                <th class="header2">
			                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="last_active" />
		                </th>
		                <th class="header2">
			                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="active" />
		                </th>
		                <th class="header2">
			                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="browser" />
		                </th>
		                <th class="header2">
			                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="platform" />
		                </th>
                        <th id="Iptd_header1" class="header2" runat="server" visible='<%# this.PageContext.IsAdmin %>'>
			                <strong>IP</strong>
		                </th>
                    </tr>
                </thead>
                <tbody>
	    </HeaderTemplate>
		<ItemTemplate>
                    <tr>
				        <td class="post">		
					        <YAF:UserLink ID="NameLink"  runat="server" ReplaceName='<%# this.Get<YafBoardSettings>().EnableDisplayName
                              ? Eval("UserDisplayName")
                              : Eval("UserName") %>' CrawlerName='<%# Convert.ToInt32(Eval("IsCrawler")) > 0 ? Eval("Browser").ToString() : String.Empty %>' 
                                UserID='<%# Convert.ToInt32(Eval("UserID")) %>' 				
                                Style='<%# Eval("Style").ToString() %>' />
                            <asp:PlaceHolder ID="HiddenPlaceHolder" runat="server" Visible='<%# Convert.ToBoolean(Eval("IsHidden"))%>' >
                                (<YAF:LocalizedLabel ID="Hidden" LocalizedTag="HIDDEN" runat="server" />)
                            </asp:PlaceHolder>				    
				        </td>
				        <td class="post">				
					        <YAF:ActiveLocation ID="ActiveLocation2" UserID='<%# Convert.ToInt32((Eval("UserID") == DBNull.Value)? 0 : Eval("UserID")) %>' 
                                UserName='<%# Eval("UserName") %>' HasForumAccess='<%# Convert.ToBoolean(Eval("HasForumAccess")) %>' ForumPage='<%# Eval("ForumPage") %>' 
                                ForumID='<%# Convert.ToInt32((Eval("ForumID") == DBNull.Value)? 0 : Eval("ForumID")) %>' ForumName='<%# Eval("ForumName") %>' 
                                TopicID='<%# Convert.ToInt32((Eval("TopicID") == DBNull.Value)? 0 : Eval("TopicID")) %>' TopicName='<%# Eval("TopicName") %>' 
                                LastLinkOnly="false"  runat="server"></YAF:ActiveLocation>     
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
		        </tbody>
	        </table>
            <div id="ActiveUsersPager" class="tableSorterPager">
                        <a href="#" class="first pagelink"><span>&lt;&lt;</span></a>
                        <a href="#" class="prev pagelink"><span>&lt;</span></a>
                        <input type="text" class="pagedisplay"/>
                        <a href="#" class="next pagelink"><span>&gt;</span></a>
                        <a href="#" class="last pagelink"><span>&gt;&gt;</span></a>
                        <select class="pagesize">
		                    <option selected="selected"  value="10">10</option>
		                    <option value="20">20</option>
                        	<option value="30">30</option>
                        	<option  value="40">40</option>
                        </select>
                    </div>
	    </td>
            <tr class="footer1">
	    <td align="center">            
            <YAF:ThemeButton ID="btnReturn" runat="server" CssClass="yafcssbigbutton rightItem"
                TextLocalizedPage="COMMON" TextLocalizedTag="OK" TitleLocalizedPage="COMMON" TitleLocalizedTag="OK" OnClick="Return_Click" />                
	    </td>
           </tr>
		</FooterTemplate>
	</asp:Repeater>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>

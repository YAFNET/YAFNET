<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.admin" Codebehind="admin.ascx.cs" %>
<%@ Import Namespace="YAF.Core.BBCode" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">		
	
	<table width="100%" cellspacing="1" cellpadding="0" class="content">
		<asp:Repeater ID="UserList" runat="server" OnItemCommand="UserList_ItemCommand">
    <HeaderTemplate>      
      
        <tr>
          <td class="header1" colspan="5">
              <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_ADMIN" />
          </td>
        </tr>
        <tr>
          <td class="header2">
             <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ADMIN_NAME" LocalizedPage="ADMIN_ADMIN" /></td>
          <td class="header2">
           <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="ADMIN_EMAIL" LocalizedPage="ADMIN_ADMIN" /></td>
          <td class="header2">
             <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="LOCATION" /></td>
          <td class="header2">
            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="ADMIN_JOINED" LocalizedPage="ADMIN_ADMIN" /></td>
          <td class="header2">
            &nbsp;</td>
        </tr>
    		</HeaderTemplate>
    <ItemTemplate>
      <tr>
        <td class="post">
            <YAF:UserLink ID="UnverifiedUserLink" UserID='<%# Eval("UserID") %>' Style='<%# Eval("Style") %>'  runat="server"/>
        </td>
        <td class="post">
          <%# Eval("Email") %>
        </td>
        <td class="post">
          <%# this.HtmlEncode(YafUserProfile.GetProfile(Eval("Name").ToString()).Location)%>
        </td>
        <td class="post">
          <%# this.Get<IDateTime>().FormatDateTime((DateTime)this.Eval("Joined")) %>
        </td>
        <td class="post">
          <asp:LinkButton OnLoad="Approve_Load" runat="server" CommandName="approve" CommandArgument='<%# Eval("UserID") %>'> 
             <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="ADMIN_APPROVE" LocalizedPage="ADMIN_ADMIN"></YAF:LocalizedLabel> 
           </asp:LinkButton>
          |
          <asp:LinkButton OnLoad="Delete_Load" runat="server" CommandName="delete" CommandArgument='<%# Eval("UserID") %>'> 
            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="ADMIN_DELETE" LocalizedPage="ADMIN_ADMIN" />
           </asp:LinkButton>
        </td>
      </tr>
    		</ItemTemplate>
    <FooterTemplate>
      <tr>
        <td class="footer1" colspan="5">
          <asp:Button OnLoad="ApproveAll_Load" CommandName="approveall" CssClass="pbutton"  runat="server" />
          <asp:Button OnLoad="DeleteAll_Load" CommandName="deleteall" CssClass="pbutton" runat="server" />
          <asp:TextBox ID="DaysOld" runat="server" Width="40px" MaxLength="5"  Text="14" ></asp:TextBox>
          </td>          
      </tr>
      
    		</FooterTemplate>
    	</asp:Repeater>
	</table>
	&nbsp;<br />
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		
		<tr>
			<td class="header1" colspan="4">
			  <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_ADMIN" />
          <span runat="server" id="boardSelector" Visible='<%# this.PageContext.IsHostAdmin %>' > <asp:DropDownList ID="BoardStatsSelect" runat="server" DataTextField="Name" DataValueField="BoardID" OnSelectedIndexChanged="BoardStatsSelect_Changed" AutoPostBack="true" /></span></td>
			
		</tr>
		<tr>
			<td class="postheader" width="25%">
              <YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedTag="NUM_POSTS" LocalizedPage="ADMIN_ADMIN" />
            </td>
			<td class="post" width="25%">
			<asp:Label ID="NumPosts" runat="server"></asp:Label></td>
			
			<td class="postheader" width="25%">
			  <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="POSTS_DAY" LocalizedPage="ADMIN_ADMIN" />
            </td>
			<td class="post" width="25%">
			<asp:Label ID="DayPosts" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="NUM_TOPICS" LocalizedPage="ADMIN_ADMIN" />
            </td>
			<td class="post">
			<asp:Label ID="NumTopics" runat="server"></asp:Label>
            </td>
			<td class="postheader">
              <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="TOPICS_DAY" LocalizedPage="ADMIN_ADMIN" />
            </td>
			<td class="post">
			<asp:Label ID="DayTopics" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="NUM_USERS" LocalizedPage="ADMIN_ADMIN" />
            </td>
			<td class="post">
			<asp:Label ID="NumUsers" runat="server"></asp:Label>
            </td>
			<td class="postheader">
              <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="USERS_DAY" LocalizedPage="ADMIN_ADMIN" />
            </td>
			<td class="post">
			<asp:Label ID="DayUsers" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td class="postheader">
			<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="BOARD_STARTED" LocalizedPage="ADMIN_ADMIN" />
            </td>
			<td class="post">
			<asp:Label ID="BoardStart" runat="server"></asp:Label></td>
			<td class="postheader">
			  <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="SIZE_DATABASE" LocalizedPage="ADMIN_ADMIN" />
            </td>
			<td class="post">
			<asp:Label ID="DBSize" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td class="postfooter" colspan="4">
			<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="STATS_DONTCOUNT" LocalizedPage="ADMIN_ADMIN" />
			</td>
		</tr>
	</table>
		<p id="UpgradeNotice" runat="server" visible="false">
          <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="ADMIN_UPGRADE" LocalizedPage="ADMIN_ADMIN" />
		
		</p>

&nbsp;<br />
	<table width="100%" cellspacing="1" cellpadding="0" class="content">
    <asp:Repeater ID="ActiveList" runat="server">
    <HeaderTemplate>
      
        <tr>
          <td class="header1" colspan="4">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_ADMIN" />
          </td>
        </tr>
        <tr>
          <td class="header2">
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ADMIN_NAME" LocalizedPage="ADMIN_ADMIN" />
          </td>
          <td class="header2">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ADMIN_IPADRESS" LocalizedPage="ADMIN_ADMIN" />
          </td>
          <td class="header2">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="LOCATION" />
          </td>
          <td class="header2">
              <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="BOARD_LOCATION" LocalizedPage="ADMIN_ADMIN" />
           </td>   
        </tr>
    		</HeaderTemplate>
    <ItemTemplate>
      <tr>
        <td class="post">
         <YAF:UserLink ID="ActiveUserLink" UserID='<%# Eval("UserID") %>' ReplaceName='<%# Convert.ToInt32(Eval("IsCrawler")) > 0 ? Eval("Browser").ToString() : String.Empty %>' Style='<%# Eval("Style") %>'  runat="server"/>
        </td>
        <td class="post">
        <a id="A1" href='<%# string.Format(this.PageContext.BoardSettings.IPInfoPageURL,Eval("IP")) %>' title='<%# this.GetText("COMMON","TT_IPDETAILS") %>' target="_blank" runat="server"><%# Eval("IP") %></a>          
        </td>
        <td class="post">
          <%# this.HtmlEncode(YafUserProfile.GetProfile(Eval("UserName").ToString()).Location)%>
        </td>
         <td class="post">
         <YAF:ActiveLocation ID="ActiveLocation2" UserID='<%# Convert.ToInt32((Eval("UserID") == DBNull.Value)? 0 : Eval("UserID")) %>' UserName='<%# Eval("UserName") %>'  ForumPage='<%# Eval("ForumPage") %>' ForumID='<%# Convert.ToInt32((Eval("ForumID") == DBNull.Value)? 0 : Eval("ForumID")) %>' ForumName='<%# Eval("ForumName") %>' TopicID='<%# Convert.ToInt32((Eval("TopicID") == DBNull.Value)? 0 : Eval("TopicID")) %>' TopicName='<%# Eval("TopicName") %>' LastLinkOnly="false"  runat="server"></YAF:ActiveLocation>     
        </td>      
      </tr>
    		</ItemTemplate>
    <FooterTemplate>
      
    		</FooterTemplate>
    	</asp:Repeater>
	</table>
	
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

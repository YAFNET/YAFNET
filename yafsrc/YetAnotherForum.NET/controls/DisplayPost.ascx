<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayPost" EnableViewState="false" Codebehind="DisplayPost.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="DisplayPostFooter" Src="DisplayPostFooter.ascx" %>
<%@ Import Namespace="YAF.Core"%>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<tr class="postheader">		
    <%#GetIndentCell()%>
    <td width="140" id="NameCell" class="postUser" runat="server">
        <a id="post<%# DataRow["MessageID"] %>" /><strong>
            <YAF:OnlineStatusImage id="OnlineStatusImage" runat="server" Visible='<%# this.Get<YafBoardSettings>().ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( DataRow["UserID"] )%>' Style="vertical-align: bottom" UserID='<%# DataRow["UserID"] %>'  />
            <YAF:ThemeImage ID="ThemeImgSuspended" ThemePage="ICONS" ThemeTag="USER_SUSPENDED"  UseTitleForEmptyAlt="True" Enabled='<%# DataRow["Suspended"] != DBNull.Value && DataRow["Suspended"].ToType<DateTime>() > DateTime.UtcNow %>' runat="server"></YAF:ThemeImage>
            <YAF:UserLink  ID="UserProfileLink" runat="server" UserID='<%# DataRow["UserID"]%>' ReplaceName='<%# this.Get<YafBoardSettings>().EnableDisplayName && (!DataRow["IsGuest"].ToType<bool>() || (DataRow["IsGuest"].ToType<bool>() && DataRow["DisplayName"].ToString() == DataRow["UserName"].ToString())) ? DataRow["DisplayName"] : DataRow["UserName"]%>' PostfixText='<%# DataRow["IP"].ToString() == "NNTP" ? this.GetText("EXTERNALUSER") : String.Empty %>' Style='<%#DataRow["Style"]%>' CssClass="UserPopMenuLink" EnableHoverCard="False" />
        </strong>
        &nbsp;<YAF:ThemeButton ID="AddReputation" CssClass='<%# "AddReputation_" + DataRow["UserID"]%>' runat="server" ImageThemeTag="VOTE_UP" Visible="false" TitleLocalizedTag="VOTE_UP_TITLE" OnClick="AddUserReputation"></YAF:ThemeButton>
        <YAF:ThemeButton ID="RemoveReputation" CssClass='<%# "RemoveReputation_" + DataRow["UserID"]%>' runat="server" ImageThemeTag="VOTE_DOWN" Visible="false" TitleLocalizedTag="VOTE_DOWN_TITLE" OnClick="RemoveUserReputation"></YAF:ThemeButton>
    </td>
    <td width="80%" class="postPosted" colspan='<%#GetIndentSpan()%>'>
        <div class="leftItem postedLeft">        
            <strong><a href='<%# YafBuildLink.GetLink(ForumPages.posts,"m={0}#post{0}",DataRow["MessageID"]) %>'>
                #<%# (CurrentPage * this.Get<YafBoardSettings>().PostsPerPage) + PostCount + 1%></a>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POSTED" />
                :</strong>
            <YAF:DisplayDateTime id="DisplayDateTime" runat="server" DateTime='<%# DataRow["Posted"] %>'></YAF:DisplayDateTime>
            </div>
        <div class="rightItem postedRight">
            <YAF:ThemeButton ID="Retweet" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_RETWEET"
                TitleLocalizedTag="BUTTON_RETWEET_TT" OnClick="Retweet_Click" />
            <span id="<%# "dvThankBox" + DataRow["MessageID"] %>">
                <YAF:ThemeButton ID="Thank" runat="server" CssClass="yaflittlebutton" Visible="false" TextLocalizedTag="BUTTON_THANKS"
                    TitleLocalizedTag="BUTTON_THANKS_TT" />
            </span>        
            <YAF:ThemeButton ID="Attach" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_ATTACH"
                TitleLocalizedTag="BUTTON_ATTACH_TT" />
            <YAF:ThemeButton ID="Edit" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_EDIT"
                TitleLocalizedTag="BUTTON_EDIT_TT" />
            <YAF:ThemeButton ID="MovePost" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_MOVE"
                TitleLocalizedTag="BUTTON_MOVE_TT" />
            <YAF:ThemeButton ID="Delete" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_DELETE"
                TitleLocalizedTag="BUTTON_DELETE_TT" />
            <YAF:ThemeButton ID="UnDelete" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_UNDELETE"
                TitleLocalizedTag="BUTTON_UNDELETE_TT" />
            <YAF:ThemeButton ID="Quote" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_QUOTE"
                TitleLocalizedTag="BUTTON_QUOTE_TT" />
                <asp:CheckBox runat="server" ID="MultiQuote" CssClass="MultiQuoteButton"  />
        </div>
                
    </td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td <%# GetRowSpan() %> valign="top" height="<%# GetUserBoxHeight() %>" class="UserBox" colspan='<%#GetIndentSpan()%>'>
        <YAF:UserBox id="UserBox1" runat="server" Visible="<%# !PostData.IsSponserMessage %>" PageCache="<%# PageContext.CurrentForumPage.PageCache %>" DataRow='<%# DataRow %>'></YAF:UserBox>
    </td>
    <td valign="top" class="message">
        <div class="postdiv">
            <asp:panel id="panMessage" runat="server">      
                <YAF:MessagePostData ID="MessagePost1" runat="server" DataRow="<%# DataRow %>" IsAltMessage="<%# this.IsAlt %>" ColSpan="<%#GetIndentSpan()%>" ShowEditMessage="True"></YAF:MessagePostData>
            </asp:panel> 
        </div>
    </td>
</tr>
<tr class="postfooter">
    <td class="small postTop" colspan='<%#GetIndentSpan()%>'>
        <a onclick="ScrollToTop();" class="postTopLink" href="javascript: void(0)">            
            <YAF:ThemeImage LocalizedTitlePage="POSTS" LocalizedTitleTag="TOP"  runat="server" ThemeTag="TOTOPPOST" />
        </a>
     <span id="IPSpan1" class="rightItem postInfoRight" runat="server" visible="false"> 
		&nbsp;&nbsp;
		<b><%# this.GetText("IP") %>:</b>&nbsp;<a id="IPLink1" target="_blank" runat="server"/>			   
	</span> 		
    </td>
		<td class="postfooter postInfoBottom">
			<YAF:DisplayPostFooter id="PostFooter" runat="server" DataRow="<%# DataRow %>"></YAF:DisplayPostFooter>
		</td>
</tr>
<tr class="<%#GetPostClass()%> postThanksRow">
    <td style="padding: 5px;" colspan="2" valign="top">
        <div id="<%# "dvThanksInfo" + DataRow["MessageID"] %>" class="ThanksInfo">
            <asp:Literal runat="server"  Visible="false" ID="ThanksDataLiteral"></asp:Literal></div>
    </td>
    <td class="message" style="padding: 5px;" valign="top">
        <div id="<%# "dvThanks" + DataRow["MessageID"] %>" class="ThanksList">
            <asp:Literal runat="server" Visible="false" ID="thanksDataExtendedLiteral"></asp:Literal>
        </div>
    </td>
</tr>
<tr class="postsep">
    <td colspan="3">
        <YAF:PopMenu runat="server" ID="PopMenu1" Control="UserName" />
    </td>
</tr>

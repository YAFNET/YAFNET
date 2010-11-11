<%@ Register TagPrefix="user" TagName="DisplayPostFooter" Src="DisplayPostFooter.ascx" %>
<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayPost"
	EnableViewState="false" Codebehind="DisplayPost.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="DisplayPostFooter" Src="DisplayPostFooter.ascx" %>
<%@ Import Namespace="YAF.Classes.Core"%>
<tr class="postheader">		
    <%#GetIndentCell()%>
    <td width="140" id="NameCell" class="postUser" runat="server">
        <a name="post<%# DataRow["MessageID"] %>" /><b>						
            <YAF:UserLink ID="UserProfileLink" runat="server" UserID='<%#DataRow["UserID"]%>' Style='<%#DataRow["Style"]%>' />
            <%#PageContext.BoardSettings.ShowIrkooRepOnlyInTopics ? YafIrkoo.IrkRating(DataRow["UserID"]) : string.Empty%>
            <YAF:OnlineStatusImage id="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( DataRow["UserID"] )%>' Style="vertical-align: bottom" UserID='<%# DataRow["UserID"] %>'  />
        </b>
        <div class="Irkoo" style="float:right">
            <%#YafIrkoo.IrkVote(DataRow["MessageID"], DataRow["UserID"])%>
        </div>
    </td>
    <td width="80%" class="postPosted" colspan='<%#GetIndentSpan()%>'>
        <div class="leftItem postedLeft">        
            <strong><a href='<%# YafBuildLink.GetLink(ForumPages.posts,"m={0}#post{0}",DataRow["MessageID"]) %>'>
                #<%# Convert.ToInt32((DataRow["Position"]))+1 %></a>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POSTED" />
                :</strong>
            <YAF:DisplayDateTime id="DisplayDateTime" runat="server" DateTime='<%# DataRow["Posted"] %>'></YAF:DisplayDateTime>
            </div>
        <div class="rightItem postedRight">
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
        </div>
                
    </td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td valign="top" height="<%# GetUserBoxHeight() %>" class="UserBox" colspan='<%#GetIndentSpan()%>'>
        <YAF:UserBox id="UserBox1" runat="server" Visible="<%# !PostData.IsSponserMessage %>" PageCache="<%# PageContext.CurrentForumPage.PageCache %>" DataRow="<%# DataRow %>"></YAF:UserBox>
    </td>
    <td valign="top" class="message">
        <div class="postdiv">
            <asp:panel id="panMessage" runat="server">      
                <YAF:MessagePostData ID="MessagePost1" runat="server" DataRow="<%# DataRow %>"></YAF:MessagePostData>
            </asp:panel>            
        </div>
    </td>
</tr>
<tr class="postfooter">
    <td class="small postTop" colspan='<%#GetIndentSpan()%>'>
        <a href="javascript:scroll(0,0)">
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TOP" />
        </a>
    </td>
		<td class="postfooter postInfoBottom">
			<YAF:DisplayPostFooter id="PostFooter" runat="server" DataRow="<%# DataRow %>"></YAF:DisplayPostFooter>
		</td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td style="padding: 5px;" colspan="2" valign="top">
        <div style="font-weight: bold;" id="<%# "dvThanksInfo" + DataRow["MessageID"] %>">
            <asp:Literal runat="server"  Visible="false" ID="ThanksDataLiteral"></asp:Literal></div>
    </td>
    <td class="message" style="padding: 5px;" valign="top">
        <div id="<%# "dvThanks" + DataRow["MessageID"] %>">
            <asp:Literal runat="server" Visible="false" ID="thanksDataExtendedLiteral"></asp:Literal>
        </div>
    </td>
</tr>
<tr class="postsep">
    <td colspan="3">
        <YAF:PopMenu runat="server" ID="PopMenu1" Control="UserName" />
    </td>
</tr>

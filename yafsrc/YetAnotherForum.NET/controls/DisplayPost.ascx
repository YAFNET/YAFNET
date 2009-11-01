<%@ Control Language="c#" AutoEventWireup="True" CodeFile="DisplayPost.ascx.cs" Inherits="YAF.Controls.DisplayPost"
    EnableViewState="false" %>
<%@ Import Namespace="YAF.Classes.Core"%>
<tr class="postheader">		
    <%#GetIndentCell()%>
    <td width="140" id="NameCell" class="postUser" runat="server">
        <a name="post<%# DataRow["MessageID"] %>" /><b>						
            <YAF:UserLink ID="UserProfileLink" runat="server" UserID='<%#DataRow["UserID"]%>' UserName='<%#DataRow["UserName"]%>' Style='<%#DataRow["Style"]%>' />
            <YAF:OnlineStatusImage id="OnlineStatusImage" runat="server" Style="vertical-align: bottom" UserID='<%# DataRow["UserID"] %>'  />
        </b>
    </td>
    <td width="80%" class="postPosted" colspan='<%#GetIndentSpan()%>'>
        <div class="leftItem postedLeft">        
            <b><a href='<%# YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.posts,"m={0}#post{0}",DataRow["MessageID"]) %>'>
                #<%# Convert.ToInt32((DataRow["Position"]))+1 %></a>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POSTED" />
                :</b>
            <%# YafServices.DateTime.FormatDateTime((System.DateTime)DataRow["Posted"]) %>
            </div>
        <div class="rightItem postedRight">
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
            <span id="<%# "dvThankBox" + DataRow["MessageID"] %>">
                <YAF:ThemeButton ID="Thank" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_THANKS"
                    TitleLocalizedTag="BUTTON_THANKS_TT" />
            </span>
        </div>
                
    </td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td valign="top" height="<%# GetUserBoxHeight() %>" class="UserBox" colspan='<%#GetIndentSpan()%>'>
        <YAF:UserBox id="UserBox1" runat="server" Visible="<%# !IsSponserMessage %>" PageCache="<%# ParentPage.PageCache %>" DataRow="<%# DataRow %>"></YAF:UserBox>
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
        <div class="leftItem postInfoLeft">
                    <YAF:ThemeButton ID="btnTogglePost" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="TOGGLEPOST" Visible="false" />                
                    <YAF:ThemeButton ID="Pm" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="PM" ImageThemeTag="PM" />
                    <YAF:ThemeButton ID="Email" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="EMAIL" ImageThemeTag="EMAIL" />
                    <YAF:ThemeButton ID="Home" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="HOME" ImageThemeTag="HOME" />
                    <YAF:ThemeButton ID="Blog" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="BLOG" ImageThemeTag="BLOG" />
                    <YAF:ThemeButton ID="Msn" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="MSN" ImageThemeTag="MSN" />
                    <YAF:ThemeButton ID="Aim" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="AIM" ImageThemeTag="AIM" />
                    <YAF:ThemeButton ID="Yim" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="YIM" ImageThemeTag="YIM" />
                    <YAF:ThemeButton ID="Icq" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="ICQ" ImageThemeTag="ICQ" />
                    <YAF:ThemeButton ID="Skype" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="SKYPE" ImageThemeTag="SKYPE" />
       </div>
       <div class="rightItem postInfoRight">
            &nbsp;<asp:LinkButton ID="ReportButton" CommandName="ReportAbuse" CommandArgument='<%# DataRow["MessageID"] %>'
                runat="server"></asp:LinkButton>
            |
            <asp:LinkButton ID="ReportSpamButton" CommandName="ReportSpam" CommandArgument='<%# DataRow["MessageID"] %>'
                runat="server"></asp:LinkButton>
            <span id="AdminInformation" runat="server" class="smallfont"></span>
        </div>
    </td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td style="padding: 5px;" colspan="2" valign="top">
        <div style="font-weight: bold;" id="<%# "dvThanksInfo" + DataRow["MessageID"] %>">
            <asp:Literal runat="server" ID="Literal1"></asp:Literal></div>
    </td>
    <td class="message" style="padding: 5px;" valign="top">
        <div id="<%# "dvThanks" + DataRow["MessageID"] %>">
            <asp:Literal runat="server" ID="Literal2"></asp:Literal>
        </div>
    </td>
</tr>
<tr class="postsep">
    <td colspan="3">
        <YAF:PopMenu runat="server" ID="PopMenu1" Control="UserName" />
    </td>
</tr>

<%@ Control Language="c#" AutoEventWireup="True" CodeFile="DisplayPost.ascx.cs" Inherits="YAF.Controls.DisplayPost"
    EnableViewState="false" %>
<tr class="postheader">
    <%#GetIndentCell()%>
    <td width="140" id="NameCell" runat="server">
        <a name="post<%# DataRow["MessageID"] %>" /><b>
            <YAF:UserLink ID="UserProfileLink" runat="server" UserID='<%#DataRow["UserID"]%>'
                UserName='<%#DataRow["UserName"]%>' />
        </b>
    </td>
    <td width="80%" colspan='<%#GetIndentSpan()%>'>
        <table cellspacing="0" cellpadding="0" width="100%">
            <tr>
                <td class="postheader">
                    <b><a href='<%# YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.posts,"m={0}#post{0}",DataRow["MessageID"]) %>'>
                        #<%# Convert.ToInt32((DataRow["Position"]))+1 %></a>
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POSTED" />
                        :</b>
                    <%# YafDateTime.FormatDateTime((System.DateTime)DataRow["Posted"]) %>
                </td>
                <td class="postheader" width="50%">
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
                </td>
            </tr>
        </table>
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
    <td class="small" colspan='<%#GetIndentSpan()%>'>
        <a href="javascript:scroll(0,0)">
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TOP" />
        </a>
    </td>
    <td class="postfooter">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td>
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
                </td>
                <td align="right">
                    &nbsp;<asp:LinkButton ID="ReportButton" CommandName="ReportAbuse" CommandArgument='<%# DataRow["MessageID"] %>'
                        runat="server"></asp:LinkButton>
                    |
                    <asp:LinkButton ID="ReportSpamButton" CommandName="ReportSpam" CommandArgument='<%# DataRow["MessageID"] %>'
                        runat="server"></asp:LinkButton>
                    <span id="AdminInformation" runat="server" class="smallfont"></span>
                </td>
            </tr>
        </table>
    </td>
</tr>
<tr class="postsep">
    <td colspan="3">
        <YAF:PopMenu runat="server" ID="PopMenu1" Control="UserName" />
    </td>
</tr>

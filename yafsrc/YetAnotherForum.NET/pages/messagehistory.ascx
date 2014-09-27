<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.messagehistory"CodeBehind="messagehistory.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="7">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
        </td>
    </tr>
    <asp:Repeater ID="RevisionsList" runat="server"  OnItemCommand="RevisionsList_ItemCommand">
        <HeaderTemplate>
            <tr>
                <td class="header2" width="13">
                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="OLD" />
                </td>
                <td class="header2" width="13">
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="NEW" />
                </td>
                <td class="header2" width="550">
                    <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITEREASON" />
                </td>
                <td class="header2">
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITEDBY" />
                </td>
                 <td class="header2" width="50">
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITEDBY_MOD" />
                </td>
                <td class="header2">
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITED" />
                </td>
                <td class="header2">
                </td>
            </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="post">
                    <asp:RadioButton runat="server" ID="Old" onclick="toggleOldSelection(this);" />
                </td>
                <td class="post">
                    <asp:RadioButton runat="server" ID="New" onclick="toggleNewSelection(this);" />
                </td>
                <td class="post">
                    <asp:HiddenField runat="server" Value='<%# FormatMessage((System.Data.DataRow)Container.DataItem)%>' ID="MessageField" />
                    <%# Container.DataItemToField<DateTime>("Edited") != Container.DataItemToField<DateTime>("Posted") ? Container.DataItemToField<string>("EditReason").IsNotSet() ? this.GetText("EDIT_REASON_NA") : Container.DataItemToField<string>("EditReason"): this.GetText("ORIGINALMESSAGE") %>
                    <%# Container.ItemIndex.Equals(this.RevisionsCount-1) ? "({0})".FormatWith(this.GetText("MESSAGEHISTORY","CURRENTMESSAGE")) : string.Empty %>
                </td>
                <td class="post">
                    <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# this.Get<YafBoardSettings>().ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( Container.DataItemToField<int>("UserID") )%>'
                        Style="vertical-align: bottom" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
                    <YAF:UserLink ID="UserLink3" runat="server" UserID='<%# Container.DataItemToField<int>("EditedBy") %>' />
                    <br />
                    <span id="IPSpan1" runat="server" visible='<%# PageContext.IsAdmin || (this.Get<YafBoardSettings>().AllowModeratorsViewIPs && PageContext.ForumModeratorAccess)%>'>
                        <strong>
                            <%# this.GetText("IP") %>:</strong><a id="IPLink1" href='<%# this.Get<YafBoardSettings>().IPInfoPageURL.FormatWith(IPHelper.GetIp4Address(Container.DataItemToField<string>("IP"))) %>'
                                title='<%# this.GetText("COMMON","TT_IPDETAILS") %>'
                                target="_blank" runat="server"><%# IPHelper.GetIp4Address(Container.DataItemToField<string>("IP")) %></a>
                    </span>
                </td>
                <td class="post">
                    <%# Container.DataItemToField<bool>("IsModeratorChanged") ?  this.GetText("YES") : this.GetText("NO") %>
                </td>
                <td class="post"><%# this.Get<IDateTime>().FormatDateTimeTopic( Container.DataItemToField<DateTime>("Edited") ) %></td>
                <td class="post">
                    <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" runat="server"
                        CommandName='restore' CommandArgument='<%# Container.DataItemToField<DateTime>("Edited") %>' 
                        TitleLocalizedTag="RESTORE_MESSAGE" TextLocalizedTag="RESTORE_MESSAGE"
                        Visible='<%# (this.PageContext.IsAdmin || this.PageContext.IsModeratorInAnyForum) && !Container.ItemIndex.Equals(this.RevisionsCount-1) %>'
                        OnLoad="RestoreVersion_Load">
                    </YAF:ThemeButton>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr class="postfooter">
        <td class="post" colspan="7">
            <a onclick="RenderMessageDiff('<%# this.GetText("MESSAGEHISTORY","MESSAGE_EDITEDAT") %>','<%# this.GetText("MESSAGEHISTORY","NOTHING_SELECTED") %>','<%# this.GetText("MESSAGEHISTORY","SELECT_BOTH") %>','<%# this.GetText("MESSAGEHISTORY","SELECT_DIFFERENT") %>');" class="yafcssbigbutton leftItem">
                <span>
                    <%# this.GetText("MESSAGEHISTORY","COMPARE_VERSIONS") %>
                </span>
            </a>            
            <YAF:ThemeButton ID="ReturnBtn" CssClass="yafcssbigbutton leftItem" OnClick="ReturnBtn_OnClick"
                TextLocalizedTag="TOMESSAGE" Visible="false" runat="server">
            </YAF:ThemeButton>
            <YAF:ThemeButton ID="ReturnModBtn" CssClass="yafcssbigbutton leftItem" OnClick="ReturnModBtn_OnClick"
                TextLocalizedTag="GOMODERATE" Visible="false" runat="server">
            </YAF:ThemeButton>
        </td>
    </tr>
</table>

<br/>

<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="7">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="COMPARE_TITLE" />
        </td>
    </tr>
    <tr>
        <td class="header2">
            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="TEXT_CHANGES" />
        </td>
    </tr>
    <tr>
        <td class="post" id="diffContent"><%# this.GetText("MESSAGEHISTORY","SELECT_DIFFERENT") %></td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>

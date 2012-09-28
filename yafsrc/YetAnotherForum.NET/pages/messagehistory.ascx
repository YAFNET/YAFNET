<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.messagehistory"CodeBehind="messagehistory.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
        </td>
    </tr>
    <asp:Repeater ID="RevisionsList" runat="server">
        <ItemTemplate>
            <tr runat="server" id="history_tr" visible='<%# (Container.DataItemToField<DateTime>("Edited") != Container.DataItemToField<DateTime>("Posted")) %>'
                class="postheader">
                <td colspan="1" class="header2">
                    &nbsp;
                </td>
                <td id="history_column" colspan="1" class='<%# Container.DataItemToField<bool>("IsModeratorChanged") ?  "post_res" : "postheader" %>'
                    runat="server">
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITED" />
                    </strong>:
                    <%# this.Get<IDateTime>().FormatDateTimeTopic( Container.DataItemToField<DateTime>("Edited") ) %>
                    <br />
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITEDBY" />
                    </strong>
                    <YAF:UserLink ID="UserLink2" runat="server" UserID='<%# Container.DataItemToField<int>("EditedBy") %>' />
                    <br />
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITREASON" />
                    </strong>
                    <%# Container.DataItemToField<string>("EditReason").IsNotSet() ? this.GetText("EDIT_REASON_NA") : Container.DataItemToField<string>("EditReason") %>
                    <br />
                    <span id="IPSpan1" runat="server" visible='<%# PageContext.IsAdmin || (this.Get<YafBoardSettings>().AllowModeratorsViewIPs && PageContext.ForumModeratorAccess)%>'>
                        <strong>
                            <%# this.GetText("IP") %>:</strong><a id="IPLink1" href='<%# this.Get<YafBoardSettings>().IPInfoPageURL.FormatWith(IPHelper.GetIp4Address(Container.DataItemToField<string>("IP"))) %>'
                                title='<%# this.GetText("COMMON","TT_IPDETAILS") %>'
                                target="_blank" runat="server"><%# IPHelper.GetIp4Address(Container.DataItemToField<string>("IP")) %></a>
                    </span>
                </td>
            </tr>
            <tr runat="server" id="original_tr" visible='<%# (Container.DataItemToField<DateTime>("Edited") == Container.DataItemToField<DateTime>("Posted")) %>'
                class="postheader">
                <td class="header2" colspan="1">
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="MESSAGEHISTORY"
                        LocalizedTag="ORIGINALMESSAGE">
                    </YAF:LocalizedLabel>
                </td>
                <td id="original_column" colspan="1" class='<%# Container.DataItemToField<bool>("IsModeratorChanged") ?  "post_res" : "postheader" %>'
                    runat="server">
                    <strong>
                        <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
                    </strong>
                    <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# this.Get<YafBoardSettings>().ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( Container.DataItemToField<int>("UserID") )%>'
                        Style="vertical-align: bottom" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
                    &nbsp; <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="POSTED" />
                    </strong>
                    <%# this.Get<IDateTime>().FormatDateTimeTopic( Container.DataItemToField<DateTime>("Posted") )%>
                    &nbsp; <span id="IPSpan2" runat="server" visible='<%# PageContext.IsAdmin || (this.Get<YafBoardSettings>().AllowModeratorsViewIPs && PageContext.IsModeratorInAnyForum)%>'>
                        <strong>
                            <%# this.GetText("IP") %>: </strong><a id="IPLink2" href='<%# this.Get<YafBoardSettings>().IPInfoPageURL.FormatWith(YAF.Utils.Helpers.IPHelper.GetIp4Address(Container.DataItemToField<string>("IP"))) %>'
                                title='<%# this.GetText("COMMON","TT_IPDETAILS") %>'
                                target="_blank" runat="server"><%# YAF.Utils.Helpers.IPHelper.GetIp4Address(Container.DataItemToField<string>("IP")) %></a>
                    </span>
                </td>
            </tr>
            <tr>
                <td class="post" colspan="2" align="center">
                    <YAF:MessagePostData ID="MessagePostPrimary" runat="server" ShowAttachments="false"
                        ShowSignature="false" DataRow="<%# PageContext.IsAdmin || PageContext.IsModeratorInAnyForum ? Container.DataItem : null %>">
                    </YAF:MessagePostData>
                </td>
            </tr>
            <tr runat="server" id="historystart_tr" visible='<%# (Container.DataItemToField<DateTime>("Edited") == Container.DataItemToField<DateTime>("Posted")) && !singleReport %>'
                class="postheader">
                <td class="header2" colspan="2">
                    <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedPage="MESSAGEHISTORY"
                        LocalizedTag="HISTORYSTART">
                    </YAF:LocalizedLabel>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Repeater ID="CurrentMessageRpt" Visible="false" runat="server">
        <ItemTemplate>
            <tr class="postheader">
                <td class="header2" colspan="1" valign="top">
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="MESSAGEHISTORY"
                        LocalizedTag="CURRENTMESSAGE" />
                </td>
                <td colspan="1" class='<%# Container.DataItemToField<bool>("IsModeratorChanged") ?  "post_res" : "postheader" %>'
                    runat="server">
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITED" /> </strong>
                    
                    <%# this.Get<IDateTime>().FormatDateTimeTopic( Container.DataItemToField<DateTime>("Edited") ) %>
                  
                    <br />
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITEDBY" />
                    </strong>
                    <YAF:UserLink ID="UserLink2" runat="server" UserID='<%# Container.DataItemToField<int>("EditedBy") %>' />
                    <br />                  
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITREASON" />                   
                    <%# Container.DataItemToField<string>("EditReason").IsNotSet() ? this.GetText("EDIT_REASON_NA") : Container.DataItemToField<string>("EditReason") %>
                    <br />
                    <span id="IPSpan3" runat="server" visible='<%# PageContext.IsAdmin || (this.Get<YafBoardSettings>().AllowModeratorsViewIPs && PageContext.IsModeratorInAnyForum)%>'>
                        <strong>
                            <%# this.GetText("IP") %>: </strong><a id="IPLink3" href='<%# this.Get<YafBoardSettings>().IPInfoPageURL.FormatWith(YAF.Utils.Helpers.IPHelper.GetIp4Address(Container.DataItemToField<string>("IP"))) %>'
                                title='<%# this.GetText("COMMON","TT_IPDETAILS") %>'
                                target="_blank" runat="server"><%# YAF.Utils.Helpers.IPHelper.GetIp4Address(Container.DataItemToField<string>("IP")) %></a>
                    </span>
                    <br />
                </td>
            </tr>
            <tr>
                <td class="post" colspan="2" align="center">
                    <YAF:MessagePostData ID="MessagePostCurrent" runat="server" ShowAttachments="false"
                        ShowSignature="false" DataRow="<%# PageContext.IsAdmin || PageContext.IsModeratorInAnyForum ? Container.DataItem : null %>">
                    </YAF:MessagePostData>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr class="postfooter">
        <td class="post" colspan="2">
            <YAF:ThemeButton ID="ReturnBtn" CssClass="yafcssbigbutton leftItem" OnClick="ReturnBtn_OnClick"
                TextLocalizedTag="TOMESSAGE" Visible="false" runat="server">
            </YAF:ThemeButton>
            <YAF:ThemeButton ID="ReturnModBtn" CssClass="yafcssbigbutton leftItem" OnClick="ReturnModBtn_OnClick"
                TextLocalizedTag="GOMODERATE" Visible="false" runat="server">
            </YAF:ThemeButton>
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>

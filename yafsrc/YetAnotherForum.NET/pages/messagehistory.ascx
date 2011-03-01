<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.messagehistory"CodeBehind="messagehistory.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
        </td>
    </tr>
    <asp:Repeater ID="RevisionsList" runat="server">
        <ItemTemplate>
            <tr class="header2">
                <td>
                    <strong>
                       <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="MESSAGEHISTORY"
                          LocalizedTag='<%# (Container.DataItemToField<DateTime>("Edited") == Container.DataItemToField<DateTime>("Posted")) ? "ORIGINALMESSAGE" : "HISTORYSTART"  %>'
                        />
                        
                    </strong>
                </td>
                <td colspan="2">
                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag='<%# (Container.DataItemToField<DateTime>("Edited") == Container.DataItemToField<DateTime>("Posted")) ? "POSTEDBY" : "EDITEDBY"  %>' />
                    <YAF:UserLink ID="UserLink3" runat="server" UserID='<%# Container.DataItemToField<int>("EditedBy") %>' />&nbsp;-&nbsp;
                    <%# this.Get<IDateTime>().FormatDateTimeTopic( Container.DataItemToField<DateTime>("Edited") ) %>
                </td>
            </tr>
            <tr runat="server" id="history_tr"
                class="postheader">
                <td id="original_column" class='<%# Container.DataItemToField<bool>("IsModeratorChanged") ?  "post_res" : "postheader" %>'
                    runat="server" visible='<%# (Container.DataItemToField<DateTime>("Edited") == Container.DataItemToField<DateTime>("Posted")) %>'
                    style="width:20%;vertical-align:top;">
                    
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="POSTED" />
                    </strong>
                    <%# this.Get<IDateTime>().FormatDateTimeTopic( Container.DataItemToField<DateTime>("Posted") )%>
                    <br />
                    <strong><YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="POSTEDBY" />:</strong>
                      <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( Container.DataItemToField<int>("UserID") )%>'
                        Style="vertical-align: bottom" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
                      <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
                      <br />
                      <span id="IPSpan2" runat="server" visible='<%# PageContext.IsAdmin || (PageContext.BoardSettings.AllowModeratorsViewIPs && PageContext.IsModerator)%>'>
                        <strong>
                            <%# this.GetText("IP") %>:</strong><a id="IPLink2" href='<%# string.Format(this.PageContext.BoardSettings.IPInfoPageURL,Container.DataItemToField<string>("IP")) %>'
                                title='<%# this.GetText("COMMON","TT_IPDETAILS") %>'
                                target="_blank" runat="server"><%# Container.DataItemToField<string>("IP") %></a>
                    </span>
                </td>
                <td id="history_column" colspan="1" class='<%# Container.DataItemToField<bool>("IsModeratorChanged") ?  "post_res" : "postheader" %>'
                     visible='<%# (Container.DataItemToField<DateTime>("Edited") != Container.DataItemToField<DateTime>("Posted")) %>'
                    runat="server" style="width:20%;vertical-align:top;">
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
                    <span id="IPSpan1" runat="server" visible='<%# PageContext.IsAdmin || (PageContext.BoardSettings.AllowModeratorsViewIPs && PageContext.IsModerator)%>'>
                        <strong>
                            <%# this.GetText("IP") %>:</strong><a id="IPLink1" href='<%# string.Format(this.PageContext.BoardSettings.IPInfoPageURL,Container.DataItemToField<string>("IP")) %>'
                                title='<%# this.GetText("COMMON","TT_IPDETAILS") %>'
                                target="_blank" runat="server"><%# Container.DataItemToField<string>("IP") %></a>
                    </span>
                </td>
                <td class="post">
                    <YAF:MessagePostData ID="MessagePostPrimary" runat="server" ShowAttachments="false"
                        ShowSignature="false" DataRow="<%# PageContext.IsAdmin || PageContext.IsModerator ? Container.DataItem : null %>">
                    </YAF:MessagePostData>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Repeater ID="CurrentMessageRpt" Visible="false" runat="server">
        <ItemTemplate>
            <tr class="header2">
                <td><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="MESSAGEHISTORY"
                        LocalizedTag="CURRENTMESSAGE" /> </td>
                <td style="vertical-align:top">
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITEDBY" />
                    <YAF:UserLink ID="UserLink3" runat="server" UserID='<%# Container.DataItemToField<int>("EditedBy") %>' />&nbsp;-&nbsp;
                    <%# this.Get<IDateTime>().FormatDateTimeTopic( Container.DataItemToField<DateTime>("Edited") ) %>
                </td>
            </tr>
                <td class='<%# Container.DataItemToField<bool>("IsModeratorChanged") ?  "post_res" : "postheader" %>'
                    runat="server" style="vertical-align:top">
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITED" />
                    </strong>
                    <%# this.Get<IDateTime>().FormatDateTimeTopic( Container.DataItemToField<DateTime>("Edited") ) %>
                    <br />
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITEDBY" />
                    </strong>
                    <YAF:UserLink ID="UserLink2" runat="server" UserID='<%# Container.DataItemToField<int>("EditedBy") %>' />
                    <br />
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITREASON" />
                    </strong>
                    <%# Container.DataItemToField<string>("EditReason").IsNotSet() ? this.GetText("EDIT_REASON_NA") : Container.DataItemToField<string>("EditReason") %>
                    <br />
                    <span id="IPSpan3" runat="server" visible='<%# PageContext.IsAdmin || (PageContext.BoardSettings.AllowModeratorsViewIPs && PageContext.IsModerator)%>'>
                        <strong>
                            <%# this.GetText("IP") %>:</strong><a id="IPLink3" href='<%# string.Format(this.PageContext.BoardSettings.IPInfoPageURL,Container.DataItemToField<string>("IP")) %>'
                                title='<%# this.GetText("COMMON","TT_IPDETAILS") %>'
                                target="_blank" runat="server"><%# Container.DataItemToField<string>("IP") %></a>
                    </span>
                    <br />
                </td>
                <td class="post" style="vertical-align:top">
                    <YAF:MessagePostData ID="MessagePostCurrent" runat="server" ShowAttachments="false"
                        ShowSignature="false" DataRow="<%# PageContext.IsAdmin || PageContext.IsModerator ? Container.DataItem : null %>">
                    </YAF:MessagePostData>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr class="postfooter">
        <td></td>
        <td>
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

<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.LastPosts"
    CodeBehind="LastPosts.ascx.cs" %>
<%@ Import Namespace="YAF.Utils" %>
<asp:Timer ID="LastPostUpdateTimer" runat="server" Interval="30000" OnTick="LastPostUpdateTimer_Tick">
</asp:Timer>
<div style="overflow: scroll; height: 400px;">
    <asp:UpdatePanel ID="LastPostUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:UpdatePanel ID="InnerUpdatePanel" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="LastPostUpdateTimer" />
                </Triggers>
                <ContentTemplate>
                    <table class="content LastPosts" width="100%" align="center">
                        <asp:Repeater ID="repLastPosts" runat="server">
                            <HeaderTemplate>
                                <tr>
                                    <td class="header2" align="center" colspan="2">
                                        <YAF:LocalizedLabel ID="Last10" LocalizedTag="LAST10" runat="server" />
                                    </td>
                                </tr>
                            </HeaderTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <ItemTemplate>
                                <tr class="postheader">
                                    <td width="20%">
                                        <strong>
                                            <YAF:UserLink ID="ProfileLink" runat="server" UserID='<%# Container.DataItemToField<int>("UserID") %>' ReplaceName='<%# Container.DataItemToField<string>("UserName") %>'
                                                BlankTarget="true" />
                                        </strong>
                                    </td>
                                    <td width="80%" class="small" align="left">
                                        <strong>
                                            <YAF:LocalizedLabel ID="Posted" LocalizedTag="POSTED" runat="server" />
                                        </strong>
                                        <YAF:DisplayDateTime id="DisplayDateTime" runat="server" DateTime='<%# Container.DataItemToField<DateTime>("Posted") %>'></YAF:DisplayDateTime>
                                    </td>
                                </tr>
                                <tr class="post">
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td valign="top" class="message">
                                        <YAF:MessagePostData ID="MessagePostPrimary" runat="server" DataRow="<%# Container.DataItem %>"
                                            ShowAttachments="false">
                                        </YAF:MessagePostData>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
                                <tr class="postheader">
                                    <td width="20%">
                                        <strong>
                                            <YAF:UserLink ID="ProfileLinkAlt" runat="server" UserID='<%# Container.DataItemToField<int>("UserID") %>' ReplaceName='<%# Container.DataItemToField<string>("UserName") %>'
                                                BlankTarget="true" />
                                        </strong>
                                    </td>
                                    <td width="80%" class="small" align="left">
                                        <strong>
                                            <YAF:LocalizedLabel ID="PostedAlt" LocalizedTag="POSTED" runat="server" />
                                        </strong>
                                        <YAF:DisplayDateTime id="DisplayDateTime" runat="server" DateTime='<%# Container.DataItemToField<DateTime>("Posted") %>'></YAF:DisplayDateTime>
                                    </td>
                                </tr>
                                <tr class="post_alt">
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td valign="top" class="message">
                                        <YAF:MessagePostData ID="MessagePostAlt" runat="server" DataRow="<%# Container.DataItem %>"
                                            ShowAttachments="false">
                                        </YAF:MessagePostData>
                                    </td>
                                </tr>
                            </AlternatingItemTemplate>
                        </asp:Repeater>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

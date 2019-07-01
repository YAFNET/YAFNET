<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.LastPosts"
    CodeBehind="LastPosts.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
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
                        <asp:Repeater ID="repLastPosts" runat="server">
                            <HeaderTemplate>
                                <div class="row">
                                <div class="col">
                                <div class="card mb-3">
                                <div class="card-header">
                                    <i class="fa fa-comment fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="Last10" LocalizedTag="LAST10" runat="server" />
                                </div>
                                <div class="card-body">
                            </HeaderTemplate>
                            <FooterTemplate>
                            </div>
                            </div>
                            </div>
                            </div>
                            </FooterTemplate>
                            <ItemTemplate>
                                <div class="card mb-3">
                                    <div class="card-body">
                                        <h5 class="card-title">
                                             <YAF:UserLink ID="ProfileLink" runat="server" UserID='<%# Container.DataItemToField<int>("UserID") %>' 
                                                                              ReplaceName='<%# this.Get<YafBoardSettings>().EnableDisplayName ? Container.DataItemToField<string>("DisplayName") : Container.DataItemToField<string>("UserName") %>' 
                                                                              BlankTarget="true" />

                                        </h5>
                                        <div class="card-text">
                                             <YAF:MessagePostData ID="MessagePostPrimary" runat="server" DataRow='<%# Container.DataItem %>'
                                                                                   ShowAttachments="false">
                                        </YAF:MessagePostData>
                                        </div>
                                        
                                    </div>
                                    <div class="card-footer">
                                        <small class="text-muted">
                                        <YAF:LocalizedLabel ID="Posted" LocalizedTag="POSTED" runat="server" />
                                        <YAF:DisplayDateTime id="DisplayDateTime" runat="server" DateTime='<%# Container.DataItemToField<DateTime>("Posted") %>'></YAF:DisplayDateTime>
                                        </small>
                                    </div>
                                </div>

                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

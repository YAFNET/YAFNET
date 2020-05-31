<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.LastPosts"
    CodeBehind="LastPosts.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="YAF.Types.Models" %>

<asp:Repeater ID="repLastPosts" runat="server">
    <HeaderTemplate>
        <div class="row">
            <div class="col">
        <div class="card mb-3">
        <div class="card-header">
            <YAF:IconHeader runat="server"
                            IconName="comment"
                            LocalizedTag="LAST10" />
        </div>
        <div class="card-body p-2" style="overflow-y: auto; height: 400px;">
    </HeaderTemplate>
    <FooterTemplate>
    </div>
    </div>
    </div>
    </div>
    </FooterTemplate>
    <ItemTemplate>
        <div class="card my-3">
            <div class="card-body">
                <div class="card-title h5">
                    <footer class="blockquote-footer">
                        <YAF:UserLink ID="ProfileLink" runat="server" 
                                      UserID="<%# ((Message)Container.DataItem).UserID %>"
                                      BlankTarget="true" />
                        <small class="text-muted">
                            <YAF:Icon runat="server" 
                                      IconName="calendar-day"
                                      IconNameBadge="clock"></YAF:Icon>
                            <YAF:DisplayDateTime ID="DisplayDateTime" runat="server" DateTime="<%# ((Message)Container.DataItem).Posted %>"></YAF:DisplayDateTime>
                        </small>
                    </footer>
                </div>
                <div class="card-text">
                    <YAF:MessagePostData ID="MessagePostPrimary" runat="server" 
                                         CurrentMessage="<%# ((Message)Container.DataItem)%>"
                                         ShowAttachments="false">
                    </YAF:MessagePostData>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
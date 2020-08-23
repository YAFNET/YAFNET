<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.LastPosts"
    CodeBehind="LastPosts.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Models" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

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
                                      ReplaceName="<%# ((Tuple<Message, User>)Container.DataItem).Item2.DisplayOrUserName() %>"
                                      UserID="<%# ((Tuple<Message, User>)Container.DataItem).Item2.ID %>"
                                      Suspended="<%# ((Tuple<Message, User>)Container.DataItem).Item2.Suspended %>"
                                      Style="<%# ((Tuple<Message, User>)Container.DataItem).Item2.UserStyle %>"
                                      BlankTarget="true" />
                        <small class="text-muted">
                            <YAF:Icon runat="server" 
                                      IconName="calendar-day"
                                      IconNameBadge="clock"></YAF:Icon>
                            <YAF:DisplayDateTime ID="DisplayDateTime" runat="server" 
                                                 DateTime="<%# ((Tuple<Message, User>)Container.DataItem).Item1.Posted %>" />
                        </small>
                    </footer>
                </div>
                <div class="card-text">
                    <YAF:MessagePostData ID="MessagePostPrimary" runat="server" 
                                         CurrentMessage="<%# ((Tuple<Message, User>)Container.DataItem).Item1%>"
                                         ShowAttachments="false">
                    </YAF:MessagePostData>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
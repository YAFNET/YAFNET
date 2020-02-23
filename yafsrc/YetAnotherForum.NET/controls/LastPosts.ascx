<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.LastPosts"
    CodeBehind="LastPosts.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="System.Data" %>

<asp:Repeater ID="repLastPosts" runat="server">
    <HeaderTemplate>
        <div class="row">
            <div class="col">
        <div class="card mb-3">
        <div class="card-header">
            <i class="fas fa-comment fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="Last10" LocalizedTag="LAST10" runat="server" />
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
                                      UserID='<%# Container.DataItemToField<int>("UserID") %>'
                                      ReplaceName='<%# this.Get<BoardSettings>().EnableDisplayName ? Container.DataItemToField<string>("DisplayName") : Container.DataItemToField<string>("UserName") %>'
                                      BlankTarget="true" />
                    </footer>
                </div>
                <div class="card-text">
                    <YAF:MessagePostData ID="MessagePostPrimary" runat="server" 
                                         DataRow="<%# (DataRow)Container.DataItem %>"
                                         ShowAttachments="false">
                    </YAF:MessagePostData>
                </div>
            </div>
            <div class="card-footer">
                <small class="text-muted">
                    <YAF:LocalizedLabel ID="Posted" LocalizedTag="POSTED" runat="server" />
                    <YAF:DisplayDateTime ID="DisplayDateTime" runat="server" DateTime='<%# Container.DataItemToField<DateTime>("Posted") %>'></YAF:DisplayDateTime>
                </small>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
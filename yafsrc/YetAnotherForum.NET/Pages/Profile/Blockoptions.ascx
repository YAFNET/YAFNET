<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Profile.BlockOptions" CodeBehind="BlockOptions.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Models" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
    <div class="col">
        <div class="row">
            <div class="col">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconName="user-lock"
                                        LocalizedTag="BLOCK_OPTIONS" />
                    </div>
                    <div class="card-body">
                        <h6 class="card-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel200" runat="server" 
                                                LocalizedTag="SELECT_OPTIONS" />
                        </h6>
                        <div class="mb-3">
                            <asp:CheckBox runat="server" ID="BlockPMs" 
                                      CssClass="form-check"/>
                        </div>
                        <div class="mb-3">
                            <asp:CheckBox runat="server" ID="BlockFriendRequests" 
                                          CssClass="form-check"/>
                        </div>
                        <div class="mb-3">
                            <asp:CheckBox runat="server" ID="BlockEmails" 
                                          CssClass="form-check"/>
                        </div>
                        <YAF:Alert runat="server" Type="info">
                            <YAF:Icon runat="server"
                                      IconName="info-circle"/>
                            <YAF:LocalizedLabel runat="server" LocalizedTag="NOTE_BLOCK"></YAF:LocalizedLabel>
                        </YAF:Alert>
                        <div class="text-lg-center">
                            <YAF:ThemeButton ID="SaveUser" runat="server"
                                             OnClick="SaveUser_OnClick"
                                             TextLocalizedTag="SAVE"
                                             Type="Primary"
                                             Icon="save"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="IgnoredUserHolder">
    <div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="users"
                                LocalizedTag="IGNORED_USERS" />
            </div>
            <div class="card-body">
                <asp:Repeater ID="UserIgnoredList" runat="server" OnItemCommand="IgnoredItemCommand">
                    <HeaderTemplate>
                        <ul class="list-group list-group-flush">
                    </HeaderTemplate>
                    <FooterTemplate>
                                    </ul>
                    </FooterTemplate>
                    <ItemTemplate>
                        <li class="list-group-item">
                            <YAF:UserLink runat="server" 
                                          Suspended="<%# ((User)Container.DataItem).Suspended %>"
                                          Style="<%# ((User)Container.DataItem).UserStyle %>"
                                          ReplaceName="<%# ((User)Container.DataItem).DisplayOrUserName() %>"
                                          UserID="<%# ((User)Container.DataItem).ID %>"/>
                            <YAF:ThemeButton runat="server"
                                             Type="Secondary"
                                             Icon="eye"
                                             TextLocalizedPage="POSTS"
                                             TextLocalizedTag="TOGGLEUSERPOSTS_SHOW"
                                             CommandName="delete"
                                             CommandArgument="<%# ((User)Container.DataItem).ID %>" />
                            
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="card-footer text-body-secondary">
                <YAF:LocalizedLabel runat="server" LocalizedTag="NOTE_USERS" />
            </div>
        </div>
    </div>
</div>
        </asp:PlaceHolder>
        </div>
</div>
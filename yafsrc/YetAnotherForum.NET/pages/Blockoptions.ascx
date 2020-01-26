<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.BlockOptions" CodeBehind="BlockOptions.ascx.cs" %>

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
                        <i class="fas fa-user-lock fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" 
                                                                                                      LocalizedTag="BLOCK_OPTIONS" />
                    </div>
                    <div class="card-body">
                        <h6 class="card-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel200" runat="server" 
                                                LocalizedTag="SELECT_OPTIONS" />
                        </h6>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="BlockPMs" 
                                      CssClass="custom-control custom-checkbox"/>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="BlockFriendRequests" 
                                          CssClass="custom-control custom-checkbox"/>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="BlockEmails" 
                                          CssClass="custom-control custom-checkbox"/>
                        </div>
                        <YAF:Alert runat="server" Type="info">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="NOTE_BLOCK"></YAF:LocalizedLabel>
                        </YAF:Alert>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton ID="SaveUser" runat="server"
                                         OnClick="SaveUser_OnClick"
                                         TextLocalizedTag="SAVE"
                                         Type="Primary"
                                         Icon="save"/>
                    </div>
                </div>
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="IgnoredUserHolder">
    <div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-users fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
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
                                          UserID='<%# this.Eval("IgnoredUserID") %>'></YAF:UserLink>
                            &nbsp;<YAF:ThemeButton runat="server"
                                                   Type="Secondary"
                                                   Icon="eye"
                                                   TextLocalizedPage="POSTS"
                                                   TextLocalizedTag="TOGGLEUSERPOSTS_SHOW"
                                                   CommandName="delete"
                                                   CommandArgument='<%# this.Eval("IgnoredUserID") %>'>
                            </YAF:ThemeButton>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="card-footer text-muted">
                <YAF:LocalizedLabel runat="server" LocalizedTag="NOTE_USERS"></YAF:LocalizedLabel>
            </div>
        </div>
    </div>
</div>
        </asp:PlaceHolder>
        </div>
</div>
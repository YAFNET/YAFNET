<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.AccessMasks" Codebehind="AccessMasks.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Types.Models" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="universal-access"
                                LocalizedPage="ADMIN_ACCESSMASKS"></YAF:IconHeader>
            </div>
            <div class="card-body">
                <asp:Repeater ID="List" runat="server" OnItemCommand="ListItemCommand">
                    <HeaderTemplate>
                        <ul class="list-group">
                        </HeaderTemplate>
                    <ItemTemplate>
                <li class="list-group-item list-group-item-action list-group-item-menu">
                    <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1"><%# this.Eval( "Name") %></h5>
                        <small class="d-none d-md-block">
                            <span class="fw-bold">
                                <YAF:LocalizedLabel runat="server" LocalizedTag="SORT_ORDER"></YAF:LocalizedLabel>
                            </span>
                            <%# this.Eval( "SortOrder") %>
                        </small>
                    </div>
                    <p>
                        <ul class="list-inline">
                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                                    LocalizedTag="READ" LocalizedPage="ADMIN_ACCESSMASKS" />:
                                <asp:Label ID="Label1" runat="server"
                                           CssClass="<%# this.GetItemColor(Container.DataItem.ToType<AccessMask>().AccessFlags.ReadAccess) %>">
                                    <%# this.GetItemName(Container.DataItem.ToType<AccessMask>().AccessFlags.ReadAccess) %>
                                </asp:Label>
                            </li>
                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server"
                                                    LocalizedTag="POST" LocalizedPage="ADMIN_ACCESSMASKS" />:
                                <asp:Label ID="Label2" runat="server"
                                           CssClass="<%# this.GetItemColor(Container.DataItem.ToType<AccessMask>().AccessFlags.PostAccess) %>">
                                    <%# this.GetItemName(Container.DataItem.ToType<AccessMask>().AccessFlags.PostAccess) %>
                                </asp:Label>
                            </li>
                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server"
                                                    LocalizedTag="REPLY" LocalizedPage="ADMIN_ACCESSMASKS" />:
                                <asp:Label ID="Label3" runat="server"
                                           CssClass="<%# this.GetItemColor(Container.DataItem.ToType<AccessMask>().AccessFlags.ReplyAccess) %>">
                                    <%# this.GetItemName(Container.DataItem.ToType<AccessMask>().AccessFlags.ReplyAccess) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server"
                                                    LocalizedTag="PRIORITY" LocalizedPage="ADMIN_ACCESSMASKS" />:
                                <asp:Label ID="Label4" runat="server"
                                           CssClass="<%# this.GetItemColor(Container.DataItem.ToType<AccessMask>().AccessFlags.PriorityAccess) %>">
                                    <%# this.GetItemName(Container.DataItem.ToType<AccessMask>().AccessFlags.PriorityAccess) %>
                                </asp:Label>

                            </li>
                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server"
                                                    LocalizedTag="POLL" LocalizedPage="ADMIN_ACCESSMASKS" />:
                                <asp:Label ID="Label5" runat="server"
                                           CssClass="<%# this.GetItemColor(Container.DataItem.ToType<AccessMask>().AccessFlags.PollAccess) %>">
                                    <%# this.GetItemName(Container.DataItem.ToType<AccessMask>().AccessFlags.PollAccess) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server"
                                                    LocalizedTag="VOTE" LocalizedPage="ADMIN_ACCESSMASKS" />:
                                <asp:Label ID="Label6" runat="server"
                                           CssClass="<%# this.GetItemColor(Container.DataItem.ToType<AccessMask>().AccessFlags.VoteAccess) %>">
                                    <%# this.GetItemName(Container.DataItem.ToType<AccessMask>().AccessFlags.VoteAccess) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server"
                                                    LocalizedTag="MODERATOR" LocalizedPage="ADMIN_ACCESSMASKS" />:
                                <asp:Label ID="Label7" runat="server"
                                           CssClass="<%# this.GetItemColor(Container.DataItem.ToType<AccessMask>().AccessFlags.ModeratorAccess) %>">
                                    <%# this.GetItemName(Container.DataItem.ToType<AccessMask>().AccessFlags.ModeratorAccess) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server"
                                                    LocalizedTag="EDIT" LocalizedPage="ADMIN_ACCESSMASKS" />:
                                <asp:Label ID="Label8" runat="server"
                                           CssClass="<%# this.GetItemColor(Container.DataItem.ToType<AccessMask>().AccessFlags.EditAccess) %>">
                                    <%# this.GetItemName(Container.DataItem.ToType<AccessMask>().AccessFlags.EditAccess) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server"
                                                    LocalizedTag="DELETE" LocalizedPage="ADMIN_ACCESSMASKS"/>:
                                <asp:Label ID="Label9" runat="server"
                                           CssClass="<%# this.GetItemColor(Container.DataItem.ToType<AccessMask>().AccessFlags.DeleteAccess) %>">
                                    <%# this.GetItemName(Container.DataItem.ToType<AccessMask>().AccessFlags.DeleteAccess) %>
                                </asp:Label>
                            </li>
                    </ul>
                        </p>
                    <small>
                        <div class="btn-group btn-group-sm">
                            <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                                             Type="Info"
                                             Size="Small"
                                             CommandName="edit"
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TitleLocalizedTag="EDIT"
                                             Icon="edit"
                                             TextLocalizedTag="EDIT">
                            </YAF:ThemeButton>
                            <YAF:ThemeButton ID="ThemeButtonDelete" runat="server"
                                             Type="Danger"
                                             Size="Small"
                                             CommandName="delete"
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TitleLocalizedTag="DELETE"
                                             Icon="trash"
                                             TextLocalizedTag="DELETE"
                                             ReturnConfirmTag="CONFIRM_DELETE">
                            </YAF:ThemeButton>
                        </div>

                        <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                            <YAF:ThemeButton ID="ThemeButton1" runat="server"
                                             Type="None"
                                             CssClass="dropdown-item"
                                             CommandName="edit"
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TitleLocalizedTag="EDIT"
                                             Icon="edit"
                                             TextLocalizedTag="EDIT">
                            </YAF:ThemeButton>
                            <YAF:ThemeButton ID="ThemeButton2" runat="server"
                                             Type="None"
                                             CssClass="dropdown-item"
                                             CommandName="delete"
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TitleLocalizedTag="DELETE"
                                             Icon="trash"
                                             TextLocalizedTag="DELETE"
                                             ReturnConfirmTag="CONFIRM_DELETE">
                            </YAF:ThemeButton>
                            <div class="dropdown-divider"></div>
                            <YAF:ThemeButton ID="New" runat="server"
                                             OnClick="NewClick"
                                             Type="None"
                                             CssClass="dropdown-item"
                                             Icon="plus-square"
                                             TextLocalizedTag="NEW_MASK" />
                        </div>
                    </small>
                </li>
            </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="New" runat="server"
                                 OnClick="NewClick"
                                 Type="Primary"
                                 Icon="plus-square"
                                 TextLocalizedTag="NEW_MASK" />
            </div>
        </div>
    </div>
</div>
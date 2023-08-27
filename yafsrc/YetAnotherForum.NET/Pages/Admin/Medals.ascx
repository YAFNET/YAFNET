<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.Medals" Codebehind="Medals.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconType="text-secondary"
                                    IconType="text-secondary"
                                    IconName="medal"
                                    LocalizedPage="ADMIN_MEDALS"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <YAF:EmptyState runat="server" ID="EmptyState" Visible="False"
                                    Icon="medal"
                                    HeaderTextPage="ADMIN_MEDALS" HeaderTextTag="EMPTY_HEADER"
                                    MessageTextPage="ADMIN_MEDALS" MessageTextTag="EMPTY_MESSAGE"/>
                    <asp:Repeater ID="MedalList"
                                  OnItemCommand="MedalListItemCommand" runat="server">
                        <HeaderTemplate>
                            <ul class="list-group">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item list-group-item-action list-group-item-menu">
                             <div class="d-flex w-100 justify-content-between">
                                 <h5 class="mb-1 text-break">
                                     <%# this.RenderImages(Container.DataItem) %>
                                     <%# this.Eval("Name") %>
                                 </h5>
                                 <small class="d-none d-md-block">
                                     <span class="fw-bold">
                                         <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server"
                                                             LocalizedTag="CATEGORY"
                                                             LocalizedPage="MODERATE_DEFAULT" />:
                                     </span>
                                     <%# this.Eval("Category") %>
                                 </small>
                             </div>
                                <p class="mb-1">
                                <span class="fw-bold">
                                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server"
                                                        LocalizedTag="DESCRIPTION"
                                                        LocalizedPage="ADMIN_BBCODE" />:
                                </span>
                                <%# this.Eval("Description") %>
                            </p>
                            <small>
                                <div class="btn-group btn-group-sm">
                                    <YAF:ThemeButton ID="ThemeButtonEdit"
                                                     Type="Info"
                                                     Size="Small"
                                                     CommandName="edit"
                                                     CommandArgument='<%# this.Eval( "ID") %>'
                                                     TitleLocalizedTag="EDIT"
                                                     Icon="edit"
                                                     TextLocalizedTag="EDIT"
                                                     runat="server" />
                                    <YAF:ThemeButton ID="ThemeButtonDelete"
                                                     Type="Danger"
                                                     Size="Small"
                                                     CommandName="delete"
                                                     CommandArgument='<%# this.Eval( "ID") %>'
                                                     TitleLocalizedTag="DELETE"
                                                     Icon="trash"
                                                     TextLocalizedTag="DELETE"
                                                     ReturnConfirmTag="CONFIRM_DELETE"
                                                     runat="server"/>
                                </div>
                            </small>
                                <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                                    <YAF:ThemeButton ID="ThemeButton1"
                                                     Type="None"
                                                     CssClass="dropdown-item"
                                                 CommandName="edit"
                                                 CommandArgument='<%# this.Eval( "ID") %>'
                                                 TitleLocalizedTag="EDIT"
                                                 Icon="edit"
                                                 TextLocalizedTag="EDIT"
                                                 runat="server">
                                </YAF:ThemeButton>
                                    <YAF:ThemeButton ID="ThemeButton4"
                                                     Type="None"
                                                     CssClass="dropdown-item"
                                                     CommandName="delete"
                                                     CommandArgument='<%# this.Eval( "ID") %>'
                                                     TitleLocalizedTag="DELETE"
                                                     Icon="trash"
                                                     TextLocalizedTag="DELETE"
                                                     ReturnConfirmTag="CONFIRM_DELETE"
                                                     runat="server">
                                    </YAF:ThemeButton>
                                    <div class="dropdown-divider"></div>
                                    <YAF:ThemeButton ID="NewMedal" runat="server"
                                                     OnClick="NewMedalClick"
                                                     Type="None"
                                                     CssClass="dropdown-item"
                                                     Icon="plus-square"
                                                     TextLocalizedTag="NEW_MEDAL" />
                                </div>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="NewMedal" runat="server"
                                     OnClick="NewMedalClick"
                                     Type="Primary"
                                     Icon="plus-square"
                                     TextLocalizedTag="NEW_MEDAL" />
                </div>
            </div>
        </div>
    </div>



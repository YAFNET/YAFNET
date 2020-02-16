<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.medals" Codebehind="medals.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_MEDALS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-trophy fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                                LocalizedTag="TITLE" 
                                                                                LocalizedPage="ADMIN_MEDALS" />
                </div>
                <div class="card-body">
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
                                     <%# this.Eval( "Name") %>
                                 </h5>
                                 <small class="d-none d-md-block">
                                     <%# this.Eval("SortOrder") %>
                                 </small>
                             </div>
                            <p class="mb-1">
                                <span class="font-weight-bold">
                                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                                        LocalizedTag="CATEGORY" 
                                                        LocalizedPage="MODERATE_DEFAULT" />:
                                </span>
                                <%# this.Eval("Category") %>
                            </p>
                            <p class="mb-1">
                                <span class="font-weight-bold">
                                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                                        LocalizedTag="DESCRIPTION" 
                                                        LocalizedPage="ADMIN_BBCODE" />:
                                </span>
                                <%# this.Eval("Description") %>
                            </p>
                            <small>
                                <YAF:ThemeButton ID="ThemeButtonEdit" 
                                                 Type="Info" 
                                                 Size="Small"
                                                 CommandName="edit" 
                                                 CommandArgument='<%# this.Eval( "ID") %>'
                                                 TitleLocalizedTag="EDIT"
                                                 Icon="edit"
                                                 TextLocalizedTag="EDIT"
                                                 runat="server">
                                </YAF:ThemeButton>
                                <YAF:ThemeButton ID="ThemeButtonMoveUp" 
                                                 Type="Warning"
                                                 Size="Small"
                                                 CommandName="moveup" 
                                                 CommandArgument='<%# this.Eval("ID") %>'
                                                 TitleLocalizedTag="MOVE_UP"
                                                 Icon="level-up-alt"
                                                 TextLocalizedTag="MOVE_UP"
                                                 runat="server"/>
                                <YAF:ThemeButton ID="ThemeButtonMoveDown" 
                                                 Type="Warning"
                                                 Size="Small"
                                                 CommandName="movedown" 
                                                 CommandArgument='<%# this.Eval("ID") %>'
                                                 TitleLocalizedTag="MOVE_DOWN"
                                                 Icon="level-down-alt"
                                                 TextLocalizedTag="MOVE_DOWN"
                                                 runat="server" />
                                <YAF:ThemeButton ID="ThemeButtonDelete" 
                                                 Type="Danger" 
                                                 Size="Small"
                                                 CommandName="delete" 
                                                 CommandArgument='<%# this.Eval( "ID") %>'
                                                 TitleLocalizedTag="DELETE"
                                                 Icon="trash"
                                                 TextLocalizedTag="DELETE"
                                                 ReturnConfirmText='<%# this.GetText("ADMIN_MEDALS", "CONFIRM_DELETE") %>'
                                                 runat="server">
                                </YAF:ThemeButton>
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
                                                     ReturnConfirmText='<%# this.GetText("ADMIN_MEDALS", "CONFIRM_DELETE") %>'
                                                     runat="server">
                                    </YAF:ThemeButton>
                                    <div class="dropdown-divider"></div>
                                    <YAF:ThemeButton ID="ThemeButton2" 
                                                     Type="None" 
                                                     CssClass="dropdown-item"
                                                     CommandName="moveup" 
                                                     CommandArgument='<%# this.Eval("ID") %>'
                                                     TitleLocalizedTag="MOVE_UP"
                                                     Icon="level-up-alt"
                                                     TextLocalizedTag="MOVE_UP"
                                                     runat="server"/>
                                    <YAF:ThemeButton ID="ThemeButton3" 
                                                     Type="None" 
                                                     CssClass="dropdown-item"
                                                     CommandName="movedown" 
                                                     CommandArgument='<%# this.Eval("ID") %>'
                                                     TitleLocalizedTag="MOVE_DOWN"
                                                     Icon="level-down-alt"
                                                     TextLocalizedTag="MOVE_DOWN"
                                                     runat="server" />
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



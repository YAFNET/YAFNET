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
                    <i class="fa fa-trophy fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                                LocalizedTag="TITLE" 
                                                                                LocalizedPage="ADMIN_MEDALS" />
                </div>
                <div class="card-body">
                    <asp:Repeater ID="MedalList" 
                                  OnItemCommand="MedalListItemCommand" runat="server">
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                        <HeaderTemplate>
                            <ul class="list-group">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item list-group-item-action">
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
                                                 CommandName='edit' 
                                                 CommandArgument='<%# this.Eval( "MedalID") %>'
                                                 TitleLocalizedTag="EDIT"
                                                 Icon="edit"
                                                 TextLocalizedTag="EDIT"
                                                 runat="server">
                                </YAF:ThemeButton>
                                <YAF:ThemeButton ID="ThemeButtonMoveUp" 
                                                 CssClass="btn btn-warning btn-sm"
                                                 CommandName='moveup' 
                                                 CommandArgument='<%# this.Eval("MedalID") %>'
                                                 TitleLocalizedTag="MOVE_UP"
                                                 TitleLocalizedPage="ADMIN_SMILIES"
                                                 Icon="level-up-alt"
                                                 TextLocalizedTag="MOVE_UP"
                                                 TextLocalizedPage="ADMIN_SMILIES"
                                                 runat="server"/>
                                <YAF:ThemeButton ID="ThemeButtonMoveDown" 
                                                 CssClass="btn btn-warning btn-sm"
                                                 CommandName='movedown' 
                                                 CommandArgument='<%# this.Eval("MedalID") %>'
                                                 TitleLocalizedTag="MOVE_DOWN"
                                                 TitleLocalizedPage="ADMIN_SMILIES"
                                                 Icon="level-down-alt"
                                                 TextLocalizedTag="MOVE_DOWN"
                                                 TextLocalizedPage="ADMIN_SMILIES"
                                                 runat="server" />
                                <YAF:ThemeButton ID="ThemeButtonDelete" 
                                                 Type="Danger" 
                                                 Size="Small"
                                                 CommandName='delete' 
                                                 CommandArgument='<%# this.Eval( "MedalID") %>'
                                                 TitleLocalizedTag="DELETE"
                                                 Icon="trash"
                                                 TextLocalizedTag="DELETE"
                                                 ReturnConfirmText='<%# this.GetText("ADMIN_MEDALS", "CONFIRM_DELETE") %>'
                                                 runat="server">
                                </YAF:ThemeButton>
                            </small>
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



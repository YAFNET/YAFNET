<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.medals" Codebehind="medals.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_MEDALS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-trophy fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_MEDALS" />
                </div>
                <div class="card-body">
		<asp:Repeater ID="MedalList" OnItemCommand="MedalListItemCommand" runat="server">
			<HeaderTemplate>
			    <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                    <table class="table">
				<tr>
				    <thead>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="ORDER" /></th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="IMAGE_TEXT" /></th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NAME" LocalizedPage="COMMON" /></th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="CATEGORY" LocalizedPage="MODERATE_DEFAULT" /></th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_BBCODE" /></th>
					<th>
						</th>
                        </thead>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td>
						<%# this.Eval( "SortOrder") %>
					</td>
					<td>
						<%# this.RenderImages(Container.DataItem) %>
					</td>
					<td>
						<%# this.Eval( "Name") %>
					</td>
					<td>
						<%# this.Eval( "Category") %>
					</td>
					<td>
						<%# ((string)this.Eval( "Description")).Substring(0, Math.Min(this.Eval( "Description").ToString().Length, 100)) + "..." %>
					</td>
					<td>
					    <span class="float-right">
					    <YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" CssClass="btn-sm"
                            CommandName='edit' CommandArgument='<%# this.Eval( "MedalID") %>'
                            TitleLocalizedTag="EDIT"
                            Icon="edit"
                            TextLocalizedTag="EDIT"
                            runat="server">
					    </YAF:ThemeButton>
					    <YAF:ThemeButton ID="ThemeButtonMoveUp" CssClass="btn btn-warning btn-sm"
                            CommandName='moveup' CommandArgument='<%# this.Eval("MedalID") %>'
					        TitleLocalizedTag="MOVE_UP"
                            TitleLocalizedPage="ADMIN_SMILIES"
					        Icon="level-up-alt"
					        TextLocalizedTag="MOVE_UP"
                            TextLocalizedPage="ADMIN_SMILIES"
					        runat="server"/>
					    <YAF:ThemeButton ID="ThemeButtonMoveDown" CssClass="btn btn-warning btn-sm"
					        CommandName='movedown' CommandArgument='<%# this.Eval("MedalID") %>'
					        TitleLocalizedTag="MOVE_DOWN"
                            TitleLocalizedPage="ADMIN_SMILIES"
					        Icon="level-down-alt"
					        TextLocalizedTag="MOVE_DOWN"
                            TextLocalizedPage="ADMIN_SMILIES"
					        runat="server" />
						<YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm"
                                    CommandName='delete' CommandArgument='<%# this.Eval( "MedalID") %>'
                                    TitleLocalizedTag="DELETE"
                                    Icon="trash"
                                    TextLocalizedTag="DELETE"
                                    ReturnConfirmText='<%# this.GetText("ADMIN_MEDALS", "CONFIRM_DELETE") %>'
                                    runat="server">
                                </YAF:ThemeButton>
                            </span>
					</td>
				</tr>
			</ItemTemplate>
            <FooterTemplate>
                </table></div>
            </FooterTemplate>
		</asp:Repeater>
                </div>
                <div class="card-footer text-lg-center">
				    <YAF:ThemeButton ID="NewMedal" runat="server" OnClick="NewMedalClick" Type="Primary"
				                     Icon="plus-square" TextLocalizedTag="NEW_MEDAL" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

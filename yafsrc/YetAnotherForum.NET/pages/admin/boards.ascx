<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.boards" Codebehind="boards.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BOARDS" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-globe fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BOARDS" />
                </div>
                <div class="card-body">
                    <p class="card-text">
                        		<asp:Repeater ID="List" runat="server">
		    <HeaderTemplate>
		        <div class="table-responsive">
		       <table class="table">
                <thead>
		        <tr>
			<th>
				<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ID" LocalizedPage="ADMIN_BOARDS" />
			</th>
			<th>
				<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_BOARDS" />
			</th>
                    <th>&nbsp;</th>
		</tr>
                    </thead>
		    </HeaderTemplate>
			<ItemTemplate>
				<tr id="BoardRow" class='<%# this.Eval("BoardID").ToType<int>() != this.PageContext.PageBoardID ? "" : "table-success" %>' runat="server">
					<td>
						<%# this.Eval( "BoardID") %>
					</td>
					<td>
						<%# this.HtmlEncode(this.Eval( "Name")) %>
					</td>
                    <td>
                        <span class="float-right">
					    <YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" CssClass="btn-sm"
                            CommandName='edit' CommandArgument='<%# this.Eval( "BoardID") %>'
                            TitleLocalizedTag="EDIT"
                            TextLocalizedTag="EDIT"
                            Icon="edit"
                            runat="server">
					    </YAF:ThemeButton>
                        &nbsp;
                        <YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm"
                            CommandName='delete' CommandArgument='<%# this.Eval( "BoardID") %>'
                            TitleLocalizedTag="DELETE"
                            TextLocalizedTag="DELETE"
                            Icon="trash"
                            OnLoad="DeleteLoad"  runat="server">
                        </YAF:ThemeButton>
                            </span>
					</td>
				</tr>
			</ItemTemplate>
            <FooterTemplate>
                </table></div>
            </FooterTemplate>
		</asp:Repeater>

                    </p>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="New" runat="server" Type="Primary" 
                        TextLocalizedTag="NEW_BOARD"
                        Icon="plus-square"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

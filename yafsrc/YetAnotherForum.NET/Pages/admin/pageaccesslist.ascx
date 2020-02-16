<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.pageaccesslist" Codebehind="pageaccesslist.ascx.cs" %>


<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_PAGEACCESSLIST" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-building fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_PAGEACCESSLIST" />
                </div>
                <div class="card-body">
		<asp:Repeater ID="List" runat="server" OnItemCommand="ListItemCommand">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
			<ItemTemplate>
                <li class="list-group-item list-group-item-action">
                    <div class="d-flex w-100 justify-content-between">
                        <h5 class="mb-1">
                            <i class="fa fa-user-secret fa-fw"></i>&nbsp;
                            <%# this.HtmlEncode(this.Get<BoardSettings>().EnableDisplayName ? this.Eval("DisplayName") : this.Eval("Name"))%>
                        </h5>
                    </div>
                    <p class="mb-1">
                        <YAF:LocalizedLabel ID="BoardNameLabel" runat="server" 
                                            LocalizedTag="BOARDnAME"  
                                            LocalizedPage="ADMIN_PAGEACCESSLIST" />:
                        <%# this.HtmlEncode(this.Eval( "BoardName")) %>
                    </p>
                    <small>
                        <YAF:ThemeButton ID="ThemeButtonEdit" runat="server" 
                                         Type="Info" 
                                         Size="Small"
                                         TitleLocalizedPage="ADMIN_PAGEACCESSLIST" 
                                         CommandName='edit' 
                                         CommandArgument='<%# this.Eval( "UserID") %>'
                                         TitleLocalizedTag="EDIT"
                                         Icon="edit"
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                    </small>
                </li>
			</ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
		</asp:Repeater>
                    
            </div>
        </div>
    </div>
        </div>
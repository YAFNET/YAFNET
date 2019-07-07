<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.eventloggroups" Codebehind="eventloggroups.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EVENTLOGGROUPS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-users fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EVENTLOGGROUPS" />
                </div>
                <div class="card-body">
		<asp:Repeater ID="List" runat="server" OnItemCommand="ListItemCommand">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
			<ItemTemplate>
                <li class="list-group-item list-group-item-action">
                    <div class="d-flex w-100 justify-content-between">
                        <h5 class="mb-1">
                            <i class="fa fa-users fa-fw"></i>&nbsp;<%# this.HtmlEncode(this.Eval("Name"))%>
                        </h5>
                    </div>
                    <p class="mb-1">
                        <YAF:LocalizedLabel ID="BoardNameLabel" runat="server" LocalizedTag="BOARDNAME"  LocalizedPage="ADMIN_EVENTLOGGROUPS" />:
                        <%# this.HtmlEncode(this.Eval( "BoardName")) %>
                    </p>
                    <small>
                        <YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" Size="Small"
                                         TitleLocalizedPage="ADMIN_EVENTLOGGROUPS" 
                                         TitleLocalizedTag="EDIT"
                                         CommandName='edit' CommandArgument='<%# this.Eval( "GroupID") %>' 
                                         TextLocalizedTag="EDIT"
                                         Icon="edit" runat="server"></YAF:ThemeButton>
                    </small>
                </li>
			</ItemTemplate>
		</asp:Repeater>
                </div>
            </div>
        </div>
    </div>



<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.nntpretrieve" Codebehind="nntpretrieve.ascx.cs" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                LocalizedTag="HEADER" 
                                LocalizedPage="ADMIN_NNTPRETRIEVE" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-newspaper fa-fw text-secondary pr-1"></i>
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                        LocalizedTag="HEADER"
                                        LocalizedPage="ADMIN_NNTPRETRIEVE" />
                    </div>
                <div class="card-body">
                    <asp:Repeater runat="server" ID="List">
			<HeaderTemplate>
                <ul class="list-group">
			</HeaderTemplate>
			<ItemTemplate>
                <li class="list-group-item list-group-item-action">
                    <div class="d-flex w-100 justify-content-between">
                        <h5 class="mb-1">
                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                LocalizedTag="GROUPS" 
                                                LocalizedPage="ADMIN_NNTPRETRIEVE" />:
                        </h5>
                        <small>
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                LocalizedTag="LAST_MESSAGE" 
                                                LocalizedPage="ADMIN_NNTPRETRIEVE" />:&nbsp;
                            <%# this.LastMessageNo(Container.DataItem) %>
                        </small>
                    </div>
                    <p class="mb-1">
                        <%# this.Eval("GroupName") %>
                    </p>
                    <small>
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                LocalizedTag="LAST_UPDATE" 
                                                LocalizedPage="ADMIN_NNTPRETRIEVE" />:&nbsp;
                        </span>
                        <%# this.Get<IDateTime>().FormatDateTime(this.Eval("LastUpdate")) %>
                    </small>
                </li>
			</ItemTemplate>
            <FooterTemplate>
                </ul>

            </FooterTemplate>
		</asp:Repeater>
                        <div class="form-row mt-3">
                            <div class="form-group col-md-6">
                                <asp:Label runat="server" AssociatedControlID="Seconds">
                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                        LocalizedTag="TIME" 
                                                        LocalizedPage="ADMIN_NNTPRETRIEVE" /> 
                                </asp:Label>
                                <div class="input-group">
                                    <asp:TextBox runat="server" ID="Seconds" 
                                                 Text="30" 
                                                 CssClass="form-control" 
                                                 TextMode="Number" />
                                    <div class="input-group-append">
                                        <div class="input-group-text">
                                            <YAF:LocalizedLabel runat="server" 
                                                                LocalizedTag="SECONDS"></YAF:LocalizedLabel>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton runat="server" ID="Retrieve" 
                                     Type="Primary" 
                                     OnClick="RetrieveClick"
                                     Icon="download" 
                                     TextLocalizedTag="RETRIEVE"/>
                </div>
            </div>
            </div>
        </div>



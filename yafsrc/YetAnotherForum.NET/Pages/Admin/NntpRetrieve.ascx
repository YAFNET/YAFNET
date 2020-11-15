<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.NntpRetrieve" Codebehind="NntpRetrieve.ascx.cs" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="newspaper"
                                LocalizedPage="ADMIN_NNTPRETRIEVE"></YAF:IconHeader>
            </div>
            <div class="card-body">
                <YAF:Alert runat="server" Type="danger">
                    <YAF:LocalizedLabel runat="server" 
                                        LocalizedTag="BETA_WARNING" />
                </YAF:Alert>
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
                                    <%# this.Eval("Item1.GroupName") %>
                                </h5>
                                <small>
                                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                        LocalizedTag="LAST_MESSAGE" 
                                                        LocalizedPage="ADMIN_NNTPRETRIEVE" />:&nbsp;
                                    <%# this.LastMessageNo(Container.DataItem) %>
                                </small>
                            </div>
                            <small>
                                <span class="fw-bold">
                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                        LocalizedTag="LAST_UPDATE" 
                                                        LocalizedPage="ADMIN_NNTPRETRIEVE" />:&nbsp;
                                </span>
                                <%# this.Get<IDateTime>().FormatDateTime(this.Eval("Item1.LastUpdate")) %>
                            </small>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Panel runat="server" ID="RetrievePanel"
                           CssClass="mb-3">
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
                        <div class="input-group-text">
                            <YAF:LocalizedLabel runat="server" 
                                                LocalizedTag="SECONDS" />
                        </div>
                    </div>
                </asp:Panel>
                <YAF:Alert runat="server" ID="NoContent" Type="info">
                    <YAF:Icon runat="server" IconName="info-circle" />
                    <YAF:LocalizedLabel runat="server" 
                                        LocalizedTag="NO_GROUPS" />
                </YAF:Alert>
            </div>
            <asp:Panel runat="server" ID="Footer" CssClass="card-footer text-center">
                <YAF:ThemeButton runat="server" ID="Retrieve" 
                                 Type="Primary" 
                                 OnClick="RetrieveClick"
                                 Icon="download" 
                                 TextLocalizedTag="RETRIEVE"/>
            </asp:Panel>
        </div>
    </div>
</div>
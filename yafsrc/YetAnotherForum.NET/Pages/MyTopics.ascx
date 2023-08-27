<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.MyTopics" Codebehind="MyTopics.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="row">
    <div class="col">
        <div class="card my-3">
            <div class="card-header">
                <div class="row justify-content-between align-items-center">
                    <div class="col-auto">
                        <YAF:IconHeader runat="server" ID="IconHeader"
                                        IconType="text-secondary"
                                        IconName="comments"
                                        LocalizedPage="MYTOPICS"
                                        LocalizedTag="MEMBERTITLE" />
                    </div>
                    <div class="col-auto">
                        <div class="btn-toolbar" role="toolbar">
                            <div class="input-group input-group-sm me-2 mb-1" role="group">
                                <div class="input-group-text">
                                    <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                                </div>
                                <asp:DropDownList runat="server" ID="PageSize"
                                                  AutoPostBack="True"
                                                  OnSelectedIndexChanged="PageSizeSelectedIndexChanged"
                                                  CssClass="form-select">
                                </asp:DropDownList>
                            </div>
                            <div class="btn-group me-2 mb-1" role="group" aria-label="Filters">
                        <YAF:ThemeButton runat="server"
                                         CssClass="dropdown-toggle"
                                         DataToggle="dropdown"
                                         Size="Small"
                                         Type="Secondary"
                                         Icon="filter"
                                         TextLocalizedTag="FILTER_DROPDOWN"
                                         TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>

                        <div class="dropdown-menu dropdown-menu-end dropdown-menu-lg-start">
                            <div class="px-3 py-1">
                                <div class="mb-3">
                                    <asp:Label runat="server" AssociatedControlID="TopicMode">
                                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server"
                                                            LocalizedTag="SELECT"/>
                                    </asp:Label>
                                    <asp:DropDownList ID="TopicMode" runat="server" 
                                                      AutoPostBack="True" 
                                                      OnSelectedIndexChanged="TopicModeSelectedIndexChanged" 
                                                      CssClass="form-select" />
                                </div>
                                <div class="mb-3">
                                    <asp:Label runat="server" AssociatedControlID="Since">
                                        <YAF:LocalizedLabel ID="SinceLabel" runat="server"
                                                            LocalizedTag="SINCE"/>
                                    </asp:Label>
                                    <asp:DropDownList ID="Since" runat="server" 
                                                      AutoPostBack="True" 
                                                      OnSelectedIndexChanged="Since_SelectedIndexChanged" 
                                                      CssClass="form-select" />
                                </div>
                            </div>
                        </div>
                        </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <asp:Repeater ID="TopicList" runat="server">
                    <ItemTemplate>
                        <%# this.CreateTopicLine(Container.DataItem) %>
                    </ItemTemplate>
                    <SeparatorTemplate>
                        <div class="row">
                            <div class="col">
                                <hr/>
                            </div>
                        </div>
                    </SeparatorTemplate>
                </asp:Repeater>
                <asp:PlaceHolder runat="server" Visible="<%# this.TopicList.Items.Count == 0 %>">
                    <div class="card-body">
                        <YAF:Alert runat="server" ID="Info"
                                   Type="info">
                            <YAF:Icon runat="server" IconName="info-circle" />
                            <YAF:LocalizedLabel runat="server" LocalizedTag="NO_POSTS" />
                        </YAF:Alert>
                    </div>
                </asp:PlaceHolder>
            </div>
            <div class="card-footer">
                <div class="row justify-content-end">
                    <div class="col-auto">
                        <YAF:ThemeButton runat="server" OnClick="MarkAll_Click" ID="MarkAll"
                                         TextLocalizedTag="MARK_ALL_ASREAD" TextLocalizedPage="DEFAULT"
                                         Type="Secondary"
                                         Size="Small"
                                         Icon="glasses"/>
                    </div>
                </div>
            </div>
    </div>
    </div>
</div>
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager runat="server" ID="PagerTop" 
                   OnPageChange="Pager_PageChange" />
    </div>
</div>
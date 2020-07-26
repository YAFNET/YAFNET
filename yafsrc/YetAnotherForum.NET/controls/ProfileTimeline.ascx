<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="true" Inherits="YAF.Controls.ProfileTimeline"
    CodeBehind="ProfileTimeline.ascx.cs" %>

<YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />

<div class="row">
    <div class="col">
        <div class="card mb-3 mt-2" id="activity">
            <div class="card-header">
                <div class="row justify-content-between">
                    <div class="col-md-3">
                        <YAF:Icon runat="server" 
                                  IconName="stream"
                                  IconType="text-secondary"></YAF:Icon>
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                            LocalizedTag="ACTIVITY"
                                            LocalizedPage="ACCOUNT" />
                    </div>
                    <div class="col-md-2 mt-1">
                        <div class="btn-group" role="group" aria-label="Filters">
                            <YAF:ThemeButton runat="server"
                                     CssClass="dropdown-toggle"
                                     DataToggle="dropdown"
                                     Type="Secondary"
                                     Icon="filter"
                                     TextLocalizedTag="FILTER_DROPDOWN"
                                     TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
                            <div class="dropdown-menu dropdown-menu-right dropdown-menu-lg-left">
                                <div class="px-3 py-1">
                                    <div class="form-group">
                                        <div class="custom-control custom-switch">
                                            <asp:CheckBox runat="server" ID="CreatedTopic" 
                                                          Checked="True"/>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="custom-control custom-switch">
                                            <asp:CheckBox runat="server" ID="CreatedReply" 
                                                          Checked="True"/>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="custom-control custom-switch">
                                            <asp:CheckBox runat="server" ID="GivenThanks" 
                                                          Checked="True"/>
                                        </div>
                                    </div>
                                    <hr />
                                    <YAF:ThemeButton runat="server" ID="Update"
                                                     OnClick="UpdateFilterClick"
                                                     TextLocalizedTag="UPDATE"
                                                     Size="Small"
                                                     Icon="sync">
                                    </YAF:ThemeButton>
                                    &nbsp;
                                    <YAF:ThemeButton ID="Reset" runat="server"
                                                     OnClick="ResetClick"
                                                     TextLocalizedTag="CLEAR"
                                                     Type="Secondary"
                                                     Size="Small"
                                                     Icon="trash">
                                    </YAF:ThemeButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <asp:Repeater runat="server" ID="ActivityStream" 
                              OnItemDataBound="ActivityStream_OnItemDataBound" 
                              OnItemCommand="ActivityStream_OnItemCommand">
                    <HeaderTemplate>
                        <div class="container">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="row">
                            <div class="col-auto text-center flex-column d-none d-sm-flex">
                                <div class="row h-50">
                                    <div class="col <%# this.GetFirstItemClass(Container.ItemIndex) %>">&nbsp;</div>
                                    <div class="col">&nbsp;</div>
                                </div>
                                <h5 class="m-2">
                                    <asp:Label runat="server" ID="Icon"
                                               CssClass="fa-stack fa-1x" ></asp:Label>
                                </h5>
                                <div class="row h-50">
                                    <div class="col <%# this.GetLastItemClass(Container.ItemIndex) %>">&nbsp;</div>
                                    <div class="col">&nbsp;</div>
                                </div>
                            </div>
                            <div class="col py-2">
                                <asp:Panel runat="server" ID="Card">
                                    <div class="card-body py-2">
                                        <div class="d-flex w-100 justify-content-between">
                                            <h5 class="card-title mb-1 text-break d-none d-md-block">
                                                <asp:Literal runat="server" ID="Title"></asp:Literal>
                                            </h5>
                                            <small class="d-none d-md-block">
                                                <YAF:Icon runat="server" 
                                                          IconName="calendar-day"
                                                          IconType="text-secondary"
                                                          IconNameBadge="clock" 
                                                          IconBadgeType="text-secondary"></YAF:Icon>
                                                <YAF:DisplayDateTime id="DisplayDateTime" runat="server">
                                                </YAF:DisplayDateTime>
                                            </small>
                                        </div>
                                        <asp:PlaceHolder runat="server" ID="Message"></asp:PlaceHolder>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                    </div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="card-footer">
                <div class="input-group col-md-4">
                    <div class="input-group-prepend">
                        <div class="input-group-text">
                            <YAF:LocalizedLabel ID="SinceLabel" runat="server"
                                                LocalizedTag="ITEMS"/>:
                        </div>
                    </div>
                    <asp:DropDownList ID="PageSize" runat="server" 
                                      AutoPostBack="True"
                                      OnSelectedIndexChanged="PageSizeSelectedIndexChanged" 
                                      CssClass="select2-select custom-select">
                        <asp:ListItem Text="5" Value="5" Selected="True" />
                        <asp:ListItem Text="10" Value="10" />
                        <asp:ListItem Text="20" Value="20" />
                        <asp:ListItem Text="30" Value="30" />
                        <asp:ListItem Text="40" Value="40" />
                        <asp:ListItem Text="50" Value="50" />
                    </asp:DropDownList>
                            
                </div>
            </div>
        </div>
        <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
    </div>
</div>
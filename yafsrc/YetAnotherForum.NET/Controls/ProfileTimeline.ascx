<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="true" Inherits="YAF.Controls.ProfileTimeline"
    CodeBehind="ProfileTimeline.ascx.cs" %>

<div class="row">
    <div class="col">
        <div class="card mb-3 mt-2" id="activity">
            <div class="card-header">
                <div class="row justify-content-between align-items-center">
                    <div class="col-auto">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="stream"
                                        LocalizedTag="ACTIVITY"
                                        LocalizedPage="ACCOUNT" />
                    </div>
                    <div class="col-auto">
                        <div class="btn-toolbar" role="toolbar">
                            <div class="input-group input-group-sm me-2" role="group">
                                <div class="input-group-text">
                                    <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                                </div>
                                <asp:DropDownList runat="server" ID="PageSize"
                                                  AutoPostBack="True"
                                                  OnSelectedIndexChanged="PageSizeSelectedIndexChanged"
                                                  CssClass="form-select">
                                </asp:DropDownList>
                            </div>
                        <div class="btn-group" role="group" aria-label="Filters">
                            <YAF:ThemeButton runat="server"
                                             CssClass="dropdown-toggle"
                                             DataToggle="dropdown"
                                             Type="Secondary"
                                             Size="Small"
                                             Icon="filter"
                                             TextLocalizedTag="FILTER_DROPDOWN"
                                             TextLocalizedPage="ADMIN_USERS"/>
                            <div class="dropdown-menu dropdown-menu-end dropdown-menu-lg-start">
                                <div class="px-3 py-1">
                                    <div class="mb-3">
                                        <div class="form-check form-switch">
                                            <asp:CheckBox runat="server" ID="CreatedTopic" 
                                                          Checked="True"/>
                                        </div>
                                    </div>
                                    <div class="mb-3">
                                        <div class="form-check form-switch">
                                            <asp:CheckBox runat="server" ID="CreatedReply" 
                                                          Checked="True"/>
                                        </div>
                                    </div>
                                    <div class="mb-3">
                                        <div class="form-check form-switch">
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

                                    <YAF:ThemeButton ID="Reset" runat="server"
                                                     OnClick="ResetClick"
                                                     TextLocalizedTag="CLEAR"
                                                     Type="Secondary"
                                                     Size="Small"
                                                     Icon="trash" CssClass="float-end">
                                    </YAF:ThemeButton>
                                </div>
                            </div>
                        </div>
                            </div>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <asp:Repeater runat="server" ID="ActivityStream" 
                              OnItemDataBound="ActivityStream_OnItemDataBound">
                    <ItemTemplate>
                        <div class="row">
                            <div class="col-auto text-center flex-column d-none d-sm-flex">
                                <h5 class="m-2">
                                    <asp:Label runat="server" ID="Icon"
                                               CssClass="fa-stack fa-1x pt-3"></asp:Label>
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
                </asp:Repeater>
            </div>
        </div>
        
    </div>
</div>
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server"
                   OnPageChange="PagerTop_PageChange" />
    </div>
</div>
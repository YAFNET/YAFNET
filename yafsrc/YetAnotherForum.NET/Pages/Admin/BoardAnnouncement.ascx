<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.BoardAnnouncement" CodeBehind="BoardAnnouncement.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col">
        <div class="card">
            <div class="card-header">
                <YAF:IconHeader runat="server" 
                                IconName="bullhorn"
                                LocalizedTag="ANNOUNCEMENT_TITLE" />
            </div>
            <div class="card-body">
                <asp:PlaceHolder runat="server" ID="CurrentAnnouncement" Visible="False">
                    <YAF:Alert runat="server" ID="Current" Type="info">
                        <asp:Label runat="server" ID="CurrentMessage"></asp:Label>
                        <YAF:ThemeButton runat="server"
                                         Type="Danger"
                                         Icon="trash"
                                         OnClick="DeleteClick"></YAF:ThemeButton>
                    </YAF:Alert>
                </asp:PlaceHolder>
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="Message">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                            LocalizedTag="ANNOUNCEMENT_MESSAGE" />
                    </asp:Label>
                    <asp:TextBox ID="Message" runat="server" 
                                 TextMode="MultiLine" 
                                 CssClass="form-control" 
                                 Rows="3"
                                 required="required" />
                    <div class="invalid-feedback">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NEED_MESSAGE" />
                    </div>
                </div>
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="BoardAnnouncementUntil">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                            LocalizedTag="ANNOUNCEMENT_UNTIL" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="BoardAnnouncementUntil" 
                                 CssClass="form-control" 
                                 TextMode="Number" />
                    <div class="form-check form-check-inline mt-1">
                        <asp:RadioButtonList
                            runat="server" ID="BoardAnnouncementUntilUnit" 
                            RepeatLayout="UnorderedList"
                            CssClass="list-unstyled" />
                    </div>
                </div>
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="BoardAnnouncementType">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                            LocalizedTag="ANNOUNCEMENT_TYPE" />
                    </asp:Label>
                    <asp:DropDownList runat="server" ID="BoardAnnouncementType" 
                                       CssClass="select2-select">
                        <asp:ListItem Text="primary" Value="primary"></asp:ListItem>
                        <asp:ListItem Text="secondary" Value="secondary"></asp:ListItem>
                        <asp:ListItem Text="success" Value="success"></asp:ListItem>
                        <asp:ListItem Text="danger" Value="danger"></asp:ListItem>
                        <asp:ListItem Text="warning" Value="warning"></asp:ListItem>
                        <asp:ListItem Text="info" Value="info" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="light" Value="light"></asp:ListItem>
                        <asp:ListItem Text="dark" Value="dark"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="SaveAnnouncement" runat="server" 
                                 Type="Primary" 
                                 CausesValidation="True"
                                 OnClick="SaveAnnouncementClick"
                                 Icon="save" 
                                 TextLocalizedTag="SAVE">
                </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>
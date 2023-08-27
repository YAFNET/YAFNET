<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Profile.Attachments" Codebehind="Attachments.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
    <div class="col">
        <div class="row">
            <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <div class="row justify-content-between align-items-center">
                    <div class="col-auto">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        LocalizedTag="TITLE"
                                        LocalizedPage="ATTACHMENTS"
                                        IconName="paperclip"></YAF:IconHeader>
                    </div>
                    <div class="col-auto">
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
                    </div>
                </div>
            </div>
            <div class="card-body">
                <asp:Repeater runat="server" ID="List">
                    <HeaderTemplate>
                        <ul class="list-group list-group-flush">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item d-inline-flex align-items-center">
                            <asp:HiddenField runat="server" ID="FileID" Value='<%# this.Eval( "ID") %>' />
                            <asp:CheckBox ID="Selected" runat="server"
                                              Text="&nbsp;"
                                              CssClass="form-check" />
                            <%# this.GetPreviewImage(Container.DataItem) %>
                            <span class="text-truncate">
                                <%# this.Eval( "FileName") %> <em>(<%# this.Eval("Bytes").ToType<int>() / 1024%> kb)</em>
                            </span>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
                <YAF:ThemeButton ID="DeleteAttachment2" runat="server"
                                 TextLocalizedTag="BUTTON_DELETEATTACHMENT" TitleLocalizedTag="BUTTON_DELETEATTACHMENT_TT"
                                 ReturnConfirmTag="CONFIRM_DELETE"
                                 OnClick="DeleteAttachments_Click"
                                 Icon="trash"
                                 Type="Danger"
                                 Visible="<%# this.List.Items.Count > 0 %>"
                                 CssClass="m-3"/>
                <YAF:Pager ID="PagerTop" runat="server" 
                           OnPageChange="PagerTop_PageChange" />
            </div>
        </div>
    </div>
        </div>
    </div>
</div>
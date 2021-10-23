<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersAttachments" Codebehind="EditUsersAttachments.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<h2>
    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                        LocalizedTag="TITLE" 
                        LocalizedPage="ATTACHMENTS" />
</h2>

<div class="row justify-content-end">
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

<asp:Repeater runat="server" ID="List" OnItemCommand="List_ItemCommand">
    <HeaderTemplate>
        <ul class="list-group list-group-flush">
    </HeaderTemplate>
    <ItemTemplate>
        <li class="list-group-item">
            <asp:CheckBox ID="Selected" runat="server"
                          Text="&nbsp;"
                          CssClass="form-check d-inline-flex align-middle" />
            <%# this.GetPreviewImage(Container.DataItem) %>
            <%# this.Eval( "FileName") %> <em>(<%# this.Eval("Bytes").ToType<int>() / 1024%> kb)</em>
                            
            <YAF:ThemeButton ID="ThemeButtonDelete" runat="server"
                             CommandName="delete" CommandArgument='<%# this.Eval( "ID") %>' 
                             TitleLocalizedTag="DELETE" 
                             TextLocalizedTag="DELETE"
                             Icon="trash"
                             Type="Danger"
                             ReturnConfirmText='<%#this.GetText("ATTACHMENTS", "CONFIRM_DELETE") %>'>
            </YAF:ThemeButton>

        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
<YAF:ThemeButton ID="DeleteAttachment2" runat="server"
                 TextLocalizedTag="BUTTON_DELETEATTACHMENT" TitleLocalizedTag="BUTTON_DELETEATTACHMENT_TT"
                 ReturnConfirmText='<%#this.GetText("ATTACHMENTS", "CONFIRM_DELETE") %>'
                 OnClick="DeleteAttachments_Click"
                 Icon="trash"
                 Type="Danger"
                 Visible="<%# this.List.Items.Count > 0 %>"
                 CssClass="m-3"/>
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server" 
                   OnPageChange="PagerTop_PageChange" />
    </div>
</div>
<hr />
<div class="text-lg-center">
    <YAF:ThemeButton runat="server" ID="Back" 
                     OnClick="Back_Click"
                     TextLocalizedTag="CANCEL"
                     Type="Secondary"
                     Icon="reply"/>
</div>
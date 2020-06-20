<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersAttachments" Codebehind="EditUsersAttachments.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<h2>
    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                        LocalizedTag="TITLE" 
                        LocalizedPage="ATTACHMENTS" />
</h2>

<YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />

<asp:Repeater runat="server" ID="List" OnItemCommand="List_ItemCommand">
    <HeaderTemplate>
        <ul class="list-group list-group-flush mt-3">
    </HeaderTemplate>
    <ItemTemplate>
        <li class="list-group-item">
            <asp:CheckBox ID="Selected" runat="server"
                          Text="&nbsp;"
                          CssClass="custom-control custom-checkbox d-inline-flex" />
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
<YAF:Pager ID="PagerBottom" runat="server" 
           LinkedPager="PagerTop" />
<hr />
<div class="text-lg-center">
    <YAF:ThemeButton runat="server" ID="Back" 
                     OnClick="Back_Click"
                     TextLocalizedTag="CANCEL"
                     Type="Secondary"
                     Icon="reply"/>
</div>
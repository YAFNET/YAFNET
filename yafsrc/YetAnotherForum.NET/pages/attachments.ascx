<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Attachments" Codebehind="Attachments.ascx.cs" %>

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
                <i class="fa fa-paperclip fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                                                                  LocalizedTag="TITLE" />
            </div>
            <div class="card-body">
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
                <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
            </div>
            <div class="card-footer text-center">
               <YAF:ThemeButton runat="server" ID="Back" 
                                OnClick="Back_Click"
                                Type="Secondary"
                                Icon="reply"/>
            </div>
        </div>
    </div>
</div>
</div>
    </div>
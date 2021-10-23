<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModForumUser.ascx.cs" Inherits="YAF.Dialogs.ModForumUser" %>


<div class="modal fade" id="ModForumUserDialog" tabindex="-1" role="dialog" aria-labelledby="Moderate Forum User Dialog" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                        LocalizedTag="TITLE"
                                        LocalizedPage="MOD_FORUMUSER"/>
                </h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                </button>
            </div>
            <div class="modal-body">
             <!-- Modal Content START !-->
                <div class="input-group mb-3">
                    <asp:TextBox runat="server" ID="UserName" 
                                 CssClass="form-control" 
                                 PlaceHolder='<%# this.GetText("USER") %>' />
                    <YAF:ThemeButton runat="server" ID="FindUsers" 
                                     TextLocalizedTag="FIND"
                                     NavigateUrl="#"
                                     Type="Secondary"
                                     Icon="search" />
                </div>
                <div class="mb-3">
                    <asp:DropDownList runat="server" ID="ToList" style="display:none" />
                </div>
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="AccessMaskID">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ACCESSMASK" />
                    </asp:Label>
                    <asp:DropDownList runat="server" ID="AccessMaskID" CssClass="select2-select" />
                </div>
            </div>
                        <!-- Modal Content END !-->           
            <div class="modal-footer">
                <YAF:ThemeButton runat="server" ID="Update"
                                 OnClick="UpdateClick"
                                 TextLocalizedTag="UPDATE"
                                 Type="Primary"
                                 Icon="save"/>
                <YAF:ThemeButton runat="server" ID="Cancel"
                                 DataDismiss="modal"
                                 TextLocalizedTag="CANCEL"
                                 Type="Secondary"
                                 Icon="times">
                </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>

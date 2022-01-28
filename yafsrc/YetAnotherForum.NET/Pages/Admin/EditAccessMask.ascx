<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.EditAccessMask" Codebehind="EditAccessMask.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="universal-access"
                                    LocalizedPage="ADMIN_EDITACCESSMASKS"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                           AssociatedControlID="Name"
                                           LocalizedTag="MASK_NAME" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                            <asp:TextBox runat="server" ID="Name"
                                         required="required"
                                         CssClass="form-control" />
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="MSG_MASK_NAME" />
                            </div>
                        </div>
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel2" runat="server"
                                           AssociatedControlID="SortOrder"
                                           LocalizedTag="MASK_ORDER" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                            <asp:TextBox runat="server" ID="SortOrder"
                                         required="required"
                                         MaxLength="5"
                                         CssClass="form-control"
                                         TextMode="Number" />
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="MSG_NUMBER_SORT" />
                            </div>
                        </div>
                    </div>
                        <div class="row">
                            <div class="mb-3 col-md-2">
                                <YAF:HelpLabel ID="HelpLabel3" runat="server"
                                                   AssociatedControlID="ReadAccess"
                                                   LocalizedTag="READ_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox runat="server" ID="ReadAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="mb-3 col-md-2">
                                <YAF:HelpLabel ID="HelpLabel4" runat="server"
                                                   AssociatedControlID="PostAccess"
                                                   LocalizedTag="POST_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox runat="server" ID="PostAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="mb-3 col-md-2">
                                <YAF:HelpLabel ID="HelpLabel5" runat="server"
                                                   AssociatedControlID="ReplyAccess"
                                                   LocalizedTag="REPLY_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox runat="server" ID="ReplyAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="mb-3 col-md-2">
                                <YAF:HelpLabel ID="HelpLabel6" runat="server"
                                                   AssociatedControlID="PriorityAccess"
                                                   LocalizedTag="PRIORITY_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox runat="server" ID="PriorityAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="mb-3 col-md-2">
                                <YAF:HelpLabel ID="HelpLabel7" runat="server"
                                                   AssociatedControlID="PollAccess"
                                                   LocalizedTag="POLL_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox runat="server" ID="PollAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="mb-3 col-md-2">
                                <YAF:HelpLabel ID="HelpLabel8" runat="server"
                                                   AssociatedControlID="VoteAccess"
                                                   LocalizedTag="VOTE_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox runat="server" ID="VoteAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            </div>
                        <div class="row">
                            <div class="mb-3 col-md-2">
                                <YAF:HelpLabel ID="HelpLabel9" runat="server"
                                                   AssociatedControlID="ModeratorAccess"
                                                   LocalizedTag="MODERATOR_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox runat="server" ID="ModeratorAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="mb-3 col-md-2">
                                <YAF:HelpLabel ID="HelpLabel10" runat="server"
                                               AssociatedControlID="EditAccess"
                                               LocalizedTag="EDIT_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox runat="server" ID="EditAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="mb-3 col-md-2">
                                <YAF:HelpLabel ID="HelpLabel11" runat="server"
                                               AssociatedControlID="DeleteAccess"
                                               LocalizedTag="DELETE_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />

                            <div class="form-check form-switch">
                                <asp:CheckBox runat="server" ID="DeleteAccess" Text="&nbsp;" />
                            </div>
                        </div>
                        </div>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Save" runat="server"
                                     CausesValidation="True"
                                     OnClick="SaveClick"
                                     Type="Primary"
                                     Icon="save"
                                     TextLocalizedTag="SAVE" />
                    <YAF:ThemeButton ID="Cancel" runat="server"
                                     OnClick="CancelClick"
                                     Type="Secondary"
                                     Icon="reply"
                                     TextLocalizedTag="CANCEL" />
                </div>
            </div>
        </div>
    </div>



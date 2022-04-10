<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NntpForumEdit.ascx.cs" Inherits="YAF.Dialogs.NntpForumEdit" %>


<div class="modal fade" id="NntpForumEditDialog" tabindex="-1" role="dialog" aria-labelledby="NntpForumEditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="Title" runat="server"
                                LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="LocalizedLabel2" runat="server"
                                               AssociatedControlID="NntpServerID"
                                               LocalizedTag="SERVER" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                                <asp:DropDownList ID="NntpServerID" runat="server"
                                                  CssClass="select2-select" />
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="LocalizedLabel3" runat="server"
                                               AssociatedControlID="GroupName"
                                               LocalizedTag="GROUP" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                                <asp:TextBox ID="GroupName" runat="server"
                                             required="required"
                                             CssClass="form-control" />
                                <div class="invalid-feedback">
                                    <YAF:LocalizedLabel runat="server"
                                                        LocalizedPage="ADMIN_EDITNNTPFORUM"
                                                        LocalizedTag="MSG_VALID_GROUP" />
                                </div>
                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="LocalizedLabel4" runat="server"
                                           AssociatedControlID="ForumListSelected"
                                           LocalizedTag="FORUM" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                            <select id="ForumList" name="forumList"></select>
                            <asp:HiddenField runat="server" ID="ForumListSelected" Value="0" />
                        </div>
                        <div class="row">
                            <div class="mb-3 col-auto">
                                <YAF:HelpLabel ID="HelpLabel10" runat="server"
                                               AssociatedControlID="DateCutOff"
                                               LocalizedTag="DATECUTOFF" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                                <asp:TextBox ID="DateCutOff" runat="server"
                                             CssClass="form-control"
                                             Enabled="true"
                                             TextMode="DateTime" />
                            </div>
                            <div class="mb-3 col-auto">
                                <YAF:HelpLabel ID="LocalizedLabel5" runat="server"
                                               AssociatedControlID="Active"
                                               LocalizedTag="ACTIVE" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox ID="Active" runat="server" Checked="true" Text="&nbsp;" />
                                </div>
                            </div>
                        </div>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server"
                                         OnClick="Save_OnClick"
                                         TextLocalizedTag="ADMIN_EDITNNTPFORUM" TextLocalizedPage="TITLE"
                                         Type="Primary"
                                         CausesValidation="True"
                                         Icon="save">
                        </YAF:ThemeButton>
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

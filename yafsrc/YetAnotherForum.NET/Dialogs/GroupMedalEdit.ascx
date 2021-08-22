<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupMedalEdit.ascx.cs" Inherits="YAF.Dialogs.GroupMedalEdit" %>

<div class="modal fade" id="GroupEditDialog" tabindex="-1" role="dialog" aria-labelledby="GroupEditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <asp:Label runat="server" ID="GroupMedalEditTitle" />
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel14" runat="server"
                                           AssociatedControlID="AvailableGroupList"
                                           LocalizedTag="MEDAL_GROUP" LocalizedPage="ADMIN_EDITMEDAL"/>
                            <asp:DropDownList runat="server" ID="AvailableGroupList" CssClass="select2-select"/>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel15" runat="server"
                                           AssociatedControlID="GroupMessage"
                                           LocalizedTag="OVERRIDE_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL"/>
                            <asp:TextBox ID="GroupMessage" runat="server" MaxLength="100" CssClass="form-control"/>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel16" runat="server"
                                           AssociatedControlID="GroupSortOrder"
                                           LocalizedTag="OVERRIDE_ORDER" LocalizedPage="ADMIN_EDITMEDAL"/>
                            <asp:TextBox ID="GroupSortOrder" runat="server" CssClass="form-control" TextMode="Number"/>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel18" runat="server"
                                           AssociatedControlID="GroupHide"
                                           LocalizedTag="HIDE" LocalizedPage="ADMIN_EDITMEDAL"/>
                            <div class="form-check form-switch">
                                <asp:CheckBox runat="server" ID="GroupHide" Checked="false" Text="&nbsp;"/>
                            </div>
                        </div>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton runat="server"
                                         OnClick="Save_OnClick"
                                         CssClass="me-2"
                                         ID="AddGroupSave"
                                         Type="Primary"
                                         Icon="save"
                                         TextLocalizedTag="SAVE" />
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

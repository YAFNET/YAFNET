<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditProfileDefinition.ascx.cs" Inherits="YAF.Dialogs.EditProfileDefinition" %>

<div class="modal fade" id="EditDialog" tabindex="-1" role="dialog" aria-labelledby="EditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <YAF:LocalizedLabel ID="Title" runat="server"
                                        LocalizedTag="TITLE"/>
                </h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                </button>
            </div>
            <div class="modal-body">
                <!-- Modal Content START !-->
                <div class="mb-3">
                    <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                   AssociatedControlID="Name"
                                   LocalizedTag="NAME"
                                   LocalizedPage="ADMIN_EDIT_PROFILEDEFINITION"/>
                    <asp:TextBox ID="Name" runat="server"
                                 required="required"
                                 CssClass="form-control" />
                    <div class="invalid-feedback">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedPage="ADMIN_EDIT_PROFILEDEFINITION"
                                            LocalizedTag="NEED_NAME" />
                    </div>
                </div>
                <div class="mb-3">
                    <YAF:HelpLabel runat="server"
                                   AssociatedControlID="DataTypes"
                                   LocalizedTag="DATA_TYPE"
                                   LocalizedPage="ADMIN_EDIT_PROFILEDEFINITION"/>
                    <asp:DropDownList runat="server" ID="DataTypes"
                                      CssClass="select2-select"/>
                </div>
                <div class="mb-3">
                    <YAF:HelpLabel ID="HelpLabel2" runat="server"
                                   AssociatedControlID="Length"
                                   LocalizedTag="LENGTH"
                                   LocalizedPage="ADMIN_EDIT_PROFILEDEFINITION"/>
                    <asp:TextBox ID="Length" runat="server"
                                 Text="1"
                                 TextMode="Number"
                                 required="required"
                                 CssClass="form-control" />
                    <div class="invalid-feedback">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedPage="ADMIN_EDIT_PROFILEDEFINITION"
                                            LocalizedTag="NEED_LENGTH" />
                    </div>
                </div>
                <div class="row">
                    <div class="mb-3 col-md-6">
                        <YAF:HelpLabel ID="HelpLabel5" runat="server"
                                       AssociatedControlID="ShowInUserInfo"
                                       LocalizedTag="SHOW_USERINFO"
                                       LocalizedPage="ADMIN_EDIT_PROFILEDEFINITION"/>
                        <div class="form-check form-switch">
                            <asp:CheckBox ID="ShowInUserInfo" runat="server"
                                          Text="&nbsp;" />
                        </div>
                    </div>
                    <div class="mb-3 col-md-6">
                        <YAF:HelpLabel ID="HelpLabel3" runat="server"
                                       AssociatedControlID="Required"
                                       LocalizedTag="REQUIRED"
                                       LocalizedPage="ADMIN_EDIT_PROFILEDEFINITION"/>
                        <div class="form-check form-switch">
                            <asp:CheckBox ID="Required" runat="server"
                                          Text="&nbsp;" />
                        </div>
                    </div>
                </div>
                <div class="mb-3">
                    <YAF:HelpLabel ID="HelpLabel4" runat="server"
                                   AssociatedControlID="DefaultValue"
                                   LocalizedTag="DEFAULT"
                                   LocalizedPage="ADMIN_EDIT_PROFILEDEFINITION"/>
                    <asp:TextBox ID="DefaultValue" runat="server"
                                 CssClass="form-control">
                    </asp:TextBox>
                </div>
                <!-- Modal Content END !-->
            </div>
            <div class="modal-footer">
                <YAF:ThemeButton id="Save" runat="server"
                                 OnClick="SaveClick"
                                 TextLocalizedPage="ADMIN_EDIT_PROFILEDEFINITION"
                                 TextLocalizedTag="SAVE"
                                 CausesValidation="True"
                                 Type="Primary"
                                 Icon="save">
                </YAF:ThemeButton>
                <YAF:ThemeButton runat="server" ID="Cancel"
                                 DataDismiss="modal"
                                 TextLocalizedTag="CANCEL"
                                 CausesValidation="False"
                                 Type="Secondary"
                                 Icon="times">
                </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>
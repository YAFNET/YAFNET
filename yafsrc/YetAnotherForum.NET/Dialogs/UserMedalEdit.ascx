<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserMedalEdit.ascx.cs" Inherits="YAF.Dialogs.UserMedalEdit" %>


<div class="modal fade" id="UserEditDialog" tabindex="-1" role="dialog" aria-labelledby="UserEditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <asp:Label runat="server" ID="UserMedalEditTitle" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel23" runat="server" 
                                           AssociatedControlID="UserName"
                                           LocalizedTag="MEDAL_USER" LocalizedPage="ADMIN_EDITMEDAL"/>
                            <asp:TextBox ID="UserName" runat="server"
                                         CssClass="form-control"/>
                            <asp:DropDownList runat="server" ID="UserNameList"
                                              Visible="false"
                                              CssClass="custom-select"/>
                            <YAF:ThemeButton runat="server" ID="FindUsers"
                                             OnClick="FindUsersClick"
                                             Type="Info"
                                             Size="Small"
                                             TextLocalizedTag="FIND"/>
                            <YAF:ThemeButton runat="server" ID="Clear"
                                             OnClick="ClearClick"
                                             Visible="false"
                                             Type="Info"
                                             Size="Small"
                                             TextLocalizedTag="CLEAR"/>
                            <asp:TextBox Visible="false" ID="UserID" runat="server"
                                         CssClass="form-control"/>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel19" runat="server" 
                                           AssociatedControlID="UserMessage"
                                           LocalizedTag="OVERRIDE_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL"/>
                            <asp:TextBox ID="UserMessage" runat="server" MaxLength="100" CssClass="form-control"/>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel20" runat="server" 
                                           AssociatedControlID="UserSortOrder"
                                           LocalizedTag="OVERRIDE_ORDER" LocalizedPage="ADMIN_EDITMEDAL"/>
                            <asp:TextBox ID="UserSortOrder" runat="server" CssClass="form-control" TextMode="Number"/>
                        </div>
                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="HelpLabel21" runat="server" 
                                               AssociatedControlID="UserOnlyRibbon"
                                               LocalizedTag="ONLY_RIBBON" LocalizedPage="ADMIN_EDITMEDAL"/>
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox runat="server" ID="UserOnlyRibbon" Checked="false" Text="&nbsp;"/>
                                </div>
                            </div>
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="HelpLabel22" runat="server" 
                                               AssociatedControlID="UserHide"
                                               LocalizedTag="HIDE" LocalizedPage="ADMIN_EDITMEDAL"/>
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox runat="server" ID="UserHide" Checked="false" Text="&nbsp;"/>
                                </div>
                            </div>
                            </div>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton runat="server" 
                                         OnClick="Save_OnClick" 
                                         ID="AddUserSave" 
                                         Type="Primary"
                                         Icon="save" 
                                         TextLocalizedTag="SAVE" />&nbsp;
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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BannedUserAgentEdit.ascx.cs" Inherits="YAF.Dialogs.BannedUserAgentEdit" %>


<div class="modal fade" id="EditDialog" tabindex="-1" role="dialog" aria-labelledby="EditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="Title" runat="server" 
                                                LocalizedTag="TITLE" 
                                                LocalizedPage="ADMIN_BANNED_USERAGENTS_EDIT" />
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                           AssociatedControlID="mask"
                                           LocalizedTag="MASK" LocalizedPage="ADMIN_BANNED_USERAGENTS_EDIT" />
                            <asp:TextBox ID="mask" runat="server"
                                         CssClass="form-control" 
                                         Height="100"
                                         TextMode="MultiLine" 
                                         required="required"/>
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedPage="ADMIN_BANNED_USERAGENTS_EDIT"
                                                    LocalizedTag="NEED_MASK" />
                            </div>
                        </div>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server" 
                                         OnClick="Save_OnClick"
                                         TextLocalizedTag="ADMIN_BANNED_USERAGENTS_EDIT" TextLocalizedPage="SAVE"
                                         CausesValidation="True"
                                         Type="Primary" 
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

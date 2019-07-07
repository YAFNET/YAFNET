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
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel23" runat="server" LocalizedTag="MEDAL_USER" LocalizedPage="ADMIN_EDITMEDAL" />
                        </h4>
                        <p>
                            <asp:TextBox ID="UserName" runat="server" 
                                         CssClass="form-control" />
                            <asp:DropDownList  runat="server" ID="UserNameList" 
                                               Visible="false" 
                                               CssClass="custom-select" />
                            </p>
                        <p>
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
                                         CssClass="form-control" />
                        </p><hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel19" runat="server" LocalizedTag="OVERRIDE_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
                        </h4>
                        <p>
                            <asp:TextBox ID="UserMessage" runat="server" MaxLength="100" CssClass="form-control"  />
                        </p><hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel20" runat="server" LocalizedTag="OVERRIDE_ORDER" LocalizedPage="ADMIN_EDITMEDAL" />
                        </h4>
                        <p>
                            <asp:TextBox ID="UserSortOrder" runat="server" CssClass="form-control" TextMode="Number"  />
                        </p><hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel21" runat="server" LocalizedTag="ONLY_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
                        </h4>
                        <div class="custom-control custom-switch">
                            <asp:CheckBox runat="server" ID="UserOnlyRibbon" Checked="false" Text="&nbsp;"  />
                        </div><hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel22" runat="server" LocalizedTag="HIDE" LocalizedPage="ADMIN_EDITMEDAL" />
                        </h4>
                        <div class="custom-control custom-switch">
                            <asp:CheckBox runat="server" ID="UserHide" Checked="false" Text="&nbsp;" />
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
                        
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            <i class="fa fa-times fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedTag="CANCEL"></YAF:LocalizedLabel>
                        </button>
                    </div>
                </div>
    </div>
</div>

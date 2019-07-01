<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupMedalEdit.ascx.cs" Inherits="YAF.Dialogs.GroupMedalEdit" %>

<div class="modal fade" id="GroupEditDialog" tabindex="-1" role="dialog" aria-labelledby="GroupEditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <asp:Label runat="server" ID="GroupMedalEditTitle" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="MEDAL_GROUP" LocalizedPage="ADMIN_EDITMEDAL" />
                        </h4>
                        <p>
                            <asp:DropDownList runat="server" ID="AvailableGroupList" CssClass="custom-select" />
                        </p><hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel15" runat="server" LocalizedTag="OVERRIDE_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
                        </h4>
                        <p>
                            <asp:TextBox ID="GroupMessage" runat="server" MaxLength="100" CssClass="form-control" />
                        </p><hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel16" runat="server" LocalizedTag="OVERRIDE_ORDER" LocalizedPage="ADMIN_EDITMEDAL" />
                        </h4>
                        <p>
                            <asp:TextBox ID="GroupSortOrder" runat="server" CssClass="form-control" TextMode="Number" />
                        </p><hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel17" runat="server" LocalizedTag="ONLY_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
                        </h4>
                        <div class="custom-control custom-switch">
                            <asp:CheckBox runat="server" ID="GroupOnlyRibbon" Checked="false" Text="&nbsp;"  />
                        </div><hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel18" runat="server" LocalizedTag="HIDE" LocalizedPage="ADMIN_EDITMEDAL" />
                        </h4>
                        <div class="custom-control custom-switch">
                            <asp:CheckBox runat="server" ID="GroupHide" Checked="false" Text="&nbsp;"  />
                        </div>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton runat="server" 
                                         OnClick="Save_OnClick" 
                                         ID="AddGroupSave" 
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

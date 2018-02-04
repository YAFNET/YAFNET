<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BannedEmailEdit.ascx.cs" Inherits="YAF.Dialogs.BannedEmailEdit" %>

<div class="modal fade" id="EditDialog" tabindex="-1" role="dialog" aria-labelledby="EditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="Title" runat="server" LocalizedTag="TITLE" 
                                LocalizedPage="ADMIN_BANNEDEMAIL_EDIT" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="MASK" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="mask" runat="server" TextMode="Email"></asp:TextBox>
                        </p>
                        <hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="REASON" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="BanReason" runat="server" MaxLength="128"></asp:TextBox>
                        </p>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server" OnClick="Save_OnClick" 
                            TextLocalizedTag="ADMIN_BANNEDEMAIL_EDIT" TextLocalizedPage="SAVE"
                            Type="Primary" Icon="upload">
                        </YAF:ThemeButton>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            <i class="fa fa-times fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedTag="CANCEL"></YAF:LocalizedLabel>
                        </button>
                    </div>
                </div>
    </div>
</div>

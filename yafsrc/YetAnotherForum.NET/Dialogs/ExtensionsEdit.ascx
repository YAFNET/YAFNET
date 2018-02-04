<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExtensionsEdit.ascx.cs" Inherits="YAF.Dialogs.ExtensionsEdit" %>

<div class="modal fade" id="ExtensionsEditDialog" tabindex="-1" role="dialog" aria-labelledby="ExtensionsEditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="Title" runat="server" 
                                LocalizedTag="TITLE" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="FILE_EXTENSION" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                        </h4>
                        <p>
                            <asp:TextBox ID="extension" runat="server" CssClass="form-control"></asp:TextBox>
                        </p>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server" OnClick="Save_OnClick" 
                            TextLocalizedTag="ADMIN_EXTENSIONS_EDIT" TextLocalizedPage="ADD"
                            Type="Primary" Icon="upload">
                        </YAF:ThemeButton>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            <i class="fa fa-times fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedTag="CANCEL"></YAF:LocalizedLabel>
                        </button>
                    </div>
                </div>
    </div>
</div>

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
                        <div class="form-group">
                            <YAF:HelpLabel ID="LocalizedLabel2" runat="server" 
                                           AssociatedControlID="extension"
                                           LocalizedTag="FILE_EXTENSION" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                            <asp:TextBox ID="extension" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server" 
                                         OnClick="Save_OnClick"
                                         TextLocalizedTag="ADMIN_EXTENSIONS_EDIT" TextLocalizedPage="ADD"
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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReplaceWordsImport.ascx.cs" Inherits="YAF.Dialogs.ReplaceWordsImport" %>


<div class="modal fade" id="ReplaceWordsImportDialog" tabindex="-1" role="dialog" aria-labelledby="ReplaceWordsImportDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" 
                                LocalizedPage="ADMIN_REPLACEWORDS_IMPORT" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <YAF:HelpLabel ID="LocalizedLabel2" runat="server" 
                                       AssociatedControlID="importFile"
                                       LocalizedTag="SELECT_IMPORT" LocalizedPage="ADMIN_REPLACEWORDS_IMPORT" />
                        <div class="custom-file">
                            <input type="file" id="importFile" class="custom-file-input" runat="server" />
                            <asp:Label runat="server" CssClass="custom-file-label" AssociatedControlID="importFile">
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="SELECT_IMPORT" 
                                                    LocalizedPage="ADMIN_EXTENSIONS_IMPORT"></YAF:LocalizedLabel>
                            </asp:Label>
                        </div>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Import" runat="server"
                                         OnClick="Import_OnClick"
                                         TextLocalizedTag="ADMIN_REPLACEWORDS_IMPORT" TextLocalizedPage="TITLE"
                                         Type="Primary"
                                         Icon="upload">
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

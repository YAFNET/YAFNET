<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpamWordsImport.ascx.cs" Inherits="YAF.Dialogs.SpamWordsImport" %>


<div class="modal fade" id="SpamWordsImportDialog" tabindex="-1" role="dialog" aria-labelledby="SpamWordsImportDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" 
                                LocalizedPage="ADMIN_SPAMWORDS_IMPORT" />
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SELECT_IMPORT" LocalizedPage="ADMIN_SPAMWORDS_IMPORT" />
                        </h4>
                        <div class="mb-3">
                            <label for="<%# this.importFile.ClientID %>" class="form-label">
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="SELECT_IMPORT" 
                                                    LocalizedPage="ADMIN_EXTENSIONS_IMPORT"/>
                            </label>
                            <input type="file" id="importFile" class="form-control" runat="server" />
                        </div>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Import" runat="server" OnClick="Import_OnClick" 
                            TextLocalizedTag="ADMIN_SPAMWORDS_IMPORT" TextLocalizedPage="TITLE"
                            Type="Primary" Icon="upload">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton runat="server"
                                         Type="Secondary"
                                         DataDismiss="modal"
                                         Icon="times"
                                         TextLocalizedTag="CANCEL"></YAF:ThemeButton>
                    </div>
                </div>
    </div>
</div>

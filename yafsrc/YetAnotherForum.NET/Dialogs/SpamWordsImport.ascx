<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpamWordsImport.ascx.cs" Inherits="YAF.Dialogs.SpamWordsImport" %>


<div class="modal fade" id="SpamWordsImportDialog" tabindex="-1" role="dialog" aria-labelledby="SpamWordsImportDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" 
                                LocalizedPage="ADMIN_SPAMWORDS_IMPORT" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SELECT_IMPORT" LocalizedPage="ADMIN_SPAMWORDS_IMPORT" />
                        </h4>
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

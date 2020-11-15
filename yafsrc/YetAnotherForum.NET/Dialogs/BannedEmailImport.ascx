<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BannedEmailImport.ascx.cs" Inherits="YAF.Dialogs.BannedEmailImport" %>


<div class="modal fade" id="ImportDialog" tabindex="-1" role="dialog" aria-labelledby="ImportDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" 
                                LocalizedPage="ADMIN_BANNEDEMAIL_IMPORT" />
                        </h5>
                        <button type="button" class="btn-close" data-dismiss="modal" aria-label="Close">
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <YAF:HelpLabel ID="HelpLabel1" runat="server" 
                                       AssociatedControlID="importFile"
                                       LocalizedTag="IMPORT_FILE" LocalizedPage="ADMIN_BANNEDEMAIL_IMPORT" />
                        <YAF:Alert runat="server" Type="info">
                            <YAF:Icon runat="server" IconName="info-circle" />
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                LocalizedTag="NOTE" LocalizedPage="ADMIN_BANNEDEMAIL">
                            </YAF:LocalizedLabel>
                        </YAF:Alert>
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
                        <YAF:ThemeButton id="Import" runat="server" 
                                         OnClick="Import_OnClick"
                                         TextLocalizedTag="ADMIN_BANNEDEMAIL_IMPORT" TextLocalizedPage="IMPORT"
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

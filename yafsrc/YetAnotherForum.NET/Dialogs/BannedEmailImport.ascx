<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BannedEmailImport.ascx.cs" Inherits="YAF.Dialogs.BannedEmailImport" %>

<div class="modal fade" id="ImportDialog" tabindex="-1" role="dialog" aria-labelledby="ImportDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" 
                                LocalizedPage="ADMIN_BANNEDEMAIL_IMPORT" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="IMPORT_FILE" LocalizedPage="ADMIN_BANNEDEMAIL_IMPORT" />
                        </h4>
                        <div class="alert alert-warning" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                LocalizedTag="NOTE" LocalizedPage="ADMIN_BANNEDEMAIL">
                            </YAF:LocalizedLabel>
                        </div>
                        <p>
                            <input type="file" id="importFile" class="form-control-file" runat="server" />
                        </p>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Import" runat="server" OnClick="Import_OnClick" 
                            TextLocalizedTag="ADMIN_BANNEDEMAIL_IMPORT" TextLocalizedPage="IMPORT"
                            Type="Primary" Icon="upload">
                        </YAF:ThemeButton>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            <i class="fa fa-times fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedTag="CANCEL"></YAF:LocalizedLabel>
                        </button>
                    </div>
                </div>
    </div>
</div>

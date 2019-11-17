<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpamWordsEdit.ascx.cs" Inherits="YAF.Dialogs.SpamWordsEdit" %>


<div class="modal fade" id="SpamWordsEditDialog" tabindex="-1" role="dialog" aria-labelledby="SpamWordsImportDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="Title" runat="server" 
                                LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMWORDS_EDIT" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel2" runat="server" 
                                           LocalizedTag="SPAM" 
                                           LocalizedPage="ADMIN_SPAMWORDS_EDIT" />
                        </h4>
                        <p>
                            <asp:TextBox ID="spamword" runat="server" 
                                         CssClass="form-control"></asp:TextBox>
                        </p>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server" 
                                         OnClick="Save_OnClick"
                                         TextLocalizedTag="ADMIN_SPAMWORDS_EDIT" 
                                         TextLocalizedPage="TITLE"
                                         Type="Primary" 
                                         Icon="save">
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

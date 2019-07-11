<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReplaceWordsEdit.ascx.cs" Inherits="YAF.Dialogs.ReplaceWordsEdit" %>


<div class="modal fade" id="ReplaceWordsEditDialog" tabindex="-1" role="dialog" aria-labelledby="ReplaceWordsEditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="Title" runat="server" 
                                LocalizedTag="TITLE" LocalizedPage="ADMIN_REPLACEWORDS_EDIT" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <div class="form-group">
                            <YAF:HelpLabel ID="LocalizedLabel2" runat="server" 
                                           AssociatedControlID="badword"
                                           LocalizedTag="BAD" LocalizedPage="ADMIN_REPLACEWORDS_EDIT" />
                            <asp:TextBox ID="badword" runat="server" 
                                         CssClass="form-control"
                                         Height="100"
                                         TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="LocalizedLabel3" runat="server"
                                           AssociatedControlID="goodword"
                                           LocalizedTag="GOOD" LocalizedPage="ADMIN_REPLACEWORDS_EDIT" />
                            <asp:TextBox ID="goodword" runat="server" 
                                         CssClass="form-control"
                                         Height="100"
                                         TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server" 
                                         OnClick="Save_OnClick"
                                         TextLocalizedTag="ADMIN_REPLACEWORDS_EDIT" TextLocalizedPage="TITLE"
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

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
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="BAD" LocalizedPage="ADMIN_REPLACEWORDS_EDIT" />
                        </h4>
                        <p>
                            <asp:TextBox ID="badword" runat="server" CssClass="form-control"></asp:TextBox>
                        </p>
                        <hr />
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="GOOD" LocalizedPage="ADMIN_REPLACEWORDS_EDIT" />
                        </h4>
                        <p>
                            <asp:TextBox ID="goodword" runat="server" CssClass="form-control"></asp:TextBox>
                        </p>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server" OnClick="Save_OnClick" 
                            TextLocalizedTag="ADMIN_REPLACEWORDS_EDIT" TextLocalizedPage="TITLE"
                            Type="Primary" Icon="upload">
                        </YAF:ThemeButton>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            <i class="fa fa-times fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedTag="CANCEL"></YAF:LocalizedLabel>
                        </button>
                    </div>
                </div>
    </div>
</div>

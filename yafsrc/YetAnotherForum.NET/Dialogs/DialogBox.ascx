<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Dialogs.DialogBox" CodeBehind="DialogBox.ascx.cs" %>

<asp:Panel ID="YafForumPageErrorPopup" runat="server" CssClass="modal fade" role="dialog">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"><asp:label id="Header" runat="server"></asp:label></h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div id="YafPopupErrorMessageOuter" class="modal-body">
                <p id="YafPopupErrorMessageInner">
                    <asp:Literal ID="MessageText" runat="server"></asp:Literal>
                </p>
            </div>
            <div class="modal-footer">
                <asp:Button ID="OkButton" runat="server" 
                            CausesValidation="false" 
                            OnClick="OkButton_Click" 
                            Text="Ok" 
                            data-dismiss="modal" />
                <asp:Button ID="CancelButton"
                            runat="server" 
                            CausesValidation="false" 
                            OnClick="CancelButton_Click" 
                            Visible="false" 
                            Text="Cancel" 
                            data-dismiss="modal" />
            </div>
        </div>
    </div>
</asp:Panel>
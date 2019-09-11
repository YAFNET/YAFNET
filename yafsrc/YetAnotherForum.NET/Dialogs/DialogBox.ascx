<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Dialogs.DialogBox" CodeBehind="DialogBox.ascx.cs" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.HtmlControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<%@ Import Namespace="YAF" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Web.Controls" %>

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
                <YAF:ThemeButton ID="OkButton" runat="server" 
                                 OnClick="OkButton_Click"
                                 Icon="check"/>
                <YAF:ThemeButton ID="CancelButton" runat="server" 
                                 OnClick="CancelButton_Click" 
                                 Icon="times"
                                 Visible="false"
                                 DataDismiss="modal"/>
            </div>
        </div>
    </div>
</asp:Panel>
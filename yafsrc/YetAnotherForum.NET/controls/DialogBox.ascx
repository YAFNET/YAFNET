<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DialogBox" CodeBehind="DialogBox.ascx.cs" %>

<asp:Panel ID="YafForumPageErrorPopup" runat="server" CssClass="MessageBox" style="display:none">
    <div class="modalHeader">
        <h3><asp:label id="Header" runat="server"></asp:label></h3>
    </div>
    <div id="YafPopupErrorMessageOuter" class="modalOuter">
        <div style="float:left;">
            <asp:Image id="ImageIcon" AlternateText="Icon" ToolTip="Icon" Visible="false" runat="server" />
        </div>
        <div id="YafPopupErrorMessageInner" class="modalInner">
            <asp:Literal ID="MessageText" runat="server"></asp:Literal>
        </div>
    </div>
    <div class="clear"></div>
    <hr />
    <div class="modalFooter">
        <asp:Button ID="OkButton" CssClass="StandardButtton" runat="server" CausesValidation="false" OnClick="OkButton_Click" Text="Ok" />
        <asp:Button ID="CancelButton" CssClass="StandardButtton" runat="server" CausesValidation="false" OnClick="CancelButton_Click" Visible="false" Text="Cancel" />
    </div>
</asp:Panel>
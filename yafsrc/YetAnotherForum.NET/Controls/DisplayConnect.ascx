<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayConnect"
    EnableViewState="false" Codebehind="DisplayConnect.ascx.cs" %>

<div class="row">
    <div class="col">
        <YAF:Alert runat="server" Type="warning" Dismissing="True">
            <asp:PlaceHolder runat="server" ID="ConnectHolder"></asp:PlaceHolder>
        </YAF:Alert>
    </div>
</div>

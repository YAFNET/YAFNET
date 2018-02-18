<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayConnect"
    EnableViewState="false" Codebehind="DisplayConnect.ascx.cs" %>

<div class="row">
    <div class="col">
        <div class="alert alert-warning alert-dismissible fade show" role="alert">
            <asp:PlaceHolder runat="server" ID="ConnectHolder"></asp:PlaceHolder>
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span>
            </button>
        </div>
    </div>
</div>

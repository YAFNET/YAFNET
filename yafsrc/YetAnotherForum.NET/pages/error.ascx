<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Error" Codebehind="Error.ascx.cs" %>

<YAF:Alert runat="server" ID="Alert" Type="danger">
    <h4 class="alert-heading">Error</h4>
    <asp:Label runat="server" ID="ErrorMessageHolder">
    </asp:Label>
</YAF:Alert>
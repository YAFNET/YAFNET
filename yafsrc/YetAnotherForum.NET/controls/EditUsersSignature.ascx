<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersSignature" Codebehind="EditUsersSignature.ascx.cs" %>

    <h2 runat="server" id="trHeader">
            <YAF:LocalizedLabel runat="server" LocalizedPage="CP_SIGNATURE" LocalizedTag="title" />
        </h2>
    <hr />
        <h4>
           <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="CP_SIGNATURE"
                LocalizedTag="SIGNATURE_PREVIEW" />
        </h4>
    <hr />
      <asp:PlaceHolder id="PreviewLine" runat="server">
      </asp:PlaceHolder>
    <hr />
       <h4>
           <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="CP_SIGNATURE"
                LocalizedTag="TITLE" />
        </h4>
    <hr />
      <asp:PlaceHolder id="EditorLine" runat="server">
            <!-- editor goes here -->
        </asp:PlaceHolder>
    <hr />
   <div class="alert alert-info" role="alert">
        <h2>
           <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="CP_SIGNATURE"
                LocalizedTag="SIGNATURE_PERMISSIONS" />
        </h2>
    <hr />
        <p>
            <asp:Label ID="TagsAllowedWarning" runat="server" />
        </p>
       </div>
                <div class="text-lg-center">

            <asp:LinkButton ID="preview" CssClass="btn btn-success" runat="server" />&nbsp;
            <asp:LinkButton ID="save" Type="Primary" runat="server" />&nbsp;
            <asp:LinkButton ID="cancel" Type="Secondary" runat="server" />
            </div>
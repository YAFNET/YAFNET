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
<YAF:Alert runat="server" Type="info">
    <h2>
        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="CP_SIGNATURE"
                            LocalizedTag="SIGNATURE_PERMISSIONS" />
    </h2>
    <hr />
    <div class="text-break">
        <asp:Label ID="TagsAllowedWarning" runat="server" />
    </div>
</YAF:Alert>
                <div class="text-lg-center">

            <YAF:ThemeButton ID="preview" runat="server"
                             Type="Secondary"
                             Icon="image"
                             TextLocalizedTag="PREVIEW"/>
            <YAF:ThemeButton ID="save" Type="Primary" runat="server"
                             TextLocalizedTag="SAVE"
                             Icon="save"/>&nbsp;
            <YAF:ThemeButton ID="cancel" Type="Secondary" runat="server"
                             Icon="reply"
                             TextLocalizedTag="CANCEL"/>
            </div>
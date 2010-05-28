<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PostOptions" Codebehind="PostOptions.ascx.cs" %>
<tr id="OptionsRow" runat="server">
    <td class="postformheader" valign="top">
        <YAF:LocalizedLabel ID="NewPostOptionsLabel" runat="server" LocalizedTag="NEWPOSTOPTIONS" />
    </td>
    <td class="post">
        <asp:PlaceHolder ID="PersistencyHolder" runat="server" Visible="false">
        <asp:CheckBox ID="Persistency" runat="server" />
        <YAF:LocalizedLabel ID="PersistencyLabel" runat="server" LocalizedTag="PERSISTENCY" />?
        (<YAF:LocalizedLabel ID="PersistencyLabel2" runat="server" LocalizedTag="PERSISTENCY_INFO" />)
        <br id="PersistantAttachBr" runat="server" />
        </asp:PlaceHolder>
        
        <asp:CheckBox ID="TopicWatch" runat="server" />
        <YAF:LocalizedLabel ID="TopicWatchLabel" runat="server" LocalizedTag="TOPICWATCH" />
        <br id="TopicAttachBr" runat="server" />
        <asp:CheckBox ID="TopicAttach" runat="server" Visible="false" />
        <YAF:LocalizedLabel ID="TopicAttachLabel" runat="server" LocalizedTag="TOPICATTACH"
            Visible="false" />
    </td>
</tr>

<%@ Control language="c#" Codebehind="cp_editprofile.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.cp_editprofile" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="../controls/EditUsersProfile.ascx" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:ProfileEdit runat="server" id="ProfileEditor" />

<YAF:SmartScroller id="SmartScroller1" runat = "server" />

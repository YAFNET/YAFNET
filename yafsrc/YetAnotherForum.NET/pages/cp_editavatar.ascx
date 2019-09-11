<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Pages.cp_editavatar" Codebehind="cp_editavatar.ascx.cs" %>
<%@ Import Namespace="YAF" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Web.Controls" %>
<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="../controls/EditUsersAvatar.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
<div class="col-sm-auto">
    <YAF:ProfileMenu ID="ProfileMenu1" runat="server" />
</div>
<div class="col">
<YAF:ProfileEdit runat="server" ID="ProfileEditor" />
    </div>
</div>
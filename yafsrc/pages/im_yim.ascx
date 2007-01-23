<%@ Control language="c#" Codebehind="im_yim.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.im_yim" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<center>
<asp:hyperlink runat="server" id="Msg"><img runat="server" id="Img" border="0"/></asp:hyperlink>
</center>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />

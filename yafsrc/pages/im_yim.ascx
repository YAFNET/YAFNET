<%@ Control language="c#" Inherits="yaf.pages.im_yim" CodeFile="im_yim.ascx.cs" CodeFileBaseClass="yaf.pages.ForumPage" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<center>
<asp:hyperlink runat="server" id="Msg"><img runat="server" id="Img" border="0"/></asp:hyperlink>
</center>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />

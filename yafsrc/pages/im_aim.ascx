<%@ Control language="c#" Codebehind="im_aim.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.im_aim" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<!-- Begin AIM Remote -->
	 
<table cellspacing=0 cellpadding=0 border=0 align=center>
<tr>
    <td width=121 height=67><img src=http://cdn.aim.com/remote/i/hgenhd.gif width=121 height=67 border=0></td><td width=110><asp:hyperlink runat="server" id="Msg"><img src=http://cdn.aim.com/remote/i/hgen1.gif width=110 height=67 border=0 alt="I am Online"></asp:hyperlink></td><td width=109><asp:hyperlink runat="server" id="Buddy"><img src=http://cdn.aim.com/remote/i/hgen2.gif width=109 height=67 border=0 alt="Add me to your Buddy List"></asp:hyperlink></td>  
	<td width=101 height=67><a href=http://www.aim.com/remote/index.adp><img src=http://cdn.aim.com/remote/i/hgen_foot1.gif width=107 height=34 border=0></a><br /><a href=http://aim.aol.com/aimnew/NS/congratsd2.adp?promo=106695><img src=http://cdn.aim.com/remote/i/hgen_foot2.gif width=107 height=33 border=0></a></td>
</tr>
</table>


<yaf:SmartScroller id="SmartScroller1" runat = "server" />

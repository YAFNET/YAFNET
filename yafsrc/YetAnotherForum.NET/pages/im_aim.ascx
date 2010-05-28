<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.im_aim" Codebehind="im_aim.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
    <table cellspacing="0" cellpadding="0" border="0" align="center">
        <tr>
            <td width="121" height="67">
                <img src="http://cdn.aim.com/remote/i/hgenhd.gif" width="121" height="67" border="0" /></td>
            <td width="110">
                <asp:HyperLink runat="server" ID="Msg"><img src="http://cdn.aim.com/remote/i/hgen1.gif" width="110" height="67" border="0" alt="I am Online" /></asp:HyperLink></td>
            <td width="109">
                <asp:HyperLink runat="server" ID="Buddy"><img src="http://cdn.aim.com/remote/i/hgen2.gif" width="109" height="67" border="0" alt="Add me to your Buddy List" /></asp:HyperLink></td>
            <td width="101" height="67">
                <a href="http://www.aim.com/remote/index.adp">
                    <img src="http://cdn.aim.com/remote/i/hgen_foot1.gif" width="107" height="34" alt="" border="0"></a><br />
                <a href="http://aim.aol.com/aimnew/NS/congratsd2.adp?promo=106695">
                    <img src="http://cdn.aim.com/remote/i/hgen_foot2.gif" width="107" height="33" alt="" border="0"></a></td>
        </tr>
    </table>
</div>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>

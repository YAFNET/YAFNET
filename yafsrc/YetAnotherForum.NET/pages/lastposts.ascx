<%@ Control Language="c#" AutoEventWireup="True" CodeFile="lastposts.ascx.cs" Inherits="YAF.Pages.lastposts"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Repeater ID="repLastPosts" runat="server" Visible="true">
    <HeaderTemplate>
        <table class="content" cellspacing="1" cellpadding="0" width="100%" align="center">
            <tr>
                <td class="header2" align="center" colspan="2">
                    <YAF:LocalizedLabel ID="Last10" LocalizedTag="LAST10" runat="server" />
                </td>
            </tr>
    </HeaderTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr class="postheader">
            <td width="140">
                <b>
                    <YAF:UserLink ID="ProfileLink" runat="server" UserID='<%# Convert.ToInt32(Eval( "UserID" )) %>'
                        UserName='<%# Eval( "UserName" ).ToString() %>' BlankTarget="true" />
                </b>
            </td>
            <td width="80%" class="small" align="left">
                <b>
                    <YAF:LocalizedLabel ID="Posted" LocalizedTag="POSTED" runat="server" />
                </b>
                <%# YafDateTime.FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %>
            </td>
        </tr>
        <tr class="post">
            <td>
                &nbsp;</td>
            <td valign="top" class="message">
                <%# FormatBody(Container.DataItem) %>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="postheader">
            <td width="140">
                <b>
                    <YAF:UserLink ID="ProfileLinkAlt" runat="server" UserID='<%# Convert.ToInt32(Eval( "UserID" )) %>'
                        UserName='<%# Eval( "UserName" ).ToString() %>' BlankTarget="true" />
                </b>
            </td>
            <td width="80%" class="small" align="left">
                <b>
                    <YAF:LocalizedLabel ID="PostedAlt" LocalizedTag="POSTED" runat="server" />
                </b>
                <%# YafDateTime.FormatDateTime( ( System.DateTime ) ( ( System.Data.DataRowView ) Container.DataItem ) ["Posted"] )%>
            </td>
        </tr>
        <tr class="post_alt">
            <td>
                &nbsp;</td>
            <td valign="top" class="message">
                <%# FormatBody(Container.DataItem) %>
            </td>
        </tr>
    </AlternatingItemTemplate>
</asp:Repeater>
<div id="DivSmartScroller">
    
</div>
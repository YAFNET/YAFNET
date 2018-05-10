<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Controls.EditUsersSuspend" Codebehind="EditUsersSuspend.ascx.cs" %>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr runat="server" id="trHeader">
		<td class="header1" colspan="2">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_USER" />
		</td>
	</tr>
    <asp:PlaceHolder runat="server" ID="SuspendedHolder">
    <tr>
        <td class="header2" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_CURRENT" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_REASON" />
        </td>
        <td class="post">
            <asp:Label runat="server" ID="CurrentSuspendedReason"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_BY" />
        </td>
        <td class="post">
            <YAF:UserLink runat="server" ID="SuspendedBy"></YAF:UserLink>
        </td>
    </tr>
	<tr>
		<td class="postheader">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="PROFILE" LocalizedTag="ENDS" />
		</td>
		<td class="post">
			<%= GetSuspendedTo() %>
			&nbsp;<asp:Button runat="server" ID="RemoveSuspension" CssClass="pbutton" OnClick="RemoveSuspension_Click" />
		</td>
	</tr>
    </asp:PlaceHolder>
    <tr>
        <td class="header2" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_NEW" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_REASON" />
        </td>
        <td class="post">
            <asp:TextBox Style="width:99%;height:80px;" ID="SuspendedReason" runat="server" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
	<tr>
		<td class="postheader">
			<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_USER" />
		</td>
		<td class="post">
			<asp:TextBox runat="server" ID="SuspendCount" Style="width: 60px" CssClass="Numeric" />&nbsp;<asp:DropDownList
				runat="server" ID="SuspendUnit" CssClass="standardSelectMenu" />
		</td>
	</tr>
    <tr>
       <td class="postheader">
       </td> 
        <td class="post">
            <div class="ui-widget">
                <div class="ui-state-highlight ui-corner-all">
                    <p><span class="ui-icon ui-icon-info" style="float: left; margin-right: .3em;"></span>
                        <asp:Label runat="server" ID="SuspendInfo"></asp:Label>
                    </p>
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td class="footer1" colspan="2" align="center">
            <asp:Button runat="server" ID="Suspend" OnClick="Suspend_Click" CssClass="pbutton"  />
        </td>
    </tr>
</table>

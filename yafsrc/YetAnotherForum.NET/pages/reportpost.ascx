<%@ Control Language="C#" AutoEventWireup="true" CodeFile="reportpost.ascx.cs" Inherits="YAF.Pages.reportpost" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="2">
				Report Post <asp:HiddenField ID="MessageIDH" runat="server" />
			</td>			
		</tr> 		  
		<tr>
			<td class="postheader" style="width: 100px" valign="top">
				<b>Enter report text:</b>
			</td>		
			<td class="post">
				<asp:TextBox ID="txtReport" runat="server" TextMode="MultiLine" Width="100%" Height="100px"></asp:TextBox>
			</td>
		</tr>
		<tr class="footer1">
			<td colspan="2" align="center">
				<asp:Button ID="btnReport" runat="server" Text="Report" OnClick="btnReport_Click" />
				<asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
			</td>			
		</tr>	
	</table>


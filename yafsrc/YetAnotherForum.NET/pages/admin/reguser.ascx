<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.reguser" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Codebehind="reguser.ascx.cs" %>
<YAF:PageLinks id="PageLinks" runat="server" />
<YAF:adminmenu id="Adminmenu1" runat="server">
	
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
              <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_REGUSER" />
            </td>
		</tr>
		<tr>
			<td class="header2" colspan="2">
               <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_REGUSER" />
               
            </td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
               <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="USERNAME" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
            </td>
			<td class="post">
			<asp:TextBox Style="width: 350px" id="UserName" runat="server"></asp:TextBox>
			
			<asp:RequiredFieldValidator id="Requiredfieldvalidator1" runat="server" EnableClientScript="False"
          ControlToValidate="UserName" ErrorMessage="User Name is required."></asp:RequiredFieldValidator>
          </td>
		</tr>
		<tr>
			<td class="postheader">
               <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="EMAIL" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
            </td>
			<td class="post">
			<asp:TextBox Style="width: 350px" id="Email" runat="server"></asp:TextBox>
			
			<asp:RequiredFieldValidator id="Requiredfieldvalidator5" runat="server" EnableClientScript="False"
          ControlToValidate="Email" ErrorMessage="Email address is required."></asp:RequiredFieldValidator>
          </td>
		</tr>
		<tr>
			<td class="postheader">
               <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="PASSWORD" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>
            </td>
			<td class="post">
			   <asp:TextBox Style="width: 350px" id="Password" runat="server" TextMode="Password"></asp:TextBox>
			   <asp:RequiredFieldValidator id="Requiredfieldvalidator2" runat="server" EnableClientScript="False"
          ControlToValidate="Password" ErrorMessage="Password is required."></asp:RequiredFieldValidator>
            </td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="CONFIRM_PASSWORD" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
            </td>
			<td class="post">
			<asp:TextBox Style="width: 350px" id="Password2" runat="server" TextMode="Password"></asp:TextBox>
			<asp:CompareValidator id="Comparevalidator1" runat="server" NAME="Comparevalidator1" EnableClientScript="False"
          ControlToValidate="Password2" ErrorMessage="Passwords didnt match." ControlToCompare="Password"></asp:CompareValidator></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="SECURITY_QUESTION" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
            </td>
			<td class="post">
			<asp:TextBox Style="width: 350px" id="Question" runat="server"></asp:TextBox>
			
			<asp:RequiredFieldValidator id="Requiredfieldvalidator3" runat="server" EnableClientScript="False"
          ControlToValidate="Question" ErrorMessage="Password Question is Required."></asp:RequiredFieldValidator></td>
			
		</tr>
		<tr>
			<td class="postheader">
              <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="SECURITY_ANSWER" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
            </td>
			<td class="post">
			<asp:TextBox Style="width: 350px" id="Answer" runat="server"></asp:TextBox>
			
			<asp:RequiredFieldValidator id="Requiredfieldvalidator4" runat="server" EnableClientScript="False"
          ControlToValidate="Answer" ErrorMessage="Password Answer is Required."></asp:RequiredFieldValidator></td>
		</tr>
		<tr>
			<td class="header2" colspan="2">
              <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_REGUSER" />:
            </td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="LOCATION" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>:
             </td>
			<td class="post">
			<asp:TextBox Style="width: 350px" id="Location" runat="server"></asp:TextBox></td>
			
			
		</tr>
		<tr>
			<td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="HOMEPAGE" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>:
            </td>
			<td class="post">
			<asp:TextBox Style="width: 350px" id="HomePage" runat="server"></asp:TextBox></td>
			
			
		</tr>
		<tr>
			<td class="header2" colspan="2">
              <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEADER4" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>
            </td>
		</tr>
		<tr>
			<td class="postheader">
               <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="TIMEZONE" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>:
            </td>
			<td class="post">
			<asp:DropDownList Style="width: 350px" id="TimeZones" runat="server" DataValueField="Value" DataTextField="Name" CssClass="standardSelectMenu"></asp:DropDownList></td>
			
			
			
		</tr>
		<tr>
			<td class="footer1" align="center" colspan="2">
			<asp:Button id="ForumRegister" runat="server" onclick="ForumRegister_Click" CssClass="pbutton"></asp:Button>
			
			
			
			<asp:button id="cancel" runat="server" onclick="cancel_Click" CssClass="pbutton"></asp:button></td>
			
			
			
		</tr>
	</table>
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat="server" />

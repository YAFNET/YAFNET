<%@ Page language="c#" Codebehind="install.aspx.cs" AutoEventWireup="false" Inherits="yaf.install" %>

<form runat=server>

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1>Yet Another Forum.net Installation</td>
</tr>

<tr><td class=post>

	<table cellspacing=1 cellpadding=0 width=100% runat=server id=stepWelcome>
	<tr>
		<td class=header2 colspan=2>Welcome</td>
	</tr>
	<tr>
		<td colspan=2>
		<p>
			This installation wizard that will guide you through the
			installation of Yet Another Forum.net.
		</p>
		<p>
			Before you begin you should make sure you have setup your MS SQL server
			correctly. You can use an existing database, or you can create a new one.
		</p>
		<p>
			To begin the installation, click next.
		</p>
		</td>
	</tr>
	</table>

	<table cellspacing=1 cellpadding=0 width=100% runat=server visible=false id=stepConnect>
	<tr>
		<td class=header2 colspan=2>Step 1: Connect to database</td>
	</tr>
	<tr>
		<td colspan=2>
		<p>
			You will have to manually modify the connection string found in
			"<%= Server.MapPath("Web.config") %>" to point to your database.
		</p>
		<p>
			When you have entered the correct connection string, click next
			to continue with the installation.
		</p>
		</td>
	</tr>
	</table>

	<table cellspacing=1 cellpadding=0 width=100% runat=server visible=false id=stepDatabase>
	<tr>
		<td class=header2>Step 2: Initialize database</td>
	</tr>
	<tr>
		<td>
		<p>
			Your database will now be initialized. In a fresh install this means
			creating all tables and stored procedures. If this is an update to an
			already existing version, your database will be updated to the latest
			version.
		</p>
		<p>
			Click next to setup your database.
		</p>
		</td>
	</tr>
	</table>


	<table width=100% cellspacing=1 cellpadding=0 runat=server visible=false id=stepForum>
		<tr>
			<td colspan=2 class=header2>Step 3: Create Forum</td>
		</tr>
		<tr>
			<td class=postheader><b>Forum Name:</b><br>The name of your forum.</td>
			<td class=post>
				<asp:TextBox id=TheForumName runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td class=postheader><b>Time Zone:</b><br></td>
			<td class=post>
				<asp:DropDownList id=TimeZones runat="server" DataTextField="Name" DataValueField="Value"></asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td class=postheader><b>Forum Email:</b><br>The official forum email address.</td>
			<td class=post>
				<asp:TextBox id=ForumEmailAddress runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td class=postheader><b>SMTP Server:</b><br>The name of a smtp server used to send emails.</td>
			<td class=post>
				<asp:TextBox id=SmptServerAddress runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td class=postheader><b>Admin User Name:</b><br>The name of the admin user.</td>
			<td class=post>
				<asp:TextBox id=UserName runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td class=postheader><b>Admin Email:</b><br>The administrators email address.</td>
			<td class=post>
				<asp:TextBox id="AdminEmail" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td class=postheader><b>Admin Password:</b><br>The password of the admin user.</td>
			<td class=post>
				<asp:TextBox id=Password1 runat="server" TextMode="Password"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td class=postheader><b>Re-type Password:</b><br>Verify the password.</td>
			<td class=post>
				<asp:TextBox id="Password2" runat="server" TextMode="Password"></asp:TextBox>
			</td>
		</tr>
	</table>


	<table cellspacing=1 cellpadding=0 width=100% runat=server visible=false id=stepFinished>
	<tr>
		<td class=header2>Finished</td>
	</tr>
	<tr>
		<td>
		<p>
			Your forum is now ready to use. If this was a fresh install you will need to
			enter the admin section of the forum to setup categories and forums.
		</p>
		</td>
	</tr>
	</table>


</td></tr>

<tr>
	<td class=footer1 align=center>
		<asp:button id=back text=Back runat=server enabled=false></asp:button>
		<asp:button id=next text=Next runat=server></asp:button>
		<asp:button id=finish text=Finish runat=server enabled=false></asp:button>
		<asp:label id=cursteplabel runat=server visible=false></asp:label>
	</td>
</tr>

</table>


</form>

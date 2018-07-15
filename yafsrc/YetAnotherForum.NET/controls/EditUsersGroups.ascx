<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Controls.EditUsersGroups" Codebehind="EditUsersGroups.ascx.cs" %>

	<asp:Repeater ID="UserGroups" runat="server">
		<HeaderTemplate>
			
				<h2>
					<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEAD_USER_GROUPS" LocalizedPage="ADMIN_EDITUSER" />
                </h2>
			<hr />

		</HeaderTemplate>
		<ItemTemplate>
			    <h4>
			        <%# DataBinder.Eval(Container.DataItem, "Name") %>
			    </h4>
				<p>
					<asp:CheckBox CssClass="form-control" runat="server" ID="GroupMember" Checked='<%# this.IsMember(DataBinder.Eval(Container.DataItem,"Member")) %>' />
                </p>
					<asp:Label ID="GroupID" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "GroupID") %>'></asp:Label>
			
		</ItemTemplate>
	</asp:Repeater>
	
                <div class="text-lg-center">
                    <asp:CheckBox runat="server" CssClass="form-control" ID="SendEmail"/>
                    <YAF:ThemeButton ID="Save" runat="server" Type="Primary" OnClick="Save_Click" Icon="save" TextLocalizedTag="SAVE" />
                </div>
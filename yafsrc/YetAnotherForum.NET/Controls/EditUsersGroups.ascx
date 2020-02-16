<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Controls.EditUsersGroups" Codebehind="EditUsersGroups.ascx.cs" %>


<asp:Repeater ID="UserGroups" runat="server">
		<HeaderTemplate>
			
				<h2>
					<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEAD_USER_GROUPS" LocalizedPage="ADMIN_EDITUSER" />
                </h2>
			<div class="form-row">
        </HeaderTemplate>
		<ItemTemplate>
              <div class="form-group col-md-4">
			    <asp:Label runat="server" AssociatedControlID="GroupMember">
			        <%# DataBinder.Eval(Container.DataItem, "Name") %>
			    </asp:Label>
				<div class="custom-control custom-switch">
					<asp:CheckBox Text="&nbsp;" runat="server" ID="GroupMember" 
                                  Checked='<%# this.IsMember(DataBinder.Eval(Container.DataItem,"Member")) %>'/>
                </div>
					<asp:Label ID="GroupID" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "GroupID") %>'></asp:Label>
              </div>
        </ItemTemplate>
       <FooterTemplate>
           </div>
       </FooterTemplate>
	</asp:Repeater>
	<hr/>
                
<div class="text-lg-center">
    <div class="custom-control custom-switch">
        <asp:CheckBox runat="server" Text="&nbsp;" ID="SendEmail"/>
                    
    </div>
    <YAF:ThemeButton ID="Save" runat="server" 
                     Type="Primary" 
                     OnClick="Save_Click" 
                     Icon="save" 
                     TextLocalizedTag="SAVE" />
</div>
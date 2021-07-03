<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.EditUsersGroups" Codebehind="EditUsersGroups.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Objects" %>


<asp:Repeater ID="UserGroups" runat="server">
        <HeaderTemplate>
            <h2>
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server"
                                        LocalizedTag="HEAD_USER_GROUPS"
                                        LocalizedPage="ADMIN_EDITUSER" />
                </h2>
            <div class="row">
        </HeaderTemplate>
        <ItemTemplate>
              <div class="mb-3 col-md-4">
                <asp:Label runat="server" AssociatedControlID="GroupMember">
                    <%# (Container.DataItem as GroupMember).Name %>
                </asp:Label>
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" runat="server" ID="GroupMember"
                                  Checked="<%# this.IsMember((Container.DataItem as GroupMember).MemberCount) %>"/>
                </div>
                    <asp:Label ID="GroupID" Visible="false" runat="server" Text="<%# (Container.DataItem as GroupMember).GroupID %>"></asp:Label>
              </div>
        </ItemTemplate>
       <FooterTemplate>
           </div>
       </FooterTemplate>
    </asp:Repeater>
    <hr/>
<div class="row justify-content-md-center g-3 align-items-center">
    <div class="col-md-auto">
        <div class="form-check form-switch">
            <asp:CheckBox runat="server" ID="SendEmail"/>
        </div>
    </div>
    <div class="col-md-auto">
        <YAF:ThemeButton ID="Save" runat="server"
                         Type="Primary"
                         OnClick="Save_Click"
                         Icon="save"
                         TextLocalizedTag="SAVE" />
    </div>
</div>
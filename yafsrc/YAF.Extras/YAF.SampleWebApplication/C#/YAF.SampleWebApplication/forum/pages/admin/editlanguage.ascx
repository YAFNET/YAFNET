<%@ Control language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editlanguage" Codebehind="editlanguage.ascx.cs" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:AdminMenu runat="server" id="Adminmenu1">
  <table class="content" width="100%" cellspacing="0" cellpadding="0">
    <tr>
      <td class="header1" colspan="3">
        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EDITLANGUAGE" />
	    <asp:Label runat="server" id="lblPageName"></asp:Label>
      </td>
    </tr>
    <tr>
      <td class="header2" style="text-align:center">
      <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SELECT_PAGE" LocalizedPage="ADMIN_EDITLANGUAGE" />
	    <asp:DropDownList runat="server" id="dDLPages" ></asp:DropDownList>
	    <asp:Button runat="server" id="btnLoadPageLocalization"/>
      </td>
    </tr>
    <tr>
      <td style="text-align:center">
        <asp:Label runat="server" id="lblInfo" ForeColor="Red" Font-Bold="true"></asp:Label>
      </td>
    </tr>
    <tr>
      <td class="post" style="padding:0;margin:0;">
	    <asp:DataGrid id="grdLocals" Width="100%" BorderStyle="None" GridLines="None" BorderWidth="0px" runat="server" AutoGenerateColumns="False">
		  <Columns>
		    <asp:TemplateColumn>
			  <HeaderStyle HorizontalAlign="Center" CssClass="header2"></HeaderStyle>
              <HeaderTemplate>
                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="RESOURCE_NAME" LocalizedPage="ADMIN_EDITLANGUAGE" />
              </HeaderTemplate>
			  <ItemStyle HorizontalAlign="Center" CssClass="post"></ItemStyle>
			  <ItemTemplate>
			    <asp:Label id="lblResourceName" runat="server" Text='<%# Eval("ResourceName") %>'></asp:Label>
			  </ItemTemplate>
		    </asp:TemplateColumn>
			<asp:TemplateColumn>
			  <HeaderStyle HorizontalAlign="Center" CssClass="header2"></HeaderStyle>
               <HeaderTemplate>
               <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ORIGINAL_RESOURCE" LocalizedPage="ADMIN_EDITLANGUAGE" />
              </HeaderTemplate>
			  <ItemStyle HorizontalAlign="Center" CssClass="post"></ItemStyle>
			  <ItemTemplate>
			    <asp:TextBox id="txtResource" runat="server" Text='<%# Eval("ResourceValue") %>' Width="300px" Height="30px" Enabled="false">
                </asp:TextBox>
			  </ItemTemplate>
			</asp:TemplateColumn>
		    <asp:TemplateColumn>
			  <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="header2"></HeaderStyle>
               <HeaderTemplate>
                 <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="LOCALIZED_RESOURCE" LocalizedPage="ADMIN_EDITLANGUAGE" />
              </HeaderTemplate>
			  <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="post"></ItemStyle>
			  <ItemTemplate>
			    <asp:TextBox id="txtLocalized" runat="server" Text='<%# Eval("LocalizedValue") %>' Width="300px" Height="30px" ToolTip='<%# Eval("ResourceValue") %>'>
                </asp:TextBox>
                 <asp:CustomValidator runat="server" id="custTextLocalized" ControlToValidate="txtLocalized" OnServerValidate="LocalizedTextCheck"></asp:CustomValidator>
			  </ItemTemplate>
		    </asp:TemplateColumn>
		  </Columns>
	    </asp:DataGrid>
      </td>
    </tr>
    <tr>
      <td class="postfooter" style="text-align:center">
	    <asp:Button runat="server" CssClass="pbutton" id="btnSave" />&nbsp;
	    <asp:Button runat="server" CssClass="pbutton" id="btnCancel" />
      </td>
    </tr>
  </table>
</YAF:AdminMenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />

<%@ Control language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editlanguage" Codebehind="editlanguage.ascx.cs" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:AdminMenu runat="server" id="Adminmenu1">
  <table class="content" width="100%" cellspacing="0" cellpadding="0">
    <tr>
      <td class="header1" colspan="3"> 
	    Language Translation - <em>Current Ressource Page : <asp:Label runat="server" id="lblPageName"></asp:Label></em>
      </td>
    </tr>
    <tr>
      <td class="post" style="text-align:center">
	    <asp:Label runat="server" id="lblPages" Text="Select a Page:"></asp:Label>
	    <asp:DropDownList runat="server" id="dDLPages" ></asp:DropDownList>
	    <asp:Button runat="server" id="btnLoadPageLocalization" Text="Load Page Localization"/>
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
		    <asp:TemplateColumn HeaderText="Ressource Name">
			  <HeaderStyle HorizontalAlign="Center" CssClass="header2"></HeaderStyle>
			  <ItemStyle HorizontalAlign="Center" CssClass="post"></ItemStyle>
			  <ItemTemplate>
			    <asp:Label id="lblResourceName" runat="server" Text='<%# Eval("ResourceName") %>'></asp:Label>
			  </ItemTemplate>
		    </asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Original Ressource">
			  <HeaderStyle HorizontalAlign="Center" CssClass="header2"></HeaderStyle>
			  <ItemStyle HorizontalAlign="Center" CssClass="post"></ItemStyle>
			  <ItemTemplate>
			    <asp:TextBox id="txtResource" runat="server" Text='<%# Eval("ResourceValue") %>' Width="300px" Height="30px" Enabled="false">
                </asp:TextBox>
			  </ItemTemplate>
			</asp:TemplateColumn>
		    <asp:TemplateColumn HeaderText="Localized Ressource">
			  <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="header2"></HeaderStyle>
			  <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="post"></ItemStyle>
			  <ItemTemplate>
			    <asp:TextBox id="txtLocalized" runat="server" OnTextChanged="TextBoxTextChanged" AutoPostBack="true" Text='<%# Eval("LocalizedValue") %>' Width="300px" Height="30px" ToolTip='<%# Eval("ResourceValue") %>'>
                </asp:TextBox>
			  </ItemTemplate>
		    </asp:TemplateColumn>
		  </Columns>
	    </asp:DataGrid>
      </td>
    </tr>
    <tr>
      <td class="postfooter" style="text-align:center">
	    <asp:Button runat="server" id="btnSave" Text="Save Resources" />&nbsp;
	    <asp:Button runat="server" id="btnCancel" Text="Return" />
      </td>
    </tr>
  </table>
</YAF:AdminMenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />

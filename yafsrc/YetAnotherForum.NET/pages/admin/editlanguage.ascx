<%@ Control language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editlanguage" Codebehind="editlanguage.ascx.cs" %>

<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EDITLANGUAGE" /></h1>
	            </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3 card-outline-primary">
                <div class="card-header card-primary">
                    <i class="fa fa-language fa-fw"></i>&nbsp;<asp:Label runat="server" id="lblPageName"></asp:Label>
                </div>
                <div class="card-block text-lg-center">
                <h4>
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SELECT_PAGE" LocalizedPage="ADMIN_EDITLANGUAGE" />
                </h4>
                <p>
	                <asp:DropDownList runat="server" id="dDLPages" CssClass="custom-select"></asp:DropDownList>
                </p>
                <p>
	                <asp:LinkButton runat="server" id="btnLoadPageLocalization" CssClass="btn btn-primary" />
                </p>
                <p>
                    <asp:Label runat="server" id="lblInfo" ForeColor="Red" Font-Bold="true"></asp:Label>
                </p>
                <hr />
                <div class="alert alert-info hidden-sm-up" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                    <asp:DataGrid id="grdLocals" CssClass="table" runat="server" AutoGenerateColumns="False">
		  <Columns>
		    <asp:TemplateColumn>
			  <HeaderTemplate>
                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="RESOURCE_NAME" LocalizedPage="ADMIN_EDITLANGUAGE" />
              </HeaderTemplate>
			  <ItemTemplate>
			    <asp:Label id="lblResourceName" runat="server" Text='<%# this.Eval("ResourceName") %>'></asp:Label>
			  </ItemTemplate>
		    </asp:TemplateColumn>
			<asp:TemplateColumn>
			  <HeaderTemplate>
               <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ORIGINAL_RESOURCE" LocalizedPage="ADMIN_EDITLANGUAGE" />
              </HeaderTemplate>
			  <ItemTemplate>
			    <asp:TextBox id="txtResource" runat="server" Text='<%# this.Eval("ResourceValue") %>' Width="300px" Height="30px" Enabled="false" CssClass="form-control">
                </asp:TextBox>
			  </ItemTemplate>
			</asp:TemplateColumn>
		    <asp:TemplateColumn>
			  <HeaderTemplate>
                 <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="LOCALIZED_RESOURCE" LocalizedPage="ADMIN_EDITLANGUAGE" />
              </HeaderTemplate>
			  <ItemTemplate>
			    <asp:TextBox id="txtLocalized" runat="server" Text='<%# this.Eval("LocalizedValue") %>' Width="300px" Height="30px" ToolTip='<%# this.Eval("ResourceValue") %>' CssClass="form-control">
                </asp:TextBox>
                 <asp:CustomValidator runat="server" id="custTextLocalized" ControlToValidate="txtLocalized" OnServerValidate="LocalizedTextCheck"></asp:CustomValidator>
			  </ItemTemplate>
		    </asp:TemplateColumn>
		  </Columns>
	    </asp:DataGrid>
                    </div>
                </div>
                <div class="card-footer text-lg-center">
	                <asp:LinkButton runat="server" CssClass="btn btn-primary" id="btnSave" />&nbsp;
	                <asp:LinkButton runat="server" CssClass="btn btn-secondary" id="btnCancel" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />

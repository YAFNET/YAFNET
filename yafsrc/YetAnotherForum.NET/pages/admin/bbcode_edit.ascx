<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bbcode_edit" Codebehind="bbcode_edit.ascx.cs" %>

<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BBCODE_EDIT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-plug fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BBCODE_EDIT" />
                </div>
                <div class="card-body">
                    <h2><YAF:LocalizedLabel ID="HelpLabel13" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_BBCODE_EDIT" /></h2>
                    <hr />
			<h4>
              <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="BBCODE_NAME" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:textbox id="txtName" runat="server" CssClass="form-control"></asp:textbox>

			</p>
                    <hr />
			<h4>
              <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="EXEC_ORDER" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:TextBox ID="txtExecOrder" MaxLength="5" runat="server" Text="1" CssClass="form-control" TextMode="Number"></asp:TextBox>

			</p>
                    <hr />
			<h4>
              <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="BBCODE_DESC" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>

			</p>
                    <hr />
		    <h2>
               <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_BBCODE_EDIT" />
            </h2>
		    <hr />

			<h4>
              <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="SEARCH_REGEX" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:TextBox ID="txtSearchRegEx" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>

			</p>
                    <hr />
			<h4>
              <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="REPLACE_REGEX" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:TextBox ID="txtReplaceRegEx" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
			</p>
                    <hr />
			<h4>
              <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="USE_MODULE" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:CheckBox ID="chkUseModule" runat="server" CssClass="form-control" />
			</p>
                    <hr />
			<h4>
              <YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="CLASS_NAME" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:TextBox ID="txtModuleClass" runat="server" CssClass="form-control"></asp:TextBox>

			</p>
                    <hr />
			<h4>
              <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="REPLACE_VARIABLES" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:TextBox ID="txtVariables" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
			</p>
                    <hr />
		    <h2>
              <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_BBCODE_EDIT" />
            </h2>
			<h4>
              <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="ONCLICK_JS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:TextBox ID="txtOnClickJS" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
			</p>
                    <hr />
			<h4>
              <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="DISPLAY_JS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:TextBox ID="txtDisplayJS" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
			</p>
                    <hr />
			<h4>
              <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="EDIT_JS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:TextBox ID="txtEditJS" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
			</p>
                    <hr />
			<h4>
              <YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="DISPLAY_CSS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </h4>
			<p>
				<asp:TextBox ID="txtDisplayCSS" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
			</p>
                </div>
                <div class="card-footer text-lg-center">
				<YAF:ThemeButton id="save" runat="server"  OnClick="Add_Click" Type="Primary"
				                 Icon="save" TextLocalizedTag="SAVE"></YAF:ThemeButton>
				<YAF:ThemeButton id="cancel" runat="server"  OnClick="Cancel_Click" Type="Secondary"
				                 Icon="times" TextLocalizedTag="CANCEL"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />

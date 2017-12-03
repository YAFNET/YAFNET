<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bbcode_import" Codebehind="bbcode_import.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BBCODE_IMPORT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-plug fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BBCODE_IMPORT" />
                </div>
                <div class="card-body">
			    <h4>
                    <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="IMPORT_FILE" LocalizedPage="ADMIN_EXTENSIONS_IMPORT" />
                </h4>
			    <p>
			        <input type="file" id="importFile" class="form-control-file" runat="server" />
	            </p>
            </div>
            <div class="card-footer text-lg-center">
				<asp:LinkButton id="Import" runat="server" OnClick="Import_OnClick" CssClass="btn btn-primary"></asp:LinkButton>
				<asp:LinkButton id="cancel" runat="server" OnClick="Cancel_OnClick" CssClass="btn btn-secondary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
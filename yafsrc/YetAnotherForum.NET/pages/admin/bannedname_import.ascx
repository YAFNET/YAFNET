<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bannedname_import" Codebehind="bannedname_import.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BANNEDNAME_IMPORT" /></h1>
    </div>
    </div>
    <div class="row">
        
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="alert alert-warning" role="alert">
                <YAF:LocalizedLabel ID="LocalizedLabelRequirementsText" runat="server" 
                    LocalizedTag="NOTE" LocalizedPage="ADMIN_BANNEDNAME_IMPORT">
                </YAF:LocalizedLabel>
            </div>
            <div class="card card-primary-outline">
                <div class="card-header card-primary">
                    <i class="fa fa-hand-stop-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BANNEDNAME_IMPORT" />
                </div>
                <div class="card-block">
			        <h4>
                        <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="IMPORT_FILE" LocalizedPage="ADMIN_BANNEDNAME_IMPORT" />
                    </h4>
			        <p>
			            <input type="file" id="importFile" class="form-control-file" runat="server" />
			        </p>
                </div>
                <div class="card-footer text-lg-center">
				<asp:LinkButton id="Import" runat="server" CssClass="btn btn-primary" OnClick="Import_OnClick"></asp:LinkButton>
				<asp:LinkButton id="cancel" runat="server" CssClass="btn btn-secondary" OnClick="Cancel_OnClick"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.topicstatus_import" Codebehind="topicstatus_import.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_TOPICSTATUS_IMPORT" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-exclamation-triangle fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_TOPICSTATUS_IMPORT" />
			</div>
                <div class="card-body">
                    <h4>
              <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="IMPORT_FILE" LocalizedPage="ADMIN_TOPICSTATUS_IMPORT" />
            </h4>
			<p>
			  <input type="file" id="importFile" class="form-control-file" runat="server" />
			</p>
		 </div>
                <div class="card-footer text-lg-center">
				<asp:LinkButton id="Import" runat="server" CssClass="btn btn-primary" OnClick="Import_OnClick"></asp:LinkButton>
                &nbsp;
				<asp:LinkButton id="cancel" runat="server" CssClass="btn btn-secondary" OnClick="Cancel_OnClick"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

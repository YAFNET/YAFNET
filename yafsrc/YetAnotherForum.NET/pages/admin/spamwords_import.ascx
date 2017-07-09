<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.spamwords_import" Codebehind="spamwords_import.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_SPAMWORDS_IMPORT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3 card-outline-primary">
                <div class="card-header card-primary">
                    <i class="fa fa-shield fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_SPAMWORDS_IMPORT" />
		        </div>
                <div class="card-block">
                    <h4>
                        <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SELECT_IMPORT" LocalizedPage="ADMIN_SPAMWORDS_IMPORT" />
                    </h4>
			        <p>
                        <input type="file" id="importFile" class="form-control-file" runat="server" />
			        </p>
                </div>
                <div class="card-footer text-lg-center">
				    <asp:LinkButton id="Import" runat="server" OnClick="Import_OnClick" CssClass="btn btn-primary"></asp:LinkButton>&nbsp;
				    <asp:LinkButton id="cancel" runat="server" OnClick="Cancel_OnClick" CssClass="btn btn-secondary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

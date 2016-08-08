<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.extensions_edit" Codebehind="extensions_edit.ascx.cs" %>

<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EXTENSIONS_EDIT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card card-primary-outline">
                <div class="card-header card-primary">
                    <i class="fa fa-puzzle-piece fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                </div>
                <div class="card-block">
			        <h4>
                       <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="FILE_EXTENSION" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                    </h4>
			        <p>
				       <asp:textbox id="extension" runat="server" CssClass="form-control"></asp:textbox>
			        </p>
                </div>
                <div class="card-footer text-lg-center">
				    <asp:Linkbutton id="save" runat="server" CssClass="btn btn-primary"></asp:Linkbutton>&nbsp;
				    <asp:Linkbutton id="cancel" runat="server" CssClass="btn btn-secondary"></asp:Linkbutton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />

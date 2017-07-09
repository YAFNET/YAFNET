<%@ Control language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.smilies_import" Codebehind="smilies_import.ascx.cs" %>


<YAF:PageLinks runat="server" id="PageLinks"/>
<YAF:AdminMenu runat="server">
<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_SMILIES_IMPORT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3 card-outline-primary">
                <div class="card-header card-primary">
                    <i class="fa fa-smile-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_SMILIES_IMPORT" />
                </div>
                <div class="card-block">
                    <h4>
                        <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="CHOOSE_PAK" LocalizedPage="ADMIN_SMILIES_IMPORT" />
                    </h4>
	                <p>
	                    <asp:dropdownlist id="File" runat="server" CssClass="custom-select"/>
	                </p>
	                <h4>
	                    <YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETE_EXISTING" LocalizedPage="ADMIN_SMILIES_IMPORT" />
	                </h4>
	                <p>
	                    <asp:checkbox id="DeleteExisting" runat="server" CssClass="form-control"/>
	                </p>
                </div>
                <div class="card-footer text-lg-center">
                    <asp:LinkButton id="import" runat="server"  CssClass="btn btn-primary"/>
		            <asp:LinkButton id="cancel" runat="server"  CssClass="btn btn-secondary"/>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />

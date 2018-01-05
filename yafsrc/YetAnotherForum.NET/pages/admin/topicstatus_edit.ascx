<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.topicstatus_edit" Codebehind="topicstatus_edit.ascx.cs" %>

<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:AdminMenu runat="server" id="Adminmenu1">
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_TOPICSTATUS_EDIT" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-exclamation-triangle fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_TOPICSTATUS_EDIT" />
			</div>
                <div class="card-body">
                    <h4>
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="TOPICSTATUS_NAME" LocalizedPage="ADMIN_TOPICSTATUS_EDIT" />
            </h4>
			<p>
				<asp:textbox id="TopicStatusName" runat="server" CssClass="form-control" MaxLength="100"></asp:textbox></p>
            <hr />
			<h4>
                <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="DEFAULT_DESCRIPTION" LocalizedPage="ADMIN_TOPICSTATUS_EDIT" />
            </h4>
			<p>
				<asp:textbox id="DefaultDescription" runat="server" CssClass="form-control" MaxLength="100"></asp:textbox></p>
		 </div>
                <div class="card-footer text-lg-center">
				<asp:LinkButton id="save" runat="server" CssClass="btn btn-primary"></asp:LinkButton>
                &nbsp;
				<asp:LinkButton id="cancel" runat="server" CssClass="btn btn-secondary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />

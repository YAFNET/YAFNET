<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.deleteforum"
    CodeBehind="deleteforum.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_DELETEFORUM" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-comments fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_DELETEFORUM" />
                    <asp:Label ID="ForumNameTitle" runat="server"></asp:Label>
                </div>
                <div class="card-body">
            <h4 class="postheader">
                <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="MOVE_TOPICS" LocalizedPage="ADMIN_DELETEFORUM" />
            </h4>
            <p>
                <asp:CheckBox ID="MoveTopics" runat="server" AutoPostBack="true" CssClass="form-control"></asp:CheckBox>
            </p>
            <hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="NEW_FORUM" LocalizedPage="ADMIN_DELETEFORUM" />
            </h4>
            <p class="post">
                <asp:DropDownList ID="ForumList" runat="server" Enabled="false" CssClass="custom-select">
                </asp:DropDownList>
            </p>
                </div>
                <div class="card-footer text-lg-center">
                    <YAF:ThemeButton ID="Delete" runat="server" CssClass="btn btn-danger"
                                     Icon="trash" TextLocalizedTag="DELETE_FORUM" TextLocalizedPage="ADMIN_DELETEFORUM"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_FORUMS", "CONFIRM_DELETE") %>'></YAF:ThemeButton>&nbsp;
                    <YAF:ThemeButton ID="Cancel" runat="server" Type="Secondary"
                                     Icon="times" TextLocalizedTag="CANCEL"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
	<ContentTemplate>
		<asp:Timer ID="UpdateStatusTimer" runat="server" Enabled="false" Interval="4000" OnTick="UpdateStatusTimerTick" />
	</ContentTemplate>
</asp:UpdatePanel>

<div>
	<div id="DeleteForumMessage" style="display:none">
		<div class="card text-white text-center bg-danger mb-3">
		    <div class="card-body">
		        <blockquote class="blockquote">
                    <p>
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DELETE_TITLE" LocalizedPage="ADMIN_DELETEFORUM" />
                    </p>
                    <p>
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="DELETE_MSG" LocalizedPage="ADMIN_DELETEFORUM" />
                    </p>
                    <footer>
                        <asp:Image ID="LoadingImage" runat="server" alt="Processing..." />
                    </footer>
                </blockquote>
            </div>
        </div>
    </div>
</div>

<YAF:SmartScroller ID="SmartScroller1" runat="server" />

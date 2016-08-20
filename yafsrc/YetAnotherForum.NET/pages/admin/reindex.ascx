<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.reindex" Codebehind="reindex.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="AdminMenu1" runat="server">
<div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_REINDEX" /></h1>
    </div>
</div>
<div class="row">
    <div class="col-xl-12">
        <div class="card card-primary-outline">
            <div class="card-header card-primary">
                <i class="fa fa-database fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_REINDEX" />
            </div>
            <div class="card-block">
                <asp:TextBox ID="txtIndexStatistics" runat="server" Height="400px" TextMode="MultiLine"
                    CssClass="form-control"></asp:TextBox>
                <asp:Placeholder ID="PanelGetStats" runat="server" Visible="False">
                    <p class="card-text">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SHOW_STATS" LocalizedPage="ADMIN_REINDEX" />
                    </p>
                    <p class="card-text">
                        <asp:LinkButton ID="btnGetStats" CssClass="btn btn-primary" runat="server" OnClick="btnGetStats_Click" />
                    </p>
                    <hr />
                </asp:Placeholder>
                <asp:Placeholder ID="PanelRecoveryMode" runat="server" Visible="False">
                    <p class="card-text">
                        <asp:LinkButton ID="btnRecoveryMode" CssClass="btn btn-primary" runat="server" OnClick="btnRecoveryMode_Click" />
					    <asp:RadioButtonList ID="RadioButtonList1" runat="server" CssClass="form-control">
					    </asp:RadioButtonList>
                    </p>
                    <hr />
                </asp:Placeholder>
                <asp:Placeholder ID="PanelReindex" runat="server" Visible="False">
					<p class="card-text">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="REINDEX" LocalizedPage="ADMIN_REINDEX" />
                    </p>
                    <p class="card-text">
                        <asp:LinkButton ID="btnReindex" CssClass="btn btn-primary" runat="server" OnClick="btnReindex_Click" />
                    </p>
                    <hr />
                </asp:Placeholder>
                <asp:Placeholder ID="PanelShrink" runat="server" Visible="False">
					<p class="card-text">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="SHRINK" LocalizedPage="ADMIN_REINDEX" />
					</p>
                    <p class="card-text">
                        <asp:LinkButton ID="btnShrink" CssClass="btn btn-primary" runat="server" OnClick="btnShrink_Click" />
                    </p>
                </asp:Placeholder>
            </div>
        </div>
    </div>
</div>
</YAF:AdminMenu>

<div>
	<div id="DeleteForumMessage" style="display:none">
		<div class="card card-inverse card-danger text-xs-center">
		    <div class="card-block">
		        <blockquote class="card-blockquote">
                    <p>
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="REINDEX_TITLE" LocalizedPage="ADMIN_REINDEX" />
                    </p>
                    <p>
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="REINDEX_MSG" LocalizedPage="ADMIN_REINDEX" />
                    </p>
                    <footer>
                        <asp:Image ID="LoadingImage" runat="server" alt="Processing..." />
                    </footer>
                </blockquote>
            </div>
        </div>
    </div>
</div>

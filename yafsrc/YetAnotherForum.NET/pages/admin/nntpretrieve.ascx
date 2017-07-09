<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.nntpretrieve" Codebehind="nntpretrieve.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_NNTPRETRIEVE" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3 card-outline-primary">
                <div class="card-header card-primary">
                    <i class="fa fa-newspaper-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_NNTPRETRIEVE" />
                    </div>
                <div class="card-block">
                    <asp:Repeater runat="server" ID="List">
			<HeaderTemplate>
                <div class="alert alert-info hidden-sm-up" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                <table class="table">
				<tr>
                    <thead>
					<th>
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="GROUPS" LocalizedPage="ADMIN_NNTPRETRIEVE" />
					</th>
					<th>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="LAST_MESSAGE" LocalizedPage="ADMIN_NNTPRETRIEVE" />
					</th>
					<th>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="LAST_UPDATE" LocalizedPage="ADMIN_NNTPRETRIEVE" />
					</th>
                    </thead>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td>
						<%# this.Eval("GroupName") %>
					</td>
					<td>
						<%# this.LastMessageNo(Container.DataItem) %>
					</td>
					<td>
						<%# this.Get<IDateTime>().FormatDateTime(this.Eval("LastUpdate")) %>
					</td>
				</tr>
			</ItemTemplate>
            <FooterTemplate></table></div></FooterTemplate>
		</asp:Repeater>
                        <hr class="col-lg-12" />
                        <h4><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="TIME" LocalizedPage="ADMIN_NNTPRETRIEVE" /></h4>
                        <p><asp:TextBox runat="server" ID="Seconds" Text="30" CssClass="form-control SecondsInput" TextMode="Number" /></p>
                    </div>
                <div class="card-footer text-center">
                    <asp:LinkButton runat="server" ID="Retrieve" Text="Retrieve" CssClass="btn btn-primary" OnClick="Retrieve_Click" />
                </div>
            </div>
            </div>
        </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

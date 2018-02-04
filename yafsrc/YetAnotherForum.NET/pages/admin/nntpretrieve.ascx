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
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-newspaper fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_NNTPRETRIEVE" />
                    </div>
                <div class="card-body">
                    <asp:Repeater runat="server" ID="List">
			<HeaderTemplate>
                <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
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
                    <YAF:ThemeButton runat="server" ID="Retrieve" Type="Primary" OnClick="RetrieveClick"
                                     Icon="download" TextLocalizedTag="RETRIEVE"/>
                </div>
            </div>
            </div>
        </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

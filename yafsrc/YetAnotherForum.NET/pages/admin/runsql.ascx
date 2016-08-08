<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.runsql" Codebehind="runsql.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="AdminMenu1" runat="server">
<div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_RUNSQL" /></h1>
    </div>
</div>
<div class="row">
               <div class="col-xl-12">
                    <div class="card card-primary-outline">
                        <div class="card-header card-primary">
                             <i class="fa fa-database fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="SQL_COMMAND" LocalizedPage="ADMIN_RUNSQL" />
                        </div>
                        <div class="card-block">
                            <asp:TextBox ID="txtQuery" runat="server" TextMode="MultiLine" Width="100%" Height="100px" CssClass="form-control"></asp:TextBox>
                            <asp:Checkbox ID="chkRunInTransaction" runat="server" Text="Run In Transaction" Checked="true" CssClass="form-control" />
                        </div>
                        <div class="card-footer text-xs-center">
                            <asp:LinkButton ID="btnRunQuery" runat="server" CssClass="btn btn-primary" Text="Run Query" OnClick="btnRunQuery_Click" />
                        </div>
                    </div>
                </div>
    </div>
    <asp:PlaceHolder ID="ResultHolder" runat="server" Visible="false">
    <div class="row">
        <div class="col-xl-12">
                    <div class="card card-primary-outline">
                        <div class="card-header card-primary">
                             <i class="fa fa-rocket fa-fw"></i>&nbsp;Result
                        </div>
                        <div class="card-block">
                            <asp:TextBox ID="txtResult" runat="server" TextMode="MultiLine"  
                                Width="100%" Height="300px" Wrap="false" style="font-size: 8pt;" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
    </div>
		</asp:PlaceHolder>
</YAF:AdminMenu>

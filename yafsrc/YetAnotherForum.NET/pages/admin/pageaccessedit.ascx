<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.pageaccessedit" Codebehind="pageaccessedit.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE"  LocalizedPage="ADMIN_PAGEACCESSEDIT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3 card-outline-primary">
                <div class="card-header card-primary">
                    <i class="fa fa-building fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="TITLE"  LocalizedPage="ADMIN_PAGEACCESSEDIT" />
                </div>
                <div class="card-block">
                    <div class="alert alert-info hidden-sm-up" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                        <table class="table">
         <tr>
             <thead>
             <th>
                     <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_PAGEACCESSEDIT" />&nbsp;
                     <YAF:LocalizedLabel ID="UserNameLabel" runat="server" LocalizedTag="USERNAME" LocalizedPage="ADMIN_PAGEACCESSEDIT" />&nbsp;
                     <asp:Label ID="UserName" runat="server"  />&nbsp;
             </th>
             <th>
                     <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="PAGE" LocalizedPage="ADMIN_PAGEACCESSEDIT" />&nbsp;
             </th>
             <th>
                     <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="CANACCESS"  LocalizedPage="ADMIN_PAGEACCESSEDIT" />
            </th>
             </thead>
        </tr>

	      <asp:Repeater ID="AccessList" OnItemDataBound="AccessList_OnItemDataBound" runat="server">
            <ItemTemplate>
                <tr>
                     <td>
                        <strong>
                           <asp:Label ID="PageText" runat="server" />
                        </strong>
                      </td>
                     <td>
                        <strong>
                           <asp:Label ID="PageName" runat="server" />
                        </strong>
                    </td>
                    <td>
                      <asp:CheckBox  ID="ReadAccess" runat="server"/>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
                    </table>
                        </div>
                </div>
                <div class="card-footer text-lg-center">
				    <asp:LinkButton ID="Save" runat="server" OnClick="Save_Click" CssClass="btn btn-primary" />&nbsp;
                    <asp:LinkButton ID="GrantAll" runat="server" OnClick="GrantAll_Click" CssClass="btn btn-info" />&nbsp;
                    <asp:LinkButton ID="RevokeAll" runat="server" OnClick="RevokeAll_Click" CssClass="btn btn-danger" />&nbsp;
				    <asp:LinkButton ID="Cancel" runat="server" OnClick="Cancel_Click" CausesValidation="false" CssClass="btn btn-secondary" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

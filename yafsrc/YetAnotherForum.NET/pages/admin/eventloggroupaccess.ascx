<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.eventloggroupaccess" Codebehind="eventloggroupaccess.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE"  LocalizedPage="ADMIN_EVENTLOGROUPACCESS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-group fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE"  LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
                </div>
                <div class="card-body">
                    <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                        <table class="table">
                            <tr>
                                <thead>
             <th>
                     <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />&nbsp;
                     <YAF:LocalizedLabel ID="GroupNameLabel" runat="server" LocalizedTag="GROUPNAME" LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />&nbsp;
                     <asp:Label ID="GroupName" runat="server"  />&nbsp;
             </th>
             <th>
                     <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="EVENT" LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />&nbsp;
             </th>
             <th>
                     <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="VIEWACCESS"  LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
            </th>
             <th>
                     <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETEACCESS"  LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
            </th>
                                </thead>
        </tr>

	      <asp:Repeater ID="AccessList" OnItemDataBound="AccessList_OnItemDataBound" runat="server">
            <ItemTemplate>
                <tr>
                     <td>
                        <strong>
                           <asp:Label ID="EventText" runat="server" />
                        </strong>
                      </td>
                     <td>
                        <strong>
                           <asp:Label ID="EventTypeName" runat="server" />
                        </strong>
                    </td>
                      <td>
                      <asp:CheckBox  ID="ViewAccess" runat="server"/>
                    </td>
                    <td>
                      <asp:CheckBox  ID="DeleteAccess" runat="server"/>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
                    </table>
                 </div>
                </div>
                    <div class="card-footer text-lg-center">
				    <asp:LinkButton ID="Save" runat="server" OnClick="Save_Click" CssClass="btn btn-primary" />
                    <asp:LinkButton ID="GrantAll" runat="server" OnClick="GrantAll_Click" CssClass="btn btn-info" />
                    <asp:LinkButton ID="RevokeAll" runat="server" OnClick="RevokeAll_Click" CssClass="btn btn-danger" />
                    <asp:LinkButton ID="GrantAllDelete" runat="server" OnClick="GrantAllDelete_Click" CssClass="btn btn-info" />
                    <asp:LinkButton ID="RevokeAllDelete" runat="server" OnClick="RevokeAllDelete_Click" CssClass="btn btn-danger" />
				    <asp:LinkButton ID="Cancel" runat="server" OnClick="Cancel_Click" CausesValidation="false" CssClass="btn btn-secondary" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

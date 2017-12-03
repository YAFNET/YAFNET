<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.topicstatus" Codebehind="topicstatus.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
	  <asp:Repeater ID="list" runat="server">
        <HeaderTemplate>
      	<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_TOPICSTATUS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-warning fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_TOPICSTATUS" />
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
                        &nbsp;</th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TOPICSTATUS_NAME" LocalizedPage="ADMIN_TOPICSTATUS" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="DEFAULT_DESCRIPTION" LocalizedPage="ADMIN_TOPICSTATUS" />
                     </th>
                     <th>
                        &nbsp;</th>
                        </thead>
                </tr>
        	 </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                        <YAF:ThemeImage runat="server" ID="TopicStatusIcon" ThemePage="TOPIC_STATUS" ThemeTag='<%# this.HtmlEncode(this.Eval("TopicStatusName")) %>'></YAF:ThemeImage>

                </td>
                <td>
                        <%# this.HtmlEncode(this.Eval("TopicStatusName")) %>
                </td>
                <td>
						<%# this.HtmlEncode(this.Eval("DefaultDescription"))%>
				</td>
                <td>
                    <span class="pull-right">
                    <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="btn btn-info btn-sm" CommandName='edit' CommandArgument='<%# this.Eval("TopicStatusId") %>'
                        TextLocalizedTag="EDIT" TitleLocalizedTag="EDIT" Icon="edit" runat="server"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" CssClass="btn btn-danger btn-sm" OnLoad="Delete_Load"  CommandName='delete' CommandArgument='<%# this.Eval("TopicStatusId") %>'
                        TextLocalizedTag="DELETE" TitleLocalizedTag="DELETE" Icon="trash" runat="server"></YAF:ThemeButton>
                        </span>
                </td>
            </tr>
        	 </ItemTemplate>
        <FooterTemplate>
            </table></div>
                </div>
                <div class="card-footer text-lg-center">
                    <asp:LinkButton runat="server" CommandName='add' ID="Linkbutton3" CssClass="btn btn-primary" OnLoad="addLoad"></asp:LinkButton>
                    &nbsp;
                    <asp:LinkButton runat="server" CommandName='import' ID="Linkbutton5" CssClass="btn btn-info" OnLoad="importLoad"> </asp:LinkButton>
                    &nbsp;
                    <asp:LinkButton runat="server" CommandName='export' ID="Linkbutton4" CssClass="btn btn-warning" OnLoad="exportLoad"></asp:LinkButton>
                                    </div>
            </div>
        </div>
    </div>
        	 </FooterTemplate>
    	 </asp:Repeater>

</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

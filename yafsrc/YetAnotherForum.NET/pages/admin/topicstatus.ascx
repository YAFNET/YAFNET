<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.topicstatus" Codebehind="topicstatus.ascx.cs" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/TopicStatusImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/TopicStatusEdit.ascx" %>

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
                    <i class="fa fa-exclamation-triangle fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_TOPICSTATUS" />
                </div>
                <div class="card-body">
                    <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                        <table class="table">
                <tr>
                    <thead>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ICON" LocalizedPage="ADMIN_TOPICSTATUS" />
                    </th>
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
                    <span class="float-right">
                    <YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" CssClass="btn-sm" CommandName='edit' CommandArgument='<%# this.Eval("ID") %>'
                        TextLocalizedTag="EDIT" TitleLocalizedTag="EDIT" Icon="edit" runat="server"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm" CommandName='delete' CommandArgument='<%# this.Eval("ID") %>'
                        TextLocalizedTag="DELETE" TitleLocalizedTag="DELETE" Icon="trash" runat="server"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_TOPICSTATUS", "CONFIRM_DELETE") %>'></YAF:ThemeButton>
                        </span>
                </td>
            </tr>
        	 </ItemTemplate>
        <FooterTemplate>
            </table></div>
                </div>
                <div class="card-footer text-lg-center">
                    <YAF:ThemeButton runat="server" CommandName='add' ID="Linkbutton3" Type="Primary"
                                     Icon="plus-square" TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_TOPICSTATUS"></YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton runat="server" Icon="upload" DataTarget="TopicStatusImportDialog" ID="Linkbutton5" Type="Info"
                                     TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_TOPICSTATUS"> </YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton runat="server" CommandName='export' ID="Linkbutton4" Type="Warning"
                                     Icon="download" TextLocalizedTag="EXPORT" TextLocalizedPage="ADMIN_TOPICSTATUS"></YAF:ThemeButton>
                                    </div>
            </div>
        </div>
    </div>
        	 </FooterTemplate>
    	 </asp:Repeater>

</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />
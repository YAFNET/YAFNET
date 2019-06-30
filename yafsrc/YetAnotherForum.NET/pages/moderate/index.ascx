<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.moderate.index" Codebehind="index.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel runat="server" LocalizedTag="MODERATEINDEX_TITLE" /></h2>
    </div>
</div>
<div class="row">

<asp:Repeater ID="CategoryList" runat="server">
    <ItemTemplate>
        <div class="col">
                       <div class="card mb-3">
                       <div class="card-header">
                           <i class="fa fa-folder fa-fw text-warning"></i>&nbsp;<%# this.Eval( "Name") %>
                       </div>
                       <div class="card-body text-center">
			<asp:Repeater ID="ForumList" runat="server" OnItemCommand="ForumListItemCommand"
				DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'>
				<ItemTemplate>
                           <div class="list-group list-group-flush small">
                               <div class="list-group-item list-group-item-action">
                                   <h5 class="font-weight-bold"><%# DataBinder.Eval(Container.DataItem, "[\"Name\"]") %></h5>
                                   <asp:LinkButton ID="ViewUnapprovedPostsBtn" runat='server' 
                                                   CommandName='viewunapprovedposts' CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>' 
                                                   Visible='<%# this.Eval( "[\"MessageCount\"]").ToType<int>() > 0 %>' 
                                                   CssClass="btn btn-secondary btn-sm">
                                       <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                                           LocalizedTag="UNAPPROVED" /> 
                                       <span class="badge badge-light"><%# this.Eval( "[\"MessageCount\"]") %></span>
                                   </asp:LinkButton>
                                   <YAF:ThemeButton ID="NoUnapprovedInfo" 
                                                    TextLocalizedTag="NO_POSTS" TextLocalizedPage="MODERATE" 
                                                    runat="server"
                                                    Visible='<%# this.Eval( "[\"MessageCount\"]").ToType<int>() == 0 %>' 
                                                    Type="Secondary" CssClass="btn-sm disabled">
                                   </YAF:ThemeButton>
                                   <asp:LinkButton ID="ViewReportedBtn" runat='server' 
                                                   CommandName='viewreportedposts' CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>' 
                                                   Visible='<%# this.Eval( "[\"ReportedCount\"]").ToType<int>() > 0 %>' 
                                                   CssClass="btn btn-secondary btn-sm">
                                       <YAF:LocalizedLabel ID="ReportedCountLabel" runat="server" 
                                                           LocalizedTag="REPORTED" /> 
                                       <span class="badge badge-light"><%# this.Eval( "[\"ReportedCount\"]") %></span>
                                   </asp:LinkButton>
                                   <YAF:ThemeButton ID="NoReportedInfo" 
                                                    TextLocalizedTag="NO_POSTS" TextLocalizedPage="MODERATE"
                                                    runat="server"
                                                    Visible='<%# this.Eval( "[\"ReportedCount\"]").ToType<int>() == 0 %>' 
                                                    Type="Secondary" CssClass="btn-sm disabled">
                                   </YAF:ThemeButton>
                               </div>

                           </div>
				</ItemTemplate>
			</asp:Repeater>
                               </div>
                           </div>
            </div>
		</ItemTemplate>
	</asp:Repeater>
</div>
<asp:PlaceHolder id="InfoPlaceHolder" runat="server" Visible="false">
    <YAF:Alert runat="server" Dismissing="False" Type="success">
        <i class="fa fa-check fa-fw text-success"></i>&nbsp;
        <YAF:LocalizedLabel ID="NoCountInfo" 
                            LocalizedTag="NOMODERATION" 
                            LocalizedPage="MODERATE" 
                            runat="server">
        </YAF:LocalizedLabel>
    </YAF:Alert>
    
</asp:PlaceHolder>
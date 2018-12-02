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
                           <i class="fa fa-briefcase fa-fw"></i>&nbsp;<%# this.Eval( "Name") %>
                       </div>
                       <div class="card-body text-center">
			<asp:Repeater ID="ForumList" runat="server" OnItemCommand="ForumListItemCommand"
				DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'>
				<ItemTemplate>
                           <div class="list-group list-group-flush small">
                               <div class="list-group-item list-group-item-action">
                                   <h5 class="font-weight-bold"><%# DataBinder.Eval(Container.DataItem, "[\"Name\"]") %></h5>
                                   <p class="font-italic"><%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %></p>
						
                                   <strong><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                                               LocalizedTag="UNAPPROVED" />:</strong>
                                   <asp:LinkButton ID="ViewUnapprovedPostsBtn" runat='server' 
                                                   CommandName='viewunapprovedposts' CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>' 
                                                   Text='<%# this.Eval( "[\"MessageCount\"]") %>'
                                                   Visible='<%# this.Eval( "[\"MessageCount\"]").ToType<int>() > 0 %>' 
                                                   CssClass="btn btn-secondary btn-sm">
                                   </asp:LinkButton>
                                   <YAF:ThemeButton ID="NoUnapprovedInfo" 
                                                    TextLocalizedTag="NO_POSTS" TextLocalizedPage="MODERATE" 
                                                    runat="server"
                                                    Visible='<%# this.Eval( "[\"MessageCount\"]").ToType<int>() == 0 %>' 
                                                    Type="Secondary" CssClass="btn-sm disabled">
                                   </YAF:ThemeButton>
                                   <strong><YAF:LocalizedLabel ID="ReportedCountLabel" runat="server" 
                                                               LocalizedTag="REPORTED" />:</strong>
                                   <asp:LinkButton ID="ViewReportedBtn" runat='server' 
                                                   CommandName='viewreportedposts' CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>' 
                                                   Text='<%# this.Eval( "[\"ReportedCount\"]") %>'
                                                   Visible='<%# this.Eval( "[\"ReportedCount\"]").ToType<int>() > 0 %>' 
                                                   CssClass="btn btn-secondary btn-sm">
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
    <YAF:LocalizedLabel ID="NoCountInfo" 
                        LocalizedTag="NOMODERATION" 
                        LocalizedPage="MODERATE" 
                        runat="server">
    </YAF:LocalizedLabel>
</asp:PlaceHolder>


<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>

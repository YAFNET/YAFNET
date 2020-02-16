<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Moderate.Index" Codebehind="Index.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel runat="server" LocalizedTag="MODERATEINDEX_TITLE" /></h2>
    </div>
</div>
<div class="row">
    <asp:Repeater ID="CategoryList" runat="server" OnItemDataBound="CategoryList_OnItemDataBound">
        <ItemTemplate>
            <div class="col">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:Icon runat="server"
                                  IconName="folder"
                                  IconType="text-warning"></YAF:Icon>
                        <%# this.Eval( "Name") %>
                    </div>
                    <div class="card-body text-center">
                        <asp:Repeater ID="ForumList" runat="server" 
                                      OnItemCommand="ForumListItemCommand">
                            <ItemTemplate>
                                <div class="list-group list-group-flush small">
                                    <div class="list-group-item list-group-item-action">
                                        <h5 class="font-weight-bold">
                                            <%# DataBinder.Eval(Container.DataItem, "[\"Name\"]") %>
                                        </h5>
                                        <YAF:ThemeButton ID="ViewUnapprovedPostsBtn" runat="server" 
                                                         CommandName="viewunapprovedposts" 
                                                         CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>' 
                                                         Visible='<%# this.Eval( "[\"MessageCount\"]").ToType<int>() > 0 %>' 
                                                         Type="Secondary"
                                                         Size="Small">
                                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                                                LocalizedTag="UNAPPROVED" />
                                            <span class="badge badge-light"><%# this.Eval( "[\"MessageCount\"]") %></span>
                                        </YAF:ThemeButton>
                                        <YAF:ThemeButton ID="NoUnapprovedInfo" runat="server"
                                                         TextLocalizedTag="NO_POSTS" TextLocalizedPage="MODERATE" 
                                                         Visible='<%# this.Eval( "[\"MessageCount\"]").ToType<int>() == 0 %>' 
                                                         Type="Secondary"
                                                         Enabled="False"
                                                         Size="Small">
                                        </YAF:ThemeButton>
                                        <YAF:ThemeButton ID="ViewReportedBtn" runat="server" 
                                                         CommandName="viewreportedposts" 
                                                         CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>' 
                                                         Visible='<%# this.Eval( "[\"ReportedCount\"]").ToType<int>() > 0 %>' 
                                                         Type="Secondary"
                                                         Size="Small">
                                            <YAF:LocalizedLabel ID="ReportedCountLabel" runat="server" 
                                                                LocalizedTag="REPORTED" /> 
                                            <span class="badge badge-light"><%# this.Eval( "[\"ReportedCount\"]") %></span>
                                        </YAF:ThemeButton>
                                        <YAF:ThemeButton ID="NoReportedInfo" runat="server"
                                                         TextLocalizedTag="NO_POSTS" TextLocalizedPage="MODERATE"
                                                         Visible='<%# this.Eval( "[\"ReportedCount\"]").ToType<int>() == 0 %>' 
                                                         Type="Secondary"
                                                         Enabled="False"
                                                         Size="Small">
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
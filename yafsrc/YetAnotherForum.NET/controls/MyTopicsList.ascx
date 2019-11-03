<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.MyTopicsList" CodeBehind="MyTopicsList.ascx.cs"  EnableViewState="true" %>


<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-comments fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel runat="server" ID="Title" 
                                                                              LocalizedPage="MYTOPICS"
                                                                              LocalizedTag="ActiveTopics"></YAF:LocalizedLabel>
            </div>
            <div class="card-body">
                <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
                <asp:Repeater ID="TopicList" runat="server">
                    <ItemTemplate>
                        <h5 class="card-title">
                            <%# this.PrintForumName((System.Data.DataRowView)Container.DataItem) %>
                        </h5>
                        
                        
                        <%# this.CreateTopicLine((System.Data.DataRowView)Container.DataItem) %>
                        
                    </ItemTemplate>
                </asp:Repeater>
                <YAF:Pager runat="server" ID="PagerBottom" LinkedPager="PagerTop" OnPageChange="Pager_PageChange" />
            </div>
            <div class="card-footer">
                <div class="mb-1 form-inline">
                    <asp:Label runat="server" AssociatedControlID="Since">
                        <YAF:LocalizedLabel ID="SinceLabel" runat="server"
                                            LocalizedTag="SINCE"/>
                    </asp:Label>&nbsp;
                    <asp:DropDownList ID="Since" runat="server" 
                                      AutoPostBack="True" 
                                      OnSelectedIndexChanged="Since_SelectedIndexChanged" 
                                      CssClass="select2-select" />
                </div>
            </div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col">
        <div class="btn-group float-right" role="group" aria-label="Tools">
            <YAF:ThemeButton runat="server" OnClick="MarkAll_Click" ID="MarkAll"
                             TextLocalizedTag="MARK_ALL_ASREAD" TextLocalizedPage="DEFAULT"
                             Type="Secondary"
                             Size="Small"
                             Icon="glasses"/>
            <YAF:RssFeedLink ID="RssFeed" runat="server" Visible="False" />
        </div>
    </div>
</div>


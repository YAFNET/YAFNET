<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.MyTopicsList" CodeBehind="MyTopicsList.ascx.cs"  EnableViewState="true" %>


<div class="row">
    <div class="col">
        <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server" ID="IconHeader"
                                IconName="comments"
                                LocalizedPage="MYTOPICS"
                                LocalizedTag="ActiveTopics" />
            </div>
            <div class="card-body">
                <asp:Repeater ID="TopicList" runat="server">
                    <ItemTemplate>
                        <%# this.CreateTopicLine((System.Data.DataRowView)Container.DataItem) %>
                    </ItemTemplate>
                    <SeparatorTemplate>
                        <div class="row">
                            <div class="col">
                                <hr/>
                            </div>
                        </div>
                    </SeparatorTemplate>
                </asp:Repeater>
                <asp:PlaceHolder runat="server" Visible="<%# this.TopicList.Items.Count == 0 %>">
                    <div class="card-body">
                        <YAF:Alert runat="server" ID="Info"
                                   Type="info">
                            <YAF:Icon runat="server" IconName="info-circle" />
                            <YAF:LocalizedLabel runat="server" LocalizedTag="NO_POSTS" />
                        </YAF:Alert>
                    </div>
                </asp:PlaceHolder>
            </div>
            <asp:Panel runat="server" ID="Footer" 
                       CssClass="card-footer">
                <div class="input-group align-items-center">
                    <asp:Label runat="server" AssociatedControlID="Since"
                               CssClass="input-group-text">
                        <YAF:LocalizedLabel ID="SinceLabel" runat="server"
                                            LocalizedTag="SINCE"/>
                    </asp:Label>
                    <asp:DropDownList ID="Since" runat="server" 
                                      AutoPostBack="True" 
                                      OnSelectedIndexChanged="Since_SelectedIndexChanged" 
                                      CssClass="form-select" />
                </div>
            </asp:Panel>
    </div>
        <YAF:Pager runat="server" ID="PagerBottom" LinkedPager="PagerTop" OnPageChange="Pager_PageChange" />
    </div>
</div>
<div class="d-flex flex-row-reverse">
    <div>
        <YAF:ThemeButton runat="server" OnClick="MarkAll_Click" ID="MarkAll"
                         TextLocalizedTag="MARK_ALL_ASREAD" TextLocalizedPage="DEFAULT"
                         Type="Secondary"
                         Size="Small"
                         Icon="glasses"/>
        <YAF:RssFeedLink ID="RssFeed" runat="server" Visible="False" />
    </div>
</div>
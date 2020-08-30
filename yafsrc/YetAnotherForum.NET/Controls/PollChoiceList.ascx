<%@ Control Language="C#" AutoEventWireup="True" EnableViewState="false" CodeBehind="PollChoiceList.ascx.cs"
    Inherits="YAF.Controls.PollChoiceList" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

<asp:Repeater ID="Poll" runat="server" 
              OnItemDataBound="Poll_OnItemDataBound" 
              OnItemCommand="Poll_ItemCommand"
              Visible="true" DataSource="<%# this.DataSource %>">
    <HeaderTemplate>
        <ul class="list-group">
    </HeaderTemplate>
    <ItemTemplate>
        <li class="list-group-item list-group-item-action">
                <div class="d-flex w-100 justify-content-between">
                    <h5>
                        <asp:Image id="ChoiceImage" runat="server"
                                   CssClass="img-fluid mr-1"
                                   data-toggle="tooltip"
                                   style="max-height:80px"/>
                        <%# this.Get<IBadWordReplace>().Replace(this.Eval("Item2.ChoiceName").ToString()) %>
                    </h5>
                    <small>
                        <asp:Label id="YourChoice" visible="false" runat="server" 
                                   CssClass="badge bg-success"
                                   ToolTip='<%# this.GetText("POLLEDIT", "POLL_VOTED") %>'>
                            <i class="fa fa-check-circle fa-fw"></i>&nbsp;<%# this.GetText("POLLEDIT", "POLL_VOTED") %>
                        </asp:Label>
                    </small>
                </div>
                <YAF:ThemeButton ID="VoteButton"  runat="server" 
                                 CssClass="mb-2"
                                 Type="Success"
                                 Size="Small"
                                 Enabled="false"
                                 CommandName="vote"
                                 CommandArgument='<%# this.Eval("Item2.ID") %>'
                                 Icon="vote-yea"
                                 TitleLocalizedPage="POLLEDIT"
                                 TitleLocalizedTag="POLL_PLEASEVOTE"
                                 Text='<%# this.HtmlEncode(this.Get<IBadWordReplace>().Replace(this.Eval("Item2.ChoiceName").ToString())) %>' />
                <asp:Panel ID="resultsSpan" Visible="false" runat="server" CssClass="progress">
                    <div class="progress-bar progress-bar-striped" 
                         role="progressbar" 
                         style="width: <%# this.VoteWidth(Container.DataItem) %>%" 
                         aria-valuenow="<%# this.VoteWidth(Container.DataItem) %>" aria-valuemin="0" aria-valuemax="100">
                        <%# this.VoteWidth(Container.DataItem) %>%
                    </div>
                </asp:Panel>
                <asp:Label ID="VoteSpan" Visible="false" runat="server">
                    <%# this.Eval("Item2.Votes") %>&nbsp; <%# this.GetText("VOTES") %>
                    <asp:Label runat="server" ID="Voters" 
                               CssClass="ml-1 text-muted"></asp:Label>
                </asp:Label>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
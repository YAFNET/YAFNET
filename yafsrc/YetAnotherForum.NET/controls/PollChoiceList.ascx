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
            <asp:PlaceHolder id="VoteTr" runat="server">
                <div class="d-flex w-100 justify-content-between">
                    <h5>
                        <img id="ChoiceImage" runat="server"
                             class="img-thumbnail mr-1"
                             alt='<%# this.HtmlEncode(this.Get<IBadWordReplace>().Replace(Convert.ToString(DataBinder.Eval(Container.DataItem, "Choice")))) %>' />
                        <%# DataBinder.Eval(Container.DataItem, "Choice") %>
                    </h5>
                    <small>
                        <asp:Label id="YourChoice" visible="false" runat="server" 
                                   CssClass="badge badge-success"
                                   ToolTip='<%# this.GetText("POLLEDIT", "POLL_VOTED") %>'>
                            <i class="fa fa-check-circle fa-fw"></i>&nbsp;<%# this.GetText("POLLEDIT", "POLL_VOTED") %>
                        </asp:Label>
                    </small>
                </div>
                         <p>
                          <asp:LinkButton ID="MyLinkButton1" 
                                           CssClass="btn btn-success btn-sm" 
                                           Enabled="false" runat="server" 
                                           CommandName="vote"
                                           CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>'>
                              <i class="fa fa-vote-yea fa-fw"></i>&nbsp;<%# this.HtmlEncode(this.Get<IBadWordReplace>().Replace(Convert.ToString(DataBinder.Eval(Container.DataItem, "Choice")))) %>
                          </asp:LinkButton>
                         </p>
                
                <asp:Panel ID="resultsSpan" Visible="false" runat="server" CssClass="progress">
                    <div class="progress-bar progress-bar-striped" 
                         role="progressbar" 
                         style="width: <%# DataBinder.Eval(Container.DataItem,"Stats") %>%" 
                         aria-valuenow='<%# DataBinder.Eval(Container.DataItem,"Stats") %>' aria-valuemin="0" aria-valuemax="100">
                        <%# DataBinder.Eval(Container.DataItem,"Stats") %>%
                    </div>
                </asp:Panel>
                <asp:Label ID="VoteSpan" Visible="false" runat="server">
                    <%# DataBinder.Eval(Container.DataItem, "Votes") %>&nbsp; <%# this.GetText("VOTES") %>
                    <asp:Label runat="server" ID="Voters"></asp:Label>
                </asp:Label>
                    </asp:PlaceHolder>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
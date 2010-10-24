<%@ Control Language="C#" AutoEventWireup="True" EnableViewState="false" CodeBehind="PollChoiceList.ascx.cs"
    Inherits="YAF.controls.PollChoiceList" %>
            <asp:Repeater ID="Poll" runat="server" OnItemDataBound="Poll_OnItemDataBound" OnItemCommand="Poll_ItemCommand"
                Visible="true" DataSource="<%# this.DataSource %>">
                <HeaderTemplate> 
                </HeaderTemplate>
                <ItemTemplate>
                    <tr id="VoteTr" runat="server">
                        <td class="post" align="center" width="1">
                            <div class="attachedimg" style="display: inline; height: 50px">
                                <a id="ChoiceAnchor" runat="server">
                                    <img id="ChoiceImage" src="" alt="" runat="server" />
                                </a>
                            </div>
                        </td>
                        <td class="post">
                         <img id="YourChoice" visible="false" runat="server" alt='<%# PageContext.Localization.GetText("POLLEDIT", "POLL_VOTED") %>'
                                   title='<%# PageContext.Localization.GetText("POLLEDIT", "POLL_VOTED") %>'
                                   width="16" height="16" src='<%# GetThemeContents("VOTE","POLL_VOTED") %>' />&nbsp; 
                          <YAF:MyLinkButton ID="MyLinkButton1" Enabled="false" runat="server" CommandName="vote"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>' Text='<%# this.HtmlEncode(this.Get<YafBadWordReplace>().Replace(Convert.ToString(DataBinder.Eval(Container.DataItem, "Choice")))) %>' />
                                
                        </td>
                        <td class="post" align="center">
                            <asp:Panel ID="VoteSpan" Visible="false" runat="server">
                                <%# DataBinder.Eval(Container.DataItem, "Votes") %>
                            </asp:Panel>
                            <asp:Panel ID="MaskSpan" Visible="false" runat="server">
                                <img alt='<%# PageContext.Localization.GetText("POLLEDIT", "POLLRESULTSHIDDEN_SHORT") %>'
                                    title='<%# PageContext.Localization.GetText("POLLEDIT", "POLLRESULTSHIDDEN_SHORT") %>'
                                    src='<%# GetThemeContents("VOTE","POLL_MASK") %>' />
                            </asp:Panel>
                        </td>
                        <td class="post">
                            <asp:Panel ID="resultsSpan" Visible="false" runat="server">
                                <nobr>               
					<img alt="" src="<%# GetThemeContents("VOTE","LCAP") %>" /><img alt="" src='<%# GetThemeContents("VOTE","BAR") %>'
						height="12" width='<%# VoteWidth(Container.DataItem) %>%' /><img alt="" src='<%# GetThemeContents("VOTE","RCAP") %>' /></nobr>
                                <%# DataBinder.Eval(Container.DataItem,"Stats") %>
                                %
                            </asp:Panel>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>                
                 </FooterTemplate>
            </asp:Repeater>        


<br />

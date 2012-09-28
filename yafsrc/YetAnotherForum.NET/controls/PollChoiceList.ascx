<%@ Control Language="C#" AutoEventWireup="True" EnableViewState="false" CodeBehind="PollChoiceList.ascx.cs"
    Inherits="YAF.Controls.PollChoiceList" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<asp:Repeater ID="Poll" runat="server" OnItemDataBound="Poll_OnItemDataBound" OnItemCommand="Poll_ItemCommand"
                Visible="true" DataSource="<%# this.DataSource %>">
                <HeaderTemplate> 
                </HeaderTemplate>
                <ItemTemplate>
                    <tr id="VoteTr" runat="server">
                        <td class="post" align="center" width="1">
                            <div class="attachedimg ceebox" style="display: inline; height: 50px">
                                <a id="ChoiceAnchor" runat="server" title='<%# this.HtmlEncode(this.Get<IBadWordReplace>().Replace(Convert.ToString(DataBinder.Eval(Container.DataItem, "Choice")))) %>'>
                                    <img id="ChoiceImage" src="" alt='<%# this.HtmlEncode(this.Get<IBadWordReplace>().Replace(Convert.ToString(DataBinder.Eval(Container.DataItem, "Choice")))) %>' runat="server" />
                                </a>
                            </div>
                        </td>
                        <td class="post">
                         <img id="YourChoice" visible="false" runat="server" alt='<%# this.GetText("POLLEDIT", "POLL_VOTED") %>'
                                   title='<%# this.GetText("POLLEDIT", "POLL_VOTED") %>'
                                   width="16" height="16" src='<%# GetThemeContents("VOTE","POLL_VOTED") %>' />&nbsp; 
                          <YAF:MyLinkButton ID="MyLinkButton1"   CssClass="pollvote a" Enabled="false" runat="server" CommandName="vote"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>' Text='<%# this.HtmlEncode(this.Get<IBadWordReplace>().Replace(Convert.ToString(DataBinder.Eval(Container.DataItem, "Choice")))) %>' />
                                
                        </td>
                        <td class="post" align="center">
                            <asp:Panel ID="VoteSpan" Visible="false" runat="server">
                                <%# DataBinder.Eval(Container.DataItem, "Votes") %>
                            </asp:Panel>
                            <asp:Panel ID="MaskSpan" Visible="false" runat="server">
                                <img alt='<%# this.GetText("POLLEDIT", "POLLRESULTSHIDDEN_SHORT") %>'
                                    title='<%# this.GetText("POLLEDIT", "POLLRESULTSHIDDEN_SHORT") %>'
                                    src='<%# GetThemeContents("VOTE","POLL_MASK") %>' />
                            </asp:Panel>
                        </td>
                        <td class="post">
                            <asp:Panel ID="resultsSpan" Visible="false" runat="server">
                                <nobr>               
					<img alt="" src="<%# GetThemeContents("VOTE","LCAP") %>" /><img id="ImgVoteBar" alt="" src='<%# GetThemeContents("VOTE","BAR") %>'
						height="12" width='<%# VoteWidth(Container.DataItem) %>' runat="server" /><img alt="" src='<%# GetThemeContents("VOTE","RCAP") %>' /></nobr>
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

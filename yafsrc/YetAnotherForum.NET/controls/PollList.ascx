<%@ Control Language="C#" AutoEventWireup="True" EnableViewState="false" CodeBehind="PollList.ascx.cs"
    Inherits="YAF.Controls.PollList" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Register TagPrefix="YAF" TagName="PollChoiceList" Src="PollChoiceList.ascx" %>
<asp:PlaceHolder id="PollListHolder" runat="server" Visible="true">
<table cellpadding="0" cellspacing="1" class="content" width="100%">
    <asp:Repeater ID="PollGroup" OnItemCommand="PollGroup_ItemCommand" OnItemDataBound="PollGroup_OnItemDataBound"
        runat="server" Visible="true">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
               <tr>
                <td class="header1" align="center" style="width: 1">
                    <div class="attachedimg ceebox" style="display: inline; height: 50px">
                        <a id="QuestionAnchor" runat="server" title='<%# GetPollQuestion(DataBinder.Eval(Container.DataItem, "PollID"))%>'>
                            <img id="QuestionImage" src="" alt="" runat="server" />
                        </a>
                    </div>
                </td>
                <td class="header1" colspan="3">                  
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="question" />
                    :
                    <asp:Label ID="QuestionLabel" Text='<%# GetPollQuestion(DataBinder.Eval(Container.DataItem, "PollID"))%>'  runat="server"></asp:Label>          

                </td>
            </tr>
            <tr>
                <td class="header2">        
                </td>
                <td class="header2">
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="choice" />
                </td>
                <td class="header2" align="center" width="10%">
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="votes" />
                </td>
                <td class="header2" width="40%">
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="statistics" />
                </td>
            </tr>
        <YAF:PollChoiceList ID="PollChoiceList1"  runat="server" /> 
          <tr>
                <td class="header2">
                <img id="PollClosedImage" title="" src="" alt="" visible="false"  runat="server" />&nbsp; 
                              <YAF:ThemeButton ID="RefuseVoteButton" runat="server" Visible="false"
                        CommandName="refuse" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>' CssClass="yaflittlebutton rightItem" ImageThemePage="vote" ImageThemeTag="VOTE_REFUSE"  TitleLocalizedTag="POLL_ALLOWSKIPVOTE_INFO" />  
                </td>
                <td class="header2">
                    <%= this.GetText("total") %>
                </td>
                <td class="header2" align="center">
                    <%# DataBinder.Eval(Container.DataItem, "Total") %>
                </td>
                <td class="header2">
                    100%
                </td>            
               
            </tr>
            <tr id="PollInfoTr" runat="server" visible="false">
               <td class="post" colspan="4" align="center">
                <asp:Label ID="PollNotification" Visible="false" runat="server" />                   
               </td>
            </tr>   
           <tr id="PollCommandRow" runat="server">
                <td class="command" width="100%" colspan="4">
                    <YAF:ThemeButton ID="RemovePollAll" runat="server" Visible="false" CommandName="removeall"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>' CssClass="yafcssbigbutton rightItem"
                        TextLocalizedTag="REMOVEPOLL_ALL" />
                    <YAF:ThemeButton ID="RemovePoll" runat="server" Visible="false" CommandName="remove"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>' CssClass="yafcssbigbutton rightItem"
                        TextLocalizedTag="REMOVEPOLL" />
                    <YAF:ThemeButton ID="EditPoll" runat="server" Visible='<%# CanEditPoll(DataBinder.Eval(Container.DataItem, "PollID")) %>'
                        CommandName="edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>'
                        CssClass="yafcssbigbutton rightItem" TextLocalizedTag="EDITPOLL" />
                    <YAF:ThemeButton ID="CreatePoll" runat="server" Visible='<%# CanCreatePoll() %>'
                        CommandName="new" CssClass="yafcssbigbutton rightItem" TextLocalizedTag="CREATEPOLL" />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr id="PollGroupInfoTr" runat="server" visible="false">
               <td class="post" colspan="4" align="center">                         
                <asp:Label ID="PollGroupNotification" Visible="false" runat="server" />          
               </td>
            </tr>   
            <tr id="PollGroupCommandRow" runat="server">
                <td class="command" width="100%" colspan="4">
                    <YAF:ThemeButton ID="RemoveGroupAll" runat="server" Visible='<%# CanRemoveGroupCompletely() %>'
                        CommandName="removegroupall" CssClass="yafcssbigbutton rightItem" TextLocalizedTag="REMOVEPOLLGROUP_ALL" />
                    <YAF:ThemeButton ID="RemoveGroupEverywhere" runat="server" Visible='<%# CanRemoveGroupEverywhere() %>'
                        CommandName="removegroupevery" CssClass="yafcssbigbutton rightItem" TextLocalizedTag="DETACHGROUP_EVERYWHERE" />
                    <YAF:ThemeButton ID="RemoveGroup" runat="server" Visible='<%# CanRemoveGroup() %>'
                        CommandName="removegroup" CssClass="yafcssbigbutton rightItem" TextLocalizedTag="DETACHPOLLGROUP" />
                </td>
            </tr>
        </FooterTemplate>
    </asp:Repeater>
    <tr id="NewPollRow" runat="server" visible="false">
        <td width="100%" colspan="3">
            <YAF:ThemeButton ID="CreatePoll1" runat="server" CssClass="yafcssbigbutton rightItem"
                TextLocalizedTag="CREATEPOLL" />
        </td>
    </tr>
</table>
</asp:PlaceHolder>
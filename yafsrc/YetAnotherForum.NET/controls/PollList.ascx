<%@ Control Language="C#" AutoEventWireup="True" EnableViewState="false" CodeBehind="PollList.ascx.cs"
    Inherits="YAF.Controls.PollList" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Register TagPrefix="YAF" TagName="PollChoiceList" Src="PollChoiceList.ascx" %>


<asp:PlaceHolder id="PollListHolder" runat="server" Visible="true">
    <asp:Repeater ID="PollGroup" OnItemCommand="PollGroup_ItemCommand" OnItemDataBound="PollGroup_OnItemDataBound"
        runat="server" Visible="true">
        <ItemTemplate>
            <div class="row">
            <div class="col">
            <div class="card bg-light  mb-3">
            <div class="card-header">
                <i class="fa fa-question-circle fa-fw"></i>&nbsp; <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="question" />
                :
                <asp:Label ID="QuestionLabel"
                           Text='<%# this.GetPollQuestion(DataBinder.Eval(Container.DataItem, "PollID"))%>'  runat="server"></asp:Label>
                <asp:Label runat="server" ID="PollClosed" Visible="False"></asp:Label>
            </div>
            <div class="card-body">
                <img id="QuestionImage" src="" alt="" runat="server" />
               
        <YAF:PollChoiceList ID="PollChoiceList1"  runat="server" />
          <YAF:ThemeButton ID="RefuseVoteButton" runat="server" Visible="false"
                           CommandName="refuse" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>' 
                           Size="Small" 
                           TitleLocalizedTag="POLL_ALLOWSKIPVOTE_INFO"
                           Icon="poll"/>
            <p class="card-text">
                <%= this.GetText("total") %>: <%# DataBinder.Eval(Container.DataItem, "Total") %>
            </p>
            <asp:PlaceHolder id="PollInfoTr" runat="server" visible="false">
               <YAF:Alert runat="server" Type="info">
                   <asp:Label ID="PollNotification" Visible="false" runat="server" />
               </YAF:Alert>
            </asp:PlaceHolder>
           <asp:PlaceHolder id="PollCommandRow" runat="server">
               
                    <YAF:ThemeButton ID="RemovePollAll" runat="server" Visible="false" 
                                     CommandName="removeall" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>'
                                     CssClass="btn-sm mr-1"
                                     TextLocalizedTag="REMOVEPOLL_ALL"
                                     ReturnConfirmText='<%# this.GetText("POLLEDIT", "ASK_POLL_DELETE_ALL") %>'
                                     Type="Secondary"
                                     Icon="trash"/>
                    <YAF:ThemeButton ID="RemovePoll" runat="server" Visible="false" 
                                     CommandName="remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>' 
                                     CssClass="btn-sm mr-1"
                                     TextLocalizedTag="REMOVEPOLL"
                                     ReturnConfirmText='<%# this.GetText("POLLEDIT", "ASK_POLL_DELETE")  %>'
                                     Type="Secondary"
                                     Icon="trash"/>
                    <YAF:ThemeButton ID="EditPoll" runat="server" Visible='<%#  this.CanEditPoll(DataBinder.Eval(Container.DataItem, "PollID")) %>'
                                     CommandName="edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>'
                                     CssClass="btn-sm mr-1" 
                                     TextLocalizedTag="EDITPOLL"
                                     Type="Secondary"
                                     Icon="edit"/>
                    <YAF:ThemeButton ID="CreatePoll" runat="server" Visible='<%# this.CanCreatePoll() %>'
                                     CommandName="new" 
                                     Size="Small" 
                                     TextLocalizedTag="CREATEPOLL"
                                     Icon=""
                                     Type="Secondary" />
            </asp:PlaceHolder>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            <div class="card-footer text-center">


            <asp:PlaceHolder id="PollGroupInfoTr" runat="server" visible="false">
               <small class="text-muted">
                <asp:Label ID="PollGroupNotification" Visible="false" runat="server" />
                </small>
            </asp:PlaceHolder>
            <asp:PlaceHolder id="PollGroupCommandRow" runat="server">
                    <YAF:ThemeButton ID="RemoveGroupAll" runat="server" Visible='<%# this.CanRemoveGroupCompletely() %>'
                                     CommandName="removegroupall" 
                                     ReturnConfirmText='<%# this.GetText("POLLEDIT", "ASK_POLLROUP_DELETE_ALL") %>'
                                     TextLocalizedTag="REMOVEPOLLGROUP_ALL"
                                     CssClass="mr-1"
                                     Type="Secondary"
                                     Icon="trash"/>
                    <YAF:ThemeButton ID="RemoveGroupEverywhere" runat="server" Visible='<%# this.CanRemoveGroupEverywhere() %>'
                                     CommandName="removegroupevery" 
                                     ReturnConfirmText='<%# this.GetText("POLLEDIT", "ASK_POLLROUP_DETACH_EVR") %>'
                                     TextLocalizedTag="DETACHGROUP_EVERYWHERE"
                                     CssClass="mr-1"
                                     Type="Secondary"
                                     Icon="compress-arrows-alt"/>
                    <YAF:ThemeButton ID="RemoveGroup" runat="server" Visible='<%# this.CanRemoveGroup() %>'
                                     CommandName="removegroup" 
                                     ReturnConfirmText='<%# this.GetText("POLLEDIT", "ASK_POLLGROUP_DETACH") %>'
                                     TextLocalizedTag="DETACHPOLLGROUP"
                                     Type="Secondary"
                                     Icon="compress-arrows-alt"/>
                </asp:PlaceHolder>
            </div>
            </div>
            </div>
            </div>
        </FooterTemplate>
    </asp:Repeater>
    <asp:PlaceHolder id="NewPollRow" runat="server" visible="false">
        <div class="row">
            <div class="col">
                <YAF:ThemeButton ID="CreatePoll1" runat="server"
                                 TextLocalizedTag="CREATEPOLL"
                                 Icon="poll-h"
                                 Type="Secondary"/>
            </div>
        </div>
        
    </asp:PlaceHolder>
</asp:PlaceHolder>
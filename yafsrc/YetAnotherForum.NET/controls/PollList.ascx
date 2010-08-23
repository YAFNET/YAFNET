<%@ Control Language="C#" AutoEventWireup="True" EnableViewState="false" CodeBehind="PollList.ascx.cs" Inherits="YAF.controls.PollList" %>

<table cellpadding="0" cellspacing="1" class="content" width="100%">
<asp:Repeater ID="PollGroup" OnItemCommand="PollGroup_ItemCommand" OnItemDataBound="PollGroup_OnItemDataBound"  runat="server" Visible="true"> 
<HeaderTemplate>          
</HeaderTemplate>
<ItemTemplate>
<tr>
                <td class="header1" colspan="3">
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="question" />
                    : <asp:Label ID="QuestionLabel" Text='<%# GetPollQuestion(DataBinder.Eval(Container.DataItem, "PollID"))%>' runat="server"></asp:Label>
                   
                    <%# GetPollIsClosed(DataBinder.Eval(Container.DataItem, "PollID"))%>
                </td>
            </tr>
            <tr>
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
 <asp:HiddenField ID="PollID"  runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "PollID") %>' />
<asp:Repeater ID="Poll" runat="server"  OnItemDataBound="Poll_OnItemDataBound" OnItemCommand="Poll_ItemCommand" Visible="false" >
    <HeaderTemplate></HeaderTemplate>
    <ItemTemplate>    
        <tr>        
            <td class="post"> 
                 <asp:HiddenField ID="PollIDChoice"  runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "PollID") %>' />              
                <YAF:MyLinkButton ID="MyLinkButton1" Enabled="false" runat="server" CommandName="vote" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>'
                    Text='<%# HtmlEncode(YafServices.BadWordReplace.Replace(Convert.ToString(DataBinder.Eval(Container.DataItem, "Choice")))) %>' />
            </td>
            <td class="post" align="center">
            <asp:Panel id="VoteSpan" visible="false" runat="server">
                <%# DataBinder.Eval(Container.DataItem, "Votes") %>
           </asp:Panel>
            </td>             
            <td class="post" >
            <asp:Panel id="resultsSpan" visible="false" runat="server">
            <nobr>               
					<img alt="" src="<%# GetThemeContents("VOTE","LCAP") %>"><img alt="" src='<%# GetThemeContents("VOTE","BAR") %>'
						height="12" width='<%# VoteWidth(Container.DataItem) %>%'><img alt="" src='<%# GetThemeContents("VOTE","RCAP") %>'></nobr>
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
             <tr>
            <td class="header2">
                <%= PageContext.Localization.GetText("total") %>
            </td>
            <td class="header2" align="center">
            <%# DataBinder.Eval(Container.DataItem, "Total") %>
            </td>
            <td class="header2">
                100%
            </td>   
        </tr>
        <tr>        
        <td class="post" colspan="3" align="center">
        <asp:Label ID="PollVotesLabel" Visible="false" runat="server" />
         <asp:Label ID="GuestOptionsHidden" Visible="false" runat="server" />
        <asp:Label ID="AlreadyVotedLabel" Visible="false" runat="server" />
        <asp:Label ID="PollWillExpire" Visible="false" runat="server" />
        <asp:Label ID="PollExpired"  Visible="false" runat="server" />
        </td>
        </tr>
        <tr id="PollCommandRow" runat="server">
		<td class="command" width="100%" colspan="3">   
           <YAF:ThemeButton ID="RemovePollAll" runat="server"  Visible="false" CommandName="removeall" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>' CssClass="yafcssbigbutton rightItem"
			  TextLocalizedTag="REMOVEPOLL_ALL" /> 
           <YAF:ThemeButton ID="RemovePoll" runat="server" Visible="false" CommandName="remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>' CssClass="yafcssbigbutton rightItem"
			  TextLocalizedTag="REMOVEPOLL" />
           <YAF:ThemeButton ID="EditPoll" runat="server" Visible='<%# CanEditPoll(DataBinder.Eval(Container.DataItem, "PollID")) %>' CommandName="edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PollID") %>' CssClass="yafcssbigbutton rightItem"
			  TextLocalizedTag="EDITPOLL" />           
           <YAF:ThemeButton ID="CreatePoll" runat="server" Visible='<%# CanCreatePoll() %>' CommandName="new" CssClass="yafcssbigbutton rightItem"
			  TextLocalizedTag="CREATEPOLL" />              
		</td>	
	   </tr>
 </ItemTemplate>
    <FooterTemplate>
        <tr id="PollGroupCommandRow" runat="server">
		<td class="command" width="100%" colspan="3">
        	       <YAF:ThemeButton ID="RemoveGroupAll" runat="server" Visible='<%# CanRemoveGroupCompletely() %>' CommandName="removegroupall"  CssClass="yafcssbigbutton rightItem"
			  TextLocalizedTag="REMOVEPOLLGROUP_ALL" /> 
           <YAF:ThemeButton ID="RemoveGroupEverywhere" runat="server" Visible='<%# CanRemoveGroupEverywhere() %>' CommandName="removegroupevery"  CssClass="yafcssbigbutton rightItem"
			  TextLocalizedTag="REMOVEPOLLGROUP_EVERYWHERE" />
           <YAF:ThemeButton ID="RemoveGroup" runat="server" Visible='<%# CanRemoveGroup() %>' CommandName="removegroup"  CssClass="yafcssbigbutton rightItem"
			  TextLocalizedTag="REMOVEPOLLGROUP" />	
        </td>
        </tr> 
    </FooterTemplate>    
</asp:Repeater>
<tr id="NewPollRow" runat="server" visible="false" >
    <td width="100%" colspan="3">  
    <YAF:ThemeButton ID="CreatePoll1" runat="server" CssClass="yafcssbigbutton rightItem"
			  TextLocalizedTag="CREATEPOLL" />
 </td>
 </tr> 
</table>


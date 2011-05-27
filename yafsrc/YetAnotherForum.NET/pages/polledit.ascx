<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="polledit.ascx.cs" Inherits="YAF.Pages.polledit" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<table align="center" cellpadding="4" cellspacing="1" class="content" 
    width="100%" style="height: 223px" >
    <tr>
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel ID="PollNameLabel" runat="server" LocalizedPage="POLLEDIT" LocalizedTag="POLLHEADER" />
        </td>
    </tr>
<tr id="PollRow1" runat="server" visible="true">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="pollquestion" />
              
			</em>
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="Question" runat="server" CssClass="edit" MaxLength="255" Width="400" />            
		</td>
</tr>
<tr id="PollObjectRow1" runat="server" visible="<%# (PageContext.IsAdmin || PageContext.BoardSettings.AllowUsersImagedPoll) && PageContext.ForumPollAccess %>" >
		<td class="header2" width="20%">
			<em>				
               <YAF:LocalizedLabel ID="PollQuestionObjectLabel"  runat="server" LocalizedTag="POLLIMAGE_TEXT" />
			</em>
		</td>
		<td class="post" width="80%">			
         <asp:TextBox ID="QuestionObjectPath" runat="server" CssClass="edit" MaxLength="255" Width="400" />
		</td>
</tr>
    <asp:Repeater ID="ChoiceRepeater" runat="server" Visible="false" >
        <HeaderTemplate>
              </HeaderTemplate>
                  <ItemTemplate>
                    <tr>
                     <td colspan="2"><hr /></td>
                    </tr>
                    <tr>
                        <td class="postformheader" width="20%">
                                   <em>
                                   <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="choice" Param0='<%# DataBinder.Eval(Container.DataItem, "ChoiceOrderID") %>' />
                                   </em>
                        </td>
                        <td class="post" width="80%">
                             <asp:HiddenField ID="PollChoiceID"  Value='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>' runat="server" />
                             <asp:TextBox ID="PollChoice" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Choice") %>' CssClass="edit" MaxLength="50" Width="400" />
                        </td>
                   </tr>
                   <tr id="ChoiceRow1" visible="<%# (PageContext.IsAdmin || PageContext.BoardSettings.AllowUsersImagedPoll) && PageContext.ForumPollAccess %>" runat="server" >
                     <td class="header2" width="20%">
                          <em>
                          <YAF:LocalizedLabel ID="PollChoiceObjectLabel"  runat="server" LocalizedTag="POLLIMAGE_TEXT" />
                          </em>
                     </td>
                     <td class="post" width="80%">
                     <asp:TextBox ID="ObjectPath" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ObjectPath") %>' CssClass="edit" MaxLength="255" Width="400" />
                     </td>
                  </tr>
</ItemTemplate>
<FooterTemplate>	
</FooterTemplate>
</asp:Repeater>
    <tr>
                     <td colspan="2"><hr /></td>
                    </tr>
    <tr id="tr_AllowMultipleChoices" runat="server" visible="<%# PageContext.BoardSettings.AllowMultipleChoices %>">	
       	<td class="postformheader" width="20%">
			<em>
				 <YAF:LocalizedLabel ID="AllowMultipleChoicesLabel" runat="server" LocalizedTag="POLL_MULTIPLECHOICES" />
			</em>
		</td>
		<td class="post" width="80%">
                 <asp:CheckBox ID="AllowMultipleChoicesCheckBox" runat="server" CssClass="edit" MaxLength="10" Width="400" />					
	    </td>		
	</tr> 
    <tr id="tr_AllowSkipVote" runat="server" visible="<%# PageContext.BoardSettings.AllowMultipleChoices %>">	
       	<td class="postformheader" width="20%">
			<em>
				 <YAF:LocalizedLabel ID="AllowSkipVoteLocalizedLabel" runat="server" LocalizedTag="POLL_MULTIPLECHOICES" />
			</em>
		</td>
		<td class="post" width="80%">
                 <asp:CheckBox ID="AllowSkipVoteCheckBox" runat="server" CssClass="edit" MaxLength="10" Width="400" />					
	    </td>		
	</tr> 
    <tr id="tr_ShowVoters" runat="server" visible="true">	
       	<td class="postformheader" width="20%">
			<em>
				 <YAF:LocalizedLabel ID="ShowVotersLocalizedLabel" runat="server" LocalizedTag="POLL_SHOWVOTERS" />
			</em>
		</td>
		<td class="post" width="80%">
                 <asp:CheckBox ID="ShowVotersCheckBox" runat="server" CssClass="edit" MaxLength="10" Width="400" />					
	    </td>		
	</tr> 
     <tr id="PollRowExpire" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="poll_expire" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="PollExpire" runat="server" CssClass="edit" MaxLength="10" Width="400" />
			<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="poll_expire_explain" />
		</td>
	</tr> 
     <tr id="IsBound" runat="server" visible="false">	
        	<td class="postformheader" width="20%">
			<em>
				 <YAF:LocalizedLabel ID="IsBoundLabel" runat="server" LocalizedTag="POLLGROUP_BOUNDWARN" />
			</em>
		</td>
		<td class="post" width="80%">
                 <asp:CheckBox ID="IsBoundCheckBox" runat="server" CssClass="edit" MaxLength="10" Width="400" />					
		</td>		
	</tr> 
    <tr id="IsClosedBound" runat="server" visible="false">
    	<td class="postformheader" width="20%">
			<em>
				 <YAF:LocalizedLabel ID="IsClosedBoundLabel" runat="server" LocalizedTag="pollgroup_closedbound" />&nbsp;:&nbsp;
                 <YAF:LocalizedLabel ID="IsClosedBoundExplainLabel" runat="server" LocalizedTag="POLLGROUP_CLOSEDBOUND_WARN" /> 
			</em>
		</td>
		<td class="post" width="80%">
                 <asp:CheckBox ID="IsClosedBoundCheckBox"  runat="server"  CssClass="edit" MaxLength="10" Width="400" />					
		</td>	
	</tr> 
    <tr id="PollGroupList" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel ID="PollGroupListLabel" runat="server" LocalizedTag="pollgroup_list" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:DropDownList ID="PollGroupListDropDown" runat="server" CssClass="edit" MaxLength="10" Width="400" />			
		</td>
	</tr>
    <tr>
    <td class="postformheader" colspan="2" width="100%">
           <YAF:ThemeButton ID="SavePoll" runat="server" CssClass="yafcssbigbutton leftItem"
			 OnClick="SavePoll_Click"  TextLocalizedTag="POLLSAVE" />
            <YAF:ThemeButton ID="Cancel" runat="server" CssClass="yafcssbigbutton leftItem"
			 OnClick="Cancel_Click" TextLocalizedTag="CANCEL" />
              </td>
</tr>
    </table>    

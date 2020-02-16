<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PollEdit.ascx.cs" Inherits="YAF.Pages.PollEdit" %>

<YAF:PageLinks ID="PageLinks" runat="server" />

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-poll-h fa-fw"></i>&nbsp; <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                                                   LocalizedPage="POLLEDIT" 
                                                                                   LocalizedTag="POLLHEADER" />
            </div>
            <div class="card-body">
                <asp:PlaceHolder id="PollRow1" runat="server" visible="true">
		            <div class="form-group">
			            <asp:Label runat="server" AssociatedControlID="Question">
				            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="pollquestion" />
			            </asp:Label>
			            <asp:TextBox ID="Question" runat="server" CssClass="form-control" MaxLength="255" />            
		            </div>
                </asp:PlaceHolder>
               <asp:PlaceHolder id="PollObjectRow1" runat="server"
                                visible="<%# (this.PageContext.IsAdmin || this.PageContext.BoardSettings.AllowUsersImagedPoll) && this.PageContext.ForumPollAccess %>">
                   <div class="form-group">
			           <asp:Label runat="server" AssociatedControlID="QuestionObjectPath">				
                           <YAF:LocalizedLabel ID="PollQuestionObjectLabel"  runat="server" LocalizedTag="POLLIMAGE_TEXT" />
			           </asp:Label>
                       <asp:TextBox ID="QuestionObjectPath" runat="server" CssClass="form-control" MaxLength="255" />
		           </div>
               </asp:PlaceHolder>
                <asp:Repeater ID="ChoiceRepeater" runat="server" Visible="false" >
                    <ItemTemplate>
                        <div class="form-row">
                        <div class="form-group col-md-6">
                            <asp:Label runat="server" AssociatedControlID="PollChoice">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                    LocalizedTag="choice" 
                                                    Param0='<%# DataBinder.Eval(Container.DataItem, "ChoiceOrderID") %>' />
                            </asp:Label>
                            <asp:HiddenField ID="PollChoiceID"  Value='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>' runat="server" />
                            <asp:TextBox ID="PollChoice" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Choice") %>' 
                                         CssClass="form-control" 
                                         MaxLength="50" />
                        </div>
                        <asp:PlaceHolder id="ChoiceRow1" 
                                         visible="<%# (this.PageContext.IsAdmin || this.PageContext.BoardSettings.AllowUsersImagedPoll) && this.PageContext.ForumPollAccess %>" runat="server" >
                            <div class="form-group col-md-6">
                                <asp:Label runat="server" AssociatedControlID="ObjectPath">
                                    <YAF:LocalizedLabel ID="PollChoiceObjectLabel"  runat="server" LocalizedTag="POLLIMAGE_TEXT" />
                                </asp:Label>
                                <asp:TextBox ID="ObjectPath" runat="server" 
                                             Text='<%# DataBinder.Eval(Container.DataItem, "ObjectPath") %>' 
                                             CssClass="form-control" MaxLength="255" />
                            </div>
                        </asp:PlaceHolder>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder id="tr_AllowMultipleChoices" runat="server" visible="<%# this.PageContext.BoardSettings.AllowMultipleChoices %>">
                    <div class="form-group">
                        <asp:CheckBox ID="AllowMultipleChoicesCheckBox" runat="server" 
                                      CssClass="custom-control custom-checkbox" />
                    </div>
                </asp:PlaceHolder> 
                <asp:PlaceHolder id="tr_AllowSkipVote" runat="server" visible="<%# this.PageContext.BoardSettings.AllowMultipleChoices %>">	
                    <div class="form-group">
                        <asp:CheckBox ID="AllowSkipVoteCheckBox" runat="server" 
                                      CssClass="custom-control custom-checkbox" />
                    </div>
                </asp:PlaceHolder> 
                <asp:PlaceHolder id="tr_ShowVoters" runat="server" visible="true">	
                    <div class="form-group">
                        <asp:CheckBox ID="ShowVotersCheckBox" runat="server" 
                                      CssClass="custom-control custom-checkbox" />
                    </div>
                </asp:PlaceHolder> 
                <asp:PlaceHolder id="PollRowExpire" runat="server" visible="false">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="PollExpire">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="poll_expire" />
                        </asp:Label>
                        <asp:TextBox ID="PollExpire" runat="server" TextMode="Number" MaxLength="10" />
                        <small class="form-text text-muted">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="poll_expire_explain" />
                        </small>
                    </div>
                </asp:PlaceHolder> 
                <asp:PlaceHolder id="IsBound" runat="server" visible="false">	
                    <div class="form-group">
                        <asp:CheckBox ID="IsBoundCheckBox" runat="server" 
                                      CssClass="custom-control custom-checkbox" />
                    </div>		
                </asp:PlaceHolder> 
                <asp:PlaceHolder id="IsClosedBound" runat="server" visible="false">
                    <div class="form-group">
                        <asp:CheckBox ID="IsClosedBoundCheckBox"  runat="server" 
                                      CssClass="custom-control custom-checkbox" />
                        <small class="form-text text-muted">
                            <YAF:LocalizedLabel ID="IsClosedBoundExplainLabel" runat="server" LocalizedTag="POLLGROUP_CLOSEDBOUND_WARN" /> 
                        </small>
                    </div>	
                </asp:PlaceHolder> 
                <asp:PlaceHolder id="PollGroupList" runat="server" visible="false">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="PollGroupListDropDown">
				            <YAF:LocalizedLabel ID="PollGroupListLabel" runat="server" LocalizedTag="pollgroup_list" />
			            </asp:Label>
                        <asp:DropDownList ID="PollGroupListDropDown" runat="server" CssClass="select2-select" MaxLength="10" />			
		            </div>
                </asp:PlaceHolder>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="SavePoll" runat="server"
                                 OnClick="SavePoll_Click"  TextLocalizedTag="POLLSAVE"
                                 Icon="save"/>
                <YAF:ThemeButton ID="Cancel" runat="server"
                                 OnClick="Cancel_Click" TextLocalizedTag="CANCEL"
                                 Icon="times"
                                 Type="Secondary"/>
            </div>
        </div>
    </div>
</div>
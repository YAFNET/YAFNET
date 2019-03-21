<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.team" Codebehind="team.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col">
        <h2><YAF:LocalizedLabel runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>
<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-user-shield fa-fw"></i>&nbsp;<YAF:LocalizedLabel 
                                                                 ID="LocalizedLabel2"
                                                                 runat="server" 
                                                                 LocalizedTag="Admins" LocalizedPage="TEAM" />
            </div>
            <div class="card-body">
                <YAF:Alert runat="server" ID="Alert2" Type="info" MobileOnly="True">
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                        LocalizedTag="TABLE_RESPONSIVE" 
                                        LocalizedPage="ADMIN_COMMON" />
                    <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                </YAF:Alert>
                <div class="table-responsive">
                <asp:DataGrid ID="AdminsGrid" runat="server"  
                              Width="100%" BorderStyle="None" GridLines="None" BorderWidth="0px" 
                              AutoGenerateColumns="False"
                              CssClass="table">
    		         <Columns>
             		 	<asp:TemplateColumn HeaderText="User">
			  			    <HeaderStyle HorizontalAlign="Center" CssClass="font-weight-bold"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" Width="40px"></ItemStyle>
			  				    <ItemTemplate>
                                    <asp:Image ID="AdminAvatar" runat="server"
                                               Width="40px" Height="40px" />
			  				    </ItemTemplate>
             		 	</asp:TemplateColumn>
             		 	<asp:TemplateColumn>
             		 	    <HeaderStyle HorizontalAlign="Center" CssClass="font-weight-bold"></HeaderStyle>
			  			    <ItemStyle Width="150px"></ItemStyle>
			  				    <ItemTemplate>
               				        <YAF:UserLink ID="AdminLink" runat="server" 
                                                     IsGuest="False" 
                                                     UserID='<%# this.Eval("UserID").ToType<int>() %>' 
                                                     Style='<%# this.Eval("Style") %>'  />
			  				    </ItemTemplate>
             		 	</asp:TemplateColumn>
             		 	<asp:TemplateColumn HeaderText="Forums">
			  			    <HeaderStyle CssClass="font-weight-bold"></HeaderStyle>
			  			    <ItemStyle Width="390px"></ItemStyle>
			  				    <ItemTemplate>
               				        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="FORUMS_ALL" LocalizedPage="TEAM" />
			  				    </ItemTemplate>
             		 	</asp:TemplateColumn>
                         <asp:TemplateColumn>
			  			    <HeaderStyle CssClass="font-weight-bold"></HeaderStyle>
			  			    <ItemStyle HorizontalAlign="Left"></ItemStyle>
			  				    <ItemTemplate>
                                    <div class="btn-group" role="group">
               				        <YAF:ThemeButton ID="PM" runat="server"
                                                        Size="Small" Visible="false" 
                                                        TextLocalizedPage="POSTS" TextLocalizedTag="PM"
                                                        TitleLocalizedPage="POSTS" TitleLocalizedTag="PM_TITLE"
                                                        Icon="envelope" Type="Secondary"/>
			                        <YAF:ThemeButton ID="Email" runat="server" 
                                                     Size="Small" Visible="false"
                                                     TextLocalizedPage="POSTS" TextLocalizedTag="EMAIL"
                                                     TitleLocalizedPage="POSTS" TitleLocalizedTag="EMAIL_TITLE"
                                                     Icon="at" Type="Secondary" />
			                        <YAF:ThemeButton ID="AdminUserButton" runat="server" 
                                                     TitleLocalizedPage="PROFILE" TitleLocalizedTag="ADMIN_USER"
                                                     TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE" 
                                                     Size="Small" Visible="false"
                                                     Icon="user-cog" Type="Secondary"
                                                     NavigateUrl='<%# YafBuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", this.Eval("UserID").ToType<int>() ) %>'>
			                        </YAF:ThemeButton>
                                    </div>
			  				    </ItemTemplate>
             		 	</asp:TemplateColumn>
    		         </Columns>
			</asp:DataGrid>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="row" id="ModsTable" runat="server">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-user-secret fa-fw"></i>&nbsp;<YAF:LocalizedLabel 
                                                                 ID="LocalizedLabel1" 
                                                                 runat="server" 
                                                                 LocalizedTag="MODS" LocalizedPage="TEAM" />
            </div>
            <div class="card-body">
                <YAF:Alert runat="server" ID="Alert1" Type="info" MobileOnly="True">
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server"
                                        LocalizedTag="TABLE_RESPONSIVE" 
                                        LocalizedPage="ADMIN_COMMON" />
                    <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                </YAF:Alert>
                <div class="table-responsive">
                <asp:DataGrid ID="ModeratorsGrid" runat="server" 
                              Width="100%" BorderStyle="None" GridLines="None" BorderWidth="0px" 
                              CssClass="table"
                              AutoGenerateColumns="False">
    		         <Columns>
             		 	<asp:TemplateColumn HeaderText="User">
			  			    <HeaderStyle HorizontalAlign="Center" CssClass="font-weight-bold"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" Width="40px"></ItemStyle>
			  				    <ItemTemplate>
                                    <asp:Image ID="ModAvatar" runat="server"
                                               Width="40px" Height="40px" />
			  				    </ItemTemplate>
             		 	</asp:TemplateColumn>
             		 	<asp:TemplateColumn>
             		 	    <HeaderStyle HorizontalAlign="Center" CssClass="font-weight-bold"></HeaderStyle>
			  			    <ItemStyle Width="150px"></ItemStyle>
			  				    <ItemTemplate>
               				        <YAF:UserLink ID="ModLink" runat="server" 
                                                     ReplaceName='<%# this.Get<YafBoardSettings>().EnableDisplayName ? this.Eval("DisplayName").ToString() : this.Eval("Name").ToString() %>' UserID='<%# this.Eval("ModeratorID").ToType<int>() %>' IsGuest="False" Style='<%# this.Eval("Style") %>'  />
			  				    </ItemTemplate>
             		 	</asp:TemplateColumn>
             		 	<asp:TemplateColumn HeaderText="Forums">
			  			    <HeaderStyle CssClass="font-weight-bold"></HeaderStyle>
			  			    <ItemStyle Width="390px"></ItemStyle>
			  				    <ItemTemplate>
               				        <asp:DropDownList ID="ModForums" runat="server" CssClass="standardSelectMenu">
               				        </asp:DropDownList>
                                     <YAF:ThemeButton ID="GoToForumButton" runat="server" 
                                                      Size="Small"
                                                      Icon="external-link-alt" Type="Secondary"
                                                      TextLocalizedTag="GO" OnClick="GoToForum"></YAF:ThemeButton>
			  				    </ItemTemplate>
             		 	</asp:TemplateColumn>
                        <asp:TemplateColumn>
			  			    <HeaderStyle CssClass="font-weight-bold"></HeaderStyle>
			  			    <ItemStyle HorizontalAlign="Left"></ItemStyle>
			  				    <ItemTemplate>
                                    <div class="btn-group" role="group">
               				        <YAF:ThemeButton ID="PM" runat="server" 
                                                        Size="Small" Visible="false" 
                                                        TextLocalizedPage="POSTS" TextLocalizedTag="PM" 
                                                        TitleLocalizedPage="POSTS" TitleLocalizedTag="PM_TITLE"
                                                        Icon="envelope" Type="Secondary" />
			                        <YAF:ThemeButton ID="Email" runat="server" 
                                                     Size="Small" Visible="false" 
                                                     TextLocalizedPage="POSTS" TextLocalizedTag="EMAIL"
                                                     TitleLocalizedPage="POSTS" TitleLocalizedTag="EMAIL_TITLE"
                                                     Icon="at" Type="Secondary" />
			                        <YAF:ThemeButton ID="AdminUserButton" runat="server" 
                                                     Size="Small" Visible="false"
                                                     TitleLocalizedPage="PROFILE" TitleLocalizedTag="ADMIN_USER"
                                                     TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE"
                                                     Icon="user-cog" Type="Secondary"
                                                     NavigateUrl='<%# YafBuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", this.Eval("ModeratorID").ToType<int>() ) %>'>
			                        </YAF:ThemeButton>
                                    </div>
			  				    </ItemTemplate>
             		 	</asp:TemplateColumn>
    		         </Columns>
			</asp:DataGrid>
                    </div>
            </div>
        </div>
    </div>
</div>
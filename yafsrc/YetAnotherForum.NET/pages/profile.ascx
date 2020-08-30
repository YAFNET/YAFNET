<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Profile" Codebehind="Profile.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="System.Data" %>

<%@ Register TagPrefix="YAF" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="YAF" TagName="SuspendUser" Src="../controls/EditUsersSuspend.ascx" %>
<%@ Register TagPrefix="YAF" TagName="BuddyList" Src="../controls/BuddyList.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />


    <div class="row">
        <div class="col-md-10">
             <h1 class="">
                 <YAF:UserLabel ID="UserLabel1" runat="server" />
             </h1>
            <YAF:ThemeButton ID="lnkBuddy" runat="server" 
                             OnCommand="lnk_AddBuddy"/>
            <YAF:ThemeButton ID="PM" runat="server" Visible="false" 
                             TextLocalizedPage="POSTS" TextLocalizedTag="PM" 
                             TitleLocalizedTag="PM_TITLE" TitleLocalizedPage="POSTS"
                             Icon="envelope-open-text"
                             Type="Info"/>
            <YAF:ThemeButton ID="Email" runat="server" Visible="false" 
                             TextLocalizedPage="POSTS" TextLocalizedTag="EMAIL" 
                             TitleLocalizedTag="EMAIL_TITLE" TitleLocalizedPage="POSTS"
                             Icon="at"
                             Type="Info"/>
            <YAF:ThemeButton ID="AdminUserButton" runat="server" Visible="false"
                             TextLocalizedTag="ADMIN_USER" 
                             NavigateUrl='<%# BuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", this.UserId ) %>'
                             Icon="user-cog"
                             Type="Danger">
            </YAF:ThemeButton>
        </div>
        <div class="col-md-2">
            <asp:Image ID="Avatar" runat="server" 
                       CssClass="img-fluid float-right rounded" 
                       AlternateText="avatar" />
        </div>
    </div>
    <br>
    <div class="row">
        <div class="col-md-3">
            <!--left col-->
            <ul class="list-group mb-3">
                <li class="list-group-item text-muted">
                    <YAF:LocalizedLabel runat="server" LocalizedTag="profile" />
                </li>
                <li class="list-group-item text-right">
                    <span class="float-left font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="joined" />
                    </span>
                    <asp:Label ID="Joined" runat="server" />
                </li>
                <li class="list-group-item text-right">
                    <span class="float-left font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="lastvisit" />
                    </span>
                    <asp:Label ID="LastVisit" runat="server" Visible="false" />
                    <YAF:DisplayDateTime id="LastVisitDateTime" runat="server" Visible="false"></YAF:DisplayDateTime>
                </li>
                <asp:PlaceHolder runat="server" id="RealNameTR" visible="false">
                <li class="list-group-item text-right">
                    <span class="float-left font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="realname" />
                    </span> 
                    <asp:Literal runat="server" ID="RealName"></asp:Literal>
                </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" id="userGroupsRow" visible="false">
                    <li class="list-group-item text-right">
                    <span class="float-left font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="groups" />
                    </span>
                    <asp:Repeater ID="Groups" runat="server">
                        <ItemTemplate>
                            <%# Container.DataItem %>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            ,
                        </SeparatorTemplate>
                    </asp:Repeater>
                </li>
                </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" id="RankTR" visible="false">
                        <li class="list-group-item text-right">
                        <span class="float-left font-weight-bold">
									<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="rank" />
                        </span> 
									<asp:Label ID="Rank" runat="server" />
                        </li>
							</asp:PlaceHolder>
							
                            <asp:PlaceHolder runat="server" id="CountryTR" visible="false">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
								<YAF:LocalizedLabel ID="LocalizedLabel27" runat="server" LocalizedTag="country" />
                                </span> 
									<asp:Label ID="CountryLabel" runat="server" />
                                </li>
							</asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" id="RegionTR" visible="false">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
									<YAF:LocalizedLabel ID="LocalizedLabel28" runat="server" LocalizedTag="region" />
                                </span> 
									<asp:Label ID="RegionLabel" runat="server" />
                                </li>
							</asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" id="CityTR" visible="false">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
									<YAF:LocalizedLabel ID="LocalizedLabel26" runat="server" LocalizedTag="city" />
                                </span> 
									<asp:Label ID="CityLabel" runat="server" />
                                </li>
							</asp:PlaceHolder>
							<asp:PlaceHolder runat="server" id="LocationTR" visible="false">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
									<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="location" />
                                </span> 
									<asp:Label ID="Location" runat="server" />
								</li>
							</asp:PlaceHolder>
							<asp:PlaceHolder runat="server" id="BirthdayTR" visible="false">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
									<YAF:LocalizedLabel ID="LocalizedLabel23" runat="server" LocalizedTag="BIRTHDAY" />
                                </span> 
									<asp:Label ID="Birthday" runat="server" />
                                </li>
							</asp:PlaceHolder>
							<asp:PlaceHolder  runat="server" id="OccupationTR" visible="false">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
									<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="occupation" />
                                </span>
								<asp:Literal runat="server" id="Occupation" />
                                </li>
							</asp:PlaceHolder>
							
							<asp:PlaceHolder runat="server" id="GenderTR" visible="false">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
									<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="gender" />
                                </span> 
								<asp:Literal runat="server" id="Gender" />
                                </li>
							</asp:PlaceHolder>
            </ul>
            <asp:PlaceHolder runat="server" ID="HomePlaceHolder">
            <div class="card mb-3">
                <div class="card-header"><YAF:LocalizedLabel runat="server" LocalizedTag="HOME"></YAF:LocalizedLabel>
                </div>
                <div class="card-body">
                    <asp:HyperLink runat="server" ID="Home">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="HOME" />
                    </asp:HyperLink>
                </div>
            </div>
            </asp:PlaceHolder>
            <ul class="list-group mb-3">
                <li class="list-group-item text-muted">
                    <YAF:LocalizedLabel ID="LocalizedLabel41" runat="server" LocalizedTag="STATISTICS" />
                </li>
                <li class="list-group-item text-right">
                     <span class="float-left font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="numposts" />
								</span>
								<asp:Literal runat="server" id="Stats" />
							</li>

							<asp:PlaceHolder id="divTF" runat="server" visible="<%# this.Get<BoardSettings>().EnableThanksMod %>">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
                                <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="THANKSFROM" />
                                </span>
									<asp:Label ID="ThanksFrom" runat="server" />
								</li>
							</asp:PlaceHolder>
							<asp:PlaceHolder id="divTTT" runat="server" visible="<%# this.Get<BoardSettings>().EnableThanksMod %>">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
									<YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedTag="THANKSTOTIMES" />
                                </span>
									<asp:Label ID="ThanksToTimes" runat="server" />
								</li>
							</asp:PlaceHolder>
							<asp:PlaceHolder id="divTTP" runat="server" visible="<%# this.Get<BoardSettings>().EnableThanksMod %>">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
									<YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedTag="THANKSTOPOSTS" />
                                </span>
									<asp:Label ID="ThanksToPosts" runat="server" />
								</li>
							</asp:PlaceHolder>
                            <asp:PlaceHolder id="divRR" runat="server" visible="<%# this.Get<BoardSettings>().EnableUserReputation %>">
                                <li class="list-group-item">
                                    <span class="font-weight-bold">
                                        <YAF:LocalizedLabel ID="LocalizedLabel29" runat="server" LocalizedTag="REPUTATION_RECEIVED" />
                                    </span>
									<asp:Literal ID="ReputationReceived" runat="server" />
								</li>
							</asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" id="MedalsRow" Visible="False">
                                <li class="list-group-item text-right">
                                <span class="float-left font-weight-bold">
									<YAF:LocalizedLabel ID="LocalizedLabel30" runat="server" LocalizedTag="MEDALS" />
                                </span>
									<asp:Label ID="MedalsPlaceHolder" runat="server" />
								</li>
                            </asp:PlaceHolder>
            </ul>
            <asp:PlaceHolder runat="server" ID="SocialMediaHolder">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:LocalizedLabel runat="server"
                                        LocalizedTag="SOCIAL_MEDIA"></YAF:LocalizedLabel>
                </div>
                <div class="card-body">
                    <YAF:ThemeButton ID="Blog" runat="server" Visible="false" 
                                     TextLocalizedPage="POSTS" TextLocalizedTag="BLOG" 
                                     TitleLocalizedTag="BLOG_TITLE" TitleLocalizedPage="POSTS"
                                     Type="None" Icon="blog"/>
                    <YAF:ThemeButton ID="XMPP" runat="server" Visible="false" 
                                     TextLocalizedPage="POSTS" TextLocalizedTag="XMPP" 
                                     TitleLocalizedTag="XMPP_TITLE" TitleLocalizedPage="POSTS"
                                     Type="None"/>
                    <YAF:ThemeButton ID="Skype" runat="server" Visible="false" 
                                     TitleLocalizedTag="SKYPE_TITLE" TitleLocalizedPage="POSTS"
                                     Type="None" Icon="skype" CssClass="fab"/>
                    <YAF:ThemeButton ID="Facebook" runat="server" Visible="false" 
                                     TitleLocalizedTag="FACEBOOK_TITLE" TitleLocalizedPage="POSTS"
                                     Type="None" Icon="facebook" IconCssClass="fab"/>
                    <YAF:ThemeButton ID="Twitter" runat="server" Visible="false" 
                                     TitleLocalizedTag="TWITTER_TITLE" TitleLocalizedPage="POSTS"
                                     Type="None" Icon="twitter" IconCssClass="fab"/>
                    
                </div>
            </div>
            </asp:PlaceHolder>
        </div>
        <!--/col-3-->
        <div class="col-md-9" contenteditable="false" style="">
            <asp:PlaceHolder runat="server" id="InterestsTR" visible="false">
            
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="interests" />
                </div>
                <div class="card-body">
                    <asp:Literal runat="server" id="Interests" />
                </div>
            </div>
            </asp:PlaceHolder>
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:LocalizedLabel ID="LocalizedLabel43" runat="server" LocalizedTag="LAST10" />
                </div>
                <div class="card-body">
                    <YAF:ThemeButton ID="SearchUser" runat="server"
                                     TextLocalizedPage="POSTS" TextLocalizedTag="SEARCHUSER"
                                     Icon="search" Type="Secondary" CssClass="mb-3"/>
                   
                        <asp:Repeater ID="LastPosts" runat="server">
                            <ItemTemplate>
                                <div class="card mb-3">
                                <div class="card-header">
                                    <i class="fa fa-comment fa-fw"></i>&nbsp;<span class="font-weight-bold">
                                        <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="topic" />
                                    </span><a 
                                               title='<%# this.GetText("COMMON", "VIEW_TOPIC") %>' 
                                               href='<%# BuildLink.GetLink(ForumPages.Posts,"t={0}",Container.DataItemToField<int>("TopicID")) %>'>
                                        <%# this.Get<IBadWordReplace>().Replace(this.HtmlEncode(Container.DataItemToField<string>("Subject"))) %>
                                    </a>
                                </div>
                                    <div class="card-body">
                                        <YAF:MessagePostData ID="MessagePost" runat="server" ShowAttachments="false" DataRow="<%# (DataRow)Container.DataItem %>">
                                        </YAF:MessagePostData>
                                    </div>
                                    <div class="card-footer">
                                        <small class="text-muted">
                                            <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="posted" />&nbsp;
                                            <%# this.Get<IDateTime>().FormatDateTime(Container.DataItemToField<DateTime>("Posted"))%>
                                        </small>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                </div>
            </div>
            <div runat="server" id="ModerateTab" class="card mb-3">
                <div class="card-header">
                    <YAF:LocalizedLabel ID="LocalizedLabel46" runat="server" LocalizedTag="MODERATION" />
                </div>
                <div class="card-body">
                    <nav>
                        <div class="nav nav-tabs" id="nav-tab" role="tablist">
                            <a class="nav-item nav-link" 
                               id="nav-suspend-tab" 
                               data-toggle="tab" 
                               href="#nav-suspend" 
                               role="tab">
                                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_USER" />
                            </a>
                            <a class="nav-item nav-link" 
                               id="nav-signature-tab" 
                               data-toggle="tab" 
                               href="#nav-signature" 
                               role="tab">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="PROFILE" LocalizedTag="SIGNATURE" />
                            </a>
                        </div>
                    </nav>
                    <div class="tab-content" id="nav-tabContent">
                        <div class="tab-pane fade show active" id="nav-suspend" role="tabpanel">
                            <YAF:SuspendUser runat="server" ID="SuspendUserControl" ShowHeader="False" />
                        </div>
                        <div class="tab-pane fade" id="nav-signature" role="tabpanel">
                            <YAF:SignatureEdit runat="server" ID="SignatureEditControl" ShowHeader="False" />
                        </div>
                    </div>
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="BuddyCard">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:LocalizedLabel ID="LocalizedLabel44" runat="server" 
                                            LocalizedTag='<%# this.UserId == BoardContext.Current.PageUserID ? "BUDDIES" : "BUDDIESTITLE"%>' />
                    </div>
                    <div class="card-body">
                        <div runat="server" id="BuddyListTab" class="tab-pane" role="tabpanel">
                            <YAF:BuddyList runat="server" ID="BuddyList" />
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
            
        </div>
    </div>
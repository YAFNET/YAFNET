﻿<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.UserProfile" CodeBehind="UserProfile.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>
<%@ Import Namespace="YAF.Types.Models" %>

<%@ Register TagPrefix="YAF" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="YAF" TagName="SuspendUser" Src="../controls/EditUsersSuspend.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row justify-content-between">
    <div class="col-auto">
        <h1>
            <YAF:UserLabel ID="UserLabel1" runat="server" />
        </h1>
        <YAF:ThemeButton ID="lnkBuddy" runat="server"
                         OnCommand="lnk_AddBuddy"
                         CssClass="mb-1"/>
        <YAF:ThemeButton ID="PM" runat="server"
                         Visible="false"
                         TextLocalizedPage="POSTS" TextLocalizedTag="PM"
                         TitleLocalizedTag="PM_TITLE" TitleLocalizedPage="POSTS"
                         CssClass="mb-1"
                         Icon="envelope-open-text"
                         Type="Info" />
        <YAF:ThemeButton ID="Email" runat="server"
                         Visible="false"
                         TextLocalizedPage="POSTS" TextLocalizedTag="EMAIL"
                         TitleLocalizedTag="EMAIL_TITLE" TitleLocalizedPage="POSTS"
                         CssClass="mb-1"
                         Icon="at"
                         Type="Info" />
        <YAF:ThemeButton ID="AdminUserButton" runat="server"
                         Visible="false"
                         TextLocalizedTag="ADMIN_USER"
                         NavigateUrl='<%# this.Get<LinkBuilder>().GetLink(ForumPages.Admin_EditUser,new { u = this.UserId }) %>'
                         CssClass="mb-1"
                         Icon="user-cog"
                         Type="Danger"/>

    </div>
    <div class="col-auto">
        <asp:Image ID="Avatar" runat="server"
                   CssClass="img-fluid rounded"
                   AlternateText="avatar" />
    </div>
</div>
<br>
<div class="row">
    <div class="col-md-3">
        <!--left col-->
        <ul class="list-group mb-3">
            <li class="list-group-item link-light bg-primary">
                <YAF:LocalizedLabel runat="server" LocalizedTag="profile" />
            </li>
            <li class="list-group-item text-end">
                <span class="float-start fw-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="joined" />
                </span>
                <asp:Label ID="Joined" runat="server" />
            </li>
            <li class="list-group-item text-end">
                <span class="float-start fw-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="lastvisit" />
                </span>
                <asp:Label ID="LastVisit" runat="server" Visible="false" />
                <YAF:DisplayDateTime ID="LastVisitDateTime" runat="server" Visible="false"></YAF:DisplayDateTime>
            </li>
            <asp:PlaceHolder runat="server" ID="RealNameTR" Visible="false">
                <li class="list-group-item text-end">
                    <span class="float-start fw-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="realname" />
                    </span>
                    <asp:Literal runat="server" ID="RealName"></asp:Literal>
                </li>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="userGroupsRow" Visible="false">
                <li class="list-group-item text-end">
                    <span class="float-start fw-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="groups" />
                    </span>
                    <asp:Repeater ID="Groups" runat="server">
                        <ItemTemplate>
                            <%# this.Eval("Name") %>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            ,
                        </SeparatorTemplate>
                    </asp:Repeater>
                </li>
            </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="RankTR" Visible="false">
                    <li class="list-group-item text-end">
                        <span class="float-start fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="rank" />
                        </span>
                        <asp:Label ID="Rank" runat="server" />
                    </li>
                </asp:PlaceHolder>

                <asp:PlaceHolder runat="server" ID="CountryTR" Visible="false">
                    <li class="list-group-item text-end">
                        <span class="float-start fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel27" runat="server" LocalizedTag="country" />
                        </span>
                        <asp:Label ID="CountryLabel" runat="server" />
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="RegionTR" Visible="false">
                    <li class="list-group-item text-end">
                        <span class="float-start fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel28" runat="server" LocalizedTag="region" />
                        </span>
                        <asp:Label ID="RegionLabel" runat="server" />
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="CityTR" Visible="false">
                    <li class="list-group-item text-end">
                        <span class="float-start fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel26" runat="server" LocalizedTag="city" />
                        </span>
                        <asp:Label ID="CityLabel" runat="server" />
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="LocationTR" Visible="false">
                    <li class="list-group-item text-end">
                        <span class="float-start fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="location" />
                        </span>
                        <asp:Label ID="Location" runat="server" />
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="BirthdayTR" Visible="false">
                    <li class="list-group-item text-end">
                        <span class="float-start fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel23" runat="server" LocalizedTag="BIRTHDAY" />:
                        </span>
                        <asp:Label ID="Birthday" runat="server" />
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="OccupationTR" Visible="false">
                    <li class="list-group-item text-end">
                        <span class="float-start fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="occupation" />
                        </span>
                        <asp:Literal runat="server" ID="Occupation" />
                    </li>
                </asp:PlaceHolder>

                <asp:PlaceHolder runat="server" ID="GenderTR" Visible="false">
                    <li class="list-group-item text-end">
                        <span class="float-start fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="gender" />
                        </span>
                        <asp:Literal runat="server" ID="Gender" />
                    </li>
                </asp:PlaceHolder>

                <asp:Repeater runat="server" ID="CustomProfile">
                    <ItemTemplate>
                        <li class="list-group-item text-end">
                            <span class="float-start fw-bold">
                                <%# this.Eval("Item2.Name") %>:
                            </span>
                            <%# this.HtmlEncode(this.Eval("Item1.Value")) %>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
        </ul>
        <asp:PlaceHolder runat="server" ID="HomePlaceHolder">
            <div class="card mb-3">
                <div class="card-header link-light bg-primary">
                    <YAF:LocalizedLabel runat="server" LocalizedTag="HOME"></YAF:LocalizedLabel>
                </div>
                <div class="card-body">
                    <asp:HyperLink runat="server" ID="Home">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="HOME" />
                    </asp:HyperLink>
                </div>
            </div>
        </asp:PlaceHolder>
        <ul class="list-group mb-3">
            <li class="list-group-item link-light bg-primary">
                <YAF:LocalizedLabel ID="LocalizedLabel41" runat="server" LocalizedTag="STATISTICS" />
            </li>
            <li class="list-group-item d-flex justify-content-between align-items-end">
                <span class="fw-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="numposts" />
                </span>
                <span class="ms-1 text-end"><asp:Literal runat="server" ID="Stats" /></span>
            </li>
            <li class="list-group-item d-flex justify-content-between align-items-end">
                <span class="fw-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="THANKSFROM" />
                </span>
                <span class="ms-1"><asp:Label ID="ThanksFrom" runat="server" /></span>
            </li>
            <li class="list-group-item d-flex justify-content-between align-items-end">
                <span class="fw-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedTag="THANKSTOTIMES" />
                </span>
                <span class="ms-1"><asp:Label ID="ThanksToTimes" runat="server" /></span>
            </li>
            <li class="list-group-item d-flex justify-content-between align-items-end">
                <span class="fw-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedTag="THANKSTOPOSTS" />
                </span>
                <span class="ms-1"><asp:Label ID="ThanksToPosts" runat="server" /></span>
            </li>
            <asp:PlaceHolder ID="divRR" runat="server" Visible="<%# this.PageBoardContext.BoardSettings.EnableUserReputation %>">
                <li class="list-group-item">
                    <span class="fw-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel29" runat="server" LocalizedTag="REPUTATION_RECEIVED" />
                    </span>
                    <asp:Literal ID="ReputationReceived" runat="server" />
                </li>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="MedalsRow" Visible="False">
                <li class="list-group-item d-flex justify-content-between align-items-end">
                    <span class="fw-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel30" runat="server" LocalizedTag="MEDALS" />
                    </span>
                    <span class="ms-1"><asp:Label ID="MedalsPlaceHolder" runat="server" /></span>
                </li>
            </asp:PlaceHolder>
        </ul>
        <asp:PlaceHolder runat="server" ID="SocialMediaHolder">
            <div class="card mb-3">
                <div class="card-header link-light bg-primary">
                    <YAF:LocalizedLabel runat="server"
                        LocalizedTag="SOCIAL_MEDIA">
                    </YAF:LocalizedLabel>
                </div>
                <div class="card-body">
                    <YAF:ThemeButton ID="Blog" runat="server" Visible="false"
                        TextLocalizedPage="POSTS" TextLocalizedTag="BLOG"
                        TitleLocalizedTag="BLOG_TITLE" TitleLocalizedPage="POSTS"
                        Type="None" Icon="blog" />
                    <YAF:ThemeButton ID="XMPP" runat="server" Visible="false"
                        TextLocalizedPage="POSTS" TextLocalizedTag="XMPP"
                        TitleLocalizedTag="XMPP_TITLE" TitleLocalizedPage="POSTS"
                        Type="None" />
                    <YAF:ThemeButton ID="Facebook" runat="server" Visible="false"
                        TitleLocalizedTag="FACEBOOK_TITLE" TitleLocalizedPage="POSTS"
                        Type="None" Icon="facebook" IconCssClass="fab" />

                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <!--/col-3-->
    <div class="col-md-9">
        <asp:PlaceHolder runat="server" ID="InterestsTR" Visible="false">

            <div class="card mb-3">
                <div class="card-header">
                    <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="interests" />
                </div>
                <div class="card-body">
                    <asp:Literal runat="server" ID="Interests" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" id="LastPostsHolder">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:LocalizedLabel ID="LocalizedLabel43" runat="server" LocalizedTag="LAST10" />
                </div>
                <div class="card-body">
                    <YAF:ThemeButton ID="SearchUser" runat="server"
                                     TextLocalizedPage="POSTS" TextLocalizedTag="SEARCHUSER"
                                     Icon="search" Type="Secondary" CssClass="mb-3" />
                    <asp:Repeater ID="LastPosts" runat="server">
                    <ItemTemplate>
                        <div class="card mb-3">
                            <div class="card-header">
                                <YAF:Icon runat="server" IconName="comment" />
                                <span class="fw-bold">
                                    <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server"
                                                        LocalizedTag="topic" />
                                </span><a
                                    title='<%# this.GetText("COMMON", "VIEW_TOPIC") %>'
                                    href='<%# this.Get<LinkBuilder>().GetTopicLink(((Tuple<Message, Topic, User>)Container.DataItem).Item2.ID, this.Eval("Item2.TopicName").ToString()) %>'>
                                    <%# this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.Eval("Item2.TopicName").ToString())) %>
                                </a>
                            </div>
                            <div class="card-body">
                                <YAF:MessagePostData ID="MessagePost" runat="server"
                                                     ShowAttachments="false"
                                                     CurrentMessage='<%# this.Eval("Item1") %>' />
                            </div>
                            <div class="card-footer">
                                <small class="text-body-secondary">
                                    <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server"
                                                        LocalizedTag="posted" />
                                    &nbsp;
                                            <%# this.Get<IDateTimeService>().FormatDateTime(((Tuple<Message, Topic, User>)Container.DataItem).Item1.Posted)%>
                                </small>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                </div>
            </div>
        </asp:PlaceHolder>
        <div runat="server" id="ModerateTab" class="card mb-3">
            <div class="card-header">
                <YAF:LocalizedLabel ID="LocalizedLabel46" runat="server" LocalizedTag="MODERATION" />
            </div>
            <div class="card-body">
                <nav>
                    <div class="nav nav-tabs d-flex flex-nowrap" id="nav-tab" role="tablist" style="overflow-x: auto; overflow-y:hidden; white-space: nowrap;">
                        <a class="nav-item nav-link active"
                            id="nav-suspend-tab"
                            data-bs-toggle="tab"
                            href="#nav-suspend"
                            role="tab">
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_USER" />
                        </a>
                        <a class="nav-item nav-link"
                            id="nav-signature-tab"
                            data-bs-toggle="tab"
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
                        LocalizedTag='<%# this.UserId == this.PageBoardContext.PageUserID ? "BUDDIES" : "BUDDIESTITLE"%>' />
                </div>
                <div class="card-body">
                    <asp:Repeater ID="Friends" runat="server">
                        <HeaderTemplate>
                            <ul class="list-group list-group-flush">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item">
                                <YAF:UserLink ID="UserProfileLink" runat="server"
                                              ReplaceName="<%# this.PageBoardContext.BoardSettings.EnableDisplayName ? (Container.DataItem as dynamic).DisplayName : (Container.DataItem as dynamic).Name %>"
                                              Suspended="<%# (Container.DataItem as dynamic).Suspended %>"
                                              Style="<%# (Container.DataItem as dynamic).UserStyle %>"
                                              UserID="<%#  this.UserId == (int)(Container.DataItem as dynamic).UserID ? (Container.DataItem as dynamic).FromUserID: (Container.DataItem as dynamic).UserID %>" />

                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                        </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
</div>



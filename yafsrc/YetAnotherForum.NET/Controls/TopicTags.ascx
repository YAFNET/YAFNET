<%@ Control Language="C#" AutoEventWireup="True" EnableViewState="false" CodeBehind="TopicTags.ascx.cs"
    Inherits="YAF.Controls.TopicTags" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Core.Services" %>


<div class="row mb-3">
    <div class="col">
        <asp:Repeater runat="server" ID="Tags">
            <HeaderTemplate>
                <h5>
            </HeaderTemplate>
            <FooterTemplate>
                </h5>
            </FooterTemplate>
            <ItemTemplate>
                <span class="badge text-bg-primary">
                    <YAF:Icon runat="server" IconName="tag"></YAF:Icon>
                    <a href="<%# this.Get<LinkBuilder>().GetLink(
                                     ForumPages.Search,
                                     new { tag = this.Eval("Item2.TagName")} ) %>"
                       class="link-light"><%# this.Eval("Item2.TagName") %>
                    </a>
                </span>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
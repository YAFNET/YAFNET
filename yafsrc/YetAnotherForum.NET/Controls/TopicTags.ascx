<%@ Control Language="C#" AutoEventWireup="True" EnableViewState="false" CodeBehind="TopicTags.ascx.cs"
    Inherits="YAF.Controls.TopicTags" %>
<%@ Import Namespace="YAF.Types.Constants" %>


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
                <span class="badge badge-primary">
                    <YAF:Icon runat="server" IconName="tag"></YAF:Icon>
                    <a href="<%# BuildLink.GetLinkNotEscaped(
                                     ForumPages.Search,
                                     "tag={0}",
                                     this.Eval("Item2.TagName")) %>"
                       class="text-white"><%# this.Eval("Item2.TagName") %>
                    </a>
                </span>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
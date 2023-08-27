<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.MostActiveUsers" Codebehind="MostActiveUsers.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>


<div class="card mb-3">
    <div class="card-header d-flex align-items-center">
        <YAF:IconHeader runat="server" ID="IconHeader"
                        IconType="text-secondary"
                        IconName="chart-line"
                        IconSize="fa-2x"
                        LocalizedTag="MOST_ACTIVE"></YAF:IconHeader>
    </div>
    <div class="card-body">
        <asp:Repeater runat="server" ID="Users" OnItemDataBound="Users_OnItemDataBound">
            <HeaderTemplate>
                <ol class="mb-0">
            </HeaderTemplate>
            <ItemTemplate>
                <li><YAF:UserLink runat="server" ID="UserLink"/>
                (<%# (Container.DataItem as LastActive).NumOfPosts %>)</li>
            </ItemTemplate>
            <FooterTemplate>
                </ol>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</div>
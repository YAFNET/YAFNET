<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="true" Inherits="YAF.Controls.ProfileTimeline"
    CodeBehind="ProfileTimeline.ascx.cs" %>

<YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />


<asp:Repeater runat="server" ID="ActivityStream" OnItemDataBound="ActivityStream_OnItemDataBound" OnItemCommand="ActivityStream_OnItemCommand">
    <HeaderTemplate>
        <div class="container">
    </HeaderTemplate>
    <ItemTemplate>
        <div class="row">
            <div class="col-auto text-center flex-column d-none d-sm-flex">
                <div class="row h-50">
                    <div class="col <%# this.GetFirstItemClass(Container.ItemIndex) %>">&nbsp;</div>
                    <div class="col">&nbsp;</div>
                </div>
                <h5 class="m-2">
                    <asp:Label runat="server" ID="Icon"
                               CssClass="fa-stack fa-1x" ></asp:Label>
                </h5>
                <div class="row h-50">
                    <div class="col <%# this.GetLastItemClass(Container.ItemIndex) %>">&nbsp;</div>
                    <div class="col">&nbsp;</div>
                </div>
            </div>
            <div class="col py-2">
                <asp:Panel runat="server" ID="Card">
                    <div class="card-body">
                        <div class="float-right text-muted">
                            <YAF:DisplayDateTime id="DisplayDateTime" runat="server">
                            </YAF:DisplayDateTime>
                        </div>
                        <h4 class="card-title">
                            <asp:Literal runat="server" ID="Title"></asp:Literal>
                        </h4>
                        <p>
                            <asp:PlaceHolder runat="server" ID="Message"></asp:PlaceHolder>
                        </p>
                        <YAF:ThemeButton runat="server" ID="MarkRead"
                                         Type="Secondary"
                                         Size="Small"
                                         TextLocalizedTag="MARK_ASREAD"
                                         CommandName="read"
                                         Icon="glasses"
                                         Visible="False">
                        </YAF:ThemeButton>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
</asp:Repeater>
<YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />

<div class="row">
    <div class="col">
        <div class="btn-group float-right" role="group" aria-label="Tools">
            <YAF:ThemeButton runat="server" OnClick="MarkAll_Click" ID="MarkAll"
                             TextLocalizedTag="MARK_ALL_ASREAD" TextLocalizedPage="DEFAULT"
                             Type="Secondary"
                             Size="Small"
                             Icon="glasses"/>
        </div>
    </div>
</div>
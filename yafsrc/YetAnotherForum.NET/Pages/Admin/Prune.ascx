<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.Prune" CodeBehind="Prune.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h1>
            <YAF:HelpLabel ID="LocalizedLabel1" runat="server" 
                                LocalizedTag="TITLE" 
                                LocalizedPage="ADMIN_PRUNE" />
        </h1>
    </div>
</div>
<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="trash"
                                LocalizedPage="ADMIN_PRUNE"></YAF:IconHeader>
            </div>
            <div class="card-body">
                <asp:Label ID="lblPruneInfo" runat="server"></asp:Label>
                <div class="form-group">
                    <YAF:HelpLabel ID="LocalizedLabel4" runat="server"
                                   LocalizedTag="PRUNE_FORUM" LocalizedPage="ADMIN_PRUNE"
                                   AssociatedControlID="forumlist"/>
                    <asp:DropDownList ID="forumlist" runat="server"
                                      CssClass="custom-select">
                    </asp:DropDownList>
                </div>
                <div class="form-row">
                    <div class="form-group col-md-4">
                        <YAF:HelpLabel ID="LocalizedLabel3" runat="server"
                                       LocalizedTag="PRUNE_DAYS" LocalizedPage="ADMIN_PRUNE"
                                       AssociatedControlID="days"/>
                        <div class="input-group">
                            <asp:TextBox ID="days" runat="server" 
                                         CssClass="form-control" 
                                         TextMode="Number"></asp:TextBox>
                            <div class="input-group-append">
                                <div class="input-group-text">
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="DAYS"></YAF:LocalizedLabel>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="form-group col-md-4">
                        <YAF:HelpLabel ID="LocalizedLabel2" runat="server" 
                                       LocalizedTag="PRUNE_PERMANENT" LocalizedPage="ADMIN_PRUNE"
                                       AssociatedControlID="permDeleteChkBox"/>
                        <div class="custom-control custom-switch">
                            <asp:CheckBox ID="permDeleteChkBox" runat="server" 
                                          Text="&nbsp;" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="commit" runat="server"
                    Type="Primary"
                    OnClick="CommitClick"
                    Icon="trash"
                    TextLocalizedTag="PRUNE_START"
                    ReturnConfirmText='<%# this.GetText("ADMIN_PRUNE", "CONFIRM_PRUNE") %>'>
                </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>
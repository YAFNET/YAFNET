<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.EditRank" Codebehind="EditRank.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconType="text-secondary"
                                    IconName="graduation-cap"
                                    LocalizedPage="ADMIN_EDITRANK"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                           AssociatedControlID="Name"
                                           LocalizedTag="RANK_NAME" LocalizedPage="ADMIN_EDITRANK" />
                            <asp:TextBox ID="Name" runat="server"
                                          CssClass="form-control"
                                         required="required" />
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="NEED_NAME" />
                            </div>
                        </div>
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel6" runat="server"
                                           AssociatedControlID="Description"
                                           LocalizedTag="RANK_DESC" LocalizedPage="ADMIN_EDITRANK" />
                            <asp:TextBox ID="Description" runat="server"
                                         CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel12" runat="server"
                                           AssociatedControlID="RankPriority"
                                           LocalizedTag="RANK_PRIO" LocalizedPage="ADMIN_EDITRANK" />
                            <asp:TextBox ID="RankPriority" runat="server"
                                         Text="0"
                                         CssClass="form-control"
                                         required="required"
                                         TextMode="Number" />
                        </div>
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel2" runat="server"
                                           AssociatedControlID="IsStart"
                                           LocalizedTag="IS_START" LocalizedPage="ADMIN_EDITRANK" />
                            <div class="form-check form-switch">
                                <asp:CheckBox ID="IsStart" runat="server"
                                              Text="&nbsp;"></asp:CheckBox>
                            </div>
                        </div>
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel3" runat="server"
                                           AssociatedControlID="IsLadder"
                                           LocalizedTag="LADDER_GROUP" LocalizedPage="ADMIN_EDITRANK" />
                            <div class="form-check form-switch">
                                <asp:CheckBox ID="IsLadder" runat="server"
                                              Text="&nbsp;"></asp:CheckBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel4" runat="server"
                                           AssociatedControlID="MinPosts"
                                           LocalizedTag="MIN_POSTS" LocalizedPage="ADMIN_EDITRANK" />
                            <asp:TextBox ID="MinPosts" runat="server"
                                         CssClass="form-control"
                                         TextMode="Number" />
                        </div>
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel5" runat="server"
                                           AssociatedControlID="PMLimit"
                                           LocalizedTag="PRIVATE_MESSAGES" LocalizedPage="ADMIN_EDITRANK" />
                            <asp:TextBox ID="PMLimit" runat="server"
                                         Text="0"
                                         CssClass="form-control"
                                         TextMode="Number" />
                        </div>
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel7" runat="server"
                                           AssociatedControlID="UsrSigChars"
                                           LocalizedTag="SIG_LENGTH" LocalizedPage="ADMIN_EDITRANK" />
                            <asp:TextBox ID="UsrSigChars" runat="server"
                                         Text="128"
                                         CssClass="form-control"
                                         TextMode="Number" />
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel8" runat="server"
                                       AssociatedControlID="UsrSigBBCodes"
                                       LocalizedTag="SIG_BBCODE" LocalizedPage="ADMIN_EDITRANK" />
                        <asp:TextBox ID="UsrSigBBCodes" runat="server"
                                     CssClass="form-control" />
                    </div>
                    <div class="row">
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel10" runat="server"
                               AssociatedControlID="UsrAlbums"
                               LocalizedTag="ALBUMS_NUMBER" LocalizedPage="ADMIN_EDITRANK" />
                            <asp:TextBox ID="UsrAlbums" runat="server"
                                         Text="0"
                                         CssClass="form-control"
                                         TextMode="Number" />
                        </div>
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel11" runat="server"
                                           AssociatedControlID="UsrAlbumImages"
                                           LocalizedTag="IMAGES_NUMBER" LocalizedPage="ADMIN_EDITRANK" />
                            <asp:TextBox ID="UsrAlbumImages" runat="server"
                                         Text="0"
                                         CssClass="form-control"
                                         TextMode="Number" />
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel13" runat="server"
                                       AssociatedControlID="Style"
                                       LocalizedTag="RANK_STYLE" LocalizedPage="ADMIN_EDITRANK" />
                        <asp:TextBox ID="Style" runat="server"
                                     CssClass="form-control"
                                     TextMode="MultiLine" />
                    </div>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Save" runat="server"
                                     OnClick="Save_Click"
                                     CausesValidation="True"
                                     CssClass="me-2"
                                     Type="Primary"
                                     Icon="save"
                                     TextLocalizedTag="SAVE">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="Cancel" runat="server"
                                     OnClick="Cancel_Click"
                                     Type="Secondary"
                                     Icon="times"
                                     TextLocalizedTag="CANCEL">
                    </YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>



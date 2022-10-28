<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.EditForum"
    CodeBehind="EditForum.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server" ID="IconHeader"
                                    IconName="comments"></YAF:IconHeader>
                </div>
                <asp:Panel CssClass="card-body" runat="server" ID="Body">
                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel3" runat="server" 
                                           AssociatedControlID="Name"
                                           LocalizedTag="NAME" LocalizedPage="ADMIN_EDITFORUM" />
                            <asp:TextBox ID="Name" runat="server" 
                                         required="required"
                                         CssClass="form-control"></asp:TextBox>
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="MSG_NAME_FORUM" />
                            </div>
                        </div>
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel4" runat="server" 
                                           AssociatedControlID="Description"
                                           LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_EDITFORUM" />
                            <asp:TextBox ID="Description" runat="server" 
                                         CssClass="form-control"
                                         required="required"></asp:TextBox>
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="MSG_VALUE" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server" 
                                           AssociatedControlID="CategoryList"
                                           LocalizedTag="CATEGORY" LocalizedPage="ADMIN_EDITFORUM" />
                            <asp:DropDownList ID="CategoryList" runat="server" OnSelectedIndexChanged="CategoryChange"
                                              DataValueField="ID" 
                                              DataTextField="Name" 
                                              CssClass="form-select">
                            </asp:DropDownList>
                        </div>
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel2" runat="server" 
                                           AssociatedControlID="ParentList"
                                           LocalizedTag="PARENT_FORUM" LocalizedPage="ADMIN_EDITFORUM" />
                            <asp:DropDownList ID="ParentList" runat="server" 
                                              CssClass="select2-image-select">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <hr/>
                    <h3>
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                            LocalizedTag="HEADER2" LocalizedPage="ADMIN_EDITFORUM" />
                    </h3>
                    <asp:Repeater ID="AccessList" runat="server">
                        <HeaderTemplate>
                            <div class="row">
                        </HeaderTemplate>
                        <FooterTemplate>
                        </div>
                        </FooterTemplate>
                        <ItemTemplate>
                            <div class="mb-3 col-md-4">
                                <asp:HiddenField ID="GroupID" Visible="false" runat="server" 
                                                 Value='<%# this.Eval( "GroupID") %>'>
                                </asp:HiddenField>
                                <asp:Label runat="server" Text='<%# this.Eval( "GroupName") %>' 
                                           AssociatedControlID="AccessMaskID"></asp:Label>
                                <asp:DropDownList runat="server" ID="AccessMaskID" 
                                                  OnDataBinding="BindDataAccessMaskId" 
                                                  CssClass="form-select"
                                                  OnPreRender="SetDropDownIndex" 
                                                  Value='<%# this.Eval("AccessMaskID") %>' />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <hr/>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel14" runat="server" 
                                       AssociatedControlID="remoteurl"
                                       LocalizedTag="REMOTE_URL" LocalizedPage="ADMIN_EDITFORUM" />
             
                        <asp:TextBox ID="remoteurl" runat="server" 
                                     CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel12" runat="server" 
                                           AssociatedControlID="SortOrder"
                                           LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_EDITFORUM" />
                            <asp:TextBox ID="SortOrder" runat="server"
                                         MaxLength="5"
                                         Text="10" 
                                         CssClass="form-control" 
                                         TextMode="Number" data-min="1"></asp:TextBox>
                        </div>
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel13" runat="server" 
                                           AssociatedControlID="ThemeList"
                                           LocalizedTag="THEME" LocalizedPage="ADMIN_EDITFORUM" />

                            <asp:DropDownList ID="ThemeList" runat="server" CssClass="form-select">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel11" runat="server" 
                                           AssociatedControlID="HideNoAccess"
                                           LocalizedTag="HIDE_NOACESS" LocalizedPage="ADMIN_EDITFORUM" />
             
                            <div class="form-check form-switch">
                                <asp:CheckBox ID="HideNoAccess" runat="server" Text="&nbsp;"></asp:CheckBox>
                            </div>
                        </div>
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel10" runat="server" 
                                           AssociatedControlID="Locked"
                                           LocalizedTag="LOCKED" LocalizedPage="ADMIN_EDITFORUM" />
                            <div class="form-check form-switch">
                                <asp:CheckBox ID="Locked" runat="server" Text="&nbsp;"></asp:CheckBox>
                            </div>
                        </div>
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel9" runat="server" 
                                           AssociatedControlID="IsTest"
                                           LocalizedTag="NO_POSTSCOUNT" LocalizedPage="ADMIN_EDITFORUM" />
                            <div class="form-check form-switch">
                                <asp:CheckBox ID="IsTest" runat="server" Text="&nbsp;"></asp:CheckBox>
                            </div>
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel8" runat="server" 
                                       AssociatedControlID="Moderated"
                                       LocalizedTag="PRE_MODERATED" LocalizedPage="ADMIN_EDITFORUM" />
                        <div class="form-check form-switch">
                            <asp:CheckBox ID="Moderated" runat="server" 
                                          AutoPostBack="true" 
                                          OnCheckedChanged="ModeratedCheckedChanged" 
                                          Text="&nbsp;"></asp:CheckBox>
                        </div>
                    </div>
                    <asp:PlaceHolder runat="server" id="ModerateNewTopicOnlyRow" Visible="false">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel16" runat="server" 
                                           AssociatedControlID="ModerateNewTopicOnly"
                                           LocalizedTag="MODERATED_NEWTOPIC_ONLY" LocalizedPage="ADMIN_EDITFORUM" />
             
                            <div class="form-check form-switch">
                                <asp:CheckBox ID="ModerateNewTopicOnly" runat="server" Text="&nbsp;"></asp:CheckBox>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" id="ModeratedPostCountRow" Visible="false">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel15" runat="server" 
                                           AssociatedControlID="ModerateAllPosts"
                                           LocalizedTag="MODERATED_COUNT" LocalizedPage="ADMIN_EDITFORUM" />
             
                            <div class="form-check form-switch">
                                <asp:CheckBox ID="ModerateAllPosts" runat="server" AutoPostBack="true" 
                                              OnCheckedChanged="ModerateAllPostsCheckedChanged" 
                                              Checked="true" 
                                              Text="&nbsp;">
                                </asp:CheckBox>
                            </div>
                            <asp:TextBox ID="ModeratedPostCount" runat="server" 
                                         Visible="false" MaxLength="5" Text="5" CssClass="form-control" 
                                         TextMode="Number">
                            </asp:TextBox>
                        </div>
                    </asp:PlaceHolder>
                    <div class="mb-3">
                    <YAF:HelpLabel ID="HelpLabel7" runat="server" 
                                   AssociatedControlID="ForumImages"
                                   LocalizedTag="FORUM_IMAGE" LocalizedPage="ADMIN_EDITFORUM" />
                    <YAF:ImageListBox ID="ForumImages" runat="server" 
                                      CssClass="select2-image-select" />
                </div>
                <div class="mb-3">
                    <YAF:HelpLabel ID="HelpLabel6" runat="server" 
                                   AssociatedControlID="Styles"
                                   LocalizedTag="STYLES" LocalizedPage="ADMIN_EDITFORUM" />
                    <asp:TextBox ID="Styles" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                </asp:Panel>
                <div class="card-footer text-center">
                <YAF:ThemeButton ID="Save" runat="server" 
                                 CssClass="me-2"
                                 CausesValidation="True"
                                 Type="Primary"
                                 Icon="save" 
                                 TextLocalizedTag="SAVE">
                </YAF:ThemeButton>
                <YAF:ThemeButton ID="Cancel" runat="server" 
                                 Type="Secondary"
                                 Icon="times" 
                                 TextLocalizedTag="CANCEL">
                </YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>



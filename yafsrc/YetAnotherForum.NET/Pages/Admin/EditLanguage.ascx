<%@ Control language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.EditLanguage" Codebehind="EditLanguage.ascx.cs" %>


<YAF:PageLinks runat="server" id="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server" ID="IconHeader"
                                IconType="text-secondary"
                                IconName="language"
                                LocalizedPage="ADMIN_VERSION"></YAF:IconHeader>
            </div>
            <div class="card-body">
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="Pages">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                            LocalizedTag="SELECT_PAGE" 
                                            LocalizedPage="ADMIN_EDITLANGUAGE" />
                    </asp:Label>
                    <asp:DropDownList runat="server" id="Pages" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="mb-3">
                    <YAF:ThemeButton runat="server" id="LoadPageLocalization"
                                     OnClick="LoadPageLocalizationClick"
                                     Type="Primary" 
                                     Icon="share" 
                                     TextLocalizedTag="LOAD_PAGE" TextLocalizedPage="ADMIN_EDITLANGUAGE" />
                </div>
                <hr />
                <div class="table-responsive">
                <asp:DataGrid id="Locals" runat="server"
                              OnItemDataBound="LocalsItemDataBound"
                              CssClass="table table-striped"
                              GridLines="None"
                              AutoGenerateColumns="False"
                              UseAccessibleHeader="True">
                    <Columns>
                        <asp:TemplateColumn>
                            <HeaderTemplate>
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                    LocalizedTag="RESOURCE_NAME" 
                                                    LocalizedPage="ADMIN_EDITLANGUAGE" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label id="lblResourceName" runat="server" 
                                           Text='<%# this.Eval("ResourceName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderTemplate>
                                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                    LocalizedTag="ORIGINAL_RESOURCE" 
                                                    LocalizedPage="ADMIN_EDITLANGUAGE" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:TextBox id="txtResource" runat="server" 
                                             Text='<%# this.Eval("OriginalResourceText") %>' 
                                             TextMode="MultiLine" 
                                             Rows="3" 
                                             Enabled="false" 
                                             CssClass="form-control">
                                    </asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderTemplate>
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                    LocalizedTag="LOCALIZED_RESOURCE" 
                                                    LocalizedPage="ADMIN_EDITLANGUAGE" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:TextBox id="txtLocalized" runat="server" 
                                             Text='<%# this.Eval("LocalizedResourceText") %>' 
                                             TextMode="MultiLine" 
                                             Rows="3" 
                                             ToolTip='<%# this.Eval("OriginalResourceText") %>' 
                                             CssClass="form-control">
                                </asp:TextBox>
                                <div class="invalid-feedback">
                                    <YAF:LocalizedLabel runat="server" LocalizedTag="NEED_TRANSLATION"></YAF:LocalizedLabel>
                                </div>
                                <asp:CustomValidator runat="server" id="custTextLocalized" 
                                                     ControlToValidate="txtLocalized" 
                                                     OnServerValidate="LocalizedTextCheck">
                                </asp:CustomValidator>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
                    </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" id="Save"
                                 CssClass="me-1"
                                 OnClick="SaveClick"
                                 Type="Primary"
                                 Icon="save" 
                                 TextLocalizedTag="SAVE" />
                <YAF:ThemeButton runat="server" id="Cancel"
                                 Type="Secondary" 
                                 Icon="times"
                                 OnClick="CancelClick"
                                 TextLocalizedTag="CANCEL" />
            </div>
        </div>
    </div>
</div>
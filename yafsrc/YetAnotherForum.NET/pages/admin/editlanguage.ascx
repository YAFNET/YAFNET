<%@ Control language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editlanguage" Codebehind="editlanguage.ascx.cs" %>


<YAF:PageLinks runat="server" id="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:Icon runat="server" IconName="language" IconType="text-secondary"></YAF:Icon>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EDITLANGUAGE" />
                <asp:Label runat="server" id="lblPageName"></asp:Label>
            </div>
            <div class="card-body">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="dDLPages">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                            LocalizedTag="SELECT_PAGE" 
                                            LocalizedPage="ADMIN_EDITLANGUAGE" />
                    </asp:Label>
                    <asp:DropDownList runat="server" id="dDLPages" CssClass="custom-select"></asp:DropDownList>
                </div>
                <div class="form-group">
                    <YAF:ThemeButton runat="server" id="btnLoadPageLocalization" 
                                     Type="Primary" 
                                     Icon="share" 
                                     TextLocalizedTag="LOAD_PAGE" TextLocalizedPage="ADMIN_EDITLANGUAGE" />
                </div>
                <YAF:Alert ID="Info" runat="server" Type="danger">
                    <asp:Label runat="server" id="lblInfo"></asp:Label>
                </YAF:Alert>
                <hr />
                <asp:DataGrid id="grdLocals" runat="server" 
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
                                             Text='<%# this.Eval("ResourceValue") %>' 
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
                                             Text='<%# this.Eval("LocalizedValue") %>' 
                                             TextMode="MultiLine" 
                                             Rows="3" 
                                             ToolTip='<%# this.Eval("ResourceValue") %>' 
                                             CssClass="form-control">
                                </asp:TextBox>
                                <asp:CustomValidator runat="server" id="custTextLocalized" 
                                                     ControlToValidate="txtLocalized" 
                                                     OnServerValidate="LocalizedTextCheck">
                                </asp:CustomValidator>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" id="btnSave"
                                 CssClass="mr-1"
                                 Type="Primary"
                                 Icon="save" 
                                 TextLocalizedTag="SAVE" />
                <YAF:ThemeButton runat="server" id="btnCancel"
                                 Type="Secondary" 
                                 Icon="times" 
                                 TextLocalizedTag="CANCEL" />
            </div>
        </div>
    </div>
</div>
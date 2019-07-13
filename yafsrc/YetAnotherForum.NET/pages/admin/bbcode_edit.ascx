<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bbcode_edit" Codebehind="bbcode_edit.ascx.cs" %>

<YAF:PageLinks runat="server" id="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                LocalizedTag="HEADER" 
                                LocalizedPage="ADMIN_BBCODE_EDIT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-plug fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                                                             LocalizedTag="HEADER" 
                                                                                             LocalizedPage="ADMIN_BBCODE_EDIT" />
                </div>
                <div class="card-body">
                    <h2><YAF:LocalizedLabel ID="HelpLabel13" runat="server" 
                                            LocalizedTag="HEADER1" 
                                            LocalizedPage="ADMIN_BBCODE_EDIT" /></h2>
                    <hr />
                    <div class="ml-2">
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server" 
                                           AssociatedControlID="txtName"
                                           LocalizedTag="BBCODE_NAME" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                            <asp:TextBox id="txtName" runat="server" 
                                         CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel2" runat="server" 
                                           AssociatedControlID="txtExecOrder"
                                           LocalizedTag="EXEC_ORDER" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />

                            <asp:TextBox ID="txtExecOrder" runat="server" 
                                         Text="1" 
                                         CssClass="form-control" 
                                         TextMode="Number"
                                         MaxLength="5"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel3" runat="server" 
                                           AssociatedControlID="txtDescription"
                                           LocalizedTag="BBCODE_DESC" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                            <asp:TextBox ID="txtDescription" runat="server" 
                                         TextMode="MultiLine" 
                                         CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel14" runat="server" 
                                           AssociatedControlID="UseToolbar"
                                           LocalizedTag="USE_TOOLBAR" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                     
                            <div class="custom-control custom-switch">
                                <asp:CheckBox ID="UseToolbar" runat="server" Text="&nbsp;" />
                            </div>
                        </div>
                    </div>
                    <h2>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                            LocalizedTag="HEADER2" LocalizedPage="ADMIN_BBCODE_EDIT" />
                    </h2>
                    <hr />
                    <div class="ml-2">
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel4" runat="server" 
                                           AssociatedControlID="txtSearchRegEx"
                                           LocalizedTag="SEARCH_REGEX" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                            <asp:TextBox ID="txtSearchRegEx" runat="server" 
                                         TextMode="MultiLine" 
                                         CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel5" runat="server" 
                                           AssociatedControlID="txtReplaceRegEx"
                                           LocalizedTag="REPLACE_REGEX" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                            <asp:TextBox ID="txtReplaceRegEx" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel8" runat="server" 
                                           AssociatedControlID="txtVariables"
                                           LocalizedTag="REPLACE_VARIABLES" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                            <asp:TextBox ID="txtVariables" runat="server" 
                                         TextMode="MultiLine"
                                         CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="HelpLabel6" runat="server" 
                                               AssociatedControlID="chkUseModule"
                                               LocalizedTag="USE_MODULE" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox ID="chkUseModule" runat="server" 
                                                  Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="HelpLabel7" runat="server" 
                                               AssociatedControlID="txtModuleClass"
                                               LocalizedTag="CLASS_NAME" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                                <asp:TextBox ID="txtModuleClass" runat="server" 
                                             CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <h2>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_BBCODE_EDIT" />
                    </h2>
                    <hr />
                    <div class="ml-2">
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel9" runat="server" 
                                           AssociatedControlID="txtOnClickJS"
                                           LocalizedTag="ONCLICK_JS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                            <asp:TextBox ID="txtOnClickJS" runat="server" 
                                         TextMode="MultiLine" 
                                         CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel10" runat="server" 
                                           AssociatedControlID="txtDisplayJS"
                                           LocalizedTag="DISPLAY_JS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                            <asp:TextBox ID="txtDisplayJS" runat="server" 
                                         TextMode="MultiLine" 
                                         CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel11" runat="server" 
                                           AssociatedControlID="txtEditJS"
                                           LocalizedTag="EDIT_JS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
                            <asp:TextBox ID="txtEditJS" runat="server" 
                                         TextMode="MultiLine" 
                                         CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel12" runat="server" 
                                           AssociatedControlID="txtDisplayCSS"
                                           LocalizedTag="DISPLAY_CSS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />

                            <asp:TextBox ID="txtDisplayCSS" runat="server" 
                                         TextMode="MultiLine" 
                                         CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="card-footer text-center">
				<YAF:ThemeButton id="save" runat="server" 
                                 OnClick="Add_Click" 
                                 Type="Primary"
				                 Icon="save" 
                                 TextLocalizedTag="SAVE">
                </YAF:ThemeButton>
				<YAF:ThemeButton id="cancel" runat="server"
                                 OnClick="Cancel_Click"
                                 Type="Secondary"
				                 Icon="times"
                                 TextLocalizedTag="CANCEL">
                </YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
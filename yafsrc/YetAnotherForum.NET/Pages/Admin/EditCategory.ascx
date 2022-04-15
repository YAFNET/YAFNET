<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.EditCategory" Codebehind="EditCategory.ascx.cs" %>


<YAF:PageLinks ID="PageLinks" runat="server" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server" ID="IconHeader"
                                    IconName="comments"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                           AssociatedControlID="Name"
                                           LocalizedTag="CATEGORY_NAME" 
                                           LocalizedPage="ADMIN_EDITCATEGORY" />
                            <asp:TextBox ID="Name" runat="server" 
                                         MaxLength="50"
                                         required="required" 
                                         CssClass="form-control"></asp:TextBox>
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="MSG_VALUE" />
                            </div>
                        </div>
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel3" runat="server"
                                           AssociatedControlID="SortOrder"
                                           LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_EDITCATEGORY" />
                            <asp:TextBox ID="SortOrder" runat="server" 
                                         MaxLength="5" 
                                         required="required" 
                                         CssClass="form-control" 
                                         TextMode="Number"></asp:TextBox>
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="MSG_VALUE" />
                            </div>
                        </div>
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel4" runat="server"
                                           AssociatedControlID="Active"
                                           LocalizedTag="Active" LocalizedPage="ADMIN_EDITCATEGORY" />
                            <div class="form-check form-switch">
                                <asp:CheckBox runat="server" ID="Active" Text="&nbsp;" />
                            </div>
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel2" runat="server"
                                       AssociatedControlID="CategoryImages"
                                       LocalizedTag="CATEGORY_IMAGE" LocalizedPage="ADMIN_EDITCATEGORY" />
                        <YAF:ImageListBox ID="CategoryImages" runat="server" 
                                          CssClass="select2-image-select"/>
                    </div>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Save" runat="server" 
                                     OnClick="SaveClick" 
                                     CausesValidation="True"
                                     Type="Primary"
                                     Icon="save" 
                                     TextLocalizedTag="Save">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="Cancel" runat="server" 
                                     OnClick="CancelClick" 
                                     Type="Secondary"
                                     Icon="times" 
                                     TextLocalizedTag="CANCEL">
                    </YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>



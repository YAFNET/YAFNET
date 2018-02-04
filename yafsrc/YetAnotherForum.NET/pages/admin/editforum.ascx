<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editforum"
    CodeBehind="editforum.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_EDITFORUM" />&nbsp;<asp:Label ID="Label1" runat="server"></asp:Label></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-comments fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_EDITFORUM" />&nbsp;<asp:Label ID="ForumNameTitle" runat="server"></asp:Label>
                </div>
                <div class="card-body">
            <h4>
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="CATEGORY" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:DropDownList ID="CategoryList" runat="server" OnSelectedIndexChanged="CategoryChange"
                    DataValueField="ID" DataTextField="Name" CssClass="custom-select">
                </asp:DropDownList>
            </p><hr />
      
        
            <h4>
                <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="PARENT_FORUM" LocalizedPage="ADMIN_EDITFORUM" />
                <strong></strong>
                <br />
            </h4>
            <p>
                <asp:DropDownList ID="ParentList" runat="server" CssClass="custom-select">
                </asp:DropDownList>
            </p><hr />
      
        
            <h4>
                <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:TextBox ID="Name" runat="server" CssClass="form-control"></asp:TextBox>
            </p><hr />
      
        
            <h4>
                <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:TextBox ID="Description" runat="server" CssClass="form-control"></asp:TextBox>
            </p><hr />
      
        
            <h4>
                <YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="REMOTE_URL" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:TextBox ID="remoteurl" runat="server" CssClass="form-control"></asp:TextBox>
            </p><hr />
      
        
            <h4>
                <YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="THEME" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:DropDownList ID="ThemeList" runat="server" CssClass="custom-select">
                </asp:DropDownList>
            </p><hr />
      
        
            <h4>
                <YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:TextBox ID="SortOrder" MaxLength="5" runat="server" Text="10" CssClass="form-control" TextMode="Number"></asp:TextBox>
            </p><hr />
      
        
            <h4>
                <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="HIDE_NOACESS" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:CheckBox ID="HideNoAccess" runat="server" CssClass="form-control"></asp:CheckBox>
            </p><hr />
      
        
            <h4>
                <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="LOCKED" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:CheckBox ID="Locked" runat="server" CssClass="form-control"></asp:CheckBox>
            </p><hr />
      
        
            <h4>
                <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="NO_POSTSCOUNT" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:CheckBox ID="IsTest" runat="server" CssClass="form-control"></asp:CheckBox>
            </p><hr />
      
        
            <h4>
                <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="PRE_MODERATED" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:CheckBox ID="Moderated" runat="server" AutoPostBack="true" OnCheckedChanged="ModeratedCheckedChanged" CssClass="form-control"></asp:CheckBox>
            </p><hr />
      
        <asp:PlaceHolder runat="server" id="ModerateNewTopicOnlyRow" Visible="false">
            <h4>
                <YAF:HelpLabel ID="HelpLabel16" runat="server" LocalizedTag="MODERATED_NEWTOPIC_ONLY" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:CheckBox ID="ModerateNewTopicOnly" runat="server" AutoPostBack="true" CssClass="form-control"></asp:CheckBox>
            </p><hr />
       </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" id="ModeratedPostCountRow" Visible="false">
            <h4>
                <YAF:HelpLabel ID="HelpLabel15" runat="server" LocalizedTag="MODERATED_COUNT" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:CheckBox ID="ModerateAllPosts" runat="server" AutoPostBack="true" 
                    OnCheckedChanged="ModerateAllPostsCheckedChanged" Checked="true" CssClass="form-control">
                </asp:CheckBox>
                <div>
                    <asp:TextBox ID="ModeratedPostCount" runat="server" 
                        Visible="false" MaxLength="5" Text="5" CssClass="form-control" TextMode="Number">
                    </asp:TextBox>
                </div>
            </p><hr />
      
        </asp:PlaceHolder>
            <h4>
                <YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="FORUM_IMAGE" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <img align="middle" runat="server" id="Preview" alt="" />
            </p>
            <p>
                <asp:DropDownList ID="ForumImages" runat="server" CssClass="custom-select" />
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="STYLES" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:TextBox ID="Styles" runat="server" CssClass="form-control"></asp:TextBox>
            </p><hr />
         <h3><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_EDITFORUM" /></h3>
        <asp:PlaceHolder id="NewGroupRow" runat="server">
            <h4>
                <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="INITAL_MASK" LocalizedPage="ADMIN_EDITFORUM" />
            </h4>
            <p>
                <asp:DropDownList ID="AccessMaskID" OnDataBinding="BindDataAccessMaskId" CssClass="custom-select"
                    runat="server">
                </asp:DropDownList>
            </p><hr />
         </asp:PlaceHolder>
      
        <asp:Repeater ID="AccessList" runat="server">
            <ItemTemplate>
                
                    <h4>
                        <asp:Label ID="GroupID" Visible="false" runat="server" Text='<%# this.Eval( "GroupID") %>'>
                        </asp:Label>
                        <%# this.Eval( "GroupName") %>
                    </h4>
                    <p>
                        <asp:DropDownList runat="server" ID="AccessMaskID" OnDataBinding="BindDataAccessMaskId" CssClass="custom-select"
                            OnPreRender="SetDropDownIndex" value='<%# this.Eval("AccessMaskID") %>' />
                        ...
                   </p><hr />  
            </ItemTemplate>
        </asp:Repeater>
                </div>
                <div class="card-footer text-lg-center">
                <YAF:ThemeButton ID="Save" runat="server" Type="Primary"
                                 Icon="save" TextLocalizedTag="SAVE"></YAF:ThemeButton>&nbsp;
                <YAF:ThemeButton ID="Cancel" runat="server" Type="Secondary"
                                 Icon="times" TextLocalizedTag="CANCEL"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

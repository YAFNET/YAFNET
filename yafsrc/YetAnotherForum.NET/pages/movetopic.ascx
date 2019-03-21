<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.movetopic" Codebehind="movetopic.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-arrows-alt fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                                                LocalizedTag="TITLE" />
            </div>
            <div class="card-body text-center">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="ForumList">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                            LocalizedTag="select_forum" />
                    </asp:Label>
                    <asp:DropDownList ID="ForumList" runat="server"
                                      DataValueField="ForumID" 
                                      DataTextField="Title" 
                                      CssClass="standardSelectMenu" />
                </div>
                <asp:PlaceHolder id="trLeaveLink" runat="server">
                    <div class="form-check">
                        <asp:CheckBox ID="LeavePointer" runat="server" CssClass="form-check-input" />
                        <asp:Label runat="server" 
                                   AssociatedControlID="LeavePointer"
                                   CssClass="form-check-label">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                LocalizedTag="LEAVE_POINTER" />
                        </asp:Label>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder id="trLeaveLinkDays" runat="server">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="LinkDays">
                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                LocalizedTag="POINTER_DAYS" />
                        </asp:Label>
                        <asp:TextBox ID="LinkDays" runat="server" CssClass="Numeric" TextMode="Number" />
                    </div>
                </asp:PlaceHolder> 
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="Move" runat="server" 
                                 OnClick="Move_Click"
                                 TextLocalizedTag="MOVE"
                                 Type="Primary"
                                 Icon="arrows-alt"/>
            </div>
        </div>
    </div>
</div>


<div id="DivSmartScroller">
	
</div>

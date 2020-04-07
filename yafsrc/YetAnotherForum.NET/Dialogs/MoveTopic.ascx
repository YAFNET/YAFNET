<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MoveTopic.ascx.cs" Inherits="YAF.Dialogs.MoveTopic" %>


<div class="modal fade" id="MoveTopicDialog" tabindex="-1" role="dialog" aria-labelledby="Move Topic Dialog" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="BUTTON_MOVETOPIC" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
            <div class="modal-body">
             <!-- Modal Content START !-->
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="ForumList">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                    LocalizedTag="select_forum" />
                            </asp:Label>
                            <asp:DropDownList ID="ForumList" runat="server"
                                              DataValueField="ForumID" 
                                              DataTextField="Title" 
                                              CssClass="select2-image-select" />
                        </div>
                        <asp:PlaceHolder id="trLeaveLink" runat="server">
                            <div class="form-group">
                                <asp:Label runat="server" 
                                           AssociatedControlID="LeavePointer">
                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                        LocalizedTag="LEAVE_POINTER" />
                                </asp:Label>
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox ID="LeavePointer" runat="server" Text="&nbsp;" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder id="trLeaveLinkDays" runat="server">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="LinkDays">
                                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                        LocalizedTag="POINTER_DAYS" />
                                </asp:Label>
                                <asp:TextBox ID="LinkDays" runat="server" CssClass="form-control" TextMode="Number" />
                            </div>
                        </asp:PlaceHolder>
                    </div>
                        <!-- Modal Content END !-->           
            <div class="modal-footer">
                <YAF:ThemeButton ID="Move" runat="server" 
                                 OnClick="Move_Click"
                                 TextLocalizedTag="BUTTON_MOVETOPIC"
                                 Type="Primary"
                                 Icon="arrows-alt"/>
                <YAF:ThemeButton runat="server" ID="Cancel"
                                 DataDismiss="modal"
                                 TextLocalizedTag="CANCEL"
                                 Type="Secondary"
                                 Icon="times">
                </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>

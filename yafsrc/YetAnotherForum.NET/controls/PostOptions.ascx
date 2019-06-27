<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PostOptions"
    CodeBehind="PostOptions.ascx.cs" %>
<%@ Import Namespace="ServiceStack" %>

<asp:PlaceHolder id="OptionsRow" runat="server">
    <div class="row">
        <div class="col">
            <h6>
                <YAF:LocalizedLabel ID="NewPostOptionsLabel" runat="server" LocalizedTag="NEWPOSTOPTIONS" />
            </h6>
            <div id="liAddPoll" runat="server" class="form-check">
                <asp:CheckBox ID="AddPollCheckBox" runat="server" Text='<%# "{0}?".Fmt(this.GetText("POSTMESSAGE", "POLLADD")) %>' />
            </div>
            <div id="liQuestion" runat="server" visible="false" class="custom-control custom-checkbox">
                <asp:CheckBox ID="chkIsQuestion" runat="server" Text='<%# this.GetText("POSTMESSAGE", "ISQUESTION") %>' />
            </div>
            <div id="liPersistency" runat="server" class="custom-control custom-checkbox">
                <asp:CheckBox ID="Persistency" runat="server" Checked="True"/>
                <asp:Label runat="server" AssociatedControlID="Persistency">
                    <YAF:LocalizedLabel ID="PersistencyLabel" runat="server" LocalizedTag="PERSISTENCY" /> 
                    (<YAF:LocalizedLabel ID="PersistencyLabel2" runat="server" LocalizedTag="PERSISTENCY_INFO" />) 
                </asp:Label>
            </div>
            <div id="liTopicWatch" runat="server" class="custom-control custom-checkbox">
                <asp:CheckBox ID="TopicWatch" runat="server" Text='<%# this.GetText("POSTMESSAGE", "TOPICWATCH") %>' />
            </div>
        </div>
    </div>
</asp:PlaceHolder>
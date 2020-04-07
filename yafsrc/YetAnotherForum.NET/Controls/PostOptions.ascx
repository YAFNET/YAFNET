<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PostOptions"
    CodeBehind="PostOptions.ascx.cs" %>

<asp:PlaceHolder id="OptionsRow" runat="server">
    <div class="row">
        <div class="col">
            <h6>
                <YAF:LocalizedLabel ID="NewPostOptionsLabel" runat="server" LocalizedTag="NEWPOSTOPTIONS" />
            </h6>
            <div id="liAddPoll" runat="server" class="custom-control custom-checkbox">
                <asp:CheckBox ID="AddPollCheckBox" runat="server" />
                <asp:Label runat="server" AssociatedControlID="AddPollCheckBox">
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POLLADD" />?
                </asp:Label>
            </div>
            <div id="liPersistency" runat="server" class="custom-control custom-checkbox">
                <asp:CheckBox ID="Persistency" runat="server" Checked="True"/>
                <asp:Label runat="server" AssociatedControlID="Persistency">
                    <YAF:LocalizedLabel ID="PersistencyLabel" runat="server" LocalizedTag="PERSISTENCY" /> 
                    (<YAF:LocalizedLabel ID="PersistencyLabel2" runat="server" LocalizedTag="PERSISTENCY_INFO" />) 
                </asp:Label>
            </div>
            <div id="liTopicWatch" runat="server" class="custom-control custom-checkbox">
                <asp:CheckBox ID="TopicWatch" runat="server" />
                <asp:Label runat="server" AssociatedControlID="TopicWatch">
                    <YAF:LocalizedLabel ID="TopicWatchLabel" runat="server" LocalizedTag="TOPICWATCH" /> 
                </asp:Label>
            </div>
        </div>
    </div>
</asp:PlaceHolder>
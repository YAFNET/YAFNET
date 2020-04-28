<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.Profiler" Codebehind="Profiler.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                    LocalizedTag="TITLE" 
                                    LocalizedPage="ADMIN_PROFILER" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="diagnoses"
                                    LocalizedPage="ADMIN_PROFILER"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <YAF:WebProfiler runat="server"></YAF:WebProfiler>
                </div>
            </div>
        </div>
    </div>



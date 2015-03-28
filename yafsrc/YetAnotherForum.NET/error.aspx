<%@ Page Language="c#" AutoEventWireup="True" Inherits="YAF.Error" CodeBehind="error.aspx.cs" %>

<%@ Import Namespace="System.Security.Cryptography" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<script runat="server">
    void Page_Load()
    {
        var delay = new byte[1];
        RandomNumberGenerator prng = new RNGCryptoServiceProvider();

        prng.GetBytes(delay);
        Thread.Sleep(delay[0]);

        var disposable = prng.ToType<IDisposable>();
        disposable.Dispose();
    }
</script>
<!doctype html>
<html lang="en">
   <head runat="server">
       <title>Forum Error</title>
       <meta name="viewport" content="width=device-width, initial-scale=1.0">
       <link href="~/Content/bootstrap-forum.min.css" rel="stylesheet" runat="server" />
   </head>
   <body>
       <div class="jumbotron">
           <div class="container">
               <h1>Forum Error</h1>
           <div class="alert alert-danger">
               <asp:Label ID="ErrorMessage" Enabled="true" runat="server" />
           </div>
           <p>
               Please contact the administrator if this message persists.
           </p>
           <div class="alert alert-info">
               <span class="label label-primary">Note</span> 
               If you are the administrator, and need help with this problem, then Turn off <strong>CustomErrors</strong> in your <strong>web.config</strong>.
           </div>
           <p><a href="Default.aspx" class="btn btn-primary btn-lg"><i class="glyphicon glyphicon-home"></i> Try Again</a>
           </div>
           </div>
   </body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Gallery.aspx.cs" Inherits="SmartPhotoGallery.GalleryPage" %>
<!DOCTYPE html><html><head runat="server"><title>Gallery</title><link href="Content/site.css" rel="stylesheet" /></head>
<body><form id="form1" runat="server"><div class="top"><a href="Default.aspx">Home</a><h1><asp:Label ID="lblTitle" runat="server" /></h1></div>
<div class="wrap"><div class="photos"><asp:Repeater ID="rptPhotos" runat="server"><ItemTemplate>
<a href='<%# Eval("ImageUrl") %>' target="_blank"><img src='<%# Eval("ImageUrl") %>' alt="Gallery photo" /></a>
</ItemTemplate></asp:Repeater></div></div></form></body></html>

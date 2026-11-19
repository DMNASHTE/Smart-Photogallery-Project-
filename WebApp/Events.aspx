<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Events.aspx.cs" Inherits="SmartPhotoGallery.EventsPage" %>
<!DOCTYPE html><html><head runat="server"><title>Events</title><link href="Content/site.css" rel="stylesheet" /></head>
<body><form id="form1" runat="server"><div class="top"><a href="Default.aspx">Home</a><h1>Events - <asp:Label ID="lblYear" runat="server" /></h1></div>
<div class="wrap"><asp:Repeater ID="rptEvents" runat="server"><ItemTemplate>
<a class="card" href='Gallery.aspx?year=<%# Server.UrlEncode(Eval("YearName").ToString()) %>&event=<%# Server.UrlEncode(Eval("EventName").ToString()) %>'>
<img src='<%# Eval("PreviewUrl") %>' alt="Event preview" /><h3><%# Eval("EventName") %></h3><p><%# Eval("PhotoCount") %> photos</p></a>
</ItemTemplate></asp:Repeater></div></form></body></html>

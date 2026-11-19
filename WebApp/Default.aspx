<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="SmartPhotoGallery.DefaultPage" %>
<!DOCTYPE html>
<html><head runat="server"><title>Smart Photo Gallery</title><link href="Content/site.css" rel="stylesheet" /></head>
<body><form id="form1" runat="server"><div class="top"><h1>Smart Photo Gallery</h1>
<asp:TextBox ID="txtSearch" runat="server" placeholder="Search event name..." />
<asp:Button ID="btnSearch" runat="server" Text="Search events" OnClick="btnSearch_Click" />
<a href="KeywordManagement.aspx">Keyword management</a> | <a href="PersonManagement.aspx">People</a> | <a href="FaceSearch.aspx">Face search</a>
</div><div class="wrap"><h2>Browse by year</h2><asp:Repeater ID="rptYears" runat="server">
<ItemTemplate><a class="card" href='Events.aspx?year=<%# Server.UrlEncode(Eval("YearName").ToString()) %>'>
<img src='<%# Eval("PreviewUrl") %>' alt="Year preview" /><h3><%# Eval("YearName") %></h3>
<p><%# Eval("EventCount") %> events</p></a></ItemTemplate></asp:Repeater>
<asp:Label ID="lblMessage" runat="server" /></div></form></body></html>

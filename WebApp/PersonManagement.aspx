
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PersonManagement.aspx.cs" Inherits="SmartPhotoGallery.PersonManagementPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Person Management - Smart Photo Gallery</title>
    <link href="Content/site.css" rel="stylesheet" />

    <style>
        .person-card {
            max-width: 700px;
            margin: 30px auto;
            padding: 28px;
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 3px 18px rgba(0,0,0,.10);
        }

        .person-card h2 {
            margin-bottom: 22px;
        }

        .field {
            margin-bottom: 20px;
        }

        .field label {
            display: block;
            margin-bottom: 8px;
            font-weight: bold;
        }

        .field input {
            width: 100%;
            box-sizing: border-box;
            padding: 11px;
            border: 1px solid #ccc;
            border-radius: 6px;
        }

        .register-btn {
            padding: 12px 25px;
            border: none;
            border-radius: 6px;
            background: #2457a7;
            color: white;
            cursor: pointer;
        }

        .register-btn:hover {
            background: #173f80;
        }

        .status {
            display: block;
            margin-top: 18px;
            padding: 10px;
            border-radius: 6px;
            overflow-wrap: anywhere;
        }

        .hint {
            color: #666;
            font-size: 13px;
        }

        .connection-panel {
            background: #f1f6fc;
            border: 1px solid #d5e3f5;
            border-radius: 8px;
            padding: 18px;
            margin-bottom: 25px;
            line-height: 2.2;
        }

        .connection-panel h3 {
            margin-top: 0;
            color: #2457a7;
        }

        .connection-row {
            display: flex;
            justify-content: space-between;
            gap: 12px;
            border-bottom: 1px solid #e1e8f0;
        }

        .connection-row:last-child {
            border-bottom: none;
        }

        .connection-value {
            font-weight: bold;
        }

        .validation-error {
            color: #c62828;
            font-size: 13px;
        }

        .people-section {
            max-width: 1200px;
            margin: 30px auto;
            padding: 25px;
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 3px 18px rgba(0,0,0,.08);
        }

        .people-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 12px;
            margin-bottom: 20px;
        }

        .people-header h2 {
            margin: 0;
        }

        .count-label {
            color: #2457a7;
            font-weight: bold;
        }

        .table-scroll {
            width: 100%;
            overflow-x: auto;
        }

        .people-table {
            width: 100%;
            min-width: 750px;
            border-collapse: collapse;
        }

        .people-table th,
        .people-table td {
            padding: 13px;
            border-bottom: 1px solid #e5e7eb;
            text-align: left;
        }

        .people-table th {
            background: #f1f6fc;
            color: #2457a7;
            white-space: nowrap;
        }

        .people-table tr:hover {
            background: #f8fafc;
        }

        .people-message {
            display: block;
            margin-top: 15px;
            overflow-wrap: anywhere;
        }

        .empty-message {
            padding: 20px;
            text-align: center;
            color: #777;
        }

        .action-label {
            color: #888;
            font-size: 12px;
        }

        @media(max-width:700px) {
            .person-card,
            .people-section {
                margin: 15px 5px;
                padding: 18px;
            }
        }
    </style>
</head>

<body>

<form id="form1" runat="server" enctype="multipart/form-data">

    <div class="top">
        <a href="Default.aspx">Home</a>
        <h1>Person Management</h1>
    </div>

    <div class="wrap">

        <!-- REGISTRATION FORM -->
        <div class="person-card">

            <h2>Register New Person</h2>

            <!-- CONNECTION STATUS -->
            <div class="connection-panel">

                <h3>System Connection Status</h3>

                <div class="connection-row">
                    <span>Python AI Service</span>
                    <asp:Label ID="lblPythonStatus"
                        runat="server"
                        CssClass="connection-value"
                        Text="Checking..." />
                </div>

                <div class="connection-row">
                    <span>MySQL Database</span>
                    <asp:Label ID="lblDatabaseStatus"
                        runat="server"
                        CssClass="connection-value"
                        Text="Checking..." />
                </div>

                <div class="connection-row">
                    <span>Database Name</span>
                    <asp:Label ID="lblDatabaseName"
                        runat="server"
                        CssClass="connection-value"
                        Text="Checking..." />
                </div>

                <div class="connection-row">
                    <span>Face Model</span>
                    <asp:Label ID="lblModelStatus"
                        runat="server"
                        CssClass="connection-value"
                        Text="Not loaded yet" />
                </div>

            </div>

            <!-- PERSON NAME -->
            <div class="field">

                <label>Person Name *</label>

                <asp:TextBox ID="txtPersonName"
                    runat="server"
                    MaxLength="200"
                    placeholder="Enter person name" />

                <asp:RequiredFieldValidator
                    ID="rfvName"
                    runat="server"
                    ControlToValidate="txtPersonName"
                    ErrorMessage="Please enter person name."
                    CssClass="validation-error"
                    Display="Dynamic" />

            </div>

            <!-- DESIGNATION -->
            <div class="field">

                <label>Designation</label>

                <asp:TextBox ID="txtDesignation"
                    runat="server"
                    MaxLength="200"
                    placeholder="Enter designation" />

            </div>

            <!-- REFERENCE PHOTO -->
            <div class="field">

                <label>Reference Photo *</label>

                <asp:FileUpload
                    ID="fuReferencePhoto"
                    runat="server"
                    accept=".jpg,.jpeg,.png,.bmp" />

                <p class="hint">
                    Upload a clear photo containing exactly one visible face.
                    Maximum size: 10 MB.
                </p>

                <asp:CustomValidator
                    ID="cvPhoto"
                    runat="server"
                    ErrorMessage="Please select a reference photo."
                    CssClass="validation-error"
                    Display="Dynamic"
                    OnServerValidate="cvPhoto_ServerValidate" />

            </div>

            <!-- REGISTER BUTTON -->
            <asp:Button
                ID="btnRegister"
                runat="server"
                Text="Register Person"
                CssClass="register-btn"
                OnClick="btnRegister_Click" />

            <asp:Label
                ID="lblStatus"
                runat="server"
                CssClass="status"
                EnableViewState="false" />

        </div>

        <!-- REGISTERED PEOPLE TABLE -->
        <div class="people-section">

            <div class="people-header">

                <h2>Registered People</h2>

                <asp:Label
                    ID="lblTotalPersons"
                    runat="server"
                    CssClass="count-label"
                    Text="Total Registered People: 0" />

            </div>

            <div class="table-scroll">

                <asp:Repeater ID="rptPersons" runat="server">

                    <HeaderTemplate>
                        <table class="people-table">
                            <thead>
                                <tr>
                                    <th>Person ID</th>
                                    <th>Person Name</th>
                                    <th>Designation</th>
                                    <th>Registered Date</th>
                                    <th>Matched Photos</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>

                    <ItemTemplate>
                        <tr>

                            <td>
                                <%#: Eval("PersonId") %>
                            </td>

                            <td>
                                <%#: Eval("PersonName") %>
                            </td>

                            <td>
                                <%#: Eval("Designation") %>
                            </td>

                            <td>
                                <%#: Eval("CreatedAt") %>
                            </td>

                            <td>
                                <%#: Eval("MatchedPhotos") %>
                            </td>

                            <td>
                                <span class="action-label">
                                    Edit and Delete coming next
                                </span>
                            </td>

                        </tr>
                    </ItemTemplate>

                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>

                </asp:Repeater>

            </div>

            <asp:Label
                ID="lblPeopleMessage"
                runat="server"
                CssClass="people-message" />

        </div>

    </div>

</form>

</body>
</html>

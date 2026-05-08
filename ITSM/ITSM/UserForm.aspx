<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserForm.aspx.cs" Inherits="ITSM.UserForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-label {
            font-weight: 600;
            color: #333;
            text-transform: uppercase;
            font-size: 0.85rem;
            margin-bottom: 0.5rem;
            display: block;
        }

        .subheader-title {
            color: #0F406B;
            font-weight: bold;
            margin-bottom: 20px;
        }

        .section-title {
            border-bottom: 2px solid #0F406B;
            margin: 25px 0 15px 0;
            padding-bottom: 5px;
            color: #0F406B;
            font-size: 0.95rem;
            font-weight: bold;
        }

        .main-container {
            padding: 30px;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            margin-bottom: 50px;
        }

        .footer-buttons {
            padding-top: 30px;
            margin-top: 30px;
            border-top: 1px solid #eee;
            display: flex;
            justify-content: flex-end;
            gap: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfID" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertType" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertMessage" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertTitle" runat="server" Value="" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" Value="" />

    <div class="subheader">
        <h1 class="subheader-title">
            <asp:Label ID="lblPageTitle" runat="server" Text="USER MANAGEMENT FORM"></asp:Label>
        </h1>
    </div>

    <div class="main-container">
        <div class="section-title">ACCOUNT DETAILS</div>

        <div class="row">
            <div class="col-md-4 mb-3">
                <label class="form-label">Employee ID</label>
                <asp:TextBox ID="txtEmployeeID" runat="server" CssClass="form-control" MaxLength="50" placeholder="Optional"></asp:TextBox>
            </div>

            <div class="col-md-4 mb-3">
                <label class="form-label">Name & Surname <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtNameSurname" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtNameSurname"
                    ErrorMessage="Name Surname is required" CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveGroup"></asp:RequiredFieldValidator>
            </div>

            <div class="col-md-4 mb-3">
                <label class="form-label">Username <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" MaxLength="30"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="txtUserName"
                    ErrorMessage="Username is required" CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveGroup"></asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Email Address <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="70" TextMode="Email"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="Email is required" CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveGroup"></asp:RequiredFieldValidator>
            </div>

            <div class="col-md-6 mb-3">
                <label class="form-label">Password <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtPass" runat="server" CssClass="form-control" MaxLength="30" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPass" runat="server" ControlToValidate="txtPass"
                    ErrorMessage="Password is required" CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveGroup"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">Start Date</label>
                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">End Date</label>
                <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
        </div>

        <div class="section-title">JOB & ROLE INFORMATION</div>
        <div class="row">
            <div class="col-md-4 mb-3">
                <label class="form-label">Department</label>
                <asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Title</label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">User Type</label>
                <asp:TextBox ID="txtUserType" runat="server" CssClass="form-control" MaxLength="50" placeholder="e.g. Admin, User, Manager"></asp:TextBox>
            </div>
        </div>

        <div class="section-title">SYSTEM STATUS</div>
        <div class="row">
            <div class="col-md-4 mb-3">
                <label class="form-label">Status <span class="text-danger">*</span></label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Active" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Passive" Value="0"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        <div class="footer-buttons">
            <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-secondary px-5" OnClick="btnCancel_Click" CausesValidation="false">
                <i class="fal fa-arrow-left mr-1"></i> BACK TO LIST
            </asp:LinkButton>
            <asp:LinkButton ID="btnSave" runat="server" CssClass="btn px-5" OnClick="btnSave_Click" ValidationGroup="SaveGroup" Style="background-color: #0F406B; border-color: #0F406B; color: white;">
                <i class="fal fa-save mr-1"></i> SAVE USER
            </asp:LinkButton>
        </div>
    </div>

    <script type="text/javascript">
        window.onload = function () {
            var alertType = document.getElementById('<%= hfAlertType.ClientID %>').value;
            var alertMessage = document.getElementById('<%= hfAlertMessage.ClientID %>').value;
            var alertTitle = document.getElementById('<%= hfAlertTitle.ClientID %>').value;
            var redirectUrl = document.getElementById('<%= hfRedirectUrl.ClientID %>').value;

            if (alertType && alertMessage) {
                Swal.fire({
                    icon: alertType,
                    title: alertTitle,
                    text: alertMessage,
                    confirmButtonText: 'OK',
                    confirmButtonColor: '#886ab5'
                }).then((result) => {
                    if (redirectUrl && alertType === 'success') {
                        window.location.href = redirectUrl;
                    }

                    document.getElementById('<%= hfAlertType.ClientID %>').value = '';
                    document.getElementById('<%= hfAlertMessage.ClientID %>').value = '';
                    document.getElementById('<%= hfAlertTitle.ClientID %>').value = '';
                    document.getElementById('<%= hfRedirectUrl.ClientID %>').value = '';
                });
            }
        };
    </script>
</asp:Content>

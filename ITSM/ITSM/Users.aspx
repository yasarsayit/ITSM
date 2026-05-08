<%@ Page Title="User Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="ITSM.Users" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .no-hover tbody tr:hover {
            background-color: transparent !important;
            color: inherit !important;
        }

        .action-buttons {
            display: flex;
            gap: 5px;
            justify-content: center;
        }

            .action-buttons i {
                pointer-events: none;
            }

        .pagination .page-item.active .page-link {
            background-color: #0F406B;
            border-color: #0F406B;
            color: white;
        }

        .pagination .page-link {
            color: #0F406B;
        }

        .arama td {
            padding: 4px 5px !important;
            background-color: #f1f5f9 !important;
        }

        .frmara {
            font-size: 11px;
        }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid" style="padding: 20px;">
        <div class="card shadow">
            <div class="card-header d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white;">
                <h3 class="card-title mb-0">User List</h3>
                <a href="UserForm.aspx" class="btn btn-light btn-sm"><i class="fa fa-plus"></i>Add New User</a>
            </div>

            <div class="card-body">

                <div class="d-flex justify-content-between align-items-center mb-3">
                    <div style="font-size: 13px; font-weight: 600; color: #555;">
                        Show 
                        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="form-select form-select-sm d-inline-block mx-1" Style="width: auto;">
                            <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="25" Value="25"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                            <asp:ListItem Text="100" Value="100"></asp:ListItem>
                        </asp:DropDownList>
                        entries
                    </div>
                </div>

                <div class="table-responsive">
                    <table class="table table-sm table-bordered align-middle no-hover" style="font-size: 12px;">
                        <thead class="table-light text-nowrap align-middle">
                            <tr>
                                <th style="width: 80px; text-align: center;">UserID</th>
                                <th>Employee ID</th>
                                <th>Name Surname</th>
                                <th>UserName</th>
                                <th>Email</th>
                                <th>Department</th>
                                <th>Title</th>
                                <th>Status</th>
                                <th>User Type</th>
                                <th style="width: 100px; text-align: center;">Actions</th>
                            </tr>
                            <tr class="arama">
                                <td></td>
                                <td>
                                    <asp:TextBox ID="txtFilterEmployeeID" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search..."></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search..."></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtFilterUser" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search..."></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtFilterEmail" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search..."></asp:TextBox></td>
                                <td>
                                    <asp:DropDownList ID="ddlFilterDept" runat="server" CssClass="form-select form-select-sm frmara"></asp:DropDownList></td>
                                <td>
                                    <asp:TextBox ID="txtFilterTitle" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search..."></asp:TextBox></td>
                                <td>
                                    <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-select form-select-sm frmara">
                                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Active" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlFilterType" runat="server" CssClass="form-select form-select-sm frmara"></asp:DropDownList></td>
                                <td style="text-align: center;">
                                    <div class="btn-group btn-group-sm gridsearchgrp">
                                        <asp:LinkButton ID="lbAraTemizle" runat="server" CssClass="btn btn-warning btn-sm gridsearchbtn" OnClick="lbAraTemizle_Click" title="Clear Filters"><i class="fas fa-broom"></i></asp:LinkButton>
                                        <asp:LinkButton ID="lbAra" runat="server" CssClass="btn btn-primary btn-sm gridsearchbtn" OnClick="lbAra_Click" title="Search"><i class="fas fa-search"></i></asp:LinkButton>
                                    </div>
                                </td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptUsers" runat="server" OnItemCommand="rptUsers_ItemCommand">
                                <ItemTemplate>
                                    <tr class="text-nowrap">
                                        <td class="fw-bold text-center"><%# Eval("UserID") %></td>

                                        <td class="text-primary fw-bold"><%# Eval("EmployeeID") %></td>

                                        <td><strong><%# Eval("NameSurname") %></strong></td>
                                        <td><%# Eval("UserName") %></td>
                                        <td><%# Eval("Email") %></td>
                                        <td><%# Eval("Department") %></td>
                                        <td><%# Eval("Title") %></td>
                                        <td><%# Eval("StatusText") %></td>
                                        <td><%# Eval("UserType") %></td>
                                        <td class="text-center">
                                            <div class="action-buttons">
                                                <asp:LinkButton ID="lbDevices" CssClass="btn btn-outline-info btn-sm" runat="server"
                                                    CommandName="ViewDevices" CommandArgument='<%# Eval("UserID") %>' title="User Devices">
                                                    <i class="fal fa-user-tag"></i>
                                                </asp:LinkButton>

                                                <button type="button" class="btn btn-outline-danger btn-sm btn-delete-trigger"
                                                    data-id='<%# Eval("UserID") %>'
                                                    data-name='<%# Eval("NameSurname").ToString().Replace("'", "").Replace("\"", "") %>'>
                                                    <i class="fal fa-trash-alt"></i>
                                                </button>

                                                <asp:LinkButton ID="lbEdit" CssClass="btn btn-outline-success btn-sm" runat="server"
                                                    CommandName="Edit" CommandArgument='<%# Eval("UserID") %>'>
                                                    <i class="fal fa-edit"></i>
                                                </asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                            <tr id="trNoData" runat="server" visible="false">
                                <td colspan="10" class="text-center text-muted p-4">
                                    <i class="fal fa-search fa-2x mb-2"></i>
                                    <br />
                                    No users found matching your criteria.
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <div class="d-flex justify-content-between align-items-center mt-3">
                    <div class="text-muted" style="font-size: 13px;">
                        <asp:Label ID="lblPageInfo" runat="server"></asp:Label>
                    </div>
                    <div>
                        <ul class="pagination pagination-sm mb-0">
                            <li class="page-item" id="liPrev" runat="server">
                                <asp:LinkButton ID="lbPrev" runat="server" CssClass="page-link" OnClick="lbPrev_Click">&laquo; Prev</asp:LinkButton>
                            </li>
                            <asp:Repeater ID="rptPagination" runat="server" OnItemCommand="rptPagination_ItemCommand">
                                <ItemTemplate>
                                    <li class='page-item <%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "" %>'>
                                        <asp:LinkButton ID="lbPage" runat="server" CssClass="page-link" CommandName="Page" CommandArgument='<%# Eval("PageNumber") %>'><%# Eval("PageNumber") %></asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="page-item" id="liNext" runat="server">
                                <asp:LinkButton ID="lbNext" runat="server" CssClass="page-link" OnClick="lbNext_Click">Next &raquo;</asp:LinkButton>
                            </li>
                        </ul>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfDeleteId" runat="server" ClientIDMode="Static" />
    <asp:Button ID="btnDeleteConfirm" runat="server" OnClick="btnDeleteConfirm_Click" Style="display: none;" ClientIDMode="Static" />

    <asp:HiddenField ID="hfAlertType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfAlertMessage" runat="server" ClientIDMode="Static" />

    <script type="text/javascript">
        document.addEventListener('click', function (e) {
            const deleteBtn = e.target.closest('.btn-delete-trigger');
            if (deleteBtn) {
                const id = deleteBtn.getAttribute('data-id');
                const name = deleteBtn.getAttribute('data-name');

                Swal.fire({
                    title: 'Are you sure?',
                    text: '"' + name + '" will be deleted permanently!',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#3085d6',
                    confirmButtonText: 'Yes, Delete!',
                    cancelButtonText: 'Cancel'
                }).then((result) => {
                    if (result.isConfirmed) {
                        document.getElementById('hfDeleteId').value = id;
                        document.getElementById('btnDeleteConfirm').click();
                    }
                });
            }
        });

        window.onload = function () {
            const type = document.getElementById('hfAlertType').value;
            const msg = document.getElementById('hfAlertMessage').value;
            if (type && msg) {
                Swal.fire({ icon: type, title: type.toUpperCase(), text: msg });
                document.getElementById('hfAlertType').value = '';
                document.getElementById('hfAlertMessage').value = '';
            }
        };
    </script>
</asp:Content>

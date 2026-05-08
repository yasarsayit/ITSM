<%@ Page Title="Software Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Software.aspx.cs" Inherits="ITSM.Software" %>

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
                <h3 class="card-title mb-0">Software List</h3>
                <a href="SoftwareForm.aspx" class="btn btn-light btn-sm"><i class="fa fa-plus"></i>Add New Software</a>
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
                    <table class="table table-sm table-bordered align-middle no-hover" style="font-size: 11px;">
                        <thead class="table-light text-nowrap">
                            <tr>
                                <th style="width: 60px; text-align: center;">ID</th>
                                <th>Description</th>
                                <th>License No</th>
                                <th>Software Type</th>
                                <th>Purchase Date</th>
                                <th>Expiry Date</th>
                                <th>Assigned User</th>
                                <th>Device No</th>
                                <th>Acc. Code</th>
                                <th style="width: 100px; text-align: center;">Actions</th>
                            </tr>

                            <tr class="arama">
                                <td></td>
                                <td>
                                    <asp:TextBox ID="txtFilterDesc" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search desc..."></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtFilterLicense" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search license..."></asp:TextBox></td>
                                <td>
                                    <asp:DropDownList ID="ddlFilterType" runat="server" CssClass="form-select form-select-sm frmara"></asp:DropDownList></td>
                                <td>
                                    <asp:TextBox ID="txtFilterPurDate" runat="server" CssClass="form-control form-control-sm frmara" TextMode="Date"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtFilterExpDate" runat="server" CssClass="form-control form-control-sm frmara" TextMode="Date"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtFilterUser" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search user..."></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtFilterDevice" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search device..."></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtFilterAccCode" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search code..."></asp:TextBox></td>
                                <td style="text-align: center;">
                                    <div class="btn-group btn-group-sm gridsearchgrp">
                                        <asp:LinkButton ID="lbAraTemizle" runat="server" CssClass="btn btn-warning btn-sm gridsearchbtn" OnClick="lbAraTemizle_Click" title="Clear Filters"><i class="fas fa-broom"></i></asp:LinkButton>
                                        <asp:LinkButton ID="lbAra" runat="server" CssClass="btn btn-primary btn-sm gridsearchbtn" OnClick="lbAra_Click" title="Search"><i class="fas fa-search"></i></asp:LinkButton>
                                    </div>
                                </td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptSoftware" runat="server" OnItemCommand="rptSoftware_ItemCommand">
                                <ItemTemplate>
                                    <tr class="text-nowrap">
                                        <td class="fw-bold text-center"><%# Eval("SoftwareID") %></td>
                                        <td><strong><%# Eval("Description") %></strong></td>
                                        <td><%# Eval("LicenseNumber") %></td>
                                        <td><%# Eval("SoftwareType") %></td>
                                        <td><%# Eval("PurchaseDate", "{0:dd.MM.yyyy}") %></td>
                                        <td><%# Eval("ExpiryDate", "{0:dd.MM.yyyy}") %></td>
                                        <td><%# Eval("AssignedUser") %></td>
                                        <td><%# Eval("DeviceNo") %></td>
                                        <td><%# Eval("AccountingCode") %></td>
                                        <td class="text-center">
                                            <div class="action-buttons">
                                                <asp:HyperLink ID="lbShowDevices" runat="server"
                                                    NavigateUrl='<%# "SoftwareToDevice.aspx?id=" + Eval("SoftwareID") %>'
                                                    CssClass="btn btn-outline-info btn-sm"
                                                    ToolTip="Show Devices">
                                                    <i class="fal fa-laptop-code"></i>
                                                </asp:HyperLink>

                                                <button type="button" class="btn btn-outline-danger btn-sm btn-delete-trigger"
                                                    data-id='<%# Eval("SoftwareID") %>'
                                                    data-name='<%# Eval("Description").ToString().Replace("'", "").Replace("\"", "") %>'>
                                                    <i class="fal fa-trash-alt"></i>
                                                </button>

                                                <asp:LinkButton ID="lbEdit" CssClass="btn btn-outline-success btn-sm" runat="server"
                                                    CommandName="Edit" CommandArgument='<%# Eval("SoftwareID") %>'>
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
                                    No software records found matching your criteria.
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

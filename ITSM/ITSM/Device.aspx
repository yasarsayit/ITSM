<%@ Page Title="Device Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Device.aspx.cs" Inherits="ITSM.Device" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .action-buttons {
            display: flex;
            gap: 5px;
            justify-content: center;
        }

            .action-buttons .btn {
                display: inline-flex;
                align-items: center;
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

    <div class="container-fluid" style="padding: 20px;">

        <div class="card shadow mb-5">
            <div class="card-header d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white;">
                <h3 class="card-title mb-0" style="font-size: 16px;"><i class="fal fa-laptop me-2"></i>Main Device List</h3>
                <asp:HyperLink ID="lnkAddNew" runat="server" NavigateUrl="DeviceForm.aspx" CssClass="btn btn-light btn-sm">
                    <i class="fal fa-plus"></i> Add New Device
                </asp:HyperLink>
            </div>

            <div class="card-body">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <div style="font-size: 13px; font-weight: 600; color: #555;">
                        Show 
                        <asp:DropDownList ID="ddlPageSize1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize1_SelectedIndexChanged" CssClass="form-select form-select-sm d-inline-block mx-1" Style="width: auto;">
                            <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="25" Value="25"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                        </asp:DropDownList>
                        entries
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:Repeater ID="rptDevices" runat="server" OnItemCommand="rptDevices_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-sm table-bordered table-hover align-middle">
                                <thead class="table-light text-nowrap">
                                    <tr>
                                        <th>Brand</th>
                                        <th>Model</th>
                                        <th>Serial Number</th>
                                        <th>Device Type</th>
                                        <th>Device Name / Network Name</th>
                                        <th>Linked Add Devices</th>
                                        <th style="width: 100px; text-align: center;">Actions</th>
                                    </tr>
                                    <tr class="arama">
                                        <td>
                                            <asp:TextBox ID="txtF1Brand" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Brand..."></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtF1Model" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Model..."></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtF1Serial" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Serial..."></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtF1Type" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Type..."></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtF1ItemName" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search device name / network name..."></asp:TextBox></td>
                                        <td></td>
                                        <td style="text-align: center;">
                                            <div class="btn-group btn-group-sm gridsearchgrp">
                                                <asp:LinkButton ID="lbAraTemizle1" runat="server" CssClass="btn btn-warning btn-sm gridsearchbtn" OnClick="lbAraTemizle1_Click" title="Clear Filters"><i class="fas fa-broom"></i></asp:LinkButton>
                                                <asp:LinkButton ID="lbAra1" runat="server" CssClass="btn btn-primary btn-sm gridsearchbtn" OnClick="lbAra1_Click" title="Search"><i class="fas fa-search"></i></asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Brand") %></td>
                                <td><%# Eval("Model") %></td>
                                <td><%# Eval("SerialNumber") %></td>
                                <td><%# Eval("DeviceTypeText") %></td>
                                <td><%# Eval("ItemName") %></td>
                                <td>
                                    <div class="d-flex flex-column">

                                        <div style="font-size: 0.75rem; color: #666; line-height: 1.2;">
                                            <i class="fal fa-link text-primary"></i>
                                            <strong>Connected:</strong> <%# Eval("LinkedAdditionalDevices") %>
                                        </div>
                                    </div>
                                </td>
                                <td class="action-buttons">
                                    <asp:LinkButton ID="lbEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("DeviceID") %>' CssClass="btn btn-outline-success btn-sm">
                                        <i class="fal fa-edit"></i>
                                    </asp:LinkButton>
                                    <button type="button" class="btn btn-outline-danger btn-sm btn-delete-trigger"
                                        data-id='<%# Eval("DeviceID") %>'
                                        data-name='<%# Eval("Brand").ToString() + " " + Eval("Model").ToString() %>'>
                                        <i class="fal fa-trash-alt"></i>
                                    </button>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <tr id="trNoData1" runat="server" visible="false">
                                <td colspan="7" class="text-center text-muted p-4">No records found.</td>
                            </tr>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="d-flex justify-content-between align-items-center mt-3">
                    <div class="text-muted" style="font-size: 13px;">
                        <asp:Label ID="lblPageInfo1" runat="server"></asp:Label>
                    </div>
                    <div>
                        <ul class="pagination pagination-sm mb-0">
                            <li class="page-item" id="liPrev1" runat="server">
                                <asp:LinkButton ID="lbPrev1" runat="server" CssClass="page-link" OnClick="lbPrev1_Click">&laquo; Prev</asp:LinkButton>
                            </li>
                            <asp:Repeater ID="rptPagination1" runat="server" OnItemCommand="rptPagination1_ItemCommand">
                                <ItemTemplate>
                                    <li class='page-item <%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "" %>'>
                                        <asp:LinkButton ID="lbPage" runat="server" CssClass="page-link" CommandName="Page" CommandArgument='<%# Eval("PageNumber") %>'><%# Eval("PageNumber") %></asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="page-item" id="liNext1" runat="server">
                                <asp:LinkButton ID="lbNext1" runat="server" CssClass="page-link" OnClick="lbNext1_Click">Next &raquo;</asp:LinkButton>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>



        <div class="card shadow mb-4">
            <div class="card-header d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white;">
                <h3 class="card-title mb-0" style="font-size: 16px;"><i class="fal fa-user-tag me-2"></i>Active Assigned Device List</h3>
            </div>
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <div style="font-size: 13px; font-weight: 600; color: #555;">
                        Show 
                        <asp:DropDownList ID="ddlPageSize3" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize3_SelectedIndexChanged" CssClass="form-select form-select-sm d-inline-block mx-1" Style="width: auto;">
                            <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="25" Value="25"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                        </asp:DropDownList>
                        entries
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:Repeater ID="rptAssignedDevices" runat="server">
                        <HeaderTemplate>
                            <table class="table table-sm table-bordered table-hover align-middle">
                                <thead class="table-light text-nowrap">
                                    <tr>
                                        <th>Personnel Name</th>
                                        <th>Device & Serial Number</th>
                                        <th>Assignment Date</th>
                                        <th style="width: 80px;"></th>
                                    </tr>
                                    <tr class="arama">
                                        <td>
                                            <asp:TextBox ID="txtF3User" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Personnel..."></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtF3Device" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Device..."></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtF3Date" runat="server" CssClass="form-control form-control-sm frmara" TextMode="Date"></asp:TextBox></td>
                                        <td style="text-align: center;">
                                            <div class="btn-group btn-group-sm gridsearchgrp">
                                                <asp:LinkButton ID="lbAraTemizle3" runat="server" CssClass="btn btn-warning btn-sm gridsearchbtn" OnClick="lbAraTemizle3_Click" title="Clear Filters"><i class="fas fa-broom"></i></asp:LinkButton>
                                                <asp:LinkButton ID="lbAra3" runat="server" CssClass="btn btn-primary btn-sm gridsearchbtn" OnClick="lbAra3_Click" title="Search"><i class="fas fa-search"></i></asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><strong><%# Eval("NameSurname") %></strong></td>
                                <td><%# Eval("DeviceFullInfo") %></td>
                                <td><%# Eval("StartDate", "{0:dd.MM.yyyy}") %></td>
                                <td></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <tr id="trNoData3" runat="server" visible="false">
                                <td colspan="4" class="text-center text-muted p-4">No records found.</td>
                            </tr>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="d-flex justify-content-between align-items-center mt-3">
                    <div class="text-muted" style="font-size: 13px;">
                        <asp:Label ID="lblPageInfo3" runat="server"></asp:Label>
                    </div>
                    <div>
                        <ul class="pagination pagination-sm mb-0">
                            <li class="page-item" id="liPrev3" runat="server">
                                <asp:LinkButton ID="lbPrev3" runat="server" CssClass="page-link" OnClick="lbPrev3_Click">&laquo; Prev</asp:LinkButton>
                            </li>
                            <asp:Repeater ID="rptPagination3" runat="server" OnItemCommand="rptPagination3_ItemCommand">
                                <ItemTemplate>
                                    <li class='page-item <%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "" %>'>
                                        <asp:LinkButton ID="lbPage" runat="server" CssClass="page-link" CommandName="Page" CommandArgument='<%# Eval("PageNumber") %>'><%# Eval("PageNumber") %></asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="page-item" id="liNext3" runat="server">
                                <asp:LinkButton ID="lbNext3" runat="server" CssClass="page-link" OnClick="lbNext3_Click">Next &raquo;</asp:LinkButton>
                            </li>
                        </ul>
                    </div>
                </div>

            </div>
        </div>

    </div>

    <asp:HiddenField ID="hfDeleteId" runat="server" />
    <asp:Button ID="btnDeleteConfirm" runat="server" Style="display: none" OnClick="btnDeleteConfirm_Click" />


    <script type="text/javascript">
        document.addEventListener('click', function (e) {

            const deleteBtn = e.target.closest('.btn-delete-trigger');
            if (deleteBtn) {
                const id = deleteBtn.getAttribute('data-id');
                const name = deleteBtn.getAttribute('data-name');

                Swal.fire({
                    title: 'Are you sure?',
                    text: '"' + name + '" will be deleted!',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    confirmButtonText: 'Yes, Delete!'
                }).then((result) => {
                    if (result.isConfirmed) {
                        document.getElementById('<%= hfDeleteId.ClientID %>').value = id;
                        document.getElementById('<%= btnDeleteConfirm.ClientID %>').click();
                    }
                });
            }



        });
    </script>
</asp:Content>

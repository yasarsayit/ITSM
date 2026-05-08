<%@ Page Title="Additional Devices" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AddDevices.aspx.cs" Inherits="ITSM.AddDevice" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .pagination .page-item.active .page-link { background-color: #0F406B; border-color: #0F406B; color: white; }
        .pagination .page-link { color: #0F406B; }
        .arama td { padding: 4px 5px !important; background-color: #f1f5f9 !important; }
        .frmara { font-size: 11px; }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <div class="subheader" style="display: none !important;">
        <h1 class="subheader-title">ADDITIONAL DEVICES MANAGEMENT</h1>
    </div>

    <div class="container-fluid" style="padding: 20px;">
        <div class="card shadow mb-5">
            <div class="card-header d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white;">
                <h3 class="card-title mb-0" style="font-size: 16px;"><i class="fal fa-keyboard me-2"></i>Additional Device List</h3>
                <a href="AddDeviceForm.aspx" class="btn btn-light btn-sm shadow-sm">
                    <i class="fal fa-plus-circle mr-1"></i> Add New Additional Device
                </a>
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
                        </asp:DropDownList>
                        entries
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:Repeater ID="rptAdditionalDevices" runat="server" OnItemCommand="rptAdditionalDevices_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-sm table-bordered table-hover align-middle">
                                <thead class="table-light text-nowrap">
                                    <tr>
                                        <th>Type</th>
                                        <th>Description</th>
                                        <th>Linked Main Device</th>
                                        <th>Accounting Code</th>
                                        <th class="text-center" style="width: 100px;">Actions</th>
                                    </tr>
                                    <tr class="arama">
                                        <td><asp:TextBox ID="txtFType" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Type..."></asp:TextBox></td>
                                        <td><asp:TextBox ID="txtFDesc" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Description..."></asp:TextBox></td>
                                        <td></td>
                                        <td><asp:TextBox ID="txtFAcc" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Accounting..."></asp:TextBox></td>
                                        <td style="text-align: center;">
                                            <div class="btn-group btn-group-sm">
                                                <asp:LinkButton ID="lbAraTemizle" runat="server" CssClass="btn btn-warning btn-sm" OnClick="lbAraTemizle_Click" title="Clear Filters"><i class="fas fa-broom"></i></asp:LinkButton>
                                                <asp:LinkButton ID="lbAra" runat="server" CssClass="btn btn-primary btn-sm" OnClick="lbAra_Click" title="Search"><i class="fas fa-search"></i></asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("DeviceType") %></td>
                                <td><strong><%# Eval("Description") %></strong></td>
                                <td>
                                    <div style="font-size: 0.8rem; color: #555;">
                                        <i class="fal fa-link text-primary"></i> <%# Eval("MainDeviceName") %>
                                    </div>
                                </td>
                                <td><%# Eval("AccountingCode") %></td>
                                <td class="text-center">
                                    <asp:LinkButton ID="lbEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("AD_ID") %>' CssClass="btn btn-outline-success btn-sm">
                                        <i class="fal fa-edit"></i>
                                    </asp:LinkButton>
                                    <button type="button" class="btn btn-outline-danger btn-sm" onclick="confirmDelete('<%# Eval("AD_ID") %>', '<%# Eval("Description") %>')">
                                        <i class="fal fa-trash-alt"></i>
                                    </button>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <tr id="trNoData" runat="server" visible="false">
                                <td colspan="5" class="text-center text-muted p-4">No records found.</td>
                            </tr>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="d-flex justify-content-between align-items-center mt-3">
                    <div class="text-muted" style="font-size: 13px;"><asp:Label ID="lblPageInfo" runat="server"></asp:Label></div>
                    <div>
                        <ul class="pagination pagination-sm mb-0">
                            <li class="page-item" id="liPrev" runat="server"><asp:LinkButton ID="lbPrev" runat="server" CssClass="page-link" OnClick="lbPrev_Click">&laquo; Prev</asp:LinkButton></li>
                            <asp:Repeater ID="rptPagination" runat="server" OnItemCommand="rptPagination_ItemCommand">
                                <ItemTemplate>
                                    <li class='page-item <%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "" %>'>
                                        <asp:LinkButton ID="lbPage" runat="server" CssClass="page-link" CommandName="Page" CommandArgument='<%# Eval("PageNumber") %>'><%# Eval("PageNumber") %></asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="page-item" id="liNext" runat="server"><asp:LinkButton ID="lbNext" runat="server" CssClass="page-link" OnClick="lbNext_Click">Next &raquo;</asp:LinkButton></li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfDeleteId" runat="server" Value="" />
    <asp:Button ID="btnDeleteConfirm" runat="server" CssClass="d-none" OnClick="btnDeleteConfirm_Click" />
    <asp:HiddenField ID="hfAlertType" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertMessage" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertTitle" runat="server" Value="" />

    <script type="text/javascript">
        function confirmDelete(id, name) {
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

        window.onload = function () {
            var type = document.getElementById('<%= hfAlertType.ClientID %>').value;
            var msg = document.getElementById('<%= hfAlertMessage.ClientID %>').value;
            var title = document.getElementById('<%= hfAlertTitle.ClientID %>').value;
            if (type && msg) {
                Swal.fire({ icon: type, title: title, text: msg }).then(() => {
                    document.getElementById('<%= hfAlertType.ClientID %>').value = '';
                    document.getElementById('<%= hfAlertMessage.ClientID %>').value = '';
                    document.getElementById('<%= hfAlertTitle.ClientID %>').value = '';
                });
            }
        };
    </script>
</asp:Content>
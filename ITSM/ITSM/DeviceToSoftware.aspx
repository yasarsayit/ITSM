<%@ Page Title="Software - Device Assignment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DeviceToSoftware.aspx.cs" Inherits="ITSM.DeviceToSoftware" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid" style="padding: 20px;">

        <div class="card shadow mb-4">
            <div class="card-header d-flex justify-content-between align-items-center bg-primary-500 color-white">
                <h3 class="card-title mb-0">Software Assignment</h3>
                <a href="Device.aspx" class="btn btn-light btn-sm">
                    <i class="fa fa-arrow-left"></i>Back to List
                </a>
            </div>
            <div class="card-body bg-light">
                <div class="row g-2 align-items-end">
                    <asp:HiddenField ID="hfSelectedID" runat="server" />
                    <div class="col-md-4">
                        <label class="form-label fw-bold" style="font-size: 11px;">Select Software</label>
                        <asp:DropDownList ID="ddlSoftware" runat="server" CssClass="form-select form-select-sm" Style="font-size: 11px;"></asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-bold" style="font-size: 11px;">Start Date</label>
                        <asp:TextBox ID="txtStartDate" runat="server" TextMode="DateTimeLocal" CssClass="form-control form-control-sm" Style="font-size: 11px;"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-bold" style="font-size: 11px;">End Date</label>
                        <asp:TextBox ID="txtEndDate" runat="server" TextMode="DateTimeLocal" CssClass="form-control form-control-sm" Style="font-size: 11px;"></asp:TextBox>
                    </div>
                    <div class="col-md-2 text-center">

                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-outline-primary btn-sm" OnClick="btnSave_Click"
                            Style="width: 40px; height: 31px; display: inline-flex; align-items: center; justify-content: center;">
                            <i class="fal fa-save" style="font-size: 16px;"></i>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

        <div class="card shadow">
            <div class="card-header bg-primary-500 color-white">
                <h5 class="mb-0">Software-Device List</h5>
            </div>
            <div class="card-body">
                <asp:PlaceHolder ID="phNoSoftware" runat="server" Visible="false">
                    <div class="alert alert-warning" style="font-size: 11px;">
                        <i class="fa fa-info-circle"></i>No software assigned to this device yet.
                    </div>
                </asp:PlaceHolder>

                <asp:Panel ID="pnlSoftwareDetails" runat="server" Visible="false">
                    <div class="table-responsive">
                        <table class="table table-sm table-bordered table-hover align-middle" style="font-size: 11px;">
                            <thead class="table-light text-nowrap">
                                <tr>
                                    <th>Software Information (Name / Type / License)</th>
                                    <th>Start Date</th>
                                    <th>End Date</th>
                                    <th style="width: 100px;">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptSoftware" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-nowrap">
                                                <strong style="color: #444;"><%# Eval("Description") %></strong>
                                                <small class="text-muted">
                                                    <%# !string.IsNullOrEmpty(Eval("SoftwareType").ToString()) ? " | " + Eval("SoftwareType") : "" %>
                                                    <%# !string.IsNullOrEmpty(Eval("LicenseType").ToString()) ? " | " + Eval("LicenseType") : "" %>
                                                </small>
                                            </td>
                                            <td class="text-nowrap"><%# Eval("StartDate", "{0:dd/MM/yyyy HH:mm}") %></td>
                                            <td class="text-nowrap"><%# Eval("EndDate", "{0:dd/MM/yyyy HH:mm}") %></td>
                                            <td class="text-center">
                                                <div class="action-buttons d-flex justify-content-center gap-1">
                                                    <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%# Eval("DeviceSoftwareID") %>' OnClick="btnEdit_Click" CssClass="btn btn-outline-success btn-sm">
                                                        <i class="fal fa-edit"></i>
                                                    </asp:LinkButton>
                                                    <button type="button" class="btn btn-outline-danger btn-sm" onclick="ConfirmSoftwareDelete(<%# Eval("DeviceSoftwareID") %>)">
                                                        <i class="fal fa-trash-alt"></i>
                                                    </button>
                                                </div>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>


    <asp:HiddenField ID="hfDeleteId" runat="server" />
    <asp:Button ID="btnDeleteConfirm" runat="server" OnClick="btnDeleteConfirm_Click" Style="display: none;" />

    <asp:HiddenField ID="hfAlertType" runat="server" />
    <asp:HiddenField ID="hfAlertMessage" runat="server" />
    <asp:HiddenField ID="hfAlertTitle" runat="server" />

    <script type="text/javascript">
        function ConfirmSoftwareDelete(id) {
            Swal.fire({
                title: 'Are you sure?',
                text: "This software assignment will be removed!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, Delete!',
                cancelButtonText: 'Cancel'
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
                Swal.fire({ icon: type, title: title, text: msg });
                document.getElementById('<%= hfAlertType.ClientID %>').value = '';
                document.getElementById('<%= hfAlertMessage.ClientID %>').value = '';
                document.getElementById('<%= hfAlertTitle.ClientID %>').value = '';
            }
        };
    </script>
</asp:Content>

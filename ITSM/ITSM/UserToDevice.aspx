<%@ Page Title="User Devices" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserToDevice.aspx.cs" Inherits="ITSM.UserToDevice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>


    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />


    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <style>
        .no-hover tbody tr:hover {
            background-color: transparent !important;
            color: inherit !important;
        }


        .select2-results__option--highlighted[aria-selected] {
            background-color: #e9ecef !important;
            color: #333 !important;
        }


        .select2-results__option--highlighted .device-sn {
            background-color: #fff !important;
            border-color: #ccc !important;
            color: #0F406B !important;
        }


        .select2-results__option {
            color: #333;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfEditUserDeviceID" runat="server" />
    <asp:HiddenField ID="hfDeleteId" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfAlertType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfAlertTitle" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfAlertMessage" runat="server" ClientIDMode="Static" />
    <asp:Button ID="btnDeleteConfirm" runat="server" ClientIDMode="Static" Style="display: none;"
        OnClick="btnDeleteConfirm_Click" />

    <div class="container-fluid" style="padding: 20px;">
        <div class="card shadow mb-4" style="margin-bottom: 25px;">
            <div class="card-header d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white;">
                <h5 class="card-title mb-0">Assign New Device</h5>
                <a href="Users.aspx" class="btn btn-light btn-sm text-dark"><i class="fal fa-arrow-left mr-1"></i>Back To List</a>
            </div>

            <div class="card-body">
                <div class="row align-items-end">
                    <div class="col-md-5">
                        <label class="form-label fw-bold">Device (Brand - Model - Serial Number)</label>

                        <asp:DropDownList ID="ddlDevices" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-bold">Start Date</label>

                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <label class="form-label fw-bold">End Date</label>
                        <asp:TextBox ID="txtEndDate"
                            runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-2 text-center">
                        <label class="form-label fw-bold d-block">&nbsp;</label>
                        <asp:LinkButton ID="btnAssign" CssClass="btn btn-outline-info btn-sm 
w-30"
                            runat="server" OnClick="btnAssign_Click" title="Assign Device">
                            <i class="fal fa-save"></i>
                        </asp:LinkButton>
                    </div>
                </div>

            </div>
        </div>

        <div class="card shadow">
            <div class="card-header" style="background-color: #0F406B; color: white;">
                <h3 class="card-title mb-0">
                    <asp:Label ID="lblUserName" runat="server" Text="Assigned Devices"></asp:Label>
                </h3>
            </div>
            <div class="card-body">

                <div class="table-responsive">
                    <asp:Repeater ID="rptUserDevices" runat="server" OnItemCommand="rptUserDevices_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-sm table-bordered align-middle no-hover" style="font-size: 13px;">

                                <thead class="table-light text-nowrap">
                                    <tr>
                                        <th>Brand</th>

                                        <th>Model</th>
                                        <th>Serial Number</th>

                                        <th>Start Date</th>
                                        <th>End Date</th>
                                        <th class="text-center">Actions</th>

                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr class="text-nowrap">

                                <td class="fw-bold"><%# Eval("Brand") %></td>
                                <td><%# Eval("Model") %></td>
                                <td><%# Eval("SerialNumber") %></td>

                                <td><%# Eval("StartDate", "{0:dd.MM.yyyy}") %></td>
                                <td><%# Eval("EndDate", "{0:dd.MM.yyyy}") %></td>
                                <td class="text-center">

                                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn btn-outline-success btn-sm me-1" CommandName="EditDevice" CommandArgument='<%# Eval("UserDeviceID") %>'>
                                        <i class="fal fa-edit"></i>
                                
                                    </asp:LinkButton>
                                    <button type="button" class="btn btn-outline-danger btn-sm" onclick="ConfirmDelete('<%# Eval("UserDeviceID") %>', '<%# Eval("Brand") %>')">
                                        <i class="fal fa-trash-alt"></i>

                                    </button>
                                </td>
                            </tr>

                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
        
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlNoDevice" runat="server" Visible="false" CssClass="alert alert-warning mt-3 text-center">
                        <i class="fal fa-exclamation-triangle mr-1"></i>No device was found assigned to this user.
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function ConfirmDelete(id, name) {
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

        window.onload = function () {
            const type = document.getElementById('hfAlertType').value;
            const msg = document.getElementById('hfAlertMessage').value;
            const title = document.getElementById('hfAlertTitle').value;

            if (type && msg) {
                Swal.fire({
                    icon: type,
                    title: title || type.toUpperCase(),
                    text: msg,

                    confirmButtonColor: '#3085d6'
                });
                document.getElementById('hfAlertType').value = '';
                document.getElementById('hfAlertMessage').value = '';
                document.getElementById('hfAlertTitle').value = '';
            }
        };
        function initUserDeviceSelect2() {
            var $ddl = $('#<%= ddlDevices.ClientID %>');
    if ($ddl.length) {
        if ($ddl.hasClass("select2-hidden-accessible")) {
            $ddl.select2('destroy');
        }

        $ddl.select2({
            placeholder: "-- Select Device --",
            allowClear: true,
            width: '100%',

            templateResult: formatUserDeviceResult,
            templateSelection: formatUserDeviceSelection,
            escapeMarkup: function (m) { return m; },

            matcher: function (params, data) {
                if ($.trim(params.term)
                    === '') return data;

                var optionText = $('<div>').html(data.text).text().trim().toLowerCase();
                var term = params.term.toLowerCase();
                if (optionText.startsWith(term)) {

                    data._priority = 1;
                    return data;
                }
                if (optionText.indexOf(term) > -1) {

                    data._priority = 2;
                    return data;
                }

                return null;
            },
            sorter: function (data) {
                return data.sort(function (a, b) {
                    var pa = a._priority || 99;

                    var pb = b._priority || 99;
                    return pa - pb;
                });
            }
        });
        var editId = document.getElementById('<%= hfEditUserDeviceID.ClientID %>').value;
                if (!editId || editId === "0" || editId === "") {
                    $ddl.val(null).trigger('change');
                }
            }
        }

        function formatUserDeviceResult(state) {
            if (!state.id || state.id === "") return state.text;
            var optionText = $('<div>').html(state.text).text().trim();
            var parts = optionText.split(' | ');
            var deviceMain = parts[0];
            var serial = parts.length > 1 ? parts[1] : "";
            return $(
                '<div style="display: flex; justify-content: space-between; align-items: center;">' +
                '<span style="font-weight: 600;">' + deviceMain + '</span>' +
                '<span class="device-sn" style="font-size: 11px; background: #f8f9fa; padding: 2px 8px; border-radius: 4px; border: 1px solid #ddd;">' + serial + '</span>' +
                '</div>'

            );
        }

        function formatUserDeviceSelection(state) {
            if (!state.id || state.id === "") return "-- Select Device --";
            return $('<div>').html(state.text).text().trim();
        }

        $(document).ready(function () {
            initUserDeviceSelect2();
        });
    </script>
</asp:Content>

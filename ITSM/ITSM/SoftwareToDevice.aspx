<%@ Page Title="Software Assignment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SoftwareToDevice.aspx.cs" Inherits="ITSM.SoftwareToDevice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>


    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />


    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <style>
        .action-buttons {
            display: flex;
            gap: 5px;
            justify-content: center;
        }

            .action-buttons i {
                pointer-events: none;
            }

        .table-sm td, .table-sm th {
            font-size: 13px;
            vertical-align: middle;
            padding: 10px 8px;
        }

        .custom-table-header {
            background-color: #f2f2f2 !important;
            color: #333 !important;
        }

        .table-container-wrapper {
            padding: 20px;
        }

        .card-inner-header {
            background-color: #0F406B;
            color: white !important;
            font-weight: bold;
            padding: 12px 15px;
            border-top-left-radius: 4px;
            border-top-right-radius: 4px;
            margin-bottom: 0;
        }

        .form-row-gray {
            background-color: #f8f9fa;
            padding: 20px;
            border-radius: 5px;
            margin: 0 !important;
            border: 1px solid #dee2e6;
        }

            .form-row-gray label {
                font-weight: bold;
                color: #444;
                margin-bottom: 5px;
            }



        .select2-device-result {
            display: flex;
            justify-content: space-between;
            align-items: center;
            width: 100%;
        }

        .device-sn {
            font-size: 11px;
            background: #f8f9fa;
            padding: 2px 8px;
            border-radius: 4px;
            border: 1px solid #ddd;
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




    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfIsEdit" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hfSelectedDeviceID" runat="server" Value="" ClientIDMode="Static" />

    <div class="container-fluid" style="padding: 20px;">
        <div class="card shadow mb-4">
            <div class="card-header py-3 d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white;">
                <h6 class="m-0 font-weight-bold">
                    <i class="fal fa-link"></i>Software Assignment: 
                    <asp:Literal ID="litSoftwareName" runat="server"></asp:Literal>
                </h6>
                <a href="Software.aspx" class="btn btn-sm btn-outline-light">
                    <i class="fa fa-arrow-left"></i>Back to List
                </a>
            </div>
            <div class="card-body">
                <div class="row align-items-end form-row-gray">
                    <div class="col-md-4">
                        <label>Device Model</label>
                        <asp:DropDownList ID="ddlModels" runat="server" ClientIDMode="Static" Style="width: 100%;"></asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label>Start Date</label>
                        <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-control form-control-sm" ClientIDMode="Static"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label>End Date</label>
                        <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control form-control-sm" ClientIDMode="Static"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-sm btn-primary w-100" OnClick="btnSave_Click" Style="background-color: #0F406B; border: none;">
                            <i class="fa fa-save"></i> SAVE
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

        <div class="card shadow">
            <div class="card-inner-header">
                <i class="fal fa-history"></i>Assignment History
            </div>

            <div class="table-container-wrapper">
                <asp:Panel ID="pnlTable" runat="server" CssClass="table-responsive border rounded" ClientIDMode="Static">
                    <asp:Repeater ID="rptDeviceSoftware" runat="server" OnItemCommand="rptDeviceSoftware_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-sm mb-0" id="mainTable">
                                <thead class="custom-table-header">
                                    <tr>
                                        <th style="padding-left: 15px;">Device Model</th>
                                        <th class="text-center">Start Date</th>
                                        <th class="text-center">End Date</th>
                                        <th class="text-center" style="width: 100px;">Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td style="padding-left: 15px;">
                                    <div class="fw-bold"><%# Eval("Model") %></div>
                                    <small class="text-primary font-weight-bold">SN: <%# Eval("DeviceNo") %></small>
                                </td>
                                <td class="text-center">
    <%# Eval("StartDate") == DBNull.Value ? "—" : Eval("StartDate", "{0:dd.MM.yyyy}") %>
</td>
                                <td class="text-center"><%# Eval("EndDate", "{0:dd.MM.yyyy}") %></td>
                                <td class="text-center">
                                    <div class="action-buttons">
                                        <asp:LinkButton ID="lbEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("DeviceID") %>' CssClass="btn btn-outline-success btn-sm">
                    <i class="fal fa-edit"></i>
                </asp:LinkButton>
                                        <button type="button" class="btn btn-outline-danger btn-sm" onclick="confirmDelete('<%# Eval("DeviceID") %>', '<%# Eval("Model").ToString().Replace("'", "") %>')">
                                            <i class="fal fa-trash-alt"></i>
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </asp:Panel>

                <asp:Panel ID="pnlEmpty" runat="server" Visible="false" ClientIDMode="Static">
                    <div style="background-color: #fff9e6; border: 1px solid #ffeeba; color: #856404; padding: 20px; border-radius: 8px; text-align: center;">
                        <i class="fal fa-exclamation-triangle"></i>No device was found assigned to this software.
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfDeleteId" runat="server" ClientIDMode="Static" />
    <asp:Button ID="btnDeleteConfirm" runat="server" OnClick="btnDeleteConfirm_Click" Style="display: none;" ClientIDMode="Static" />
    <asp:HiddenField ID="hfAlertType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfAlertMessage" runat="server" ClientIDMode="Static" />


    <script type="text/javascript">
        window.ListFilter = function () {
            return {
                init: function () { console.log("SmartFilter bypassed for this page."); },
                destroy: function () { }
            };
        };


        $(document).ready(function () {
            initMySelect2();
            showAlert();
        });
        function showAlert() {
            var t = document.getElementById('<%= hfAlertType.ClientID %>').value;
           var m = document.getElementById('<%= hfAlertMessage.ClientID %>').value;
    if (t && m) {
        Swal.fire({ 
            icon: t, 
            title: t.charAt(0).toUpperCase() + t.slice(1), 
            text: m, 
            confirmButtonColor: '#0F406B' 
        });
        document.getElementById('<%= hfAlertType.ClientID %>').value = '';
               document.getElementById('<%= hfAlertMessage.ClientID %>').value = '';
           }
       }

        function confirmDelete(id, name) {
            Swal.fire({
                title: 'Are you sure?',
                text: name + " will be deleted!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, delete it!',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('<%= hfDeleteId.ClientID %>').value = id;
            document.getElementById('<%= btnDeleteConfirm.ClientID %>').click();
                }
            });
        }



        function initMySelect2() {
            var $ddl = $('#ddlModels');

            if ($ddl.length) {
                if ($ddl.hasClass("select2-hidden-accessible")) {
                    $ddl.select2('destroy');
                }

                $ddl.select2({
                    placeholder: "-- Select Device --",
                    allowClear: true,
                    width: '100%',
                    templateResult: formatDeviceResult,
                    templateSelection: formatDeviceSelection,
                    escapeMarkup: function (m) { return m; },

                    matcher: function (params, data) {
                        if ($.trim(params.term) === '') {
                            return data;
                        }

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


                $ddl.val('0').trigger('change');
            }
        }

        function formatDeviceResult(state) {
            if (!state.id || state.id === "0") return state.text;

            var optionText = $('<div>').html(state.text).text().trim();
            var parts = optionText.split(' | ');
            var model = parts[0];
            var serial = parts.length > 1 ? parts[1] : "";

            return $(
                '<div style="display: flex; justify-content: space-between; align-items: center;">' +
                '<span style="font-weight: 600;">' + model + '</span>' +
                '<span class="device-sn">' + serial + '</span>' +
                '</div>'
            );
        }

        function formatDeviceSelection(state) {
            if (!state.id || state.id === "0") return "-- Select Device --"; // placeholder göster
            return $('<div>').html(state.text).text().trim();
        }
    </script>
</asp:Content>

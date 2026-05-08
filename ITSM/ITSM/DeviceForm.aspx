<%@ Page Title="DEVICE INFORMATION ENTRY FORM" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DeviceForm.aspx.cs" Inherits="ITSM.DeviceForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
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
            color: #0F406B;;
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

        .form-control, .form-select {
            border-radius: 4px;
            border: 1px solid #ced4da;
        }

            .form-control:focus, .form-select:focus {
                border-color: #8844cc;
                box-shadow: 0 0 0 0.2rem rgba(136, 68, 204, 0.25);
            }
    </style>

  

    <div class="main-container">
        
        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger mb-3">
            <asp:Label ID="lblErrorMessage" runat="server"></asp:Label>
        </asp:Panel>

        <div class="section-title">CORE DEVICE DETAILS</div>
        <div class="row">
            <div class="col-md-3 mb-3">
                <label class="form-label">Brand *</label>
                <asp:TextBox ID="txtBrand" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvBrand" runat="server" ControlToValidate="txtBrand"
                    ErrorMessage="Brand is required!" CssClass="text-danger small" Display="Dynamic" />
            </div>

            <div class="col-md-3 mb-3">
                <label class="form-label">Model *</label>
                <asp:TextBox ID="txtModel" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvModel" runat="server" ControlToValidate="txtModel"
                    ErrorMessage="Model is required!" CssClass="text-danger small" Display="Dynamic" />
            </div>

            <div class="col-md-3 mb-3">
                <label class="form-label">Serial Number *</label>
                <asp:TextBox ID="txtSerial" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSerial" runat="server" ControlToValidate="txtSerial"
                    ErrorMessage="Serial is required!" CssClass="text-danger small" Display="Dynamic" />
            </div>

            <div class="col-md-3 mb-3">
                <label class="form-label">Status *</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                    <asp:ListItem Text="-- Select Status --" Value=""></asp:ListItem>
                    <asp:ListItem Text="Active" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvStatus" runat="server" ControlToValidate="ddlStatus"
                    InitialValue="" ErrorMessage="Please select status!" CssClass="text-danger small" Display="Dynamic" />
            </div>
        </div>

        
        <div class="section-title">HARDWARE SPECIFICATIONS</div>
        <div class="row">
            <div class="col-md-3 mb-3">
                <label class="form-label">CPU</label>
                <asp:TextBox ID="txtCPU" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">RAM</label>
                <asp:TextBox ID="txtRAM" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">Device Type *</label>
                <asp:DropDownList ID="ddlDeviceType" runat="server" CssClass="form-select text-dark">
                    <asp:ListItem Text="-- Select Type --" Value=""></asp:ListItem>
                    <asp:ListItem Text="Laptop PC" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Desktop PC" Value="2"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvDeviceType" runat="server" ControlToValidate="ddlDeviceType"
                    InitialValue="" ErrorMessage="Device type is required!"
                    CssClass="text-danger small" Display="Dynamic" />
            </div>
        </div>

        <div class="section-title">HARDDISK SPECIFICATIONS</div>
        <div class="row">
            <div class="col-md-3 mb-3">
                <label class="form-label">Disk 1 Capacity (GB)</label>
                <asp:TextBox ID="txtDiskCapacity" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">Disk 1 Type</label>
                <asp:DropDownList ID="ddlDiskType" runat="server" CssClass="form-select">
                    <asp:ListItem Text="-- Select Disk Type --" Value=""></asp:ListItem>
                    <asp:ListItem Text="SSD" Value="1"></asp:ListItem>
                    <asp:ListItem Text="HDD" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">Disk 2 Capacity (GB)</label>
                <asp:TextBox ID="txtDisk2Capacity" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">Disk 2 Type</label>
                <asp:DropDownList ID="ddlDisk2Type" runat="server" CssClass="form-select">
                    <asp:ListItem Text="-- Select Disk Type --" Value=""></asp:ListItem>
                    <asp:ListItem Text="HDD" Value="1"></asp:ListItem>
                    <asp:ListItem Text="SSD" Value="2"></asp:ListItem>
                    <asp:ListItem Text="NVMe" Value="3"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        
        <div class="section-title">PURCHASE & ACCOUNTING</div>
        <div class="row">
            <div class="col-md-4 mb-3">
                <label class="form-label">Purchase Date</label>
                <asp:TextBox ID="txtPurchaseDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Warranty End Date</label>
                <asp:TextBox ID="txtWarrantyEnd" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Scrap Date</label>
                <asp:TextBox ID="txtScrapDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            
            <div class="col-md-4 mb-3">
                <label class="form-label">Purchase Number</label>
                <asp:TextBox ID="txtPurchaseNumber" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Accounting Code</label>
                <asp:TextBox ID="txtAccountingCode" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
                    <div class="col-md-4 mb-3">
    <label class="form-label">Scrap Description</label>
    <asp:TextBox ID="txtScrapDescription" runat="server" CssClass="form-control"></asp:TextBox>
</div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Device Name / Network Name</label>
                <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>

        <div class="section-title mt-5">LINKED ADDITIONAL DEVICES</div>
        <div class="row mb-4 align-items-end">
            <div class="col-md-9">
                <label class="form-label">Select Additional Device to Link</label>
                <asp:DropDownList ID="ddlAdditionalDevices" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:LinkButton ID="btnAddAddLink" runat="server" CssClass="btn btn-outline-primary w-100" OnClick="btnAddAddLink_Click">
                    <i class="fal fa-link mr-1"></i> Add
                </asp:LinkButton>
            </div>
        </div>

        <div class="row mb-4">
            <div class="col-12">
                <asp:Repeater ID="rptLinkedAddDevices" runat="server" OnItemCommand="rptLinkedAddDevices_ItemCommand">
                    <HeaderTemplate>
                        <table class="table table-sm table-bordered table-hover align-middle">
                            <thead class="table-light">
                                <tr>
                                    <th>Additional Device Info</th>
                                    <th style="width: 100px;" class="text-center">Action</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><strong><%# Eval("DeviceInfo") %></strong></td>
                            <td class="text-center">
                                <asp:LinkButton ID="btnRemove" runat="server" CommandName="Remove" CommandArgument='<%# Eval("AD_ID") %>' CssClass="btn btn-sm btn-outline-danger">
                                    <i class="fal fa-times"></i> Remove
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
        
       <div class="section-title">DEVICE ASSIGNMENT (ENTRUSTMENT) INFORMATION</div>
<div class="row align-items-end">
    <div class="col-md-4">
        <label class="form-label">Personnel to be Assigned</label>
        <asp:DropDownList ID="ddlUsers" runat="server" CssClass="form-select select2"></asp:DropDownList>
    </div>
    <div class="col-md-3">
        <label class="form-label">Start Date</label>
        <asp:TextBox ID="txtAssignStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
    </div>
    <div class="col-md-3">
        <label class="form-label">End Date (Optional)</label>
        <asp:TextBox ID="txtAssignEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
    </div>
    <div class="col-md-2">
        <asp:LinkButton ID="btnAddAssignment" runat="server" CssClass="btn btn-outline-primary w-100" OnClick="btnAddAssignment_Click" CausesValidation="false">
            <i class="fal fa-plus"></i> Confirm
        </asp:LinkButton>
    </div>
</div>

<div class="mt-4">
    <asp:Repeater ID="rptAssignments" runat="server" OnItemCommand="rptAssignments_ItemCommand">
        <HeaderTemplate>
            <table class="table table-bordered table-hover">
                <thead class="bg-light">
                    <tr>
                        <th>Personnel</th>
                        <th>Start Date</th>
                        <th>End Date</th>
                        <th>Status</th>
                        <th style="width: 100px;">Actions</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("NameSurname") %></td>
                <td><%# Eval("StartDate", "{0:dd.MM.yyyy}") %></td>
                <td><%# Eval("EndDate", "{0:dd.MM.yyyy}") %></td>
                <td><%# Eval("EndDate") == null ? "<span class='badge bg-success'>Active</span>" : "<span class='badge bg-secondary'>Previous</span>" %></td>
                <td class="text-center">
 <asp:LinkButton ID="btnEditAssign" runat="server" 
    CommandName="EditAssign" 
    CommandArgument='<%# Eval("UserDeviceID") %>' 
    CssClass="text-primary me-2">
    <i class="fal fa-edit"></i>
</asp:LinkButton>

<asp:LinkButton ID="btnDeleteAssign" runat="server" 
    CommandName="DeleteAssign" 
    CommandArgument='<%# Eval("UserDeviceID") %>' 
    CssClass="text-danger" 
    OnClientClick='<%# "return confirmDelete(this, " + Eval("UserDeviceID") + ");" %>'>
    <i class="fal fa-trash-alt"></i> 
</asp:LinkButton>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

        <div class="footer-buttons">
            <a href="Device.aspx" class="btn btn-secondary px-5">
                <i class="fal fa-arrow-left mr-1"></i>BACK TO LIST
            </a>
            <asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" CssClass="btn btn-primary px-5"
                Style="background-color: #8844cc; border-color: #8844cc; font-weight: bold;">
                <i class="fal fa-save mr-1"></i> SAVE DEVICE
            </asp:LinkButton>
        </div>
    </div>

    <asp:HiddenField ID="HiddenField1" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertType" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertMessage" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertTitle" runat="server" Value="" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" Value="" />
    <asp:HiddenField ID="hfSelectedUserDeviceID" runat="server" Value="" />

    <script type="text/javascript">
        function confirmDelete(sender, id) {
           
            if ($(sender).attr("data-confirmed") === "true") {
                return true;
            }

            Swal.fire({
                title: 'Are you sure?',
                text: "This assignment record will be permanently deleted!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, delete!',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {

                    $(sender).attr("data-confirmed", "true");
                    sender.click();
                }
            });

            return false;
        }
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
                    confirmButtonColor: '#8844cc'
                }).then((result) => {
                    if (redirectUrl && alertType === 'success') {
                        window.location.href = redirectUrl;
                    }
                    
                    document.getElementById('<%= hfAlertType.ClientID %>').value = '';
                    document.getElementById('<%= hfAlertMessage.ClientID %>').value = '';
                    document.getElementById('<%= hfAlertTitle.ClientID %>').value = '';
                });
            }
        };
    </script>
</asp:Content>

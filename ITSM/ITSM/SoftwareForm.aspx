<%@ Page Title="Software Management System" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SoftwareForm.aspx.cs" Inherits="ITSM.SoftwareForm" %>

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

        .description-textarea {
            resize: vertical;
            min-height: 80px;
        }

        /* RequestForm'dan Gelen Dosya Yükleme Stilleri */
        .btn-file-select {
            background-color: #f8f9fa;
            border: 1px solid #ced4da;
            font-size: 11px;
            font-weight: 600;
            padding: 8px 15px;
            cursor: pointer;
            transition: all 0.2s;
            display: inline-block;
        }

            .btn-file-select:hover {
                background-color: #e9ecef;
                border-color: #adb5bd;
            }

        .file-display-box {
            border: 1px solid #ced4da;
            padding: 10px 15px;
            background: #ffffff;
            border-radius: 4px;
            min-height: 42px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            font-size: 12px;
            color: #666;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfID" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertType" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertMessage" runat="server" Value="" />
    <asp:HiddenField ID="hfAlertTitle" runat="server" Value="" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" Value="" />


    <div class="main-container">
        <div class="section-title">SOFTWARE CORE DETAILS</div>
        <div class="row">

            <div class="col-md-12 mb-3">
                <label class="form-label">Description <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" MaxLength="250"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                    ErrorMessage="Description is required" CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveGroup"></asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="section-title">LICENSE & CLASSIFICATION</div>
        <div class="row">
            <div class="col-md-4 mb-3">
                <label class="form-label">License Code <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtLicenseNumber" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvLicenseNumber" runat="server" ControlToValidate="txtLicenseNumber"
                    ErrorMessage="License Code is required" CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveGroup"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">License Type <span class="text-danger">*</span></label>
                <asp:DropDownList ID="ddlLicenseType" runat="server" CssClass="form-select">
                    <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                    <asp:ListItem Text="Subscription" Value="Subscription"></asp:ListItem>
                    <asp:ListItem Text="Perpetual" Value="Perpetual"></asp:ListItem>
                    <asp:ListItem Text="Core Based" Value="Core Based"></asp:ListItem>
                    <asp:ListItem Text="Named User" Value="Named User"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtLicenseNumber"
                    ErrorMessage="License Type is required" CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveGroup"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Software Type <span class="text-danger">*</span></label>
                <asp:DropDownList ID="ddlSoftwareType" runat="server" CssClass="form-select">
                    <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                    <asp:ListItem Text="Productivity" Value="Productivity"></asp:ListItem>
                    <asp:ListItem Text="ERP" Value="ERP"></asp:ListItem>
                    <asp:ListItem Text="Engineering" Value="Engineering"></asp:ListItem>
                    <asp:ListItem Text="Communication" Value="Communication"></asp:ListItem>
                    <asp:ListItem Text="Design" Value="Design"></asp:ListItem>
                    <asp:ListItem Text="Utility" Value="Utility"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtLicenseNumber"
                    ErrorMessage="Software Type is required" CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveGroup"></asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="section-title">USAGE & ACCOUNTING</div>
        <div class="row g-2">
            <div class="col" style="flex: 0 0 15%;">
                <label class="form-label">User Count  </label>
                <asp:TextBox ID="txtUserCount" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
            </div>
            <div class="col">
                <label class="form-label">Accounting Code <span class="text-danger">*</span> </label>
                <asp:TextBox ID="txtAccountingCode" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtLicenseNumber"
                    ErrorMessage="Accounting Code is required" CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveGroup"></asp:RequiredFieldValidator>
            </div>
            <div class="col">
                <label class="form-label">Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Active" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Passive" Value="0"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col">
                <label class="form-label">Purchase Date  </label>
                <asp:TextBox ID="txtPurchaseDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col">
                <label class="form-label">Expiry Date  </label>
                <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
        </div>
        <div class="section-title">PURCHASE & RENEWAL HISTORY</div>
        <div class="purchase-entry-container p-3 mb-3" style="background-color: #f8f9fa; border-radius: 5px; border: 1px solid #dee2e6;">
            <div class="row g-2 align-items-end">
                <div class="col-md-3">
                    <label class="form-label">New Purchase Date</label>
                    <asp:TextBox ID="txtNewPurchaseDate" runat="server" CssClass="form-control form-control-sm" TextMode="Date"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <label class="form-label">New License Start</label>
                    <asp:TextBox ID="txtNewLicenseStart" runat="server" CssClass="form-control form-control-sm" TextMode="Date"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <label class="form-label">New Expiry Date</label>
                    <asp:TextBox ID="txtNewLicenseEnd" runat="server" CssClass="form-control form-control-sm" TextMode="Date"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:LinkButton ID="btnAddPurchase" runat="server" CssClass="btn btn-outline-primary w-100" OnClick="btnAddPurchase_Click">
                <i class="fal fa-plus-circle"></i> ADD 
            </asp:LinkButton>
                </div>
            </div>
        </div>


        <div class="table-responsive">
            <asp:GridView ID="gvPurchases" runat="server" AutoGenerateColumns="False"
                CssClass="table table-hover table-bordered" GridLines="None" Width="100%"
                OnRowCommand="gvPurchases_RowCommand"
                DataKeyNames="SoftwarePurchaseID">
                <Columns>
                    <asp:BoundField DataField="PurchaseDate" HeaderText="Purchase Date" DataFormatString="{0:dd.MM.yyyy}" />
                    <asp:BoundField DataField="LicenseStart" HeaderText="Start Date" DataFormatString="{0:dd.MM.yyyy}" />
                    <asp:BoundField DataField="LicenseEnd" HeaderText="Expiry Date" DataFormatString="{0:dd.MM.yyyy}" />
                    <asp:BoundField DataField="RecordDate" HeaderText="Recorded At" DataFormatString="{0:dd.MM.yyyy HH:mm}" />
                    <asp:BoundField DataField="RecordUser" HeaderText="Recorded By" />

                    <asp:TemplateField HeaderText="Actions">
                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" runat="server"
                                CommandName="DeleteRecord"
                                CommandArgument='<%# Eval("SoftwarePurchaseID") %>'
                                CssClass="btn btn-sm btn-delete-confirm"
                                Style="border: 1px solid #ff007f; color: #ff007f; border-radius: 4px; padding: 2px 8px;">
    <i class="fal fa-trash-alt"></i>
</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="text-center p-3 text-muted">No renewal history found.</div>
                </EmptyDataTemplate>
                <HeaderStyle BackColor="#f1f5f9" Font-Bold="True" ForeColor="#475569" />
            </asp:GridView>
        </div>



        <div class="section-title">ATTACHMENTS</div>
        <div class="mb-2">
            <label class="btn-file-select rounded shadow-sm">
                <i class="fal fa-paperclip me-1"></i>SELECT ATTACHMENTS
       
                <asp:FileUpload ID="fuAttachment" runat="server" Style="display: none;" AllowMultiple="true" onchange="updateFileList(this)" />
            </label>
        </div>
        <div id="fileDisplay" class="file-display-box mb-4">
            <asp:Repeater ID="rptAttachments" runat="server" OnItemCommand="rptAttachments_ItemCommand">
                <ItemTemplate>
                    <div class="d-flex justify-content-between align-items-center mb-1 pb-1 border-bottom">
                        <span><i class="fal fa-file-check me-2 text-success"></i><%# Eval("FileName") %></span>
                        <asp:LinkButton ID="btnDeleteSaved" runat="server"
                            CssClass="text-danger btn-delete-confirm"
                            CommandName="DeleteFile"
                            CommandArgument='<%# Eval("SoftwareAttachmentID") %>'>
    <i class="fal fa-times-circle" style="font-size: 1.2rem;"></i>
</asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <div id="newFilesList"></div>

            <span id="noFilesMsg"><i class="fal fa-info-circle me-2"></i>No files attached</span>
        </div>


        <div class="footer-buttons">
            <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-secondary px-5" OnClick="btnCancel_Click" CausesValidation="false">
                <i class="fal fa-arrow-left mr-1"></i> BACK TO LIST
            </asp:LinkButton>
            <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary px-5" OnClick="btnSave_Click" ValidationGroup="SaveGroup" Style="background-color: #0F406B; border-color: #0F406B;">
                <i class="fal fa-save mr-1"></i> SAVE SOFTWARE
            </asp:LinkButton>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            setInitialMessage();
        });
        $(document).on("click", ".btn-delete-confirm", function (e) {
            var linkButton = $(this);

            if (linkButton.data("confirmed")) return true;

            e.preventDefault();

            Swal.fire({
                title: 'Are you sure?',
                text: "Do you really want to delete this record?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, delete it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    linkButton.data("confirmed", true);

                    var href = linkButton.attr('href');
                    if (href && href.indexOf('javascript:__doPostBack') === 0) {
                        eval(href.replace('javascript:', ''));
                    } else {
                        linkButton[0].click();
                    }
                }
            });
            return false;
        });


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
                    confirmButtonColor: '#0F406B'
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

        function checkLength(textbox, maxLength, charCountId) {
            if (!textbox) return;

            var countSpan = document.getElementById(charCountId);


            if (textbox.value.length === 0) {
                countSpan.innerText = "";
                return;
            }

            if (textbox.value.length > maxLength) {
                textbox.value = textbox.value.substring(0, maxLength);
                alert("Text limit exceeded! Only " + maxLength + " characters allowed.");
            }


            countSpan.innerText = (maxLength - textbox.value.length) + " character..";
        }


        document.addEventListener('DOMContentLoaded', function () {

            var descBox = document.getElementById('<%= txtDescription.ClientID %>');

            if (descBox) {
                checkLength(descBox, 100, 'charCountName');
            }
        });

        function updateFileList(input) {
            var display = document.getElementById('fileDisplay');
            var maxSizeBytes = 10 * 1024 * 1024;
            var totalSize = 0;
            if (!display) return;

            if (input.files && input.files.length > 0) {
                for (var i = 0; i < input.files.length; i++) {
                    totalSize += input.files[i].size;
                }
                if (totalSize > maxSizeBytes) {
                    Swal.fire({
                        icon: 'error',
                        title: 'File Size Exceeded!',
                        text: 'The total size of the selected files exceeds the 10 MB limit.',
                        confirmButtonColor: '#0F406B'
                    });
                    input.value = '';
                    display.innerHTML = '<span><i class="fal fa-info-circle me-2"></i> No files attached</span>';
                    return;
                }
                display.innerHTML = "";
                for (var j = 0; j < input.files.length; j++) {
                    display.innerHTML += '<div><i class="fal fa-file me-2 text-primary"></i>' + input.files[j].name + '</div>';
                }
            } else {
                display.innerHTML = '<span><i class="fal fa-info-circle me-2"></i> No files attached</span>';
            }
        }

        var selectedFiles = new DataTransfer();

        function updateFileList(input) {
            var display = document.getElementById('fileDisplay');
            var maxSizeBytes = 10 * 1024 * 1024;

            if (!display) return;

            if (input.files && input.files.length > 0) {
                for (var i = 0; i < input.files.length; i++) {
                    selectedFiles.items.add(input.files[i]);
                }
            }
            var totalSize = 0;
            for (var i = 0; i < selectedFiles.files.length; i++) {
                totalSize += selectedFiles.files[i].size;
            }

            if (totalSize > maxSizeBytes) {
                Swal.fire({
                    icon: 'error',
                    title: 'File Size Exceeded!',
                    text: 'The total size of all attached files exceeds the 10 MB limit.',
                    confirmButtonColor: '#0F406B'
                });
                selectedFiles.items.clear();
                input.files = selectedFiles.files;
                display.innerHTML = '<span><i class="fal fa-info-circle me-2"></i> No files attached</span>';
                return;
            }
            input.files = selectedFiles.files;
            renderFileList(input, display);
        }
        function setInitialMessage() {
            var noFilesMsg = document.getElementById('noFilesMsg');
            var hasNewFiles = selectedFiles.files.length > 0;
            var hasSavedFiles = document.querySelectorAll('#fileDisplay .border-bottom').length > 0;

            if (!hasNewFiles && !hasSavedFiles) {
                noFilesMsg.style.display = 'inline';
            } else {
                noFilesMsg.style.display = 'none';
            }
        }

        function renderFileList(input, display) {
            var newFilesList = document.getElementById('newFilesList');
            var noFilesMsg = document.getElementById('noFilesMsg');

            if (selectedFiles.files.length > 0) {
                newFilesList.innerHTML = "";
                noFilesMsg.style.display = 'none';

                for (let j = 0; j < selectedFiles.files.length; j++) {
                    newFilesList.innerHTML +=
                        '<div class="d-flex justify-content-between align-items-center mb-1 pb-1 border-bottom">' +
                        '<span><i class="fal fa-file me-2 text-primary"></i> ' + selectedFiles.files[j].name + ' <span class="badge bg-primary ms-2" style="font-size:0.6rem;">New</span></span>' +
                        '<a href="javascript:void(0);" onclick="removeFile(' + j + ', \'' + input.id + '\')" class="text-danger" title="Remove File">' +
                        '<i class="fal fa-times-circle" style="font-size: 1.2rem;"></i>' +
                        '</a>' +
                        '</div>';
                }
            } else {
                setInitialMessage();
            }
        }

        function removeFile(index, inputId) {
            var input = document.getElementById(inputId);
            var display = document.getElementById('fileDisplay');
            selectedFiles.items.remove(index);
            input.files = selectedFiles.files;
            renderFileList(input, display);
        }


    </script>

</asp:Content>

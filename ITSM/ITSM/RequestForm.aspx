<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RequestForm.aspx.cs" Inherits="ITSM.RequestForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .btn-custom-save {
            background-color: #0F406B;
            color: white !important;
            font-weight: 600;
            font-size: 12px;
            height: 38px;
            border: none;
            text-shadow: none;
            display: inline-flex;
            align-items: center;
            gap: 0.25rem;
        }

            .btn-custom-save:hover {
                background-color: #0b3152;
                transform: translateY(-1px);
            }

        .btn-file-select {
            background-color: #f8f9fa;
            border: 1px solid #ced4da;
            font-size: 11px;
            font-weight: 600;
            padding: 8px 15px;
            cursor: pointer;
            transition: all 0.2s;
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

        .suggestion-item {
            cursor: pointer;
            transition: background-color 0.2s ease;
        }

            .suggestion-item:hover {
                background-color: #f8f9fa;
            }

        .suggestion-card-header {
            font-size: 12px;
            font-weight: 600;
            background-color: #0F406B;
            color: white;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid py-4">
        <div class="col-lg-9 mx-auto">
            <div class="card shadow-sm border">
                <div class="card-header d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white;">
                    <h5 class="card-title mb-0" style="font-size: 15px; font-weight: 600;">
                        <i class="fal fa-plus-circle me-2"></i><%= gettext("reqform","CREATE NEW SUPPORT REQUEST")%>
                    </h5>
                    <a href="Requests.aspx" class="btn btn-sm btn-outline-light border-0">
                        <i class="fal fa-times"></i>
                    </a>
                </div>

                <div class="card-body p-4">



                    <div class="mb-3">
                        <label class="form-label fw-bold" style="font-size: 11px; color: #0F406B;"><%= gettext("topc","TOPIC")%></label>
                        <asp:TextBox ID="txtTopic" runat="server"
                            TextMode="MultiLine"
                            Rows="2"
                            CssClass="form-control form-control-sm"
                            placeholder="Brief summary of the issue"
                            onkeyup="checkLength(this, 250, 'charCountTopic'); triggerSuggestionPostback();"></asp:TextBox>
                        <span id="charCountTopic" style="font-size: 0.75rem; color: #64748b; margin-top: 0.25rem; display: block;"></span>
                    </div>

                    <div class="mb-3">
                        <label class="form-label fw-bold" style="font-size: 11px; color: #0F406B;"><%= gettext("description","DESCRIPTION")%></label>
                        <asp:TextBox ID="txtDescription" runat="server"
                            TextMode="MultiLine"
                            Rows="5"
                            CssClass="form-control form-control-sm"
                            placeholder="Detailed explanation..."
                            onkeyup="triggerSuggestionPostback();"></asp:TextBox>
                    </div>
                    <asp:UpdatePanel ID="updSuggestions" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <asp:Button ID="btnLoadSuggestions" runat="server" Style="display: none;" OnClick="btnLoadSuggestions_Click" UseSubmitBehavior="false" />
                            <asp:HiddenField ID="hfFocusedControl" runat="server" />
                            <asp:HiddenField ID="hfSelectionStart" runat="server" />
                            <asp:HiddenField ID="hfSelectionEnd" runat="server" />
                            <asp:Panel ID="pnlSuggestions" runat="server" Visible="false" CssClass="mb-3">
                                <div class="card border shadow-sm">
                                    <div class="card-header suggestion-card-header">
                                        <%= gettext("simreq","SIMILAR REQUESTS")%>
                                    </div>
                                    <div class="card-body p-2">
                                        <asp:Repeater ID="rptSuggestions" runat="server">
                                            <ItemTemplate>
                                                <div class="suggestion-item p-2 border-bottom">
                                                    <div style="font-size: 12px; font-weight: 600; color: #0F406B;">
                                                        <%# Eval("Title") %>
                                                    </div>
                                                    <div style="font-size: 11px; color: #64748b; margin-top: 4px;">
                                                        <%# Eval("ShortDescription") %>
                                                    </div>
                                                    <div style="font-size: 10px; color: #999; margin-top: 4px;">
                                                        Source: <%# Eval("SourceDB") %> | ID: <%# Eval("RecordId") %>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </asp:Panel>

                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnLoadSuggestions" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <div class="row mb-4">

                        <asp:Panel ID="pnlRequestOwner" runat="server" CssClass="col-md-3" Visible="false">
                            <label class="form-label fw-bold" style="font-size: 11px; color: #0F406B;"><%= gettext("reqowner","REQUEST OWNER")%></label>
                            <asp:DropDownList ID="ddlRequestOwner" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlRequestOwner_SelectedIndexChanged"></asp:DropDownList>
                        </asp:Panel>

                        <asp:Panel ID="gizle" runat="server" CssClass="col-md-4" Visible="false">
                            <div class="col-md-3">
                                <label class="form-label fw-bold" style="font-size: 11px; color: #0F406B;"><%= gettext("reldev","RELATED DEVICE")%></label>
                                <asp:DropDownList ID="ddlDevices" runat="server" Visible="false" CssClass="form-select form-select-sm"></asp:DropDownList>
                            </div>
                        </asp:Panel>

                        <div class="col-md-3">
                            <label class="form-label fw-bold" style="font-size: 11px; color: #0F406B;"><%= gettext("reluser","RELATED USER (CC)")%></label>
                            <asp:DropDownList ID="ddlUsers" runat="server" CssClass="form-select form-select-sm"></asp:DropDownList>
                        </div>

                        <div class="col-md-3">
                            <label class="form-label fw-bold" style="font-size: 11px; color: #0F406B;"><%= gettext("reqdate","REQUEST DATE")%></label>
                            <asp:TextBox ID="txtRequestDate" runat="server" TextMode="DateTimeLocal" CssClass="form-select form-select-sm"></asp:TextBox>
                        </div>

                    </div>

                    <div class="mb-2">
                        <label class="btn-file-select rounded shadow-sm">
                            <i class="fal fa-paperclip me-1"></i><%= gettext("selattch","SELECT ATTACHMENTS")%>
                            <asp:FileUpload ID="fuAttachment" runat="server" Style="display: none;" AllowMultiple="true" onchange="updateFileList(this)" />
                        </label>
                    </div>

                    <div id="fileDisplay" class="file-display-box mb-4">
                        <span><i class="fal fa-info-circle me-2"></i><%= gettext("nofile","No files attached")%></span>
                    </div>

                    <div class="d-flex justify-content-end gap-2">
                        <a href="Requests.aspx" class="btn btn-light border btn-sm px-4 fw-bold"><%= gettext("cancel","CANCEL")%></a>
                        <asp:LinkButton ID="btnSubmitRequest" runat="server" CssClass="btn btn-custom-save px-4 rounded shadow-sm" OnClick="btnSubmitRequest_Click">
                            <i class="fal fa-save me-2" aria-hidden="true"></i><%= gettext("sbmt","SUBMIT REQUESTS")%>
                        </asp:LinkButton>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        var lang_fileSizeTitle = '<%= gettext("filesizetitle", "File Size Exceeded!") %>';
        var lang_fileSizeText = '<%= gettext("filesizemsg", "The total size of the selected files exceeds the 10 MB limit. Please select smaller files.") %>';

        var suggestionTimer = null;
        var isSuggestionPostbackRunning = false;

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
                        title: lang_fileSizeTitle,
                        text: lang_fileSizeText,
                        confirmButtonColor: '#0F406B'
                    });
                    input.value = '';
                    display.innerHTML = '<span><i class="fal fa-info-circle me-2"></i><%= gettext("nofile","No files attached")%></span>';
                    return;
                }

                display.innerHTML = "";
                for (var j = 0; j < input.files.length; j++) {
                    display.innerHTML += '<div><i class="fal fa-file me-2 text-primary"></i>' + input.files[j].name + '</div>';
                }
            } else {
                display.innerHTML = '<span><i class="fal fa-info-circle me-2"></i>No files attached</span>';
            }
        }

        function checkLength(textbox, maxLength, charCountId) {
            if (!textbox) return;
            var countSpan = document.getElementById(charCountId);

            if (textbox.value.length === 0) {
                countSpan.innerText = "";
                return;
            }

            if (textbox.value.length > maxLength) {
                textbox.value = textbox.value.substring(0, maxLength);

                alert(`<%= gettext("text_limit_exceeded", "Text limit exceeded! Only ") %>` + maxLength + `<%= gettext("characters_allowed", " characters allowed.") %>`);
         }


         countSpan.innerText = (maxLength - textbox.value.length) + ` <%= gettext("character", "character..") %>`;
        }

        function saveFocusInfo() {
            var active = document.activeElement;
            var hfFocused = document.getElementById('<%= hfFocusedControl.ClientID %>');
         var hfStart = document.getElementById('<%= hfSelectionStart.ClientID %>');
         var hfEnd = document.getElementById('<%= hfSelectionEnd.ClientID %>');

            if (!active || !hfFocused || !hfStart || !hfEnd) return;

            hfFocused.value = active.id || '';

            if (typeof active.selectionStart !== "undefined") {
                hfStart.value = active.selectionStart;
                hfEnd.value = active.selectionEnd;
            } else {
                hfStart.value = '';
                hfEnd.value = '';
            }
        }

        function restoreFocusInfo() {
            var hfFocused = document.getElementById('<%= hfFocusedControl.ClientID %>');
            var hfStart = document.getElementById('<%= hfSelectionStart.ClientID %>');
            var hfEnd = document.getElementById('<%= hfSelectionEnd.ClientID %>');

            if (!hfFocused) return;
            if (!hfFocused.value) return;

            var el = document.getElementById(hfFocused.value);
            if (!el) return;

            var topicId = '<%= txtTopic.ClientID %>';
            var descId = '<%= txtDescription.ClientID %>';
            var active = document.activeElement;


            if (active === el) {
                return;
            }

            if (active && active.id && (active.id === topicId || active.id === descId)) {
                return;
            }

            try {
                el.focus();

                if (typeof el.selectionStart !== "undefined" && hfStart.value !== '') {
                    var start = parseInt(hfStart.value, 10);
                    var end = parseInt(hfEnd.value, 10);

                    if (!isNaN(start) && !isNaN(end)) {
                        var len = (el.value && el.value.length) ? el.value.length : 0;
                        if (start <= len && end <= len) {
                            el.setSelectionRange(start, end);
                        } else {
                            el.setSelectionRange(len, len);
                        }
                    }
                }
            } catch (e) {
                console.log(e);
            }
        }

        function triggerSuggestionPostback() {
            clearTimeout(suggestionTimer);

            suggestionTimer = setTimeout(function () {
                if (isSuggestionPostbackRunning) return;

                var topic = document.getElementById('<%= txtTopic.ClientID %>');
                var desc = document.getElementById('<%= txtDescription.ClientID %>');

                if (!topic || !desc) return;

                var fullText = ((topic.value || '') + ' ' + (desc.value || '')).replace(/\s+/g, ' ').trim();

                if (fullText.length < 10) return;

                saveFocusInfo();
                isSuggestionPostbackRunning = true;

                __doPostBack('<%= btnLoadSuggestions.UniqueID %>', '');
            }, 700);
        }

        document.addEventListener('DOMContentLoaded', function () {
            var topicBox = document.getElementById('<%= txtTopic.ClientID %>');
            if (topicBox) {
                checkLength(topicBox, 250, 'charCountTopic');
            }

            if (typeof Sys !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                var prm = Sys.WebForms.PageRequestManager.getInstance();

                prm.add_endRequest(function () {
                    isSuggestionPostbackRunning = false;
                    restoreFocusInfo();
                });
            }
        });
    </script>

</asp:Content>

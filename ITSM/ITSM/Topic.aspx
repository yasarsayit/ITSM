<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Topic.aspx.cs" Inherits="ITSM.Konu" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/tinymce/6.8.2/tinymce.min.js"></script>

    <style>
        .konu-wrapper {
            max-width: 1100px;
            margin: 20px auto;
            padding: 15px;
        }

        .editor-box {
            margin-top: 15px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            overflow: hidden;
            border: 1px solid #0F406B;
        }




        .preview-area {
            display: none;
            margin-top: 30px;
            padding: 0;
            border: 2px solid #0F406B;
            background: #ffffff;
            border-radius: 12px;
            box-shadow: 0 10px 25px rgba(15, 64, 107, 0.1);
            overflow: hidden;
        }

        .preview-header {
            background: #0F406B;
            padding: 10px 20px;
            border-bottom: 1px solid #eee;
            color: #ffffff;
            font-weight: bold;
        }

        .preview-content {
            padding: 25px;
            word-wrap: break-word;
        }

            .preview-content img {
                max-width: 100%;
                height: auto;
                border-radius: 8px;
            }

        .btn-action-row {
            margin-top: 25px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }


        .btn-modern {
            padding: 10px 25px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 1px;
            border-radius: 6px;
            transition: all 0.3s;
        }


        .btn-outline-custom {
            border: 2px solid #0F406B;
            color: #0F406B;
            background: transparent;
        }

            .btn-outline-custom:hover {
                background: #0F406B;
                color: white;
            }


        .btn-custom-save {
            background-color: #0F406B !important;
            color: white !important;
            border: none !important;
            transition: all 0.3s ease;
        }

            .btn-custom-save:hover {
                background-color: #0b2e4d !important;
                box-shadow: 0 4px 10px rgba(0,0,0,0.2);
                transform: translateY(-1px);
            }


            .btn-custom-save:disabled {
                background-color: #a5b1c2 !important;
                cursor: not-allowed;
            }


        .switch {
            position: relative;
            display: inline-block;
            width: 50px;
            height: 24px;
        }

            .switch input {
                opacity: 0;
                width: 0;
                height: 0;
            }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #ff7675;
            transition: .4s;
            border-radius: 24px;
        }

            .slider:before {
                position: absolute;
                content: "";
                height: 18px;
                width: 18px;
                left: 3px;
                bottom: 3px;
                background-color: white;
                transition: .4s;
                border-radius: 50%;
            }


        input:checked + .slider {
            background-color: #2ecc71;
        }

        input:focus + .slider {
            box-shadow: 0 0 1px #2ecc71;
        }


        input:checked + .slider:before {
            transform: translateX(26px);
        }


        .status-container {
            padding: 15px;
            margin-bottom: 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            border-radius: 8px;
            border: 1px solid #e0e6ed;
            transition: background-color 0.4s ease, border-color 0.4s ease;
            [cite: 34]
        }


        .status-active {
            background-color: #e6fffa !important;
            border-color: #38b2ac !important;
        }


        .status-inactive {
            background-color: #fff5f5 !important;
            border-color: #feb2b2 !important;
        }


        .text-active {
            color: #2c7a7b !important;
            font-weight: bold;
        }

        .text-inactive {
            color: #c53030 !important;
            font-weight: bold;
        }


        input:checked + .slider {
            background-color: #38b2ac;
        }

        .slider {
            background-color: #fc8181;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="konu-wrapper">
        <div class="mb-4">

            <p class="text-muted"><%= gettext("tmuted","Fill in the fields below to create a professional knowledge base article.")%></p>
        </div>

        <div class="card shadow-sm border-0" style="border-radius: 12px; border: 1px solid #e0e6ed;">
            <div class="card-body p-4">

                <div class="form-group mb-4">
                    <label class="fw-bold mb-2" style="color: #0F406B; font-size: 13px; text-transform: uppercase;"><%= gettext("sbjtitletr","Turkish Subject Title")%></label>
                    <asp:TextBox ID="txtKonuBaslik" runat="server" CssClass="form-control form-control-lg"
                        placeholder='<%# gettext("ph_titletr", "Enter turkish article title...") %>' Style="border-radius: 8px; border: 1px solid #ced4da;"
                        MaxLength="250" oninput="checkLength(this, 250, 'charCountTitle');"></asp:TextBox>
                    <span id="charCountTitle" style="font-size: 0.75rem; color: #64748b; margin-top: 0.25rem; display: block;"></span>
                </div>

                <div class="form-group mb-4">
                    <label class="fw-bold mb-2" style="color: #0F406B; font-size: 13px; text-transform: uppercase;"><%= gettext("sbjtitleeng"," English Subject Title ")%></label>
                    <asp:TextBox ID="txtKonuBaslikEng" runat="server" CssClass="form-control form-control-lg"
                        placeholder='<%# gettext("ph_titleeng", "Enter english article title...") %>' Style="border-radius: 8px; border: 1px solid #ced4da;"
                        MaxLength="250" oninput="checkLength(this, 250, 'charCountTitle2');"></asp:TextBox>
                    <span id="charCountTitle2" style="font-size: 0.75rem; color: #64748b; margin-top: 0.25rem; display: block;"></span>
                </div>

                <div class="form-group mb-4">
                    <label class="fw-bold mb-2" style="color: #0F406B; font-size: 13px; text-transform: uppercase;"><%= gettext("summary","Summary / Short Description")%></label>
                    <asp:TextBox ID="txtOzet" runat="server" CssClass="form-control"
                        placeholder='<%# gettext("ph_summary", "A brief summary for previews...") %>' TextMode="MultiLine" Rows="2"
                        Style="border-radius: 8px; border: 1px solid #ced4da; background-color: #fcfcfc; font-size: 14px;"
                        MaxLength="500" oninput="checkLength(this, 500, 'charCountSummary');"></asp:TextBox>
                    <span id="charCountSummary" style="font-size: 0.75rem; color: #64748b; margin-top: 0.25rem; display: block;"></span>
                </div>

                <div id="divPublishStatus" class="status-container status-active">
                    <span id="labelStatus" class="fw-bold text-active">
                        <i class="fa fa-globe me-2"></i><%= gettext("pstatus","Publishing Status")%>
                    </span>

                    <div class="d-flex align-items-center">
                        <span id="statusNote" class="me-2" style="font-size: 12px;"><%= gettext("vsbstatus","Visible to all users")%></span>
                        <label class="switch">
                            <asp:CheckBox ID="chkIsVisible" runat="server" Checked="true" ClientIDMode="Static" />
                            <span class="slider"></span>
                        </label>
                    </div>
                </div>

                <div class="form-group mb-4">
                    <label class="fw-bold mb-2" style="color: #0F406B; font-size: 13px; text-transform: uppercase;"><%= gettext("detailcont","Detailed Content")%></label>
                    <div class="editor-box" style="border: 1px solid #0F406B; border-radius: 8px; overflow: hidden; margin-top: 0;">
                        <asp:TextBox ID="proeditor" runat="server" TextMode="MultiLine" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>

                <asp:HiddenField ID="hfEditorIcerik" runat="server" ClientIDMode="Static" />

                <div class="btn-action-row d-flex justify-content-between align-items-center mt-4">
                    <button type="button" class="btn btn-modern btn-outline-custom" onclick="generatePreview()">
                        <i class="fa fa-eye"></i><%= gettext("preview","PREVIEW")%>
                    </button>

                    <div class="d-flex align-items-center">
                        <asp:Label ID="lblMesaj" runat="server" Text="" ClientIDMode="Static"
                            Style="margin-right: 15px; font-weight: bold; color: #0F406B;"></asp:Label>

                        <asp:Button ID="btnKaydet" runat="server"
                            Text='<%#gettext("publishentry", "GİRİŞİ YAYINLA") %>'
                            CssClass="btn btn-modern btn-custom-save" OnClick="btnKaydet_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div id="previewBox" class="preview-area mt-5">
            <div class="preview-header"><i class="fa fa-desktop"></i><%= gettext("liveprev","Live Preview")%></div>
            <div id="previewResult" class="preview-content"></div>
        </div>


    </div>

    <script type="text/javascript">
        var activeLang = '<%= gettext("tinymce_lang_code", "tr") %>';
        var lang_limit_msg = '<%= gettext("limit_alert", "Karakter sınırı aşıldı! En fazla {0} karakter girebilirsiniz.") %>';
        var lang_char_rem = '<%= gettext("char_rem", "karakter kaldı..") %>';
        var lang_prev_error = '<%= gettext("prev_alert", "Önizleme için lütfen içerik giriniz.") %>';
        var lang_vis_all = '<%= gettext("vis_all", "Tüm kullanıcılara görünür") %>';
        var lang_draft_mode = '<%= gettext("draft_mode", "Gizli / Taslak Modu") %>';
        var lang_vis_all = '<%= gettext("vsbstatus", "Visible to all users") %>';
        var lang_draft_mode = '<%= gettext("draft_status_text", "Private / Draft") %>';

        tinymce.init({
            selector: '#proeditor',
            promotion: false,
            branding: false,
            toolbar_mode: 'floating',

            language: activeLang === 'tr' ? 'tr' : 'en',
            language_url: activeLang === 'tr' ? 'https://cdn.jsdelivr.net/npm/tinymce-i18n@23.10.9/langs6/tr.js' : '',

            plugins: 'advlist autolink lists link image charmap preview anchor searchreplace visualblocks code fullscreen insertdatetime media table wordcount emoticons autoresize',
            toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline | alignleft aligncenter alignright | bullist numlist | table emoticons fullscreen preview | image code',
            min_height: 500,
            autoresize_bottom_margin: 50,
            image_title: true,
            automatic_uploads: true,
            images_upload_url: 'ImageUpload.ashx',
            file_picker_types: 'image',
            file_picker_callback: function (cb, value, meta) {
                if (meta.filetype === 'image') {

                    const dialogConfig = {
                        title: 'Media Library',
                        url: 'Gallery.html',
                        width: 700,
                        height: 500,
                        buttons: [],
                        onMessage: function (api, data) {
                            if (data.mceAction === 'gorselSecildi') {

                                cb(data.url, { alt: data.title || '' });
                                api.close();
                            }
                        }
                    };
                    tinymce.activeEditor.windowManager.openUrl(dialogConfig);
                }
            },
            paste_data_images: true,
            object_resizing: true,
            content_style: 'body { font-family:Helvetica,Arial,sans-serif; font-size:16px; line-height:1.6; } img { max-width:100%; height:auto; }',
            setup: function (editor) {

                editor.on('init', function () {
                    var saved = document.getElementById('<%= hfEditorIcerik.ClientID %>').value;
                    if (saved) {
                        editor.setContent(saved);

                    }
                });


                editor.on('change keyup input', function () {
                    updateHiddenField(editor);
                });
            }
        });


        function updateHiddenField(editor) {
            var content = editor.getContent();
            document.getElementById('<%= hfEditorIcerik.ClientID %>').value = content;

            var msg = document.getElementById('lblMesaj');
            if (msg) msg.innerText = "";
        }


        function prepareSave() {
            var editor = tinymce.get('proeditor');
            if (editor) {
                updateHiddenField(editor);
            }
            return true;
        }


        document.addEventListener("DOMContentLoaded", function () {
            var btn = document.getElementById('<%= btnKaydet.ClientID %>');
            if (btn) {
                btn.onclick = prepareSave;
            }
        });

        function hideMessageAuto() {
            var msg = document.getElementById('lblMesaj');
            if (msg) {
                msg.style.opacity = "1";
                setTimeout(function () {
                    msg.style.transition = "opacity 1.5s ease";
                    msg.style.opacity = "0";
                    setTimeout(function () {
                        msg.innerText = "";
                        msg.style.opacity = "1";
                    }, 1500);
                }, 3000);
            }
        }

        function safeClearEditor() {
            try {
                var editor = tinymce.get('proeditor');
                if (editor) { editor.setContent(''); }
                document.getElementById('<%= hfEditorIcerik.ClientID %>').value = "";

            } catch (e) { console.error("Editor clear error:", e); }
        }

        function generatePreview() {
            var editor = tinymce.get('proeditor');
            if (!editor) return false;

            var content = editor.getContent();


            if (content.trim().length < 5) {
                Swal.fire({
                    title: '<%= gettext("warn_title", "Uyarı") %>',
                    text: '<%= gettext("warn_content_needed", "Önizleme için lütfen içerik giriniz.") %>',
                    icon: 'warning',
                    confirmButtonText: '<%= gettext("ok_button", "Tamam") %>',
                    confirmButtonColor: '#0F406B'
                });
                return false;
            }

            var pResult = document.getElementById('previewResult');
            var pBox = document.getElementById('previewBox');

            if (pResult && pBox) {
                pResult.innerHTML = content;
                pBox.style.display = 'block';

                pBox.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }

            return false;
        }

        function updateStatusUI() {
            var chk = document.getElementById('chkIsVisible');
            var note = document.getElementById('statusNote');
            var container = document.getElementById('divPublishStatus');
            var label = document.getElementById('labelStatus');


            if (chk && chk.checked) {
                container.className = 'status-container status-active';
                label.className = 'fw-bold text-active';
                note.innerText = lang_vis_all;
                note.style.color = "#2c7a7b";
            } else {
                container.className = 'status-container status-inactive';
                label.className = 'fw-bold text-inactive';
                note.innerText = lang_draft_mode;
                note.style.color = "#c53030";
            }
        }


        document.addEventListener("DOMContentLoaded", function () {
            var chk = document.getElementById('chkIsVisible');
            if (chk) {

                chk.addEventListener('change', updateStatusUI);

                updateStatusUI();
            }
        });


        function checkLength(textbox, maxLength, charCountId) {
            if (!textbox) return;

            var countSpan = document.getElementById(charCountId);


            if (textbox.value.length === 0) {
                countSpan.innerText = "";
                return;
            }


            if (textbox.value.length > maxLength) {
                textbox.value = textbox.value.substring(0, maxLength);
                alert(lang_limit_msg.replace("{0}", maxLength));
            }


            countSpan.innerText = (maxLength - textbox.value.length) + " " + lang_char_rem;
        }


        document.addEventListener("DOMContentLoaded", function () {
            var titleBox = document.getElementById('<%= txtKonuBaslik.ClientID %>');
            var titleEngBox = document.getElementById('<%= txtKonuBaslikEng.ClientID %>');
            var summaryBox = document.getElementById('<%= txtOzet.ClientID %>');


            if (titleBox) checkLength(titleBox, 250, 'charCountTitle');
            if (titleEngBox) checkLength(titleEngBox, 250, 'charCountTitle2');
            if (summaryBox) checkLength(summaryBox, 500, 'charCountSummary');
        });


    </script>
</asp:Content>

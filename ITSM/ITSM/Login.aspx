<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ITSM.Login" %>

<!DOCTYPE html>
<html lang="en" data-bs-theme="light" class="set-nav-dark" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <title>Login - SEÇİL KAUÇUK</title>
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no, maximum-scale=5">

    <link rel="icon" href="img/favicon-32x32.png" type="image/png">
    <link rel="stylesheet" media="screen, print" href="plugins/waves/waves.min.css">
    <link rel="stylesheet" media="screen, print" href="css/smartapp.min.css">
    <link rel="stylesheet" media="screen, print" href="webfonts/smartadmin/sa-icons.css">
    <link rel="stylesheet" media="screen, print" href="webfonts/fontawesome/fontawesome.css">

    <style>
        body, html {
            height: 100%;
            margin: 0;
        }

        .hero-section {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            position: relative;
        }

        #net {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: 0;
        }

        .btn-primary {
            background-color: #4a90e2 !important;
            border-color: #4a90e2 !important;
            font-weight: 600;
            transition: all 0.3s ease;
        }

            .btn-primary:hover {
                background-color: #357abd !important;
                border-color: #357abd !important;
                box-shadow: 0 4px 12px rgba(74, 144, 226, 0.3);
            }

        .form-control:focus {
            border-color: #4a90e2 !important;
            box-shadow: 0 0 0 0.25 margin rgba(74, 144, 226, 0.25) !important;
        }

        .navbar-brand-custom {
            display: flex;
            align-items: center;
            gap: 12px;
            text-decoration: none !important;
            margin-left: -55px;
        }

        .container-sola-yasla {
            padding-left: 0 !important;
        }

        .brand-logo {
            height: 40px;
            width: auto;
        }

        .brand-text {
            font-size: 1.5rem;
            font-weight: 800;
            color: #fff;
            letter-spacing: 1px;
            margin-bottom: 0;
        }

        .login-card {
            position: relative;
            z-index: 10;
            max-width: 450px;
            width: 100%;
        }

        .translucent-dark {
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.1);
        }

        .brand-text, h2, label, p {
            transition: all 0.5s ease;
            cursor: default;
        }

        .navbar-brand-custom:hover .brand-text {
            color: #e2e8f0 !important;
        }

        .brand-logo:hover {
            filter: opacity(0.85);
            cursor: pointer;
        }

        h2.text-white:hover {
            color: #f1f5f9 !important;
            cursor: pointer;
        }

        label:hover {
            color: #cbd5e0 !important;
        }

        .login-header-flex {
            display: flex;
            justify-content: center;
            align-items: center;
            width: 100%;
            margin-bottom: 30px;
        }

        .login-logo {
            max-width: 120px;
            height: auto;
            display: block;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">


        <section class="hero-section position-relative overflow-hidden">
            <div id="net"></div>

            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-11 col-md-8 col-lg-6 col-xl-4">
                        <div id="regular-login" class="login-card p-4 p-md-5 bg-dark bg-opacity-50 translucent-dark rounded-4 shadow-lg">
                            <div class="login-header-flex">
                                <a href="Default.aspx" class="app-logo" style="display: flex; align-items: center; padding: 10px;">
                                    <img src="img/login-logo.jpg" alt="logo" style="width: 300px; height: 120px; mix-blend-mode: screen;">
                                </a>
                            </div>
                            <p class="text-center text-white opacity-50 mb-4"><%= gettext("logininfo","Please enter your username and password")%> </p>

                            <div class="mb-3">
                                <label for="txtUsername" class="form-label text-white"><%= gettext("username","Username")%></label>
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control form-control-lg text-white bg-dark border-light border-opacity-25 bg-opacity-25"></asp:TextBox>
                            </div>

                            <div class="mb-4">
                                <label for="txtPassword" class="form-label text-white"><%= gettext("pass","Password")%></label>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
                                    CssClass="form-control form-control-lg text-white bg-dark border-light border-opacity-25 bg-opacity-25"></asp:TextBox>
                            </div>

                            <div class="d-grid">
                                <asp:LinkButton ID="btnLogin" runat="server" CssClass="btn btn-primary btn-lg shadow-sm" OnClick="btnLogin_Click">
                                    <%= gettext("signinbtn","Sign In")%>
                                </asp:LinkButton>
                            </div>
                            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label><br />
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </form>

    <script src="plugins/bootstrap/bootstrap.bundle.min.js"></script>
    <script src="plugins/waves/waves.min.js"></script>
    <script src="plugins/three/three.min.js"></script>
    <script src="plugins/vanta/vanta.net.min.js"></script>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            if (typeof VANTA !== 'undefined') {
                VANTA.NET({
                    el: "#net",
                    mouseControls: true,
                    touchControls: true,
                    gyroControls: false,
                    minHeight: 200.00,
                    minWidth: 200.00,
                    scale: 1.00,
                    scaleMobile: 1.00,
                    color: 0x22abf1,
                    backgroundColor: 0x111344,
                    points: 12.00,
                    maxDistance: 20.00,
                    spacing: 15.00
                });
            }
        });
    </script>
</body>
</html>

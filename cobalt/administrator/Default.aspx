<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Cobalt.administrator.Default" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Admin Panel</title>
    <link href="../theme/vista/css/bootstrap.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" EnablePartialRendering="true"
        runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <div class="navbar navbar-inverse">
        <div class="navbar-inner">
            <div class="container">
                <a class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse"><span
                    class="icon-bar"></span><span class="icon-bar"></span><span class="icon-bar"></span>
                </a><a class="brand" href="#">Project name</a>
                <div class="nav-collapse">
                    <div class="nav-collapse subnav-collapse">
                        <ul class="nav">
                            <li><a href="#">Home</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li class="dropdown"><a data-toggle="dropdown" class="dropdown-toggle" href="#">Dropdown
                                <b class="caret"></b></a>
                                <ul class="dropdown-menu">
                                    <li><a href="#">Action</a></li>
                                    <li><a href="#">Another action</a></li>
                                    <li><a href="#">Something else here</a></li>
                                    <li class="divider"></li>
                                    <li class="nav-header">Nav header</li>
                                    <li><a href="#">Separated link</a></li>
                                    <li><a href="#">One more separated link</a></li>
                                </ul>
                            </li>
                        </ul>
                        <div class="navbar-search pull-left">
                            <asp:TextBox ID="TextBox1" CssClass="search-query" runat="server" placeholder="Search"></asp:TextBox>
                        </div>
                        <ul class="nav pull-right">
                            <li><a href="#">Link</a></li>
                            <li class="divider-vertical"></li>
                            <li class="dropdown"><a data-toggle="dropdown" class="dropdown-toggle" href="#">Dropdown
                                <b class="caret"></b></a>
                                <ul class="dropdown-menu">
                                    <li><a href="#">Action</a></li>
                                    <li><a href="#">Another action</a></li>
                                    <li><a href="#">Something else here</a></li>
                                    <li class="divider"></li>
                                    <li><a href="#">Separated link</a></li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="container">
        <div class="row">
            <div class="span3">
                <div class="well sidebar-nav">
                    <ul class="nav nav-list">
                        <li class="nav-header">Site Settings</li>
                        <li>
                            <asp:LinkButton ID="seo_settings" runat="server" OnClick="seo_settings_Click"><i class="icon-search"></i>SEO Settings</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="email_settings" runat="server" OnClick="email_settings_Click"><i class="icon-envelope"></i>E-mail Settings</asp:LinkButton></li>
                        <li>
                        <li>
                            </li>
                        <li class="nav-header">Manage Listings</li>
                        <li>
                            <asp:LinkButton ID="add_listing" runat="server" OnClick="add_listing_Click">Add Listing</asp:LinkButton></li>
                    <li class="nav-header">Manage Users</li>
                        <li>
                            <asp:LinkButton ID="add_user" runat="server" OnClick="add_user_Click">Add User</asp:LinkButton></li>                  
                    <li>
                            <asp:LinkButton ID="users_list" runat="server" OnClick="users_list_Click">Users</asp:LinkButton></li>                  
                    
                    </ul>
                </div>
            </div>
            <div class="span9">
                <div class="row">
                    <div class="span5">
                        <asp:UpdateProgress ID="UpdateProgress1" DisplayAfter="0" DynamicLayout="true" runat="server">
                            <ProgressTemplate>
                                <div class="loading">
                                    <img src="../theme/vista/img/uploader.gif" />
                                    Loading...
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:UpdatePanel ID="ajax_page" runat="server">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="seo_settings" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="email_settings" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="add_listing" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="add_user" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="users_list" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
    <script src="../theme/vista/js/jquery.js" type="text/javascript"></script>
    <script src="../theme/vista/js/bootstrap.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            // $("li:first-child").addClass("test");
            $('li').click(function () {
                $('.nav li').removeClass('active');
                $(this).addClass('active');
            });
        });
    </script>
</body>
</html>
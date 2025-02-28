<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Assign_Client.aspx.cs" Inherits="Assign_Client" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Assign Client</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_header" runat="Server">
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
    <meta charset="utf-8" />
    <script src="js/jquery-1.12.3.min.js"></script>
    <script src="Scripts/jquery-1.11.3.js"></script>
    <script src="js/bootstrap.js" type="text/javascript"></script>
    <script src="Scripts/datetimepicker.js"></script>
    <script src="Scripts/jquery-ui-1.8.20.min.js"></script>
    <script src="Scripts/jquery-ui-1.8.20.js"></script>
    <script src="Scripts/jquery-1.7.1.js"></script>
    <script src="Scripts/jquery-ui.min.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <script src="js/jquery.blockUI.js"></script>
    <link href="Scripts/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/jquery-ui.css" rel="stylesheet" />


    <link href="css/new_stylesheet.css" rel="stylesheet" />

    <link href="css/GridViewFreezeStyle.css" rel="stylesheet" type="text/css" />

    <style>
        .container {
            max-width: 99%;
        }

        .label_text {
            font-size: 14px;
            font-weight: bold;
            margin-bottom: 10px;
        }

        .text_box {
            margin-top: 7px;
        }

        .auto-style1 {
            color: #FFFFFF;
        }

        h2 {
            border-radius: 5px;
        }

        .button {
            background-color: #D3D3D3;
            border: none;
            color: white;
            /*padding: 10px;*/
            text-align: center;
            text-decoration-color: black;
            text-decoration: black;
            display: inline-block;
            font-size: 12px;
            /*margin: 10px 5px;*/
            cursor: pointer;
            border-radius: 8px;
            border-color: #00008B;
            border-width: 1px;
            border-style: solid;
        }

        .grid-view {
            height: auto;
            max-height: 300px;
            overflow: scroll;
        }

        h5 {
            font-weight: bold;
            font-size: 15px;
        }
    </style>

    <script>

        $(function () {

            $('#<%=btnSubmit.ClientID%>').click(function () {
                   if (validate()) {
                       $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
                   }
               });
           });
    </script>
    <script type="text/javascript">
        window.onfocus = function () {
            $.unblockUI();

        }
        function validate() {
            var e = document.getElementById("<%=ddl_client.ClientID%>");
            if (e.options[e.selectedIndex].value == "Select") {
                alert("Please Select Employee.");
                e.focus();
                return false;
            }
            return true;

        }
        function openWindow() {
            window.open("html/travelling_managment.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
        }

        function pageLoad(sender, args) {

        }
        $(document).ready(function () {

            $(".check_list").hide();
            $(".check_dept").click(function () {

                $(".check_list").toggle();

                $(".listbox_left").hide();

            });

            $(".listbox_left").show();
        });
    </script>
    <%--  <script type="text/javascript">  
        $(document).ready(
            function ()
            {
                $('#btnleft').click(
                    function (e) {
                        $('#lstLeft > option:selected').appendTo('#lstRight');
                        e.preventDefault();
                    });

                $('#btnright').click(
              function (e) {
                  $('#lstRight > option:selected').appendTo('#lstLeft');
                  e.preventDefault();
              });
            });
            
       </script>--%>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cph_righrbody" runat="Server">

    <div class="container-fluid">


        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <%--  <asp:UpdatePanel runat="server" ID="update">
            <ContentTemplate>--%>
        <asp:Panel ID="Panel3" runat="server" CssClass="panel panel-primary">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-9">
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase" font-size="25px"><b>Assign Client</b></div>
                    </div>
                    <div class="col-sm-2 text-right">

                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <div class="panel-body">
                <div class="row">

                    <div class="col-sm-2 col-xs-12" style="margin-top: 10px;">
                        <asp:Label ID="lbl_client" runat="server" Text="Client List" Font-Bold="true"></asp:Label>
                    </div>
                    <div class="col-sm-2 col-xs-12 ">
                        <asp:RadioButton ID="rdb_branch_list" runat="server" Text="&nbsp&nbspBranch List" OnCheckedChanged="rdb_branch_list_CheckedChanged" AutoPostBack="true" />
                    </div>
                    <div class="col-md-2 col-sm-3 col-xs-12">
                        <asp:RadioButton ID="rdb_employee" runat="server" GroupName="group" Text="&nbsp&nbspEmployee List" OnCheckedChanged="rdb_employee_CheckedChanged" AutoPostBack="true" />

                    </div>
                    <div class="col-sm-2 col-xs-12 "></div>

                    <div class="col-md-3 col-sm-3 col-xs-12" style="margin-top: 10px;">
                        <%--<asp:CheckBox ID="checkemp" runat="server" Text="&nbsp&nbspEmployee List" OnCheckedChanged="checkemp_click" AutoPostBack="true" />--%>
                    </div>
                    <%-- <div class="col-md-2 col-sm-3 col-xs-12"style="margin-top:10px;">
                               <%-- <asp:CheckBox ID="CheckDepartment" class="check_dept" runat="server" Text="&nbsp&nbspDepartment List" OnCheckedChanged="CheckDepartment_click" AutoPostBack="true" />

                               <asp:RadioButton ID="CheckDepartment" runat="server" GroupName="group" Text="&nbsp&nbspClient List" OnCheckedChanged="CheckDepartment_click" AutoPostBack="true" />

                            </div>
                            <div class="col-md-2 col-sm-3 col-xs-12"style="margin-top:10px;">
                             <%--   <asp:CheckBox ID="ChecUnit" runat="server" Text="&nbsp&nbspUnit List" OnCheckedChanged="ChecUnit_click" AutoPostBack="true" />

                                 <asp:RadioButton ID="ChecUnit" runat="server" GroupName="group" Text="&nbsp&nbspBranch List" OnCheckedChanged="ChecUnit_click" AutoPostBack="true" />
                            </div>--%>
                </div>
                <br />
                <asp:Panel ID="employeepanel" runat="server">


                    <div class="row">
                        <div class="col-sm-2 col-xs-12 " style="border: 1px;">
                            <asp:ListBox ID="ddl_client" runat="server" SelectionMode="Multiple" Height="300" class="form-control text_box"></asp:ListBox>
                        </div>
                        <div class="col-sm-2 col-xs-12 ">
                            <asp:ListBox ID="lst_branch" runat="server" SelectionMode="Multiple" Height="300" class="form-control text_box"></asp:ListBox>
                        </div>
                        <div class="col-sm-3 col-xs-12 ">
                            <asp:ListBox ID="lstLeft" runat="server" SelectionMode="Multiple" Width="100%" Visible="true"
                                Height="300"></asp:ListBox>

                        </div>

                        <div class="col-sm-1 col-xs-12 ">
                            <br />
                            <br />
                            <div class="row">
                                <asp:Button ID="brnallleft" value=">>" OnClick="brnallleft_Click" runat="server" CssClass="button" Text=">>" />
                            </div>
                            <br />
                            <div class="row">
                                <asp:Button ID="btnright" value="<" OnClick="btnright_click" runat="server" CssClass="button" Text=">" />
                            </div>
                            <br />
                            <div class="row">
                                <asp:Button ID="btnleft" value="<" OnClick="btnleft_click" runat="server" CssClass="button" Text="<" />
                            </div>
                            <br />
                            <div class="row">
                                <asp:Button ID="allriht" value="<<" OnClick="allriht_click" runat="server" CssClass="button" Text="<<" />
                            </div>




                        </div>
                        <div class="col-sm-3 col-xs-12">
                            <asp:ListBox ID="lstRight" DataTextField="emp_name" DataValueField="emp_code" runat="server" SelectionMode="Multiple" Width="100%"
                                Height="300"></asp:ListBox>
                        </div>
                    </div>
                </asp:Panel>
                <br />
                <div class="row text-center">
                    <asp:Button ID="btnSubmit" Text="Submit" runat="server" OnClick="btnSubmit_click" OnClientClick="return validate();" Class="btn btn-primary" />
                    <asp:Button ID="btnclose" Text="Close" runat="server" class="btn btn-danger" OnClick="btnclose_click" />
                </div>

            </div>
        </asp:Panel>
        <%--   </ContentTemplate>
        </asp:UpdatePanel>--%>
    </div>
</asp:Content>





<%@ Page Language="C#" AutoEventWireup="true" CodeFile="assign_billing.aspx.cs" Inherits="Travelling_Management" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Assign Policy</title>
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
            var e = document.getElementById("<%=ddlpolicies.ClientID%>");
            if (e.options[e.selectedIndex].value == 0) {
                alert("Please Select Policy.");
                e.focus();
                return false;
            }
            return true;

        }
        function openWindow() {
            window.open("html/travelling_managment.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
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
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>ASSIGN POLICY</b></div>
                    </div>
                    <div class="col-sm-2 text-right">

                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <div class="panel-body">
                <br />
                <br />
                <div class="row">
                    <div class="col-lg-2 col-md-2 col-sm-3 col-xs-12">
                        Policies :
                     <asp:DropDownList AppendDataBoundItems="true" ID="ddlpolicies" runat="server" DataTextField="txt_policy_name" DataValueField="id" class="form-control  dropdown-toggle" ItemType="button" data-toggle="dropdown" OnSelectedIndexChanged="ddlpolicies_SelectedIndexChanged" AutoPostBack="true">
                         <asp:ListItem Value="0">Select Policy</asp:ListItem>
                     </asp:DropDownList>
                    </div>
                    <div class="col-lg-2 col-md-2 col-sm-3 col-xs-12">
                        Client Name :
                        <asp:DropDownList ID="ddl_client" DataValueField="client_code" DataTextField="client_name" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                    </div>
                </div>
                <br />
                <asp:Panel ID="employeepanel" runat="server">
                    <div class="row">
                        <div class="col-sm-3 col-xs-12 " style="margin-left: 170px">
                            <asp:ListBox ID="lstLeft" runat="server" DataTextField="unit_name" DataValueField="unit_code" SelectionMode="Multiple" Width="100%" Visible="true"
                                Height="300"></asp:ListBox>

                        </div>
                        <div class="col-sm-1 col-xs-12 "></div>
                        <div class="col-sm-2 col-xs-12 ">
                            <br />
                            <br />
                            <div class="row">
                                <asp:Button ID="brnallleft" value=">>" OnClick="brnallleft_Click" runat="server" Class="btn btn-primary" Text=">>" Style="margin-left: 25px; font-weight: bold;" />
                            </div>
                            <br />
                            <br />
                            <br />
                            <div class="row">
                                <asp:Button ID="btnright" value="<" OnClick="btnright_click" runat="server" Class="btn btn-primary" Style="padding-left: 20px; text-align: center; margin-left: 25px; font-weight: bold;" Text=">" />
                            </div>
                            <br />
                            <br />
                            <br />
                            <div class="row">
                                <asp:Button ID="btnleft" value="<" OnClick="btnleft_click" runat="server" Text="<" Style="padding-left: 20px; text-align: center; margin-left: 25px; font-weight: bold;" Class="btn btn-primary" />
                            </div>
                            <br />
                            <br />
                            <br />
                            <div class="row">
                                <asp:Button ID="allriht" value="<<" OnClick="allriht_click" runat="server" Text="<<" Class="btn btn-primary" Style="margin-left: 25px; font-weight: bold;" />
                            </div>
                            <br />
                            <br />
                            <br />

                        </div>
                        <div class="col-sm-3 col-xs-12">
                            <asp:ListBox ID="lstRight" DataTextField="unit_name" DataValueField="unit_code" runat="server" SelectionMode="Multiple" Width="100%"
                                Height="300"></asp:ListBox>
                        </div>
                    </div>
                </asp:Panel>
                <br />
                <br />
                <div class="row text-center">
                    <asp:Button ID="btnSubmit" Text="Save" runat="server" OnClick="btnSubmit_click" OnClientClick="return validate();" Class="btn btn-primary" Style="font-weight: bold;" />
                    <asp:Button ID="btnclose" Text="Close" runat="server" class="btn btn-danger" OnClick="btnclose_click" Style="font-weight: bold;" />
                </div>

            </div>
        </asp:Panel>
        <%--   </ContentTemplate>
        </asp:UpdatePanel>--%>
    </div>
</asp:Content>





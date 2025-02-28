<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Assign_Attendance_Policy.aspx.cs" Inherits="Assign_Attandance_Ploicy" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Assing Attandance Policy</title>
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
        #ctl00_cph_righrbody_btnSubmit {
            margin-left: 32em;
        }

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
        function unblock() {
            $.unblockUI();
        }


        $(function () {
            $('#<%=btnSubmit.ClientID%>').click(function () {
                   if (validate()) {
                       $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
                   }
               });
               $('#<%=Button1.ClientID%>').click(function () {

                   $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

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
                alert("Please select Policy.");
                e.focus();
                return false;
            }
            return true;

        }
        function openWindow() {
            window.open("html/assign_attendance_policy.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
        }

        function pageLoad(sender, args) {
            $(document).ready(function () {
                $('#<%=checkemp.ClientID%>').change(function () {
                    if (this.checked) {
                        document.getElementById("<%=CheckDepartment.ClientID%>").checked = false;
                        document.getElementById("<%=ChecUnit.ClientID%>").checked = false;
                    }
                });
                $('#<%=CheckDepartment.ClientID%>').change(function () {
                    if (this.checked) {
                        document.getElementById("<%=checkemp.ClientID%>").checked = false;
                        document.getElementById("<%=ChecUnit.ClientID%>").checked = false;
                    }
                });
                $('#<%=ChecUnit.ClientID%>').change(function () {
                    if (this.checked) {
                        document.getElementById("<%=CheckDepartment.ClientID%>").checked = false;
                        document.getElementById("<%=checkemp.ClientID%>").checked = false;
                    }
                });
            });
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
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>Assign Attandance Policy</b></div>
                    </div>
                    <div class="col-sm-2 text-right">

                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <br />
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-4"></div>
                    <div class="col-sm-9">
                        <div style="text-align: left; color:white; font-size: small;"><b>Assign Attandance Policy Details</b></div>
                    </div>
                    <div class="col-sm-2 text-left">
                        
                    </div>
                </div>
            </div>

            <div class="panel-body">
                <div class="container-fluid" style="background: #f3f1fe; border: 1px solid #e2e2dd; border-radius: 10px; padding:25px 25px 25px 25px; margin-bottom:20px; margin-top:20px">
                <div class="row">

                    <div class="col-md-2 col-sm-3 col-xs-12" style="margin-top: 10px;"><b>Select Policies :</b></div>
                    <div class="col-md-2 col-sm-3 col-xs-12">

                        <asp:DropDownList AppendDataBoundItems="true" ID="ddlpolicies" runat="server" DataTextField="txt_policy_name" DataValueField="id" class="form-control text_box" OnSelectedIndexChanged="ddlpolicies_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="0">Select Policy</asp:ListItem>
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataSource6" runat="server" ConnectionString="<%$ ConnectionStrings:CELTPAYConnectionString %>"
                            ProviderName="<%$ ConnectionStrings:celtpayConnectionString.ProviderName %>"
                            SelectCommand="SELECT POLICY_NAME FROM attandance_police_master  WHERE comp_code=@comp_code  and submit=1">
                            <SelectParameters>
                                <asp:SessionParameter Name="comp_code" SessionField="comp_code" />
                                <asp:SessionParameter Name="UNIT_CODE" SessionField="UNIT_CODE" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </div>
                    <div class="col-md-1  col-sm-3 col-xs-12"></div>
                    <div class=" col-md-2 col-sm-3 col-xs-12" style="margin-top: 10px;">
                        <%--<asp:CheckBox ID="checkemp" runat="server" Text="&nbsp&nbspEmployee List" OnCheckedChanged="checkemp_click" AutoPostBack="true" />--%>
                        <asp:RadioButton ID="checkemp" runat="server" GroupName="group" Text="&nbsp&nbspEmployee List" OnCheckedChanged="checkemp_click" AutoPostBack="true" />
                    </div>
                    <div class="col-md-2 col-sm-3 col-xs-12" style="margin-top: 10px;">
                        <%-- <asp:CheckBox ID="CheckDepartment" class="check_dept" runat="server" Text="&nbsp&nbspDepartment List" OnCheckedChanged="CheckDepartment_click" AutoPostBack="true" />--%>

                        <asp:RadioButton ID="CheckDepartment" runat="server" GroupName="group" Text="&nbsp&nbspClient List" OnCheckedChanged="CheckDepartment_click" AutoPostBack="true" />

                    </div>
                    <div class="col-md-2 col-sm-3 col-xs-12" style="margin-top: 10px;">
                        <%--   <asp:CheckBox ID="ChecUnit" runat="server" Text="&nbsp&nbspUnit List" OnCheckedChanged="ChecUnit_click" AutoPostBack="true" />--%>

                        <asp:RadioButton ID="ChecUnit" runat="server" GroupName="group" Text="&nbsp&nbspBranch List" OnCheckedChanged="ChecUnit_click" AutoPostBack="true" />
                    </div>

                </div>
                <br />
                <asp:Panel ID="employeepanel" runat="server">
                    <asp:SqlDataSource ID="SqlDataSource" runat="server"
                        ConnectionString="<%$ ConnectionStrings:CELTPAYConnectionString %>" ProviderName="<%$ ConnectionStrings:celtpayConnectionString.ProviderName %>"
                        SelectCommand="SELECT DEPT_NAME FROM pay_department_master WHERE (comp_code = @comp_code) ">
                        <SelectParameters>
                            <asp:SessionParameter Name="comp_code" SessionField="comp_code" />
                        </SelectParameters>
                    </asp:SqlDataSource>

                    <br />
                    <br />
                    <div class="row">
                        <div class="col-sm-3 col-xs-12 " style="border: 1px;">
                            <asp:CheckBoxList ID="checklistcox" runat="server" SelectionMode="Multiple" Width="100%"
                                Height="300" OnSelectedIndexChanged="checklistcox_OnSelectedIndexChanged" AutoPostBack="true">
                            </asp:CheckBoxList>
                        </div>
                        <div class="col-sm-3 col-xs-12 ">
                            <asp:ListBox ID="lstLeft" runat="server" DataTextField="emp_name" DataValueField="emp_code" SelectionMode="Multiple" Width="100%" Visible="true"
                                Height="300"></asp:ListBox>

                        </div>
                        <div class="col-sm-1 col-xs-12 "></div>
                        <div class="col-sm-2 col-xs-12 ">
                            <br />
                            <br />
                            <div class="row">
                                <asp:Button ID="brnallleft" value=">>" OnClick="brnallleft_Click" runat="server" Text=">>" Class="btn btn-primary" Style="padding-left: 1em; margin-left: 3em;" />
                            </div>
                            <br />
                            <div class="row">
                                <asp:Button ID="btnright" value="<" OnClick="btnright_click" runat="server" Text=">" Class="btn btn-primary" Style="padding-left: 17px; margin-left: 3em;" />
                            </div>
                            <br />
                            <div class="row">
                                <asp:Button ID="btnleft" value="<" OnClick="btnleft_click" runat="server" Text="<" Class="btn btn-primary" Style="padding-left: 17px; margin-left: 3em;" />
                            </div>
                            <br />
                            <div class="row">
                                <asp:Button ID="allriht" value="<<" OnClick="allriht_click" runat="server" Text="<<" Class="btn btn-primary" Style="padding-left: 1em; margin-left: 3em;" />
                            </div>
                            <br />



                        </div>
                        <div class="col-sm-3 col-xs-12">
                            <asp:ListBox ID="lstRight" DataTextField="emp_name" DataValueField="emp_code" runat="server" SelectionMode="Multiple" Width="100%"
                                Height="300"></asp:ListBox>
                        </div>
                    </div>
                </asp:Panel>
                <br />
                    <br />
                    <br />
                <div class="row text-center">
                    <asp:Button ID="btnSubmit" Text="Submit" runat="server" OnClick="btnSubmit_click" OnClientClick="return validate();" Class="btn btn-primary" />
                    <asp:Button ID="btnclose" Text="Close" runat="server" class="btn btn-danger" OnClick="btnclose_click" />
                    <asp:Button ID="Button1" Text="Calculation" runat="server" OnClick="btnsalarycalculation_Click" Class="btn btn-large" />
                </div>
                    <br />
                    </div>

            </div>
        </asp:Panel>
        <%--   </ContentTemplate>
        </asp:UpdatePanel>--%>
    </div>
</asp:Content>





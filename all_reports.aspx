<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="all_reports.aspx.cs" Inherits="all_reports" Title="Get Reports" EnableEventValidation="false" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Get Reports</title>
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
    <script src="js/select2.min.js"></script>
    <link href="css/select2.min.css" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <link href="css/GridViewFreezeStyle.css" rel="stylesheet" type="text/css" />
    <link href="datatable/dataTables.bootstrap.min.css" rel="stylesheet" />
    <link href="datatable/buttons.bootstrap.min.css" rel="stylesheet" />
    <script src="datatable/jquery.dataTables.min.js"></script>
    <script src="datatable/dataTables.bootstrap.min.js"></script>
    <script src="datatable/dataTables.buttons.min.js"></script>
    <script src="datatable/buttons.bootstrap.min.js"></script>
    <script src="datatable/vfs_fonts.js"></script>
    <script src="datatable/buttons.html5.min.js"></script>
    <script src="datatable/buttons.print.min.js"></script>
    <script src="datatable/buttons.colVis.min.js"></script>
    <script src="datatable/vfs_fonts.js"></script>
    <script src="datatable/buttons.html5.min.js"></script>
    <script src="datatable/buttons.print.min.js"></script>
    <script src="datatable/buttons.colVis.min.js"></script>
    <script src="datatable/pdfmake.min.js"></script>


    <script type="text/javascript">
        function pageLoad() {
            //$(".date-picker1").val("");
            // $(".date-picker2").val("");

            $(".date-picker1").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                yearRange: '1950',
                onSelect: function (selected) {
                    $(".date-picker2").datepicker("option", "minDate", selected)
                }
            });


            $(".date-picker2").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                yearRange: '1950',
                onSelect: function (selected) {
                    $(".date-picker1").datepicker("option", "maxDate", selected)
                }
            });
            $(".date-picker1").attr("readonly", "true");
            $(".date-picker2").attr("readonly", "true");

            $('.date-picker12').datepicker({
                changeMonth: true,
                changeYear: true,
                maxDate: 0,
                yearRange: "1990:+100",
                showButtonPanel: true,
                dateFormat: 'mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                }
            });
            $('.date-picker12').focus(function () {
                $(".ui-datepicker-calendar").hide();

            });
            $(".date-picker12").attr("readonly", "true");
            bill_check();
        }


        function Req_validation() {
            var t_client = document.getElementById('<%=ddl_client.ClientID %>');
            var Selectedclient = t_client.options[t_client.selectedIndex].text;

            if (Selectedclient == "Select") {
                alert("Please Select Client.");
                t_client.focus();
                return false;
            }
        }

        function AllowAlphabet_Number10(e) {
            if (null != e) {

                isIE = document.all ? 1 : 0
                keyEntry = !isIE ? e.which : e.keyCode;
                if (((keyEntry >= '65') && (keyEntry <= '90')) || ((keyEntry >= '97') && (keyEntry <= '122')) || (keyEntry < '31') || (keyEntry == '32') || (keyEntry == '9') || (keyEntry == '46') || (keyEntry == '44'))

                    return true;
                else {
                    // alert('Please Enter Only Character values.');
                    return false;
                }
            }
        }
        function AllowAlphabet_address(e) {
            if (null != e) {
                isIE = document.all ? 1 : 0
                keyEntry = !isIE ? e.which : e.keyCode;
                if (((keyEntry >= '65') && (keyEntry <= '90')) || ((keyEntry >= '97') && (keyEntry <= '122')) || (keyEntry < '31') || ((keyEntry >= '48') && (keyEntry <= '57')) ||
                    (keyEntry == '32') || (keyEntry == '38') || ((keyEntry == '39') && (keyEntry == '34')) || (keyEntry == '44') || ((keyEntry >= '45') && (keyEntry <= '47')) ||
                    (keyEntry == '58') || (keyEntry == '59') || (keyEntry == '61') || (keyEntry == '92'))
                    return true;
                else {
                    // alert('Please Enter Only Character values.');
                    return false;
                }
            }
        }
        function isNumber_dot(evt) {
            if (null != evt) {
                evt = (evt) ? evt : window.event;

                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && (charCode < 48 || charCode > 57) && (charCode < 46 || charCode > 46)) {

                    return false;

                }

            }
            return true;
        }

        function valid_gst() {
            var gst_from_date = document.getElementById('<%=gst_from_date.ClientID %>');
            var gst_to_date = document.getElementById('<%=gst_to_date.ClientID %>');
            if (gst_from_date.value == "") {
                alert("Please Select From Month");
                gst_from_date.focus();
                return false;
            }
            if (gst_to_date.value == "") {
                alert("Please Select To Month");
                gst_to_date.focus();
                return false;
            }

            var ddl_gst_type = document.getElementById('<%=ddl_gst_type.ClientID %>');
            var Selected_ddl_gst_type = ddl_gst_type.options[ddl_gst_type.selectedIndex].text;
            if (Selected_ddl_gst_type == "Select") {
                alert("Please Select Bill Type ");
                ddl_gst_type.focus();
                return false;
            }
        }


        function validate() {
            var month_date = document.getElementById('<%=txt_date.ClientID %>');

            if (month_date.value == "") {
                alert("Please Select  Month");
                month_date.focus();
                return false;
            }

            //var t_client = document.getElementById('<%=ddl_client.ClientID %>');
            //var Selected_ddl_client = ddl_gst_type.options[ddl_client.selectedIndex].text;
            //var Selectedclient = t_client.options[t_client.selectedIndex].text;
            //ddl_client

            //  if (Selectedclient == "Select") {
            //  alert("Please Select Client.");
            //t_client.focus();
            //  return false;
        }




        function openWindow() {
            window.open("html/reports_main.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
        }

        $(document).ready(function () {
            var st = $(this).find("input[id*='hidtab']").val();
            if (st == null)
                st = 0;
            $('[id$=tabs]').tabs({ selected: st });


            var table = $('#<%=gv_attendance.ClientID%>').DataTable({
                "responsive": true,
                "sPaginationType": "full_numbers",
                buttons: [
                    {
                        extend: 'csv',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'print',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'copyHtml5',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    'colvis'
                ]

            });

            table.buttons().container()
                .appendTo('#<%=gv_attendance.ClientID%>_wrapper .col-sm-6:eq(0)');
            $.fn.dataTable.ext.errMode = 'none';



            /////
            var table = $('#<%=gv_state_attendance.ClientID%>').DataTable({
                "responsive": true,
                "sPaginationType": "full_numbers",
                buttons: [
                    {
                        extend: 'csv',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'print',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'copyHtml5',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    'colvis'
                ]

            });

            table.buttons().container()
                .appendTo('#<%=gv_state_attendance.ClientID%>_wrapper .col-sm-6:eq(0)');
            $.fn.dataTable.ext.errMode = 'none';


            //Sachin Start MD approve grid CSV
            var table = $('#<%=gv_Md_Approve.ClientID%>').DataTable({
                "responsive": true,
                "sPaginationType": "full_numbers",
                buttons: [
                    {
                        extend: 'csv',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'print',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'copyHtml5',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    'colvis'
                ]

            });

            table.buttons().container()
                .appendTo('#<%=gv_Md_Approve.ClientID%>_wrapper .col-sm-6:eq(0)');
            $.fn.dataTable.ext.errMode = 'none';
            //END


        });
        function monthly_paid_report_val() {

            var ddl_type = document.getElementById('<%=ddl_type.ClientID %>');
            var Selected_ddl_type = ddl_type.options[ddl_type.selectedIndex].text;

            if (Selected_ddl_type == "Select") {
                alert("Please Select Type");
                ddl_type.focus();
                return false;
            }
            var ddl_type_client = document.getElementById('<%=ddl_type_client.ClientID %>');
            var Selected_ddl_type_client = ddl_type_client.options[ddl_type_client.selectedIndex].text;

            if (Selected_ddl_type_client == "Select") {
                alert("Please Select Client Type");
                ddl_type_client.focus();
                return false;
            }

            var txt_payment_date_from = document.getElementById('<%=txt_payment_date_from.ClientID %>');
            if (txt_payment_date_from.value == "") {
                alert("Please Select From Date");
                txt_payment_date_from.focus();
                return false;
            }

            var txt_payment_date_to = document.getElementById('<%=txt_payment_date_to.ClientID %>');
            if (txt_payment_date_to.value == "") {
                alert("Please Select To Date");
                txt_payment_date_to.focus();
                return false;
            }
        }

        function tally_report_val() {

            var ddl_type = document.getElementById('<%=ddl_type_tally.ClientID %>');
            var Selected_ddl_type = ddl_type.options[ddl_type.selectedIndex].text;

            if (Selected_ddl_type == "Select") {
                alert("Please Select Type");
                ddl_type.focus();
                return false;
            }
            var ddl_select_type = document.getElementById('<%=ddl_select_type.ClientID %>');
            var Selected_ddl_select_type = ddl_select_type.options[ddl_select_type.selectedIndex].text;

            if (Selected_ddl_select_type == "Select") {
                alert("Please Select Client Type");
                ddl_select_type.focus();
                return false;
            }

            var txt_tally_from_date = document.getElementById('<%=txt_tally_from_date.ClientID %>');
            if (txt_tally_from_date.value == "") {
                alert("Please Select From Date");
                txt_tally_from_date.focus();
                return false;
            }

            var txt_tally_to_date = document.getElementById('<%=txt_tally_to_date.ClientID %>');
            if (txt_tally_to_date.value == "") {
                alert("Please Select To Date");
                txt_tally_to_date.focus();
                return false;
            }
        }
        function summary_payment_val() {

            var ddl_payment_report_type = document.getElementById('<%=ddl_payment_report_type.ClientID %>');
            var Selected_ddl_payment_report_type = ddl_payment_report_type.options[ddl_payment_report_type.selectedIndex].text;
            if (Selected_ddl_payment_report_type == "Select") {
                alert("Please Select Report Type");
                ddl_payment_report_type.focus();
                return false;
            }

            var ddl_type_payment = document.getElementById('<%=ddl_type_payment.ClientID %>');
            var Selected_ddl_type_payment = ddl_type_payment.options[ddl_type_payment.selectedIndex].text;

            var ddl_payment_client_vendor_name = document.getElementById('<%=ddl_payment_client_vendor_name.ClientID %>');
            var Selected_ddl_payment_client_vendor_name = ddl_payment_client_vendor_name.options[ddl_payment_client_vendor_name.selectedIndex].text;
            if (Selected_ddl_type_payment == "Select") {
                alert("Please Select Payment Type");
                ddl_type_payment.focus();
                return false;
            }
            if (Selected_ddl_payment_client_vendor_name == "Select") {

                if (Selected_ddl_type_payment != "VENDOR PAYMENT") {
                    alert("Please Select Client Name");
                    ddl_payment_client_vendor_name.focus();
                    return false;
                }
            }
            if (Selected_ddl_type_payment == "Select") {
                alert("Please Select Payment Type");
                ddl_type_payment.focus();
                return false;
            }

            var gst_from_month = document.getElementById('<%=gst_from_month.ClientID %>');
            if (gst_from_month.value == "") {
                alert("Please Select From Month");
                gst_from_month.focus();
                return false;
            }

            var gst_to_month = document.getElementById('<%=gst_to_month.ClientID %>');
            if (gst_to_month.value == "") {
                alert("Please Select To Year");
                gst_to_month.focus();
                return false;
            }
            return true;
        }

        function outstanding_required() {
            var txt_from_date = document.getElementById('<%=txt_out_from_month.ClientID %>');
            var txt_to_date = document.getElementById('<%=txt_out_to_month.ClientID %>');

            if (txt_from_date.value == "") {
                alert("Please Select From Date");
                txt_from_date.focus();
                return false;
            }

            if (txt_to_date.value == "") {
                alert("Please Select To Date");
                txt_to_date.focus();
                return false;
            }

        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cph_righrbody" runat="Server">
    <div class="container-fluid">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <asp:Panel ID="Panel3" runat="server" CssClass="panel panel-primary">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-9">
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>Get Reports</b></div>
                    </div>
                    <div class="col-sm-2 text-right">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <br />

            <div class="panel-body">
                <div class="container-fluid" style="background: #f3f1fe; border: 1px solid #e2e2dd; border-radius: 10px; padding: 25px 25px 25px 25px; margin-bottom: 20px; margin-top: 20px">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-sm-2 col-xs-12 text-left">
                                    <b>Select Month :</b>
                                    <asp:TextBox ID="txt_date" CssClass="form-control date-picker12" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12 text-left">
                                    <b>Client Name :   </b>
                                    <asp:DropDownList ID="ddl_client" class="form-control pr_state js-example-basic-single" runat="server" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12 text-left">
                                    <b>State Name :</b>
                                    <asp:DropDownList ID="ddl_state" runat="server" DataTextField="STATE" DataValueField="STATE" class="form-control text_box" OnSelectedIndexChanged="ddl_state_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12 text-left">
                                    <b>Branch Name : </b>
                                    <asp:DropDownList ID="ddl_unitcode" class="form-control pr_state js-example-basic-single" runat="server">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            <br />
            <br />
            <div id="tabs" style="background: #f3f1fe; padding: 20px 20px 20px 20px; border-radius: 10px">
                <asp:HiddenField ID="hf_lwf" runat="server" />
                <asp:HiddenField ID="hidtab" Value="0" runat="server" />
                <ul>
                    <li><a href="#menu1"><b>GST Reports</b></a></li>
                    <li><a href="#menu2"><b>Payment Paid Report</b></a></li>
                    <li><a href="#menu3"><b>Tally Report</b></a></li>
                    <li><a href="#menu4"><b>Payment Summary Report</b></a></li>
                    <li><a href="#menu5"><b>MIS Report</b></a></li>
                    <li><a href="#menu6"><b>Reject Bill Report</b></a></li>
                    <li><a href="#menu7"><b>Finance Copy</b></a></li>
                    <li><a href="#menu8"><b>Soft Copy Send Mail Status</b></a></li>
                    <li><a href="#menu9"><b>Attendance Report</b></a></li>
                    <li><a href="#menu10"><b>MD Approve Report</b></a></li>
                    <li><a href="#menu11"><b>Monthwise Outstanding</b></a></li>
                </ul>
                <div id="menu1">
                    <br />
                    <div class="row">
                        <div class="col-sm-1 col-xs-12">
                            <b>From Date :</b>
                            <asp:TextBox ID="gst_from_date" CssClass="form-control date-picker1" runat="server" Style="width: 105px;"></asp:TextBox>
                        </div>
                        <div class="col-sm-1 col-xs-12">
                            <b>To Date :</b>
                            <asp:TextBox ID="gst_to_date" CssClass="form-control date-picker2" runat="server" Style="width: 105px;"></asp:TextBox>
                        </div>
                        <div class="col-sm-2 col-xs-12">
                            <b>Select Bill Type</b>
                            <asp:DropDownList ID="ddl_gst_type" runat="server" class="form-control">
                                <asp:ListItem Value="Select">Select</asp:ListItem>
                                <asp:ListItem Value="ALL">ALL</asp:ListItem>
                                <asp:ListItem Value="1">MAN POWER BILLING</asp:ListItem>
                                <asp:ListItem Value="2">CONVEYANCE BILLING</asp:ListItem>
                                <asp:ListItem Value="3">DRIVER CONVEYANCE BILLING</asp:ListItem>
                                <asp:ListItem Value="4">MATERIAL BILLING</asp:ListItem>
                                <asp:ListItem Value="5">DEEP CLEANING BILLING</asp:ListItem>
                                <asp:ListItem Value="6">MACHINE RENTAL BILLING</asp:ListItem>
                                <asp:ListItem Value="7">ARREARS MANPOAWER BILLING</asp:ListItem>
                                <asp:ListItem Value="8">MANUAL BILLING</asp:ListItem>
                                <asp:ListItem Value="9">R&M BILLING</asp:ListItem>
                                <asp:ListItem Value="10">ADMINISTRATIVE BILLING</asp:ListItem>
                                <asp:ListItem Value="11">SHIFTWISE BILLING</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                            <asp:Button ID="gst_report" runat="server" class="btn btn-large" OnClick="gst_report_Click" Text="GST Report" OnClientClick="return valid_gst();" />
                        </div>
                        <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                            <asp:Button ID="btn_sac_wise_gst_report" runat="server" class="btn btn-large" OnClick="btn_sac_wise_gst_report_Click" Width="100%" Text="SAC Wise GST Report" OnClientClick="return valid_gst();" />
                        </div>
                    </div>
                </div>
                <div id="menu2">
                    <br />
                    <div class="row">
                        <div class="col-sm-2 col-xs-12">
                            <b>Party Type :</b>
                            <asp:DropDownList ID="ddl_type" runat="server" class="form-control" OnSelectedIndexChanged="ddl_type_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Value="Select">Select</asp:ListItem>
                                <asp:ListItem Value="1">Client</asp:ListItem>
                                <asp:ListItem Value="2">Vendor</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-2 col-xs-12">
                            <b>Select Client/Vendor :</b>
                            <asp:DropDownList ID="ddl_type_client" runat="server" class="form-control">
                                <asp:ListItem Value="Select">Select</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-2 col-xs-12">
                            <b>Payment Date(From):</b>
                            <asp:TextBox ID="txt_payment_date_from" CssClass="form-control date-picker1" runat="server" Style="width: 150px;"></asp:TextBox>
                        </div>
                        <div class="col-sm-2 col-xs-12 ">
                            <b>Payment Date(To) :</b>
                            <asp:TextBox ID="txt_payment_date_to" CssClass="form-control date-picker2" runat="server" Style="width: 150px;"></asp:TextBox>
                        </div>

                                <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                                    <asp:Button ID="btn_report" runat="server" class="btn btn-large" OnClientClick="return monthly_paid_report_val();" OnClick="btn_report_Click" Text="Report" />
                                </div>

                            </div>
                        </div>
                        <div id="menu3">
                            <br />
                            <div class="row">

                                <div class="col-sm-2 col-xs-12">
                                    <b>Party Type :</b>
                                    <asp:DropDownList ID="ddl_type_tally" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_type_tally_SelectedIndexChanged">
                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                        <asp:ListItem Value="1">Client</asp:ListItem>
                                        <asp:ListItem Value="2">Vendor</asp:ListItem>
                                        <asp:ListItem Value="3">R&M</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Select Client/Vendor :</b>
                                    <asp:DropDownList ID="ddl_select_type" runat="server" class="form-control" AutoPostBack="true">
                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>From Booking Date:</b>
                                    <asp:TextBox ID="txt_tally_from_date" CssClass="form-control date-picker1" runat="server" Style="width: 150px;"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>To Booking Date:</b>
                                    <asp:TextBox ID="txt_tally_to_date" CssClass="form-control date-picker2" runat="server" Style="width: 140px;"></asp:TextBox>
                                </div>
                                <br />
                                <div class="col-sm-2 col-xs-12">
                                    <asp:Button ID="btn_get_report" runat="server" class="btn btn-large" Text="Get Report" OnClick="btn_get_report_Click" OnClientClick="return tally_report_val();" />
                                </div>

                            </div>
                        </div>
                        <div id="menu4">
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                    <b>Report Type :</b>
                                    <asp:DropDownList ID="ddl_payment_report_type" runat="server" class="form-control">
                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                        <asp:ListItem Value="1">Invoicewise payment</asp:ListItem>
                                        <asp:ListItem Value="2">Employeewise payment</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Select Payment Type</b>
                                    <asp:DropDownList ID="ddl_type_payment" runat="server" class="form-control">
                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                        <asp:ListItem Value="1">MAN POWER PAYMENT</asp:ListItem>
                                        <asp:ListItem Value="2">CONVEYANCE PAYMENT</asp:ListItem>
                                        <asp:ListItem Value="3">DRIVER CONVEYANCE PAYMENT</asp:ListItem>
                                        <asp:ListItem Value="4">MATERIAL PAYMENT</asp:ListItem>
                                        <asp:ListItem Value="5">ARREARS MANPOAWER PAYMENT</asp:ListItem>
                                        <asp:ListItem Value="6">R&M PAYMENT</asp:ListItem>
                                        <asp:ListItem Value="7">ADMINISTRATIVE PAYMENT</asp:ListItem>
                                        <asp:ListItem Value="8">SHIFTWISE PAYMENT</asp:ListItem>
                                        <asp:ListItem Value="9">VENDOR PAYMENT</asp:ListItem>
                                        <asp:ListItem Value="10">OT PAYMENT</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Select Client :</b>
                                    <asp:DropDownList ID="ddl_payment_client_vendor_name" runat="server" class="form-control">
                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                    from Month :
                            <asp:TextBox ID="gst_from_month" CssClass="form-control date-picker12" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    To Month :
                            <asp:TextBox ID="gst_to_month" CssClass="form-control date-picker12" runat="server"></asp:TextBox>
                                </div>

                                <br />
                                <div class="col-sm-2 col-xs-12">
                                    <asp:Button ID="btn_get_payment" runat="server" class="btn btn-large" Text="Get Report" OnClick="btn_get_payment_Click" OnClientClick="return summary_payment_val();" />
                                </div>

                            </div>
                            <br />
                        </div>
                        <div id="menu5">
                            <br />
                            <div class="row">


                                <div class="col-sm-2 col-xs-12">
                                    from Month :
                            <asp:TextBox ID="mis_from_month" CssClass="form-control date-picker12" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    To Month :
                            <asp:TextBox ID="mis_to_month" CssClass="form-control date-picker12" runat="server"></asp:TextBox>
                                </div>

                                <br />
                                <div class="col-sm-2 col-xs-12">
                                    <asp:Button ID="btn_get_mis" runat="server" class="btn btn-large" Text="Get Report" OnClick="btn_get_mis_Click"/>
                                </div>

                            </div>
                            <br />
                        </div>
                    
                        <div id="menu6">
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                    from Month :
                            <asp:TextBox ID="txt_fromdate" CssClass="form-control date-picker12" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    To Month :
                            <asp:TextBox ID="txt_todate" CssClass="form-control date-picker12" runat="server"></asp:TextBox>
                                </div>
                                 <div class="col-sm-2 col-xs-12">
                                    <b>Select Bill Type</b>
                                    <asp:DropDownList ID="ddl_billtypes" runat="server" class="form-control">
                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                        <asp:ListItem Value="ALL">ALL</asp:ListItem>
                                        <asp:ListItem Value="1">MAN POWER BILLING</asp:ListItem>
                                        <asp:ListItem Value="2">CONVEYANCE BILLING</asp:ListItem>
                                        <asp:ListItem Value="3">DRIVER CONVEYANCE BILLING</asp:ListItem>
                                        <asp:ListItem Value="4">MATERIAL BILLING</asp:ListItem>
                                        <asp:ListItem Value="5">DEEP CLEANING BILLING</asp:ListItem>
                                        <asp:ListItem Value="6">MACHINE RENTAL BILLING</asp:ListItem>
                                        <asp:ListItem Value="7">ARREARS MANPOAWsER BILLING</asp:ListItem>
                                        <asp:ListItem Value="8">MANUAL BILLING</asp:ListItem>
                                        <asp:ListItem Value="9">R&M BILLING</asp:ListItem>
                                        <asp:ListItem Value="10">ADMINISTRATIVE BILLING</asp:ListItem>
                                        <asp:ListItem Value="11">SHIFTWISE BILLING</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <br />
                                <div class="col-sm-2 col-xs-12">
                                    <asp:Button ID="btn_rejectbill" runat="server" class="btn btn-large" Text="Report" OnClick="btn_rejectbill_Click"  />
                                </div>

                            </div>
                            <br />
                           
                        </div>

                         <div id="menu7">
                            <br />
                          
                            <div class="row">
                              
                                 <div class="col-sm-2 col-xs-12">
                                    <b>Select Bill Type</b>
                                    <asp:DropDownList ID="ddl_billtype_financecopy" runat="server" class="form-control" OnSelectedIndexChanged="ddl_billtype_financecopy_SelectedIndexChanged"  AutoPostBack="true"></asp:DropDownList>
                                </div>

                                 <div class="col-sm-2 col-xs-13 " id="conveyance_type" runat="server" visible="false">
                                Conveyance Billing : <span class="text-red" style="color: red">*</span>
                                <asp:DropDownList ID="ddl_conveyance_type" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="1">Employee Conveyance Billing</asp:ListItem>
                                    <asp:ListItem Value="2">Driver Convenyance Billing</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                                <%--<div class="col-md-2 col-xs-12">
                                    <b>Invoice type :</b><span class="text-red">*</span>
                                    <asp:DropDownList ID="ddl_invoice_type" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_invoice_type_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Value="1">CLUB</asp:ListItem>
                                        <asp:ListItem Value="2">UNCLUB</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                 <div class=" col-md-2 col-xs-12">
                                    <asp:Panel ID="desigpanel" runat="server" Visible="false">
                                        <b>Designation :</b><span class="text-red">*</span>
                                        <asp:DropDownList ID="ddl_designation" runat="server" CssClass="form-control" />
                                    </asp:Panel>
                                </div>
                                <div class="col-md-2 col-xs-12  billingProcess" style="display: none">
                                    <b>Billing Process :</b>
                                    <asp:DropDownList ID="ddl_billing_process" runat="server" CssClass="form-control" >
                                        <asp:ListItem Value="Regular">Regular</asp:ListItem>
                                        <asp:ListItem Value="Metro">Metro</asp:ListItem>
                                        <asp:ListItem Value="Non Metro">Non Metro</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                 <div class="col-sm-2 col-xs-12 region" style="display: none">
                                    <b>Region :</b>
                            <asp:DropDownList ID="ddlregion" runat="server" CssClass="form-control" >
                            </asp:DropDownList>  
                                </div>
                                  <div class="col-sm-2 col-xs-12" id="abc111">
                                    <b>Billing Start Day:</b> 
                                        <asp:DropDownList ID="ddl_start_date_common" runat="server" CssClass="form-control text_box" Width="100%">
                                            <asp:ListItem Value="0">Select</asp:ListItem>
                                            <asp:ListItem Value="1">1</asp:ListItem>
                                            <asp:ListItem Value="2">2</asp:ListItem>
                                            <asp:ListItem Value="3">3</asp:ListItem>
                                            <asp:ListItem Value="4">4</asp:ListItem>
                                            <asp:ListItem Value="5">5</asp:ListItem>
                                            <asp:ListItem Value="6">6</asp:ListItem>
                                            <asp:ListItem Value="7">7</asp:ListItem>
                                            <asp:ListItem Value="8">8</asp:ListItem>
                                            <asp:ListItem Value="9">9</asp:ListItem>
                                            <asp:ListItem Value="10">10</asp:ListItem>
                                            <asp:ListItem Value="11">11</asp:ListItem>
                                            <asp:ListItem Value="12">12</asp:ListItem>
                                            <asp:ListItem Value="13">13</asp:ListItem>
                                            <asp:ListItem Value="14">14</asp:ListItem>
                                            <asp:ListItem Value="15">15</asp:ListItem>
                                            <asp:ListItem Value="16">16</asp:ListItem>
                                            <asp:ListItem Value="17">17</asp:ListItem>
                                            <asp:ListItem Value="18">18</asp:ListItem>
                                            <asp:ListItem Value="19">19</asp:ListItem>
                                            <asp:ListItem Value="20">20</asp:ListItem>
                                            <asp:ListItem Value="21">21</asp:ListItem>
                                            <asp:ListItem Value="22">22</asp:ListItem>
                                            <asp:ListItem Value="23">23</asp:ListItem>
                                            <asp:ListItem Value="24">24</asp:ListItem>
                                            <asp:ListItem Value="25">25</asp:ListItem>
                                            <asp:ListItem Value="26">26</asp:ListItem>
                                            <asp:ListItem Value="27">27</asp:ListItem>
                                            <asp:ListItem Value="28">28</asp:ListItem>
                                            <asp:ListItem Value="29">29</asp:ListItem>
                                            <asp:ListItem Value="30">30</asp:ListItem>
                                            <asp:ListItem Value="31">31</asp:ListItem>
                                        </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12" id="abc11">
                                    <b>Billing End Day:</b> 
                                        <asp:DropDownList ID="ddl_end_date_common" runat="server" CssClass="form-control text_box" Width="100%">
                                            <asp:ListItem Value="0">Select</asp:ListItem>
                                            <asp:ListItem Value="1">1</asp:ListItem>
                                            <asp:ListItem Value="2">2</asp:ListItem>
                                            <asp:ListItem Value="3">3</asp:ListItem>
                                            <asp:ListItem Value="4">4</asp:ListItem>
                                            <asp:ListItem Value="5">5</asp:ListItem>
                                            <asp:ListItem Value="6">6</asp:ListItem>
                                            <asp:ListItem Value="7">7</asp:ListItem>
                                            <asp:ListItem Value="8">8</asp:ListItem>
                                            <asp:ListItem Value="9">9</asp:ListItem>
                                            <asp:ListItem Value="10">10</asp:ListItem>
                                            <asp:ListItem Value="11">11</asp:ListItem>
                                            <asp:ListItem Value="12">12</asp:ListItem>
                                            <asp:ListItem Value="13">13</asp:ListItem>
                                            <asp:ListItem Value="14">14</asp:ListItem>
                                            <asp:ListItem Value="15">15</asp:ListItem>
                                            <asp:ListItem Value="16">16</asp:ListItem>
                                            <asp:ListItem Value="17">17</asp:ListItem>
                                            <asp:ListItem Value="18">18</asp:ListItem>
                                            <asp:ListItem Value="19">19</asp:ListItem>
                                            <asp:ListItem Value="20">20</asp:ListItem>
                                            <asp:ListItem Value="21">21</asp:ListItem>
                                            <asp:ListItem Value="22">22</asp:ListItem>
                                            <asp:ListItem Value="23">23</asp:ListItem>
                                            <asp:ListItem Value="24">24</asp:ListItem>
                                            <asp:ListItem Value="25">25</asp:ListItem>
                                            <asp:ListItem Value="26">26</asp:ListItem>
                                            <asp:ListItem Value="27">27</asp:ListItem>
                                            <asp:ListItem Value="28">28</asp:ListItem>
                                            <asp:ListItem Value="29">29</asp:ListItem>
                                            <asp:ListItem Value="30">30</asp:ListItem>
                                            <asp:ListItem Value="31">31</asp:ListItem>
                                        </asp:DropDownList>
                                </div>--%>
                              
                                <br />
                                <div class="col-sm-2 col-xs-12">
                                    <asp:Button ID="btn_financecopy" runat="server" class="btn btn-large" Text="Report" OnClick="btn_financecopy_Click"  />
                                </div>

                    </div>

                    <br />

                </div>
                <div id="menu8">
                    <br />
                    <div class="col-sm-2 col-xs-12">
                        <asp:Button ID="btn_softcopy" runat="server" class="btn btn-large" Text="Report" OnClick="btn_softcopy_Click" />
                    </div>
                </div>
                <br />
                <div id="menu9">
                    <div class="row">
                        <div class="col-sm-2 col-xs-12">
                            <asp:Button ID="btn_attendancere" runat="server" class="btn btn-large" Text="Attendance Report" OnClick="btn_attendancere_Click" Style="width: auto" OnClientClick="return validate();" />
                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row">
                        <asp:Panel ID="Panel20" runat="server" Style="overflow-x: auto;">
                            <asp:GridView ID="gv_attendance" class="table" runat="server" BackColor="White"
                                BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="1" DataKeyNames="CLIENT_CODE"
                                AutoGenerateColumns="False" Width="100%" OnRowDataBound="gv_attendance_RowDataBound" OnPreRender="gv_attendance_PreRender">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <EditRowStyle BackColor="#999999" />
                                <FooterStyle BackColor="White" ForeColor="#000066" />
                                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" CssClass="text-uppercase" />
                                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Center" />
                                <RowStyle ForeColor="#000066" BackColor="#ffffff" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                <SortedDescendingHeaderStyle BackColor="#00547E" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr No.">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="client_name" HeaderText="CLIENT NAME" SortExpression="client_name" />
                                    <asp:BoundField DataField="MONTH" HeaderText="Month" SortExpression="MONTH" />
                                    <asp:BoundField DataField="YEAR" HeaderText="YEAR" SortExpression="YEAR" />
                                    <asp:BoundField DataField="Current_month_attendance" HeaderText="Current Month Branches" SortExpression="Current_month_attendance" />
                                    <asp:BoundField DataField="pre_month_cnt" HeaderText="Previous Month Branches" SortExpression="pre_month_cnt" />
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btn_view" Text="View" runat="server" CssClass="btn btn-primary" OnClick="btn_view_Click" Style="color: white"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <%-- <div class="row text-center">
                                        <asp:Button ID="btn_view" runat="server" class="btn btn-large" OnClick="btn_view_Click" Text="View" />
                                    </div>--%>
                        </asp:Panel>
                        <br />
                        <br />
                        <br />
                        <div class="row">
                            <asp:Panel ID="Panel1" runat="server" Style="overflow-x: auto;">
                                <asp:GridView ID="gv_state_attendance" class="table" runat="server" BackColor="White"
                                    BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="1" OnPreRender="gv_state_attendance_PreRender"
                                    AutoGenerateColumns="False" Width="100%">
                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    <EditRowStyle BackColor="#999999" />
                                    <FooterStyle BackColor="White" ForeColor="#000066" />
                                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" CssClass="text-uppercase" />
                                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Center" />
                                    <RowStyle ForeColor="#000066" BackColor="#ffffff" />
                                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                    <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                    <SortedDescendingHeaderStyle BackColor="#00547E" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr No.">
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="CLIENT_NAME" HeaderText="CLIENT NAME" SortExpression="CLIENT_NAME" />
                                        <asp:BoundField DataField="STATE_NAME" HeaderText="State Name" SortExpression="STATE_NAME" />
                                        <asp:BoundField DataField="MONTH" HeaderText="Month" SortExpression="MONTH" />
                                        <asp:BoundField DataField="YEAR" HeaderText="YEAR" SortExpression="YEAR" />
                                        <asp:BoundField DataField="Approve_by_admin" HeaderText="Current Month Approve By Admin" SortExpression="Approve_by_admin" />
                                        <asp:BoundField DataField="approve_by_admin_manager" HeaderText="Current Month Approve By Admin Manager" SortExpression="approve_by_admin_manager" />
                                        <asp:BoundField DataField="approve_by_finance" HeaderText="Current Month Approve By Finance" SortExpression="approve_by_finance" />
                                        <asp:BoundField DataField="Pending" HeaderText="Pending" SortExpression="Pending" />
                                        <asp:BoundField DataField="pre_month_cnt" HeaderText="previous Month Attendance Count" SortExpression="pre_month_cnt" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
                <div id="menu10">
                    <div class="row">
                        <div class="col-sm-2 col-xs-12 text-left">
                            <b>Select :</b>
                            <asp:DropDownList ID="ddl_Select" class="form-control pr_state js-example-basic-single" runat="server">
                                <asp:ListItem Value="1">ALL</asp:ListItem>
                                <asp:ListItem Value="2">Month Wise</asp:ListItem>
                                <asp:ListItem Value="3">Year Wise</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-2 col-xs-12 text-left">
                            <b>Select Type :</b>
                            <asp:DropDownList ID="ddl_approved_or_not" class="form-control pr_state js-example-basic-single" runat="server">
                                <asp:ListItem Value="1">ALL</asp:ListItem>
                                <asp:ListItem Value="2">Approved</asp:ListItem>
                                <asp:ListItem Value="3">Not Approved</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-2 col-xs-12 text-left">
                            <b>Select Month :</b>
                            <asp:TextBox ID="txt_report_month_year" CssClass="form-control date-picker12" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-sm-2 col-xs-12">
                            <asp:Button ID="btn_mdApprove" runat="server" class="btn btn-large" Text="Report" OnClick="btn_mdApprove_Click" OnClientClick="return validate1();" />
                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row">
                        <asp:Panel ID="Panel2" runat="server" Style="overflow-x: auto;">
                            <asp:GridView ID="gv_Md_Approve" class="table" runat="server" BackColor="White"
                                BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="1"
                                AutoGenerateColumns="false" Width="100%" OnRowDataBound="gv_Md_Approve_RowDataBound" OnPreRender="gv_Md_Approve_PreRender">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <EditRowStyle BackColor="#999999" />
                                <FooterStyle BackColor="White" ForeColor="#000066" />
                                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" CssClass="text-uppercase" />
                                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Center" />
                                <RowStyle ForeColor="#000066" BackColor="#ffffff" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                <SortedDescendingHeaderStyle BackColor="#00547E" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr No.">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="client_name" HeaderText="CLIENT NAME" SortExpression="client_name" />
                                    <asp:BoundField DataField="month" HeaderText="MONTH" SortExpression="month" />
                                    <asp:BoundField DataField="year" HeaderText="YEAR" SortExpression="YEAR" />
                                    <asp:BoundField DataField="type" HeaderText="BILL TYPE" SortExpression="type" />
                                    <asp:BoundField DataField="Total_Invoice" HeaderText="Total Invoice" SortExpression="Total_Invoice" />
                                    <asp:BoundField DataField="Approve" HeaderText="APPROVED" SortExpression="Approve" />
                                    <asp:BoundField DataField="NOTApprove" HeaderText="NOT APPROVED" SortExpression="NOTApprove" />
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </div>
                </div>
                <div id="menu11">
                    <div class="row">
                        <div class="col-sm-2 col-xs-12">
                            <b>From Month :</b><span class="text-red">*</span>
                            <asp:TextBox ID="txt_out_from_month" CssClass="form-control date-picker12" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-sm-2 col-xs-12">
                            <b>To Month :</b><span class="text-red">*</span>
                            <asp:TextBox ID="txt_out_to_month" CssClass="form-control date-picker12" runat="server"></asp:TextBox>
                        </div>
                        <br />
                        <div class="col-sm-2 col-xs-12">
                            <asp:Button ID="btn_excle_report" runat="server" class="btn btn-large" Text="Report" OnClick="btn_excle_report_Click" OnClientClick="return outstanding_required();" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

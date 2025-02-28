<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="reports_main.aspx.cs" Inherits="Employee_salary_details" Title="Employee salary details" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Employee Salary Details</title>
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
    <script src="datatable/jszip.min.js"></script>
    <script src="datatable/vfs_fonts.js"></script>
    <script src="datatable/buttons.html5.min.js"></script>
    <script src="datatable/buttons.print.min.js"></script>
    <script src="datatable/buttons.colVis.min.js"></script>

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

            function unblock() {
                $.unblockUI();
            }

            $(document).ready(function () {
                $(document).on("Keyup", function () {
                    SearchGrid('<%=gv_check_einv.ClientID%>');
            });

            $('[id*=chk_header_material]').click(function () {
                $("[id*='chk_record_material']").attr('checked', this.checked);
            });


        });

        }

        function bill_check() {
            var ddl_report = document.getElementById('<%=ddl_report.ClientID %>');
            var Selected_ddl_report = ddl_report.options[ddl_report.selectedIndex].text;
            if (Selected_ddl_report == "Monthwise Billing Details") {
                  $(".bill").show();
              }

              else { $(".bill").hide(); }
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
        function valid_date() {
            var txt_month_year = document.getElementById('<%=txt_month_year.ClientID %>');
            if (txt_month_year.value == "") {
                alert("Please Select Date");
                txt_month_year.focus();
                return false;
            }
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

        function openWindow() {
            window.open("html/reports_main.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
        }
        function accouting_val() {
            var acc_from_date = document.getElementById('<%=acc_from_date.ClientID %>');
            var acc_to_date = document.getElementById('<%=acc_to_date.ClientID %>');
            var ddl_client = document.getElementById('<%=ddl_client.ClientID %>');
            var Selected_ddl_client = ddl_client.options[ddl_client.selectedIndex].text;

            var dll_type = document.getElementById('<%=dll_type.ClientID %>');
            var Selected_dll_type = dll_type.options[dll_type.selectedIndex].text;

        if (Selected_ddl_client == "ALL") {
            alert("Please Select Client Name");
            ddl_client.focus();
            return false;
        }
        if (acc_from_date.value == "") {
            alert("Please Select From Date");
            acc_from_date.focus();
            return false;
        }
        if (acc_to_date.value == "") {
            alert("Please Select To Date");
            acc_to_date.focus();
            return false;
        }
        if (Selected_dll_type == "Select") {
            alert("Please Select Type");
            dll_type.focus();
            return false;
        }
    }
    function report_validation() {
        var ddl_report = document.getElementById('<%=ddl_report.ClientID %>');
            var Selected_ddl_report = ddl_report.options[ddl_report.selectedIndex].text;

            var ddl_client = document.getElementById('<%=ddl_client.ClientID %>');
            var Selected_ddl_client = ddl_client.options[ddl_client.selectedIndex].text;

            var ddl_state = document.getElementById('<%=ddl_state.ClientID %>');
            var Selected_ddl_state = ddl_state.options[ddl_state.selectedIndex].text;

            var txt_date = document.getElementById('<%=txt_date.ClientID %>');

            var ddl_bill_type = document.getElementById('<%=ddl_bill_type.ClientID %>');
            var Selected_ddl_bill_type = ddl_bill_type.options[ddl_bill_type.selectedIndex].text;

            if ((Selected_ddl_report == "PF XL") || (Selected_ddl_report == "LWF XL") || (Selected_ddl_report == "PT XL") || (Selected_ddl_report == "GST XL") || (Selected_ddl_report == "Salary Slip Sending Details") || (Selected_ddl_report == "Monthwise Billing Details")) {
                if ((Selected_ddl_client == "ALL") && (Selected_ddl_bill_type != "Clientwise Bill")) {
                    alert("Please Select Client Name");
                    ddl_client.focus();
                    return false;
                }
                if (txt_date.value == "") {
                    alert("Please Select Month");
                    txt_date.focus();
                    return false;
                }
                if (Selected_ddl_report == "Monthwise Billing Details") {
                    if (Selected_ddl_bill_type == "Select") {
                    alert("Please Select Billing Type");
                    ddl_bill_type.focus();
                    return false;
                }

                }
                if (Selected_ddl_bill_type == "Clientwise Bill") {
                    if (Selected_ddl_client != "ALL") {
                        alert("Please Select Client Name  ALL");
                        ddl_client.focus();
                        return false;
                    }
                }
                    
            }
            if (Selected_ddl_report == "ESIC XL") {
                if (Selected_ddl_client == "ALL") {
                    alert("Please Select Client Name");
                    ddl_client.focus();
                    return false;
                }
                if (Selected_ddl_state == "ALL") {
                    alert("Please Select State Name");
                    ddl_client.focus();
                    return false;
                }
                if (txt_date.value == "") {
                    alert("Please Select Month");
                    txt_date.focus();
                    return false;
                }
            }
            if ((Selected_ddl_report == "Branch Head Contact Details") || (Selected_ddl_report == "Joining Letter Sending Details")) {
                if (Selected_ddl_client == "ALL") {
                    alert("Please Select Client Name");
                    ddl_client.focus();
                    return false;
                }
            }
           
           
        }
        $(document).ready(function () {
            var st = $(this).find("input[id*='hidtab']").val();
            if (st == null)
                st = 0;
            $('[id$=tabs]').tabs({ selected: st });
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

        function confirmation()
        {

            var select_record = false;
            {

                var gv_bank = document.getElementById('<%= gv_check_einv.ClientID %>');
                    for (var i = 1; i < gv_bank.rows.length; i++) {
                        var input = gv_bank.rows[i].getElementsByTagName('input');
                        if (input != null) {
                            if (input[0].type == "checkbox") {
                                if (input[0].checked) {
                                    gv_bank = true;
                                    return true;
                                }
                            }
                        }
                    }
                    alert("Please select atleast one Record ");
                    return false;

                }
                ($.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } }))
                return true;

                if (confirm('Are you want to process all this invoices for E-INVOICING ?'))
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
        function all_inv_date_validation() {

            var t_client = document.getElementById('<%=ddl_client.ClientID %>');
            var Selectedclient = t_client.options[t_client.selectedIndex].text;

            var all_inv_from_date = document.getElementById('<%=gst_from_date.ClientID %>');
            var all_inv_to_date = document.getElementById('<%=gst_to_date.ClientID %>');


            var ddl_gst_type = document.getElementById('<%=ddl_gst_type.ClientID %>');
            var Selected_ddl_gst_type = ddl_gst_type.options[ddl_gst_type.selectedIndex].text;

            var ddl_state = document.getElementById('<%=ddl_state.ClientID %>');
            var Selected_ddl_state = ddl_state.options[ddl_state.selectedIndex].text;

            var ddl_unitcode = document.getElementById('<%=ddl_unitcode.ClientID %>');
            var Selected_ddl_unitcode = ddl_unitcode.options[ddl_unitcode.selectedIndex].text;

            if (Selectedclient == "Select" || Selectedclient == "ALL") {
                alert("Please Select Client.");
                t_client.focus();
                return false;
            }

            if (all_inv_from_date.value == "") {
                alert("Please Select From Date");
                all_inv_from_date.focus();
                return false;
            }

            if (all_inv_to_date.value == "") {
                alert("Please Select To Date");
                all_inv_to_date.focus();
                return false;
            }

            if (Selected_ddl_gst_type == "Select") {
                alert("Please Select Bill Type ");
                ddl_gst_type.focus();
                return false;
            }

            if (Selected_ddl_state != "ALL") {
                alert("Please Select ALL State Name");
                ddl_client.focus();
                return false;
            }

            if (Selected_ddl_unitcode != "ALL") {
                alert("Please Select ALL Branch Name");
                ddl_client.focus();
                return false;
            }

            //document.getElementById("loader").style.display = 'block';
        }
    </script>
     <script type="text/javascript">

         $(document).ready(function () {
             $.fn.dataTable.ext.errMode = 'none';
             var table = $('#<%=gv_check_einv.ClientID%>').DataTable({
                "responsive": true,
                "sPaginationType": "full_numbers",
                scrollY: "210px",
                scrollX: true,
                scrollCollapse: true,
                paging: true,
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
                ],

            });

            table.buttons().container()
               .appendTo('#<%=gv_check_einv.ClientID%>_wrapper .col-sm-6:eq(0)');

        });

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
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>EMPLOYEE SALARY</b></div>
                    </div>
                    <div class="col-sm-2 text-right">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <br />
           <%-- <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-4"></div>
                    <div class="col-sm-9">
                        <div style="text-align: left; color:white; font-size: small;"><b>Employee Salary Details</b></div>
                    </div>
                    <div class="col-sm-2 text-left">
                        
                    </div>
                </div>
            </div>--%>
            <div class="panel-body">
                <div class="container-fluid" style="background: #f3f1fe; border: 1px solid #e2e2dd; border-radius: 10px; padding:25px 25px 25px 25px; margin-bottom:20px; margin-top:20px">
                 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="row">

                    <div class="col-sm-2 col-xs-12 text-left">
                       <b> Client Name :   </b>
                <asp:DropDownList ID="ddl_client" class="form-control pr_state js-example-basic-single" runat="server" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
                    </div>
                    <div class="col-sm-2 col-xs-12 text-left">
                        <b> State Name :</b>   
                 <asp:DropDownList ID="ddl_state" runat="server" DataTextField="STATE" DataValueField="STATE" class="form-control text_box" OnSelectedIndexChanged="ddl_state_SelectedIndexChanged" AutoPostBack="true">
                 </asp:DropDownList>
                    </div>
                    <div class="col-sm-2 col-xs-12 text-left">
                         <b>Branch Name : </b>  
                <asp:DropDownList ID="ddl_unitcode" class="form-control pr_state js-example-basic-single" runat="server" OnSelectedIndexChanged="ddlunitselect_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
                    </div>
                    <div class="col-sm-2 col-xs-12 text-left">
                        <b> Employee Type :</b>
                                    <asp:DropDownList ID="ddl_employee_type" runat="server" class="form-control" OnSelectedIndexChanged="ddlunitselect_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Value="ALL">ALL</asp:ListItem>
                                        <asp:ListItem Value="Permanent">Permanent</asp:ListItem>
                                        <%--<asp:ListItem Value="PermanentReliever">Permanent Reliever</asp:ListItem>--%>
                                        <asp:ListItem Value="Reliever">Reliever</asp:ListItem>
                                        <asp:ListItem Value="Staff">Staff</asp:ListItem>
                                        <asp:ListItem Value="Temporary">Temporary</asp:ListItem>
                                        <%--<asp:ListItem Value="RM">Repair & Maintenance</asp:ListItem>--%>
                                        <asp:ListItem Value="Left">Left</asp:ListItem>
                                    </asp:DropDownList>
                    </div>
                    <div class="col-sm-2 col-xs-12 text-left">
                        <b> Employee Name :   </b> 
                <asp:DropDownList ID="ddl_employee" class="form-control pr_state js-example-basic-single" runat="server" />
                    </div>
                      <div class="col-lg-2 col-md-2 col-sm-3 col-xs-12">
                             <b>Select Month :</b>
                            <asp:TextBox ID="txt_date" CssClass="form-control date-picker12" runat="server" style="width: 105px;"></asp:TextBox>
                                </div>
                </div>
                </div>
                   </ContentTemplate>
                     </asp:UpdatePanel>
                <br />
                    <br />
                    <div id="tabs" style="background: #f3f1fe; padding: 20px 20px 20px 20px; border-radius: 10px">
                        <asp:HiddenField ID="hf_lwf" runat="server" />

                        <asp:HiddenField ID="hidtab" Value="0" runat="server" />
                        <ul>

                            <li><a id="A1" href="#menu1" runat="server"><b>Employee</b></a></li>
                            <li><a id="A3" href="#menu2" runat="server"><b>Allowance</b></a></li>
                            <li><a id="A4" href="#menu3" runat="server"><b>Payment Hold/Unhold</b></a></li>
                            <li><a id="A5" href="#menu4" runat="server"><b>Loan</b></a></li>
                            <li><a href="#menu5"><b>Reports</b></a></li>
                            <li><a href="#menu6"><b>GST Reports</b></a></li>
                            <li><a href="#menu7"><b>Accounting</b></a></li>
                            <li><a href="#menu8"><b>Monthly Paid Amount</b></a></li>
                            <li><a href="#menu9"><b>Tally Report</b></a></li>
                            <li><a href="#menu10"><b>Purchase GST Report</b></a></li>
                        </ul>
                        <div id="menu1">
                            <br />
                            <br />
                            <asp:Panel runat="server" CssClass="panel panel-primary">
                                <div class="row text-center">
                                    <h4><u>PF/ESIC/UAN</u></h4>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-sm-1 col-xs-12"></div>
                                    <div class="col-sm-2 col-xs-12">
                                        <asp:DropDownList ID="ddl_emp_type" runat="server" class="form-control">
                                            <asp:ListItem Value="pf_number">PF</asp:ListItem>
                                            <asp:ListItem Value="esic_number">ESIC</asp:ListItem>
                                            <asp:ListItem Value="pan_number">UAN</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <asp:DropDownList ID="ddl_emp_diff" runat="server" class="form-control">
                                            <asp:ListItem Value="0">ALL</asp:ListItem>
                                            <asp:ListItem Value="1">Duplicate</asp:ListItem>
                                            <asp:ListItem Value="2">Blank</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <asp:Button ID="btn_emp_report" runat="server" class="btn btn-large" OnClick="btn_emp_report_Click" Text="Get Report" />
                                    </div>
                                </div>
                                <br />
                            </asp:Panel>
                            <br />
                            <asp:Panel ID="Panel1" runat="server" CssClass="panel panel-primary">
                                <div class="row text-center">
                                    <h4><u>Employee Report</u></h4>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-sm-1 col-xs-12"></div>
                                    <div class="col-sm-1 col-xs-12">
                                        <asp:TextBox ID="txt_month_year" runat="server" CssClass="form-control date-picker12"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-6 col-xs-12">
                                        <asp:Button ID="btn_employee_report" runat="server" class="btn btn-large" Width="30%" OnClick="btn_employee_report_Click" Text="Get Employee Report" OnClientClick="return valid_date();" />
                                    </div>
                                </div>
                                <br />
                            </asp:Panel>
                            <br />
                        </div>
                        <div id="menu2">
                            <br />
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                <ContentTemplate>
                                    <div class="row">
                                        <br />
                                        <div class="col-sm-2 col-xs-12">
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="menu3">
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12 text-left">
                                </div>
                            </div>
                        </div>
                        <div id="menu4">
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                </div>
                            </div>
                        </div>
                        <div id="menu5">
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                    <b>Report type: <span class="text-red">*</span></b>
                                    <asp:DropDownList ID="ddl_report" runat="server" onchange="return bill_check();" class="form-control">
                                        <asp:ListItem Value="PF XL">PF XL</asp:ListItem>
                                        <asp:ListItem Value="LWF XL">LWF XL</asp:ListItem>
                                        <asp:ListItem Value="PT XL">PT XL</asp:ListItem>
                                        <asp:ListItem Value="ESIC XL">ESIC XL</asp:ListItem>
                                        <asp:ListItem Value="GST XL">GST XL</asp:ListItem>
                                          <asp:ListItem Value="Branch Head Contact Details">Branch Head Contact Details</asp:ListItem>
                                          <asp:ListItem Value="Salary Slip Sending Details">Salary Slip Sending Details</asp:ListItem>
                                          <asp:ListItem Value="Joining Letter Sending Details">Joining Letter Sending Details</asp:ListItem>
                                          <asp:ListItem Value="Monthwise Billing Details">Monthwise Billing Details</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12 bill" style="display: none">
                                    Billing type: <span class="text-red" style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_bill_type" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="1">Clientwise Bill</asp:ListItem>
                                        <asp:ListItem Value="2">Statewise Bill</asp:ListItem>
                                        <asp:ListItem Value="3">Branchwise Bill</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                 <div class="col-sm-2 col-xs-12" >
                                    <asp:Button ID="btn_getxl_report" style="margin-top: 18px;" runat="server" class="btn btn-large" OnClick="btn_getxl_report_Click" Text="Get Report" OnClientClick="return report_validation()" />
                                 
                                </div>
                             </div>
                        </div>
                   <div id="menu6">
<div class="row">
    <div class="col-sm-1 col-xs-12">
                         <b>  From Date :</b>
                            <asp:TextBox ID="gst_from_date" CssClass="form-control date-picker1" runat="server" style="width: 105px;"></asp:TextBox>
                                </div>
    <div class="col-sm-1 col-xs-12">
                           <b>To Date :</b>
                            <asp:TextBox ID="gst_to_date" CssClass="form-control date-picker2" runat="server" style="width: 105px;"></asp:TextBox>
                                </div>
    <div class="col-sm-2 col-xs-12">
       <b> Select Bill Type</b>
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
                                          <asp:ListItem Value="12">INCENTIVE BILLING</asp:ListItem>
                                        <asp:ListItem Value="13">OFFICE RENT BILLING</asp:ListItem>
                                        <asp:ListItem Value="14">MANPOWER OT BILLING</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                                    <asp:Button ID="gst_report" runat="server" class="btn btn-large" OnClick="gst_report_Click" Text="GST Report" Width="100%" OnClientClick="return valid_gst();" />
                                </div>
                                <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                                    <asp:Button ID="btn_sac_wise_gst_report" runat="server" class="btn btn-large" OnClick="btn_sac_wise_gst_report_Click" Width="100%" Text="SAC Wise GST Report" OnClientClick="return valid_gst();" />
                                </div>
                                <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                                    <asp:Button ID="btn_r_m_tally_report" runat="server" class="btn btn-large" OnClick="btn_r_m_tally_report_Click" Width="100%" Text="R&M Tally Report" />
                                </div>
                                <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                                    <asp:Button ID="btn_all_inv_download" runat="server" class="btn btn-large" OnClick="btn_all_inv_download_Click" Width="100%" Text="Invoice Download" />
                                </div>
                                <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                                    <asp:Button ID="btn_einvoice" runat="server" class="btn btn-large" OnClick="btn_e_invoice_Click" Width="100%" Text="E-Invoicing" Visible="false" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                                    <asp:Button ID="btn_view_einv" runat="server" class="btn btn-large" OnClick="btn_view_einv_Click" Width="100%" Text="Check For EInvoice" />
                                </div>
                            </div>
                            <%--  <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                                    <asp:Button ID="btn_einv_process" runat="server" class="btn btn-large" OnClick="btn_einv_process_Click" Width="100%" Text="Process EInvoice" />
                               </div>--%>
                            <br /><br />
                            <div class="row">
                                <asp:Panel ID="panel7" runat="server" CssClass="grid-view">
                                    <div class="container-fluid">
                                        <asp:GridView ID="gv_check_einv" class="table" runat="server" Font-Size="X-Small" BackColor="White"
                                            BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" OnPreRender="gv_check_einv_PreRender">
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
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chk_header_material" runat="server" Text="SELECT RECORD" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chk_record_material" runat="server" CssClass="center-block" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sr No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_srnumber" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                                            <asp:Button ID="btn_einv_process" runat="server" class="btn btn-large" OnClientClick="return confirmation();" OnClick="btn_einv_process_Click" Width="100%" Text="Process EInvoice" Visible="false" />
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>



                        <div id="menu7">
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                    <b>From Date :</b>
                                    <asp:TextBox ID="acc_from_date" CssClass="form-control date-picker1" runat="server" Style="width: 105px;"></asp:TextBox>
                                </div>
    <div class="col-sm-2 col-xs-12">
                          <b> To Date :</b>
                            <asp:TextBox ID="acc_to_date" CssClass="form-control date-picker2" runat="server" style="width: 105px;"></asp:TextBox>
                                </div>


                                <div class="col-sm-2 col-xs-12">
                                    <b>Select Type :</b>
                                    <asp:DropDownList ID="dll_type" runat="server" class="form-control">
                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                        <asp:ListItem Value="1">Manpower</asp:ListItem>
                                        <asp:ListItem Value="2">Conveyance</asp:ListItem>
                                        <asp:ListItem Value="3">Driver Conveyance</asp:ListItem>
                                        <asp:ListItem Value="4"> Material</asp:ListItem>
                                        <asp:ListItem Value="5">Deep Clean</asp:ListItem>

                                     </asp:DropDownList>
                            </div>
                            <div class="col-sm-2 col-xs-12" style="margin-top: 1%">
                                <asp:Button ID="ddl_get_report" runat="server" class="btn btn-large" OnClick="ddl_get_report_Click" OnClientClick ="return accouting_val();" Text="Report" />
                            </div>

                        </div>
                    </div>
                    <div id="menu8">
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
                               <b> Select Type :</b>
                                     <asp:DropDownList ID="ddl_type_client" runat="server" class="form-control">
                                         <asp:ListItem Value="Select">Select</asp:ListItem>
                                     </asp:DropDownList>
                            </div>
                            <div class="col-sm-2 col-xs-12">
                           <b>Payment Date(From):</b>
                            <asp:TextBox ID="txt_payment_date_from" CssClass="form-control date-picker1" runat="server" style="width: 105px;"></asp:TextBox>
                                </div>
    <div class="col-sm-2 col-xs-12">
                           <b>Payment Date(To) :</b>
                            <asp:TextBox ID="txt_payment_date_to" CssClass="form-control date-picker2" runat="server" style="width: 105px;"></asp:TextBox>
                                </div>


                           
                            <div class="col-sm-2 col-xs-12" >
                                <asp:Button ID="btn_report" runat="server" class="btn btn-large" OnClientClick ="return monthly_paid_report_val();" OnClick="btn_report_Click" Text="Report" />
                            </div>

                        </div>
                    </div>
                   <div id="menu9">
                        <div class="row">

                             <div class="col-sm-2 col-xs-12">
                                  <b>Party Type :</b>
                                     <asp:DropDownList ID="ddl_type_tally" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_type_tally_SelectedIndexChanged">
                                         <asp:ListItem Value="Select">Select</asp:ListItem>
                                          <asp:ListItem Value="1">Client</asp:ListItem>
                                           <asp:ListItem Value="2">Vendor</asp:ListItem>
                                     </asp:DropDownList>
                            </div>
                             <div class="col-sm-2 col-xs-12">
                               <b> Select Type :</b>
                                     <asp:DropDownList ID="ddl_select_type" runat="server" class="form-control" AutoPostBack="true">
                                         <asp:ListItem Value="Select">Select</asp:ListItem>
                                     </asp:DropDownList>
                            </div>
                            <div class="col-sm-2 col-xs-12">
                           <b>From Booking Date:</b>
                            <asp:TextBox ID="txt_tally_from_date" CssClass="form-control date-picker1" runat="server" style="width: 105px;"></asp:TextBox>
                                </div>
    <div class="col-sm-2 col-xs-12">
                           <b>To Booking Date:</b>
                            <asp:TextBox ID="txt_tally_to_date" CssClass="form-control date-picker2" runat="server" style="width: 105px;"></asp:TextBox>
                                </div>
                                <br />
                                <div class="col-sm-2 col-xs-12">
                                    <asp:Button ID="btn_get_report" runat="server" class="btn btn-large" Text="Get Report" OnClick="btn_get_report_Click" OnClientClick="return tally_report_val();" />
                                </div>
                            </div>
                        </div>

                        <%-- Start Purchase GST Report --%>
                        <div id="menu10">
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                    <b>Vendor Type :</b>
                                    <asp:DropDownList ID="ddl_ven_type" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_ven_type_SelectedIndexChanged">
                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                        <asp:ListItem Value="1">R & M</asp:ListItem>
                                        <asp:ListItem Value="2">Vendor</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Vendor Name :</b>
                                    <asp:DropDownList ID="ddl_ven_name" runat="server" class="form-control" >
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>From Date:</b>
                                    <asp:TextBox ID="txt_ven_from_date" CssClass="form-control date-picker1" runat="server" Style="width: auto;"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>To Date:</b>
                                    <asp:TextBox ID="txt_ven_to_date" CssClass="form-control date-picker2" runat="server" Style="width: auto;"></asp:TextBox>
                                </div>
                                <br />
                                <div class="col-sm-2 col-xs-12">
                                    <asp:Button ID="btn_ven_report" runat="server" class="btn btn-large" Text="Report" OnClick="btn_ven_report_Click" />
                                </div>
                            </div>
                        </div>
                        <%-- END Purchase GST Report --%>

                    </div>
                </div>

            </div>
        </asp:Panel>
    </div>


</asp:Content>

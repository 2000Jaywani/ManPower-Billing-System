<%@ Page Title="Item Master" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Return_material.aspx.cs" Inherits="Return_material" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Return Material</title>
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


    <script type="text/javascript">
        function pageLoad() {
            $('#<%=ddl_courier_state_name.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_courier_client_name.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_client_name.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_state_name.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_branch_name.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_emp_name.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            dispatch_bill_validation1();
            support_format();
        }
        $(document).ready(function () {

            $.fn.dataTable.ext.errMode = 'none';
            var table = $('#<%=grd_inv_material.ClientID%>').DataTable({
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
              .appendTo('#<%=grd_inv_material.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';
            var table = $('#<%=grd_courier.ClientID%>').DataTable({
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
              .appendTo('#<%=grd_courier.ClientID%>_wrapper .col-sm-6:eq(0)');
            var table = $('#<%=gv_bill_material.ClientID%>').DataTable({
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
              .appendTo('#<%=gv_bill_material.ClientID%>_wrapper .col-sm-6:eq(0)');
            var table = $('#<%=gv_invoice.ClientID%>').DataTable({
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
              .appendTo('#<%=gv_invoice.ClientID%>_wrapper .col-sm-6:eq(0)');

            $('.date-picker3').datepicker({
                changeMonth: true,
                changeYear: true,
                yearRange: "1990:+100",
                showButtonPanel: true,
                dateFormat: 'mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                }
            });
            $('.date-picker3').focus(function () {
                $(".ui-datepicker-calendar").hide();

            });
            $(".date-picker3").attr("readonly", "true");

            $(".date-picker").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'mm/yy',
                yearRange: '1950',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-date .ui-datepicker-month .ui-datepicker-year :selected").val();

                }
            });

            $(".date-pickerk").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                yearRange: '1950:+100',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-date .ui-datepicker-month .ui-datepicker-year :selected").val();

                }
            });

            $(".date-pickers").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                yearRange: '1950',
                //onSelect: function (selected) {
                //    $(".date_join").datepicker("option", "minDate", selected)
                //}

            });

            $(".date_join").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                yearRange: '1950:2050',
                //onSelect: function (selected) {
                //    $(".date-pickers").datepicker("option", "maxDate", selected)

                //}
            });
            $(".date-pickers").attr("readonly", "true");
            $(".date_join").attr("readonly", "true");
            $(".date-picker").attr("readonly", "true");
            $(".date-pickerk").attr("readonly", "true");
            $(".invoice_date").attr("readonly", "true");
            $(".emp_name").attr("readonly", "true");
            $(".doc_type").attr("readonly", "true");
            $(".ship_address").attr("readonly", "true");
            $(".ship_address1").attr("readonly", "true");
            $(".invoice_no1").attr("readonly", "true");

        });

        function Req_validation1() {
            var t_client = document.getElementById('<%=ddl_sendmail_client.ClientID %>');
            var Selectedclient = t_client.options[t_client.selectedIndex].text;

            if (Selectedclient == "Select") {
                alert("Please Select Client.");
                t_client.focus();
                return false;
            }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;
        }

        function Req_validation() {
            var custcode = document.getElementById('<%=ddl_client.ClientID %>');

            if (custcode.value == "Select") {
                alert("Please Enter Client Name!!!");
                custcode.focus();
                return false;
            }
            var statecode = document.getElementById('<%=ddl_billing_state.ClientID %>');

            if (statecode.value == "Select") {
                alert("Please Select State Name!!!");
                statecode.focus();
                return false;
            }

            var ddl_unit = document.getElementById('<%=ddl_unitcode.ClientID %>');

            if (ddl_unit.value == "Select") {
                alert("Please Select Branch Name!!!");
                ddl_unit.focus();
                return false;
            }

            var txt_invoice_num1 = document.getElementById('<%=txt_invoice_num.ClientID %>');


            if (txt_invoice_num1.value == "Select") {

                alert("Please Enter Invoice Number Number !!");
                txt_invoice_num1.focus();
                return false;
            }


            var txt_start_date = document.getElementById('<%=txt_start_date.ClientID %>');


            if (txt_start_date.value == "") {

                alert("Please Enter Return Date !!");
                txt_start_date.focus();
                return false;
            }

            var txt_return_accepted_by = document.getElementById('<%=txt_return_accepted_by.ClientID %>');


            if (txt_return_accepted_by.value == "") {

                alert("Please Enter Acepted Porson Name !!");
                txt_return_accepted_by.focus();
                return false;
            }

            var txt_reason = document.getElementById('<%=txt_reason.ClientID %>');


            if (txt_reason.value == "") {

                alert("Please Enter Reason !!");
                txt_reason.focus();
                return false;
            }



        }
        function support_format() {
            var ddl_Receiver_type = document.getElementById('<%=ddl_Receiver_type.ClientID %>');
            var Selected_ddl_Receiver_type = ddl_Receiver_type.options[ddl_Receiver_type.selectedIndex].text;

            var ddl_client_name = document.getElementById('<%=ddl_client_name.ClientID %>');
            var Selected_ddl_client_name = ddl_client_name.options[ddl_client_name.selectedIndex].text;
            if (Selected_ddl_client_name == "Dewan Housing Finance Corporation Limited" && Selected_ddl_Receiver_type == "Invoice") {
                $(".region").show();
            }

            else { $(".region").hide(); }

            if (Selected_ddl_client_name == "BAJAJ ALLIANZ GENERAL INSURANCE COMPANY LTD." && Selected_ddl_Receiver_type == "Invoice") {
                $(".region").show();
            }
        }

        function Alphabet(e) {
            if (null != e) {
                isIE = document.all ? 1 : 0
                keyEntry = !isIE ? e.which : e.keyCode;
                if (((keyEntry >= '65') && (keyEntry <= '90')) || ((keyEntry >= '97') && (keyEntry <= '122')) || (keyEntry < '31') ||
                    (keyEntry == '32') || (keyEntry == '38') || ((keyEntry == '39') && (keyEntry == '34')) || (keyEntry == '44') || ((keyEntry >= '45') && (keyEntry <= '47')))

                    return true;
                else {
                    // alert('Please Enter Only Character values.');
                    return false;
                }
            }
        }
        function isnumber(e) {
            if (null != e) {
                isIE = document.all ? 1 : 0
                keyEntry = !isIE ? e.which : e.keyCode;
                if (((keyEntry >= '48') && (keyEntry <= '57')) || ((keyEntry == '39') && (keyEntry == '34')) || (keyEntry == '44') || ((keyEntry >= '45') && (keyEntry <= '47')))

                    return true;
                else {
                    // alert('Please Enter Only Character values.');
                    return false;
                }
            }
        }

        function AllowAlphabet12(e) {
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
        function bill_material_validation() {

            var txt_bill_rtn_date = document.getElementById('<%=txt_bill_rtn_date.ClientID %>');
            if (txt_bill_rtn_date.value == "") {
                alert("Please Select Material bill Return Date");
                txt_bill_rtn_date.focus();
                return false;
            }

            var txt_bill_reason = document.getElementById('<%=txt_bill_reason.ClientID %>');
            if (txt_bill_reason.value == "") {
                alert("Please Enter Material bill Return bill Reason");
                txt_bill_reason.focus();
                return false;
            }
        }
        var _validFileExtensions = [".jpg", ".jpeg", ".gif", ".pdf"];
        function ValidateSingleInput(oInput) {
            if (oInput.type == "file") {
                var sFileName = oInput.value;
                if (sFileName.length > 0) {
                    var blnValid = false;
                    for (var j = 0; j < _validFileExtensions.length; j++) {
                        var sCurExtension = _validFileExtensions[j];
                        if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                            blnValid = true;
                            break;
                        }
                    }

                    if (!blnValid) {
                        alert("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "));
                        oInput.value = "";
                        return false;
                    }
                }
            }
        }
        function Search_Gridview(strKey) {
            var strData = strKey.value.toLowerCase().split(" ");
            var tblData = document.getElementById("<%=gv_material.ClientID %>");
            var rowData;
            for (var i = 1; i < tblData.rows.length; i++) {
                rowData = tblData.rows[i].innerHTML;
                var styleDisplay = 'none';
                for (var j = 0; j < strData.length; j++) {
                    if (rowData.toLowerCase().indexOf(strData[j]) >= 0)
                        styleDisplay = '';
                    else {
                        styleDisplay = 'none';
                        break;
                    }
                }
                tblData.rows[i].style.display = styleDisplay;
            }
        }
        function unblock()
        { $.unblockUI(); }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            var st = $(this).find("input[id*='hidtab']").val();
            if (st == null)
                st = 0;
            $('[id$=tabs]').tabs({ selected: st });
        });
        function bill_dispatch_validation() {

            var txt_inv_dispath_date = document.getElementById('<%=txt_inv_dispath_date.ClientID %>');
            if (txt_inv_dispath_date.value == "") {
                alert("Please Select Dispatch Inventory Date");
                txt_inv_dispath_date.focus();
                return false;
            }
            var txt_inv_pod_no = document.getElementById('<%=txt_inv_pod_no.ClientID %>');
            if (txt_inv_pod_no.value == "") {
                alert("Please Enter Dispatch Inventory P.O.D Number");
                txt_inv_pod_no.focus();
                return false;
            }
            var fup_inv_upload = document.getElementById('<%=fup_inv_upload.ClientID %>');
            if (fup_inv_upload.value == "") {
                alert("Please Upload Report");
                fup_inv_upload.focus();
                return false;
            }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;
        }
        function Rq_validation() {
            var ddl_courier_category = document.getElementById('<%=ddl_courier_category.ClientID %>');
            var Selected_ddl_courier_category = ddl_courier_category.options[ddl_courier_category.selectedIndex].text;

            if (Selected_ddl_courier_category == "Select") {
                alert("Please Select Courier Category");
                ddl_courier_category.focus();
                return false;
            }
            var courier_date = document.getElementById('<%=courier_date.ClientID %>');

            if (courier_date.value == "") {
                alert("Please Select Courier Date");
                courier_date.focus();
                return false;
            }
            var ddl_courier_client_name = document.getElementById('<%=ddl_courier_client_name.ClientID %>');
            var Selected_ddl_courier_client_name = ddl_courier_client_name.options[ddl_courier_client_name.selectedIndex].text;

            if (Selected_ddl_courier_client_name == "Select") {
                alert("Please Select Courier Client Name");
                ddl_courier_client_name.focus();
                return false;
            }

            var ddl_courier_state_name = document.getElementById('<%=ddl_courier_state_name.ClientID %>');
            var Selected_ddl_courier_state_name = ddl_courier_state_name.options[ddl_courier_state_name.selectedIndex].text;

            if (Selected_ddl_courier_state_name == "Select") {
                alert("Please Select Courier State Name");
                ddl_courier_state_name.focus();
                return false;
            }






            var ddl_courier_branch_name = document.getElementById('<%=ddl_courier_branch_name.ClientID %>');
            var Selected_ddl_courier_branch_name = ddl_courier_branch_name.options[ddl_courier_branch_name.selectedIndex].text;

            if (Selected_ddl_courier_branch_name == "Select") {
                alert("Please Select Courier Branch Name");
                ddl_courier_branch_name.focus();
                return false;
            }

            var courier_address = document.getElementById('<%=courier_address.ClientID %>');

            if (courier_address.value == "") {
                alert("Please Enter Courier Address");
                courier_address.focus();
                return false;
            }

            var courier_weight = document.getElementById('<%=courier_weight.ClientID %>');

            if (courier_weight.value == "") {
                alert("Please Enter Courier Weight");
                courier_weight.focus();
                return false;
            }


            var courier_packet = document.getElementById('<%=courier_packet.ClientID %>');

            if (courier_packet.value == "") {
                alert("Please Enter Courier Packet");
                courier_packet.focus();
                return false;
            }

            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;
        }
        function name_date_hide() {
            var ddl_material_contract = document.getElementById('<%=ddl_material_contract.ClientID %>');
            var Selected_ddl_material_contract = ddl_material_contract.options[ddl_material_contract.selectedIndex].text;

            if (Selected_ddl_material_contract == "By Hand") {
                $(".hide_curier").show();
            }
            else { $(".hide_curier").hide(); }
        }
        function dispatch_bill_validation1() {
            var ddl_Receiver_type = document.getElementById('<%=ddl_Receiver_type.ClientID %>');
            var Selected_ddl_Receiver_type = ddl_Receiver_type.options[ddl_Receiver_type.selectedIndex].text;

            if (Selected_ddl_Receiver_type == "Select") {
                $(".material_hide").hide();
                $(".invoice").hide();
            }

            if (Selected_ddl_Receiver_type == "Material") {
                $(".material_hide").show();
                $(".invoice").hide();
            }

            if (Selected_ddl_Receiver_type == "Invoice") {
                $(".material_hide").hide();
                $(".invoice").show();
            }
        }
        function dispatch_bill_validation() {
            var ddl_Receiver_type = document.getElementById('<%=ddl_Receiver_type.ClientID %>');
            var Selected_ddl_Receiver_type = ddl_Receiver_type.options[ddl_Receiver_type.selectedIndex].text;

            if (Selected_ddl_Receiver_type == "Select") {
                alert("Please Select Receiver Type");
                ddl_Receiver_type.focus();
                return false;
            }


            var txt_month_year = document.getElementById('<%=txt_month_year.ClientID %>');
            if (txt_month_year.value == "") {
                alert("Please Select Month");
                txt_month_year.focus();
                return false;
            }

            var ddl_material_contract = document.getElementById('<%=ddl_material_contract.ClientID %>');
            var Selected_ddl_material_contract = ddl_material_contract.options[ddl_material_contract.selectedIndex].text;

            if (Selected_ddl_material_contract == "Select") {
                alert("Please Select Dispatch By");
                ddl_material_contract.focus();
                return false;
            }

            var ddl_client_name = document.getElementById('<%=ddl_client_name.ClientID %>');
            var Selected_ddl_client_name = ddl_client_name.options[ddl_client_name.selectedIndex].text;

            if (Selected_ddl_client_name == "Select") {
                alert("Please Select Client Name");
                ddl_client_name.focus();
                return false;
            }

            var ddl_state_name = document.getElementById('<%=ddl_state_name.ClientID %>');
            var Selected_ddl_state_name = ddl_state_name.options[ddl_state_name.selectedIndex].text;


            if (Selected_ddl_state_name == "Select") {
                alert("Please Select State Name");
                ddl_state_name.focus();
                return false;
            }

            var ddl_branch_name = document.getElementById('<%=ddl_branch_name.ClientID %>');
            var Selected_ddl_branch_name = ddl_branch_name.options[ddl_branch_name.selectedIndex].text;

            if (Selected_ddl_branch_name == "Select") {
                alert("Please Select Branch Name");
                ddl_branch_name.focus();
                return false;
            }

            var txt_bill_rtn_date = document.getElementById('<%=txt_bill_rtn_date.ClientID %>');
            if (txt_bill_rtn_date.value == "") {
                alert("Please Select Dispatch Date");
                txt_bill_rtn_date.focus();
                return false;
            }

            if (Selected_ddl_Receiver_type == "Material") {
                var ddl_emp_name = document.getElementById('<%=ddl_emp_name.ClientID %>');
                var Selected_ddl_emp_name = ddl_emp_name.options[ddl_emp_name.selectedIndex].text;

                if (Selected_ddl_emp_name == "Select") {
                    alert("Please Select Employee Name");
                    ddl_emp_name.focus();
                    return false;
                }
            }
            if (Selected_ddl_Receiver_type == "Invoice") {

                var lbl_invoice_num = document.getElementById('<%=lbl_invoice_num.ClientID %>');
                if (lbl_invoice_num.value == "") {
                    alert("Please Enter Invoice Number");
                    lbl_invoice_num.focus();
                    return false;
                }


                var txt_invoice_date = document.getElementById('<%=txt_invoice_date.ClientID %>');
                if (txt_invoice_date.value == "") {
                    alert("Please Enter Invoice Date");
                    txt_invoice_date.focus();
                    return false;
                }
                var txt_grand_total = document.getElementById('<%=txt_grand_total.ClientID %>');
                if (txt_grand_total.value == "") {
                    alert("Please Enter Grand Total");
                    txt_grand_total.focus();
                    return false;
                }
            }
            var txt_receiver_name = document.getElementById('<%=txt_receiver_name.ClientID %>');

            if (txt_receiver_name.value == "") {
                alert("Please Enter By Hand to Name");
                txt_receiver_name.focus();
                return false;
            }

            var txt_bill_rtn_date = document.getElementById('<%=txt_bill_rtn_date.ClientID %>');

            if (txt_bill_rtn_date.value == "") {
                alert("Please Enter Dispatch Date");
                txt_bill_rtn_date.focus();
                return false;
            }

            var txt_bill_reason = document.getElementById('<%=txt_bill_reason.ClientID %>');

            if (txt_bill_reason.value == "") {
                alert("Please Enter Shipping Address");
                txt_bill_reason.focus();
                return false;
            }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;
        }

        function dispatch_bill_validations() {
            var ddl_Receiver_type = document.getElementById('<%=ddl_Receiver_type.ClientID %>');
            var Selected_ddl_Receiver_type = ddl_Receiver_type.options[ddl_Receiver_type.selectedIndex].text;

            if (Selected_ddl_Receiver_type == "Select") {
                alert("Please Select Receiver Type");
                ddl_Receiver_type.focus();
                return false;
            }
            var ddl_material_contract = document.getElementById('<%=ddl_material_contract.ClientID %>');
            var Selected_ddl_material_contract = ddl_material_contract.options[ddl_material_contract.selectedIndex].text;

            if (Selected_ddl_material_contract == "Select") {
                alert("Please Select Dispatch By");
                ddl_material_contract.focus();
                return false;
            }

            var ddl_client_name = document.getElementById('<%=ddl_client_name.ClientID %>');
            var Selected_ddl_client_name = ddl_client_name.options[ddl_client_name.selectedIndex].text;

            if (Selected_ddl_client_name == "Select") {
                alert("Please Select Client Name");
                ddl_client_name.focus();
                return false;
            }

            var ddl_state_name = document.getElementById('<%=ddl_state_name.ClientID %>');
            var Selected_ddl_state_name = ddl_state_name.options[ddl_state_name.selectedIndex].text;


            if (Selected_ddl_state_name == "Select") {
                alert("Please Select State Name");
                ddl_state_name.focus();
                return false;
            }

            var ddl_branch_name = document.getElementById('<%=ddl_branch_name.ClientID %>');
            var Selected_ddl_branch_name = ddl_branch_name.options[ddl_branch_name.selectedIndex].text;

            if (Selected_ddl_branch_name == "Select") {
                alert("Please Select Branch Name");
                ddl_branch_name.focus();
                return false;
            }

            if (Selected_ddl_Receiver_type == "Material") {
                var ddl_emp_name = document.getElementById('<%=ddl_emp_name.ClientID %>');
                var Selected_ddl_emp_name = ddl_emp_name.options[ddl_emp_name.selectedIndex].text;

                if (Selected_ddl_emp_name == "Select") {
                    alert("Please Select Employee Name");
                    ddl_emp_name.focus();
                    return false;
                }
            }


            var txt_bill_rtn_date = document.getElementById('<%=txt_bill_rtn_date.ClientID %>');

            if (txt_bill_rtn_date.value == "") {
                alert("Please Enter Dispatch Date");
                txt_bill_rtn_date.focus();
                return false;
            }

            var txt_report_receiving = document.getElementById('<%=txt_report_receiving.ClientID %>');
            var txt_bill_rtn_date = document.getElementById('<%=txt_bill_rtn_date.ClientID %>');
            if ((Selected_ddl_material_contract == "By Curier") || (Selected_ddl_material_contract == "By Postal") && txt_bill_rtn_date != "") {
                if (txt_report_receiving.value == "") {
                    alert("Please Upload Receiving Report");
                    txt_report_receiving.focus();
                    return false;
                }
            }

            var txt_receiver_name = document.getElementById('<%=txt_receiver_name.ClientID %>');

            if (txt_receiver_name.value == "") {
                alert("Please Enter By Hand to Name");
                txt_receiver_name.focus();
                return false;
            }


            var txt_receiv_date = document.getElementById('<%=txt_receiv_date.ClientID %>');

            if (txt_receiv_date.value == "") {
                alert("Please Enter Handover Date");
                txt_receiv_date.focus();
                return false;
            }

            //End

            // Rohini Manual mail Validations
        }
    </script>
    <script type="text/javascript">

        function uniform_entery_val() {

            var ddl_emp_name = document.getElementById('<%=ddl_uniform_emp_type.ClientID %>');
            var Selected_ddl_emp_name = ddl_emp_name.options[ddl_emp_name.selectedIndex].text;

            var ddl_uniform_client = document.getElementById('<%=ddl_uniform_entry_client.ClientID %>');
            var Selected_ddl_uniform_client = ddl_uniform_client.options[ddl_uniform_client.selectedIndex].text;

            var ddl_uniform_state = document.getElementById('<%=ddl_uniform_state_id.ClientID %>');
            var Selected_ddl_uniform_state = ddl_uniform_state.options[ddl_uniform_state.selectedIndex].text;

            var ddl_uniform_branch = document.getElementById('<%=ddl_uniform_branch.ClientID %>');
            var Selected_ddl_uniform_branch = ddl_uniform_branch.options[ddl_uniform_branch.selectedIndex].text;

            var ddl_uniform_branch = document.getElementById('<%=ddl_uniform_branch.ClientID %>');
            var Selected_ddl_uniform_branch = ddl_uniform_branch.options[ddl_uniform_branch.selectedIndex].text;

            var ddl_uniform_ename = document.getElementById('<%=ddl_uniform_emp_name.ClientID %>');
            var Selected_ddl_uniform_ename = ddl_uniform_ename.options[ddl_uniform_ename.selectedIndex].text;

            var ddl_dispatch_by = document.getElementById('<%=ddl_dispatch_d1.ClientID %>');
            var Selected_ddl_dispatch_by = ddl_dispatch_by.options[ddl_dispatch_by.selectedIndex].text;

            if (Selected_ddl_emp_name == "Select") {
                alert("Please Select Employee Type");
                ddl_emp_name.focus();
                return false;
            }

            if (Selected_ddl_uniform_client == "Select") {
                alert("Please Select Client Name");
                ddl_uniform_client.focus();
                return false;
            }

            if (Selected_ddl_uniform_state == "Select") {
                alert("Please Select State Name");
                ddl_uniform_state.focus();
                return false;

            }

            if (Selected_ddl_uniform_branch == "Select") {
                alert("Please Select Branch_Name");
                ddl_uniform_branch.focus();
                return false;
            }

            if (Selected_ddl_uniform_ename == "Select") {
                alert("Please Select employee name");
                ddl_uniform_ename.focus();
                return false;
            }

            if (Selected_ddl_dispatch_by == "Select") {
                alert("Please Select dispatch by");
                ddl_dispatch_by.focus();
                return false;
            }

            var txt_receiv_date = document.getElementById('<%=Disp_uniform_date.ClientID %>');

            if (txt_receiv_date.value == "")
            {
                alert("Please Select Dispatch Date");
                txt_receiv_date.focus();
                return false;
            }

            var txt_deliver_uniform_dt = document.getElementById('<%=deliver_uniform_dt.ClientID %>');

            if (txt_deliver_uniform_dt.value == "") {
                alert("Please Select Delivery Date");
                txt_deliver_uniform_dt.focus();
                return false;
            }

            var txt_pod_uniform_no = document.getElementById('<%=pod_uniform_no.ClientID %>');

            if (txt_pod_uniform_no.value == "") {
                alert("Please Enter POD No");
                txt_pod_uniform_no.focus();
                return false;
            }

            var txt_set_uniform_no = document.getElementById('<%=set_uniform_no.ClientID %>');

            if (txt_set_uniform_no.value == "") {
                alert("Please Enter NO of Set");
                txt_set_uniform_no.focus();
                return false;
            }

            var txt_uniform_remark = document.getElementById('<%=uniform_remark.ClientID %>');

            if (txt_uniform_remark.value == "") {
                alert("Please Enter Remark");
                txt_uniform_remark.focus();
                return false;
            }
           
        }
        $(document).ready(function () {
            $.fn.dataTable.ext.errMode = 'none';
            var table = $('#<%=gv_uniform_entry.ClientID%>').DataTable({
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
               .appendTo('#<%=gv_uniform_entry.ClientID%>_wrapper .col-sm-6:eq(0)');

        });

    </script>
    <style>
        .grid-view
        {
            height: auto;
            max-height: 400px;
            overflow-x: hidden;
            overflow-y: auto;
            font-family: Verdana;
        }
    </style>


</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cph_righrbody" runat="Server">
    <div class="container-fluid">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:Panel ID="Panel1" runat="server" class="panel panel-primary">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-lg-11 col-md-11 col-sm-11 col-xs-12">
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>RETURN MATERIAL</b> </div>
                    </div>
                    <div class="col-lg-1 col-md-1 col-sm-1 col-xs-12 text-right">
                        <asp:LinkButton ID="panImgLnkBtn" runat="server" OnClientClick="openWindow();return false;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <br />
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-4"></div>
                    <div class="col-sm-9">
                        <div style="text-align: left; color: white; font-size: small;"><b>Return Material Details</b></div>
                    </div>
                    <div class="col-sm-2 text-left">
                    </div>
                </div>
            </div>
            <div class="panel-body">
                <div id="tabs" style="background: #f3f1fe; border: 1px solid #e2e2dd; margin-bottom: 20px; margin-top: 20px; border-radius: 10px">
                    <asp:HiddenField ID="hidtab" Value="0" runat="server" />
                    <ul>
                        <li><a href="#tabs-1"><b>Return Material</b></a></li>
                        <li><a href="#tabs-2"><b>Dispatch Bill</b></a></li>
                        <li><a href="#tabs-3"><b>Dispatch Inventory</b></a></li>
                        <li><a href="#tabs-4"><b>Send Email</b></a></li>
                        <li><a href="#tabs-5"><b>Courier</b></a></li>
                        <li><a href="#tabs-6"><b>Uniform Entry</b></a></li>
                    </ul>

                    <div id="tabs-1">
                        <br />
                        <div class="container-fluid" style="background: white; border-radius: 10px; border: 1px solid white">
                            <br />
                            <div class="row">
                                <div class="col-md-3 col-xs-12">
                                    <b>Client :</b>
                                    <asp:Label ID="Label9" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                    <asp:DropDownList ID="ddl_client" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged"
                                        class="form-control" MaxLength="20">
                                    </asp:DropDownList>

                                </div>
                                <div class="col-sm-3 col-xs-12">
                                    <b>State :</b>
                                    <asp:Label ID="Label8" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                    <asp:DropDownList ID="ddl_billing_state" runat="server" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged1" AutoPostBack="true"
                                        class="form-control" MaxLength="20">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-sm-3 col-xs-12">
                                    <b>Branch Name :</b>
                                    <asp:Label ID="Label7" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                    <asp:DropDownList ID="ddl_unitcode" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="fill_invoice_no">
                                        <asp:ListItem Value="ALL">ALL</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Invoice Number :</b>
                                    <asp:Label ID="Label1" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                    <asp:DropDownList ID="txt_invoice_num" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="fill_gridview"></asp:DropDownList>
                                </div>

                            </div>
                            <br />
                            <div class="row">

                                <div class="col-sm-2 col-sx-12" style="display: none">
                                    <b>Material Type :</b>
                                    <asp:Label ID="Label5" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                    <asp:DropDownList ID="ddl_product" runat="server" class="form-control" Width="170px" MaxLength="30">

                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                        <asp:ListItem Value="chemicals">Chemicals</asp:ListItem>
                                        <asp:ListItem Value="housekeeping_material">Housekeeping Materials</asp:ListItem>
                                        <asp:ListItem Value="Uniform">Uniform</asp:ListItem>
                                        <asp:ListItem Value="Shoes">Shoes</asp:ListItem>
                                        <asp:ListItem Value="Other">Other</asp:ListItem>

                                    </asp:DropDownList>
                                </div>
                                <%-- <div class="col-sm-2 col-xs-12">
                                POD Number :
                                         <asp:Label ID="Label6" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                <asp:TextBox ID="txt_number" runat="server" class="form-control text_box" MaxLength="10" onkeypress="return AllowAlphabet12(event)"></asp:TextBox>
                            </div>--%>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Return Date :</b>
                                    <asp:Label ID="Label2" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                    <asp:TextBox ID="txt_start_date" runat="server" MaxLength="10" class="form-control date-picker"></asp:TextBox>
                                </div>




                                <div class="col-sm-3 col-xs-12">
                                    <b>Return Accepted By :</b><asp:Label ID="Label3" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                    <asp:TextBox ID="txt_return_accepted_by" runat="server" class="form-control text_box" MaxLength="50" onkeypress="return Alphabet(event)"></asp:TextBox>
                                </div>
                                <div class="col-sm-3 col-xs-12">
                                    <b>Reason :</b><asp:Label ID="Label4" runat="server" ForeColor="Red" Text="*"></asp:Label>

                                    <asp:TextBox ID="txt_reason" runat="server" class="form-control text_box" MaxLength="50" TextMode="MultiLine" onkeypress="return AllowAlphabet12(event)"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                        </div>
                        <br />
                        <asp:Panel ID="Panel2" runat="server" CssClass="grid-view">
                            <asp:GridView ID="ItemGridView" class="table" runat="server" Font-Size="X-Small"
                                AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC"
                                BorderStyle="None" BorderWidth="1px" CellPadding="3" DataKeyNames="" OnPreRender="ItemGridView_PreRender">
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
                                    <asp:BoundField DataField="item_type" HeaderText="ITEM TYPE" SortExpression="item_type" />
                                    <asp:BoundField DataField="ITEM_CODE" HeaderText="ITEM CODE" SortExpression="ITEM_CODE" />
                                    <asp:BoundField DataField="PARTICULAR" HeaderText="ITEM NAME" SortExpression="PARTICULAR" />
                                    <asp:BoundField DataField="DESCRIPTION" HeaderText="DESCRIPTION" SortExpression="DESCRIPTION" />
                                    <asp:BoundField DataField="QUANTITY" HeaderText="QUANTITY" SortExpression="QUANTITY" />
                                    <asp:TemplateField HeaderText="RETURN QUANTITY">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txt_returnqty" runat="server" OnTextChanged="validation_num" onkeypress="return isnumber(event)" AutoPostBack="true" Text='<%# Eval("Return_Material")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="POD NO">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txt_opd_no" runat="server" onkeypress="return AllowAlphabet12(event)" Text='<%# Eval("POD_NUM")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                        </asp:Panel>
                        <br />
                        <div class="row text-center">
                            <asp:Button ID="btn_Save" runat="server" CssClass="btn btn-primary"
                                Text="Save" OnClientClick="return Req_validation();" OnClick="btn_save_Click" />
                            <asp:Button ID="btn_close" runat="server" CssClass="btn btn-danger"
                                Text="Close" OnClick="btn_close_Click" />
                        </div>


                    </div>
                    <div id="tabs-2">
                        <br />
                        <div class="container-fluid" style="background: white; border-radius: 10px; border: 1px solid white">
                            <br />

                            <%--komal 3-07-19--%>

                            <div class="row">

                                <div class="col-md-2 col-xs-12">
                                    <b>Receiver Type : </b><span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_Receiver_type" runat="server" CssClass="form-control text_box" OnSelectedIndexChanged="ddl_Receiver_type_SelectedIndexChanged" Width="100%" onchange="dispatch_bill_validation1();" AutoPostBack="true">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="1">Material</asp:ListItem>
                                        <asp:ListItem Value="2">Invoice</asp:ListItem>
                                        <asp:ListItem Value="3">Dublicate Id Card</asp:ListItem>


                                    </asp:DropDownList>
                                </div>

                                <asp:Panel ID="Panel_inv_type" runat="server">
                                    <div class="col-md-2 col-xs-12">
                                        <b>Billing Type :</b> <span style="color: red">*</span>
                                        <asp:DropDownList ID="ddl_inv_details" runat="server" CssClass="form-control text_box" Width="100%" onchange="dispatch_bill_validation1();">
                                            <asp:ListItem Value="0">Select</asp:ListItem>
                                            <asp:ListItem Value="5">ALL</asp:ListItem>
                                            <asp:ListItem Value="1">Manpower</asp:ListItem>
                                            <asp:ListItem Value="2">Material</asp:ListItem>
                                            <asp:ListItem Value="3">Conveyance</asp:ListItem>
                                            <asp:ListItem Value="4">DeepClean</asp:ListItem>


                                        </asp:DropDownList>
                                    </div>

                                </asp:Panel>



                                <asp:Panel ID="Panel_month" runat="server">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Month :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_month_year" runat="server" MaxLength="10" class="form-control date-picker3"></asp:TextBox>
                                    </div>
                                </asp:Panel>



                                <div class="col-md-2 col-xs-12">
                                    <b>Dispatch By :</b><span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_material_contract" runat="server" CssClass="form-control text_box" Width="100%">
                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                        <asp:ListItem Value="0">By Curier</asp:ListItem>
                                        <asp:ListItem Value="1">By Postal</asp:ListItem>
                                        <asp:ListItem Value="2">By Hand</asp:ListItem>

                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12 text-left">
                                    <b>Client Name :</b>  <span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_client_name" class="form-control pr_state js-example-basic-single" runat="server" OnSelectedIndexChanged="ddl_client_name_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12 region" style="display: none">
                                    <b>Region :</b>
                                    <asp:DropDownList ID="ddlregion" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_region_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12 text-left">
                                    <b>State Name :</b>   <span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_state_name" runat="server" DataTextField="STATE" DataValueField="STATE" class="form-control text_box" OnSelectedIndexChanged="ddl_state_name_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>

                                <asp:Panel ID="Panel_invoice" runat="server">
                                    <div class="col-md-2 col-xs-12">
                                        <b>Invoice type :</b><span class="text-red">*</span>
                                        <asp:DropDownList ID="ddl_invoice_type" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_invoice_type_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Value="1">CLUB</asp:ListItem>
                                            <asp:ListItem Value="2">UNCLUB</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </asp:Panel>



                                <div class="col-sm-2 col-xs-12 text-left">
                                    <b>Branch Name :</b> <span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_branch_name" class="form-control pr_state js-example-basic-single" OnSelectedIndexChanged="ddl_branch_name_SelectedIndexChanged" runat="server" AutoPostBack="true" />
                                </div>

                                <div class="col-sm-2 col-xs-12 text-left material_hide">
                                    <b>Employee Name :</b> <span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_emp_name" class="form-control pr_state" OnSelectedIndexChanged="ddl_emp_name_SelectedIndexChanged" runat="server" AutoPostBack="true" />
                                </div>


                                <asp:Panel ID="desigpanel" runat="server">
                                    <div class="col-md-2 col-xs-12">
                                        <b>Designation :</b>  <span style="color: red">*</span>
                                        <asp:DropDownList ID="ddl_designation" class="form-control pr_state " runat="server" OnSelectedIndexChanged="ddl_designation_SelectedIndexChanged" AutoPostBack="true" />
                                    </div>
                                </asp:Panel>


                                <div class="col-sm-2 col-xs-12 text-left hide_curier" style="display: none;">
                                    <b>Employee Dispatch Name :</b> <span style="color: red">*</span>
                                    <asp:TextBox ID="txt_emp_name" class="form-control" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12 text-left hide_curier" style="display: none;">
                                    <b>Dispatch Date :</b><span style="color: red">*</span>
                                    <asp:TextBox ID="txt_dispatch_date" class="form-control date_join" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12 ">
                                </div>
                                <asp:Panel runat="server" ID="Panel_inv_no">
                                    <div class="col-sm-2 col-xs-12 invoice" style="display: none">
                                        <b>Invoice Number :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="lbl_invoice_num" runat="server" MaxLength="10" class="form-control invoice_no1"></asp:TextBox>

                                    </div>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="Panel_inv_date">
                                    <div class="col-sm-2 col-xs-12 invoice" style="display: none">
                                        <b>Invoice Date :</b><span style="color: red">*</span>
                                        <asp:Label ID="Label14" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                        <asp:TextBox ID="txt_invoice_date" runat="server" MaxLength="10" class="form-control invoice_date "></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="Panel_receiver_name">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Receiver Name :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_receiver_name" runat="server" class="form-control invoice_no" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <%--  for dublicate id receiver name--%>
                                <asp:Panel runat="server" ID="Panel_dub_receiver_name">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Receiver Name :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_dub_receiver_name" runat="server" class="form-control invoice_no" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <%--  for dublicate id receiver date--%>

                                <asp:Panel runat="server" ID="Panel_du_receiver_date">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Receiving Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_du_receiver_date" runat="server" MaxLength="10" class="form-control date_join"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <%--  for dublicate id receiver name 3--%>
                                <asp:Panel runat="server" ID="Panel_dub_receiver_name3">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Third Receiver Name :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_dub_receiver_name_third" runat="server" class="form-control invoice_no" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <%--  for dublicate id receiver date 3--%>

                                <asp:Panel runat="server" ID="Panel_du_receiver_date_3">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Third Receiving Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_du_receiver_date_third" runat="server" MaxLength="10" class="form-control date_join"></asp:TextBox>
                                    </div>
                                </asp:Panel>





                                <%--   receiver date--%>
                                <asp:Panel runat="server" ID="Panel_receiving_date">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Receiving Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_receiv_date" runat="server" MaxLength="10" class="form-control date_join"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <%--for second receiver name--%>

                                <asp:Panel runat="server" ID="Sec_Panel_receiver_name">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Second Receiver Name :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_sec_rece_name" runat="server" class="form-control invoice_no" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="Sec_Panel_receiving_date">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Second Receiving Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_sec_rece_date" runat="server" MaxLength="10" class="form-control date-pickers"></asp:TextBox>

                                    </div>
                                </asp:Panel>
                            </div>
                            <br />
                            <div class="row">

                                <asp:Panel runat="server" ID="Panel_grand_total">
                                    <div class="col-sm-2 col-xs-12 invoice" style="display: none">
                                        <b>Grand Total :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_grand_total" runat="server" MaxLength="10" class="form-control invoice_no" onkeypress="return isnumber(event)"></asp:TextBox>

                                    </div>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="Panel_dispatch">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Dispatch Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_bill_rtn_date" runat="server" MaxLength="10" class="form-control date-pickerk"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="Panel_dis_sec">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Second Dublicate Dispatch Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_dub_dispatch_date" runat="server" MaxLength="10" class="form-control date-pickerk"></asp:TextBox>
                                    </div>
                                </asp:Panel>


                                <asp:Panel runat="server" ID="Panel_dis_third">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Third Dublicate Dispatch Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_dub_dispatch_date3" runat="server" MaxLength="10" class="form-control date-pickerk"></asp:TextBox>
                                    </div>
                                </asp:Panel>


                                <asp:Panel runat="server" ID="dispatch_date_sec">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Second Dispatch Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_sec_dispatch_date" runat="server" MaxLength="10" class="form-control date-pickers"></asp:TextBox>
                                    </div>
                                </asp:Panel>


                                <asp:Panel runat="server" ID="pod_number">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>POD Number :</b><asp:Label ID="Label12" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                        <asp:TextBox ID="txt_pod_number" runat="server" class="form-control " onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>


                                <asp:Panel runat="server" ID="Panel_du_pod">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Second Dublicate POD Number :</b><asp:Label ID="Label18" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                        <asp:TextBox ID="txt_sec_du_pod" runat="server" class="form-control " onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="Panel_du_pod_3">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Third Dublicate POD Number :</b><asp:Label ID="Label19" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                        <asp:TextBox ID="txt_third_du_pod" runat="server" class="form-control " onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>


                                <asp:Panel runat="server" ID="sec_pod_number">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Second POD Number :</b><asp:Label ID="Label13" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                        <asp:TextBox ID="txt_sec_pod" runat="server" class="form-control " onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>



                                <asp:Panel runat="server" ID="Panel_shipping">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Shipping Address :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_bill_reason" runat="server" class="form-control ship_address1" MaxLength="50" TextMode="MultiLine" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>


                                <asp:Panel runat="server" ID="Panel_id">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>ID :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_id" runat="server" class="form-control ship_address1" MaxLength="2" TextMode="MultiLine" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="Panel_id_set">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>ID No Set :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_id_set" runat="server" class="form-control ship_address1" MaxLength="2" TextMode="MultiLine" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="report_panel">
                                    <div class="col-sm-2 col-xs-12" style="margin-top: 1.5em;">
                                        <b>Report Receiving :</b>
                                        <asp:FileUpload runat="server" ID="txt_report_receiving" meta:resourcekey="photo_uploadResource1" onchange="ValidateSingleInput(this);" />
                                        <b>JPEG JPG GIF PDF</b>
                                    </div>

                                </asp:Panel>
                            </div>


                            <br />

                            <div class="row text-center">
                                <asp:Button ID="btn_bill_save" runat="server" CssClass="btn btn-primary"
                                    Text="Save" OnClick="btn_bill_save_Click" />

                                <asp:Button ID="btn_dispatch_save" runat="server" CssClass="btn btn-primary"
                                    Text="Save" OnClick="btn_dispatch_save_Click" OnClientClick="return dispatch_bill_validations();" />

                                <asp:Button ID="btn_update" runat="server" CssClass="btn btn-primary"
                                    Text="Update" OnClick="btn_update_Click" OnClientClick="return dispatch_bill_validations();" />

                                <asp:Button ID="Print_Report" runat="server" CssClass="btn btn-primary invoice"
                                    Text="Dispatch Copy" OnClick="Print_Report_Click" OnClientClick="return " />

                                <asp:Button ID="btn_uniform_shoes" runat="server" CssClass="btn btn-primary"
                                    Text="Uniform/Shoes Recieving Proof" OnClick="btn_uniform_shoes_Click" Width="13%" />

                                <asp:Button ID="second_btn_uniform_shoes" runat="server" CssClass="btn btn-primary"
                                    Text="Second Uniform/Shoes" OnClick="second_btn_uniform_shoes_Click" Width="15%" />

                                <asp:Button ID="btn_bill_close" runat="server" CssClass="btn btn-danger"
                                    Text="Close" />
                            </div>


                            <br />
                        </div>
                        <%--11-07-19 komal--%>


                        <br />
                        <%--  <div class="row">
                            <div class="col-sm-2 col-xs-12 text-left">
                                <b>Client Name : </b><span style="color: red">*</span>
                                <asp:DropDownList ID="ddl_clientname1" class="form-control pr_state js-example-basic-single" runat="server" OnSelectedIndexChanged="ddl_clientname1_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                        </div>--%>
                        <br />

                        <asp:Panel ID="Panel6" runat="server" CssClass="grid-view" meta:resourcekey="Panel6Resource1" Style="overflow-x: auto;">

                            <div class="row">
                                <div class="col-sm-10 col-xs-12"></div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Search :</b>
                                    <asp:TextBox runat="server" ID="txt_search" CssClass=" form-control" onkeyup="Search_Gridview(this)" />
                                </div>
                            </div>
                            <br />
                            <asp:GridView ID="gv_material" class="table" runat="server" Font-Size="X-Small"
                                AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" meta:resourcekey="gv_materialResource1"
                                BorderStyle="None" BorderWidth="1px" CellPadding="3" OnRowDataBound="gv_material_RowDataBound" Width="100%">
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
                                    <asp:TemplateField HeaderText="No.">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_srnumber" runat="server"
                                                Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" />
                                    <asp:BoundField DataField="client_name" HeaderText="Client Name" SortExpression="client_name" />
                                    <asp:BoundField DataField="state_dispatch" HeaderText="State Name" SortExpression="state_dispatch" />
                                    <asp:BoundField DataField="unit_code" HeaderText="Branch Name" SortExpression="unit_code" />
                                    <asp:BoundField DataField="emp_code" HeaderText="Employee Name" SortExpression="emp_code" />
                                    <asp:BoundField DataField="designation" HeaderText="Designation" SortExpression="designation" />
                                    <asp:BoundField DataField="dispatch_by" HeaderText="Dispatch By" SortExpression="dispatch_by" />
                                    <asp:BoundField DataField="dispatch_date" HeaderText="Dispatch Date" SortExpression="dispatch_date" />
                                    <asp:BoundField DataField="receiving_date" HeaderText="Receiving Date" SortExpression="receiving_date" />
                                    <asp:BoundField DataField="receiver_name_invoice" HeaderText="Receiver Name" SortExpression="receiver_name_invoice" />
                                    <asp:BoundField DataField="pod_no" HeaderText="POD Number" SortExpression="pod_no" />
                                    <asp:BoundField DataField="shipping_address" HeaderText="Shipping Address" SortExpression="shipping_address" />
                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="link_uniform" Text="Edit" runat="server" CssClass="btn btn-primary" Style="color: white" OnClick="link_uniform_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Download/Status">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnk_uniform" runat="server" Width="100%" CausesValidation="false" Text="Download" Style="color: white" OnCommand="lnk_uniform_Command" CommandArgument='<%# Eval("stamp_copy")%>' CssClass="btn btn-primary"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="stamp_copy" HeaderText="Stamp copy" SortExpression="stamp_copy" />
                                    <asp:BoundField DataField="receiver_type" HeaderText="Receiver Type" SortExpression="receiver_type" />
                                    <asp:BoundField DataField="sec_dispatch_date" HeaderText="Second Dispatch Date" SortExpression="sec_dispatch_date" />
                                    <asp:BoundField DataField="remaining_no_set" HeaderText="Remaining Uniform" SortExpression="remaining_no_set" />
                                    <asp:BoundField DataField="sec_receiver_name" HeaderText="Second Receiver Name" SortExpression="sec_receiver_name" />
                                    <asp:BoundField DataField="sec_receiving_date" HeaderText="Second Receiver Date" SortExpression="sec_receiving_date" />
                                    <asp:BoundField DataField="sec_pod_no" HeaderText="Second POD Number" SortExpression="sec_pod_no" />

                                    <asp:TemplateField HeaderText="Second Download/Status">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnk_uniform_second" runat="server" CausesValidation="false" Text="Download" Style="color: white" OnCommand="lnk_uniform_second_Command" CommandArgument='<%# Eval("second_stamp_copy")%>' CssClass="btn btn-primary"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="uniform_no_flag" HeaderText="unniform_no_flag" SortExpression="unniform_no_flag" />

                                </Columns>
                            </asp:GridView>

                        </asp:Panel>


                        <%--for dublicate id card--%>


                        <asp:Panel ID="Panel8" runat="server" CssClass="grid-view" meta:resourcekey="Panel8Resource1" Style="overflow-x: auto;">

                            <%-- <div class="row">
                                <div class="col-sm-10 col-xs-12"></div> 
                              <div class="col-sm-2 col-xs-12">
                                    Search :
                        <asp:TextBox runat="server" ID="txt_search_dublicate" CssClass=" form-control" onkeyup="Search_Gridview(this)" />
                                </div>
                                       

                            </div>--%>

                            <br />
                            <asp:GridView ID="gv_dublicate_id_card" class="table" runat="server" Font-Size="X-Small"
                                AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" meta:resourcekey="gv_dublicate_id_cardResource1"
                                BorderStyle="None" BorderWidth="1px" CellPadding="3" OnRowDataBound="gv_dublicate_id_card_RowDataBound" Width="100%">
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
                                    <asp:TemplateField HeaderText="No.">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_srnumber" runat="server"
                                                Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" />

                                    <asp:BoundField DataField="client_name" HeaderText="Client Name" SortExpression="client_name" />
                                    <asp:BoundField DataField="state_name" HeaderText="State Name" SortExpression="state_name" />
                                    <asp:BoundField DataField="unit_code" HeaderText="Branch Name" SortExpression="unit_code" />
                                    <asp:BoundField DataField="emp_code" HeaderText="Employee Name" SortExpression="emp_code" />
                                    <asp:BoundField DataField="designation" HeaderText="Designation" SortExpression="designation" />
                                    <asp:BoundField DataField="du_dispatch_by" HeaderText="Dispatch By" SortExpression="du_dispatch_by" />
                                    <asp:BoundField DataField="dispatch_date_du" HeaderText="Dublicate Dispatch Date 1" SortExpression="dispatch_date_du" />
                                    <asp:BoundField DataField="receiving_date_du" HeaderText="Dublicate Receiving Date 1" SortExpression="receiving_date_du" />
                                    <asp:BoundField DataField="du_receiving_name" HeaderText="Dublicate Receiver Name 1" SortExpression="du_receiving_name" />
                                    <asp:BoundField DataField="du_pod1" HeaderText="Dublicate POD Number 1" SortExpression="du_pod1" />
                                    <asp:BoundField DataField="shipping_address" HeaderText="Shipping Address" SortExpression="shipping_address" />
                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="link_id_dublicate" Text="Edit 1" runat="server" CssClass="btn btn-primary" Style="color: white" OnClick="link_id_dublicate_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnk_dub_id" runat="server" CausesValidation="false" Text="Download 1" Style="color: white" OnCommand="lnk_dub_id_Command" CommandArgument='<%# Eval("du_stamp_copy")%>' CssClass="btn btn-primary"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="du_stamp_copy" HeaderText="Stamp copy" SortExpression="du_stamp_copy" />
                                    <asp:BoundField DataField="id_no_set" HeaderText="Id no set" SortExpression="id_no_set" />

                                    <%--for second dublicate id card--%>

                                    <asp:BoundField DataField="du_dispatch_by2" HeaderText="Dispatch By 2" SortExpression="du_dispatch_by2" />
                                    <asp:BoundField DataField="dispatch_date_du2" HeaderText="Dublicate Dispatch Date 2" SortExpression="dispatch_date_du2" />
                                    <asp:BoundField DataField="receiving_date_du2" HeaderText="Dublicate Receiving Date 2" SortExpression="receiving_date_du2" />
                                    <asp:BoundField DataField="du_receiving_name2" HeaderText="Dublicate Receiver Name 2" SortExpression="du_receiving_name2" />
                                    <asp:BoundField DataField="du_pod_no2" HeaderText="Dublicate POD Number 2" SortExpression="du_pod_no2" />

                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="link_id_dublicate2" Text="Edit 2" runat="server" CssClass="btn btn-primary" Style="color: white" OnClick="link_id_dublicate2_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnk_dub_id2" runat="server" CausesValidation="false" Text="Download 2" Style="color: white" OnCommand="lnk_dub_id2_Command" CommandArgument='<%# Eval("du_stamp_copy")%>' CssClass="btn btn-primary"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="du_stamp_copy2" HeaderText="Stamp copy" SortExpression="du_stamp_copy2" />

                                    <%--for 3 dublicate id card--%>

                                    <asp:BoundField DataField="du_dispatch_by3" HeaderText="Dispatch By 3" SortExpression="du_dispatch_by3" />
                                    <asp:BoundField DataField="dispatch_date_du3" HeaderText="Dublicate Dispatch Date 3" SortExpression="dispatch_date_du3" />
                                    <asp:BoundField DataField="receiving_date_du3" HeaderText="Dublicate Receiving Date 3" SortExpression="receiving_date_du3" />
                                    <asp:BoundField DataField="du_receiving_name3" HeaderText="Dublicate Receiver Name 3" SortExpression="du_receiving_name3" />
                                    <asp:BoundField DataField="du_pod_no3" HeaderText="Dublicate POD Number 3" SortExpression="du_pod_no3" />

                                    <asp:TemplateField HeaderText="Edit 3">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="link_id_dublicate3" Text="Edit 3" runat="server" CssClass="btn btn-primary" Style="color: white" OnClick="link_id_dublicate3_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnk_dub_id3" runat="server" CausesValidation="false" Text="Download 3" Style="color: white" OnCommand="lnk_dub_id3_Command" CommandArgument='<%# Eval("du_stamp_copy3")%>' CssClass="btn btn-primary"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="du_stamp_copy3" HeaderText="Stamp copy" SortExpression="du_stamp_copy3" />



                                </Columns>
                            </asp:GridView>

                        </asp:Panel>



                        <%--   13-07-19 komal--%>

                        <br />
                        <div class="container" style="overflow-x: auto">

                            <asp:Panel ID="Panel7" runat="server">
                                <asp:GridView ID="gv_invoice" class="table" runat="server" Font-Size="X-Small" OnRowDataBound="gv_invoice_RowDataBound" OnPreRender="gv_invoice_PreRender"
                                    AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC"
                                    BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%">
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
                                        <asp:TemplateField HeaderText="No.">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_srnumber" runat="server"
                                                    Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                        <asp:BoundField DataField="client_name" HeaderText="Client Name" SortExpression="client_name" />
                                        <asp:BoundField DataField="state_dispatch" HeaderText="State Name" SortExpression="state_dispatch" />
                                        <asp:BoundField DataField="unit_code" HeaderText="Branch Name" SortExpression="unit_code" />
                                        <asp:BoundField DataField="dispatch_by" HeaderText="Dispatch By" SortExpression="dispatch_by" />
                                        <asp:BoundField DataField="invoice_no" HeaderText="Invoice No" SortExpression="invoice_no" />

                                        <asp:BoundField DataField="invoice_date" HeaderText=" Invoice Date" SortExpression="invoice_date" />
                                        <asp:BoundField DataField="grand_total" HeaderText=" Grand Total" SortExpression="grand_total" />
                                        <asp:BoundField DataField="dispatch_date" HeaderText="Dispatch Date" SortExpression="dispatch_date" />
                                        <asp:BoundField DataField="receiving_date" HeaderText="Receiving Date" SortExpression="receiving_date" />
                                        <asp:BoundField DataField="receiver_name_invoice" HeaderText="Receiver Name" SortExpression="receiver_name_invoice" />
                                        <asp:BoundField DataField="pod_no" HeaderText="POD Number" SortExpression="pod_no" />
                                        <asp:BoundField DataField="shipping_address" HeaderText="Shipping Address" SortExpression="shipping_address" />
                                        <asp:TemplateField HeaderText="Edit">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="link_invoice" Text="Edit" runat="server" CssClass="btn btn-primary" Style="color: white" OnClick="link_invoice_Click"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_invoice" runat="server" CausesValidation="false" Text="Download" Style="color: white" OnCommand="lnk_invoice_Command1" CommandArgument='<%# Eval ("invoice_stamp_copy") %> ' CssClass="btn btn-primary"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="invoice_stamp_copy" HeaderText="Invoice Stamp copy" SortExpression="invoice_stamp_copy" />
                                        <asp:BoundField DataField="receiver_type" HeaderText="Receiver Type" SortExpression="receiver_type" />

                                        <asp:BoundField DataField="month" HeaderText="Month_Year " SortExpression="month" />
                                        <asp:BoundField DataField="invoice_type" HeaderText="Billing Type" SortExpression="invoice_type" />

                                    </Columns>
                                </asp:GridView>

                            </asp:Panel>

                        </div>




                        <br />
                        <asp:Panel ID="Panel3" runat="server" CssClass="grid-view">
                            <asp:GridView ID="gv_bill_material" class="table" runat="server" Font-Size="X-Small"
                                AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" Visible="false"
                                BorderStyle="None" BorderWidth="1px" CellPadding="3" DataKeyNames="id" OnRowDataBound="gv_bill_material_RowDataBound" OnPreRender="gv_bill_material_PreRender" Width="100%">
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
                                    <asp:TemplateField HeaderText="No.">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_srnumber" runat="server"
                                                Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" />
                                    <asp:BoundField DataField="client_code" HeaderText="Client Name" SortExpression="client_code" />
                                    <asp:BoundField DataField="state_name" HeaderText="State Name" SortExpression="state_name" />
                                    <asp:BoundField DataField="unit_code" HeaderText="Branch Name" SortExpression="unit_code" />
                                    <asp:BoundField DataField="Month" HeaderText="Month" SortExpression="Month" />
                                    <asp:BoundField DataField="year" HeaderText="Year" SortExpression="year" />
                                    <asp:BoundField DataField="bill_type" HeaderText="Bill Type" SortExpression="bill_type" />
                                    <asp:BoundField DataField="invoice_number" HeaderText="Invoice Number" SortExpression="invoice_number" />
                                    <asp:BoundField DataField="dispatch_date" HeaderText="Dispatch Date" SortExpression="dispatch_date" />
                                    <asp:BoundField DataField="receiving_date" HeaderText="Receiving Date" SortExpression="receiving_date" />
                                    <asp:BoundField DataField="pod_number" HeaderText="POD Number" SortExpression="pod_number" />
                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkedit" Text="Edit" runat="server" OnClick="lnkedit_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Download">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("Value") %>' runat="server" OnClick="DownloadFile"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                        </asp:Panel>

                    </div>
                    <div id="tabs-3">
                        <br />
                        <div class="container-fluid" style="background: white; border-radius: 10px; border: 1px solid white">
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                    <b>Employee Name :</b>
                                    <asp:TextBox ID="lbl_employee" runat="server" MaxLength="10" class="form-control emp_name"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Document Type :</b>
                                    <asp:TextBox ID="lbl_document" runat="server" MaxLength="10" class="form-control doc_type"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Dispatch Date :</b>
                                    <asp:Label ID="Label10" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                    <asp:TextBox ID="txt_inv_dispath_date" runat="server" MaxLength="10" class="form-control date-pickers"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Receiving Date :</b>
                                    <asp:TextBox ID="txt_inv_receiving_date" runat="server" MaxLength="10" class="form-control date-pickers"></asp:TextBox>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                    <b>POD Number :</b><asp:Label ID="Label11" runat="server" ForeColor="Red" Text="*"></asp:Label>
                                    <asp:TextBox ID="txt_inv_pod_no" runat="server" class="form-control " onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Shipping Address :</b>
                                    <asp:TextBox ID="txt_inv_shipping_add" runat="server" class="form-control ship_address" MaxLength="50" TextMode="MultiLine" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                    <b>Report Receiving :</b>
                                    <asp:FileUpload runat="server" ID="fup_inv_upload" meta:resourcekey="photo_uploadResource1" onchange="ValidateSingleInput(this);" />
                                    <b>JPEG JPG GIF PDF</b>
                                </div>
                            </div>
                            <br />
                            <div class="row text-center">
                                <asp:Button ID="btn_inv_save" runat="server" CssClass="btn btn-primary"
                                    Text="Save" OnClientClick="return bill_dispatch_validation();" OnClick="btn_inv_save_Click" />
                                <asp:Button ID="Button2" runat="server" CssClass="btn btn-danger"
                                    Text="Close" />
                            </div>
                            <br />
                        </div>
                        <br />
                        <asp:Panel ID="Panel4" runat="server" CssClass="grid-view">
                            <asp:GridView ID="grd_inv_material" class="table" runat="server" Font-Size="X-Small"
                                AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC"
                                BorderStyle="None" BorderWidth="1px" CellPadding="3" DataKeyNames="id" OnRowDataBound="grd_inv_material_RowDataBound" OnPreRender="grd_inv_material_PreRender" Width="100%">
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
                                    <asp:TemplateField HeaderText="No.">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_srnumber" runat="server"
                                                Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ID" HeaderText="ID" />
                                    <asp:BoundField DataField="client_name" HeaderText="Client Name" SortExpression="client_name" />
                                    <asp:BoundField DataField="state_name" HeaderText="State Name" SortExpression="state_name" />
                                    <asp:BoundField DataField="unit_name" HeaderText="Branch Name" SortExpression="unit_name" />
                                    <asp:BoundField DataField="emp_name" HeaderText="EMP NAME" SortExpression="emp_name" />

                                    <asp:BoundField DataField="document_type" HeaderText="Document Type" SortExpression="document_type" />
                                    <asp:BoundField DataField="pod_number" HeaderText="POD Number" SortExpression="pod_number" />
                                    <asp:BoundField DataField="dispatch_date" HeaderText="Dispatch Date" SortExpression="dispatch_date" />
                                    <asp:BoundField DataField="receiving_date" HeaderText="Receiving Date" SortExpression="receiving_date" />

                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkinv_edit" Text="Edit" runat="server" OnClick="lnkinv_edit_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Download">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("filename") %>' runat="server" OnClick="DownloadFile"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                        </asp:Panel>

                    </div>
                    <div id="tabs-4">
                        <br />
                        <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col-sm-2 col-xs-12 text-left">
                                        <b>Client Name :</b>  <span style="color: red">*</span>
                                        <asp:DropDownList ID="ddl_sendmail_client" class="form-control pr_state js-example-basic-single" runat="server" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged2" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2 col-xs-12 text-left">
                                        <b>State Name :</b>
                                        <asp:DropDownList ID="ddl_state" runat="server" DataTextField="STATE" DataValueField="STATE" class="form-control text_box" OnSelectedIndexChanged="ddl_state_SelectedIndexChanged" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2 col-xs-12 text-left">
                                        <b>Branch Name : </b>
                                        <asp:DropDownList ID="ddl_sendmail_unit" class="form-control pr_state js-example-basic-single" runat="server" />
                                    </div>
                                    <div class="col-sm-3 col-xs-12">
                                        <br />
                                        <asp:Button ID="btn_send_email" runat="server" class="btn btn-primary" OnClick="btn_send_email_Click" Text="Send Email" OnClientClick="return Req_validation1();" Style="width: 40%" />
                                        <asp:Button ID="btn_emails_not_sent" runat="server" CssClass="btn btn-primary" Text="Report" OnClick="btn_emails_not_sent_Click" OnClientClick="return Req_validation2();" />
                                    </div>
                                </div>
                                <br />
                                <div class="container-fluid">
                                    <asp:Panel ID="Panel5" runat="server">
                                        <asp:GridView ID="gv_itemslist" class="table" runat="server" BackColor="White"
                                            BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                                            OnRowDataBound="gv_itemslist_RowDataBound" AutoGenerateColumns="False" Width="100%" OnPreRender="gv_itemslist_PreRender">
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
                                                        <asp:Label ID="lbl_srnumber" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="client_code" HeaderText="CLIENT" SortExpression="client_code" />
                                                <asp:BoundField DataField="state_name" HeaderText="STATE" SortExpression="ESIC_ADDRESS" />
                                                <asp:BoundField DataField="unit_name" HeaderText="BRANCH" SortExpression="unit_name" />
                                                <asp:BoundField DataField="document_type" HeaderText="Document Type" SortExpression="document_type" />
                                                <asp:BoundField DataField="emp_name" HeaderText="EMPLOYEE" SortExpression="emp_name" />
                                                <asp:BoundField DataField="email_sent" HeaderText="UNIFORM SHOES ID" SortExpression="email_sent" />
                                                <asp:BoundField DataField="joining_letter_sent_date" HeaderText="SENT DATE" SortExpression="joining_letter_sent_date" />

                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div id="tabs-5">
                        <br />
                        <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col-md-2 col-xs-12">
                                        <b>Category : </b>
                                        <asp:DropDownList ID="ddl_courier_category" runat="server" CssClass="form-control text_box" Width="100%">
                                            <asp:ListItem Value="0">Select</asp:ListItem>
                                            <asp:ListItem Value="1">Uniform</asp:ListItem>
                                            <asp:ListItem Value="2">Shoes</asp:ListItem>
                                            <asp:ListItem Value="3">Id Card</asp:ListItem>
                                            <asp:ListItem Value="4">Documents</asp:ListItem>

                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Date :</b>
                                        <asp:Label runat="server" ForeColor="Red" Text="*"></asp:Label>
                                        <asp:TextBox ID="courier_date" runat="server" MaxLength="10" class="form-control date-picker"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 col-xs-12 text-left">
                                        <b>Client Name : </b><span style="color: red">*</span>
                                        <asp:DropDownList ID="ddl_courier_client_name" class="form-control pr_state js-example-basic-single" runat="server" OnSelectedIndexChanged="ddl_courier_client_name_SelectedIndexChanged" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2 col-xs-12 text-left">
                                        <b>State Name :</b>
                                        <asp:DropDownList ID="ddl_courier_state_name" runat="server" DataTextField="STATE" DataValueField="STATE" class="form-control text_box" OnSelectedIndexChanged="ddl_courier_state_name_SelectedIndexChanged" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2 col-xs-12 text-left">
                                        <b>Branch Name :</b>
                                        <asp:DropDownList ID="ddl_courier_branch_name" class="form-control pr_state js-example-basic-single" runat="server" />
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Address:</b><asp:Label ID="Label17" runat="server"></asp:Label>
                                        <asp:TextBox ID="courier_address" runat="server" class="form-control text_box" MaxLength="50" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Weight:</b>
                                        <asp:TextBox ID="courier_weight" runat="server" class="form-control text_box" MaxLength="50" onkeypress="return isnumber(event)"></asp:TextBox>
                                    </div>
                                    <div class="col-md-2 col-xs-12">
                                        <b>Unit : </b>
                                        <asp:DropDownList ID="ddl_courier_unit" runat="server" CssClass="form-control text_box" Width="100%">
                                            <asp:ListItem Value="0">Kg</asp:ListItem>
                                            <asp:ListItem Value="1">Gram</asp:ListItem>


                                        </asp:DropDownList>
                                    </div>

                                    <div class="col-sm-2 col-xs-12">
                                        <b>Packet:</b><asp:Label ID="Label15" runat="server"></asp:Label>
                                        <asp:TextBox ID="courier_packet" runat="server" class="form-control text_box" MaxLength="50" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>POD No:</b><asp:Label ID="Label16" runat="server"></asp:Label>
                                        <asp:TextBox ID="courier_pod" runat="server" class="form-control text_box" MaxLength="50" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                    <div class="col-md-2 col-xs-12">
                                        <b>Deliver : </b>
                                        <asp:DropDownList ID="ddl_courier_deliver" runat="server" CssClass="form-control text_box" Width="100%">
                                            <asp:ListItem Value="0">No</asp:ListItem>
                                            <asp:ListItem Value="1">Yes</asp:ListItem>


                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <br />
                                <br />
                                <div class="row text-center">
                                    <asp:Button ID="btn_save_courier" runat="server" CssClass="btn btn-primary"
                                        OnClick="btn_save_courier_Click" Text="Save" OnClientClick="return Rq_validation();" />

                                    <asp:Button ID="btn_update_courier" runat="server" CssClass="btn btn-primary"
                                        OnClick="btn_update_courier_Click" Text="Update" OnClientClick="return Rq_validation();" />

                                    <asp:Button ID="btn_delete_courier" runat="server" CssClass="btn btn-primary"
                                        OnClick="btn_delete_courier_Click" Text="Delete" />


                                    <asp:Button ID="courier_tracking" runat="server" CssClass="btn btn-primary" Style="width: 20%"
                                        Text="Courier Tracking" />
                                </div>

                                <br />
                                <div class="container">

                                    <asp:GridView ID="grd_courier" class="table" runat="server" BackColor="White" OnRowDataBound="grd_courier_RowDataBound"
                                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" OnSelectedIndexChanged="grd_courier_SelectedIndexChanged"
                                        AutoGenerateColumns="False" Width="100%" OnPreRender="grd_courier_PreRender">
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
                                            <asp:BoundField DataField="id" HeaderText="id" SortExpression="id" />
                                            <asp:BoundField DataField="category" HeaderText="Category" SortExpression="category" />
                                            <asp:BoundField DataField="client_name" HeaderText="Client Name" SortExpression="client_name" />
                                            <asp:BoundField DataField="unit_name" HeaderText="Branch Name" SortExpression="unit_name" />
                                            <asp:BoundField DataField="state_courier" HeaderText="State" SortExpression="state_courier" />
                                            <asp:BoundField DataField="weight" HeaderText="Weight" SortExpression="weight" />
                                            <asp:BoundField DataField="units" HeaderText="Units" SortExpression="units" />
                                            <asp:BoundField DataField="packet" HeaderText="Packet" SortExpression="packet" />
                                            <asp:BoundField DataField="pod_no" HeaderText="Pod_NO" SortExpression="pod_no" />
                                            <asp:BoundField DataField="deliver" HeaderText="Deliver" SortExpression="deliver" />
                                            <asp:BoundField DataField="date" HeaderText="Date" SortExpression="date" />
                                            <asp:BoundField DataField="address" HeaderText="Address" SortExpression="address" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <br />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>


                    <div id="tabs-6">
                        <div class="container-fluid" style="background: white; border-radius: 10px; border: 1px solid white">
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                    <b>Employee Type : </b><span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_uniform_emp_type" runat="server" OnSelectedIndexChanged="ddl_uniform_emp_type_SelectedIndexChanged1"
                                        CssClass="form-control text_box"
                                        Width="100%">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="3">All</asp:ListItem>
                                        <asp:ListItem Value="1">Left</asp:ListItem>
                                        <asp:ListItem Value="2">Permanent</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Client Name :</b>  <span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_uniform_entry_client" class="form-control pr_state js-example-basic-single"
                                        runat="server" OnSelectedIndexChanged="ddl_uniform_entry_client_SelectedIndexChanged"
                                        AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>State Name :</b>  <span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_uniform_state_id" class="form-control pr_state js-example-basic-single"
                                        runat="server" OnSelectedIndexChanged="ddl_uniform_state_id_SelectedIndexChanged"
                                        AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Branch Name :</b>  <span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_uniform_branch" class="form-control pr_state js-example-basic-single"
                                        runat="server" OnSelectedIndexChanged="ddl_uniform_branch_SelectedIndexChanged"
                                        AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Employee Name :</b>  <span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_uniform_emp_name" class="form-control pr_state js-example-basic-single"
                                        runat="server" OnSelectedIndexChanged="ddl_uniform_emp_name_SelectedIndexChanged"
                                        AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-2 col-xs-12">
                                    <b>Dispatch By :</b><span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_dispatch_d1" runat="server" CssClass="form-control text_box"
                                        Width="100%">
                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                        <asp:ListItem Value="0">By Courier</asp:ListItem>
                                        <asp:ListItem Value="1">By Postal</asp:ListItem>
                                        <asp:ListItem Value="2">By Hand</asp:ListItem>

                                    </asp:DropDownList>
                                </div>
                                <br />
                                <br />
                            </div>
                            <br />
                            <div class="row">
                                <br />
                                <br />
                                <div class="col-sm-12 col-xs-12">
                                    <asp:Panel runat="server" ID="Panel10">
                                        <div class="col-sm-2 col-xs-12">
                                            <b>Dispatch Date :</b><span style="color: red">*</span>
                                            <asp:TextBox ID="Disp_uniform_date" runat="server" MaxLength="10" class="form-control date-pickerk"></asp:TextBox>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel runat="server" ID="Panel9">
                                        <div class="col-sm-2 col-xs-12">
                                            <b>Delivery Date :</b><span style="color: red">*</span>
                                            <asp:TextBox ID="deliver_uniform_dt" runat="server" MaxLength="10" class="form-control date-pickerk"></asp:TextBox>
                                        </div>
                                    </asp:Panel>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>POD No:</b><asp:Label ID="Label6" runat="server"></asp:Label>
                                        <asp:TextBox ID="pod_uniform_no" runat="server" class="form-control text_box" MaxLength="50"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>No of Set :</b><asp:Label ID="Label20" runat="server"></asp:Label>
                                        <asp:TextBox ID="set_uniform_no" runat="server" class="form-control text_box" MaxLength="1"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Remark :</b><asp:Label ID="Label21" runat="server"></asp:Label>
                                        <asp:TextBox ID="uniform_remark" runat="server" class="form-control text_box"   onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Attachment :</b><span class="text-red">*</span>
                                        <asp:FileUpload ID="FileUpload_uniform_entry" runat="server" meta:resourcekey="photo_uploadResource1"
                                            CssClass="text_box" />
                                    </div>

                                </div>
                            </div>
                            <br />
                            <div class="row text-center">
                                <asp:Button ID="uniform_submit_btn" runat="server" CssClass="btn btn-primary" Text="Submit"
                                    OnClick="uniform_submit_btn_Click" OnClientClick="return uniform_entery_val();" />&nbsp;&nbsp;

                               <asp:Button ID="btn_pod_detail" runat="server" CssClass="btn btn-primary" Width="14%"
                           Text="Client Details" OnClick="btn_pod_detail_Click" />&nbsp;&nbsp;<%--OnClientClick="return uniform_entery_val();"--%>


                            </div>
                            <br />
                            <br />
                            <%--Start gv_uniform_entry --%>
                            <div id="div_gv_uniform" class="container-fluid" runat="server" visible="false" cssclass="grid-view">
                                <asp:Panel ID="panel_gv" runat="server">
                                    <asp:GridView ID="gv_uniform_entry" class="table" runat="server" BackColor="White"
                                        ForeColor="#333333" CellPadding="1" Font-Size="X-Small"
                                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" DataKeyNames="id"
                                        AutoGenerateColumns="False" OnPreRender="gv_uniform_entry_PreRender"
                                        OnRowDataBound="gv_uniform_entry_RowDataBound">
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
                                            <asp:BoundField DataField="id" HeaderText="ID" SortExpression="id" />
                                            <asp:TemplateField HeaderText="Sr No">
                                                <ItemTemplate>
                                                    <asp:Label ID="uniform1_sr_number" runat="server" Text='<%# Container.DataItemIndex+1 %>'
                                                        Width="20px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="id" HeaderText="Uniform id" SortExpression="id" />
                                            <asp:BoundField DataField="client_name" HeaderText="Client Name" SortExpression="client_name" />
                                            <asp:BoundField DataField="state_dispatch" HeaderText="State Name" SortExpression="state_dispatch" />
                                            <asp:BoundField DataField="unit_code" HeaderText="Branch Name" SortExpression="unit_code" />
                                            <asp:BoundField DataField="employee_type" HeaderText="Employee Type" SortExpression="employee_type" />
                                            <asp:BoundField DataField="emp_code" HeaderText="Employee Name" SortExpression="emp_code" />
                                            <asp:BoundField DataField="dispatch_by" HeaderText="Dispatch By " SortExpression="dispatch_by" />
                                            <asp:BoundField DataField="dispatch_date" HeaderText="Dispatch Date " SortExpression="dispatch_date" />
                                            <asp:BoundField DataField="receiving_date" HeaderText="Delivery Date" SortExpression="receiving_date" />
                                            <asp:BoundField DataField="pod_no" HeaderText="Pod No" SortExpression="pod_no" />
                                            <asp:BoundField DataField="No_of_set" HeaderText="No Of Set" SortExpression="No_of_set" />
                                            <asp:BoundField DataField="Remark" HeaderText="Remark" SortExpression="Remark" />
                                            <asp:TemplateField HeaderText="Edit">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="uniform_entry_edit" Text="Edit" runat="server" CssClass="btn btn-primary"
                                                        Style="color: white" OnClick="uniform_entry_edit_Click"></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Attatchment">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="uniform_entry_download" runat="server" Width="100%" CausesValidation="false"
                                                        Text="Download" Style="color: white" OnCommand="uniform_entry_download_Command"
                                                        CommandArgument='<%# Eval("second_stamp_copy")%>'
                                                        CssClass="btn btn-primary"></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </div>
                            <%--END gv_uniform_entry --%>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>


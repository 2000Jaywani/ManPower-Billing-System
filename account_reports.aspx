<%@ Page Title="Account Reports" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="account_reports.aspx.cs" Inherits="account_reports" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
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
    <script src="datatable/jszip.min.js"></script>
    <script src="datatable/vfs_fonts.js"></script>
    <script src="datatable/buttons.html5.min.js"></script>
    <script src="datatable/buttons.print.min.js"></script>
    <script src="datatable/buttons.colVis.min.js"></script>

    <style>
        .auto-style1 {
            color: #FFFFFF;
        }

        .text-red {
            color: #f00;
        }

        h2 {
            border-radius: 5px;
        }

        .grid-view {
            height: auto;
            max-height: 300px;
            overflow-y: auto;
            overflow-x: hidden;
        }

        h5 {
            font-weight: bold;
            font-size: 15px;
        }


        .table {
            width: 50%;
            max-width: none;
        }
    </style>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cph_righrbody" runat="Server">
      <script type="text/javascript">
          function ValidateEmailID(txtEmailID) {
              var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
              if (txtEmailID.value == "") {
                  txtEmailID.style.border = "";
                  return true;
              }
              else if (filter.test(txtEmailID.value)) {
                  txtEmailID.style.border = "";
                  return true;
              }
              else {
                  txtEmailID.style.borderColor = "red";
                  return false;
              }
          }
    </script>
    <script type="text/javascript">
        function unblock() {
            $.unblockUI();
        }
        function pageLoad() {

            //$(function () {
            //    //event handler to the checkbox selection change event
            //    $("input[type=checkbox]").change(function () {
            //        //variables to store the total price of selected rows
            //        //and to hold the reference to the current checkbox control
            //        var totalPrice = 0, ctlPrice;
            //        //iterate through all the rows of the gridview
            //        $('#gv_invoice_list tr').each(function () {
            //            //if the checkbox in that rows is checked, add price to our total proce
            //            if ($(this).find('input:checkbox').attr("checked")) {
            //                ctlPrice = $(this).find('[id$= lblListPrice]');
            //                //since it is a currency column, we need to remove the $ sign and then convert it
            //                //to a number before adding it to the total
            //                totalPrice += parseFloat(ctlPrice.text().replace(/[^\d\.]/g, ''));
            //            }
            //        });
            //        //finally set the total price (rounded to 2 decimals) to the total paragraph control.
            //        $('#txt').text("$ " + totalPrice.toFixed(2));
            //    });
            //});



            $('[id*="ddl_tds_on"]').prop("disabled", true);
            $('[id*="txt_adjustment_amt"]').attr("readonly", true);
            $(document).on('change', '.droplist', function () {
                var parentRow = $(this).parents("tr:eq(0)");
                var text_flats_listmaxvacancy = $(parentRow).find('[id*="txt_tds_amt"]');
                var text_flats_listmaxvacanc = $(parentRow).find('[id*="ddl_tds_on"]');


                var _value = $(this).val();
                if (_value == "Amount") {

                    $(text_flats_listmaxvacancy).attr("readonly", false);
                    $(text_flats_listmaxvacanc).prop("disabled", true);


                }
                else {
                    $(text_flats_listmaxvacancy).attr("readonly", true);
                    $(text_flats_listmaxvacanc).prop("disabled", false);
                    $(text_flats_listmaxvacancy).val('0');
                }
            });
            $(document).on('change', '.droplist1', function () {
                var parentRow = $(this).parents("tr:eq(0)");
                var text_flats_listmaxvacancy1 = $(parentRow).find('[id*="txt_adjustment_amt"]');


                var _value = $(this).val();
                if (_value == "1" || _value == "2") {

                    $(text_flats_listmaxvacancy1).attr("readonly", false);

                }
                else {
                    $(text_flats_listmaxvacancy1).attr("readonly", true);
                    $(text_flats_listmaxvacancy1).val('0');


                }
            });
            adjustment();
            disable();
            $(".date-picker3").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                yearRange: '1951',
                onSelect: function (selected) {
                    $(".date-picker4").datepicker("option", "minDate", selected)
                }
            });

            $(".date-picker4").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                minDate: 0,
                onSelect: function (selected) {
                    $(".date-picker3").datepicker("option", "maxDate", selected)
                }
            });
            $(".date-picker3").attr("readonly", "true");
            $(".date-picker4").attr("readonly", "true");
            $('.date-picker1').datepicker({
                changeMonth: true,
                changeYear: true,
                yearRange: "1950",
                maxDate: 0,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',

            });
            $(".date-picker1").attr("readonly", "true");


            $('.date-picker2').datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-date .ui-datepicker-month .ui-datepicker-year :selected").val();

                }
            });
            $(".date-picker2").focus(function () {
                $(".ui-datepicker-calendar").hide();
            })

            $('.date-picker').datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                yearRange: '1950',
                maxDate: 0,
                dateFormat: 'mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                }
            });
            $(".date-picker").focus(function () {
                $(".ui-datepicker-calendar").hide();
            })

            $(".date-picker").attr("readonly", "true");
            $(".date-picker2").attr("readonly", "true");

           

            $('#<%=btn_save.ClientID%>').click(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_client.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_pmt_paid.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_pmt_client.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_batch_no.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });

            $.fn.dataTable.ext.errMode = 'none';
            var table = $('#<%=gv_debit_pmt_details.ClientID%>').DataTable(
                     {
                         scrollY: "210px", buttons: [
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
                         fixedHeader: {
                             header: true,
                             footer: true
                         }

                     });

            table.buttons().container()
               .appendTo('#<%=gv_debit_pmt_details.ClientID%>_wrapper .col-sm-6:eq(0)');

           
            var table = $('#<%=gv_payment.ClientID%>').DataTable({
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
               .appendTo('#<%=gv_payment.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';

            var table = $('#<%=grv_saleentery.ClientID%>').DataTable({
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
               .appendTo('#<%=grv_saleentery.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';

            var table = $('#<%=gv_payment_detail.ClientID%>').DataTable({
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
               .appendTo('#<%=gv_payment_detail.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';

            var table = $('#<%=gv_minibank.ClientID%>').DataTable({
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
               .appendTo('#<%=gv_minibank.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';


            var table = $('#<%=gv_outstanding.ClientID%>').DataTable({
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
               .appendTo('#<%=gv_outstanding.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';



        }

        function R_validation_delete() {
            var r = confirm("Are you Sure You Want to Delete Record");
            if (r == true) {
                ($.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } }))
                return true;
            }
            else {
                return false;
            }
        }



        function show() {

            alert("hello");
        }

        function openWindow() {

            window.open("html/Account_report.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');

        }

        function isNumber(evt) {
            if (null != evt) {
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode == 34 || charCode == 39) {
                    return false;
                }
                return true;
            }
        }
        function isNumbernum(evt) {
            if (null != evt) {
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode == 34 || charCode == 39) {
                    return false;
                }
                return true;
            }
        }


        function validation() {

            var txt_date = document.getElementById('<%=txt_date.ClientID%>');
            if (txt_date.value == "") {
                alert("Please Select Month");
                txt_date.focus();
                return false;
            }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;
        }
        function isNumber(evt) {
            if (null != evt) {
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                    return false;
                }
            }
            return true;
        }
        function isNumberamt(evt) {
            if (null != evt) {
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && (charCode < 46 || charCode > 57) || charcode== 47) {
                    return false;
                }
            }
            return true;
        }
        function isNumber1(evt) {
            if (null != evt) {
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if ((charCode == 34 || charCode == 39 || charCode == 47 || charCode == 92)) {
                    return false;
                }
            }
            return true;
        }
        function allowAlphaNumericSpace(e) {
            var code = ('charCode' in e) ? e.charCode : e.keyCode;
            if (!(code == 32) && // space
              !(code > 47 && code < 58) && // numeric (0-9)
              !(code > 64 && code < 91) && // upper alpha (A-Z)
              !(code > 96 && code < 123)) { // lower alpha (a-z)
                e.preventDefault();
            }
        }
        function disable() {

            var ddl_pmt_recived = document.getElementById('<%=ddl_pmt_recived.ClientID %>');
            var Selected_ddl_pmt_recived = ddl_pmt_recived.options[ddl_pmt_recived.selectedIndex].text;

        }
        function jsFunction() {

        }

        function adjustment() {


        }
        function Req_validation() {

            var client_name = ddl_upload_lg_client.options[ddl_upload_lg_client.selectedIndex].text;

            var FileUpload1 = document.getElementById('<%=FileUpload1.ClientID %>');

            if (client_name == "Select") {
                alert("Please Select Client Name");
                ddl_upload_lg_client.focus();
                return false;
            }

            if (FileUpload1.value == "") {
                alert("Please Upload the File ");
                FileUpload1.focus();
                return false;
            }
            // $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;

        }

        //Start sachin send mail validations

        function send_mail_validation() {

            var ddl_client = document.getElementById('<%=ddl_client_outstanding.ClientID %>');
            var Selected_ddl_client = ddl_client.options[ddl_client.selectedIndex].text;
            
            var to_mail = document.getElementById('<%=txt_to_mail.ClientID %>');

            var cc_mail = document.getElementById('<%=txt_cc_mail.ClientID %>');

            if (Selected_ddl_client == "Select") {
                alert("Please Select Client Name");
                ddl_client.focus();
                return false;
            }

            if (to_mail.value == "") {
                alert("Please Enter To Mail");
                to_mail.focus();
                return false;

            }

            if (cc_mail.value == "") {
                alert("Please Enter CC Mail");
                cc_mail.focus();
                return false;

            }
        }

        function view_outstanding_validation() {

            var ddl_client = document.getElementById('<%=ddl_client_outstanding.ClientID %>');
            var Selected_ddl_client = ddl_client.options[ddl_client.selectedIndex].text;

            if (Selected_ddl_client == "Select") {
                alert("Please Select Client Name");
                ddl_client.focus();
                return false;
            }
        }
        
        function ValidateEmail() {
            var email = document.getElementById("txt_to_mail").value;
            var lblError = document.getElementById("lblError");
            lblError.innerHTML = "";
            var expr = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
            if (!expr.test(email)) {
                lblError.innerHTML = "Invalid email address.";
            }
        }
            //END sachin send mail validations

            function Req_report_validation() {

            var ddl_upload_lg_client = document.getElementById('<%=ddl_upload_lg_client.ClientID %>');
            var client_name = ddl_upload_lg_client.options[ddl_upload_lg_client.selectedIndex].text;

            if (client_name == "Select") {
                alert("Please Select Client Name");
                ddl_upload_lg_client.focus();
                return false;
            }

        }
        function Search_Gridview(strKey) {
            var strData = strKey.value.toLowerCase().split(" ");
            var tblData = document.getElementById("<%=payment_gridview.ClientID %>");
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

        function Required_mini_validation() {
            var ddl_pmt_recived = document.getElementById('<%=ddl_pmt_recived.ClientID %>');
            var Selected_ddl_pmt_recived = ddl_pmt_recived.options[ddl_pmt_recived.selectedIndex].text;
            if (Selected_ddl_pmt_recived == "Select") {

                alert("Please Select Receiver type ");
                ddl_pmt_recived.focus();
                return false;
            }

            var ddl_pmt_recived = document.getElementById('<%=ddl_pmt_recived.ClientID %>');
            var Selected_ddl_pmt_recived = ddl_pmt_recived.options[ddl_pmt_recived.selectedIndex].text;
            if (Selected_ddl_pmt_recived == "Other") {

                var ddl_other = document.getElementById('<%=ddl_other.ClientID %>');
                var company_bank = ddl_other.options[ddl_other.selectedIndex].text;


                if (company_bank == "Select") {

                    alert("Please Select Other ");
                    ddl_other.focus();
                    return false;

                }

                var ddl_comp_ac_number = document.getElementById('<%=lit_comp_ac_number.ClientID %>');

                if (ddl_comp_ac_number.value == "") {
                    alert("Company Bank A/C  Number Not Added");
                    ddl_comp_ac_number.focus();
                    return false;
                }


                var txt_description = document.getElementById('<%=txt_description.ClientID %>');

                if (txt_description.value == "") {
                    alert("Please Enter Description ");
                    txt_description.focus();
                    return false;
                }


                var minibank_amount = document.getElementById('<%=txt_minibank_amount.ClientID %>');
                if (minibank_amount.value == "0" || minibank_amount.value == "") {
                    alert("Please Enter Receiving Amount");
                    minibank_amount.focus();
                    return false;

                }
                var txt_minibank_received_date = document.getElementById('<%=txt_minibank_received_date.ClientID%>');
                if (txt_minibank_received_date.value == "") {
                    alert("Please Select Reciving Date");
                    txt_minibank_received_date.focus();
                    return false;
                }

                var ddl_mode_transfer = document.getElementById('<%=ddl_mode_transfer.ClientID %>');
                var Selected_ddl_mode_transfer = ddl_mode_transfer.options[ddl_mode_transfer.selectedIndex].text;

                if (Selected_ddl_mode_transfer == "Select") {
                    alert("Please Select Mode Of Transfer");
                    ddl_mode_transfer.focus();
                    return false;
                }
                if (Selected_ddl_mode_transfer == "RTGS" || Selected_ddl_mode_transfer == "NEFT") {
                    var minibank_utr_no = document.getElementById('<%=txt_minibank_utr_no.ClientID %>');
                    if (minibank_utr_no.value == "") {
                        alert("Please Enter UTR NO.");
                        minibank_utr_no.focus();
                        return false;
                    }
                }
                if (Selected_ddl_mode_transfer == "Cheque") {
                    var txt_cheque = document.getElementById('<%=txt_cheque.ClientID %>');
                    if (txt_cheque.value == "") {
                        alert("Please Enter Cheque NO.");
                        txt_cheque.focus();
                        return false;
                    }
                }
                var photo_upload = document.getElementById('<%=photo_upload.ClientID %>');
                if (photo_upload.value == "") {
                    alert("Please Upload File");
                    photo_upload.focus();
                    return false;
                }


            }
            var ddl_pmt_recived = document.getElementById('<%=ddl_pmt_recived.ClientID %>');
            var Selected_ddl_pmt_recived = ddl_pmt_recived.options[ddl_pmt_recived.selectedIndex].text;
            if (Selected_ddl_pmt_recived == "Client") {

                var ddl_minibank_client = document.getElementById('<%=ddl_minibank_client.ClientID %>');
                var Select_ddl_minibank_client = ddl_minibank_client.options[ddl_minibank_client.selectedIndex].text;


                if (Select_ddl_minibank_client == "Select") {
                    alert("Please Select Client");
                    ddl_minibank_client.focus();
                    return false;

                }


                var ddl_bank_name = document.getElementById('<%=lit_bank_name.ClientID %>');

                if (ddl_bank_name.value == "") {
                    alert("Company Bank A/C Not Added");
                    ddl_bank_name.focus();
                    return false;
                }

                var ddl_comp_ac_number = document.getElementById('<%=lit_comp_ac_number.ClientID %>');

                if (ddl_comp_ac_number.value == "") {
                    alert("Company Bank A/C  Number Not Added");
                    ddl_comp_ac_number.focus();
                    return false;
                }

                var ddl_payment_type = document.getElementById('<%=ddl_payment_type.ClientID %>');
            var Select_ddl_payment_type = ddl_payment_type.options[ddl_payment_type.selectedIndex].text;


            if (Select_ddl_payment_type == "Select") {

                alert("Please Select Payment Against  ");
                ddl_payment_type.focus();
                return false;

            }



                var ddl_client_bank = document.getElementById('<%=lit_client_bank.ClientID %>');
                var client_bank = ddl_client_bank.options[ddl_client_bank.selectedIndex].text;


            if (client_bank == "Select" || client_bank == "") {

                alert("Please Select Client Bank ");
                ddl_client_bank.focus();
                return false;

            }


            var minibank_amount1 = document.getElementById('<%=txt_minibank_amount.ClientID %>');
                if (minibank_amount1.value == "0" || minibank_amount1.value == "") {
                    alert("Please Enter Receiving Amount");
                    minibank_amount1.focus();
                    return false;

                }



                var txt_minibank_received_date1 = document.getElementById('<%=txt_minibank_received_date.ClientID%>');
            if (txt_minibank_received_date1.value == "") {
                alert("Please Select Reciving Date");
                txt_minibank_received_date1.focus();
                return false;
            }

            var ddl_mode_transfer = document.getElementById('<%=ddl_mode_transfer.ClientID %>');
            var Selected_ddl_mode_transfer = ddl_mode_transfer.options[ddl_mode_transfer.selectedIndex].text;

            if (Selected_ddl_mode_transfer == "Select") {
                alert("Please Select Mode Of Transfer");
                ddl_mode_transfer.focus();
                return false;
            }
            if (Selected_ddl_mode_transfer == "RTGS" || Selected_ddl_mode_transfer == "NEFT") {
                var minibank_utr_no1 = document.getElementById('<%=txt_minibank_utr_no.ClientID %>');
                if (minibank_utr_no1.value == "") {
                    alert("Please Enter UTR NO.");
                    minibank_utr_no1.focus();
                    return false;
                }
            }
            if (Selected_ddl_mode_transfer == "Cheque") {
                var txt_cheque = document.getElementById('<%=txt_cheque.ClientID %>');
                if (txt_cheque.value == "") {
                    alert("Please Enter Cheque NO.");
                    txt_cheque.focus();
                    return false;
                }
            }
            var photo_upload = document.getElementById('<%=photo_upload.ClientID %>');
                if (photo_upload.value == "") {
                    alert("Please Upload File");
                    photo_upload.focus();
                    return false;
                }

            }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;
        }

        function R_validation() {


            var ddl_pmt_recived = document.getElementById('<%=ddl_pmt_recived.ClientID %>');
            var Selected_ddl_pmt_recived = ddl_pmt_recived.options[ddl_pmt_recived.selectedIndex].text;
            if (Selected_ddl_pmt_recived == "Select") {

                alert("Please Select Receiver type ");
                ddl_pmt_recived.focus();
                return false;
            }

            var ddl_pmt_recived = document.getElementById('<%=ddl_pmt_recived.ClientID %>');
            var Selected_ddl_pmt_recived = ddl_pmt_recived.options[ddl_pmt_recived.selectedIndex].text;
            if (Selected_ddl_pmt_recived == "Other") {

                var ddl_other = document.getElementById('<%=ddl_other.ClientID %>');
                var company_bank = ddl_other.options[ddl_other.selectedIndex].text;


                if (company_bank == "Select") {

                    alert("Please Select Other ");
                    ddl_other.focus();
                    return false;

                }

                var ddl_comp_ac_number = document.getElementById('<%=lit_comp_ac_number.ClientID %>');

                if (ddl_comp_ac_number.value == "") {
                    alert("Company Bank A/C  Number Not Added");
                    ddl_comp_ac_number.focus();
                    return false;
                }


                var txt_description = document.getElementById('<%=txt_description.ClientID %>');

                if (txt_description.value == "") {
                    alert("Please Enter Description ");
                    txt_description.focus();
                    return false;
                }


                var minibank_amount = document.getElementById('<%=txt_minibank_amount.ClientID %>');
                if (minibank_amount.value == "0" || minibank_amount.value == "") {
                    alert("Please Enter Receiving Amount");
                    minibank_amount.focus();
                    return false;

                }



                var txt_minibank_received_date = document.getElementById('<%=txt_minibank_received_date.ClientID%>');
                if (txt_minibank_received_date.value == "") {
                    alert("Please Select Reciving Date");
                    txt_minibank_received_date.focus();
                    return false;
                }

                var ddl_mode_transfer = document.getElementById('<%=ddl_mode_transfer.ClientID %>');
                var Selected_ddl_mode_transfer = ddl_mode_transfer.options[ddl_mode_transfer.selectedIndex].text;

                if (Selected_ddl_mode_transfer == "Select") {
                    alert("Please Select Mode Of Transfer");
                    ddl_mode_transfer.focus();
                    return false;
                }
                if (Selected_ddl_mode_transfer == "RTGS" || Selected_ddl_mode_transfer == "NEFT") {
                    var minibank_utr_no = document.getElementById('<%=txt_minibank_utr_no.ClientID %>');
                if (minibank_utr_no.value == "") {
                    alert("Please Enter UTR NO.");
                    minibank_utr_no.focus();
                    return false;
                }
            }
            if (Selected_ddl_mode_transfer == "Cheque") {
                var txt_cheque = document.getElementById('<%=txt_cheque.ClientID %>');
                if (txt_cheque.value == "") {
                    alert("Please Enter Cheque NO.");
                    txt_cheque.focus();
                    return false;
                }
            }



        }
        var ddl_pmt_recived = document.getElementById('<%=ddl_pmt_recived.ClientID %>');
            var Selected_ddl_pmt_recived = ddl_pmt_recived.options[ddl_pmt_recived.selectedIndex].text;
            if (Selected_ddl_pmt_recived == "Client") {

                var ddl_minibank_client = document.getElementById('<%=ddl_minibank_client.ClientID %>');
                var Select_ddl_minibank_client = ddl_minibank_client.options[ddl_minibank_client.selectedIndex].text;


                if (Select_ddl_minibank_client == "Select") {
                    alert("Please Select Client");
                    ddl_minibank_client.focus();
                    return false;

                }


                var ddl_bank_name = document.getElementById('<%=lit_bank_name.ClientID %>');

                if (ddl_bank_name.value == "") {
                    alert("Company Bank A/C Not Added");
                    ddl_bank_name.focus();
                    return false;
                }

                var ddl_comp_ac_number = document.getElementById('<%=lit_comp_ac_number.ClientID %>');

                if (ddl_comp_ac_number.value == "") {
                    alert("Company Bank A/C  Number Not Added");
                    ddl_comp_ac_number.focus();
                    return false;
                }

                var ddl_payment_type = document.getElementById('<%=ddl_payment_type.ClientID %>');
                var Select_ddl_payment_type = ddl_payment_type.options[ddl_payment_type.selectedIndex].text;


                if (Select_ddl_payment_type == "Select") {

                    alert("Please Select Payment Against  ");
                    ddl_payment_type.focus();
                    return false;

                }



                var ddl_client_bank = document.getElementById('<%=lit_client_bank.ClientID %>');
                var client_bank = ddl_client_bank.options[ddl_client_bank.selectedIndex].text;


            if (client_bank == "Select" || client_bank == "") {

                alert("Please Select Client Bank ");
                ddl_client_bank.focus();
                return false;

            }


            var minibank_amount1 = document.getElementById('<%=txt_minibank_amount.ClientID %>');
            if (minibank_amount1.value == "0" || minibank_amount1.value == "") {
                alert("Please Enter Receiving Amount");
                minibank_amount1.focus();
                return false;

            }



            var txt_minibank_received_date1 = document.getElementById('<%=txt_minibank_received_date.ClientID%>');
                if (txt_minibank_received_date1.value == "") {
                    alert("Please Select Reciving Date");
                    txt_minibank_received_date1.focus();
                    return false;
                }

                var ddl_mode_transfer = document.getElementById('<%=ddl_mode_transfer.ClientID %>');
            var Selected_ddl_mode_transfer = ddl_mode_transfer.options[ddl_mode_transfer.selectedIndex].text;

            if (Selected_ddl_mode_transfer == "Select") {
                alert("Please Select Mode Of Transfer");
                ddl_mode_transfer.focus();
                return false;
            }
            if (Selected_ddl_mode_transfer == "RTGS" || Selected_ddl_mode_transfer == "NEFT") {
                var minibank_utr_no1 = document.getElementById('<%=txt_minibank_utr_no.ClientID %>');
                if (minibank_utr_no1.value == "") {
                    alert("Please Enter UTR NO.");
                    minibank_utr_no1.focus();
                    return false;
                }
            }
            if (Selected_ddl_mode_transfer == "Cheque") {
                var txt_cheque = document.getElementById('<%=txt_cheque.ClientID %>');
                if (txt_cheque.value == "") {
                    alert("Please Enter Cheque NO.");
                    txt_cheque.focus();
                    return false;
                }
            }


        }

        var r = confirm("Are you Sure You Want to Approve");
        if (r == true) {
            ($.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } }))
            return true;
        }
        else {
            return false;
        }
    }

    function Required_pmt_validation() {

        var ddl_pmt_client = document.getElementById('<%=ddl_pmt_client.ClientID %>');
        var client_name = ddl_pmt_client.options[ddl_pmt_client.selectedIndex].text;

        if (client_name == "Select") {
            alert("Please Select Client Name");
            ddl_pmt_client.focus();
            return false;
        }

        var txt_comp_bank_name = document.getElementById('<%=txt_comp_bank_name.ClientID %>');
        // var company_bank = ddl_company_bank.options[ddl_company_bank.selectedIndex].text;

            if (txt_comp_bank_name.value == "") {
                alert("Please Select Comapny Bank Name");
                txt_comp_bank_name.focus();
                return false;
            }

            var ddl_pmt_desc = document.getElementById('<%=ddl_pmt_desc.ClientID %>');
            var pmt_desc = ddl_pmt_desc.options[ddl_pmt_desc.selectedIndex].text;


            if (pmt_desc == "Select") {

                alert("Please Select Payment Description ");
                ddl_pmt_desc.focus();
                return false;

            }


            var pmt_amount = document.getElementById('<%=txt_pmt_amount.ClientID %>');
            if (pmt_amount.value == "0" || pmt_amount.value == "") {
                alert("Please Enter Payment Amount");
                pmt_amount.focus();
                return false;

            }



            var txt_pmt_date = document.getElementById('<%=txt_pmt_date.ClientID%>');
            if (txt_pmt_date.value == "") {
                alert("Please Select Reciving Date");
                txt_pmt_date.focus();
                return false;
            }
            if (valid_upload()) {
                return true;
            }
            else {
                return false;
            }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

        }

        function isNumberKey(evt, id) {
            try {
                var charCode = (evt.which) ? evt.which : event.keyCode;

                if (charCode == 46) {
                    var txt = document.getElementById(id).value;
                    if (!(txt.indexOf(".") > -1)) {

                        return true;
                    }
                }
                if (charCode > 31 && (charCode < 48 || charCode > 57))
                    return false;

                return true;
            } catch (w) {
                alert(w);
            }
        }


        function valid_upload() {
            var document1_file = document.getElementById('<%=document1_file.ClientID %>');
            if (document1_file.value == "") {
                alert("Please Upload Annexure");
                document1_file.focus();
                return false;
            }

            return true;
        }
        function req_val() {
            var ddl_upload_lg_client = document.getElementById('<%=ddl_upload_lg_client.ClientID %>');
            var client_name = ddl_upload_lg_client.options[ddl_upload_lg_client.selectedIndex].text;

            if (client_name == "Select") {
                alert("Please Select Client Name");
                ddl_upload_lg_client.focus();
                return false;
            }
        }
        function Validate() {


            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            //return true;
        }
        $(document).ready(function () {
            var st = $(this).find("input[id*='hidtab']").val();
            if (st == null)
                st = 0;
            $('[id$=tabs]').tabs({ selected: st });
        });
        function utr_no() {

            var txt_utr_no = document.getElementById('<%=txt_utr_no.ClientID %>').value;
            if (txt_utr_no.length != 12) {
                alert("Invalid UTR Number ! Please Enter 12 Digit UTR Number");
                txt_utr_no.focus();
                return false;
            }
        }
        function cheque_no() {
            var txt_cheque_no = document.getElementById('<%=txt_cheque_no.ClientID %>').value;
            if (txt_cheque_no.length != 6) {
                alert("Invalid Cheque Number ! Please Enter 6 Digit Cheque Number");
                txt_cheque_no.focus();
                return false;
            }
        }
        function Required_pmt_validation() {
            var ddl_pmt_paid = document.getElementById('<%=ddl_pmt_paid.ClientID %>');
            var Selected_ddl_pmt_paid = ddl_pmt_paid.options[ddl_pmt_paid.selectedIndex].text;
            if (Selected_ddl_pmt_paid == "Employee Payment") {
                if (!employee_payment()) { return false; }
            }
            else if (Selected_ddl_pmt_paid == "Vendor Payment") {
                if (!vendor_payment()) { return false; }
            }
            else if (Selected_ddl_pmt_paid == "Internal Transfer") {
                if (!internal_transfer()) { return false; }
            }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;
        }
        function employee_payment() {
            var ddl_pmt_paid = document.getElementById('<%=ddl_pmt_paid.ClientID %>');
            var Selected_ddl_pmt_paid = ddl_pmt_paid.options[ddl_pmt_paid.selectedIndex].text;

            var ddl_pmt_client = document.getElementById('<%=ddl_pmt_client.ClientID %>');
            var Selected_ddl_pmt_client = ddl_pmt_client.options[ddl_pmt_client.selectedIndex].text;

            var ddl_batch_no = document.getElementById('<%=ddl_batch_no.ClientID %>');
            var Selected_ddl_batch_no = ddl_batch_no.options[ddl_batch_no.selectedIndex].text;

            var ddl_pmt_desc = document.getElementById('<%=ddl_pmt_desc.ClientID %>');
            var Selected_ddl_pmt_desc = ddl_pmt_desc.options[ddl_pmt_desc.selectedIndex].text;

            var txt_pmt_date = document.getElementById('<%=txt_pmt_date.ClientID %>');

            var ddl_pmt_mode = document.getElementById('<%=ddl_pmt_mode.ClientID %>');
            var Selected_ddl_pmt_mode = ddl_pmt_mode.options[ddl_pmt_mode.selectedIndex].text;

            if (Selected_ddl_pmt_paid == "Employee Payment") {
                if (Selected_ddl_pmt_client == "Select") {
                    alert("Please Select Client Name");
                    ddl_pmt_client.focus();
                    return false;
                }
                if (Selected_ddl_batch_no == "Select") {
                    alert("Please Select Batch Number");
                    ddl_batch_no.focus();
                    return false;
                }
                if (Selected_ddl_pmt_desc == "Select") {
                    alert("Please Select Description");
                    ddl_pmt_desc.focus();
                    return false;
                }
                if (txt_pmt_date.value == "") {
                    alert("Please Select Payment Date");
                    txt_pmt_date.focus();
                    return false;
                }
                if (Selected_ddl_pmt_mode == "Select") {
                    alert("Please Select Mode Of Transfer");
                    ddl_pmt_mode.focus();
                    return false;
                }
                var ddl_pmt_mode = document.getElementById('<%=ddl_pmt_mode.ClientID %>');
                var Selected_ddl_pmt_mode = ddl_pmt_mode.options[ddl_pmt_mode.selectedIndex].text;
                var txt_utr_no = document.getElementById('<%=txt_utr_no.ClientID %>');
                var txt_cheque_no = document.getElementById('<%=txt_cheque_no.ClientID %>');
                var txt_cheque_receive_date = document.getElementById('<%=txt_cheque_receive_date.ClientID %>');
                var txt_cheque_deposite_date = document.getElementById('<%=txt_cheque_deposite_date.ClientID %>');

                if (Selected_ddl_pmt_mode == "RTGS" || Selected_ddl_pmt_mode == "NEFT") {
                    if (txt_utr_no.value == "") {
                        alert("Please Enter UTR Number");
                        txt_utr_no.focus();
                        return false;
                    }
                }
                else if (Selected_ddl_pmt_mode == "Cheque") {
                    if (txt_cheque_no.value == "") {
                        alert("Please Enter Cheque Number");
                        txt_cheque_no.focus();
                        return false;
                    }
                    if (txt_cheque_receive_date.value == "") {
                        alert("Please Select Cheque Received Date");
                        txt_cheque_receive_date.focus();
                        return false;
                    }
                    if (txt_cheque_deposite_date.value == "") {
                        alert("Please Select Cheque Deposite Date");
                        txt_cheque_deposite_date.focus();
                        return false;
                    }
                }
                var document1_file = document.getElementById('<%=document1_file.ClientID %>');
                if (document1_file.value == "") {
                    alert("Please Upload Document");
                    document1_file.focus();
                    return false;
                }
            }
            return true;
        }
        function vendor_payment() {
            var ddl_pmt_paid = document.getElementById('<%=ddl_pmt_paid.ClientID %>');
            var Selected_ddl_pmt_paid = ddl_pmt_paid.options[ddl_pmt_paid.selectedIndex].text;

            var ddl_pmt_client = document.getElementById('<%=ddl_pmt_client.ClientID %>');
            var Selected_ddl_pmt_client = ddl_pmt_client.options[ddl_pmt_client.selectedIndex].text;

            var ddl_batch_no = document.getElementById('<%=ddl_batch_no.ClientID %>');
            var Selected_ddl_batch_no = ddl_batch_no.options[ddl_batch_no.selectedIndex].text;

            var ddl_company_bank = document.getElementById('<%=ddl_company_bank.ClientID %>');
            var Selected_ddl_company_bank = ddl_company_bank.options[ddl_company_bank.selectedIndex].text;

            var txt_pmt_date = document.getElementById('<%=txt_pmt_date.ClientID %>');
            var txt_pmt_amount = document.getElementById('<%=txt_pmt_amount.ClientID %>');
            var txt_pmt_desc = document.getElementById('<%=txt_pmt_desc.ClientID %>');
            var txt_pmt_amount = document.getElementById('<%=txt_pmt_amount.ClientID %>');

            var ddl_pmt_mode = document.getElementById('<%=ddl_pmt_mode.ClientID %>');
            var Selected_ddl_pmt_mode = ddl_pmt_mode.options[ddl_pmt_mode.selectedIndex].text;

            if (Selected_ddl_pmt_paid == "Vendor Payment") {
                if (Selected_ddl_pmt_client == "Select") {
                    alert("Please Select Vendor Name");
                    ddl_pmt_client.focus();
                    return false;
                }
                if (Selected_ddl_batch_no == "Select") {
                    alert("Please Select Batch Number");
                    ddl_batch_no.focus();
                    return false;
                }
                if (Selected_ddl_company_bank == "Select") {
                    alert("Please Select Company Bank Name");
                    ddl_company_bank.focus();
                    return false;
                }
                if (txt_pmt_desc.value == "") {
                    alert("Please Enter Description");
                    txt_pmt_desc.focus();
                    return false;
                }
                if (txt_pmt_amount.value == "") {
                    alert("Please Enter Payment Amount");
                    txt_pmt_amount.focus();
                    return false;
                }
                if (txt_pmt_date.value == "") {
                    alert("Please Select Payment Date");
                    txt_pmt_date.focus();
                    return false;
                }
                if (Selected_ddl_pmt_mode == "Select") {
                    alert("Please Select Mode Of Transfer");
                    ddl_pmt_mode.focus();
                    return false;
                }
                var ddl_pmt_mode = document.getElementById('<%=ddl_pmt_mode.ClientID %>');
                var Selected_ddl_pmt_mode = ddl_pmt_mode.options[ddl_pmt_mode.selectedIndex].text;
                var txt_utr_no = document.getElementById('<%=txt_utr_no.ClientID %>');
                var txt_cheque_no = document.getElementById('<%=txt_cheque_no.ClientID %>');
                var txt_cheque_receive_date = document.getElementById('<%=txt_cheque_receive_date.ClientID %>');
                var txt_cheque_deposite_date = document.getElementById('<%=txt_cheque_deposite_date.ClientID %>');

                if (Selected_ddl_pmt_mode == "RTGS") {
                    if (txt_utr_no.value == "") {
                        alert("Please Enter UTR Number");
                        txt_utr_no.focus();
                        return false;
                    }
                }
                else if (Selected_ddl_pmt_mode == "Cheque" || Selected_ddl_pmt_mode == "NEFT") {
                    if (txt_cheque_no.value == "") {
                        alert("Please Enter Cheque Number");
                        txt_cheque_no.focus();
                        return false;
                    }
                    if (txt_cheque_receive_date.value == "") {
                        alert("Please Select Cheque Received Date");
                        txt_cheque_receive_date.focus();
                        return false;
                    }
                    if (txt_cheque_deposite_date.value == "") {
                        alert("Please Select Cheque Deposite Date");
                        txt_cheque_deposite_date.focus();
                        return false;
                    }
                }
                var document1_file = document.getElementById('<%=document1_file.ClientID %>');
                if (document1_file.value == "") {
                    alert("Please Upload Document");
                    document1_file.focus();
                    return false;
                }
            }
            return true;
        }
        function internal_transfer() {
            var ddl_pmt_paid = document.getElementById('<%=ddl_pmt_paid.ClientID %>');
            var Selected_ddl_pmt_paid = ddl_pmt_paid.options[ddl_pmt_paid.selectedIndex].text;

            var ddl_pmt_client = document.getElementById('<%=ddl_pmt_client.ClientID %>');
            var Selected_ddl_pmt_client = ddl_pmt_client.options[ddl_pmt_client.selectedIndex].text;

            var ddl_company_bank = document.getElementById('<%=ddl_company_bank.ClientID %>');
            var Selected_ddl_company_bank = ddl_company_bank.options[ddl_company_bank.selectedIndex].text;

            var txt_pmt_date = document.getElementById('<%=txt_pmt_date.ClientID %>');
            var txt_pmt_amount = document.getElementById('<%=txt_pmt_amount.ClientID %>');
            var txt_pmt_desc = document.getElementById('<%=txt_pmt_desc.ClientID %>');


            var ddl_pmt_mode = document.getElementById('<%=ddl_pmt_mode.ClientID %>');
            var Selected_ddl_pmt_mode = ddl_pmt_mode.options[ddl_pmt_mode.selectedIndex].text;

            if (Selected_ddl_pmt_paid == "Internal Transfer") {
                if (Selected_ddl_pmt_client == "Select") {
                    alert("Please Select Transfer To");
                    ddl_pmt_client.focus();
                    return false;
                }
                if (Selected_ddl_company_bank == "Select") {
                    alert("Please Select Transfer From");
                    ddl_company_bank.focus();
                    return false;
                }
                if (txt_pmt_amount.value == "") {
                    alert("Please Enter Payment Amount");
                    txt_pmt_amount.focus();
                    return false;
                }
                if (txt_pmt_date.value == "") {
                    alert("Please Select Payment Date");
                    txt_pmt_date.focus();
                    return false;
                }
                if (Selected_ddl_pmt_mode == "Select") {
                    alert("Please Select Mode Of Transfer");
                    ddl_pmt_mode.focus();
                    return false;
                }
                var ddl_pmt_mode = document.getElementById('<%=ddl_pmt_mode.ClientID %>');
                var Selected_ddl_pmt_mode = ddl_pmt_mode.options[ddl_pmt_mode.selectedIndex].text;
                var txt_utr_no = document.getElementById('<%=txt_utr_no.ClientID %>');
                var txt_cheque_no = document.getElementById('<%=txt_cheque_no.ClientID %>');
                var txt_cheque_receive_date = document.getElementById('<%=txt_cheque_receive_date.ClientID %>');
                var txt_cheque_deposite_date = document.getElementById('<%=txt_cheque_deposite_date.ClientID %>');

                if (Selected_ddl_pmt_mode == "RTGS") {
                    if (txt_utr_no.value == "") {
                        alert("Please Enter UTR Number");
                        txt_utr_no.focus();
                        return false;
                    }
                }
                else if (Selected_ddl_pmt_mode == "Cheque" || Selected_ddl_pmt_mode == "NEFT") {
                    if (txt_cheque_no.value == "") {
                        alert("Please Enter Cheque Number");
                        txt_cheque_no.focus();
                        return false;
                    }
                    if (txt_cheque_receive_date.value == "") {
                        alert("Please Select Cheque Received Date");
                        txt_cheque_receive_date.focus();
                        return false;
                    }
                    if (txt_cheque_deposite_date.value == "") {
                        alert("Please Select Cheque Deposite Date");
                        txt_cheque_deposite_date.focus();
                        return false;
                    }
                }
            }
            return true;
        }
        function drp_operation() {
            var ddl_trmode = document.getElementById('<%=ddl_trmode.ClientID %>');
            var Selected_ddl_trmode = ddl_trmode.options[ddl_trmode.selectedIndex].text;
            if (Selected_ddl_trmode == "Bank") {
                $(".policy_installment").show();
            }
            else if (Selected_ddl_trmode == "Cash") {
                $(".policy_installment").hide();
            }
            else if (Selected_ddl_payment_mode == "Select") {
                $(".policy_installment").hide();
            }
        }
        function Search_Gridview(strKey) {
            var strData = strKey.value.toLowerCase().split(" ");
            var tblData = document.getElementById("<%=gv_invoice_list.ClientID %>");
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

    </script>
    <script type="text/javascript">
        function startCalc() {
            var grid = document.getElementById("<%= gv_invoice_pmt.ClientID%>");
            for (var i = 0; i < grid.rows.length - 1; i++) {
                var received_amt1 = $("input[id*=txt_received_amt1]");
                var received_amt2 = $("input[id*=txt_received_amt2]");
                var total_received = received_amt1[i].value + received_amt2[i].value;
                $("input[id*=txt_total_received]") = total_received[i].value;

            }
         
            function Credit_note_amt() {
                var grid = document.getElementById("<%= gv_invoice_pmt.ClientID%>");
                 for (var i = 0; i < grid.rows.length - 1; i++) {
                     var ddl_remark_head = $("input[id*=ddl_remark_head]");
                     var ddl_remark_head_value = ddl_remark_head[i].value ;
                     if (ddl_remark_head_value == "Credit Note") {
                         $("input[id*=txt_deduct_amt]").show();                     
                     }
                     else {
                         $("input[id*=txt_deduct_amt]").hide();
                     }
                    

                 }

        }
    </script>
    <script type="text/javascript">
        function Calbal() {


            var grid = document.getElementById("<%= gv_invoice_pmt.ClientID%>");
            for (var i = 0; i < grid.rows.length - 1; i++) {
                var recive_amt = $("input[id*=txt_recive_amt]");
                var total_received = $("input[id*=txt_total_received]");
                var balance = recive_amt[i].value - total_received[i].value;
                $("input[id*=txt_balance]") = balance[i].value;


            }
        }
        function showCal(obj) {
            var table = document.getElementById("gv_invoice_pmt");
            var sum = 0;
            for (var i = 1; i < table.rows.length; i++) {
                if (isNaN(table.rows[i].cells[8].innerText != null)) {
                    if (!isNaN(table.rows[i].cells[9].innerText)) {
                        sum = (parseFloat(sum) + parseFloat(table.rows[i].cells[9].innerText)).toString();
                    }
                }
                document.getElementById("txt_recived_am").innerText = sum;
            }
        }
    </script>


    <div class="container-fluid">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>


        <asp:Panel ID="Panel4" runat="server" CssClass="panel panel-primary" Style="border-color: gray;">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-9">
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>Payment History</b></div>
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
                        <div style="text-align: left; color:white; font-size: small;"><b>Payment History Details</b></div>
                    </div>
                    <div class="col-sm-2 text-left">
                        
                    </div>
                </div>
            </div>--%>
            <%--    vikas  --%>
            <br />
            <div class="container-fluid">
                <div id="tabs" style="background: #f3f1fe; padding: 25px 25px 25px 25px; border: 1px solid #e2e2dd; margin-bottom: 25px; margin-top: 25px; border-radius: 10px">
                    <asp:HiddenField ID="hidtab" Value="0" runat="server" />
                    <ul>
                        <li><a href="#menu1" runat="server"><b>Receipt</b></a></li>
                        <li><a href="#menu2"><b>Receipt Details</b></a></li>
                        <li><a href="#menu3"><b>Payment</b></a></li>
                        <li><a href="#menu4"><b>Sales Entry</b></a></li>
                        <li><a href="#menu5"><b>Upload Ledger</b></a></li>
                        <li><a href="#menu6"><b>Send Outsanding Mail</b></a></li>
                    </ul>
                    <div id="menu1">

                        <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                            <ContentTemplate>
                        <div class="row">
                            <div class="col-sm-2 col-xs-12" style="margin-left: 95px;"></div>
                            <div class="col-sm-2 col-xs-12 ">
                                <b>Received from :</b>

                                <asp:DropDownList ID="ddl_pmt_recived" runat="server" class="form-control" OnSelectedIndexChanged="ddl_pmt_recived_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Value="Select">Select</asp:ListItem>
                                    <asp:ListItem Value="0">Client</asp:ListItem>
                                    <asp:ListItem Value="1">Other</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                                    <div class="col-sm-2 col-xs-12 " id="for_client" runat="server">
                                        <b>Client Name :</b>
                                        <%--OnSelectedIndexChanged="ddl_minibank_client_SelectedIndexChanged" AutoPostBack="true"--%>
                                        <asp:DropDownList ID="ddl_minibank_client" runat="server" class="form-control">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2 col-xs-12 " id="for_other" runat="server">
                                        Other :

                            <asp:DropDownList ID="ddl_other" runat="server" class="form-control" AutoPostBack="true">
                            </asp:DropDownList>
                            </div>

                                    <div class="col-sm-2 col-xs-12">
                                        <asp:Button ID="btn_add_others" runat="server" class="btn btn-primary text-center" Text="Add" />
                                    </div>
                                </div>
                            </ContentTemplate>

                        </asp:UpdatePanel>

                        <div class="row">

                            <div class="col-sm-2">

                                <asp:Panel runat="server" ID="pnl_bank_details" class="panel panel-primary" Style="background: fixed;">
                                    <div class="row">
                                        <div class="col-sm-12 " id="for_other1" runat="server">
                                            <b>Bank Name :</b><br />
                                            <asp:DropDownList ID="ddl_other_bank" runat="server" class="form-control">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-sm-12 " id="bank_name" runat="server">
                                            <b>Bank Name :</b><br />
                                            <asp:Literal ID="lit_bank_name" runat="server"></asp:Literal>
                                        </div>
                                        <div class="col-xs-12">
                                            <b>A/C Number :</b><br />
                                            <asp:Literal ID="lit_comp_ac_number" runat="server"></asp:Literal>
                                        </div>
                                        <div class="col-sm-12" id="client_bank" runat="server">
                                            <b>Client Bank Name :</b><br />
                                            <asp:Literal ID="lit_client_bank" runat="server"></asp:Literal>
                                        </div>
                                        <div class="col-sm-12" id="client_ac_no" runat="server">
                                            <b>Client A/C Number :</b><br />
                                            <asp:Literal ID="lit_client_ac_number" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                </asp:Panel>

                            </div>

                            <div class="col-sm-8">
                                <asp:Panel runat="server" ID="Panel3" class="panel panel-primary" Style="border-color: white; background: white">


                                    <div class="row" style="padding-left: 20px; padding-right: 20px;">

                                        <div class="col-sm-3 col-xs-12">
                                            <b>Received Date :</b>
                                            <asp:TextBox ID="txt_minibank_received_date" runat="server" CssClass="form-control date-picker1 "></asp:TextBox>
                                        </div>



                                        <div class="col-sm-3 col-xs-12">
                                            <b>Amount :</b>
                                            <asp:TextBox ID="txt_minibank_amount" runat="server" CssClass="form-control" onkeypress="return isNumberamt(event)" value="0">0</asp:TextBox>
                                        </div>


                                        <div class="col-sm-0 col-xs-12" style="display: none">
                                            <b>Payment Hit Date :</b>
                                            <asp:TextBox ID="txt_payment_hit_date" runat="server" CssClass="form-control date-picker3 "></asp:TextBox>
                                        </div>
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                            <ContentTemplate>
                                                <div class="col-sm-3 col-xs-12 ">
                                                    <b>Pay.Mode:</b>
                                                    <asp:DropDownList ID="ddl_mode_transfer" runat="server" CssClass="form-control"> <%--OnSelectedIndexChanged="ddl_mode_transfer_SelectedIndexChanged" AutoPostBack="true"--%>
                                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                                        <asp:ListItem Value="NEFT">NEFT</asp:ListItem>
                                                        <asp:ListItem Value="RTGS">RTGS</asp:ListItem>
                                                       <%-- <asp:ListItem Value="Cheque">Cheque</asp:ListItem>--%>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-sm-3 col-xs-12" id="utr_no" runat="server">
                                                    <b>UTR NO :</b>
                                                    <asp:TextBox ID="txt_minibank_utr_no" runat="server" CssClass="form-control  " onkeypress="return allowAlphaNumericSpace(event)" MaxLength="40"></asp:TextBox>
                                                </div>
                                                <div class="col-sm-3 col-xs-12" id="cheque" runat="server">
                                                    Cheque No :
                                          <asp:TextBox ID="txt_cheque" runat="server" CssClass="form-control" MaxLength="20" onkeypress="return allowAlphaNumericSpace(event);"></asp:TextBox>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>

                                    </div>
                                    <div class="row" style="padding-left: 20px; padding-right: 20px;">


                                        <asp:Panel runat="server" ID="Panel1">
                                            <div class="col-sm-3 col-xs-12">
                                                <asp:Label ID="lbl_payment_type" runat="server" Font-Bold="true" Text=" Payment Against :"></asp:Label>
                                                <asp:DropDownList ID="ddl_payment_type" runat="server" CssClass="form-control" >  <%--OnSelectedIndexChanged="ddl_payment_type_SelectedIndexChanged"--%>
                                                    <asp:ListItem Value="Select">Select</asp:ListItem>
                                                    <asp:ListItem Value="1">Invoice Against</asp:ListItem>
                                                    <asp:ListItem Value="2">Advance Payment</asp:ListItem>
                                                     <%-- <asp:ListItem Value="3">Payment + Advance</asp:ListItem>
                                                      <asp:ListItem Value="4">Payment - Advance</asp:ListItem>
                                                     <asp:ListItem Value="5"> - Advance</asp:ListItem>--%>
                                                </asp:DropDownList>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnl_desc">
                                            <div class="col-sm-3 col-xs-12" id="desc" runat="server">
                                                Description:
                                                                <asp:TextBox ID="txt_description" runat="server" CssClass="form-control glyphicon-align-center" Style="height: 35px; width: 168px; margin-top: 11px;" Placeholder="Enter Description " onkeypress="return isNumbernum(event)"></asp:TextBox>
                                            </div>
                                        </asp:Panel>

                                        <div class="col-sm-3 col-xs-12">
                                            <b>Remark :</b>
                                            <asp:TextBox ID="txt_utr_remark" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-3 col-xs-12">
                                            <br />
                                            <asp:LinkButton ID="lbtn_addutr" runat="server" OnClientClick="return Required_mini_validation();" OnClick="lbtn_addutr_Click">
                                      <img alt="Add Item" src="Images/add_icon.png"  />
                                            </asp:LinkButton>
                                        </div>
                                        <%--OnClick="linkbtn_esic_Click"--%>
                                        <div class="col-sm-0 col-xs-12" style="display: none">
                                            <b>id :</b>
                                            <asp:TextBox ID="txt_id" runat="server" class="form-control "></asp:TextBox>
                                        </div>

                                    </div>


                                    <div class="row" style="padding-left: 20px; padding-right: 20px;">
                                        <div class="col-sm-12">
                                            <asp:GridView ID="gv_add_utr" class="table" runat="server" BackColor="White" OnRowDataBound="gv_add_utr_RowDataBound"
                                                BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                                                AutoGenerateColumns="False" Width="100%" OnPreRender="gv_add_utr_PreRender">
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
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="linkbtn_removeitem" runat="server" CausesValidation="false" OnClick="linkbtn_removeitem_Click" OnClientClick="return R_validation();"><img alt="" height="15"  src="Images/delete_icon.png" width="15" /></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Sr No.">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_srnumber" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Receipt_Date" HeaderText="Receipt Date" SortExpression="Receipt_Date" />
                                                    <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" />
                                                    <asp:BoundField DataField="UTR_no" HeaderText="UTR No" SortExpression="UTR_no" />
                                                    <asp:BoundField DataField="Payment_mode" HeaderText="Payment_mode" SortExpression="Payment_mode" />
                                                    <asp:BoundField DataField="Payment_against" HeaderText="Payment Against" SortExpression="Payment_against" />
                                                    <asp:BoundField DataField="Remark" HeaderText="Remark" SortExpression="Remark" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>


                                    <div class="row" id="btn_row" runat="server" visible="false" style="padding-left: 20px; padding-right: 20px;">
                                        <div class="col-sm-4 col-xs-12">
                                            <b>Attachment :</b>
                                            <asp:FileUpload ID="photo_upload" runat="server" meta:resourcekey="photo_uploadResource1" CssClass="text_box" />
                                            <asp:Label ID="lbl_note" runat="server" ForeColor="Red"></asp:Label>
                                        </div>


                                        <div class="col-sm-4 col-xs-12">

                                            <asp:Button ID="Button1" runat="server" class="btn btn-primary text-center" OnClick="btn_minibank_submit_Click" Text="Submit" /><%--OnClientClick="return Required_mini_validation();"--%>
                                            <asp:Button ID="btn_approve_minibank" runat="server" class="btn btn-primary text-center" OnClick="btn_approve_minibank_Click" OnClientClick="return R_validation(); " Text="Approve" />
                                            <asp:Button ID="btn_update_receipt" runat="server" class="btn btn-primary text-center" OnClick="btn_update_receipt_Click" OnClientClick="return Required_mini_validation();" Text="Update" />

                                            <asp:Button ID="Button2" runat="server" Text="Close" OnClick="btn_close_click" class="btn btn-danger text-center" />



                                        </div>



                                    </div>

                                    <br />
                                </asp:Panel>
                            </div>
                        </div>
                        <%--  </div>--%>





                        <%-- UTR Link btn --%>
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                            <ContentTemplate>
                                <div class="text-center">
                                    <asp:LinkButton ID="lbtn_utr" runat="server" Text="UTR Report" OnClick="lbtn_utr_Click" ForeColor="Blue" Font-Bold="True"></asp:LinkButton>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <%-- UTR Link btn --%>


                        <%-- View UTR Report--%>
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col-sm-8 col-md-offset-2">
                                        <asp:Panel runat="server" ID="pnl_utr_report" class="panel panel-primary" Style="border-color: white; background: white">
                                            <br />
                                            <div class="row">
                                                <div class="col-md-2 col-md-offset-2">
                                                    <label>From Date :</label>
                                                    <asp:TextBox ID="txt_from_date" CssClass="form-control date-picker1" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-2 ">
                                                    <label>To Date :</label>
                                                    <asp:TextBox ID="txt_to_date" CssClass="form-control date-picker1" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-2">
                                                    <b>Nockoff Status:</b>
                                                    <asp:DropDownList ID="ddl_nockoff_status" runat="server" CssClass="form-control">
                                                        <asp:ListItem Value="All">All</asp:ListItem>
                                                        <asp:ListItem Value="Yes">Complete</asp:ListItem>
                                                        <asp:ListItem Value="No">Pending</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>


                                                <div class="col-sm-1 col-md-offset-1">
                                                    <br />
                                                    <asp:Button ID="btn_UTR_report" runat="server" class="btn btn-primary text-center" OnClick="btn_UTR_report_Click" Text="Report" />
                                                </div>
                                                <div class="col-sm-1">
                                                    <br />
                                                    <asp:Button ID="btn_utr_view" runat="server" class="btn btn-primary text-center" OnClick="btn_utr_view_Click" Text="View" />
                                                </div>
                                            </div>
                                            <br />
                                        </asp:Panel>
                                    </div>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btn_UTR_report" />
                                <asp:PostBackTrigger ControlID="btn_utr_view" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <%-- View UTR Report--%>



                        <asp:Panel ID="panel5" runat="server" CssClass="grid-view" Style="overflow-x: auto;">
                            <h4 class="text-center">Transaction</h4>
                            <div class="container-fluid">
                             <%-- <asp:UpdatePanel ID="UpdatePanel11" runat="server">
                            <ContentTemplate>--%>
                                <asp:GridView ID="gv_minibank" class="table" DataKeyNames="Id" runat="server" Font-Size="X-Small" BackColor="White"
                                    BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnRowDataBound="gv_minibank_RowDataBound" OnPreRender="gv_minibank_PreRender">
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
                                        <asp:TemplateField HeaderText="DELETE" ControlStyle-Width="120px" Visible="false">
                                            <ItemStyle Width="20px" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="false" AutoPostBack="true" Width="120%" CommandArgument='<%# Eval("ID")%>' OnCommand="lnkminiDelete_Command" OnClientClick="return confirm('Are you sure You want to  Delete ?')"><img alt="" height="15" style = "margin-left: 23px;" src="Images/delete_icon.png" width="15" /></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="DOWNLOAD FILE" ControlStyle-Width="120px">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_download" runat="server" Style="color: white" CausesValidation="false" Text="DOWNLOAD" CommandArgument='<%# Eval("upload_file")%>' OnCommand="lnk_download_Command"  ><img alt="" height="40"  src="Images/Download_icon.jpg" width="40" /></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Approve" ControlStyle-Width="120px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btn_edit_other1" Text="Approve" runat="server" CssClass="btn btn-primary" Style="color: white" Width="80%" OnClick="btn_edit_other1_Click"></asp:LinkButton>
                                            </ItemTemplate>

                                        </asp:TemplateField>

                                        <asp:BoundField DataField="receipt_approve" HeaderText="Status" SortExpression="receipt_approve" />
                                    </Columns>
                                </asp:GridView>

                              <%-- </ContentTemplate>
                                 <Triggers>
                                <asp:PostBackTrigger ControlID="ddl_minibank_client" />
                                       
                            </Triggers>
                        </asp:UpdatePanel>--%>
                                <br />
                            </div>
                        </asp:Panel>

                        <%--<div class="container-fluid">
                                    <asp:Panel ID="panel5" runat="server" CssClass="grid-view">
                                        <asp:GridView ID="gv_minibank" class="table" runat="server" BackColor="White"
                                            BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnSelectedIndexChanged="gv_minibank_SelectedIndexChanged1" OnRowDataBound="gv_minibank_RowDataBound" OnPreRender="gv_minibank_PreRender">
                                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                            <AlternatingRowStyle BackColor="White" />
                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="text-uppercase" />
                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                            <RowStyle BackColor="#EFF3FB" />
                                            <EditRowStyle BackColor="#2461BF" />
                                            <FooterStyle BackColor="White" ForeColor="#000066" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="DELETE">
                                                    <ItemStyle Width="20px" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="false" CommandArgument='<%# Eval("ID")%>' OnCommand="lnkminiDelete_Command" OnClientClick="return confirm('Are you sure You want to  Delete ?')"><img alt="" height="15" style = "margin-left: 23px;" src="Images/delete_icon.png" width="15" /></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="DOWNLOAD FILE">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_download" runat="server" Style="color: white" CausesValidation="false" CssClass="btn btn-primary" Text="DOWNLOAD" CommandArgument='<%# Eval("upload_file")%>' OnCommand="lnk_download_Command"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </div>--%>
                        <%-- </ContentTemplate>
                        </asp:UpdatePanel>--%>
                    </div>





                    <div id="menu2">
                        <br />
                        <%--  <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>--%>
                        <div id="div_process" class="container">
                            <div class="row" id="client_row">


                                <div class="col-sm-2 col-xs-12"></div>
                                <%--<asp:UpdatePanel ID="UpdatePanel11" runat="server">
                                    <ContentTemplate>--%>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Client:</b><span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_client" runat="server" class="form-control" AutoPostBack="true" onkeypress="return AllowAlphabet(event)" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged" MaxLength="10">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>Payment Receving Date :</b><span style="color: red">*</span>
                                    <%-- <asp:TextBox ID="txt_date" runat="server" CssClass="form-control date-picker1"></asp:TextBox>--%>
                                    <asp:DropDownList ID="txt_date" runat="server" CssClass="form-control" OnSelectedIndexChanged="txt_date_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                                <%--  </ContentTemplate>
                                </asp:UpdatePanel>--%>
                                <div class="col-sm-1 col-xs-12" style="margin-top: 16px;"  >
                                    <asp:Button ID="btn_submit" runat="server" Text="Process" class="btn btn-primary" OnClick="btn_submit_Click" OnClientClick="return validation()" Visible="false" />
                                </div>

                                <div class="col-sm-2 col-xs-12" style="margin-left: -93px;">
                                    <b>Select Amount:</b><span style="color: red">*</span>
                                    <asp:DropDownList ID="ddl_client_resive_amt" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_client_resive_amt_SelectedIndexChanged">
                                    </asp:DropDownList>

                                </div>
                                <div class="col-sm-3 col-xs-12" >
                                    <div style="font-size:medium; color: purple;"> <b> <asp:Literal ID="lit_payment_type" runat="server"></asp:Literal> </b></div> 
                              <%--  </div>
                                 <div class="col-sm-2 col-xs-12" style=" color: red; ">--%>
                                   <div style="color: red; font-size:small;">  <b> <asp:Literal ID="lit_utr_no" runat="server"></asp:Literal> </b></div> 
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12"></div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>SETTLED AMOUNT :</b>
                                    <asp:TextBox ID="txt_total_invoice" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <b>REMAINING AMOUNT :</b>
                                    <asp:TextBox ID="txt_deducted" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <div class="col-sm-3 col-xs-14">
                                            <label2> <b>CURRENT RECEIVED AMOUNT :</b></label2>
                                            <asp:TextBox ID="txt_recived_am" runat="server" CssClass="form-control" ReadOnly="true" onkeyup="showCal(this);"></asp:TextBox>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>


                        <%--   <div class="col-sm-10 col-xs-12"></div>
                        <div class="col-sm-2 col-xs-12" style="margin-top: 2em;">
                            <asp:Panel ID="account_link_details" runat="server">
                                <a data-toggle="modal" href="#account_link" style="color: blue; font-weight: bold">Check Receipt Closed Date</a>
                            </asp:Panel>
                        </div>

                                <div class="modal fade" id="account_link" role="dialog" data-dismiss="modal">

                                    <div class="modal-dialog">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                                                <h4 class="modal-title">Check Close Date </h4>

                                            </div>

                                            <div class="modal-body">
                                                <div class="row">
                                                    <div class="col-sm-12">
                                                        <asp:Panel ID="Panel123" runat="server" CssClass="grid-view">
                                                            <asp:GridView ID="gv_links" class="table" Width="100%" HeaderStyle-CssClass="FixedHeader" runat="server" AutoGenerateColumns="false" ShowFooter="false" data-toggle="modal" href="#Div1">
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

                                                                    <asp:BoundField DataField="CLIENT_NAME" HeaderText="Client Name" SortExpression="CLIENT_NAME" />
                                                                    <asp:BoundField DataField="receive_date" HeaderText="receive_date" SortExpression="receive_date" />
                                                                     <asp:BoundField DataField="REMANING_AMOUNT" HeaderText="REMANING_AMOUNT" SortExpression="REMANING_AMOUNT" />
                                                                </Columns>


                                                            </asp:GridView>
                                                        </asp:Panel>
                                                    </div>

                                                </div>

                                                <div class="modal-footer">
                                                    <div class="row text-center">
                                                        <div class="col-sm-4">
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <button type="button" class="btn btn-danger" data-dismiss="modal">CLOSE</button>
                                                        </div>
                                                        <div class="col-sm-4">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                            </div>
                        </div>
                        --%>


                        <div id="div_invoice_list" class="center-block" runat="server">
                            <div class="container" style="width: 80%">
                                <div class="row">
                                    <div class="col-sm-10 col-xs-12"></div>
                                    <div class="col-sm-2 col-xs-12" id="search" runat="server">
                                        Search :
                        <asp:TextBox runat="server" ID="txt_search1" CssClass=" form-control" onkeyup="Search_Gridview(this)" />
                                            </div>
                                            <br />
                                            <br />
                                        </div>

                                <asp:Panel ID="Panel6" runat="server" CssClass="grid-view">
                                    <asp:GridView ID="gv_invoice_list" runat="server"
                                        class="table" DataKeyNames="Invoice_no" BackColor="White"
                                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%"
                                        OnRowDataBound="gv_fullmonthot_RowDataBound" OnPreRender="gv_invoice_list_PreRender">
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
                                            <asp:TemplateField HeaderText="Sr. No.">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex+1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="SELECT INVOICE" ItemStyle-Width="05%">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chk_invoice" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--<asp:TemplateField HeaderText="Sr. No.">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SELECT INVOICE" ItemStyle-Width="25%">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chk_invoice" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="Invoice_no" HeaderText="INVOICE NO" SortExpression="Invoice_no" HeaderStyle-Width="144px" ItemStyle-Width="144px" ControlStyle-Width="144px" />


                                                    <asp:BoundField DataField="bill_month" HeaderText="Billing Month" SortExpression="bill_month" ItemStyle-Width="30%" />
                                                    <asp:BoundField DataField="billing_amt" HeaderText="Billing Amount" SortExpression="billing_amt" ItemStyle-Width="50%" />
                                                     <asp:BoundField DataField="billing_amt" HeaderText="Balance Amount" SortExpression="Balanced_Amount" ItemStyle-Width="50%" />--%>

                                                </Columns>
                                            </asp:GridView>

                                            <br />

                                        </asp:Panel>
                                        <div class="row">
                                            <div class="col-xs-12 text-center">
                                                <asp:Button ID="btn_process" runat="server" class="btn btn-primary" OnClientClick="return Validate();" OnClick="btn_process_Click" Text="Submit" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <br />
                                 
                            <%-- </ContentTemplate>
                        </asp:UpdatePanel>--%>
                                
                                
                                   <%--        <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                            <ContentTemplate>--%>
                                <div class="container-fluid">
                                    <div id="div_invoice_pmt_gv" class="center-block">
                                        <asp:Panel ID="Panel_gv_pmt" runat="server">
                                 <div style="overflow:auto">
                                              <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                                              <ContentTemplate>
                                            <asp:GridView ID="gv_invoice_pmt" class="table" runat="server" CellPadding="3" AutoGenerateColumns="False" Width="100%"
                                                ForeColor="#333333" OnRowDataBound="gv_invoice_pmt_RowDataBound" OnSelectedIndexChanged="gv_invoice_pmt_SelectedIndexChanged"  >
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
                                                 <Columns >
                                                    <asp:TemplateField HeaderText="SR. NO.">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_srnumber" runat="server" Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="Invoice_no" SortExpression="Invoice_no" ControlStyle-Width="110px" HeaderText="INVOICE NO" HeaderStyle-Width="110px" ItemStyle-Width="110px" />


                                                    <asp:BoundField DataField="billing_amt" SortExpression="billing_amt" ControlStyle-Width="110px" HeaderText="BILLING AMOUNT" HeaderStyle-Width="110px" ItemStyle-Width="110px" />

                                                    <asp:TemplateField runat="server" ControlStyle-Width="100px" HeaderStyle-Width="100px" ItemStyle-Width="100px">

                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_tds_amt" runat="server" CssClass="form-control" Text='<%# Eval("tds_amt")%>' onkeypress="return isNumberKey(event,this.id)" OnTextChanged="txt_total_received_TextChanged1" AutoPostBack="true"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="RECEIVABLE AMOUNT" ControlStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-Width="110px">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_recive_amt" runat="server" CssClass="form-control" Text='<%# Eval("receviable_amt")%>' Width="150" ReadOnly="true" onkeypress="return isNumberKey(event,this.id)" AutoPostBack="true"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="RECIVING DATE" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_reciving_date" runat="server" CssClass="form-control" Text='<%# Eval("receving_date")%>' Width="100" ReadOnly="true"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                  <%--  <asp:TemplateField HeaderText="Received Amount1">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_received_amt1" runat="server" CssClass="form-control" min="0" Text='<%# Eval(" received_amt1 ")%>' OnTextChanged="txt_total_received_TextChanged1" AutoPostBack="true" EnableViewState="false"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                                      <asp:TemplateField HeaderText="Received Amount1" ControlStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-Width="110px">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_received_amt1" runat="server" CssClass="form-control" min="0" value="0" Text='<%# Eval("received_amt1")%>' onkeypress="return isNumberKey(event,this.id)"  OnTextChanged="txt_total_received_TextChanged1" AutoPostBack="true"></asp:TextBox> <%--onkeyup="startCalc();"--%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Received Amount2" ControlStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-Width="110px">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_received_amt2" runat="server" CssClass="form-control" min="0" value="0" Text='<%# Eval("received_amt2")%>'  onkeypress="return isNumberKey(event,this.id)" OnTextChanged="txt_total_received_TextChanged1" AutoPostBack="true"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Received Amount3" ControlStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-Width="110px">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_received_amt3" runat="server" CssClass="form-control" min="0" value="0" Text='<%# Eval("received_amt3")%>'  onkeypress="return isNumberKey(event,this.id)" OnTextChanged="txt_total_received_TextChanged1" AutoPostBack="true"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Total Received" ControlStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-Width="110px">
                                                        <ItemTemplate>

                                                            <asp:TextBox ID="txt_total_received" runat="server" CssClass="form-control" min="0"  value="0" Text='<%# Eval("total_received")%>'  ReadOnly="true" AutoPostBack="true"></asp:TextBox>     <%-- onkeyup="Calbal();"   onkeypress="showCal(this);--%>

                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Balance" ControlStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-Width="110px">
                                                        <ItemTemplate>

                                                            <asp:TextBox ID="txt_balance" runat="server" CssClass="form-control" min="0"  Text='<%# Eval("balance")%>' onkeypress="Calbal();" AutoPostBack="true" ReadOnly="true"></asp:TextBox>

                                                        </ItemTemplate>
                                                    </asp:TemplateField>


                                                    <%--                                     <asp:TemplateField HeaderText="TDS" >
                        <ItemTemplate>
                          <asp:DropDownList ID="ddl_tds_amount" runat="server" CssClass="form-control droplist" Width="100" SelectedValue='<%# Bind("tds") %>'>
                                          <asp:ListItem Value="Amount">Amount</asp:ListItem>
                                          <asp:ListItem Value="1">TDS 1%</asp:ListItem>
                                          <asp:ListItem Value="2">TDS 2%</asp:ListItem>
                                      </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                                                    <asp:TemplateField HeaderText="TDS ON" ControlStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-Width="110px">

                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ddl_tds_on" runat="server" CssClass="form-control" SelectedValue='<%# Bind("tds_on") %>'>
                                                                <asp:ListItem Value="0">Taxable Amount</asp:ListItem>
                                                                <asp:ListItem Value="1">Billing Amount</asp:ListItem>

                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="ADJUSTMENT" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ddl_adjustment" runat="server" CssClass="form-control droplist1" Width="100" onchange=" adjustment(event,this.id);" SelectedValue='<%# Bind("adjustment_sign") %>' Visible="false">
                                                                <asp:ListItem Value="0">Select</asp:ListItem>
                                                                <asp:ListItem Value="1">+</asp:ListItem>
                                                                <asp:ListItem Value="2">-</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="ADJUSTMENT AMOUNT" Visible="false">
                                                        <ItemTemplate>

                                                            <asp:TextBox ID="txt_adjustment_amt" runat="server" CssClass="form-control" min="0" max="10" Text='<%# Eval("adj_amt")%>' Visible="false"></asp:TextBox>

                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Payment Status" ControlStyle-Width="100px" HeaderStyle-Width="100px" ItemStyle-Width="100px" >
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ddl_remark_head" runat="server" CssClass="form-control droplist1" OnSelectedIndexChanged="ddl_remark_head_SelectedIndexChanged" AutoPostBack="true" >
                                                                <asp:ListItem Value="0">Select</asp:ListItem> <%--cannot change sequence of value--%>
                                                                <asp:ListItem Value="1">GST Hold</asp:ListItem>
                                                                <asp:ListItem Value="2">TDS Not Deduct</asp:ListItem> 
                                                                <asp:ListItem Value="3">Credit Note</asp:ListItem>                                                               
                                                                <asp:ListItem Value="4">Part Payment</asp:ListItem>
                                                                <asp:ListItem Value="5">Payment Done</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DEDUCTION AMT"  ControlStyle-Width="90px" HeaderStyle-Width="90px" ItemStyle-Width="90px">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_deduct_amt" runat="server" CssClass="form-control" min="0" Text='<%# Eval("deduct_amt")%>'  onkeypress="return isNumberKey(event,this.id)" OnTextChanged="txt_total_received_TextChanged1" AutoPostBack="true" Enabled="false"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="REMARK">
                                                        <ItemTemplate>

                                                            <asp:TextBox ID="txt_remark" runat="server" CssClass="form-control" Text='<%# Eval("remark")%>' Style="width: auto"></asp:TextBox>

                                                        </ItemTemplate>
                                                    </asp:TemplateField>




                                                </Columns>
                                            </asp:GridView>
                                              </ContentTemplate>
                                              </asp:UpdatePanel>
                                </div>
                                            <div class="row ">
                                                <div class="col-sm-2 col-md-offset-2">
                                                    <label style="text-align: left">Ledger Attachment :</label>
                                                    <asp:FileUpload ID="file_ledger_upload" runat="server" meta:resourcekey="photo_uploadResource1" CssClass="text_box" />
                                                    <asp:Label ID="lbl_ledger_file" runat="server" ForeColor="Red"></asp:Label>
                                                </div>
                                                <div class="col-sm-8">
                                                    <asp:Button ID="btn_save" CssClass="btn btn-primary" OnClick="btn_save_Click" runat="server" Text="Save" />
                                                    <asp:Button ID="btn_approve_receipt_de" runat="server" class="btn btn-primary" OnClick="btn_approve_receipt_de_Click" OnClientClick="return Validate();" Text="Approve" />
                                                    <asp:Button ID="btn_update" CssClass="btn btn-primary" OnClick="btn_update_Click" runat="server" Text="Update" />
                                                    <asp:Button ID="btn_close" CssClass="btn btn-danger" OnClick="btn_pmt_close_click" runat="server" Text="Close" />
                                                </div>
                                            </div>

                                
                                        </asp:Panel>
                                    </div>
                                </div>
                          <%-- </ContentTemplate>
                        </asp:UpdatePanel>--%>
                      


                        <div class="row">
                            <%--  <asp:UpdatePanel ID="UpdatePanel9" runat="server"> 
                            <ContentTemplate>--%>
                            <div class="col-sm-2 col-xs-12">
                                <b>Client:</b><%--<span style="color: red">*</span>--%><asp:DropDownList ID="ddl_client_gv" runat="server" class="form-control">
                                    <%-- OnClientClick="return gv_receip_detail_disable();"--%>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-2 col-xs-12">
                                <b>Remaning:</b><%--<span style="color: red">*</span>--%><asp:DropDownList ID="ddl_type" runat="server" class="form-control">
                                    <%--OnClientClick="return gv_receip_detail_disable();"--%>
                                    <asp:ListItem Value="1">SETTLED AMOUNT</asp:ListItem>
                                    <asp:ListItem Value="2">REMANING AMOUNT</asp:ListItem>
                                    <asp:ListItem Value="ALL">ALL</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <%--                                  </ContentTemplate> 
                        </asp:UpdatePanel>--%>

                            <div class="col-md-2">
                                <br />
                                <asp:Button ID="btn_download_utr_inv" CssClass="btn btn-block btn-primary" OnClick="btn_download_utr_inv_click" runat="server" Text="Download" />
                            </div>

                            <div class="col-md-1">
                                <br />
                                <asp:Button ID="btn_view" CssClass="btn btn-block btn-primary" OnClick="btn_view_Click" runat="server" Text="View" />
                            </div>
                        </div>


                        <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                            <ContentTemplate>
                                <asp:Panel ID="panel2" runat="server" CssClass="grid-view" Style="overflow-x: auto;">

                                    <div class="container-fluid">

                                        <asp:GridView ID="gv_payment" class="table" DataKeyNames="Id" runat="server" BackColor="White"
                                            BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnSelectedIndexChanged="gv_payment_SelectedIndexChanged" OnRowDataBound="gv_payment_RowDataBound" OnPreRender="gv_payment_PreRender">
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
                                                <asp:TemplateField HeaderText="Delete" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnk_remove_manual_other" runat="server" CausesValidation="false" OnClick="lnk_remove_manual_other_Click" AutoPostBack="true" OnClientClick="return confirm('Are you sure You want to  Delete ?')">
                                                    <img alt="" height="15"  src="Images/delete_icon.png" width="15" /></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="receipt_de_approve" HeaderText="Status" SortExpression="receipt_de_approve" />
                                            </Columns>
                                        </asp:GridView>


                                        <br />
                                    </div>
                                    <div>
                                        <h4 class="text-center" id="head_transction" runat="server">Transaction Details</h4>
                                        <div class="container-fluid">
                                            <asp:GridView ID="gv_payment_detail" class="table" runat="server" BackColor="White"
                                                BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnSelectedIndexChanged="gv_payment_detail_SelectedIndexChanged" OnRowDataBound="gv_payment_detail_RowDataBound" OnPreRender="gv_payment_detail_PreRender">
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
                                                    <asp:TemplateField HeaderText="DELETE" ItemStyle-Width="25%" Visible="false">
                                                        <ItemStyle Width="20px" />
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnk_remove_product" runat="server" CausesValidation="false" CommandArgument='<%# Eval("ID")%>' OnCommand="lnkDelete_Command" OnClientClick="return confirm('Are you sure You want to  Delete ?')"><img alt="" height="15" style = "margin-left: 23px;"  src="Images/delete_icon.png" width="15" /></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="receipt_de_approve" HeaderText="Status" SortExpression="receipt_de_approve" />
                                                </Columns>
                                            </asp:GridView>




                                            <br />



                                        </div>
                                        <br />

                                    </div>

                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <div id="menu3">
                        <br />
                        <div class="row">
                            <div class="col-sm-3 col-xs-12 "></div>
                            <div class="col-sm-2 col-xs-12 ">
                                <b>Paid To :</b>
                                <asp:DropDownList ID="ddl_pmt_paid" runat="server" class="form-control" OnSelectedIndexChanged="ddl_pmt_paid_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Value="1">Employee Payment</asp:ListItem>
                                    <asp:ListItem Value="2">Vendor Payment</asp:ListItem>
                                    <asp:ListItem Value="3">Internal Transfer</asp:ListItem>
                                    <%-- <asp:ListItem Value="4">Employee Adv Payment</asp:ListItem>
                                <asp:ListItem Value="5">Other Payment</asp:ListItem>--%>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-2 col-xs-12 ">
                                <asp:Label ID="lable_client" Font-Bold="true" runat="server"></asp:Label>

                                <asp:DropDownList ID="ddl_pmt_client" runat="server" class="form-control" OnSelectedIndexChanged="ddl_pmt_client_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>

                            <%--<cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server" PopupControlID="Panel7" TargetControlID="btn_pmt_add_other"
                                CancelControlID="btn_pmt_cancel" BackgroundCssClass="Background">
                            </cc1:ModalPopupExtender>
                            <asp:Panel ID="Panel7" runat="server" CssClass="Popup" Style="display: none">
                                <iframe style="width: 800px; height: 500px; background-color: #fff;" id="Iframe1" src="add_other_client.aspx" runat="server"></iframe>
                                <div class="row text-center">
                                    <asp:Button ID="btn_pmt_cancel" CssClass="btn btn-danger" OnClientClick="callfnc2()" runat="server" Text="Close" />
                                </div>

                                <br />

                            </asp:Panel>

                            <br />--%>
                            <asp:Panel runat="server" ID="panel_add_other">

                                <div class="col-sm-2 col-xs-12">
                                    <asp:Button ID="btn_pmt_add_other" runat="server" class="btn btn-primary text-center" Text="Add" />
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="panel_annexure_id">
                                <div class="col-sm-2 col-xs-12">
                                    <b>Batch No:</b>

                                    <asp:DropDownList ID="ddl_batch_no" runat="server" class="form-control" OnSelectedIndexChanged="ddl_batch_no_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <%-- <div class="col-sm-2 col-xs-12" style="margin-top: -15px;">
                                                      Annexure Payble Amount :
                                                        <asp:textbox id="txt_annuxure_amt" runat="server" cssclass="form-control " ReadOnly = "true"   ></asp:textbox>
                                                   </div>--%>
                            </asp:Panel>

                            <asp:Panel runat="server" ID="panel_ac_no">
                                <div class="col-sm-2 col-xs-12" style="margin-top: -14px;">
                                    <b>A/C No :</b>
                                    <asp:TextBox ID="txt_pmt_ac_no" runat="server" CssClass="form-control " ReadOnly="true"></asp:TextBox>
                                </div>
                            </asp:Panel>

                        </div>
                        <br />
                        <asp:Panel runat="server" ID="Panel8" class="panel panel-primary" Style="background: white; padding: 25px 25px 25px 25px; border: 1px solid #e2e2dd; margin-bottom: 20px; margin-top: 20px; border-radius: 10px">
                            <br />

                            <div class="container">
                                <div class="row">
                                    <div class="col-sm-2 col-xs-12"></div>
                                    <asp:Panel runat="server" ID="panel_txt_bank_name">
                                        <div class="col-sm-2 col-xs-12">
                                            <b>Company Bank Name :</b>
                                            <asp:TextBox ID="txt_comp_bank_name" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel runat="server" ID="panel_ddl_bank_name">
                                        <div class="col-sm-2 col-xs-12">

                                            <asp:Label ID="lable_bank" runat="server"></asp:Label>
                                            <asp:DropDownList ID="ddl_company_bank" runat="server" class="form-control" OnSelectedIndexChanged="ddl_company_bank_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem>Select</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel runat="server" ID="Panel_client_desc">
                                        <div class="col-sm-2 col-xs-12">
                                            <asp:Label ID="Label1" runat="server" Font-Bold="true" Text=" Description :"></asp:Label>
                                            <asp:DropDownList ID="ddl_pmt_desc" runat="server" CssClass="form-control">
                                                <asp:ListItem Value="Select">Select</asp:ListItem>
                                                <asp:ListItem Value="1">Salary</asp:ListItem>

                                            </asp:DropDownList>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel runat="server" ID="Panel_other_desc">
                                        <div class="col-sm-2 col-xs-12">
                                            <asp:Label ID="label_ac_no" runat="server"></asp:Label>
                                            <asp:TextBox ID="txt_pmt_desc" runat="server" CssClass="form-control "></asp:TextBox>
                                        </div>
                                    </asp:Panel>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Amount :</b>
                                        <asp:TextBox ID="txt_pmt_amount" runat="server" CssClass="form-control" onkeypress="return isNumber(event)">0</asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Payment Date :</b>
                                        <asp:TextBox ID="txt_pmt_date" runat="server" CssClass="form-control date-picker1 "></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Transaction Mode:</b>
                                        <asp:DropDownList ID="ddl_trmode" runat="server" CssClass="form-control" onchange="drp_operation()">
                                            <asp:ListItem Value="Select">Select</asp:ListItem>
                                            <asp:ListItem Value="Cash">Cash</asp:ListItem>
                                            <asp:ListItem Value="Bank">Bank</asp:ListItem>

                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-sm-2 col-xs-12"></div>
                                    <div class="col-sm-2 col-xs-12 policy_installment">
                                        <b>Mode Of Transfer :</b>
                                        <asp:DropDownList ID="ddl_pmt_mode" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_pmt_mode_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Value="Select">Select</asp:ListItem>
                                            <asp:ListItem Value="RTGS">RTGS</asp:ListItem>
                                            <asp:ListItem Value="Cheque">Cheque</asp:ListItem>
                                            <asp:ListItem Value="NEFT">NEFT</asp:ListItem>

                                        </asp:DropDownList>
                                    </div>
                                    <asp:Panel runat="server" ID="panel_mode">
                                        <div class="col-sm-2 col-xs-12 ">
                                            <b>UTR No. :</b>
                                            <asp:TextBox ID="txt_utr_no" runat="server" CssClass="form-control" MaxLength="12" onblur="return utr_no();"></asp:TextBox>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel runat="server" ID="panel_mode_cheque">
                                        <div class="col-sm-2 col-xs-12 policy_installment">
                                            <b>Cheque No :</b>
                                            <asp:TextBox ID="txt_cheque_no" runat="server" CssClass="form-control" MaxLength="6" onblur="return cheque_no();" onkeypress="return isNumber(event);"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2 col-xs-12 policy_installment">
                                            <b>Cheque Received Date :</b>
                                            <asp:TextBox ID="txt_cheque_receive_date" runat="server" CssClass="form-control date-picker3"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2 col-xs-12 policy_installment">
                                            <b>Deposite Date :</b>
                                            <asp:TextBox ID="txt_cheque_deposite_date" runat="server" CssClass="form-control date-picker4"></asp:TextBox>
                                        </div>
                                    </asp:Panel>
                                </div>
                                <br />

                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-5 col-xs-12"></div>
                                <div class="col-sm-2 col-xs-12" style="width: 13%;">
                                    <asp:Button ID="btn_pmt_save" runat="server" class="btn btn-primary text-center" OnClientClick="return Required_pmt_validation();" OnClick="btn_pmt_submit_Click" Text="Submit" />
                                    <asp:Button ID="btn_pmt_close" runat="server" Text="Close" OnClick="btn_close_click" class="btn btn-danger" />
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <table class="table table-striped">
                                        <tr>
                                            <th>
                                                <asp:FileUpload ID="document1_file" runat="server" meta:resourcekey="photo_uploadResource1" CssClass="text_box" />
                                            </th>

                                        </tr>
                                    </table>
                                </div>
                                <asp:Panel ID="att_upload_panel" runat="server">
                                </asp:Panel>
                            </div>
                            <br />
                        </asp:Panel>

                        <br />
                        <div class="container-fluid">
                            <asp:Panel ID="panel12" runat="server">
                                <asp:GridView ID="gv_debit_pmt_details" class="table" runat="server" BackColor="White"
                                    BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnRowDataBound="gv_debit_pmt_RowDataBound" OnPreRender="gv_debit_pmt_details_PreRender">
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
                                        <asp:TemplateField HeaderText="DELETE">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="LinkButton3" runat="server" CausesValidation="false" CommandArgument='<%# Eval("ID") + "," + Eval("annuxure_file") %>' OnCommand="lnkpmtDelete_Command" OnClientClick="return confirm('Are you sure You want to  Delete ?')"><img alt="" height="15" style = "margin-left: 23px;" src="Images/delete_icon.png" width="15" /></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="DOWNLOAD FILE">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="LinkButton2" runat="server" Style="color: white" CausesValidation="false" CssClass="btn btn-primary" Text="DOWNLOAD" CommandArgument='<%# Eval("annuxure_file")%>' OnCommand="lnkpmtDownload_Command"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                        </div>
                    </div>
                    <div id="menu4">
                        <div class="container-fluid" style="background: white; border-radius: 10px; margin-top: 20px; border: 1px solid white; padding: 20px 20px 20px 20px">

                            <asp:Panel ID="panel10" runat="server" ScrollBars="Auto" CssClass="grid-view">
                                <asp:GridView ID="grv_saleentery" class="table" runat="server" BackColor="White"
                                    BorderColor="#CCCCCC" BorderStyle="None" CellPadding="3" Width="100%" Font-Size="X-Small"
                                    OnPreRender="gv_minibank_menu4">
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
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>

                        </div>

                    </div>
                    <div id="menu5">
                        <%--  <asp:UpdatePanel ID="update5" runat="server">
                            <ContentTemplate>--%>
                        <div class="container-fluid">
                            <div class="row">
                                <br />
                                <div class="col-sm-2 col-xs-12 ">
                                    <b>Client Name :</b>
                                    <asp:DropDownList ID="ddl_upload_lg_client" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_upload_lg_client_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                                <div class=" col-sm-4 col-xs-12" style="margin-top: 9px;">
                                    <b>File :</b><span class="text-red">*</span>
                                    <asp:FileUpload ID="FileUpload1" runat="server" />
                                </div>

                                <div class=" col-sm-2 col-xs-12" style="margin-top: 14px;">
                                    <asp:Button ID="btn_upload" runat="server" CssClass="btn btn-primary convience"
                                        Text="Process" OnClick="btn_upload_Click" OnClientClick="return Req_validation();" />
                                    <%-- </div>
                                 
                             <div class=" col-sm-4 col-xs-12"> style="margin-top: 14px;">--%>

                                    <asp:Button ID="btn_report" runat="server" CssClass="btn btn-primary convience"
                                        Text="Report" OnClick="btn_report_Click" OnClientClick="return Req_report_validation();" />

                                </div>
                            </div>
                            <br />
                            <br />
                            <div class="container">
                                <br />
                                <asp:Panel runat="server" ID="Panel14" class="panel panel-primary" Style="background: white; padding: 25px 25px 25px 25px; border: 1px solid #e2e2dd; margin-bottom: 20px; margin-top: 20px; border-radius: 10px">
                                    <br />

                                    <div class="container">
                                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col-sm-1 col-xs-12" style="margin-top: 16px;"></div>
                                                    <div class="col-sm-2 col-xs-12">
                                                        <b>Batch ID :</b>
                                                        <asp:TextBox ID="txt_batchid" runat="server" class="form-control" AutoPostBack="true" onkeypress="return isNumber(event)"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2 col-xs-12">
                                                        <b>state :</b>
                                                        <asp:TextBox ID="txt_state" runat="server" class="form-control" AutoPostBack="true" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2 col-xs-12">
                                                        <b>Invoice No :</b>
                                                        <asp:TextBox ID="txt_invoice" runat="server" class="form-control" AutoPostBack="true" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2 col-xs-12">
                                                        <b>Invoice Date :</b>
                                                        <asp:TextBox ID="txt_date_invoice" runat="server" class="form-control" AutoPostBack="true" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2 col-xs-12">
                                                        <b>Payment :</b>
                                                        <asp:TextBox ID="txt_payment" runat="server" class="form-control" AutoPostBack="true" onkeypress="return isNumber(event)"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <br />
                                        <div class="row">
                                            <div class="col-sm-1 col-xs-12" style="margin-top: 16px;"></div>

                                            <div class="col-sm-2 col-xs-12">
                                                <b>Payment Date :</b>
                                                <asp:TextBox ID="txt_date_payment" runat="server" class="form-control date-picker1" AutoPostBack="true"> </asp:TextBox>
                                            </div>
                                            <div class="col-sm-2 col-xs-12">
                                                <b>Transaction Ref :</b>
                                                <asp:TextBox ID="txt_ref" runat="server" class="form-control" AutoPostBack="true" onkeypress="return isNumber(event)"> </asp:TextBox>
                                            </div>
                                            <div class="col-sm-2 col-xs-12">
                                                <b>TDS Deduction :</b>
                                                <asp:TextBox ID="txt_deduction" runat="server" class="form-control" AutoPostBack="true" ReadOnly="true"> </asp:TextBox>
                                            </div>

                                            <%-- <div class="col-sm-2 col-xs-12" ></div>--%>
                                            <div class="col-sm-2 col-xs-12">
                                                <b>Remark:</b>
                                                <asp:TextBox ID="txt_remark" runat="server" class="form-control" MaxLength="255">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <br />
                                        <br />
                                        <div class="row text-center">
                                            <%--<div class="col-sm-2 col-xs-12">
                                                        Select Month :
                                <asp:TextBox ID="txt_minibank_month" runat="server" CssClass="form-control date-picker"></asp:TextBox>
                                                    </div>--%>
                                            <asp:Button ID="submit_btn" runat="server" class="btn btn-primary text-center" OnClick="submit_btn_Click" OnClientClick="return  req_val();" Text="Update" />
                                            <asp:Button ID="close" runat="server" Text="Close" OnClick="close_Click" class="btn btn-danger" />
                                        </div>

                                        <%--   </ContentTemplate>--%>
                                        <%--</asp:UpdatePanel>--%>
                                    </div>
                                    <br />
                                </asp:Panel>
                            </div>
                            <br />
                            <br />

                            <div class="container-fluid">
                                <asp:Panel ID="panel11" runat="server" CssClass="grid-view" Style="overflow-x: auto">
                                    <div class="row">
                                        <div class="col-sm-10 col-xs-12"></div>
                                        <div class="col-sm-2 col-xs-12">
                                            <b>Search :</b>
                                            <asp:TextBox runat="server" ID="txt_search" CssClass=" form-control" onkeyup="Search_Gridview(this)" />
                                        </div>
                                    </div>
                                    <br />
                                    <asp:GridView ID="payment_gridview" class="table" runat="server" BackColor="White" OnSelectedIndexChanged="payment_gridview_SelectedIndexChanged" OnRowDataBound="payment_gridview_RowDataBound1"
                                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnPreRender="payment_gridview_PreRender1">
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
                                            <%--    <asp:BoundField DataField="batchid" HeaderText="batch id" SortExpression="batchid" />
                                            <asp:BoundField DataField="state" HeaderText="state" SortExpression="state" />
                                            <asp:BoundField DataField="invoice_no" HeaderText="invoice_no" SortExpression="invoice_no" />
                                            <asp:BoundField DataField="invoice_date" HeaderText="invoice_date" SortExpression="invoice_date" />
                                            <asp:BoundField DataField="payment" HeaderText="payment" SortExpression="payment" />
                                            <asp:BoundField DataField="payment_date" HeaderText="payment_date" SortExpression="payment_date" />
                                             <asp:BoundField DataField="transaction_ref" HeaderText="transaction ref" SortExpression="transaction_ref" />
                                            <asp:BoundField DataField="tds_deducted" HeaderText="tds_deducted" SortExpression="tds_deducted" />
                                            <asp:BoundField DataField="remarks" HeaderText="remarks" SortExpression="remarks" />--%>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </div>
                        </div>

                        <br />
                        <%-- </ContentTemplate>
                        </asp:UpdatePanel>--%>
                    </div>
                    <div id="menu6">
                        <div class="container-fluid">
                            <asp:Panel runat="server" ID="Panel9" class="panel panel-primary" Style="border-color: white; background: white">

                                <div class="row">
                                    <br />
                                    <div class="col-sm-3 col-md-offset-1 ">
                                        <b>Client Name :</b>
                                        <asp:DropDownList ID="ddl_client_outstanding" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_client_outstanding_SelectedIndexChanged" >
                                        </asp:DropDownList>
                                          <div style="font-size:x-large; color:blue">
                                        <asp:Literal ID="lit_outstanding_amt" runat="server"></asp:Literal>
                                        </div>

                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>TO MAIL ID:</b>
                                        <span id="lblError" style="color: red"></span>
                                        <asp:TextBox ID="txt_to_mail" runat="server"  onkeyup="ValidateEmailID(this);" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>CC MAIL ID:</b>
                                        <asp:TextBox ID="txt_cc_mail" runat="server" TextMode="MultiLine"  CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class=" col-sm-2 col-xs-12" style="margin-top: 14px;">
                                        <asp:Button ID="btn_view_outstanding" runat="server" CssClass="btn btn-block btn-primary convience" Text="Check Outstanding" OnClick="btn_view_outstanding_Click" OnClientClick="return view_outstanding_validation();" />
                                    </div>
                                    <%--   <div class=" col-sm-4 col-xs-12"> style="margin-top: 14px;">--%>
                                    <div class=" col-sm-1 col-xs-12" style="margin-top: 14px;">
                                        <asp:Button ID="btn_send_mail" runat="server" CssClass="btn btn-block btn-primary" Text="Send Mail" OnClick="btn_send_mail_Click" OnClientClick="return send_mail_validation();" Style="width: auto" />
                                    </div>
                                    <br />
                                    <br />
                                </div>
                                <br />
                                <br />
                            </asp:Panel>
                            <asp:Panel ID="panel7" runat="server" CssClass="grid-view" Style="overflow-x: auto;" Visible="false">
                                <h4 class="text-center">Outstanding</h4>
                                <div class="container-fluid">
                                    <asp:GridView ID="gv_outstanding" class="table" runat="server" Font-Size="X-Small" BackColor="White"
                                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnPreRender="gv_outstanding_PreRender">
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
                                            <%--   <asp:TemplateField HeaderText="DELETE" ControlStyle-Width="120px" Visible="false">
                                            <ItemStyle Width="20px" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="false" AutoPostBack="true" Width="120%" CommandArgument='<%# Eval("ID")%>' OnCommand="lnkminiDelete_Command" OnClientClick="return confirm('Are you sure You want to  Delete ?')"><img alt="" height="15" style = "margin-left: 23px;" src="Images/delete_icon.png" width="15" /></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="DOWNLOAD FILE" ControlStyle-Width="120px">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_download" runat="server" Style="color: white" CausesValidation="false" Width="120%" CssClass="btn btn-primary" Text="DOWNLOAD" CommandArgument='<%# Eval("upload_file")%>' OnCommand="lnk_download_Command"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Edit" ControlStyle-Width="120px" Visible="false">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btn_edit_other1" Text="Edit" runat="server" CssClass="btn btn-primary" Style="color: white" Width="80%" OnClick="btn_edit_other1_Click"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="receipt_approve" HeaderText="Status" SortExpression="receipt_approve" />--%>
                                        </Columns>
                                    </asp:GridView>
                                    <br />
                                </div>
                            </asp:Panel>

                        </div>
                        <br />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>






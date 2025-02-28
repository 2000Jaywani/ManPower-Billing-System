<%@ Page Language="C#" AutoEventWireup="true" Codefile="~/approval_request_form.cs" MasterPageFile="~/MasterPage.master" Inherits="approval_request_form" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Approval Form</title>
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
    <%--<script src="datatable/jquery-1.12.3.js"></script>--%>
    <script src="datatable/jquery.dataTables.min.js"></script>
    <script src="datatable/dataTables.bootstrap.min.js"></script>
    <script src="datatable/dataTables.buttons.min.js"></script>
    <script src="datatable/buttons.bootstrap.min.js"></script>
    <%-- <script src="datatable/jszip.min.js"></script>--%>
    <%-- <script src="datatable/pdfmake.min.js"></script>--%>
    <script src="datatable/vfs_fonts.js"></script>
    <script src="datatable/buttons.html5.min.js"></script>
    <script src="datatable/buttons.print.min.js"></script>
    <script src="datatable/buttons.colVis.min.js"></script>
    <script src="datatable/pdfmake.min.js"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            $(document).on("Keyup", function () {
                SearchGrid('<%=txt_search_material.ClientID%>', '<%=gv_material_dispatch.ClientID%>');
            });

            $('[id*=chk_header_material]').click(function () {
                $("[id*='chk_record_material']").attr('checked', this.checked);
            });


          });


        $(document).ready(function () {
            $(document).on("Keyup", function () {
                SearchGrid('<%=txt_search_invoice.ClientID%>', '<%=gv_invoice_dispatch.ClientID%>');
            });
            
            $('[id*=chk_header_invoice]').click(function () {
                $("[id*='chk_record_invoice']").attr('checked', this.checked);
            });
        });


        $(document).ready(function () {
            $(document).on("Keyup", function () {
                SearchGrid('<%=txt_search_dublicate_id.ClientID%>', '<%=gv_dublicate_id_card.ClientID%>');
            });

            $('[id*=chk_header_dublicate]').click(function () {
                $("[id*='chk_record_dublicate']").attr('checked', this.checked);
            });
        });


        function pageLoad() {
            var txt_date_request = document.getElementById('<%=txt_from_date.ClientID %>');
            $('.date-picker').datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',

                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-date .ui-datepicker-month .ui-datepicker-year :selected").val();

                }
            });
            $('.date-picker1').datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',

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

            $(".date-pickerk").attr("readonly", "true");


            $('.date-picker2').datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',

                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-date .ui-datepicker-month .ui-datepicker-year :selected").val();

                }
            });

            $(".date-picker1").attr("readonly", "true");
            $(".date-picker2").attr("readonly", "true");

        }
        $(document).ready(function () {
            $(document).on("Keyup", function () {
                SearchGrid('<%=txt_search.ClientID%>', '<%=gv_attendance.ClientID%>');
                });


                $('[id*=chk_gv_header]').click(function () {
                    $("[id*='chk_client']").attr('checked', this.checked);
                });


        });


        $(document).ready(function () {
            $(document).on("Keyup", function () {
                SearchGrid('<%=txt_search1.ClientID%>', '<%=gv_Leave.ClientID%>');
            });


             $('[id*=chk_gv_header1]').click(function () {
                 $("[id*='chk_client1']").attr('checked', this.checked);
             });


         });


            $(document).ready(function () {
                $(document).on("Keyup", function () {
                    SearchGrid('<%=txt_search.ClientID%>', '<%=gv_attendance.ClientID%>');
                 });


                 $('[id*=chk_gv_header3]').click(function () {
                     $("[id*='chk_client_material']").attr('checked', this.checked);
                 });


            });



        function Search_Gridview_material(strKey) {
            var strData = strKey.value.toLowerCase().split(" ");
            var tblData = document.getElementById("<%=gv_material_dispatch.ClientID %>");
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

        function Search_Gridview_invoice(strKey) {
            var strData = strKey.value.toLowerCase().split(" ");
            var tblData = document.getElementById("<%=gv_invoice_dispatch.ClientID %>");
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



        function Search_Gridview_dublicate(strKey) {
            var strData = strKey.value.toLowerCase().split(" ");
            var tblData = document.getElementById("<%=gv_dublicate_id_card.ClientID %>");
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





        function approve_record() {

            var ddl_client_request = document.getElementById('<%=ddl_client_request.ClientID %>');
            var select_ddl_client_request = ddl_client_request.options[ddl_client_request.selectedIndex].text;

            
            if (select_ddl_client_request == "Select") {
                alert("Please Select Request For");
                ddl_client_request.focus();
                return false;
            }


            var r = confirm("Are you Sure You Want to Approve This Record");


            if (r == true) {


                if (select_ddl_client_request == "Attendance") {

                    var isValid_re = false; {

                        var gridView3 = document.getElementById('<%= gv_attendance.ClientID %>');
                    for (var i = 1; i < gridView3.rows.length; i++) {
                        var inputs = gridView3.rows[i].getElementsByTagName('input');
                        if (inputs != null) {
                            if (inputs[0].type == "checkbox") {
                                if (inputs[0].checked) {
                                    isValid_re = true;
                                    return true;
                                }
                            }
                        }
                    }
                    alert("Please select atleast one Record ");
                    return false;

                }



            }
            else
                if (select_ddl_client_request == "Leave") {

                    var isValid_re = false; {

                        var gridView3 = document.getElementById('<%= gv_Leave.ClientID %>');
                        for (var i = 1; i < gridView3.rows.length; i++) {
                            var inputs = gridView3.rows[i].getElementsByTagName('input');
                            if (inputs != null) {
                                if (inputs[0].type == "checkbox") {
                                    if (inputs[0].checked) {
                                        isValid_re = true;
                                        return true;
                                    }
                                }
                            }
                        }
                        alert("Please select at least one Record ");
                        return false;

                    }

                }




                ($.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } }))
                return true;
            }
            else {
                return false;
            }


  }

        function validation_show_btn()
        {
            var txt_from_date = document.getElementById('<%=txt_from_date.ClientID %>');
            if (txt_from_date.value == "") {
                alert("Please Enter From Date");
                txt_from_date.focus();
                return false;
            }

            var txt_to_date = document.getElementById('<%=txt_to_date.ClientID %>');
            if (txt_to_date.value == "") {
                alert("Please Enter To Date");
                txt_to_date.focus();
                return false;
            }

            var ddl_type = document.getElementById('<%=ddl_type.ClientID %>');
            var select_ddl_type = ddl_type.options[ddl_type.selectedIndex].text;


            if (select_ddl_type == "Select") {
                alert("Please Select Material Type");
                ddl_type.focus();
                return false;
            }

            return true;
        }

        function rejected_request_function()
        {
            var ddl_type = document.getElementById('<%=ddl_type.ClientID %>');
            var select_ddl_type = ddl_type.options[ddl_type.selectedIndex].text;


            var r = confirm("Are you Sure You Want to Reject This Record");


            if (r == true) {


                if (select_ddl_type == "Material") {

                    var isValid_re = false; {

                        var gridView3 = document.getElementById('<%= gv_material_dispatch.ClientID %>');
                        for (var i = 1; i < gridView3.rows.length; i++) {
                            var inputs = gridView3.rows[i].getElementsByTagName('input');
                            if (inputs != null) {
                                if (inputs[0].type == "checkbox") {
                                    if (inputs[0].checked) {
                                        isValid_re = true;
                                        return true;
                                    }
                                }
                            }
                        }
                        alert("Please select atleast one Record ");
                        return false;

                    }



                }
                else
                    if (select_ddl_type == "Invoice") {

                        var isValid_re = false; {

                            var gridView3 = document.getElementById('<%= gv_invoice_dispatch.ClientID %>');
                            for (var i = 1; i < gridView3.rows.length; i++) {
                                var inputs = gridView3.rows[i].getElementsByTagName('input');
                                if (inputs != null) {
                                    if (inputs[0].type == "checkbox") {
                                        if (inputs[0].checked) {
                                            isValid_re = true;
                                            return true;
                                        }
                                    }
                                }
                            }
                            alert("Please select at least one Record ");
                            return false;

                        }

                    }
                    else
                        if (select_ddl_type == "DuplicateID-Card") {

                            var isValid_re = false; {

                                var gridView3 = document.getElementById('<%= gv_dublicate_id_card.ClientID %>');
                                    for (var i = 1; i < gridView3.rows.length; i++) {
                                        var inputs = gridView3.rows[i].getElementsByTagName('input');
                                        if (inputs != null) {
                                            if (inputs[0].type == "checkbox") {
                                                if (inputs[0].checked) {
                                                    isValid_re = true;
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                    alert("Please select at least one Record ");
                                    return false;

                                }

                            }

                    ($.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } }))
                    return true;
                }
            //else {
            //    return false;
            //}





        }

        function hold_request_validation()
        {
            var ddl_type = document.getElementById('<%=ddl_type.ClientID %>');
            var select_ddl_type = ddl_type.options[ddl_type.selectedIndex].text;


            var r = confirm("Are you Sure You Want to Hold This Record");


            if (r == true) {


                if (select_ddl_type == "Material") {

                    var isValid_re = false; {

                        var gridView3 = document.getElementById('<%= gv_material_dispatch.ClientID %>');
                        for (var i = 1; i < gridView3.rows.length; i++) {
                            var inputs = gridView3.rows[i].getElementsByTagName('input');
                            if (inputs != null) {
                                if (inputs[0].type == "checkbox") {
                                    if (inputs[0].checked) {
                                        isValid_re = true;
                                        return true;
                                    }
                                }
                            }
                        }
                        alert("Please select atleast one Record ");
                        return false;

                    }



                }
                else
                    if (select_ddl_type == "Invoice") {

                        var isValid_re = false; {

                            var gridView3 = document.getElementById('<%= gv_invoice_dispatch.ClientID %>');
                                for (var i = 1; i < gridView3.rows.length; i++) {
                                    var inputs = gridView3.rows[i].getElementsByTagName('input');
                                    if (inputs != null) {
                                        if (inputs[0].type == "checkbox") {
                                            if (inputs[0].checked) {
                                                isValid_re = true;
                                                return true;
                                            }
                                        }
                                    }
                                }
                                alert("Please select at least one Record ");
                                return false;

                            }

                        }
                        else
                            if (select_ddl_type == "DuplicateID-Card") {

                                var isValid_re = false; {

                                    var gridView3 = document.getElementById('<%= gv_dublicate_id_card.ClientID %>');
                                for (var i = 1; i < gridView3.rows.length; i++) {
                                    var inputs = gridView3.rows[i].getElementsByTagName('input');
                                    if (inputs != null) {
                                        if (inputs[0].type == "checkbox") {
                                            if (inputs[0].checked) {
                                                isValid_re = true;
                                                return true;
                                            }
                                        }
                                    }
                                }
                                alert("Please select at least one Record ");
                                return false;

                            }

                        }

                ($.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } }))
                return true;
            }
            //else {
            //    return false;
            //}

        }



        function download_request_validation()
        {
            var ddl_type = document.getElementById('<%=ddl_type.ClientID %>');
            var select_ddl_type = ddl_type.options[ddl_type.selectedIndex].text;


            var r = confirm("Are you Sure You Want to Download This Record");


            if (r == true) {


                if (select_ddl_type == "Material") {

                    var isValid_re = false; {

                        var gridView3 = document.getElementById('<%= gv_material_dispatch.ClientID %>');
                        for (var i = 1; i < gridView3.rows.length; i++) {
                            var inputs = gridView3.rows[i].getElementsByTagName('input');
                            if (inputs != null) {
                                if (inputs[0].type == "checkbox") {
                                    if (inputs[0].checked) {
                                        isValid_re = true;
                                        return true;
                                    }
                                }
                            }
                        }
                        alert("Please select atleast one Record ");
                        return false;

                    }



                }
                else
                    if (select_ddl_type == "Invoice") {

                        var isValid_re = false; {

                            var gridView3 = document.getElementById('<%= gv_invoice_dispatch.ClientID %>');
                            for (var i = 1; i < gridView3.rows.length; i++) {
                                var inputs = gridView3.rows[i].getElementsByTagName('input');
                                if (inputs != null) {
                                    if (inputs[0].type == "checkbox") {
                                        if (inputs[0].checked) {
                                            isValid_re = true;
                                            return true;
                                        }
                                    }
                                }
                            }
                            alert("Please select at least one Record ");
                            return false;

                        }

                    }
                    else
                        if (select_ddl_type == "DuplicateID-Card") {

                            var isValid_re = false; {

                                var gridView3 = document.getElementById('<%= gv_dublicate_id_card.ClientID %>');
                                    for (var i = 1; i < gridView3.rows.length; i++) {
                                        var inputs = gridView3.rows[i].getElementsByTagName('input');
                                        if (inputs != null) {
                                            if (inputs[0].type == "checkbox") {
                                                if (inputs[0].checked) {
                                                    isValid_re = true;
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                    alert("Please select at least one Record ");
                                    return false;

                                }

                            }

                    ($.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } }))
                    return true;
                }
            //else {
            //    return false;
            //}

            }

        function approval_request_validation()
        {
            var txt_dispatch_date = document.getElementById('<%=txt_dispatch_date.ClientID %>');
            if (txt_dispatch_date.value == "") {
                alert("Please Enter Dispatch Date");
                txt_dispatch_date.focus();
                return false;
            }



            var ddl_type = document.getElementById('<%=ddl_type.ClientID %>');
            var select_ddl_type = ddl_type.options[ddl_type.selectedIndex].text;


            var r = confirm("Are you Sure You Want to Approve This Record");


            if (r == true) {


                if (select_ddl_type == "Material") {

                    var isValid_re = false; {

                        var gridView3 = document.getElementById('<%= gv_material_dispatch.ClientID %>');
                            for (var i = 1; i < gridView3.rows.length; i++) {
                                var inputs = gridView3.rows[i].getElementsByTagName('input');
                                if (inputs != null) {
                                    if (inputs[0].type == "checkbox") {
                                        if (inputs[0].checked) {
                                            isValid_re = true;
                                            return true;
                                        }
                                    }
                                }
                            }
                            alert("Please select atleast one Record ");
                            return false;

                        }



                    }
                    else
                    if (select_ddl_type == "Invoice") {

                            var isValid_re = false; {

                                var gridView3 = document.getElementById('<%= gv_invoice_dispatch.ClientID %>');
                            for (var i = 1; i < gridView3.rows.length; i++) {
                                var inputs = gridView3.rows[i].getElementsByTagName('input');
                                if (inputs != null) {
                                    if (inputs[0].type == "checkbox") {
                                        if (inputs[0].checked) {
                                            isValid_re = true;
                                            return true;
                                        }
                                    }
                                }
                            }
                            alert("Please select at least one Record ");
                            return false;

                        }

                    }
                    else
                        if (select_ddl_type == "DuplicateID-Card") {

                            var isValid_re = false; {

                                var gridView3 = document.getElementById('<%= gv_dublicate_id_card.ClientID %>');
                                for (var i = 1; i < gridView3.rows.length; i++) {
                                    var inputs = gridView3.rows[i].getElementsByTagName('input');
                                    if (inputs != null) {
                                        if (inputs[0].type == "checkbox") {
                                            if (inputs[0].checked) {
                                                isValid_re = true;
                                                return true;
                                            }
                                        }
                                    }
                                }
                                alert("Please select at least one Record ");
                                return false;

                            }

                        }

                ($.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } }))
                return true;
            }
            //else {
            //    return false;
            //}



        }



        function upload_request_function() {


            var client_request_upload = document.getElementById('<%=client_request_upload.ClientID %>');
            if (client_request_upload.value == "") {
                alert("Please Upload File");
                client_request_upload.focus();
                return false;
            }



            var ddl_type = document.getElementById('<%=ddl_type.ClientID %>');
               var select_ddl_type = ddl_type.options[ddl_type.selectedIndex].text;

                   if (select_ddl_type == "Material") {

                       var isValid_re = false; {

                           var gridView3 = document.getElementById('<%= gv_material_dispatch.ClientID %>');
                        for (var i = 1; i < gridView3.rows.length; i++) {
                            var inputs = gridView3.rows[i].getElementsByTagName('input');
                            if (inputs != null) {
                                if (inputs[0].type == "checkbox") {
                                    if (inputs[0].checked) {
                                        isValid_re = true;
                                        return true;
                                    }
                                }
                            }
                        }
                        alert("Please select atleast one Record ");
                        return false;

                    }



                }
                else
                    if (select_ddl_type == "Invoice") {

                        var isValid_re = false; {

                            var gridView3 = document.getElementById('<%= gv_invoice_dispatch.ClientID %>');
                            for (var i = 1; i < gridView3.rows.length; i++) {
                                var inputs = gridView3.rows[i].getElementsByTagName('input');
                                if (inputs != null) {
                                    if (inputs[0].type == "checkbox") {
                                        if (inputs[0].checked) {
                                            isValid_re = true;
                                            return true;
                                        }
                                    }
                                }
                            }
                            alert("Please select at least one Record ");
                            return false;

                        }

                    }
                    else
                        if (select_ddl_type == "DuplicateID-Card") {

                            var isValid_re = false; {

                                var gridView3 = document.getElementById('<%= gv_dublicate_id_card.ClientID %>');
                                for (var i = 1; i < gridView3.rows.length; i++) {
                                    var inputs = gridView3.rows[i].getElementsByTagName('input');
                                    if (inputs != null) {
                                        if (inputs[0].type == "checkbox") {
                                            if (inputs[0].checked) {
                                                isValid_re = true;
                                                return true;
                                            }
                                        }
                                    }
                                }
                                alert("Please select at least one Record ");
                                return false;

                            }

                        }

                ($.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } }))
                return true;
            
               //else {
               //    return false;
               //}





        }




        function leave_record() {

            var ddl_client_request = document.getElementById('<%=ddl_client_request.ClientID %>');
            var select_ddl_client_request = ddl_client_request.options[ddl_client_request.selectedIndex].text;


            if (select_ddl_client_request == "Select") {
                alert("Please Select Request For");
                ddl_client_request.focus();
                return false;
            }


            var re = confirm("Are you Sure You Want to Reject This Record");

            if (re == true) {


                if (select_ddl_client_request == "Attendance") {

                    var isValid_re = false; {

                        var gridView3 = document.getElementById('<%= gv_attendance.ClientID %>');
                        for (var i = 1; i < gridView3.rows.length; i++) {
                            var inputs = gridView3.rows[i].getElementsByTagName('input');
                            if (inputs != null) {
                                if (inputs[0].type == "checkbox") {
                                    if (inputs[0].checked) {
                                        isValid_re = true;
                                        return true;
                                    }
                                }
                            }
                        }
                        alert("Please select atleast one Record ");
                        return false;

                    }



                }
                else
                    if (select_ddl_client_request == "Leave") {

                        var isValid_re = false; {

                            var gridView3 = document.getElementById('<%= gv_Leave.ClientID %>');
                        for (var i = 1; i < gridView3.rows.length; i++) {
                            var inputs = gridView3.rows[i].getElementsByTagName('input');
                            if (inputs != null) {
                                if (inputs[0].type == "checkbox") {
                                    if (inputs[0].checked) {
                                        isValid_re = true;
                                        return true;
                                    }
                                }
                            }
                        }
                        alert("Please select at least one Record ");
                        return false;

                    }

                }


            ($.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } }))
            return true;
        }
        else {
            return false;
        }


        }
        $(document).ready(function () {
            var st = $(this).find("input[id*='hidtab']").val();
            if (st == null)
                st = 0;
            $('[id$=tabs]').tabs({ selected: st });

            $('[id$=Div1]').tabs({ selected: st });

        });




    </script>


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

        .grid-view {
            height: auto;
            max-height: 400px;
            overflow-x: hidden;
            overflow-y: auto;
        }

        h5 {
            font-weight: bold;
            font-size: 15px;
        }

        .row {
            margin: 0px;
        }



        .modal {
            display: none;
            position: absolute;
            top: 0px;
            left: 0px;
            background-color: black;
            z-index: 100;
            opacity: 0.8;
            filter: alpha(opacity=60);
            -moz-opacity: 0.8;
            min-height: 100%;
        }

        #divImage {
            display: none;
            z-index: 1000;
            position: fixed;
            top: 0;
            left: 0;
            background-color: White;
            height: 550px;
            width: 600px;
            padding: 3px;
            border: solid 1px black;
        }

        .Hide {
            display: none;
        }
    </style>

    <script type="text/javascript">
        function openWindow() {
            window.open("html/Android.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
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
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>Approval Form</b></div>
                    </div>
                    <div class="col-sm-2 text-right">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
          
               </asp:Panel>
             <br />
             <br />

        <div id="tabs" style="background: #f3f1fe; padding:20px 20px 20px 20px; border: 1px solid #e2e2dd; margin:15px 15px 15px 15px; border-radius:10px">
                <asp:HiddenField ID="hidtab" Value="0" runat="server" />
                <ul>
                    <li><a id="A2" href="#menu0" runat="server"><b>Request</b></a></li>

                    <li><a href="#menu1" id="A1" runat="server" >Dispatch</a></li>
                </ul>
                <div id="menu0">
                    <div class="container-fluid">
                               <div class="row">
                                     <div class="row">

                 
                 <div class="col-sm-2 col-xs-12">
                                     <b> Request For:</b><span class="text-red"> *</span>
                                      <asp:DropDownList ID="ddl_client_request" runat="server" OnSelectedIndexChanged="ddl_client_request_SelectedIndexChanged" class="form-control" AutoPostBack="true">
                                          <asp:ListItem Value="Select">Select</asp:ListItem>
                                          <asp:ListItem Value="1">Attendance</asp:ListItem>
                                           <asp:ListItem Value="2">Leave</asp:ListItem>
                                      </asp:DropDownList>
                                  </div>

                 </div>

             <br />
             <br />

          
             <div class="container-fluid" style="background: #f3f1fe; border-radius: 10px; border: 1px solid white; padding:20px 20px 20px 20px; margin-left:-10px; margin-right:-10px">
            <asp:Panel ID="Panel26" runat="server" CssClass="grid-view">

                 <div class="row">
                                <div class="col-sm-10 col-xs-12"></div>
                                <div class="col-sm-2 col-xs-12">
                                    <b> Search :</b>
                        <asp:TextBox runat="server" ID="txt_search" CssClass=" form-control" onkeyup="Search_Gridview(this)" />
                                </div>
                            </div>

                 <br />

                                    <asp:GridView ID="gv_attendance" runat="server" AutoGenerateColumns="false" BackColor="White" BorderColor="#CCCCCC"  OnRowDataBound="gv_attendance_RowDataBound" OnPreRender="gv_attendance_PreRender"  BorderStyle="None" BorderWidth="1px" CellPadding="3" class="table"  Width="100%" >
                                        <FooterStyle BackColor="White" ForeColor="#004C99" />
                                        <SelectedRowStyle BackColor="#d1ddf1" Font-Bold="True" ForeColor="#333333" />
                                        <AlternatingRowStyle BackColor="White" />
                                        <HeaderStyle BackColor="#224173" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#224173" ForeColor="White" HorizontalAlign="Center" />
                                        <RowStyle BackColor="#EFF3FB" />
                                        <EditRowStyle BackColor="#2461BF" />
                                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                        <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                        <SortedDescendingHeaderStyle BackColor="#00547E" />
                                        <Columns>
                                          
                                            

                                            <asp:TemplateField HeaderText="Sr No.">
                                                <ItemStyle Width="20px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_srnumber" runat="server" Text="<%# Container.DataItemIndex+1 %>" Width="20px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                             <asp:BoundField DataField="id" HeaderText="id" SortExpression="id" />

                                               <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chk_gv_header" runat="server" Text=" SELECT"  />

                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chk_client" runat="server" CssClass="center-block" />
                                            </ItemTemplate>
                                        </asp:TemplateField> 

                                            <asp:TemplateField HeaderText="REJECT REASON">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_reject_reason_attendance" runat="server" CssClass="form-control" Text='<%# Eval("reject_reason")%>' Width="150" onkeypress="return isNumberKey(event,this.id)"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                            <asp:BoundField DataField="emp_code" HeaderText="emp_code" SortExpression="emp_code" />
                                            <asp:BoundField DataField="emp_name" HeaderText="Employee Name" SortExpression="emp_name" />
                                            <asp:BoundField DataField="client_request_for" HeaderText="Request For" SortExpression="client_request_for" />
                                            <asp:BoundField DataField="attendance_date" HeaderText="Attendance Date" SortExpression="attendance_date" />
                                            <asp:BoundField DataField="comment_box" HeaderText="Comment" SortExpression="comment_box" />
                                               <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" />
                                             <asp:BoundField DataField="approval_date_on" HeaderText="Approval Date On" SortExpression="approval_date_on" />
                                          
                                        
                                            
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                 </div>
              
         <asp:Panel ID="Panel4" runat="server">
               <asp:Panel ID="Panel1" runat="server" CssClass="grid-view" >


                   
                                              <div class="row">
                                <div class="col-sm-10 col-xs-12"></div>
                                <div class="col-sm-2 col-xs-12">
                                   <b> Search :</b>
                        <asp:TextBox runat="server" ID="txt_search1" CssClass=" form-control" onkeyup="Search_Gridview(this)" />
                                </div>
                            </div>
                                         <br />   


                                    <asp:GridView ID="gv_Leave" runat="server" AutoGenerateColumns="false" BackColor="White" OnRowDataBound="gv_Leave_RowDataBound" OnPreRender="gv_Leave_PreRender" BorderColor="#CCCCCC"  BorderStyle="None" BorderWidth="1px" CellPadding="3" class="table"  Width="100%">
                                        <FooterStyle BackColor="White" ForeColor="#004C99" />
                                        <SelectedRowStyle BackColor="#d1ddf1" Font-Bold="True" ForeColor="#333333" />
                                        <AlternatingRowStyle BackColor="White" />
                                        <HeaderStyle BackColor="#224173" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#224173" ForeColor="White" HorizontalAlign="Center" />
                                        <RowStyle BackColor="#EFF3FB" />
                                        <EditRowStyle BackColor="#2461BF" />
                                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                        <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                        <SortedDescendingHeaderStyle BackColor="#00547E" />
                                        <Columns>
                                            

                                            <asp:TemplateField HeaderText="Sr No.">
                                                <ItemStyle Width="20px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="Label1" runat="server" Text="<%# Container.DataItemIndex+1 %>" Width="20px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:BoundField DataField="id" HeaderText="id" SortExpression="id" />

                                             <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chk_gv_header1" runat="server" Text="SELECT"  />

                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chk_client1" runat="server" CssClass="center-block" />
                                            </ItemTemplate>
                                        </asp:TemplateField> 

                                            <asp:TemplateField HeaderText="REJECT REASON">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_reject_reason_leave" runat="server" CssClass="form-control" Text='<%# Eval("reject_reason")%>' Width="150" onkeypress="return isNumberKey(event,this.id)"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                             <asp:BoundField DataField="emp_code" HeaderText="emp_code" SortExpression="emp_code" />
                                            <asp:BoundField DataField="emp_name" HeaderText="Employee Name" SortExpression="emp_name" />
                                            <asp:BoundField DataField="client_request_for" HeaderText=" Request For" SortExpression="client_request_for" />
                                             <asp:BoundField DataField="leave_from_date" HeaderText="From Date" SortExpression="leave_from_date" />
                                             <asp:BoundField DataField="leave_to_date" HeaderText="To Date" SortExpression="leave_to_date" />
                                            <asp:BoundField DataField="comment_box" HeaderText="Comment" SortExpression="comment_box" />
                                               <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" />
                                              <asp:BoundField DataField="approval_date_on" HeaderText="Approval Date On" SortExpression="approval_date_on" />
                                            
                                        
                                        
                                            
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
     </asp:Panel>

        


            <br />
             <br />

           <div class="row text-center">
                          <asp:Button ID="btn_approve" OnClick="btn_approve_Click" runat="server" Text="Approve" OnClientClick="return approve_record();" class="btn btn-primary"
                             />

                 <asp:Button ID="btn_reject" OnClick="btn_reject_Click" runat="server" Text="Reject" OnClientClick="return leave_record();" class="btn btn-primary"
                            />
           
                 <asp:Button ID="btncloseloewe" runat="server" class="btn btn-danger"
                       OnClick="btncloseloewe_Click" Text="Close" />
           </div>

                               </div></div>
                </div>
               <div id="menu1">
                
                       <br />
                               <div class="row">
                                    <asp:Panel ID="Dispatch_panel" runat="server" CssClass="grid-view">


                                          <div class="col-sm-2 col-xs-12">
                                        <b>From Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_from_date" runat="server" class="form-control date-picker2"></asp:TextBox>
                                    </div>


                                        
                                         <div class="col-sm-2 col-xs-12">
                                        <b>To Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_to_date" runat="server" class="form-control date-picker2"></asp:TextBox>
                                    </div>


                                  
                                     <div class="col-sm-2 col-xs-12" id="dispatch_date_panel" runat="server">
                                        <b>Dispatch Date :</b><span style="color: red">*</span>
                                        <asp:TextBox ID="txt_dispatch_date" runat="server" class="form-control date-picker2"></asp:TextBox>
                                    </div>

                                    


                <%-- <div class="col-sm-2 col-xs-12 ">
                                    <b> From Date:</b>    <span class="text-red">*</span>

                                      <asp:TextBox ID="txt_from" runat="server" class="form-control date-picker1"
                                          ></asp:TextBox>
                                  </div>

                 <div class="col-sm-2 col-xs-12 ">
                                    <b> To Date:</b>    <span class="text-red">*</span>

                                      <asp:TextBox ID="txt_to" runat="server" class="form-control date-picker2"
                                         ></asp:TextBox>
                                  </div>--%>
  <div class="col-sm-2 col-xs-12">
                                     <b> Material Type:</b><span class="text-red"> *</span>
                                      <asp:DropDownList ID="ddl_type" runat="server" class="form-control"  >
                                          <asp:ListItem Value="Select">Select</asp:ListItem>
                                          <asp:ListItem Value="1">Material</asp:ListItem>
                                           <asp:ListItem Value="2">Invoice</asp:ListItem>
                                          <asp:ListItem Value="3">DuplicateID-Card</asp:ListItem>
                                      </asp:DropDownList>
                                  </div>
                                        <br />
                                         <div class="col-sm-2 col-xs-12">
                                         <asp:Button ID="btn_show_req" CssClass="btn btn-primary" style="width:150px" runat="server" Text="Show Request " OnClientClick="return validation_show_btn();" OnClick="btn_show_req_Click" />
</div>



                        </asp:Panel>
                                   </div>
                      
                    
          
                           <div class="row"  id ="">
                                <div class="col-sm-10 col-xs-12"></div>
                                <div class="col-sm-2 col-xs-12" id="for_material_gv_search" runat="server">
                                    <b>Search :</b>
                        <asp:TextBox runat="server" ID="txt_search_material" CssClass=" form-control" onkeyup="Search_Gridview_material(this)" />
                                </div>
                            </div>
                       <asp:Panel ID="for_material_gv" runat="server" CssClass="panel panel-primary" Style="overflow:auto; border:none;">
                           <%--  <div class="container-fluid">--%>

                                     <asp:GridView ID="gv_material_dispatch" runat="server" AutoGenerateColumns="false"  CellPadding="1" ForeColor="#333333" OnPreRender="gv_material_dispatch_PreRender" OnRowDataBound="gv_material_dispatch_RowDataBound" Font-Size="X-Small" BackColor="White"   class="table"  Width="100%">
                                        <FooterStyle BackColor="White" ForeColor="#004C99"  />
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
                                                <ItemStyle Width="20px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="Label2" runat="server" Text="<%# Container.DataItemIndex+1 %>" Width="20px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>


                                             <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chk_header_material" runat="server" Text="Select Record"  />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chk_record_material" runat="server" CssClass="center-block" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                             <asp:BoundField DataField="id" HeaderText="id" SortExpression="id" />
                                            <asp:BoundField DataField="material_type" HeaderText="Material Type" SortExpression="material_type" />
                                            <asp:BoundField DataField="dispatch_through" HeaderText="Dispatch Through" SortExpression="dispatch_through" />
                                            <asp:BoundField DataField="client_code" HeaderText="Client Code" SortExpression="client_code" />
                                            <asp:BoundField DataField="client_name" HeaderText="Client Name" SortExpression="client_name" />
                                            <asp:BoundField DataField="state_name" HeaderText="State Name" SortExpression="state_name" />
                                             <asp:BoundField DataField="unit_code" HeaderText="Unit Code" SortExpression="unit_code" />
                                             <asp:BoundField DataField="unit_name" HeaderText="Location Name" SortExpression="unit_name" />
                                             <asp:BoundField DataField="emp_code" HeaderText="Emp Code" SortExpression="emp_code" />
                                            <asp:BoundField DataField="emp_name" HeaderText="Employee Name" SortExpression="emp_name" />
                                            <asp:BoundField DataField="material_dispatch_date" HeaderText="Requested On Date" SortExpression="material_dispatch_date" />
                                               <asp:BoundField DataField="material_uniform_size" HeaderText="Uniform Size" SortExpression="material_uniform_size" />
                                             <asp:BoundField DataField="material_uniform_set" HeaderText="Uniform Set" SortExpression="material_uniform_set" />
                                              <asp:BoundField DataField="material_shoes_size" HeaderText=" Shoes Size" SortExpression="material_shoes_size" />
                                              <asp:BoundField DataField="material_shoes_set" HeaderText="Shoes Set" SortExpression="material_shoes_set" />
                                             <asp:BoundField DataField="material_Receiver_name" HeaderText="Receiver Name" SortExpression="material_Receiver_name" />
                                              <asp:BoundField DataField="material_shipping_address" HeaderText="Shipping Address" ItemStyle-Width="15%" SortExpression="material_shipping_address" />
                                              <asp:BoundField DataField="requested_by" HeaderText="Requested By" SortExpression="requested_by" />
                                               <asp:BoundField DataField="approve_dispatch_date" HeaderText="Dispatch Date" SortExpression="approve_dispatch_date" />
                                                <asp:TemplateField HeaderText="Reason">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_hold_material" runat="server" CssClass="form-control" Text='<%# Eval("material_hold_reason")%>' Width="150" onkeypress="return isNumberKey(event,this.id)"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                  
                                        <asp:BoundField DataField="approve_record" HeaderText="Status" SortExpression="approve_record" />


                                             <asp:TemplateField HeaderText="Download/Status">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnk_material_download" runat="server" Width="100%" CausesValidation="false" Text="Download" Style="color: white" OnCommand="lnk_material_download_Command" CommandArgument='<%# Eval("client_re_upload")%>' CssClass="btn btn-primary"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                        
                                        
                                            
                                        </Columns>
                                    </asp:GridView>

                                 <%-- </div>--%>
                              </asp:Panel>
                         
                     <div class="row">
                                <div class="col-sm-10 col-xs-12"></div>
                                <div class="col-sm-2 col-xs-12" id="for_invoice_gv_search" runat="server">
                                  <b>  Search :</b>
                        <asp:TextBox runat="server" ID="txt_search_invoice" CssClass=" form-control" onkeyup="Search_Gridview_invoice(this)" />
                                </div>
                            </div>


                           <asp:Panel ID="for_invoice_gv" runat="server" CssClass="panel panel-primary" Style="overflow-x:auto; border:none;">
                           <%--  <div class="container-fluid">--%>

                                     <asp:GridView ID="gv_invoice_dispatch" runat="server"  AutoGenerateColumns="false" BackColor="White" OnRowDataBound="gv_invoice_dispatch_RowDataBound" OnSelectedIndexChanged="gv_invoice_dispatch_SelectedIndexChanged"  BorderColor="#CCCCCC"  BorderStyle="None" BorderWidth="1px" CellPadding="3" class="table"  Width="100%">
                                        <FooterStyle BackColor="White" ForeColor="#004C99" />
                                        <SelectedRowStyle BackColor="#d1ddf1" Font-Bold="True" ForeColor="#333333" />
                                        <AlternatingRowStyle BackColor="White" />
                                        <HeaderStyle BackColor="#224173" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#224173"  Height="20px" Width="20px" ForeColor="White" HorizontalAlign="Center" />
                                        <RowStyle BackColor="#EFF3FB" />
                                        <EditRowStyle BackColor="#2461BF" />
                                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                        <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                        <SortedDescendingHeaderStyle BackColor="#00547E" />
                                        <Columns>
                                          
                                            

                                            <asp:TemplateField HeaderText="Sr No.">
                                                <ItemStyle Width="20px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="Label3" runat="server" Text="<%# Container.DataItemIndex+1 %>" Width="20px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                             <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chk_header_invoice" runat="server" Text="Select Record"  />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chk_record_invoice" runat="server" CssClass="center-block" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                             <asp:BoundField DataField="id" HeaderText="id" SortExpression="id" />
                                            <asp:BoundField DataField="material_type" HeaderText="Material Type" SortExpression="material_type" />
                                            <asp:BoundField DataField="dispatch_through" HeaderText="Dispatch Through" SortExpression="dispatch_through" />
                                            <asp:BoundField DataField="client_code" HeaderText="Client Code" SortExpression="client_code" />
                                            <asp:BoundField DataField="client_name" HeaderText="Client Name" SortExpression="client_name" />
                                            <asp:BoundField DataField="state_name" HeaderText="State Name" SortExpression="state_name" />
                                             <asp:BoundField DataField="unit_code" HeaderText="Unit Code" SortExpression="unit_code" />
                                             <asp:BoundField DataField="invoice_branch_name" HeaderText="Location Name" SortExpression="invoice_branch_name" />
                                           
                                            <asp:BoundField DataField="material_dispatch_date" HeaderText="Requested On Date" SortExpression="material_dispatch_date" />
                                              <asp:BoundField DataField="month_year" HeaderText="Month Year" SortExpression="month_year" />
                                             
                                             <asp:BoundField DataField="material_person_name" HeaderText="Person Name" SortExpression="material_person_name" />
                                              <asp:BoundField DataField="material_shipping_address" ItemStyle-Width="15%" HeaderText="Shipping Address" SortExpression="material_shipping_address" />

                                              <asp:BoundField DataField="approve_dispatch_date" HeaderText="Dispatch Date" SortExpression="approve_dispatch_date" />
                                            <asp:BoundField DataField="requested_by" HeaderText="Requested By" SortExpression="requested_by" />

                                                <asp:TemplateField HeaderText="Reason">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_invoice_reason" runat="server" CssClass="form-control" Text='<%# Eval("material_hold_reason")%>' Width="150" onkeypress="return isNumberKey(event,this.id)"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                  
                                        <asp:BoundField DataField="approve_record" HeaderText="Status" SortExpression="approve_record" />

                                               <asp:TemplateField HeaderText="Download/Status">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnk_invoice_download" runat="server" Width="100%" CausesValidation="false" Text="Download" Style="color: white" OnCommand="lnk_invoice_download_Command" CommandArgument='<%# Eval("client_re_upload")%>' CssClass="btn btn-primary"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>

                                 <%-- </div>--%>
                              </asp:Panel>

                   <div class="row">
                                <div class="col-sm-10 col-xs-12"></div>
                                <div class="col-sm-2 col-xs-12" id="for_dublicate_gv_search" runat="server">
                                  <b>  Search :</b>
                        <asp:TextBox runat="server" ID="txt_search_dublicate_id" CssClass=" form-control" onkeyup="Search_Gridview_dublicate(this)" />
                                </div>
                            </div>
                          <asp:Panel ID="for_dublicate_id" runat="server" CssClass="panel panel-primary" Style="overflow-x:auto; border:none;">
                           <%--  <div class="container-fluid">--%>

                                     <asp:GridView ID="gv_dublicate_id_card" runat="server" AutoGenerateColumns="false" BackColor="White" OnRowDataBound="gv_dublicate_id_card_RowDataBound" BorderColor="#CCCCCC"  BorderStyle="None" BorderWidth="1px" CellPadding="3" class="table"  Width="100%">
                                        <FooterStyle BackColor="White" ForeColor="#004C99" />
                                        <SelectedRowStyle BackColor="#d1ddf1" Font-Bold="True" ForeColor="#333333" />
                                        <AlternatingRowStyle BackColor="White" />
                                        <HeaderStyle BackColor="#224173" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#224173"  ForeColor="White" HorizontalAlign="Center" />
                                        <RowStyle BackColor="#EFF3FB" />
                                        <EditRowStyle BackColor="#2461BF" />
                                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                        <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                        <SortedDescendingHeaderStyle BackColor="#00547E" />
                                        <Columns>
                                          
                                            

                                            <asp:TemplateField HeaderText="Sr No.">
                                                <ItemStyle Width="20px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="Label4" runat="server" Text="<%# Container.DataItemIndex+1 %>" Width="20px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chk_header_dublicate" runat="server" Text="Select Record"  />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chk_record_dublicate" runat="server" CssClass="center-block" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                             <asp:BoundField DataField="id" HeaderText="id" SortExpression="id" />
                                            <asp:BoundField DataField="material_type" HeaderText="Material Type" SortExpression="material_type" />
                                            <asp:BoundField DataField="dispatch_through" HeaderText="Dispatch Through" SortExpression="dispatch_through" />
                                            <asp:BoundField DataField="client_code" HeaderText="Client Code" SortExpression="client_code" />
                                            <asp:BoundField DataField="client_name" HeaderText="Client Name" SortExpression="client_name" />
                                            <asp:BoundField DataField="state_name" HeaderText="State Name" SortExpression="state_name" />
                                             <asp:BoundField DataField="unit_code" HeaderText="Unit Code" SortExpression="unit_code" />
                                             <asp:BoundField DataField="unit_name" HeaderText="Location Name" SortExpression="unit_name" />
                                             <asp:BoundField DataField="emp_code" HeaderText="Emp Code" SortExpression="emp_code" />
                                            <asp:BoundField DataField="emp_name" HeaderText="Employee Name" SortExpression="emp_name" />
                                            <asp:BoundField DataField="material_dispatch_date" HeaderText="Requested On Date" SortExpression="material_dispatch_date" />
                                             <asp:BoundField DataField="material_Receiver_name" HeaderText=" Receiver Name" SortExpression="material_Receiver_name" />
                                            <asp:BoundField DataField="material_person_name" HeaderText="Person Name" SortExpression="material_person_name" />
                                              <asp:BoundField DataField="material_shipping_address" ItemStyle-Width="15%" HeaderText=" Shipping Address" SortExpression="material_shipping_address" />
                                             <asp:BoundField DataField="approve_dispatch_date" HeaderText="Dispatch Date" SortExpression="approve_dispatch_date" />

                                            <asp:BoundField DataField="requested_by" HeaderText="Requested By" SortExpression="requested_by" />

                                                <asp:TemplateField HeaderText="Reason">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_dublicate_reason" runat="server" CssClass="form-control" Text='<%# Eval("material_hold_reason")%>' Width="150" onkeypress="return isNumberKey(event,this.id)"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                  
                                        <asp:BoundField DataField="approve_record" HeaderText="Status" SortExpression="approve_record" />
                                         
                                               <asp:TemplateField HeaderText="Download/Status">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnk_dublicate_download" runat="server" Width="100%" CausesValidation="false" Text="Download" Style="color: white" OnCommand="lnk_dublicate_download_Command" CommandArgument='<%# Eval("client_re_upload")%>' CssClass="btn btn-primary"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                               
                                        </Columns>
                                    </asp:GridView>

                                 <%-- </div>--%>
                              </asp:Panel>

                         
                    <div class="col-sm-2 col-xs-12" id="upload_data" runat="server">
                    <table  class="table table-striped" style="width: 20%">
                                                <tr>
                                                    <td>File to Upload :
                                                <asp:FileUpload ID="client_request_upload" runat="server" meta:resourcekey="photo_uploadResource1" onchange="ValidateSingleInput(this);" />
                                                        <span style="color: red; font-size: 8px; font-weight: bold;">Only JPG,JPEG,GIF,PDF</span></td>
                                                    <td>
                                                        <asp:Button ID="btn_approval_upload" runat="server" class="btn btn-primary" OnClick="btn_approval_upload_Click" Style="margin-top: 1em" Text="Upload"  OnClientClick="return upload_request_function();" />
                                                    </td>
                                                </tr>
                                            </table>
                        </div>


                         <div class="row text-center" id="btn_visibility" runat="server" style="width: 100%;">

                            <asp:Button ID="btn_approve_for_material" CssClass="btn btn-primary" OnClick="btn_approve_for_material_Click" OnClientClick="return approval_request_validation();" runat="server" Text="Approve" />
                               <asp:Button ID="btn_hold_material" CssClass="btn btn-primary" OnClick="btn_hold_material_Click" OnClientClick="return hold_request_validation();"  runat="server" Text="Hold" />
                             <asp:Button ID="btn_reject_material" CssClass="btn btn-primary" OnClick="btn_reject_material_Click"  OnClientClick="return rejected_request_function();"  runat="server" Text="Reject" />
                             <asp:Button ID="btn_download_report" CssClass="btn btn-primary"  OnClick="btn_download_report_Click" OnClientClick="return download_request_validation();"  runat="server" Text="Download Report" Width="12%" />

                          </div>
                 
              
                   </div>
              </div>
      
   
</asp:Content>
<%@ Page Language="C#" EnableEventValidation="false" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="advance_policy.aspx.cs" Inherits="advance_policy" Title="Advance Policy" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Advance Policy</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_header" runat="Server">
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
    <meta charset="utf-8" />
    <script src="js/jquery-1.12.3.min.js"></script>
    <script src="Scripts/jquery-1.11.3.js"></script>
    <script src="js/bootstrap.js" type="text/javascript"></script>
    <script src="Scripts/datetimepicker.js"></script>
    <%-- <script src="Scripts/jquery-ui-1.8.20.min.js"></script>
    <script src="Scripts/jquery-ui-1.8.20.js"></script>--%>
    <script src="Scripts/jquery-1.7.1.js"></script>
    <script src="Scripts/jquery-ui.min.js"></script>
    <!-----DATE PIKAR--------->
    <script src="js/bootstrap.min.js"></script>
    <script src="js/jquery.blockUI.js"></script>
    <link href="Scripts/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/jquery-ui.css" rel="stylesheet" />
    <link href="css/select2.min.css" rel="stylesheet" />
    <script src="js/hashfunction.js"></script>
    <link href="css/new_stylesheet.css" rel="stylesheet" />
    <script src="js/select2.min.js"></script>
    <link href="css/GridViewFreezeStyle.css" rel="stylesheet" type="text/css" />
    <link href="css/new_stylesheet.css" rel="stylesheet" />
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
    <%--<script src="datatable/pdfmake.min.js"></script>--%>
    <script type="text/javascript">
        function unblock() {
            $.unblockUI();
        }
    </script>
    <style>
        .label_text {
            font-size: 14px;
            font-weight: bold;
            margin-bottom: 10px;
        }

        .text_box {
            margin-top: 7px;
        }

        .text_box_head {
            margin-top: 7px;
            text-transform: uppercase;
        }

        .auto-style1 {
            color: #FFFFFF;
        }

        h2 {
            border-radius: 5px;
        }


        h5 {
            font-weight: bold;
            font-size: 15px;
        }

        .row {
            margin: 0px;
        }

        .text-red {
            color: #f00;
        }

        .FixedHeader {
            position: absolute;
        }

        .text_margin {
            margin-right: 9em;
        }

        #MainContent_grd_company_files {
            font-size: 10px;
        }

        html input[type="button"], input[type="reset"], input[type="submit"] {
            cursor: pointer;
            font-size: 10px;
            -webkit-appearance: button;
        }

        #ctl00_cph_righrbody_document1_file {
            margin-top: 15px;
        }

        .grid-view {
            height: auto;
            max-height: 600px;
            overflow-x: hidden;
            overflow-y: auto;
            font-family: Verdana;
        }
    </style>
    <script>
        $(document).ready(function () {
            $.fn.dataTable.ext.errMode = 'none';
            var table = $('#<%=gv_client_bill.ClientID%>').DataTable({
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
               .appendTo('#<%=gv_client_bill.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';
        });
          $(function () {
              $('#<%=gv_client_bill.ClientID%> td').click(function () {
               $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
           });

       });

       function isNumber(evt) {
           evt = (evt) ? evt : window.event;
           var charCode = (evt.which) ? evt.which : evt.keyCode;
           if (charCode > 31 && (charCode < 48 || charCode > 57)) {
               return false;
           }
           return true;
       }
       //save record




       //delete record

       function Delete_record() {
           var b;
           b = confirm("Are You Sure You Want To Delete Record");

           if (b == true) {
               $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
               return true;
           }
           else {

               return false;
           }
       }

       function Update_record() {
           var b;
           b = confirm("Are You Sure you Want To Update Record !!!");

           if (b == true) {
               $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
               return true;
           }
           else {

               return false;
           }
       }
        //save vendor
       function save_vendor()
       {
          
           var ddl_vendor = document.getElementById('<%=ddl_vendor.ClientID %>');
           var Selected_ddl_vendor = ddl_vendor.options[ddl_vendor.selectedIndex].text;

           if (Selected_ddl_vendor == "Select") {

               alert("Please Select Vendor Name !!");
               ddl_vendor.focus();
               return false;
           }
           var ddl_po = document.getElementById('<%=ddl_po.ClientID %>');
           var Selected_ddl_po = ddl_po.options[ddl_po.selectedIndex].text;
           if (Selected_ddl_po == "Select") {

               alert("Please Select PO Number !!");
               ddl_po.focus();
               return false;
           }
           var ddl_invoice = document.getElementById('<%=ddl_invoice.ClientID %>');
           var Selected_ddl_invoice = ddl_invoice.options[ddl_invoice.selectedIndex].text;
           if (Selected_ddl_invoice == "Select") {

               alert("Please Select Invoice Number !!");
               ddl_invoice.focus();
               return false;
           }
           var txt_vamount = document.getElementById('<%=txt_vamount.ClientID %>');
           var txt_vdate = document.getElementById('<%=txt_vdate.ClientID %>');

           var ddl_pay_mod = document.getElementById('<%=ddl_pay_mod.ClientID %>');
           var Selected_ddl_pay_mod = ddl_pay_mod.options[ddl_pay_mod.selectedIndex].text;
           
          
          
           
           if (txt_vamount.value == "") {
               alert("Please Enter Amount !!");
               txt_vamount.focus();
               return false;
           }
           if (txt_vdate.value == "") {
                  alert("Please Select Date !!");
                  txt_vdate.focus();
                  return false;
           }

           if (Selected_ddl_pay_mod == "Cheque")
           {
               var txt_cheq_no = document.getElementById('<%=txt_cheq_no.ClientID %>');
               var txt_cheq_rby = document.getElementById('<%=txt_cheq_rby.ClientID %>');
               if (txt_cheq_no.value == "") {
                   alert("Please Enter Cheque Number !!");
                   txt_cheq_no.focus();
                   return false;
               }
               if (txt_cheq_rby.value == "") {
                   alert("Please Enter Cheque Received By  !!");
                   txt_cheq_rby.focus();
                   return false;
               }
           }
           else if (Selected_ddl_pay_mod == "Online") {
               var txt_utr_no = document.getElementById('<%=txt_utr_no.ClientID %>');
               if (txt_utr_no.value == "") {
                   alert("Please Enter UTR Number !!");
                   txt_utr_no.focus();
                   return false;
               }
           }
           else {
               
               alert("Please select Payment Mode  !!");
               ddl_pay_mod.focus();
                   return false;
               
           }
           return true;
       }

       //save button
       function Req_validation() {
           var ddl_client = document.getElementById('<%=ddl_client.ClientID %>');
              var Selected_ddl_client = ddl_client.options[ddl_client.selectedIndex].text;

              if (Selected_ddl_client == "Select") {

                  alert("Please Select Client !!");
                  ddl_client.focus();
                  return false;
              }

              var ddl_state = document.getElementById('<%=ddl_state.ClientID %>');
              var Selected_ddl_state = ddl_state.options[ddl_state.selectedIndex].text;

              if (Selected_ddl_state == "Select") {

                  alert("Please Select State !!");
                  ddl_state.focus();
                  return false;
              }

              var ddl_unit = document.getElementById('<%=ddl_unit.ClientID %>');
              var Selected_ddl_unit = ddl_unit.options[ddl_unit.selectedIndex].text;


              if (Selected_ddl_unit == "Select") {

                  alert("Please Select Branch Name !!");
                  ddl_unit.focus();
                  return false;
              }

              var ddl_user_type = document.getElementById('<%=ddl_user_type.ClientID %>');
              var Selected_ddl_user_type = ddl_user_type.options[ddl_user_type.selectedIndex].text;


              if (Selected_ddl_user_type == "Select") {

                  alert("Please Select User Type !!");
                  ddl_user_type.focus();
                  return false;
              }

              var ddl_emp_name = document.getElementById('<%=ddl_emp_name.ClientID %>');
              var Selected_ddl_emp_name = ddl_emp_name.options[ddl_emp_name.selectedIndex].text;


              if (Selected_ddl_emp_name == "Select") {

                  alert("Please Select Employee !!");
                  ddl_emp_name.focus();
                  return false;
              }

              var ddl_advance_policy = document.getElementById('<%=ddl_advance_serves.ClientID %>');
              var Selected_ddl_advance_policy = ddl_advance_policy.options[ddl_advance_policy.selectedIndex].text;


              if (Selected_ddl_advance_policy == "Select") {

                  alert("Please Select Service !!");
                  ddl_advance_policy.focus();
                  return false;
              }

              var txt_amount = document.getElementById('<%=txt_amount.ClientID %>');


              if (txt_amount.value == "") {

                  alert("Please Enter Amount !!");
                  txt_amount.focus();
                  return false;
              }


              var date_picker = document.getElementById('<%=date_picker.ClientID %>');


              if (date_picker.value == "") {

                  alert("Please Select Date !!");
                  date_picker.focus();
                  return false;
              }
              $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
              $(".policy_installment").show();
              return true;
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
        
          function upload_validation() {
              var document_file_approval = document.getElementById('<%=FileUpload1.ClientID %>');
            
              if (document_file_approval.value == "") {
                  alert("Please Upload File");
                  document_file_approval.focus();
                  return false;
              }
          }


    </script>

    <script type="text/javascript">

        function pageLoad() {
            drp_operation();
            vendor();
            $('#<%=ddl_client.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_state.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_user_type.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });

            $('.date-picker').datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                maxDate: 0,
                yearRange: '1950',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-date .ui-datepicker-month .ui-datepicker-year :selected").val();

                }
            });



            $(".date-picker").attr("readonly", "true");
            $(function () {



                $('body').on('keyup', '.maskedExt', function () {
                    var num = $(this).attr("maskedFormat").toString().split(',');
                    var regex = new RegExp("^\\d{0," + num[0] + "}(\\.\\d{0," + num[1] + "})?$");
                    if (!regex.test(this.value)) {
                        this.value = this.value.substring(0, this.value.length - 1);
                    }
                });
            });



        }
        $(document).ready(function () {
            var st = $(this).find("input[id*='hidtab']").val();
            if (st == null)
                st = 0;
            $('[id$=tabs]').tabs({ selected: st });
        });
        function drp_operation()
        {
            var ddl_payment_mode = document.getElementById('<%=ddl_payment_mode.ClientID %>');
            var Selected_ddl_payment_mode = ddl_payment_mode.options[ddl_payment_mode.selectedIndex].text;
            if (Selected_ddl_payment_mode == "Installment")
            {
                $(".policy_installment").show();
            }
            else if (Selected_ddl_payment_mode == "Full Payment") {
                $(".policy_installment").hide();
            }
            else if (Selected_ddl_payment_mode == "Select") {
                $(".policy_installment").hide();
            }
        }

        function vendor() {
            var ddl_pay_mod = document.getElementById('<%=ddl_pay_mod.ClientID %>');
            var Selected_ddl_pay_mod = ddl_pay_mod.options[ddl_pay_mod.selectedIndex].text;
            if (Selected_ddl_pay_mod == "Cheque") {
                //alert(Selected_ddl_pay_mod.text)
                $(".cheque1").show();
                $(".utr").hide();
            }
            else if (Selected_ddl_pay_mod == "Online") {
                //alert(Selected_ddl_pay_mod.text)
                $(".utr").show();
                $(".cheque1").hide();
            }
            else if (Selected_ddl_pay_mod == "Select") {
               // alert(Selected_ddl_pay_mod.text)
                $(".cheque1").hide();
                $(".utr").hide();
            }
        }
        function openWindow() {
            window.open("html/advance_policy.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
        }
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cph_righrbody" runat="Server">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>






    <div class="container-fluid">
        <asp:Panel ID="Panel3" runat="server" CssClass="panel panel-primary" Style="background-color: white">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-9">
                        <div style="text-align: center; color: #fff; font-size: small;"><b>ADVANCE POLICY</b></div>
                    </div>
                    <div class="col-sm-2 text-right">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <br />
             <%--<div class="panel-heading">
                <div class="row">
                    <div class="col-sm-4"></div>
                    <div class="col-sm-9">
                        <div style="text-align: left; color:white; font-size: small;"><b>Advance Policy Details</b></div>
                    </div>
                    <div class="col-sm-2 text-left">
                        
                    </div>
                </div>
            </div>--%>

            <%--<asp:Panel ID="reporting_panel" runat="server">
                <div class="panel-heading">
                    <div style="color: white; font-weight: bold; font-size: 15px;">Approvals Required</div>
                </div>
                
            </asp:Panel>--%>

            <div class="panel-body">
                <%-- <asp:UpdatePanel runat="server">
                    <ContentTemplate>--%>

                    <%--<asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>--%>
        <div id="tabs" style="background: #f3f1fe; padding:25px 25px 25px 25px; border: 1px solid #e2e2dd; margin-bottom:20px; margin-top:20px; border-radius:10px">
            <asp:HiddenField ID="hidtab" Value="0" runat="server" />
            <ul>
                <li><a href="#menu1"><b>Employee Adavance</b></a></li>
                <li><a href="#menu2"><b>Vendor Advance</b></a></li>

            </ul>
            <div id="menu1">
                <div class="container" style="background: white; border-radius: 10px; margin-bottom:20px; margin-top:20px; border: 1px solid white">
                    <br />
                    <div class="row">
                        <div class="col-sm-3 col-xs-12">
                           <b> Client :</b>
                                         <span class="text-red">*</span>
                            <asp:DropDownList ID="ddl_client" runat="server"
                                class="form-control" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged" AutoPostBack="true" onkeypress="return AllowAlphabet(event)"
                                MaxLength="20">
                            </asp:DropDownList>

                        </div>
                        <div class="col-sm-3 col-xs-12">
                            <b> State :</b>
                                         <span class="text-red">*</span>
                            <asp:DropDownList ID="ddl_state" runat="server"
                                class="form-control" OnSelectedIndexChanged="ddl_state_SelectedIndexChanged" AutoPostBack="true" onkeypress="return AllowAlphabet(event)"
                                MaxLength="20">
                            </asp:DropDownList>
                        </div>

                        <div class="col-sm-3 col-xs-12">
                            <b> Branch Name :</b>
                                      <span class="text-red">*</span>
                            <asp:DropDownList ID="ddl_unit" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-3 col-xs-12">
                           <b>  User Type:</b>
                                        <span class="text-red">*</span>
                            <asp:DropDownList ID="ddl_user_type" runat="server" class="form-control" MaxLength="30" OnSelectedIndexChanged="ddl_branch_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Value="Select">Select</asp:ListItem>
                                <asp:ListItem Value="Employee">Employee</asp:ListItem>
                                <asp:ListItem Value="Field_Officer">Field_Officer</asp:ListItem>
                               <%-- <asp:ListItem Value="Vendor">Vendor</asp:ListItem>--%>

                            </asp:DropDownList>

                        </div>



                    </div>
                    <br />
                    <div class="row">
                     <div class="col-sm-3 col-xs-12">
                           <b>  Advance For Service:</b>
                                        <span class="text-red">*</span>
                            <asp:DropDownList ID="ddl_advance_serves" runat="server" class="form-control" MaxLength="30" OnSelectedIndexChanged="ddl_advance_serves_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem>Select</asp:ListItem>
                                <asp:ListItem>Reliver Payment</asp:ListItem>
                                <asp:ListItem>Self Advance</asp:ListItem>
                                <asp:ListItem>Deep Cleaning</asp:ListItem>
                                <asp:ListItem>Material Purchase</asp:ListItem>
                                <asp:ListItem>Temporary Man Power</asp:ListItem>
                                <asp:ListItem>Deployment</asp:ListItem>
                                <asp:ListItem>Visit</asp:ListItem>
                                <asp:ListItem>Others</asp:ListItem>
                            </asp:DropDownList>

                        </div>

                          <div class="col-sm-2 col-xs-12">
                            <b> Employee Name:</b>
                                         <span class="text-red">*</span>
                            <asp:DropDownList ID="ddl_emp_name1" runat="server"
                                class="js-example-basic-single form-control" onkeypress="return AllowAlphabet(event)"
                                MaxLength="20">
                                <asp:ListItem Value="Select">Select</asp:ListItem>
                            </asp:DropDownList>

                        </div>

                       <div class="col-sm-3 col-xs-12" id="emp_hide" runat="server">
                            <b> Field Officer Name:</b>
                                         <span class="text-red">*</span>
                            <asp:DropDownList ID="ddl_emp_name" runat="server"
                                class="js-example-basic-single form-control" onkeypress="return AllowAlphabet(event)"
                                MaxLength="20">
                                <asp:ListItem Value="0">Select</asp:ListItem>
                            </asp:DropDownList>

                        </div>
                        <div class="col-sm-2 col-xs-12 ">
                           <b>  Amount :</b>
                                    <span class="text-red">*</span>
                            <asp:TextBox ID="txt_amount" runat="server" onkeypress="return isNumber(event);"
                                class="form-control" Width="100%">

                            </asp:TextBox>

                        </div>
                        <div class="col-sm-2 col-xs-12">
                           <b>  Date :</b>
                                    <span class="text-red">*</span>
                            <asp:TextBox ID="date_picker" runat="server" CssClass="form-control date-picker" onkeypress="return AllowAlphabet_address(event);"></asp:TextBox>
                        </div>





                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-3 col-sx-12">
                            <b> Description :</b>
                                        <asp:TextBox ID="txt_comment" Rows="2" Columns="2" runat="server" class="form-control" TextMode="MultiLine" onkeypress="return AllowAlphabet12(event)">
                                        </asp:TextBox>
                            <asp:TextBox ID="policy_id" runat="server" CssClass="form-control text_box" ReadOnly="true" Enabled="False" Visible="False"></asp:TextBox><br />

                        </div>
                        <div class="col-sm-3 col-xs-12">
                            <b> Payment Mode :</b>
                                           <asp:DropDownList ID="ddl_payment_mode" runat="server" class="form-control" MaxLength="30" onchange="drp_operation()">
                                               <asp:ListItem Value="0">Select</asp:ListItem>
                                               <asp:ListItem Value="1">Installment</asp:ListItem>
                                               <asp:ListItem Value="2">Full Payment</asp:ListItem>
                                           </asp:DropDownList>

                        </div>
                        <div class="col-sm-2 col-xs-12 policy_installment">
                           <b>  No of Installment :</b>
                                       <asp:TextBox ID="txt_installment" runat="server" onkeypress="return isNumber(event);" class="form-control" Width="100%">0</asp:TextBox>

                        </div>
                         <div class="col-sm-2 col-xs-12" style="visibility:hidden" >
                           <b>  Remaning Amount :</b>
                                       <asp:TextBox ID="txt_remaing" runat="server" onkeypress="return isNumber(event);" class="form-control" Width="100%">0</asp:TextBox>

                        </div>
                    </div>
                    <br />
                    
                    <div class="row text-center">

                        <asp:Button ID="btn_add" runat="server" Visible="true" class="btn btn-primary"
                            Text=" Save " OnClientClick="return Req_validation()" OnClick="btn_add_Click" />
                        <asp:Button ID="btn_Update" runat="server" class="btn btn-primary" Text=" Update " OnClientClick="return Update_record()" OnClick="btn_update_policy" Visible="false"/>
                        <asp:Button ID="btn_Delet" runat="server" class="btn btn-primary" Text=" Delete " OnClientClick="return Delete_record()" OnClick="ddl_delete_policy" />
                         <asp:Button ID="btn_advreport" runat="server" class="btn btn-primary" Text=" Advance Report " OnClick="btn_advreport_Click" Width="15%"/>
                        <asp:Button ID="btn_Close" runat="server" class="btn btn-danger" Text=" Close " OnClick="btnclose_Click" />
                        <div class="row">
                        <div class=" col-sm-8 col-xs-12"></div>
                                <div class=" col-sm-2 col-xs-12" style="margin-top: 9px;">
                                    <b>File Upload :</b><span class="text-red">*</span>
                                    <asp:FileUpload ID="FileUpload1" runat="server" />
                                </div>
                                <div class=" col-sm-2 col-xs-12" style="margin-top: 14px;">
                                    <asp:Button ID="btn_upload" runat="server" CssClass="btn btn-primary convience"
                                        Text="Upload" OnClick="btn_upload_Click" OnClientClick="return upload_validation();"/>
                                </div>
                            </div>


                    </div>
                    <br />
                    <br />
                </div>
                <br />
                <br />
                <asp:Panel ID="Panel6" runat="server" CssClass="grid">
                    <asp:GridView ID="gv_client_bill" class="table" runat="server" BackColor="White"
                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" OnRowDataBound="GradeGridView_RowDataBound"
                        OnSelectedIndexChanged="onSelected_IndexChange"
                        AutoGenerateColumns="False" Width="100%" OnPreRender="gv_client_bill_PreRender">
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
                            <asp:TemplateField HeaderText="Sr.No">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_GRADE_CODE" runat="server" Text='<%# Eval("policy_id") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:BoundField DataField="client_name" HeaderText="Client Name"
                                SortExpression="client_name" />
                             <asp:BoundField DataField="state_name" HeaderText="State Name"
                                SortExpression="state_name" />

                            <asp:BoundField DataField="branch_code" HeaderText="Branch"
                                SortExpression="branch_code" />
                            <asp:BoundField DataField="user_type" HeaderText="User Type"
                                SortExpression="user_type" />
                            <asp:BoundField DataField="emp_name" HeaderText="Emp Name"
                                SortExpression="emp_name" />
                            <asp:BoundField DataField="advance_service" HeaderText="Advance Service"
                                SortExpression="advance_service" />
                            <asp:BoundField DataField="amount" HeaderText="Amount"
                                SortExpression="amount" />
                            <asp:BoundField DataField="date" HeaderText="Date"
                                SortExpression="date" />

                            <asp:BoundField DataField="comment" HeaderText="Comment"
                                SortExpression="comment" />
                            <asp:BoundField DataField="emp_code" HeaderText="Emp Code"
                                SortExpression="emp_code" />


                        </Columns>

                    </asp:GridView>
                </asp:Panel>

                <br />
                <br />
                 <asp:Panel ID="Panel1" runat="server" CssClass="grid">
                    <asp:GridView ID="gv_empadvance" class="table" runat="server" BackColor="White"
                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
                        AutoGenerateColumns="False" Width="100%">
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
                         <asp:BoundField DataField="client_code" HeaderText="Client Name"
                                SortExpression="client_code" />
                         <asp:BoundField DataField="branch_code" HeaderText="Branch"
                                SortExpression="branch_code" />
                         <asp:BoundField DataField="pay_to_emp_code" HeaderText="Employee Name"                              
                                SortExpression="pay_to_emp_code" />
                              <asp:BoundField DataField="amount" HeaderText="Advance Amount"
                                SortExpression="amount" />
                             <asp:BoundField DataField="month_year" HeaderText="Advance Deduct Month"
                                SortExpression="month_year" />
                             <asp:BoundField DataField="emp_advance" HeaderText="Deduct Amount"
                                SortExpression="emp_advance" />
                             <asp:BoundField DataField="remaing_advance" HeaderText="Balance"
                                SortExpression="remaing_advance" />
                              </Columns>
                        </asp:GridView>
                     </asp:Panel>
                </div>
            <div id="menu2">
                  <div class="container" style="background: white; border-radius: 10px; margin-bottom:20px; margin-top:20px; border: 1px solid white">
                    <br />
                    <div class="row">
                         <div class="col-sm-1 col-xs-12">
                           

                        </div>
                        <div class="col-sm-3 col-xs-12">
                            <b> Vendor Name :</b>
                                         <span class="text-red">*</span>
                            <asp:DropDownList ID="ddl_vendor" runat="server"
                                class="form-control"  AutoPostBack="true" OnSelectedIndexChanged="ddl_vendor_SelectedIndexChanged">
                            </asp:DropDownList>

                        </div>
                        <div class="col-sm-3 col-xs-12">
                            <b> PO NO :</b>
                                         <span class="text-red">*</span>
                            <asp:DropDownList ID="ddl_po" runat="server"
                                class="form-control"  AutoPostBack="true" OnSelectedIndexChanged="ddl_po_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>

                        <div class="col-sm-3 col-xs-12">
                            <b> Invoice No :</b>
                                      <span class="text-red">*</span>
                            <asp:DropDownList ID="ddl_invoice" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                       



                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-1 col-xs-12">
                           

                        </div>
                        <div class="col-sm-2 col-xs-12 ">
                            <b> Amount :</b>
                                    <span class="text-red">*</span>
                            <asp:TextBox ID="txt_vamount" runat="server" onkeypress="return isNumber(event);"
                                class="form-control" Width="100%">

                            </asp:TextBox>

                        </div>
                        <div class="col-sm-2 col-xs-12">
                            <b> Date :</b>
                                    <span class="text-red">*</span>
                            <asp:TextBox ID="txt_vdate" runat="server" CssClass="form-control date-picker" onkeypress="return AllowAlphabet_address(event);"></asp:TextBox>
                        </div>
                        <div class="col-sm-3 col-sx-12">
                           <b>  Description :</b>
                                        <asp:TextBox ID="txt_des" Rows="2" Columns="2" runat="server" class="form-control" TextMode="MultiLine" onkeypress="return AllowAlphabet12(event)">
                                        </asp:TextBox>
                           

                        </div>
                        <div class="col-sm-2 col-xs-12 " style="visibility:hidden">
                            <b> ID :</b>
                                    <span class="text-red">*</span>
                            <asp:TextBox ID="txt_id" runat="server" class="form-control" Width="100%">

                            </asp:TextBox>

                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-1 col-xs-12">
                           

                        </div>
                        <div class="col-sm-3 col-xs-12">
                             <b>Payment Mode :</b>
                                           <asp:DropDownList ID="ddl_pay_mod" runat="server" class="form-control" MaxLength="30" onchange="vendor()">
                                               <asp:ListItem Value="0">Select</asp:ListItem>
                                               <asp:ListItem Value="1">Cheque</asp:ListItem>
                                               <asp:ListItem Value="2">Online</asp:ListItem>
                                           </asp:DropDownList>

                        </div>
                        <div class="col-sm-2 col-xs-12 cheque1">
                            <b> CHeque NO :</b>
                                       <asp:TextBox ID="txt_cheq_no" runat="server" onkeypress="return AllowAlphabet12(event)"  class="form-control" Width="100%"></asp:TextBox>

                        </div>
                        <div class="col-sm-2 col-xs-12 cheque1">
                             <b>CHeque Received By </b>
                                       <asp:TextBox ID="txt_cheq_rby" runat="server" onkeypress="return AllowAlphabet12(event)"  class="form-control" Width="100%"></asp:TextBox>

                        </div>
                        <div class="col-sm-2 col-xs-12 utr">
                            <b>UTR No :</b>
                                       <asp:TextBox ID="txt_utr_no" runat="server" onkeypress="return AllowAlphabet12(event)" class="form-control" Width="100%"></asp:TextBox>

                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row text-center">

                        <asp:Button ID="btn_vendor_save" runat="server"  class="btn btn-primary" OnClick="btn_vendor_save_Click"  Text=" Save " OnClientClick="return save_vendor()" />
                        <asp:Button ID="btn_vendor_update" runat="server" class="btn btn-primary" Text=" Update " OnClientClick="return Update_record()" OnClick="btn_update_policy" Visible="false"/>
                        <asp:Button ID="btn_vendor_delete" runat="server" class="btn btn-primary" Text=" Delete " OnClientClick="return Delete_record()" OnClick="btn_vendor_delete_Click" />
                        <%-- <asp:Button ID="Button4" runat="server" class="btn btn-primary" Text=" Advance Report " OnClick="btn_advreport_Click"/>--%>
                        <asp:Button ID="Button5" runat="server" class="btn btn-danger" Text=" Close " OnClick="btnclose_Click" />



                    </div>
                    <br />
                </div>
                <br />
                <br />
                 <asp:Panel ID="Panel2" runat="server" CssClass="grid">
                    <asp:GridView ID="grv_vendor" class="table" runat="server" BackColor="White" OnRowDataBound="grv_vendor_RowDataBound"
                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"  OnSelectedIndexChanged="grv_vendor_SelectedIndexChanged"
                        
                        AutoGenerateColumns="False" Width="100%" >
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
                            <asp:BoundField DataField="id" HeaderText="ID"
                                SortExpression="id" />
                            <asp:BoundField DataField="vendor_name" HeaderText="Vendor Name"
                                SortExpression="vendor_name" />
                            <asp:BoundField DataField="po_no" HeaderText="PO Number"
                                SortExpression="po_no" />
                            <asp:BoundField DataField="invoice_no" HeaderText="Invoice Number"
                                SortExpression="invoice_no" />

                            <asp:BoundField DataField="amount" HeaderText="Amount"
                                SortExpression="amount" />
                            <asp:BoundField DataField="date" HeaderText="Date"
                                SortExpression="date" />                         

                        </Columns>

                    </asp:GridView>
                </asp:Panel>

                </div>
            </div>
                
            </div>


            <%--   </ContentTemplate>
                </asp:UpdatePanel>--%>
        </asp:Panel>
    </div>
</asp:Content>

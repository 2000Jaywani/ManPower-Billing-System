<%@ Page Language="C#" AutoEventWireup="true" CodeFile="add_emp.aspx.cs" Inherits="add_emp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
    <script>
        $(document).ready(function () {
            $(".date-picker").datepicker({
                changeMonth: true,
                changeYear: true,

                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                yearRange: '1950',
                onSelect: function (selected) {
                    $(".date-picker1").datepicker("option", "minDate", selected)
                }
            });
            $(".date-picker1").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                maxDate: 0,
                dateFormat: 'dd/mm/yy',
                onSelect: function (selected) {
                    $(".date-picker2").datepicker("option", "minDate", selected)
                }
            });

            //$(".date-picker2").datepicker({
            //    changeMonth: true,
            //    changeYear: true,
            //    showButtonPanel: true,
            //    dateFormat: 'dd/mm/yy',
            //    minDate: 0,
            //    onSelect: function (selected) {
            //        $(".date-picker1").datepicker("option", "maxDate", selected)
            //    }
            //});

            $(".date-picker").attr("readonly", "true");
            $(".date-picker1").attr("readonly", "true");

            //$(".date-picker2").attr("readonly", "true");

        });
        function AllowAlphabet(e) {
            isIE = document.all ? 1 : 0
            keyEntry = !isIE ? e.which : event.keyCode;
            if (((keyEntry >= '65') && (keyEntry <= '90')) || ((keyEntry >= '97') && (keyEntry <= '122')) || (keyEntry == '32') || keyEntry == '45' || (keyEntry == '8') || (keyEntry == '9') || (keyEntry == '46'))
                return true;
            else {
                // alert('Please Enter Only Character values.');
                return false;
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
        function validation() {

            var ddl_employee_type = document.getElementById('<%=ddl_employee_type.ClientID %>');
            var Selected_ddl_employee_type = ddl_employee_type.options[ddl_employee_type.selectedIndex].text;
            if (Selected_ddl_employee_type == "Select") {
                alert("Please Select Employee Type");
                ddl_employee_type.focus();
                return false;
            }
            var ddl_grade = document.getElementById('<%=ddl_grade.ClientID%>');
            var Selected_ddl_grade = ddl_grade.options[ddl_grade.selectedIndex].text;
            if (Selected_ddl_grade == "Select") {

                alert("Please Select Designation ");
                ddl_grade.focus();
                return false;
            }
            var txt_emp_name = document.getElementById('<%=txt_emp_name.ClientID%>')
            if (txt_emp_name.value == "") {
                alert("Please entr Employee Name");
                txt_emp_name.focus();
                return false;
            }


            var ddl_gender = document.getElementById('<%=ddl_gender.ClientID%>');
            var Selected_ddl_gender = ddl_gender.options[ddl_gender.selectedIndex].text;
            if (Selected_ddl_gender == "Select") {
                alert("Please Slecte Gender");
                ddl_gender.focus();
                return false;
            }

            var txt_joining_date = document.getElementById('<%=txt_joining_date.ClientID%>')
            if (txt_joining_date.value == "") {
                alert("Please Enter Date Of Joint ");
                txt_joining_date.focus();
                return false;

            }
        }

    </script>
    <style>
        .row {
            margin-right: 0px;
            margin-left: 0px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="container-fluid">
            <div class="row text-center">
                <h2>Add New Employee</h2>

            </div>
            <hr />
            <div class="row">


                <div class="col-sm-3 col-xs-12">
                    Employee Code:
                    <asp:TextBox runat="server" ID="txt_emp_code" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>

                <div class="col-sm-3 col-xs-12 text-left">
                    Employee ID As Per DOJ :
                            <asp:TextBox ID="txt_id_as_per_dob" runat="server" class="form-control text_box"
                                onkeypress="return Alphabet_Number(event,this);" MaxLength="20" meta:resourcekey="txt_eecodeResource1" ReadOnly="true" disabled></asp:TextBox>


                </div>

                <div class="col-sm-3 col-xs-12 text-left">
                    Employee Type :
                                    <asp:DropDownList ID="ddl_employee_type" runat="server" class="form-control">
                                        <asp:ListItem Value="0" Text="Select">Select</asp:ListItem>
                                        <asp:ListItem Value="Reliever">Reliever</asp:ListItem>
                                    </asp:DropDownList>
                </div>

                <div class="col-sm-3 col-xs-12">
                    Designation:
                                    <asp:DropDownList ID="ddl_grade" class="form-control js-example-basic-single" Width="100%" runat="server" AutoPostBack="True" DataTextField="GRADE_DESC" DataValueField="GRADE_CODE" meta:resourcekey="ddl_gradeResource1" OnSelectedIndexChanged="ddl_grade_SelectedIndexChanged">
                                        <asp:ListItem Value="0" Text="Select">Select</asp:ListItem>
                                    </asp:DropDownList>

                </div>




            </div>
            <br />
            <div class="row">
                <div class="col-sm-3 col-xs-12">
                    Working Hours: 
                            <asp:DropDownList ID="txt_attendanceid" runat="server" AutoPostBack="true"
                                class="form-control text_box" MaxLength="2" meta:resourceKey="txt_attendacneidResource1">

                                <asp:ListItem meta:resourceKey="ListItemResource3" Text="Select" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                </div>


                <div class="col-sm-3 col-xs-12">
                    Employee Name:
                    <asp:TextBox runat="server" ID="txt_emp_name" CssClass="form-control" onkeypress="return AllowAlphabet(event)"></asp:TextBox>
                </div>
                <div class="col-sm-3 col-xs-12 text-left">
                    Gender :
 
                            <asp:DropDownList ID="ddl_gender" runat="server" class="form-control" meta:resourceKey="ddl_genderResource1">
                                <asp:ListItem Value="0" Text="Select">Select</asp:ListItem>
                                <asp:ListItem meta:resourceKey="ListItemResource3" Text="Male" Value="M"></asp:ListItem>
                                <asp:ListItem meta:resourceKey="ListItemResource4" Text="Female" Value="F"></asp:ListItem>
                                <%--<asp:ListItem meta:resourceKey="ListItemResource5" Text="Transgender" Value="T"></asp:ListItem>--%>
                            </asp:DropDownList>
                </div>
                <asp:TextBox runat="server" ID="txt_state" CssClass="form-control" Visible="false" ReadOnly="true"></asp:TextBox>
                <div class="col-sm-3 col-xs-12">
                    Joining Date:
                    <asp:TextBox runat="server" ID="txt_joining_date" CssClass="form-control date-picker1"></asp:TextBox>
                </div>
                <div class="col-sm-3 col-xs-12 text-left">
                    Date of Birth :   

                            <asp:TextBox ID="txt_birthdate" runat="server" class="form-control date-picker"
                                meta:resourceKey="txt_birthdateResource1"></asp:TextBox>
                </div>

            </div>

            <%-- <div class="col-sm-2 col-xs-12">
                     IHMS Code:
                    <asp:TextBox runat="server" ID="txt_ihms_code" CssClass="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                </div>--%>


            <%-- <div class="col-sm-2 col-xs-12 text-left">
                            Father/Husband Name :   

                            <asp:TextBox ID="txt_eefatharname" runat="server" CausesValidation="True" class="form-control text_box"
                                MaxLength="50" onkeypress="return AllowAlphabet(event)" ></asp:TextBox>
                            
                        </div>--%>
            <%--<div class="col-sm-2 col-xs-12 text-left">
                            Relation :
                            <asp:DropDownList ID="ddl_relation" runat="server" class="form-control" >
                                <asp:ListItem Value="0" Text="Select">Select</asp:ListItem>
                                <asp:ListItem Value="Father">Father</asp:ListItem>
                                <asp:ListItem Value="Husband">Husband</asp:ListItem>
                            </asp:DropDownList>
                        </div>--%>

            <%--<div class="col-sm-2 col-xs-12 text-left">
                            Date of Birth :   

                            <asp:TextBox ID="txt_birthdate" runat="server" class="form-control date-picker"
                                meta:resourceKey="txt_birthdateResource1"></asp:TextBox>
                        </div>--%>

            <%--  <div class="col-sm-2 col-xs-12">
                     Grade Code:
                     <asp:DropDownList ID="ddl_grade_code" runat="server" CssClass="form-control">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList> 

                 </div>--%>

            <%--<div class="col-sm-2 col-xs-12">
                  Left Date:
                    <asp:TextBox runat="server" ID="txt_left_date" CssClass="form-control date-picker2"></asp:TextBox>
                </div>--%>
            <%--   <div class="col-sm-2 col-xs-12">
                    Employee Email Id:
                    <asp:TextBox runat="server" ID="txt_email_id" CssClass="form-control"></asp:TextBox>
                </div>
            </div>--%>
            <%-- <div class="row">
                  <div class="col-sm-1 col-xs-12"></div>
                  <div class="col-sm-4 col-xs-12">
                    Bank Acc Permnant No:
                    <asp:TextBox runat="server" ID="txt_acc_no" CssClass="form-control" onkeypress="return isNumber(event)"></asp:TextBox>
                </div>
                <div class="col-sm-4 col-xs-12">
                    IFSC Code:
                    <asp:TextBox runat="server" ID="txt_ifsc_code" CssClass="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                </div>
                   <div class="col-sm-3 col-xs-12">
                                       
                                       <%--  <span class="text-red">*</span>
                                        <asp:DropDownList ID="ddl_state" runat="server" 
                                            class="form-control"  AutoPostBack="true"   MaxLength="20">
                                           
                                        </asp:DropDownList>
                       <asp:TextBox runat="server" ID="txt_state" CssClass="form-control" Visible="false" ReadOnly="true" ></asp:TextBox>
                  </div>
                
        </div>--%>
            <br />
            <br />
            <div class="row text-center">
                <asp:Button runat="server" ID="btn_save" CssClass="btn btn-primary" Text="Save" OnClick="btn_save_Click" OnClientClick="return validation()" />
                <%--  <asp:Button runat="server" ID="btn_close" CssClass="btn btn-danger" text="Close" OnClick="btn_close_Click"/>--%>
            </div>
        </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="Attendance_policy_master.aspx.cs" Inherits="Attandance_policy_master" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Attendance Policy Master</title>
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
    <script src="js/tab.js"></script>

    <link href="css/new_stylesheet.css" rel="stylesheet" />
    <link href="css/GridViewFreezeStyle.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .grid-view {
            height: auto;
            max-height: 500px;
            overflow: scroll;
        }
    </style>
    <style>
        .row {
            margin-right: -15px;
            margin-left: -15px;
        }

        .auto-style1 {
            color: #FFFFFF;
        }

        h2 {
            border-radius: 5px;
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


        .panel-disp1, .panel-disp2, .panel-disp3, .panel-disp4, .panel-disp5 .panel-disp6 {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_righrbody" runat="Server">

    <script type="text/javascript">
        function pageLoad() {
            //$(".date-picker1").val("");
            // $(".date-picker2").val("");

            $(".date-picker1").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                minDate: 0,
                onSelect: function (selected) {
                    $(".date-picker2").datepicker("option", "minDate", selected)
                }
            });


            $(".date-picker2").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                minDate: 0,
                onSelect: function (selected) {
                    $(".date-picker1").datepicker("option", "maxDate", selected)
                }
            });

            check_own_vehicle();
            check_local_convey();
            check_approve();
            check_escation();

            $(".date-picker1").attr("readonly", "true");
            $(".date-picker2").attr("readonly", "true");

        }

        $(document).ready(function () {

            var evt = null;


        });

        var e = null;
        AllowAlphabet(e);
        var evt = null;
        isNumber(evt);


        function isNumber(evt) {
            //alert("hello B");
            if (null != evt) {
                evt = (evt) ? evt : window.event;

                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                    if (charCode == 46) {
                        return true;
                    }
                    return false;
                }

            }
            return true;


        }









        function AllowAlphabet(e) {
            //alert("hello A");
            isIE = document.all ? 1 : 0
            keyEntry = !isIE ? e.which : event.keyCode;
            if (((keyEntry >= '65') && (keyEntry <= '90')) || ((keyEntry >= '97') && (keyEntry <= '122')) || (keyEntry == '46') || (keyEntry == '32') || keyEntry == '45' || (keyEntry == '8'))
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

        function validate() {
            var policyname = document.getElementById('<%=txt_policy_name1.ClientID %>');
            var working_hours = document.getElementById('<%=txt_working_hours.ClientID %>');
            var shorthours = document.getElementById('<%=txt_shorthours.ClientID %>');


            if (policyname.value == "") {
                alert("Policy Name cannot be blank !!!");
                policyname.focus();
                return false;
            }

            if (working_hours.value == "") {
                alert("working_hours cannot be blank !!!");
                working_hours.focus();
                return false;
            }

            if (shorthours.value == "") {
                alert("shorthours cannot be blank !!!");
                shorthours.focus();
                return false;
            }

            return true;

        }

        function submitItem() {

            if (confirm("Once you Submit you cannot make changes ?")) {
                return true;
            }
            return false;
        }


        function openWindow() {
            window.open("html/Attendance_policy_master.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
        }

    </script>

    <div class="container-fluid">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <%--<asp:UpdatePanel runat="server">
            <ContentTemplate>--%>
        <asp:Panel ID="Panel1" runat="server" class="panel panel-primary">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-9">

                        <div style="text-align: center; color: #fff; font-size: small;" class="text-center text-uppercase"><b>ATTENDANCE POLICY MASTER</b></div>

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
                        <div style="text-align: left; color:white; font-size: small;"><b>Attendance Policy Master Details</b></div>
                    </div>
                    <div class="col-sm-2 text-left">
                        
                    </div>
                </div>
            </div>
            <div class="panel-body">
                <div class="container-fluid" style="background: #f3f1fe; border: 1px solid #e2e2dd; border-radius: 10px; padding:15px 15px 15px 15px; margin-bottom:20px; margin-top:20px">
                <asp:Panel ID="panel2" runat="server">
                    <%--<asp:UpdatePanel ID="updatepanel1" runat="server" UpdateMode="Conditional">
                       <ContentTemplate>--%>


                    <asp:Panel ID="panel3" runat="server" CssClass="panel panel-primary ">
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-2  col-xs-12">
                                    <br />
                                   <b> Existing Policy Name </b>
                                        <asp:Label ID="Label10" runat="server" ForeColor="Red" Text=" * "></asp:Label>
                                </div>
                                <div class="col-md-2  col-xs-12">
                                    <br />
                                    <%--<asp:DropDownList ID="ddl_paid_through" runat="server" class="form-control">
                             </asp:DropDownList>--%>
                                    <asp:DropDownList AppendDataBoundItems="true" ID="ddl_Existing_policy_name" runat="server" class="form-control text_box"
                                        DataTextField="txt_policy_name" DataValueField="id" AutoPostBack="true" OnSelectedIndexChanged="ddl_Existing_policy_name_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <br />
                                    <asp:TextBox ID="txt_policy_name1" runat="server" class="form-control"
                                        placeholder=" New Policy Name : " onkeypress="return AllowAlphabet(event)"></asp:TextBox>

                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <br />
                                    <asp:TextBox ID="txt_start_date" runat="server" class="form-control date-picker1" placeholder="Start Date :"></asp:TextBox>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                    <br />
                                    <asp:TextBox ID="txt_end_date" runat="server" class="form-control date-picker2" placeholder="End Date :"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <br />
                                   <b>If End Date blank policy will not expire.</b>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_id" runat="server" Visible="false" class="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <br />
                        </div>
                        </div>
                    </asp:Panel>
        <br />
        <br />

                    <ul class="nav nav-tabs" style="background: #f3f1fe; padding:20px 20px 20px 20px; border-radius:10px">
                        <li id="tabactive5" class="active"><a data-toggle="tab" href="#menu1"><b>In/Out Time Details</b></a></li>

                        <li id="tabactive11"><a data-toggle="tab" href="#menu3"><b>Other Details</b></a></li>
                    </ul>

                    <div class="tab-content">

                        <div id="menu1" class="tab-pane fade in active">
                            <br />
                            <br />
                            <%--  <asp:Panel ID="panel5" runat="server" CssClass="panel panel-primary ">--%>

                            <div class="col-sm-2 col-xs-12">
                               <b> In Time :</b>
                                  <asp:TextBox ID="txt_intime" runat="server" class="form-control text_box" onkeypress="return AllowAlphabet_address(event)">0</asp:TextBox>
                            </div>
                            <div class="col-sm-2 col-xs-12">
                               <b> Out Time :</b>
                                  <asp:TextBox ID="txt_outtime" runat="server" class="form-control text_box" onkeypress="return AllowAlphabet_address(event)">0</asp:TextBox>
                            </div>

                            <%--</asp:Panel>--%>
                        </div>




                        <div id="menu3" class="tab-pane">
                            <br />

                            <%-- <asp:Panel ID="panel4" runat="server" CssClass="panel panel-primary ">--%>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                   <b> Working Hours : </b>
                                            <asp:Label ID="Label1" runat="server" ForeColor="Red" Text=" * "></asp:Label>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_working_hours" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                   <b> Roll :</b>

                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:DropDownList ID="ddl_roll" class="form-control" runat="server">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="1">Glatt</asp:ListItem>
                                        <asp:ListItem Value="2">Third Party</asp:ListItem>
                                        <asp:ListItem Value="3">Peace rate</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                   <b> Transaction :</b>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:DropDownList ID="ddl_transaction" class="form-control" runat="server">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="1">Pay Sheet</asp:ListItem>
                                        <asp:ListItem Value="2">Invoice Party</asp:ListItem>

                                    </asp:DropDownList>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                   <b> Week Off : </b>
                                           
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:DropDownList ID="ddl_Weekoff" class="form-control" runat="server">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="1">All_Sunday_and_1_3_Saturday</asp:ListItem>
                                        <asp:ListItem Value="1">All_Sunday_and_2_4_Saturday</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                  <b>  Early In : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_Early_In" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                  <b>  Early Out : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_Earlyout" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                  <b>  Let_In :</b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_letin" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                   <b> Let_Out :</b> 
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_letout" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                   <b> Reminders: </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:DropDownList ID="ddl_reminder" class="form-control" runat="server">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="1">Daily</asp:ListItem>
                                        <asp:ListItem Value="2">Weekly Party</asp:ListItem>
                                        <asp:ListItem Value="3">Monthly</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                  <b>  Deduction Policy : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_txtdeductionpolicy" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                   <b> Short Hours : </b>
                                    <asp:Label ID="Label2" runat="server" ForeColor="Red" Text=" * "></asp:Label>

                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_shorthours" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                   <b> Gender : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:DropDownList ID="ddl_gender" class="form-control" runat="server">

                                        <asp:ListItem Value="0">Male</asp:ListItem>
                                        <asp:ListItem Value="1">Femal</asp:ListItem>

                                    </asp:DropDownList>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                   <b> Timings : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_timing" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                   <b> Limit of Hours :</b> 
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_limitofhours" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                   <b> Punch Regularization : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_punchrealization" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                   <b> Period : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_period" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                   <b> OD :</b> 
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:DropDownList ID="ddl_od" class="form-control" runat="server">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="1">YES</asp:ListItem>
                                        <asp:ListItem Value="2">NO</asp:ListItem>

                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                   <b> Minimum OT hrs : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_min_othrs" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                   <b> Max OT Hrs : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_max_othours" runat="server" class="form-control" onkeypress="return isNumber(event)"></asp:TextBox>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                   <b> Approval Required : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:DropDownList ID="ddl_approval" class="form-control" runat="server">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="1">YES</asp:ListItem>
                                        <asp:ListItem Value="2">NO</asp:ListItem>

                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                   <b> Comp Off : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_commonoff" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-12">
                                   <b> OT Rate : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_otrate" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                   <b> General Remarks : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:TextBox ID="txt_generalremark" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                   <b> Auto Shift : </b>
                                            
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:CheckBox ID="chk_Autoshift" runat="server" />
                                </div>



                            </div>
                            <br />
                        </div>
                        <%--  </asp:Panel>--%>
                    </div>
                    <br />
                    <div class="row text-center">
                        <br />
                        <br />
                        <br />
                        <br />
                        Note: You can save as Draft many times as you want but once you submit you cannot make changes.<br />
                        After Submit Successfully Then You  Get Details In Apply Travelling Plan.
                                        
                    </div>
                    <br />
        <br />
        <br />
                    <div class="row text-center">
                        <asp:Button ID="btnadd" runat="server" class="btn  btn-large" OnClientClick="return validate();" OnClick="btnadd_Click" Text="Save as Draft" />

                        <asp:Button ID="btn_submit" runat="server" class="btn  btn-primary" OnClientClick="return submitItem();" OnClick="btn_submit_Click" Text="Submit" />

                        <asp:Button ID="btndelete" runat="server" OnClientClick="return confirm('Are you sure you want to delete this Policy?');" class="btn  btn-primary" OnClick="btndelete_Click" Text="Delete" />

                        <asp:Button ID="btncloselow" runat="server" class="btn btn-danger" OnClick="btncloselow_Click" Text="Close" />

                    </div>
                    <br />

                    <%--  </ContentTemplate>
                   </asp:UpdatePanel>--%>
                </asp:Panel>
            </div>

        </asp:Panel>
        <%--</ContentTemplate>

        </asp:UpdatePanel>--%>
    </div>
</asp:Content>



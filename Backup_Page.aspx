<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Backup_Page.aspx.cs" Inherits="Backup_Page" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Backup Page</title>
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
      <script>

          $(function () {

              $('#<%=btn_backup.ClientID%>').click(function () {

                    if (backup_validation()) {
                        $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
                    }
                });

            });
        </script>
        <script type="text/javascript">

            
                $(document).ready(function () {
                    var st = $(this).find("input[id*='hidtab']").val();
                    if (st == null)
                        st = 0;
                    $('[id$=tabs]').tabs({ selected: st });
                });

            
                function pageload()
                {

                    $.fn.dataTable.ext.errMode = 'none';
                    var table = $('#<%=gv_pass_reset.ClientID%>').DataTable({
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
                       .appendTo('#<%=gv_pass_reset.ClientID%>_wrapper .col-sm-6:eq(0)');



                    $('#<%=gv_pass_reset.ClientID%> td').click(function () {
                        $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
                    });

                }



            function backup_validation() {
                var user_id = document.getElementById('<%=txt_user_id.ClientID %>');
                var password = document.getElementById('<%=txt_password.ClientID %>');

                if (user_id.value == "") {
                    alert("Please Enter User Id !!!");
                    user_id.focus();
                    return false;
                }

                if (password.value == "") {
                    alert("Please Enter Password !!!");
                    password.focus();
                    return false;
                }

                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
                return true();

            }


            function isNumber(evt) {
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

            function generatepassword_validation() {

            var txt_empcode = document.getElementById('<%=txt_emp_login_id.ClientID %>');
                if (txt_empcode.value == "") {
                alert("Please enter employee code !!");
                txt_empcode.focus();
                return false;
            }

            var txt_password = document.getElementById('<%=txtPassword.ClientID %>');
                if (txt_password.value == "") {
                alert("Please enter password !!");
                txt_password.focus();
                return false;
            }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true();

        }


            function openWindow() {
                window.open("html/Backup_Page.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
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
        </script>
       
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="cph_righrbody">
   
       <asp:ScriptManager ID="ScriptManager1" runat="server" />
  
        <asp:Panel ID="Panel1" runat="server">

            <%--                         <div class="row" style="text-align:center; width:700px;">
                            
                                <asp:Button
                                    ID="Button1" runat="server" class="btn btn-success"
                                    Text="Backup" CausesValidation="False" OnClick="btn_BackUp_Click" OnClientClick="return backup_validation();"/>  
                                
                                <asp:Button
                                ID="Button3" runat="server" class="btn btn-danger"
                                Text="CLOSE" CausesValidation="False" OnClick="btn_Close_Click"/>
                            
                         </div>  --%>

            <%--  <div class="col-sm-1">
                        <asp:Button
                            ID="Button2" runat="server" class="btn btn-danger"
                            Text="CLOSE" CausesValidation="False" OnClick="btn_Close_Click"/>
                     </div>--%>
        </asp:Panel>

        <div class="container-fluid">
            <asp:Panel ID="Panel_new" runat="server" CssClass="panel panel-primary">
                <div class="panel-heading">
                    <div class="row">
                        <div class="col-sm-1"></div>
                        <div class="col-sm-9">
                            <div style="text-align: center; color: #fff; font-size: small;"><b>BACKUP AND DOWNLOAD</b></div>
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
                        <div style="text-align: left; color:white; font-size: small;"><b>Backup And Download Details</b></div>
                    </div>
                    <div class="col-sm-2 text-left">
                        
                    </div>
                </div>
            </div>
                <div class="panel-body">
                    <div class="container-fluid" style="background: #f3f1fe; border: 1px solid #e2e2dd; border-radius: 10px; padding:25px 25px 25px 25px; margin-bottom:20px; margin-top:20px">
                    <div class="row">

                        <div class="col-sm-3 col-xs-12">
                           <b> User Id :</b>
                               
                                   <asp:TextBox ID="txt_user_id" runat="server" class="form-control" onKeyPress="return  AllowAlphabet_address(event)"></asp:TextBox>
                        </div>
                        <div class="col-sm-3 col-xs-12">
                          <b>  Password :</b>
                               
                                   <asp:TextBox ID="txt_password" runat="server" class="form-control" TextMode="Password" onKeyPress="return  AllowAlphabet_address(event)"></asp:TextBox>
                        </div>


                        <br />


                        <%--<div class="col-sm-12 text-center">--%>
                        <asp:Button
                            ID="btn_backup" runat="server" class="btn btn-primary"
                            Text="Backup" CausesValidation="False" OnClick="btn_BackUp_Click" OnClientClick="return backup_validation();" />

                        &nbsp&nbsp
                    <asp:Button
                        ID="btnclose" runat="server" class="btn btn-danger"
                        Text="Close" CausesValidation="False" OnClick="btn_Close_Click" />
                        <%--</div>--%>
                    </div>
                    <br />
                    <br />

                     <div class="row">

                         <div class="col-sm-3 col-xs-6">
                          <b>  Employee Login Id:</b>
                               
                                   <asp:TextBox ID="txt_emp_login_id" runat="server" class="form-control" ></asp:TextBox>
                        </div>

                        <div class="col-sm-3 col-xs-12">
                            <b>Password:</b>
                               
                                   <asp:TextBox ID="txtPassword" runat="server" class="form-control" ></asp:TextBox>
                        </div>

                         <div class="col-sm-3 col-xs-8">
                          <b> Orignal Password :</b>
                               
                                   <asp:TextBox ID="txt_orignalpassword" runat="server" class="form-control"></asp:TextBox>
                        </div>

                        <div class="col-sm-4 col-xs-14">
                         <b>  Generate sha256 key :</b>
                               
                                   <asp:TextBox ID="txt_generatesha256" runat="server" class="form-control" ></asp:TextBox>
                        </div>


                        <br />


                        <%--<div class="col-sm-12 text-center">--%>
                        <asp:Button
                            ID="btn_password" runat="server" class="btn btn-large"
                            Text="Generate Password" OnClick="btn_PasswordGenerat_Click" OnClientClick="return generatepassword_validation();"/>

                    
                    </div>






                    <%--   <div class="panel panel-primary">
                <div class="panel-heading">
                    <h4 style="color:white;font-weight:bold;">Database BackUp</h4>
                </div>

                <div class="panel-body">
                                    <div class="container">
                                        <div class="row">
                                            <div class="col-sm-2">Location:</div>  
                                            <div class="col-sm-3">
                                                    <asp:TextBox ID="txt_backup_loc"
                                                        runat="server" MaxLength="50" class="form-control"></asp:TextBox>
                                            </div>  
                                         </div> 

                                        <div class="row" style="text-align:center">
                                            <asp:Button
                                                 ID="btn_browse" runat="server" class="btn btn-primary"
                                                 Text="Browse" CausesValidation="False" OnClick="btn_Browse_Click"/> 
                                             <asp:Button
                                                 ID="btn_backup" runat="server" class="btn btn-success"
                                                 Text="Backup" CausesValidation="False" OnClick="btn_BackUp_Click"/> 
                                        </div>
                          
                                   </div>
                    
                    </div>
       </div>
                    --%>
                    <%--    <div class="row text-center">
                        <asp:Button
                            ID="btnclose" runat="server" class="btn btn-danger"
                            Text="CLOSE" CausesValidation="False" OnClick="btn_Close_Click"/> 
             </div>--%>
                </div>

                    <div class="container-fluid">
                          <div id="tabs" style="background: beige; border-color: gray">
                    <asp:HiddenField ID="hidtab" Value="0" runat="server" />
                    <ul>
                        <li><a id="A1" href="#menu0" runat="server">Password Reset</a></li>
                       


                    </ul>
                              <%--/ / pass reset code komal--%>
                         <div id="menu0">
                    <div class="row">

                        <div class="col-sm-2 col-xs-12">
                                        <b>Client Name :</b> <span class="text-red" style="color: red">*</span>
                                        <asp:DropDownList ID="ddl_client" DataValueField="client_code" DataTextField="client_name" AutoPostBack="true" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="0">Select</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>


                        </div>


                               <br />
                           <div class="container-fluid">

                           <asp:Panel ID="gv_reset" runat="server" meta:resourcekey="gv_resetResource1" CssClass="grid-view" > <%--Style="overflow-x: auto;"--%>

                        <div class="row text-center">
                            <h3>Password Reset</h3>
                        </div>

                             <%--  <div class="row">
                            <div class="col-sm-10 col-xs-12"></div>
                            <div class="col-sm-2 col-xs-12" id="for_reset_gv_search" runat="server">
                                <b>Search :</b>
                                <asp:TextBox runat="server" ID="txt_search_pass_reset" CssClass=" form-control" onkeyup="Search_Gridview_pass_reset(this)" />
                            </div>
                        </div>--%>
                               <br />

                        <asp:GridView ID="gv_pass_reset" runat="server" ForeColor="#333333" meta:resourcekey="gv_pass_resetResource1" OnRowDataBound="gv_pass_reset_RowDataBound" OnPreRender="gv_pass_reset_PreRender" AutoGenerateColumns="false" class="table" >
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

                                 <%-- <asp:TemplateField HeaderText="SELECT " ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chk_client" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                              
                                <asp:TemplateField HeaderText="Sr No.">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                              
                                 <asp:BoundField DataField="ROLE" HeaderText="Type"
                                    SortExpression="ROLE" />

                                <asp:BoundField DataField="USER_NAME" HeaderText="User Name"
                                    SortExpression="USER_NAME" />

                                 <asp:BoundField DataField="LOGIN_ID" HeaderText="Login Id"
                                    SortExpression="LOGIN_ID" />

                                <asp:TemplateField HeaderText="New Password">
                                        <ItemStyle Width="50px" />
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="txt_new_pass"  Width="180px" CssClass="form-control"></asp:TextBox> <%--Text='<%# Eval("USER_PASSWORD") %>'--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                <asp:TemplateField HeaderText="Conform Password">
                                        <ItemStyle Width="50px" />
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="txt_conf_pass"  Width="180px" CssClass="form-control"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                   

                                 <asp:TemplateField HeaderText="Reset">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btn_reset" Text="Reset Password" runat="server" Width="55%" CssClass="btn btn-primary" Style=" color:white" OnClick="btn_reset_Click" ></asp:LinkButton>
                                        </ItemTemplate>
                                            
                                    </asp:TemplateField>



                               
                                   

                                 
                            </Columns>
                        </asp:GridView>

                    </asp:Panel>

                            </div>



                             </div>

                        </div>

                        </div>
                   
            </asp:Panel>
        </div>

   
</asp:Content>

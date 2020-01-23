<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaymentsFinished.aspx.cs"
Inherits="V2_PayementsFinished" UICulture="auto" Culture="auto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <!-- Bootstrap -->
    <link href="../Content/bootstrap.min.css" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="container-fluid">
                <div class="row-fluid">
                    <div class="span2">
                        <!--Sidebar content-->

                    </div>
                    <div class="span8" style="height:800px;height: auto;">
                        <div class="row-fluid" style="color: white; background-color: black;">
                           
                            <br/>
                             <br />
                            <br />
                        </div>
                        <div class="row-fluid" style="color: white; background-color: black;">

                            <div class="span10">
                                <p class="lead"><font size="18px"><b>Body</b><i>Architect</i></font></p>
                            </div>

                            <div class="span2">
                                <p align="right">
                                    <img class="img-rounded" src="../Content/images/Logo.png" />
                                </p>
                            </div>



                        </div>

                        <div class="row-fluid">
                            <br />
                            <br />
                            <br />
                            <p >
                                <asp:Label ID="lblPayPalDescription" runat="server" Text="<%$ Resources:Payments.aspx,PayPalTransactionFinished %>" /> 
                                <asp:Label ID="lblTransferujDescription" runat="server" Text="<%$ Resources:Payments.aspx,TransferujTransactionFinished %>" /> 
                            </p>
                            <br />
                            <br />
                            <br />
                            <p >
                                <asp:Label   runat="server" Text="<%$ Resources:Payments.aspx,PaymentFinishedRefreshNeededMessage %>" ></asp:Label>
                            </p>
                             
                        </div>



                       
                        
                    
                        
                    </div>

                    <div class="span2">
                        <!--Sidebar content-->

                    </div>
                </div>
            </div>
    </form>
    <script src="http://code.jquery.com/jquery-latest.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
</body>
</html>

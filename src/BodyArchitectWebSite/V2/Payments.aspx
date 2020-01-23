<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Payments.aspx.cs" Inherits="V2_Payments"    %>

<%@ Register TagPrefix="mn" Namespace="ASP" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" Title="<%$ Resources:Payments.aspx,PageResource1.Title %>">
    <!-- Bootstrap -->
    <link href="../Content/bootstrap.min.css" rel="stylesheet">
</head>
<body>
    <mn:GhostForm ID="MyRenderForm" runat="server">
        <div>


            <div class="container-fluid">
                <div class="row-fluid">
                    <div class="span2">
                        <!--Sidebar content-->

                    </div>
                    <div class="span8" style="height:800px;height: auto;">
                        <div class="row-fluid" style="color: white; background-color: black;">
                           
                            <p align="right">
                                <asp:Label runat="server" Text="<%$ Resources:Payments.aspx,lblUserNameHello.Text %>"  />
                                <asp:Label ID="lblUserName" runat="server"  />
                            </p>
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
                            <p>
                                <asp:Label ID="lblDescription" runat="server" Text="<%$ Resources:Payments.aspx,lblDescription.Text %> "/> 
                            </p>
                            <br />
                            <br />
                        </div>

                        <%--<div class="row-fluid" style="text-align: center; color: orange" >
                            <br />
                            <h2 ><asp:Label ID="Label4" runat="server" Text="<%$ Resources:Payments.aspx,lblPromotion.Text %> "/> </h2>
                            <br />
                            <br />
                        </div>--%>

                        <table class="table table-hover">
                            <thead>
                                <th><asp:Label runat="server" Text="<%$ Resources:Payments.aspx,lblTHPoints.Text %>" /> </th>
                                <th><asp:Label runat="server" Text="<%$ Resources:Payments.aspx,lblTHDescription.Text %>" /> </th>
                                <th><asp:Label runat="server" Text="<%$ Resources:Payments.aspx,lblTHPrice.Text %>" /></th>
                                <th><asp:Label runat="server" Text="<%$ Resources:Payments.aspx,lblTHCreditCard.Text %>"  /></th>
                                <th><asp:Label runat="server" Text="<%$ Resources:Payments.aspx,lblTHPayPal.Text %>"  /></th>
                            </thead>
                            <tbody class="table-hover">
                                <tr>
                                    <td style="vertical-align: middle;">
                                        <h5><asp:Label runat="server" Text="<%$ Resources:Payments.aspx,lbl30Points.Text %>"  /></h5>
                                    </td>
                                    
                                    <td style="vertical-align: middle;">
                                        <small>
                                            <asp:Label runat="server" Text="<%$ Resources:Payments.aspx,lbl30PointsDescription.Text %>"  />
                                            </small>
                                    </td>

                                    <td style="vertical-align: middle;">
                                        <%--<font color="red"><del>25zł (≈6€)</del></font>--%>
                                        <h4>15zł (≈4€)</h4>
                                    </td>

                                    <td style="vertical-align: middle;">
                                    <div id="transferuj30Form" runat="server">
                                        <form  action="https://secure.transferuj.pl" method="post" accept-charset="utf-8">
                                            <input type="hidden" name="id" value="<% Response.Write(TransferujId);%>"/>
                                            <input type="hidden" name="kwota" value="<% Response.Write(TransferujKwota);%>"/>
                                            <input type="hidden" name="opis" value="<% Response.Write(TransferujOpis);%>"/>
                                            <input type="hidden" name="crc" value="<% Response.Write(TransferujCrc);%>"/>
                                            <input type="hidden" name="md5sum" value="<% Response.Write(TransferujMd5);%>"/>
                                            <input type="hidden" name="wyn_url" value="<% Response.Write(TransferujNotifyUrl);%>"/>
                                            <input type="hidden" name="wyn_email" value="romanp81@gmail.com"/>
                                            <input type="hidden" name="jezyk" value="<% Response.Write(Language);%>"/>
                                            <input type="hidden" name="pow_url" value="<% Response.Write(PowrotTransferujUrl);%>"/>
                                            
                                            <input type="image" src="https://transferuj.pl/img/platnosci-internetowe/transferuj-kup-teraz-102x34.png" width="102" height="34" />

                                        </form>
                                    </div>
                                        
                                        <div id="przelewy30Form" runat="server">
                                        <form  action="<% Response.Write(Przelewy24Url);%>" method="post" > 
                                          <input type="hidden" name="p24_session_id" value="<% Response.Write(Przelewy24SessionId);%>" /> 
                                          <input type="hidden" name="p24_id_sprzedawcy" value="<% Response.Write(Przelewy24Id);%>" /> 
                                          <input type="hidden" name="p24_kwota" value="<% Response.Write(Przelewy24Kwota);%>" /> 
                                          <input type="hidden" name="p24_opis" value="BAPoints_30" /> 
                                          <input type="hidden" name="p24_email" value="<% Response.Write(ProfileEmail);%>" /> 
                                          <input type="hidden" name="p24_language" value="<% Response.Write(Language);%>" /> 
                                          <input type="hidden" name="p24_return_url_ok" value="<% Response.Write(Przelewy24NotifyUrl);%>" /> 
                                          <input type="hidden" name="p24_return_url_error" value="<% Response.Write(Przelewy24NotifyUrl);%>" /> 
                                          <input type="hidden" name="p24_crc" value="<% Response.Write(Przelewy24Crc);%>" /> 
                                          <input type="image" src="../Content/images/przelewy24_4.png" border="0" name="submit" />
                                        </form>
                                        </div>
                                         
                                    </td>

                                    <td style="vertical-align: middle;">
                                        <form action="<% Response.Write(PayPalUrl);%>" method="post">
                                            

                                            <input type="hidden" name="cmd" value="_s-xclick"/>
                                            <input type="hidden" name="hosted_button_id" value="<% Response.Write(PayPalButton30);%>"/>
                                            <input type="hidden" name="notify_url" value="<% Response.Write(PayPalNotifyUrl);%>"/>
                                            <input type="hidden" name="custom" value="<% Response.Write(ProfileId);%>"/>
                                            <input type="image" src="https://www.paypalobjects.com/en_US/GB/i/btn/btn_buynowCC_LG.gif" border="0" runat="server" name="submit"  />
                                            <img alt="" border="0" src="https://www.paypalobjects.com/pl_PL/i/scr/pixel.gif" width="1" height="1"/>
                                        </form>
                                    </td>
                                </tr>
                                
                                
                               

                                <tr>
                                    <td style="vertical-align: middle;">
                                        <%--<font color="red"><del>120</del></font>--%>
                                        <h4><asp:Label runat="server" Text="<%$ Resources:Payments.aspx,lbl120Points.Text %>"  /></h4>
                                    </td>
                                    <td style="vertical-align: middle;">
                                        <small>
                                        <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Payments.aspx,lbl120PointsDescription.Text %>"  />
                                        </small>
                                    </td>
                                    <td style="vertical-align: middle;">
                                        <%--<font color="red"><del>90 zł (≈22€)</del></font>--%>
                                        <h3>60 zł (≈15€)</h3>
                                    </td>
                                    <td style="vertical-align: middle;">

                                    <div id="transferuj120Form" runat="server">
                                     <form  action="https://secure.transferuj.pl" method="post" accept-charset="utf-8">
                                            <input type="hidden" name="id" value="<% Response.Write(TransferujId);%>"/>
                                            <input type="hidden" name="kwota" value="<% Response.Write(Transferuj120Kwota);%>"/>
                                            <input type="hidden" name="opis" value="<% Response.Write(Transferuj120Opis);%>" />
                                            <input type="hidden" name="crc" value="<% Response.Write(Transferuj120Crc);%>"/>
                                            <input type="hidden" name="md5sum" value="<% Response.Write(Transferuj120Md5);%>"/>
                                            <input type="hidden" name="wyn_url" value="<% Response.Write(TransferujNotifyUrl);%>"/>
                                            <input type="hidden" name="wyn_email" value="romanp81@gmail.com"/>
                                            <input type="hidden" name="jezyk" value="<% Response.Write(Language);%>"/>
                                            <input type="hidden" name="pow_url" value="<% Response.Write(PowrotTransferujUrl);%>"/>
                                            <input type="image" src="https://transferuj.pl/img/platnosci-internetowe/transferuj-kup-teraz-102x34.png" width="102" height="34" />
                                        </form>
                                    </div>
                                        
                                        <div id="przelewy120Form" runat="server">
                                            <form   action="<% Response.Write(Przelewy24Url);%>" method="post" > 
                                              <input type="hidden" name="p24_session_id" value="<% Response.Write(Przelewy24SessionId120);%>" /> 
                                              <input type="hidden" name="p24_id_sprzedawcy" value="<% Response.Write(Przelewy24Id);%>" /> 
                                              <input type="hidden" name="p24_kwota" value="<% Response.Write(Przelewy24Kwota120);%>" /> 
                                              <input type="hidden" name="p24_opis" value="BAPoints_120" /> 
                                              <input type="hidden" name="p24_language" value="<% Response.Write(Language);%>" /> 
                                              <input type="hidden" name="p24_email" value="<% Response.Write(ProfileEmail);%>" /> 
                                              <input type="hidden" name="p24_return_url_ok" value="<% Response.Write(Przelewy24NotifyUrl);%>" /> 
                                              <input type="hidden" name="p24_return_url_error" value="<% Response.Write(Przelewy24NotifyUrl);%>" /> 
                                              <input type="hidden" name="p24_crc" value="<% Response.Write(Przelewy24Crc120);%>" /> 
                                              <input type="image" src="../Content/images/przelewy24_4.png" border="0" name="submit" />
                                            </form> 
                                        </div>
                                        
                                    </td>

                                    <td style="vertical-align: middle;">
                                        <form action="<% Response.Write(PayPalUrl);%>" method="post">
                                            <input type="hidden" name="cmd" value="_s-xclick"/>
                                            <input type="hidden" name="hosted_button_id" value="<% Response.Write(PayPalButton120);%>"/>
                                            <input type="hidden" name="notify_url" value="<% Response.Write(PayPalNotifyUrl);%>"/>
                                            <input type="hidden" name="custom" value="<% Response.Write(ProfileId);%>"/>
                                            <input type="image" src="https://www.paypalobjects.com/en_US/GB/i/btn/btn_buynowCC_LG.gif" border="0" name="submit" />
                                            <img alt="" border="0" src="https://www.paypalobjects.com/pl_PL/i/scr/pixel.gif" width="1" height="1"/>
                                        </form>
                                    </td>
                                    
                                </tr>
                            </tbody>
                        </table>
                        
                        <br/>
                        <br/>
                        <p class="lead">
                        <asp:Label ID="Label2" runat="server" Text="<%$ Resources:Payments.aspx,PayPalCreditCardDescription %>"/> 
                        </p>
                        
                        <br/>
                        <br/>
                        
                        <p class="lead">

                        <p class="text-center">
                            <asp:Label ID="Label3" runat="server" Text="<%$ Resources:Payments.aspx,ProblemsInfo %>"/> 
                            <asp:HyperLink runat="server" NavigateUrl="mailto:admin@bodyarchitectonline.com" Text="<%$ Resources:Payments.aspx,ContactAddress %>"/>
                        </p>
                        </p>
                        <footer>
                            <center id="transferujFooter" runat="server">
                                <a href="https://transferuj.pl/jak-to-dziala.html" target="_blank" ><img src="https://transferuj.pl/img/platnosci-internetowe/transferuj-820x45.png" border="0"  width="820" height="45" /></a>
                            </center>
                            <center id="przelewyFooter" runat="server">
                                <a href="http://www.przelewy24.pl/cms,91,how_it_works.htm" target="_blank" ><img src="../Content/images/przelewy24_8.png" border="0"   /></a>
                            </center>
                        </footer>
                    </div>

                    <div class="span2">
                        <!--Sidebar content-->

                    </div>
                </div>
            </div>

        </div>
        
    </mn:GhostForm>

    <script src="http://code.jquery.com/jquery-latest.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
</body>
</html>

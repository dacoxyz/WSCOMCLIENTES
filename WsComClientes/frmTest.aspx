<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="frmTest.aspx.vb" Inherits="WsComClientes.frnTest" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>Verifica Operaciones</title>
    <link href="../Estilos/Estilos.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        var RelojID24 = null;
        var RelojEjecutandose24 = false;

        function DetenerReloj24 (){
	        if(RelojEjecutandose24)
		        clearTimeout(RelojID24);
	        RelojEjecutandose24 = false;
        }

        function MostrarHora24 () {
	        var ahora = new Date();
	        var horas = ahora.getHours();
	        var minutos = ahora.getMinutes();
	        var segundos = ahora.getSeconds();
	        var ValorHora;

	        //establece las horas
	        if (horas < 10)
	     	        ValorHora = "0" + horas;
	        else
		        ValorHora = "" + horas;

	        //establece los minutos
	        if (minutos < 10)
		        ValorHora += ":0" + minutos;
	        else
		        ValorHora += ":" + minutos;

	        //establece los segundos
	        if (segundos < 10)
		        ValorHora += ":0" + segundos;
	        else
		        ValorHora += ":" + segundos;
//            window.status = ValorHora    
	        document.form2.digitos.value = ValorHora;
	        //si se desea tener el reloj en la barra de estado, reemplazar la anterior por esta
	        //window.status = ValorHora

	        RelojID24 = setTimeout("MostrarHora24()",1000);
	        RelojEjecutandose24 = true;
        }
        function IniciarReloj24 () {
	        DetenerReloj24();
	        MostrarHora24();
        }
        window.onload = IniciarReloj24;
        if (document.captureEvents) {			//N4 requiere invocar la funcion captureEvents
	        document.captureEvents(Event.LOAD);
        }
        function IFRAME2_onclick() {
        }

    </script>
</head>
<body>
    <form id="form2" runat="server">
        <table style="width: 100%">
            <tr>
                <td style="height: 270px; width: 170px;">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                <td align="center" style="height: 270px; width: 1151px;">
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" background="Imagenes/bck_tran03.gif" id="TABLE1">
                        <tr>
                            <td align="left" valign="top" width="40%" style="height: 48px; border-left: black thin solid;">
                                <table border="0" cellpadding="0" cellspacing="0" width="219">
                                    <tr>
                                        <td style="height: 65px; width: 69px; border-left-width: thin; border-left-color: black;" valign="top">
                                            <img name="tran_box0101" src="Imagenes/tran_box0101.gif" border="0" width="57">
                                            <img name="tran_box0103" src="Imagenes/tran_box0103.gif"
                                                            border="0" style="width: 55px; height: 4px; border-left: black thin solid;"></td>
                                        <td style="height: 65px">
                                            <table border="0" cellpadding="0" cellspacing="0" width="162">
                                                <tr>
                                                    <td>
                                                        <img runat="server" id="msje" name="tran_box0102" src="Imagenes/tit_base.gif" border="0" alt="." height="34" width="162"></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <img name="tran_box0103" src="Imagenes/tran_box0103.gif" width="162"
                                                            border="0" style="height: 33px"></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td align="right" valign="top" style="width: 50%; height: 48px">
                                <table bgcolor="#ffffff" border="0" cellpadding="0" cellspacing="0" width="242">
                                    <tr>
                                        <td style="width: 245px">
                                            <img name="tran_box0201" src="Imagenes/tran_box0201Tx.gif"
                                                border="0" width="244"></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 245px; height: 39px; border-right: black thin solid;">
                                            <table bgcolor="#ffffff" border="0" cellpadding="0" cellspacing="0" width="242" style="height: 22px">
                                                <tr>
                                                    <td bgcolor="#ffffff">
                                                        <img src="Imagenes/spacer.gif" width="232" height="20" border="0"></td>
                                                    <td>
                                                        <img name="tran_box0203" src="Imagenes/tran_box0203.gif" width="11" height="20" border="0"></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="height: 186px">
                        <tr>
                            <td style="width: 1%; border-left: black 1px solid; height: 550px" valign="bottom"
                                align="left">
                                .</td>
                            <td align="center" valign="top">
                                <div style="margin: bottom unit" title="Test de Funcionalidad">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Size="Larger" Text="Test de funcionalidad Aplicaciones"></asp:Label><br />
                                    &nbsp;<asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Small" ForeColor="Black"></asp:Label>
                                    <asp:TextBox ID="digitos" runat="server" BorderWidth="0" Height="14px" MaxLength="10"
                                        Visible="true" Width="60px"></asp:TextBox><br />
                                    <asp:Button ID="btnTodo" runat="server" Text="Realízar Todas Las Pruebas" Width="209px" Font-Size="8pt" TabIndex="1" />
                                    <asp:Image ID="imgbtnTodo" runat="server" Visible="False" />
                                    <asp:Label ID="lblbtnTodo" runat="server"></asp:Label><br />
                                    <br />
                                    <table border="1" bordercolor="#330066" width="80%" style="border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none">
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Overline="False" Font-Size="Small"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="Black" Text="Pruebas de conexión con las Bases de Datos"
                                                    Width="297px" Font-Names="Arial"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;">
                                                <asp:Button ID="btnconexiones" runat="server" BorderColor="DimGray" Font-Overline="False"
                                                    Text="Probar Todas las CNN a BD" Width="251px" Font-Names="Arial" Font-Size="8pt" TabIndex="2" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" align="left">
                                                <asp:Image ID="imgbtnconexiones" runat="server" Visible="False" />
                                                <asp:Label ID="lblConexiones" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label6" runat="server" Font-Bold="False" Font-Overline="False" Font-Size="8pt"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="Black" Text="Conexión COMEPS"
                                                    Width="245px" Font-Names="Arial"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;">
                                                <asp:Button ID="btnCxComeps" runat="server" BorderColor="DimGray" Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="3" />
                                            </td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; height: 21px;" align="left"><asp:Image ID="imgbtnCxComeps" runat="server" Visible="False" />
                                                <asp:Label ID="lblComeps" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label5" runat="server" Font-Bold="False" Font-Overline="False" Font-Size="8pt"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="Black" Text="Conexión COMADMO"
                                                    Width="245px" Font-Names="Arial"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;"><asp:Button ID="btnCxComadmo" runat="server" BorderColor="DimGray" Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="4" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; height: 21px;" align="left">
                                                <asp:Image ID="imgbtnCxComadmo" runat="server" Visible="False" />
                                                <asp:Label ID="lblComadmo" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 46%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" valign="baseline" colspan="2">
                                                <asp:Label ID="Label4" runat="server" BorderColor="DimGray" Font-Bold="True" Font-Size="Small"
                                                    Text="Pruebas de Conectividad con Servicios" Font-Names="Arial"></asp:Label></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" colspan="2">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label16" runat="server" BorderColor="DimGray" Font-Bold="True" Font-Size="Small"
                                                    Text="Conectividad con WsComclientes" Font-Names="Arial"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;">
                                                <asp:Button ID="btnWscomclientes" runat="server" BorderColor="DimGray" BorderStyle="Outset"
                                                    Text="Probar Métodos Wscomclientes" Width="251px" Font-Size="8pt" TabIndex="5" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" align="left">
                                                <asp:Image ID="imgbtnWscomclientes" runat="server" Visible="False" />
                                                <asp:Label ID="lblbtnWscomclientes" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label18" runat="server" Font-Bold="False" Font-Names="Arial" Font-Overline="False"
                                                    Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="Black"
                                                    Text="Método ConsultaCliente" Width="245px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;"><asp:Button ID="btnWscomclientes1" runat="server" BorderColor="DimGray" Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="6" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" align="left"><asp:Image ID="imgbtnWscomclientes1" runat="server" Visible="False" />&nbsp;
                                                <asp:Label ID="lblbtnWscomclientes1" runat="server" Font-Names="Arial" Font-Size="8pt"
                                                    Width="58px"></asp:Label></td>
                                        </tr>
                                          <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label17" runat="server" Font-Bold="False" Font-Names="Arial" Font-Overline="False"
                                                    Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="Black"
                                                    Text="Método ActualizarEstadoRequisito" Width="245px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;">
                                                <asp:Button ID="btnActualizarReq" runat="server" BorderColor="DimGray" 
                                                    Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="6" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" align="left">
                                                <asp:Image ID="imgActEstReq" runat="server" Visible="False" />&nbsp;
                                                <asp:Label ID="lblActEstReq" runat="server" Font-Names="Arial" Font-Size="8pt"
                                                    Width="58px"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label19" runat="server" Font-Bold="False" Font-Names="Arial" Font-Overline="False"
                                                    Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="Black"
                                                    Text="Método ActualizarAfiliado" Width="245px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;">
                                                <asp:Button ID="btnActualizarAfil" runat="server" BorderColor="DimGray" 
                                                    Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="6" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" align="left">
                                                <asp:Image ID="imgActualizarAfil" runat="server" Visible="False" />&nbsp;
                                                <asp:Label ID="lblActualizarAfil" runat="server" Font-Names="Arial" Font-Size="8pt"
                                                    Width="58px"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label21" runat="server" Font-Bold="False" Font-Names="Arial" Font-Overline="False"
                                                    Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="Black"
                                                    Text="Método ActualizarEmpresa" Width="245px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;">
                                                <asp:Button ID="btnActEmpresa" runat="server" BorderColor="DimGray" 
                                                    Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="6" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" align="left">
                                                <asp:Image ID="ImgActEmpresa" runat="server" Visible="False" />&nbsp;
                                                <asp:Label ID="lblActEmpresa" runat="server" Font-Names="Arial" Font-Size="8pt"
                                                    Width="58px"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label7" runat="server" BorderColor="DimGray" Font-Bold="True" Font-Size="Small"
                                                    Text="Conectividad con WCompensar" Font-Names="Arial"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;">
                                                <asp:Button ID="btnWcompensar" runat="server" BorderColor="DimGray" BorderStyle="Outset"
                                                    Text="Probar Métodos WCompensar" Width="251px" Font-Size="8pt" TabIndex="5" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" align="left">
                                                <asp:Image ID="imgWcompensar" runat="server" Visible="False" />
                                                <asp:Label ID="lblWcompensar" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label8" runat="server" Font-Bold="False" Font-Names="Arial" Font-Overline="False"
                                                    Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="Black"
                                                    Text="Método ALDEA.AF" Width="245px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;"><asp:Button ID="btnALDEA" runat="server" BorderColor="DimGray" Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="6" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" align="left"><asp:Image ID="imgAldea" runat="server" Visible="False" />&nbsp;
                                                <asp:Label ID="lblAldea" runat="server" Font-Names="Arial" Font-Size="8pt"
                                                    Width="58px"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; height: 23px;" valign="baseline">
                                                <asp:Label ID="Label9" runat="server" Font-Bold="False" Font-Names="Arial" Font-Overline="False"
                                                    Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="Black"
                                                    Text="Método CAF.CAF" Width="245px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; height: 23px;"><asp:Button ID="btnCAF" runat="server" BorderColor="DimGray" Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="6" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; height: 23px;" align="left">
                                                <asp:Image ID="imgCAF" runat="server" Visible="False" />
                                                <asp:Label ID="lblCAF" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; height: 23px;" valign="baseline">
                                                <asp:Label ID="Label10" runat="server" Font-Bold="False" Font-Names="Arial" Font-Overline="False"
                                                    Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="Black"
                                                    Text="Método AFSYS.Ispec" Width="245px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; height: 23px;"><asp:Button ID="btnIspec" runat="server" BorderColor="DimGray" Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="7" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; height: 23px;" align="left">
                                               <asp:Image ID="imgInspec" runat="server" Visible="False" />
                                                <asp:Label ID="lblInspec" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label11" runat="server" BorderColor="DimGray" Font-Bold="True" Font-Names="Arial"
                                                    Font-Size="Small" Text="Conectividad con SIAM"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;">
                                                <asp:Button ID="BtnSiam" runat="server" BorderColor="DimGray" Text="Probar Métodos de SIAM" Width="251px" Font-Names="Arial" Font-Size="8pt" TabIndex="7" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" align="left">
                                                <asp:Image ID="imgBtnSiam" runat="server" Visible="False" />
                                                <asp:Label ID="lblBtnSiam" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label12" runat="server" Font-Bold="False" Font-Names="Arial" Font-Overline="False"
                                                    Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="Black"
                                                    Text="Método ConsultaAfiliados" Width="245px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;">
                                                <asp:Button ID="BtnSiam1" runat="server" BorderColor="DimGray" Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="8" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" align="left"><asp:Image ID="imgBtnSiam1" runat="server" Visible="False" />
                                                <asp:Label ID="lblBtnSiam1" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" valign="baseline">
                                                </td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;">
                                                </td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" align="right">
                                                &nbsp; &nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label14" runat="server" BorderColor="DimGray" Font-Bold="True" Font-Names="Arial"
                                                    Font-Size="Small" Text="Conectividad con MASIVO" Width="328px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;">
                                                <asp:Button ID="btnMasivoConsultaT" runat="server" BorderColor="DimGray" Text="Probar Masivo" Width="249px" Font-Names="Arial" Font-Size="8pt" TabIndex="10" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" align="left"><asp:Image ID="imgbtnMasivoConsultaT" runat="server" Visible="False" />
                                                <asp:Label ID="lblbtnMasivoConsultaT" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label15" runat="server" Font-Bold="False" Font-Names="Arial" Font-Overline="False"
                                                    Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="Black"
                                                    Text="Metodo ConsultaParametrosSemana" Width="245px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;">
                                                <asp:Button ID="btnMasivoConsulta1" runat="server" BorderColor="DimGray" Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="11" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" align="left">
                                               <asp:Image ID="imgbtnMasivoConsulta1" runat="server" Visible="False" />
                                                <asp:Label ID="lblbtnMasivoConsulta1" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" valign="baseline">
                                                <asp:Label ID="Label13" runat="server" Font-Bold="False" Font-Names="Arial" Font-Overline="False"
                                                    Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="Black"
                                                    Text="Entradas a Log Windows" Width="245px"></asp:Label></td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;">
                                                <asp:Button ID="btnLogWin" runat="server" BorderColor="DimGray" Text="Probar" Width="89px" Font-Names="Arial" Font-Size="8pt" TabIndex="11" /></td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" align="left">
                                               <asp:Image ID="imgbtnLogWin" runat="server" Visible="False" />
                                                <asp:Label ID="lblbtnLogWin" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 61%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;" valign="baseline">
                                                </td>
                                            <td style="width: 56%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;">
                                                </td>
                                            <td style="width: 26%; text-align: left; border-top-style: none; border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none;">
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <asp:Panel ID="Panel1" runat="server" Height="10px" ScrollBars="Vertical" Visible="False"
                                    Width="378px">
                                    <asp:DataGrid ID="dgdAcceso" runat="server" AccessKey="5" BorderColor="Tan" BorderStyle="Solid"
                                        BorderWidth="1px" CellPadding="2" CssClass="cssLabel" Font-Size="Small" ForeColor="Black"
                                        Height="31px" ToolTip="Alt+4" Visible="False" Width="1px">
                                        <FooterStyle BackColor="Tan" />
                                        <SelectedItemStyle BackColor="Lavender" ForeColor="GhostWhite" />
                                        <PagerStyle BackColor="Moccasin" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
                                        <AlternatingItemStyle BackColor="Transparent" />
                                        <HeaderStyle CssClass="tit_tabla2" Font-Bold="True" />
                                    </asp:DataGrid>
                                    &nbsp;&nbsp;
                                    <table>
                                        <tr>
                                            <td class="cssSubTitulos" colspan="3" style="border-right: gray 1px solid; border-top: gray 1px solid;
                                                border-left: gray 1px solid; border-bottom: gray 1px solid">
                                                <img alt="." src="./Imagenes/bullet01.gif" /><span style="font-size: 10pt">Prueba de
                                                    Servicio</span></td>
                                        </tr>
                                        <tr>
                                            <td style="border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid;
                                                border-bottom: gray 1px solid; height: 28px">
                                            </td>
                                            <td class="cssLabel" style="font-size: 10pt; height: 28px">
                                                &nbsp;<asp:TextBox ID="TXTtIPO" runat="server" Visible="False" Width="17px">1</asp:TextBox>
                                                <asp:TextBox ID="txtNit" runat="server" Visible="False" Width="104px">1012361284</asp:TextBox>
                                                <asp:Button ID="btnAf" runat="server" CssClass="cssBoton" Font-Size="7pt" Text="Consulta AF" /></td>
                                            <td style="border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid;
                                                border-bottom: gray 1px solid; height: 28px">
                                                <asp:TextBox ID="txtID_APORTANTE" runat="server" ReadOnly="True" Visible="False"></asp:TextBox>
                                                <asp:Label ID="Label20" runat="server" EnableTheming="True" Font-Size="Small"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td class="cssLabel" colspan="3" style="border-right: gray 1px solid; border-top: gray 1px solid;
                                                border-left: gray 1px solid; border-bottom: gray 1px solid; height: 33px">
                                                <div id="Div11" align="left" style="overflow: auto; width: 400px; height: 23px">
                                                    <asp:DataGrid ID="DtGrdNit" runat="server" AccessKey="4" BorderColor="Tan" BorderStyle="Solid"
                                                        BorderWidth="1px" CellPadding="2" CssClass="cssLabel" Font-Size="Smaller" ForeColor="Black"
                                                        Height="106px" ToolTip="Alt+4" Visible="False">
                                                        <SelectedItemStyle BackColor="Lavender" ForeColor="GhostWhite" />
                                                        <AlternatingItemStyle BackColor="Transparent" />
                                                        <HeaderStyle CssClass="tit_tabla2" Font-Bold="True" />
                                                        <FooterStyle BackColor="Tan" />
                                                        <Columns>
                                                            <asp:TemplateColumn HeaderText="Sel">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="Checkbox1" runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                        </Columns>
                                                        <PagerStyle BackColor="Moccasin" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
                                                    </asp:DataGrid>
                                                </div>
                                                <div id="Div4" align="left" style="overflow: auto; width: 400px; height: 22px">
                                                    <asp:DataGrid ID="DataGrid1" runat="server" AccessKey="4" BorderColor="Tan" BorderStyle="Solid"
                                                        BorderWidth="1px" CellPadding="2" CssClass="cssLabel" Font-Size="Smaller" ForeColor="Black"
                                                        ToolTip="Alt+4" Visible="False">
                                                        <SelectedItemStyle BackColor="Lavender" ForeColor="GhostWhite" />
                                                        <AlternatingItemStyle BackColor="Transparent" />
                                                        <HeaderStyle CssClass="tit_tabla2" Font-Bold="True" />
                                                        <FooterStyle BackColor="Tan" />
                                                        <Columns>
                                                            <asp:TemplateColumn HeaderText="Sel">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="Checkbox1" runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                        </Columns>
                                                        <PagerStyle BackColor="Moccasin" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
                                                    </asp:DataGrid>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:TextBox ID="txtIdentity" runat="server" Visible="False" Width="112px">9999803423383716</asp:TextBox>&nbsp;
                                    <asp:TextBox ID="txtMensajeq" runat="server" CssClass="cssLabel" Height="15px" Visible="False"
                                        Width="89%"></asp:TextBox></asp:Panel>
                            </td>
                            <td style="width: 1%; border-right: black 1px solid; height: 550px" valign="bottom"
                                align="right">
                                .</td>
                        </tr>
                    </table>
                    <table border="0" cellpadding="0" cellspacing="0" style="background: url(Imagenes/bck_tran06.gif)"
                        width="100%">
                        <tr>
                            <td align="left" style="width: 3%; height: 15px" valign="bottom">
                                <img alt="" height="15" src="Imagenes/cor_botlf.gif" /></td>
                            <td style="width: 94%; height: 15px">
                                <img alt="" height="15" src="Imagenes/bck_tran06.gif" /></td>
                            <td align="right" style="width: 3%; height: 15px" valign="bottom">
                                <img alt="" height="15" src="Imagenes/cor_botrg.gif" /></td>
                        </tr>
                    </table>
                </td>
                <td style="height: 270px; width: 91px;">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
            </tr>
        </table>
    </form>
</body>
</html>

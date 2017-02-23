Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Globalization
Imports System.Configuration
Imports ManejoMensajes
Imports System.Xml
Imports System.Threading
Imports System.Data
Imports Utilidades.CUtil '2702
Imports Compensar.SISPOS.ESL.Vinculacion
Imports Compensar.Vincula.POS

<System.Web.Services.WebService(Namespace:="http://tempuri.org/WsComClientes/Afiliaciones")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Afiliaciones
    Inherits System.Web.Services.WebService

    Dim ObjReg As New Proceso.ClsRegistro
    Dim ObjRad As New Proceso.ClsRadicacion
    Dim ObjVin As New Proceso.ClsVinculacion
    Dim ObjUtil As New Utilidades.CUtil

#Region "CAMBIO DE ESTADO Y ESTRATO:"
    ''' <summary>
    ''' Atiende solicitudes de cambio estado con radicacion previa de requisitos en la cla01.
    ''' </summary>
    ''' <param name="sAplicacion">codigo de aplicacion.</param>
    ''' <param name="sXmlParametros">xml con parametros.</param>
    ''' <returns>respuesta de CLE15.</returns>
    ''' <remarks>Modificado el 8 de sept de 2009.</remarks>
    <WebMethod()> _
    Public Function CambioEstadoRadicacion(ByVal sAplicacion As String, ByVal sXmlParametros As String) As String
        'System.Diagnostics.EventLog.WriteEntry("SISPOS", "TRAMA RECIBIDA CambioEstadoRadicacion: " & sXmlParametros)
        Dim strRepuesta As String = ""
        Dim Valores As String()
        Dim intContar As Integer
        Dim intContador As Integer = 0
        Dim AfiliacionClase As New Proceso.CAfiliacion
        sAplicacion = ConfigurationSettings.AppSettings("AfilPos_ProjectID")
        If Not (sXmlParametros.Contains("DE2400")) Then
            If AfiliacionClase.CambioEstado(sXmlParametros, strRepuesta, sAplicacion) Then
                Valores = Split(strRepuesta, ",")
                intContar = Valores.Length
                For intContador = 0 To intContar - 1
                    strRepuesta = Valores(intContador)
                    If strRepuesta.Contains("Radica") Then
                        strRepuesta = "<Req><Respuesta>" + VinculacionInc(sAplicacion, Valores(intContador)) + "</Respuesta></Req>"
                    Else
                        strRepuesta = "<Req><Respuesta>" + Valores(intContador) + "</Respuesta></Req>"
                    End If
                Next
            Else
                strRepuesta = "<Req><Error>" + strRepuesta + "</Error></Req>"
            End If
        Else
            Dim objUtil As New Utilidades.CUtil
            sXmlParametros = objUtil.TransformaDE2400(sXmlParametros)
            strRepuesta = VinculacionInc(sAplicacion, sXmlParametros)
        End If
        Return strRepuesta
    End Function
    ''' <summary>
    ''' atiende solicitudes de cambio estrato para proceso masivo mensual.
    ''' </summary>
    ''' <param name="sAplicacion">codigo de aplicacion</param>
    ''' <param name="sXmlParametros">xml con parametros.</param>
    ''' <returns>respuesta de CLE15.</returns>
    ''' <remarks>Setp 8 de 2009.</remarks>
    <WebMethod()> _
    Public Function actualizaEstrato(ByVal sAplicacion As String, ByVal sXmlParametros As String) As String
        Dim strRepuesta As String = ""
        Dim AfiliacionClase As New Proceso.CAfiliacion
        If AfiliacionClase.actualizaEstrato(sXmlParametros, strRepuesta) Then
            '<NewDataSet><row><tide_cliente>CC</tide_cliente><nide_cliente>79801350</nide_cliente><tide_empresa>NI</tide_empresa><nide_enpresa>811028936</nide_enpresa><condic_afil>2</condic_afil><indic_opcion>V</indic_opcion><nuip_cliente></nuip_cliente><nreq>NOAPOR</nreq><finireq>20090708</finireq><ffinreq>29991231</ffinreq><vreq>NI</vreq><vnumreq>811028936</vnumreq></row></NewDataSet>
            strRepuesta = "<Req><Respuesta>" + strRepuesta + "</Respuesta></Req>"
        Else
            strRepuesta = "<Req><Error>" + strRepuesta + "</Error></Req>"
        End If
        Return strRepuesta
    End Function
    ''' <summary>
    ''' realiza cambio de estado directo sin pasar por radicacion de requisitos. 
    ''' </summary>
    ''' <param name="sAplicacion">codigo aplicacion</param>
    ''' <param name="sXmlParametros">xml de parametros cambio estado vinculacion.</param>
    ''' <returns></returns>
    ''' <remarks>Setp 8 2009, desarrollado para proceso masivo cambio estado.</remarks>
    <WebMethod()> _
    Public Function actualizaEstado(ByVal sAplicacion As String, ByVal sXmlParametros As String) As String
        '9309 :: DAC Se ingresa bitacora de proceso en windows
        Dim strBefore As String = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")
        'CManejadorMensajes.PublicarMensaje("EntradaSW: " & sXmlParametros & DateTime.Now.ToString, EventLogEntryType.Warning)
        Dim strRepuesta As String = ""
        Dim AfiliacionClase As New Proceso.CAfiliacion
        If AfiliacionClase.actualizaEstado(sXmlParametros, strRepuesta) Then
            '<NewDataSet><row><tide_cliente>CC</tide_cliente><nide_cliente>79801350</nide_cliente><tide_empresa>NI</tide_empresa><nide_enpresa>811028936</nide_enpresa><condic_afil>2</condic_afil><indic_opcion>V</indic_opcion><nuip_cliente></nuip_cliente><nreq>NOAPOR</nreq><finireq>20090708</finireq><ffinreq>29991231</ffinreq><vreq>NI</vreq><vnumreq>811028936</vnumreq></row></NewDataSet>
            strRepuesta = "<Req><Respuesta>" + strRepuesta + "</Respuesta></Req>"
        Else
            strRepuesta = "<Req><Error>" + strRepuesta + "</Error></Req>"
        End If
        '9309 :: DAC Se ingresa bitacora de proceso en windows
        CManejadorMensajes.PublicarMensaje("ProcesoSW: " & sXmlParametros & ";" & strBefore & ";" & DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), EventLogEntryType.Information)
        Return strRepuesta

    End Function
#End Region


    <WebMethod()> _
    Public Function Afiliar(ByVal sAplicacion As String, ByVal sParametros As String) As String
        Try
            If ConfigurationSettings.AppSettings("Fase") Is Nothing Then
                Return AfiliarW(sAplicacion, sParametros)
            Else
                If ConfigurationSettings.AppSettings("Fase") = "" Then
                    Return AfiliarW(sAplicacion, sParametros)
                Else
                    Return AfiliarOrg(sAplicacion, sParametros)
                End If

            End If
        Catch ex As Exception
            Return ex.Message & " - " & ex.Source & " - " & ex.StackTrace
        End Try
    End Function
    <WebMethod()> _
    Public Function devolverMensaje(ByVal strRepuesta As String, ByVal Comando As String, ByVal fecha As String, ByVal boolPos As String) As String
        Dim AfiliacionClase As New Proceso.CAfiliacion
        AfiliacionClase.ManejoErrorVinc(strRepuesta, Comando, fecha, boolPos)
        Return strRepuesta & " - " & Comando & " - " & fecha & " - " & boolPos
    End Function
    Private Function AfiliarOrg(ByVal sAplicacion As String, ByVal sParametros As String) As String
        Dim objAfiliacion As New EntidadesNegocio.CDatosAfiliacion
        Dim strRepuesta As String = ""
        Dim boolPos As Boolean
        Dim boolCaj As Boolean
        Dim AfiliacionClase As New Proceso.CAfiliacion
        If AfiliacionClase.ConstruirDatoAfiliacion(sParametros, objAfiliacion, boolPos, boolCaj, strRepuesta, sAplicacion) Then
            CManejadorMensajes.PublicarMensaje("LogicaTransaccional:AfiliarOrg:Parametros: " & sParametros & " POS:" & boolPos & " CCF:" & boolCaj & " APL:" & sAplicacion, EventLogEntryType.Information)
            If AfiliacionClase.Afiliar(objAfiliacion, strRepuesta, boolCaj, boolPos) Then
                If objAfiliacion.Comando = "" And strRepuesta = "" And objAfiliacion.Fecha = "" Then
                    objAfiliacion.Comando = "EGE"
                    strRepuesta = "Afiliado con Exito. Transacción exitosa."
                    Dim mFormatter As DateTimeFormatInfo
                    mFormatter = New CultureInfo("es-CO", True).DateTimeFormat
                    Thread.CurrentThread.CurrentUICulture = New CultureInfo("es-CO", True)
                    Thread.CurrentThread.CurrentCulture = New CultureInfo("es-CO", True)
                    objAfiliacion.Fecha = DateTime.Now.ToString(mFormatter)
                End If
                strRepuesta = "<Afiliar><Repuesta>" + strRepuesta + "</Repuesta><Comando>" + objAfiliacion.Comando + "</Comando><Fecha>" + objAfiliacion.Fecha + "</Fecha><EsPos>" + boolPos.ToString() + "</EsPos><EsCaj>" + boolCaj.ToString() + "</EsCaj></Afiliar> "
            Else
                strRepuesta = "<Afiliar><Repuesta>" + strRepuesta + "</Repuesta><Comando></Comando><Fecha></Fecha><EsPos></EsPos><EsCaj></EsCaj></Afiliar> "
            End If
        Else
            strRepuesta = "<Afiliar><Repuesta>" + strRepuesta + "</Repuesta><Comando></Comando><Fecha></Fecha><EsPos></EsPos><EsCaj></EsCaj></Afiliar> "
        End If
        Return strRepuesta
    End Function
    Private Function AfiliarW(ByVal sAplicacion As String, ByVal sParametros As String) As String
        Dim objAfiliacion As New EntidadesNegocio.CDatosAfiliacion
        Dim strRepuesta As String = ""
        Dim boolPos As Boolean
        Dim boolCaj As Boolean
        Dim AfiliacionClase As New Proceso.CAfiliacion
        '2702
        Dim objUtils As New Utilidades.CUtil()
        strRepuesta = objUtils.gValidaTags(sParametros)

        If strRepuesta <> "" Then
            Return "<Afiliar><Repuesta>" + strRepuesta + "</Repuesta><Comando></Comando><Fecha></Fecha><EsPos></EsPos><EsCaj></EsCaj></Afiliar> "
        End If
        '2702
        If AfiliacionClase.ConstruirDatoAfiliacion(sParametros, objAfiliacion, boolPos, boolCaj, strRepuesta, sAplicacion) Then
            CManejadorMensajes.PublicarMensaje("LogicaTransaccional:AfiliarW:Parametros: " & sParametros & " POS:" & boolPos & " CCF:" & boolCaj & " APL:" & sAplicacion, EventLogEntryType.Information)
            If AfiliacionClase.AfiliarW(objAfiliacion, strRepuesta, boolCaj, boolPos) Then
                'MODIFICADO LCARDENAS 31-01-2012: SOLUCION INCIDENCIA 7402,8313
                If strRepuesta = "0" Then
                    Dim NroIdentificacion As String
                    Dim TipoIdentificacioin As String
                    NroIdentificacion = "0"
                    TipoIdentificacioin = "0"
                    NroIdentificacion = sParametros.Substring(CInt(sParametros.IndexOf("<NroIdentificacion>")) + 19, CInt(sParametros.IndexOf("</NroIdentificacion>")) - CInt(sParametros.IndexOf("<NroIdentificacion>")) - 19)
                    TipoIdentificacioin = sParametros.Substring(CInt(sParametros.IndexOf("<TipoIdent>")) + 11, 1)
                    strRepuesta = AfiliacionClase.consultarNautcli(ConfigurationSettings.AppSettings("ProjectID"), NroIdentificacion, TipoIdentificacioin)
                End If
                Dim Comando As String = ""
                Dim Fecha As String = ""
                If AfiliacionClase.ConstruirDatoVinculacion(sParametros, sAplicacion) Then
                    CManejadorMensajes.PublicarMensaje("LogicaTransaccional:VinculacionesIN:Parametros: " & sParametros & " NAUCLI:" & strRepuesta & " APL:" & sAplicacion, EventLogEntryType.FailureAudit)
                    strRepuesta = Me.Vinculaciones(sAplicacion, sParametros, strRepuesta)
                    CManejadorMensajes.PublicarMensaje("LogicaTransaccional:VinculacionesOUT:Parametros: " & sParametros & " NAUCLI:" & strRepuesta & " APL:" & sAplicacion, EventLogEntryType.Information)
                    AfiliacionClase.ManejoErrorVinc(strRepuesta, Comando, Fecha, boolPos)
                End If
                If String.IsNullOrEmpty(strRepuesta) Then
                    strRepuesta = "Error Sistema no disponible, Excepcion inesperada"
                    strRepuesta = "<Afiliar><Repuesta>" + strRepuesta + "</Repuesta><Comando></Comando><Fecha></Fecha><EsPos></EsPos><EsCaj></EsCaj></Afiliar> "
                Else
                    strRepuesta = "<Afiliar><Repuesta>" + strRepuesta + "</Repuesta><Comando>" + Comando + "</Comando><Fecha>" + Fecha + "</Fecha><EsPos>" + boolPos.ToString() + "</EsPos><EsCaj>" + boolCaj.ToString() + "</EsCaj></Afiliar> "
                End If
            Else
                If String.IsNullOrEmpty(strRepuesta) Then
                    strRepuesta = "Error Sistema no disponible, Excepcion inesperada"
                End If
                strRepuesta = "<Afiliar><Repuesta>" + strRepuesta + "</Repuesta><Comando></Comando><Fecha></Fecha><EsPos></EsPos><EsCaj></EsCaj></Afiliar> "
            End If
        Else
            If String.IsNullOrEmpty(strRepuesta) Then
                strRepuesta = "Error Sistema no disponible, Excepcion inesperada"
            End If
            strRepuesta = "<Afiliar><Repuesta>" + strRepuesta + "</Repuesta><Comando></Comando><Fecha></Fecha><EsPos></EsPos><EsCaj></EsCaj></Afiliar> "
        End If
        Return strRepuesta
    End Function
    <WebMethod()> Public Function Requisitos(ByVal sAplicacion As String, ByVal sParametros As String) As String
        Try
            Dim xmlDoc As New Xml.XmlDocument()
            Dim xmlNodo As Xml.XmlNode
            Dim strRepuesta As String = ""
            Dim AfiliacionClase As New Proceso.CAfiliacion
            If AfiliacionClase.Requisitos(sParametros, strRepuesta, sAplicacion) Then
                xmlDoc.LoadXml(strRepuesta)
                xmlNodo = xmlDoc.SelectSingleNode("//MESSAGE")
                strRepuesta = "<Req><Repuesta>" + xmlNodo.InnerXml.ToString() + "</Repuesta></Req> "
            Else
                strRepuesta = "<Req><Repuesta>" + strRepuesta + "</Repuesta></Req> "
            End If
            Return strRepuesta
        Catch ex As Exception
            Return ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
    End Function

    <WebMethod()> Public Function ConsultaCliente(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim returnvalue As String = ""
        Dim ObjUtil As New Utilidades.CUtil
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim xmldoc As New XmlDocument
                    With ObjReg
                        .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                        If ds.Tables(0).Columns.Contains("TipoIdent") Then
                            .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("NroIdentificacion") Then
                            .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Partealfabetica") Then
                            .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("DigitoChequeo") Then
                            .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        Else
                            .DigitoChequeo = ""
                        End If
                        If ds.Tables(0).Columns.Contains("Sucursal") Then
                            .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("CentroCosto") Then
                            .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Usuario") Then
                            .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Programa") Then
                            .Programa = ds.Tables(0).Rows(0)("Programa").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Condicion") Then
                            .Condicion = ds.Tables(0).Rows(0)("Condicion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Grupo") Then
                            .Grupo = ds.Tables(0).Rows(0)("Grupo").ToString()
                        Else
                            .Grupo = ""
                        End If
                        If ds.Tables(0).Columns.Contains("TipoIdentificacionTrabajadorResponsable") Then
                            .TipoIdentificacion1 = ds.Tables(0).Rows(0)("TipoIdentificacionTrabajadorResponsable").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("TrabajadorResponsable") Then
                            .Responsable1 = ds.Tables(0).Rows(0)("TrabajadorResponsable").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("TipoIdentificacionEmpresaResponsable") Then
                            .TipoIdentificacion2 = ds.Tables(0).Rows(0)("TipoIdentificacionEmpresaResponsable").ToString()
                        ElseIf ds.Tables(0).Columns.Contains("TipoIdentEmpr") Then
                            .TipoIdentificacion2 = ds.Tables(0).Rows(0)("TipoIdentEmpr").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("EmpresaResponsable") Then
                            .Responsable2 = ds.Tables(0).Rows(0)("EmpresaResponsable").ToString()
                        ElseIf ds.Tables(0).Columns.Contains("NroIdentEmpr") Then
                            .Responsable2 = ds.Tables(0).Rows(0)("NroIdentEmpr").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("FechaInicioAfiliacion") Then
                            .FechaInicioAfiliacion = ds.Tables(0).Rows(0)("FechaInicioAfiliacion").ToString()
                        End If
                        '4249 20150505
                        If ds.Tables(0).Columns.Contains("ConsecutivoDir") Then
                            .Ndir = ds.Tables(0).Rows(0)("ConsecutivoDir").ToString()
                        Else
                            .Ndir = 1
                        End If
                        '4249 20150505

                        Return .ConsultarCliente()
                    End With
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function
    <WebMethod()> Public Function ActualizaCliente(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim returnvalue As String = ""
        Dim ObjUtil As New Utilidades.CUtil
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim xmldoc As New XmlDocument
                    With ObjReg
                        .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                        .OperacionMod = ds.Tables(0).Rows(0)("Operacion").ToString()
                        .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        .Nombre = ds.Tables(0).Rows(0)("Nombre").ToString()
                        .PrimerApellido = ds.Tables(0).Rows(0)("PrimerApellido").ToString()
                        .SegundoApellido = ds.Tables(0).Rows(0)("SegundoApellido").ToString()
                        .TipoIdentOrg = ds.Tables(0).Rows(0)("TipoIdentOrg").ToString()
                        .NroIdentificacionOrg = ds.Tables(0).Rows(0)("NroIdentificacionOrg").ToString()
                        .PartealfabeticaOrg = ds.Tables(0).Rows(0)("PartealfabeticaOrg").ToString()
                        .DigitoChequeoOrg = ds.Tables(0).Rows(0)("DigitoChequeoOrg").ToString()
                        .sucursalOrg = ds.Tables(0).Rows(0)("sucursalOrg").ToString()
                        .CentroCostoOrg = ds.Tables(0).Rows(0)("CentroCostoOrg").ToString()
                        .ConfirmaApellNomb = ds.Tables(0).Rows(0)("ConfirmaApellNomb").ToString()
                        .Fechanacimiento = ds.Tables(0).Rows(0)("Fechanacimiento").ToString()
                        If ds.Tables(0).Columns.Contains("Genero") Then
                            .Genero = ds.Tables(0).Rows(0)("Genero").ToString()
                        End If
                        .EstadoCivil = ds.Tables(0).Rows(0)("EstadoCivil").ToString()
                        .Direccion = ds.Tables(0).Rows(0)("Direccion").ToString()
                        '4249 20140505
                        If ds.Tables(0).Columns.Contains("ConsecutivoDir") Then
                            .Ndir = ds.Tables(0).Rows(0)("ConsecutivoDir").ToString()
                            Try
                                If .Ndir >= "2" Then    '4249 20150209
                                    Dim dstRequisitos As New DataSet, dtRequisitos() As DataTable = Nothing, dtr() As DataRow
                                    Try
                                        If .Identity = "" Then
                                            .Identity = ConsultaAF.ConsultaNautcliSegunIdentificacion(.Aplicacion, .NroIdentificacion, darCodigoTipoId(.TipoIdent), _
                                                                                                        IIf(.Sucursal = "", 0, .Sucursal), IIf(.CentroCosto = "", 0, .CentroCosto), _
                                                                                                        .Partealfabetica)
                                        End If
                                        dstRequisitos = ConsultaAF.ConsultarRequisitoPorNautcli(.Aplicacion, .Identity)
                                        'FA2
                                        dtr = dstRequisitos.Tables(0).Select("XNMNCAM='" & "AFIEMP" & "' AND CMNT='A'")
                                        If dtr.Length = 0 Then
                                            '4249 20150417
                                            Return xmlDataSetResultadoMsje(0, "ERROR POS: Cliente no tiene asociado el requisito AFIEMP")
                                        End If
                                    Catch
                                        '4249 20150417
                                        Return xmlDataSetResultadoMsje(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento - ERROR: Consulta Requisitos")
                                    End Try

                                    '    Dim oBL As ReglasVinculacion
                                    '    If Not oBL.requisitoX_Alfa(.Aplicacion, .Identity, "AFIEMP", "1", "=") Then
                                    '        '4249 se verifican que tenga requisitos vigentes
                                    '        returnvalue = "<Req><Error>No se cuenta con el requisito AFIEMP</Error></Req>"


                                    '        Return returnvalue
                                    '    End If
                                End If
                            Catch ex As Exception

                            End Try
                        End If
                        '4249 20140505

                        .Telefono = ds.Tables(0).Rows(0)("Telefono").ToString()
                        .Extension = ds.Tables(0).Rows(0)("Extension").ToString()
                        .TipoDireccion = ds.Tables(0).Rows(0)("TipoDireccion").ToString()
                        .Barrio = ds.Tables(0).Rows(0)("Barrio").ToString()
                        .Zona = ds.Tables(0).Rows(0)("Zona").ToString()
                        .Ciudad = ds.Tables(0).Rows(0)("Ciudad").ToString()
                        .Detalleadicional = ds.Tables(0).Rows(0)("Detalleadicional").ToString()
                        .Razonsocial = ds.Tables(0).Rows(0)("Razonsocial").ToString()
                        .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                        .señalConfirmaFonetico = ds.Tables(0).Rows(0)("senalConfirmaFonetico").ToString()
                        .señalConfirma = ds.Tables(0).Rows(0)("senalConfirma").ToString()
                        .DireccionOrg = ds.Tables(0).Rows(0)("DireccionOrg").ToString()
                        .ConfirmaEstadoCivil = ds.Tables(0).Rows(0)("ConfirmaEstadoCivil").ToString()
                        returnvalue = .ModificarCliente()
                        '0010514
                        Try
                            If returnvalue.Contains("IDENTIFICACION ACTUALIZADA") Then
                                Dim strXml As String
                                strXml = "<RADICACION><Radica><TipoIdent>" & darNombreTipoId(.TipoIdent) & "</TipoIdent><NroIdentificacion>" & .NroIdentificacion & "</NroIdentificacion><DigitoChequeo/><Nombre/><Primerapellido/><Segundoapellido/><Fechanacimiento/><Estadocivil/><Genero/><Detalleadicional/><Accion1>D</Accion1><Requisito1>REQDTO</Requisito1><FechaInicialVigencia1/><FechaFinalVigencia1/><DatoNumerico1/><DatoAlfabetico1/><Usuario>999999999999</Usuario></Radica></RADICACION>"
                                Radicar(.Aplicacion, strXml)
                            End If
                        Catch ex As Exception

                        End Try
                        '0010514
                        Return returnvalue
                    End With
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function


    '1360
    <WebMethod(Description:="Metodo para modificar la informacion de ubicacion de un cliente desde otros aplicativos")> _
    Public Function ActualizarUbicacionCliente(ByVal strApl As String, _
            ByVal intOpc As Integer, ByVal strXml As String) As String
        Dim sAplicacion As String
        Dim returnvalue As String = ""
        Dim ObjUtil As New Utilidades.CUtil
        sAplicacion = ConfigurationSettings.AppSettings("AfilPos_ProjectID")
        Try
            If intOpc <> 1 Then
                Return "<Req><Mensaje>" & "ERROR POS: Opción no es válida." & "<Mensaje></Req>"
            End If
            Dim strXMLDatosBasicos As String = ""
            strXMLDatosBasicos = Utilidades.CUtil.ValidarXSD(strXml, (System.AppDomain.CurrentDomain.BaseDirectory + "XSD\TramaActualizarUbicacionCliente.xsd"))

            If strXMLDatosBasicos.Length > 4 Then
                ActualizarUbicacionCliente = "<Req><Mensaje>" & strXMLDatosBasicos & "</Mensaje></Req>"
                Return ActualizarUbicacionCliente
            End If
            strXMLDatosBasicos = ConsultaCliente(ConfigurationSettings.AppSettings("ProjectID"), strXml)

            If strXMLDatosBasicos.Contains("CLIENTE NO EXISTE") Then
                Return "<Req><Mensaje>" & "ERROR POS: Cliente No Existente en BD" & "</Mensaje></Req>"
            End If

            Dim ds As DataSet = ObjUtil.GetDataSet(strXml)
            Dim dsDatosBasicos As DataSet = ObjUtil.GetDataSet(strXMLDatosBasicos)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim xmldoc As New XmlDocument
                    With ObjReg
                        'Dim strXML2 As String = LoadxsdCreaClientes(PageOn, "CHG", "", .Rows(i)("TipoIdentNueva"), .Rows(i)("NroIdentificacionNueva"), "", _
                        '          "", "", "", _
                        '          dsDatosBasicos.Tables(0).Rows(0)("I_XNOMTRAB") + " " + dsDatosBasicos.Tables(0).Rows(0)("I_XDESRAZ"), dsDatosBasicos.Tables(0).Rows(0)("I_XPRIAPE"), dsDatosBasicos.Tables(0).Rows(0)("I_XSEGAPE"), _
                        '          "", dsDatosBasicos.Tables(0).Rows(0)("I_FNACCLI"), .Rows(i)("Genero"), .Rows(i)("Estadocivil"), .Rows(i)("Direccion"), .Rows(i)("Telefono"), _
                        '           .Rows(i)("Extension"), .Rows(i)("TipoDireccion"), .Rows(i)("Barrio"), .Rows(i)("Zona"), .Rows(i)("Ciudad"), dsDatosBasicos.Tables(0).Rows(0)("I_XNIVADI") _
                        '           , dsDatosBasicos.Tables(0).Rows(0)("I_XDESRAZ"), .Rows(i)("Usuario"), "", "", _
                        '          dsDatosBasicos.Tables(0).Rows(0)("I_XIDECLI"), dsDatosBasicos.Tables(0).Rows(0)("I_NIDECLI"), _
                        '          dsDatosBasicos.Tables(0).Rows(0)("I_XNUICLI"), "", _
                        '          "", "", _
                        '          dsDatosBasicos.Tables(0).Rows(0)("I_XDIRCLI"), dsDatosBasicos.Tables(0).Rows(0)("I_CECICLI"), "", "", "", Nothing, Nothing, "", "", 1)

                        .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                        .OperacionMod = ds.Tables(0).Rows(0)("Operacion").ToString()
                        .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        .TipoIdentOrg = .TipoIdent
                        .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        .NroIdentificacionOrg = .NroIdentificacion
                        Try
                            .Partealfabetica = dsDatosBasicos.Tables(0).Rows(0)("I_XNUICLI")
                        Catch ex As Exception
                            .Partealfabetica = ""
                        End Try
                        .PartealfabeticaOrg = .Partealfabetica
                        .DigitoChequeo = dsDatosBasicos.Tables(0).Rows(0)("I_NDIGCHE")
                        .DigitoChequeoOrg = .DigitoChequeo
                        If ds.Tables(0).Columns.Contains("Sucursal") Then
                            .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        Else
                            .Sucursal = dsDatosBasicos.Tables(0).Rows(0)("I_NIDESUC")
                        End If
                        .sucursalOrg = .Sucursal
                        If ds.Tables(0).Columns.Contains("CentroCosto") Then
                            .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        Else
                            .CentroCosto = dsDatosBasicos.Tables(0).Rows(0)("I_NIDECOS")
                        End If
                        .CentroCostoOrg = .CentroCosto

                        .Nombre = RTrim(dsDatosBasicos.Tables(0).Rows(0)("I_XNOMTRAB") + " " + dsDatosBasicos.Tables(0).Rows(0)("I_XDESRAZ"))    '3711 20140127
                        .PrimerApellido = dsDatosBasicos.Tables(0).Rows(0)("I_XPRIAPE")
                        .SegundoApellido = dsDatosBasicos.Tables(0).Rows(0)("I_XSEGAPE")
                        '.ConfirmaApellNomb = ds.Tables(0).Rows(0)("ConfirmaApellNomb").ToString()
                        .Fechanacimiento = dsDatosBasicos.Tables(0).Rows(0)("I_FNACCLI")
                        'If ds.Tables(0).Columns.Contains("Genero") Then
                        .Genero = dsDatosBasicos.Tables(0).Rows(0)("I_XGENCLI")
                        'End If
                        .EstadoCivil = dsDatosBasicos.Tables(0).Rows(0)("I_CECICLI")                            '3711 20140127
                        .DireccionOrg = dsDatosBasicos.Tables(0).Rows(0)("I_XDIRCLI")
                        .Direccion = .DireccionOrg
                        If ds.Tables(0).Columns.Contains("Direccion") Then
                            If ds.Tables(0).Rows(0)("Direccion").ToString().Length > 1 Then
                                .Direccion = ds.Tables(0).Rows(0)("Direccion").ToString()
                            End If
                        End If
                        '4249 20140505
                        If ds.Tables(0).Columns.Contains("ConsecutivoDir") Then
                            If ds.Tables(0).Rows(0)("ConsecutivoDir").ToString().Length >= 1 Then
                                .Ndir = ds.Tables(0).Rows(0)("ConsecutivoDir").ToString()
                            End If
                        End If
                        '4249 20140505
                        .Telefono = dsDatosBasicos.Tables(0).Rows(0)("I_NTELCLI")
                        If ds.Tables(0).Columns.Contains("Telefono") Then
                            If ds.Tables(0).Rows(0)("Telefono").ToString().Length > 1 Then
                                .Telefono = ds.Tables(0).Rows(0)("Telefono").ToString()
                            End If
                        End If
                        .Extension = dsDatosBasicos.Tables(0).Rows(0)("I_NEXTCLI")
                        If ds.Tables(0).Columns.Contains("Extension") Then
                            If ds.Tables(0).Rows(0)("Extension").ToString().Length > 0 Then
                                .Extension = ds.Tables(0).Rows(0)("Extension").ToString()
                            End If
                        End If
                        .TipoDireccion = dsDatosBasicos.Tables(0).Rows(0)("I_XTIPDIR")
                        If ds.Tables(0).Columns.Contains("TipoDireccion") Then
                            If ds.Tables(0).Rows(0)("TipoDireccion").ToString().Length > 0 Then
                                .TipoDireccion = ds.Tables(0).Rows(0)("TipoDireccion").ToString()
                            End If
                        End If
                        .Barrio = dsDatosBasicos.Tables(0).Rows(0)("I_XBARCLI")
                        If ds.Tables(0).Columns.Contains("Barrio") Then
                            If ds.Tables(0).Rows(0)("Barrio").ToString().Length > 1 Then
                                .Barrio = ds.Tables(0).Rows(0)("Barrio").ToString()
                            End If
                        End If
                        Dim bIndCiudad As Boolean
                        .Ciudad = dsDatosBasicos.Tables(0).Rows(0)("I_CCIUCLI")
                        If ds.Tables(0).Columns.Contains("Ciudad") Then
                            bIndCiudad = True
                            If ds.Tables(0).Rows(0)("Ciudad").ToString.Length > 1 Then
                                .Ciudad = ds.Tables(0).Rows(0)("Ciudad").ToString()
                            End If
                        End If

                        .Zona = IIf(dsDatosBasicos.Tables(0).Rows(0)("I_CZONBTA") = "0" OrElse dsDatosBasicos.Tables(0).Rows(0)("I_CZONBTA") = "", "99", dsDatosBasicos.Tables(0).Rows(0)("I_CZONBTA"))
                        If ds.Tables(0).Columns.Contains("Zona") Then
                            If ds.Tables(0).Rows(0)("Zona").ToString.Length > 0 Then
                                .Zona = ds.Tables(0).Rows(0)("Zona").ToString()
                            End If
                        ElseIf bIndCiudad Then
                            .Zona = "99"
                            '.Zona = IIf(dsDatosBasicos.Tables(0).Rows(0)("I_CZONBTA") = "0" OrElse dsDatosBasicos.Tables(0).Rows(0)("I_CZONBTA") = "", "999", dsDatosBasicos.Tables(0).Rows(0)("I_CZONBTA"))
                        End If

                        '.Detalleadicional = ds.Tables(0).Rows(0)("Detalleadicional").ToString()
                        '.Razonsocial = ds.Tables(0).Rows(0)("Razonsocial").ToString()
                        .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()

                        Dim dstReq As DataSet
                        dstReq = ConsultaAF.dstConsultaEga18(sAplicacion, "UPDFON")
                        Dim rowZona() As DataRow = dstReq.Tables("EGA18").Select
                        For Each dtReq As DataRow In rowZona
                            If dtReq.Item("XDES") = .Usuario Then
                                .señalConfirmaFonetico = "5"  'FONETICO
                            End If
                        Next

                        '.señalConfirmaFonetico = ds.Tables(0).Rows(0)("senalConfirmaFonetico").ToString()
                        '.señalConfirma = ds.Tables(0).Rows(0)("senalConfirma").ToString()
                        '.DireccionOrg = ds.Tables(0).Rows(0)("DireccionOrg").ToString()
                        '.ConfirmaEstadoCivil = ds.Tables(0).Rows(0)("ConfirmaEstadoCivil").ToString()
                        returnvalue = .ModificarCliente()
                        returnvalue = Replace(returnvalue, "Mensaje Estado Civil : 5211", "")
                        returnvalue = RTrim(Replace(returnvalue, "ERROR: NO ESTA CAMBIANDO ESTADO CIVIL", ""))
                        If returnvalue.Contains("ERROR") And returnvalue.Contains("EXITOS") Then
                            CManejadorMensajes.PublicarMensaje("RespuestaHost: " & returnvalue & ";" _
                                                            & strXml & ";" & strApl, EventLogEntryType.Information)

                            returnvalue = "<Req><Mensaje>" & "Mensaje Modificacion : MODIFICACIÓN EXITOSA" & "</Mensaje></Req>"
                        End If
                        Return returnvalue
                    End With
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
            If ConfigurationSettings.AppSettings("Activalog") = "S" Then
                CManejadorMensajes.PublicarMensaje("RespuestaHost: " & ex.Message & ";" _
                                                             & strXml & ";" & strApl, EventLogEntryType.Information)
            End If

        End Try
        Return returnvalue
    End Function
    '1360

    '0010514
    Public Function darNombreTipoId(ByVal intCodigo As String) As String
        Dim strNombre As String
        strNombre = intCodigo
        If intCodigo = "1" Then strNombre = "CC" ''
        If intCodigo = "2" Then strNombre = "NI" ''
        If intCodigo = "3" Then strNombre = "TI" ''
        If intCodigo = "4" Then strNombre = "CE" ''
        If intCodigo = "7" Then strNombre = "RC"
        If intCodigo = "8" Then strNombre = "NU" ''
        If intCodigo = "9" Then strNombre = "MS" ''--8992 se agrega el tipo de identificacion
        Return strNombre
    End Function
    '0010514
    <WebMethod()> Public Function CreaCliente(ByVal Aplicacion As String, ByVal parametros As String) As String
        Dim returnvalue As String = ""
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(parametros)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    With ObjReg
                        .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                        .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        .Nombre = ds.Tables(0).Rows(0)("Nombre").ToString()
                        .PrimerApellido = ds.Tables(0).Rows(0)("PrimerApellido").ToString()
                        .SegundoApellido = ds.Tables(0).Rows(0)("SegundoApellido").ToString()
                        .ConfirmaApellNomb = ds.Tables(0).Rows(0)("ConfirmaApellNomb").ToString()
                        .Fechanacimiento = ds.Tables(0).Rows(0)("Fechanacimiento").ToString()
                        .Genero = ds.Tables(0).Rows(0)("Genero").ToString()
                        .EstadoCivil = ds.Tables(0).Rows(0)("EstadoCivil").ToString()
                        .Direccion = ds.Tables(0).Rows(0)("Direccion").ToString()
                        .Telefono = ds.Tables(0).Rows(0)("Telefono").ToString()
                        .Extension = ds.Tables(0).Rows(0)("Extension")
                        .TipoDireccion = ds.Tables(0).Rows(0)("TipoDireccion").ToString()
                        .Barrio = ds.Tables(0).Rows(0)("Barrio").ToString()
                        .Zona = ds.Tables(0).Rows(0)("Zona").ToString()
                        .Ciudad = ds.Tables(0).Rows(0)("Ciudad").ToString()
                        .Detalleadicional = ds.Tables(0).Rows(0)("Detalleadicional").ToString()
                        .Razonsocial = ds.Tables(0).Rows(0)("Razonsocial").ToString()
                        .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                        .señalConfirmaFonetico = ds.Tables(0).Rows(0)("senalConfirmaFonetico").ToString()
                        .señalConfirma = ds.Tables(0).Rows(0)("senalConfirma").ToString()
                        returnvalue = .CrearCliente()
                    End With
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function
    <WebMethod()> Public Function CambiarFidelizacion(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim returnvalue As String = ""
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    With ObjReg
                        .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                        .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                    End With
                    returnvalue = ObjReg.Fidelizar()
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function
    <WebMethod()> Public Function ObtenerNuevoRadicado(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim returnvalue As String = ""
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    With ObjReg
                        .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                        .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                    End With
                    returnvalue = ObjReg.NuevoRadicado()
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function
    <WebMethod()> Public Function CambiarNivelAdicional(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim returnvalue As String = ""
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    With ObjReg
                        .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                        .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                    End With
                    returnvalue = ObjReg.NivelAdicional()
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function

    <WebMethod()> Function Radicar(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim returnvalue As String
        returnvalue = String.Empty

        Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)

        If Not ObjUtil.EvalDataSet(ds) Then
            returnvalue = AplicaRadicar(Aplicacion, ds)
        Else
            returnvalue = ModificaTrama(Aplicacion, ds)
        End If

        '0010514
        Try
            If Parametros.Contains("CERESC") And Not Parametros.Contains("REQESC") Then '-- 3339 And Not returnvalue.Contains("ERROR") Then
                Dim strXml As String, TipoIdent As String, NroIdentificacion As String
                If ds.Tables.Count > 0 Then
                    If ds.Tables(0).Rows.Count > 0 Then
                        If ds.Tables(0).Columns.Contains("TipoIdent") Then
                            TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("NroIdentificacion") Then
                            NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        End If
                        'Se envia trama de borrado
                        strXml = "<RADICACION><Radica><TipoIdent>" & darNombreTipoId(TipoIdent) & "</TipoIdent><NroIdentificacion>" & NroIdentificacion & "</NroIdentificacion><DigitoChequeo/><Nombre/><Primerapellido/><Segundoapellido/><Fechanacimiento/><Estadocivil/><Genero/><Detalleadicional/><Accion1>D</Accion1><Requisito1>REQESC</Requisito1><FechaInicialVigencia1/><FechaFinalVigencia1/><DatoNumerico1/><DatoAlfabetico1/><Usuario>999999999999</Usuario></Radica></RADICACION>"
                        Radicar(Aplicacion, strXml)
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
        '0010514

        Return returnvalue
    End Function

    Private Function ModificaTrama(ByVal Aplicacion As String, ByVal ds As DataSet) As String
        Dim returnvalue As String = ""
        Dim Dc As New DataColumn
        Dim vConta As Integer
        Dim i As Integer
        Dim vAccion As String
        vAccion = String.Empty
        vConta = 11
        Try
            Aplicacion = ConfigurationSettings.AppSettings("AfilPos_ProjectID")
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    For i = 0 To vConta
                        If ds.Tables(0).Columns.Contains("Requisito" & CStr(i + 1)) Then
                            vAccion = ds.Tables(0).Rows(0).Item("Accion" & CStr(i + 1))
                            If vAccion <> String.Empty Then
                                With ObjRad
                                    .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                                    If ds.Tables(0).Columns.Contains("TipoIdent") Then
                                        .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("NroIdentificacion") Then
                                        .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Partealfabetica") Then
                                        .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("DigitoChequeo") Then
                                        .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("DigitoChequeo") Then
                                        .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Sucursal") Then
                                        .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("CentroCosto") Then
                                        .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Nombre") Then
                                        .Nombre = ds.Tables(0).Rows(0)("Nombre").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("PrimerApellido") Then
                                        .PrimerApellido = ds.Tables(0).Rows(0)("PrimerApellido").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("SegundoApellido") Then
                                        .SegundoApellido = ds.Tables(0).Rows(0)("SegundoApellido").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Fechanacimiento") Then
                                        .Fechanacimiento = ds.Tables(0).Rows(0)("Fechanacimiento").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Genero") Then
                                        .Genero = ds.Tables(0).Rows(0)("Genero").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("EstadoCivil") Then
                                        .EstadoCivil = ds.Tables(0).Rows(0)("EstadoCivil").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Detalleadicional") Then
                                        .Detalleadicional = ds.Tables(0).Rows(0)("Detalleadicional").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Razonsocial") Then
                                        .Razonsocial = ds.Tables(0).Rows(0)("Razonsocial").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Usuario") Then
                                        .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Programa") Then
                                        .Programa = ds.Tables(0).Rows(0)("Programa").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Condicion") Then
                                        .Condicion = ds.Tables(0).Rows(0)("Condicion").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("Grupo") Then
                                        .Grupo = ds.Tables(0).Rows(0)("Grupo").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("TipoIdentOrg") Then
                                        .TipoIdentEmpr = ds.Tables(0).Rows(0)("TipoIdentOrg").ToString()
                                    End If
                                    If ds.Tables(0).Columns.Contains("NroIdentificacionOrg") Then
                                        .NroIdentEmpr = ds.Tables(0).Rows(0)("NroIdentificacionOrg").ToString()
                                    End If
                                    Dim infoadi As String = ""

                                    If ds.Tables(0).Columns.Contains("Accion" & CStr(i + 1)) Then
                                        .AccReq(0) = ds.Tables(0).Rows(0)("Accion" & CStr(i + 1)).ToString()  'Accion
                                    End If
                                    If ds.Tables(0).Columns.Contains("Requisito" & CStr(i + 1)) Then
                                        .NameReq(0) = ds.Tables(0).Rows(0)("Requisito" & CStr(i + 1)).ToString()  'Nemonico
                                    End If
                                    If ds.Tables(0).Columns.Contains("FechaInicialVigencia" & CStr(i + 1)) Then
                                        .FechIniReq(0) = ds.Tables(0).Rows(0)("FechaInicialVigencia" & CStr(i + 1)).ToString() 'Fecha Inicial de vigencia
                                    End If
                                    If ds.Tables(0).Columns.Contains("FechaFinalVigencia" & CStr(i + 1)) Then
                                        .FechFinReq(0) = ds.Tables(0).Rows(0)("FechaFinalVigencia" & CStr(i + 1)).ToString()  'Fecha Final de vigencia
                                    End If
                                    If ds.Tables(0).Columns.Contains("DatoNumerico" & CStr(i + 1)) Then
                                        .DatNumReq(0) = ds.Tables(0).Rows(0)("DatoNumerico" & CStr(i + 1)).ToString()  'Dato numerico
                                    End If
                                    If ds.Tables(0).Columns.Contains("DatoAlfabetico" & CStr(i + 1)) Then
                                        'dac 20120128 Se agrega la info adicional
                                        If infoadi = "" Then
                                            infoadi = BuildInfoAdirequisito(ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString())
                                        End If
                                        If ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString().Length > 30 Then
                                            .DatAlfReq(0) = ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString().Substring(0, 30)  'Dato Alfabetico
                                        Else
                                            .DatAlfReq(0) = ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString()
                                        End If

                                    End If

                                    If ds.Tables(0).Columns.Contains("InfoAdicional") Then
                                        .InfoAdicional = ds.Tables(0).Rows(0)("InfoAdicional").ToString()
                                    Else
                                        .InfoAdicional = infoadi 'dac 20120128 Se agrega la info adicional
                                    End If

                                    returnvalue = EjecutaRadicar(ObjRad)

                                End With
                            End If
                        End If
                    Next
                    'returnvalue = ObjRad.Radicacion()
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function

    Private Function EjecutaRadicar(ByRef pDataRadica As Proceso.ClsRadicacion) As String
        Return ObjRad.Radicacion()
    End Function
    '2100 20130617
    Private Function pReemplazarRequisitosAdicionales(ByVal Aplicacion As String, _
            ByVal ds As DataSet) As DataSet
        pReemplazarRequisitosAdicionales = Nothing
        Dim iPos As Integer = 0
        With ds.Tables(0)
            '2100 20130618
            For Each dr As DataRow In ds.Tables(2).Rows
                .Rows(0)("Accion" & CStr(iPos + 1)) = dr("Accion")
                .Rows(0)("Requisito" & CStr(iPos + 1)) = dr("Requisito")
                .Rows(0)("FechaInicialVigencia" & CStr(iPos + 1)) = dr("FechaInicialVigencia")
                .Rows(0)("FechaFinalVigencia" & CStr(iPos + 1)) = dr("FechaFinalVigencia")
                .Rows(0)("DatoNumerico" & CStr(iPos + 1)) = dr("DatoNumerico")
                .Rows(0)("DatoAlfabetico" & CStr(iPos + 1)) = dr("DatoAlfabetico")
                '       dr.Delete()
                iPos = iPos + 1
                If (iPos >= 11) Then
                    Exit For
                End If
                'Integer = 11 To 22
                'If ds.Tables(0).Columns.Contains("Accion" & CStr(i + 1)) Then
                '    .Rows(0)("Accion" & CStr(i - 11)) = ds.Tables(3).Rows(i)("Accion") '& CStr(i + 1)).ToString()  'Accion
                'End If
                'If ds.Tables(0).Columns.Contains("Requisito" & CStr(i + 1)) Then
                '    .Rows(0)("Requisito" & CStr(i - 11)) = ds.Tables(0).Rows(0)("Requisito" & CStr(i + 1)).ToString()  'Nemonico
                'End If
                'If ds.Tables(0).Columns.Contains("FechaInicialVigencia" & CStr(i + 1)) Then
                '    .Rows(0)("FechaInicialVigencia" & CStr(i - 11)) = ds.Tables(0).Rows(0)("FechaInicialVigencia" & CStr(i + 1)).ToString() 'Fecha Inicial de vigencia
                'End If
                'If ds.Tables(0).Columns.Contains("FechaFinalVigencia" & CStr(i + 1)) Then
                '    .Rows(0)("FechaFinalVigencia" & CStr(i - 11)) = ds.Tables(0).Rows(0)("FechaFinalVigencia" & CStr(i + 1)).ToString()  'Fecha Final de vigencia
                'End If
                'If ds.Tables(0).Columns.Contains("DatoNumerico" & CStr(i + 1)) Then
                '    .Rows(0)("DatoNumerico" & CStr(i - 11)) = ds.Tables(0).Rows(0)("DatoNumerico" & CStr(i + 1)).ToString()  'Dato numerico
                'End If

                'If ds.Tables(0).Columns.Contains("DatoAlfabetico" & CStr(i + 1)) Then
                '    .Rows(0)("DatoAlfabetico" & CStr(i - 11)) = ds.Tables(0).Columns.Contains("DatoAlfabetico" & CStr(i + 1)).ToString()
                'End If
            Next
            Dim iPosCon As Integer
            ' iPosCon = iPos
            For iPosCon = iPos To 1 Step -1
                ds.Tables(2).Rows(iPosCon - 1).Delete()
            Next


            '2100 20130619 Se llenan con vacios los requisitos que no se llenaron en la segunda tanda

            For iPosCon = iPos To 11
                .Rows(0)("Accion" & CStr(iPosCon + 1)) = ""
                .Rows(0)("Requisito" & CStr(iPosCon + 1)) = ""
                .Rows(0)("FechaInicialVigencia" & CStr(iPosCon + 1)) = ""
                .Rows(0)("FechaFinalVigencia" & CStr(iPosCon + 1)) = ""
                .Rows(0)("DatoNumerico" & CStr(iPosCon + 1)) = ""
                .Rows(0)("DatoAlfabetico" & CStr(iPosCon + 1)) = ""
            Next
        End With

        'Elimina la llave de nombre de columna
        'If ds.Tables(0).Columns.Contains("Accion12") Then
        '    ds.Tables(0).Columns.Remove("Accion12")
        'End If

        pReemplazarRequisitosAdicionales = ds
    End Function
    Private Function AplicaRadicar(ByVal Aplicacion As String, ByVal ds As DataSet) As String
        Dim ObjRadNew As New Proceso.ClsRadicacion  '2011 20130619
        Dim returnvalue As String = ""
        Try
            Aplicacion = ConfigurationSettings.AppSettings("AfilPos_ProjectID")
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim i As Integer
                    Dim infoadi As String = ""
                    With ObjRadNew 'ObjRad '2011 20130619
                        .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                        If ds.Tables(0).Columns.Contains("TipoIdent") Then
                            .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("NroIdentificacion") Then
                            .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Partealfabetica") Then
                            .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("DigitoChequeo") Then
                            .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("DigitoChequeo") Then
                            .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Sucursal") Then
                            .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("CentroCosto") Then
                            .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Nombre") Then
                            .Nombre = ds.Tables(0).Rows(0)("Nombre").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("PrimerApellido") Then
                            .PrimerApellido = ds.Tables(0).Rows(0)("PrimerApellido").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("SegundoApellido") Then
                            .SegundoApellido = ds.Tables(0).Rows(0)("SegundoApellido").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Fechanacimiento") Then
                            .Fechanacimiento = ds.Tables(0).Rows(0)("Fechanacimiento").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Genero") Then
                            .Genero = ds.Tables(0).Rows(0)("Genero").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("EstadoCivil") Then
                            .EstadoCivil = ds.Tables(0).Rows(0)("EstadoCivil").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Detalleadicional") Then
                            'DAC 20130125 CU-REGCLIENTES-029
                            If ds.Tables(0).Rows(0)("Detalleadicional").ToString() <> "PROVISIONAL" Then
                                .Detalleadicional = ds.Tables(0).Rows(0)("Detalleadicional").ToString()
                            Else 'DAC 20130125 CU-REGCLIENTES-029
                                infoadi = ds.Tables(0).Rows(0)("Detalleadicional").ToString() 'DAc 20121019 CU-REGCLIENTES-029
                            End If
                        End If
                        If ds.Tables(0).Columns.Contains("Razonsocial") Then
                            .Razonsocial = ds.Tables(0).Rows(0)("Razonsocial").ToString()
                            '004249 identifica 
                            If .Nombre.Length > 30 Then
                                Dim iPos2 As Integer
                                iPos2 = .Nombre.IndexOf(" ")
                                If iPos2 > 1 Then
                                    .Razonsocial = LTrim(.Nombre.Substring(iPos2)).ToUpper
                                    .Nombre = Left(.Nombre, iPos2)
                                End If
                            End If
                            ''004249
                        End If
                        If ds.Tables(0).Columns.Contains("Usuario") Then
                            .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Programa") Then
                            .Programa = ds.Tables(0).Rows(0)("Programa").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Condicion") Then
                            .Condicion = ds.Tables(0).Rows(0)("Condicion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Grupo") Then
                            .Grupo = ds.Tables(0).Rows(0)("Grupo").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("TipoIdentOrg") Then
                            .TipoIdentEmpr = ds.Tables(0).Rows(0)("TipoIdentOrg").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("NroIdentificacionOrg") Then
                            .NroIdentEmpr = ds.Tables(0).Rows(0)("NroIdentificacionOrg").ToString()
                        End If

                        Dim iPos As Integer = 0
                        For i = 0 To 11
                            If ds.Tables(0).Columns.Contains("Accion" & CStr(i + 1)) Then
                                .AccReq(i) = ds.Tables(0).Rows(0)("Accion" & CStr(i + 1)).ToString()  'Accion
                            End If
                            If ds.Tables(0).Columns.Contains("Requisito" & CStr(i + 1)) Then
                                .NameReq(i) = ds.Tables(0).Rows(0)("Requisito" & CStr(i + 1)).ToString()  'Nemonico
                            End If
                            If ds.Tables(0).Columns.Contains("FechaInicialVigencia" & CStr(i + 1)) Then
                                .FechIniReq(i) = ds.Tables(0).Rows(0)("FechaInicialVigencia" & CStr(i + 1)).ToString() 'Fecha Inicial de vigencia
                            End If
                            If ds.Tables(0).Columns.Contains("FechaFinalVigencia" & CStr(i + 1)) Then
                                .FechFinReq(i) = ds.Tables(0).Rows(0)("FechaFinalVigencia" & CStr(i + 1)).ToString()  'Fecha Final de vigencia
                            End If
                            If ds.Tables(0).Columns.Contains("DatoNumerico" & CStr(i + 1)) Then
                                .DatNumReq(i) = ds.Tables(0).Rows(0)("DatoNumerico" & CStr(i + 1)).ToString()  'Dato numerico
                            End If

                            If ds.Tables(0).Columns.Contains("DatoAlfabetico" & CStr(i + 1)) Then
                                'dac 20120128 Se agrega la info adicional
                                If infoadi = "" Then
                                    infoadi = BuildInfoAdirequisito(ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString())
                                    iPos = i
                                End If
                                If ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString().Length > 30 Then
                                    .DatAlfReq(i) = ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString().Substring(0, 30)  'Dato Alfabetico
                                Else
                                    .DatAlfReq(i) = ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString()
                                End If

                            End If
                        Next
                        '2011 20130617, 20130618
                        If ds.Tables.Count > 1 Then
                            Dim dsAdc As DataSet
                            If ds.Tables(2).Rows.Count > 0 Then
                                dsAdc = pReemplazarRequisitosAdicionales(Aplicacion, ds)
                                AplicaRadicar(Aplicacion, dsAdc)
                            End If
                        End If
                        '2011 20130617

                        If infoadi.Length > 0 Then
                            Dim strAccion As String, strRequisito As String, strFechaInicialVigencia As String, strFechaFinalVigencia As String
                            Dim strDatoNumerico As String, strDatoAlfabetico As String
                            strAccion = .AccReq(iPos)
                            strRequisito = .NameReq(iPos)
                            strFechaInicialVigencia = .FechIniReq(iPos)
                            strFechaFinalVigencia = .FechFinReq(iPos)
                            strDatoNumerico = .DatNumReq(iPos)
                            strDatoAlfabetico = .DatAlfReq(iPos)
                            'Reemplaza los datos del que tiene la informacion adicional, con la info de la primera posicion
                            .AccReq(iPos) = .AccReq(0)
                            .NameReq(iPos) = .NameReq(0)
                            .FechIniReq(iPos) = .FechIniReq(0)
                            .FechFinReq(iPos) = .FechFinReq(0)
                            .DatNumReq(iPos) = .DatNumReq(0)
                            .DatAlfReq(iPos) = .DatAlfReq(0)
                            'Reemplaza los datos de la primera posicion con la que itiene nformacion adicional
                            .AccReq(0) = strAccion
                            .NameReq(0) = strRequisito
                            .FechIniReq(0) = strFechaInicialVigencia
                            .FechFinReq(0) = strFechaFinalVigencia
                            .DatNumReq(0) = strDatoNumerico
                            .DatAlfReq(0) = strDatoAlfabetico
                        End If
                        If ds.Tables(0).Columns.Contains("InfoAdicional") Then
                            .InfoAdicional = ds.Tables(0).Rows(0)("InfoAdicional").ToString()
                        Else
                            .InfoAdicional = infoadi 'dac 20120128 Se agrega la info adicional
                        End If
                    End With
                    ObjRad = ObjRadNew
                    returnvalue = ObjRad.Radicacion()
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function

    Private Function BuildInfoAdirequisito(ByRef InfoAlfa As String) As String
        Dim returnvalue As String = ""
        If InfoAlfa.Length > 30 Then
            returnvalue = returnvalue & InfoAlfa.Substring(30, InfoAlfa.Length - 30)
            InfoAlfa = InfoAlfa.Substring(0, 30)
        End If
        Return returnvalue
    End Function

    <WebMethod()> Public Function RespuestaRadicacion(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim xmlDoc As New Xml.XmlDocument()
        Dim xmlNodo As Xml.XmlNode
        Dim returnvalue As String = ""
        Dim returnRadicacion As String = ""
        returnRadicacion = Radicar(Aplicacion, Parametros)
        xmlDoc.LoadXml(returnRadicacion)
        xmlNodo = xmlDoc.SelectSingleNode("//MESSAGE")
        returnvalue = "<Req><Respuesta>" + xmlNodo.InnerXml.ToString() + "</Respuesta></Req>"
        'If InStr(1, xmlNodo.InnerXml.ToString, "SUCCESSFUL ENTRY") > 0 Or InStr(1, xmlNodo.InnerXml.ToString, "INPUT REQUEST") > 0 Then
        '    'If InStr(1, returnRadicacion, "INPUT REQUEST") > 0 Then
        '    returnvalue = "<Req><Respuesta>" + xmlNodo.InnerXml.ToString() + "</Respuesta></Req>"
        'Else
        '    returnvalue = "<Req><Respuesta>" + xmlNodo.InnerXml.ToString() + "</Respuesta></Req>"
        'End If
        Return returnvalue
    End Function
    <WebMethod()> Public Function Vincular(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim returnvalue As String = ""
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim i As Integer
                    With ObjVin
                        .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                        If ds.Tables(0).Columns.Contains("Operacion") Then
                            .Operacion = ds.Tables(0).Rows(0)("Operacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Identity") Then
                            .Identity = ds.Tables(0).Rows(0)("Identity").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("TipoIdent") Then
                            .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("NroIdentificacion") Then
                            .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Partealfabetica") Then
                            .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("DigitoChequeo") Then
                            .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Sucursal") Then
                            .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("CentroCosto") Then
                            .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Nombre") Then
                            .Nombre = ds.Tables(0).Rows(0)("Nombre").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("PrimerApellido") Then
                            .PrimerApellido = ds.Tables(0).Rows(0)("PrimerApellido").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("SegundoApellido") Then
                            .SegundoApellido = ds.Tables(0).Rows(0)("SegundoApellido").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Fechanacimiento") Then
                            .Fechanacimiento = ds.Tables(0).Rows(0)("Fechanacimiento").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Genero") Then
                            .Genero = ds.Tables(0).Rows(0)("Genero").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("EstadoCivil") Then
                            .EstadoCivil = ds.Tables(0).Rows(0)("EstadoCivil").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Direccion") Then
                            .Direccion = ds.Tables(0).Rows(0)("Direccion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Telefono") Then
                            .Telefono = ds.Tables(0).Rows(0)("Telefono").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Extension") Then
                            .Extension = ds.Tables(0).Rows(0)("Extension").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("TipoDireccion") Then
                            .TipoDireccion = ds.Tables(0).Rows(0)("TipoDireccion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Barrio") Then
                            .Barrio = ds.Tables(0).Rows(0)("Barrio").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Zona") Then
                            .Zona = ds.Tables(0).Rows(0)("Zona").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Ciudad") Then
                            .Ciudad = ds.Tables(0).Rows(0)("Ciudad").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Detalleadicional") Then
                            .Detalleadicional = ds.Tables(0).Rows(0)("Detalleadicional").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Razonsocial") Then
                            .Razonsocial = ds.Tables(0).Rows(0)("Razonsocial").ToString()

                            '004249 identifica 
                            If .Nombre.Length > 30 Then
                                Dim iPos As Integer
                                iPos = .Nombre.IndexOf(" ")
                                If iPos > 1 Then
                                    .Razonsocial = LTrim(.Nombre.Substring(iPos)).ToUpper
                                    .Nombre = Left(.Nombre, iPos)
                                End If
                            End If
                            ''004249
                        End If
                        If ds.Tables(0).Columns.Contains("Usuario") Then
                            .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("IdEps") Then
                            .IdEps = ds.Tables(0).Rows(0)("IdEps").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("GrupoPrograma") Then
                            .GrupoPrograma = ds.Tables(0).Rows(0)("GrupoPrograma").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Programa") Then
                            .Programa = ds.Tables(0).Rows(0)("Programa").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Condicion") Then
                            .Condicion = ds.Tables(0).Rows(0)("Condicion").ToString()
                        End If
                        For i = 0 To 11
                            If ds.Tables(0).Columns.Contains("Accion" & CStr(i + 1)) Then
                                .AccReq(i) = ds.Tables(0).Rows(0)("Accion" & CStr(i + 1)).ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("Requisito" & CStr(i + 1)) Then
                                .NameReq(i) = ds.Tables(0).Rows(0)("Requisito" & CStr(i + 1)).ToString()  'Nemonico
                            End If
                            If ds.Tables(0).Columns.Contains("FechaInicialVigencia" & CStr(i + 1)) Then
                                .FechIniReq(i) = ds.Tables(0).Rows(0)("FechaInicialVigencia" & CStr(i + 1)).ToString() 'Fecha Inicial de vigencia
                            End If
                            If ds.Tables(0).Columns.Contains("FechaFinalVigencia" & CStr(i + 1)) Then
                                .FechFinReq(i) = ds.Tables(0).Rows(0)("FechaFinalVigencia" & CStr(i + 1)).ToString() 'Fecha Final de vigencia
                            End If
                            If ds.Tables(0).Columns.Contains("DatoNumerico" & CStr(i + 1)) Then
                                .DatNumReq(i) = ds.Tables(0).Rows(0)("DatoNumerico" & CStr(i + 1)).ToString() 'Dato numerico
                            End If
                            If ds.Tables(0).Columns.Contains("DatoAlfabetico" & CStr(i + 1)) Then
                                .DatAlfReq(i) = ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString() 'Dato Alfabetico
                            End If
                        Next
                        If ds.Tables(0).Columns.Contains("TipoIdentificacionTrabajadorResponsable") Then
                            .TipoIdentificacion1 = ds.Tables(0).Rows(0)("TipoIdentificacionTrabajadorResponsable").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("TrabajadorResponsable") Then
                            .Responsable1 = ds.Tables(0).Rows(0)("TrabajadorResponsable").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("TipoIdentificacionEmpresaResponsable") Then
                            .TipoIdentificacion2 = ds.Tables(0).Rows(0)("TipoIdentificacionEmpresaResponsable").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("EmpresaResponsable") Then
                            .Responsable2 = ds.Tables(0).Rows(0)("EmpresaResponsable").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("SucursalEmpresaResponsable") Then
                            .Responsable3 = ds.Tables(0).Rows(0)("SucursalEmpresaResponsable").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("CentroCostoEmpresaResponsable") Then
                            .Responsable4 = ds.Tables(0).Rows(0)("CentroCostoEmpresaResponsable").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("FechaInicioAfiliacion") Then
                            .FechaInicioAfiliacion = ds.Tables(0).Rows(0)("FechaInicioAfiliacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("FechaFinalAfiliacion") Then
                            .FechaFinalAfiliacion = ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("FechaFinalAfiliacion") Then
                            .FechaRetiro = ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("CausalRetiro") Then
                            .CausalRetiro = ds.Tables(0).Rows(0)("CausalRetiro").ToString().ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("FechaIngresoEmpresa") Then
                            .FechaIngresoEmpresa = ds.Tables(0).Rows(0)("FechaIngresoEmpresa").ToString().ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("FechaPrimerAporte") Then
                            .FechaPrimerAporte = ds.Tables(0).Rows(0)("FechaPrimerAporte").ToString().ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Valor") Then
                            .Valor = ds.Tables(0).Rows(0)("Valor").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Cantidad") Then
                            .Cantidad = ds.Tables(0).Rows(0)("Cantidad").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Cargo") Then
                            .Cargo = ds.Tables(0).Rows(0)("Cargo").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("NumeroDireccion") Then
                            .NumeroDireccion = ds.Tables(0).Rows(0)("NumeroDireccion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("InfoAdicional") Then
                            .InfoAdicional = ds.Tables(0).Rows(0)("InfoAdicional").ToString()
                        End If
                        'Se agrega estrato de la vinculacion Escala 12042010 estiscurrea
                        If ds.Tables(0).Columns.Contains("Escala") Then 'Se agrega estrato de la vinculación
                            '.Estado = ds.Tables(0).Rows(0)("Escala").ToString()
                            .Escala = ds.Tables(0).Rows(0)("Escala").ToString() '4855 20140811 para RS
                        End If
                        If ds.Tables(0).Columns.Contains("Estado") Then 'Se agrega Estado de la vinculación
                            .Estado = ds.Tables(0).Rows(0)("Estado").ToString()
                        End If
                        '1710 20130430 se agrega nueva columna de control
                        If ds.Tables(0).Columns.Contains("CondicionesEspeciales") Then 'Se agrega Estado de la vinculación
                            .CondicionEspecial = ds.Tables(0).Rows(0)("CondicionesEspeciales").ToString()
                        End If
                    End With
                    returnvalue = ObjVin.Vinculacion()
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function
    Public Function Vinculaciones(ByVal Aplicacion As String, ByVal Parametros As String, ByVal unicoId As String) As String
        Dim returnvalue As String = ""
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim i As Integer
                    With ObjVin
                        .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                        If ds.Tables(0).Columns.Contains("Operacion") Then ''??
                            .Operacion = ds.Tables(0).Rows(0)("Operacion").ToString()
                        End If
                        If unicoId <> "" Then ''NO
                            .Identity = unicoId 'ds.Tables(0).Rows(0)("Identity").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("TipoIdent") Then ''OK
                            .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("NroIdentificacion") Then ''OK
                            .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Partealfabetica") Then ''OK
                            .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("DigitoChequeo") Then ''OK
                            .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Sucursal") Then ''no
                            .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("CentroCosto") Then ''ok
                            .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Nombres") Then 'OK
                            .Nombre = ds.Tables(0).Rows(0)("Nombres").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("PrimerApellido") Then 'ok
                            .PrimerApellido = ds.Tables(0).Rows(0)("PrimerApellido").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("SegundoApellido") Then 'ok
                            .SegundoApellido = ds.Tables(0).Rows(0)("SegundoApellido").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Fechanacimiento") Then 'ok
                            .Fechanacimiento = ds.Tables(0).Rows(0)("Fechanacimiento").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Genero") Then 'ok
                            .Genero = ds.Tables(0).Rows(0)("Genero").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("EstadoCivil") Then 'ok
                            .EstadoCivil = ds.Tables(0).Rows(0)("EstadoCivil").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Direccion") Then 'ok
                            .Direccion = ds.Tables(0).Rows(0)("Direccion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Telefono") Then 'ok
                            .Telefono = ds.Tables(0).Rows(0)("Telefono").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Extension") Then ''NO lo tengo
                            .Extension = ds.Tables(0).Rows(0)("Extension").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("TipoDireccion_Zona") Then 'ok
                            .TipoDireccion = ds.Tables(0).Rows(0)("TipoDireccion_Zona").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Barrio") Then ''ok
                            .Barrio = ds.Tables(0).Rows(0)("Barrio").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("ZonaCiudad") Then ''NO
                            .Zona = ds.Tables(0).Rows(0)("ZonaCiudad").ToString()
                        Else
                            .Zona = "999"
                        End If
                        If ds.Tables(0).Columns.Contains("Ciudad") Then ''OK
                            .Ciudad = ds.Tables(0).Rows(0)("Ciudad").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Detalleadicional") Then ''NO: Empresa Detalle SUC y CC
                            .Detalleadicional = ds.Tables(0).Rows(0)("Detalleadicional").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Razonsocial") Then ''NO
                            .Razonsocial = ds.Tables(0).Rows(0)("Razonsocial").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("IdUsuarioEjecutor") Then
                            .Usuario = ds.Tables(0).Rows(0)("IdUsuarioEjecutor").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("GrupoPrograma") Then ''OK
                            .GrupoPrograma = ds.Tables(0).Rows(0)("GrupoPrograma").ToString()
                        Else
                            .GrupoPrograma = "EP"
                        End If
                        If ds.Tables(0).Columns.Contains("Programa") Then ''Numero de tabla CPRGSRV
                            .Programa = ds.Tables(0).Rows(0)("Programa").ToString()
                        End If

                        If ds.Tables(0).Columns.Contains("Condicion") Then ''Numero de tabla NCONAFI
                            .Condicion = ds.Tables(0).Rows(0)("Condicion").ToString()
                        End If
                        Dim infoadi As String = ""
                        Dim iPos As Integer = 0
                        For i = 0 To 11
                            If ds.Tables(0).Columns.Contains("Accion" & CStr(i + 1)) Then
                                .AccReq(i) = ds.Tables(0).Rows(0)("Accion" & CStr(i + 1)).ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("Requisito" & CStr(i + 1)) Then
                                .NameReq(i) = ds.Tables(0).Rows(0)("Requisito" & CStr(i + 1)).ToString()  'Nemonico
                            End If
                            If ds.Tables(0).Columns.Contains("FechaInicialVigencia" & CStr(i + 1)) Then
                                .FechIniReq(i) = ds.Tables(0).Rows(0)("FechaInicialVigencia" & CStr(i + 1)).ToString() 'Fecha Inicial de vigencia
                            End If
                            If ds.Tables(0).Columns.Contains("FechaFinalVigencia" & CStr(i + 1)) Then
                                .FechFinReq(i) = ds.Tables(0).Rows(0)("FechaFinalVigencia" & CStr(i + 1)).ToString() 'Fecha Final de vigencia
                            End If
                            If ds.Tables(0).Columns.Contains("DatoNumerico" & CStr(i + 1)) Then
                                .DatNumReq(i) = ds.Tables(0).Rows(0)("DatoNumerico" & CStr(i + 1)).ToString() 'Dato numerico
                            End If
                            If ds.Tables(0).Columns.Contains("DatoAlfabetico" & CStr(i + 1)) Then
                                If ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString().Length > 30 Then 'dac 20120903
                                    .DatAlfReq(i) = ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString().Substring(0, 30)
                                    infoadi = BuildInfoAdirequisito(ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString())
                                    iPos = i
                                Else
                                    .DatAlfReq(i) = ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString() 'Dato Alfabetico
                                End If

                            End If
                        Next
                        '10445
                        If infoadi.Length > 0 Then
                            Dim strAccion As String, strRequisito As String, strFechaInicialVigencia As String, strFechaFinalVigencia As String
                            Dim strDatoNumerico As String, strDatoAlfabetico As String
                            strAccion = .AccReq(iPos)
                            strRequisito = .NameReq(iPos)
                            strFechaInicialVigencia = .FechIniReq(iPos)
                            strFechaFinalVigencia = .FechFinReq(iPos)
                            strDatoNumerico = .DatNumReq(iPos)
                            strDatoAlfabetico = .DatAlfReq(iPos)
                            'Reemplaza los datos del que tiene la informacion adicional, con la info de la primera posicion
                            .AccReq(iPos) = .AccReq(0)
                            .NameReq(iPos) = .NameReq(0)
                            .FechIniReq(iPos) = .FechIniReq(0)
                            .FechFinReq(iPos) = .FechFinReq(0)
                            .DatNumReq(iPos) = .DatNumReq(0)
                            .DatAlfReq(iPos) = .DatAlfReq(0)
                            'Reemplaza los datos de la primera posicion con la que itiene nformacion adicional
                            .AccReq(0) = strAccion
                            .NameReq(0) = strRequisito
                            .FechIniReq(0) = strFechaInicialVigencia
                            .FechFinReq(0) = strFechaFinalVigencia
                            .DatNumReq(0) = strDatoNumerico
                            .DatAlfReq(0) = strDatoAlfabetico
                        End If
                        '10445
                        If ds.Tables(0).Columns.Contains("TipoIdentificacionResponsable1_cot") Then ''OK
                            .TipoIdentificacion1 = ds.Tables(0).Rows(0)("TipoIdentificacionResponsable1_cot").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Responsable1_cot") Then ''OK
                            .Responsable1 = ds.Tables(0).Rows(0)("Responsable1_cot").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("TipoIdentificacionResponsable2_TipoIdenEmp") Then ''OK
                            .TipoIdentificacion2 = ds.Tables(0).Rows(0)("TipoIdentificacionResponsable2_TipoIdenEmp").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Responsable2_IdEmpresa") Then ''OK
                            .Responsable2 = ds.Tables(0).Rows(0)("Responsable2_IdEmpresa").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Responsable3_Dependencia") Then ''OK
                            .Responsable3 = ds.Tables(0).Rows(0)("Responsable3_Dependencia").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Responsable4_CentroDeCostos") Then
                            .Responsable4 = ds.Tables(0).Rows(0)("Responsable4_CentroDeCostos").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("FechaFinalAfiliacion") Then ''NO
                            .FechaFinalAfiliacion = ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("FechaFinalAfiliacion") Then ''NO
                            .FechaRetiro = ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("CausalRetiro") Then ''NO
                            .CausalRetiro = ds.Tables(0).Rows(0)("CausalRetiro").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("FechaInicioAfiliacion") Then ''Fecha de Radicado del documento inicio de AFILIACION
                            .FechaInicioAfiliacion = ds.Tables(0).Rows(0)("FechaInicioAfiliacion").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("FechaIngreso") Then ''Fecha de Ingreso del tragjador a la EMPRESA FALLA
                            .FechaIngresoEmpresa = ds.Tables(0).Rows(0)("FechaIngreso").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Valor_Salario") Then ''SUELDO ok
                            .Valor = ds.Tables(0).Rows(0)("Valor_Salario").ToString()
                        End If

                        If ds.Tables(0).Columns.Contains("CargoTrabajador") Then ''OK
                            .Cargo = ds.Tables(0).Rows(0)("CargoTrabajador").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Ciudad") Then ''En POS es LA ciudad
                            .NumeroDireccion = ds.Tables(0).Rows(0)("Ciudad").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Regional") Then 'REGIONAL EN POS
                            .Cantidad = ds.Tables(0).Rows(0)("Regional").ToString()
                        Else
                            .Cantidad = "1"
                        End If
                        If ds.Tables(0).Columns.Contains("mesAporte") Then
                            .FechaPrimerAporte = ds.Tables(0).Rows(0)("mesAporte")
                        End If
                        If ds.Tables(0).Columns.Contains("InfoAdicional") Then ''NO SE TIENE
                            .InfoAdicional = ds.Tables(0).Rows(0)("InfoAdicional").ToString()
                        Else '0010445 20120903
                            If infoadi.Length > 1 Then
                                .InfoAdicional = infoadi
                                infoadi = ""
                            End If
                            '0010445 20120903
                        End If

                    End With
                    returnvalue = ObjVin.Vinculacion()

                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function
    <WebMethod()> Public Function PruebaWCW(ByVal Aplicacion As String, ByVal sParametros As String) As String
        Dim returnvalue As String = ""
        Try
            CManejadorMensajes.PublicarMensaje("LogicaTransaccional:VinculacionesIN:Parametros: " & sParametros, EventLogEntryType.FailureAudit)

            returnvalue = ObjVin.ValidaRepuesta(sParametros)
        Catch ex As Exception
            CManejadorMensajes.PublicarMensaje("LogicaTransaccional:VinculacionesIN:Parametros: " & ex.Message, EventLogEntryType.Error)

        End Try
        Return returnvalue
    End Function
    <WebMethod()> Public Function VinculacionInc(ByVal Aplicacion As String, ByVal sParametros As String) As String
        Dim returnvalue As String = ""
        Dim ReturnConsulta As String = ""
        Dim strRepuesta As String
        Dim xmlDoc As New Xml.XmlDocument()
        Dim xmlNodo As Xml.XmlNode
        Try
            ReturnConsulta = ConsultaCliente(Aplicacion, sParametros)
            If InStr(1, ReturnConsulta, "INPUT REQUEST") > 0 Then ' si contiene el tag si no encuentra error
                Dim XmlResultado As String = ""
                Dim ds As DataSet = ObjUtil.GetDataSet(sParametros)
                Dim dsCons As DataSet = ObjUtil.GetDataSet(ReturnConsulta)
                If ds.Tables.Count > 0 Then
                    If ds.Tables(0).Rows.Count > 0 Then
                        Dim i As Integer
                        With ObjVin
                            .Aplicacion = ConfigurationSettings.AppSettings("ProjectID")
                            If ds.Tables(0).Columns.Contains("Operacion") Then
                                .Operacion = ds.Tables(0).Rows(0)("Operacion").ToString()
                            Else
                                .Operacion = "CHG"
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_NAUTCLI") Then
                                .Identity = dsCons.Tables(0).Rows(0)("I_NAUTCLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XIDECLI") Then
                                .TipoIdent = dsCons.Tables(0).Rows(0)("I_XIDECLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_NIDECLI") Then
                                .NroIdentificacion = dsCons.Tables(0).Rows(0)("I_NIDECLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XNUICLI") Then
                                .Partealfabetica = dsCons.Tables(0).Rows(0)("I_XNUICLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_NDIGCHE") Then
                                .DigitoChequeo = dsCons.Tables(0).Rows(0)("I_NDIGCHE").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_NIDESUC") Then
                                .Sucursal = dsCons.Tables(0).Rows(0)("I_NIDESUC").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_NIDECOS") Then
                                .CentroCosto = dsCons.Tables(0).Rows(0)("I_NIDECOS").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XNOMTRAB") Then
                                .Nombre = dsCons.Tables(0).Rows(0)("I_XNOMTRAB").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XPRIAPE") Then
                                .PrimerApellido = dsCons.Tables(0).Rows(0)("I_XPRIAPE").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XSEGAPE") Then
                                .SegundoApellido = dsCons.Tables(0).Rows(0)("I_XSEGAPE").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_FNACCLI") Then
                                .Fechanacimiento = dsCons.Tables(0).Rows(0)("I_FNACCLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XGENCLI") Then
                                .Genero = dsCons.Tables(0).Rows(0)("I_XGENCLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_CECICLI") Then
                                .EstadoCivil = dsCons.Tables(0).Rows(0)("I_CECICLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XDIRCLI") Then
                                .Direccion = dsCons.Tables(0).Rows(0)("I_XDIRCLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_NTELCLI") Then
                                .Telefono = dsCons.Tables(0).Rows(0)("I_NTELCLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_NEXTCLI") Then
                                .Extension = dsCons.Tables(0).Rows(0)("I_NEXTCLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XTIPDIR") Then
                                .TipoDireccion = dsCons.Tables(0).Rows(0)("I_XTIPDIR").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XBARCLI") Then
                                .Barrio = dsCons.Tables(0).Rows(0)("I_XBARCLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_CZONBTA") Then
                                .Zona = dsCons.Tables(0).Rows(0)("I_CZONBTA").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_CCIUCLI") Then
                                .Ciudad = dsCons.Tables(0).Rows(0)("I_CCIUCLI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XNIVADI") Then
                                .Detalleadicional = dsCons.Tables(0).Rows(0)("I_XNIVADI").ToString()
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_XDESRAZ") Then
                                .Razonsocial = dsCons.Tables(0).Rows(0)("I_XDESRAZ").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("Usuario") Then
                                .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("GrupoPrograma") Then
                                .GrupoPrograma = ds.Tables(0).Rows(0)("GrupoPrograma").ToString()
                            Else
                                .GrupoPrograma = "EP"
                            End If
                            If ds.Tables(0).Columns.Contains("Programa") Then
                                .Programa = ds.Tables(0).Rows(0)("Programa").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("Condicion") Then
                                .Condicion = ds.Tables(0).Rows(0)("Condicion").ToString()
                            End If
                            For i = 0 To 11
                                If ds.Tables(0).Columns.Contains("Requisito") Then
                                    If ds.Tables(0).Rows(0).Item("Requisito").ToString.Equals("DE2400") Then
                                        .AccReq(i) = ""
                                        .NameReq(i) = "0"
                                        .FechIniReq(i) = "0"
                                        .FechFinReq(i) = "0"
                                        .DatNumReq(i) = "0"
                                        .DatAlfReq(i) = ""
                                    End If
                                Else
                                    If ds.Tables(0).Columns.Contains("I_XACC" & CStr(i + 1)) Then
                                        If ds.Tables(0).Rows(0)("I_XACC" & CStr(i + 1)).ToString() <> "" Then

                                        End If
                                    End If
                                    If ds.Tables(0).Columns.Contains("I_XACC" & CStr(i + 1)) Then
                                        .AccReq(i) = ds.Tables(0).Rows(0)("I_XACC" & CStr(i + 1)).ToString()
                                    ElseIf ds.Tables(0).Columns.Contains("Accion" & CStr(i + 1)) Then
                                        .AccReq(i) = ds.Tables(0).Rows(0)("Accion" & CStr(i + 1)).ToString()
                                    Else
                                        If dsCons.Tables(0).Columns.Contains("I_XACC" & CStr(i + 1)) Then
                                            .AccReq(i) = dsCons.Tables(0).Rows(0)("I_XACC" & CStr(i + 1)).ToString()
                                        Else
                                            .AccReq(i) = ""
                                        End If
                                    End If
                                    If ds.Tables(0).Columns.Contains("I_XNMNCA" & CStr(i + 1)) Then
                                        .NameReq(i) = ds.Tables(0).Rows(0)("I_XNMNCA" & CStr(i + 1)).ToString()  'Nemonico
                                    ElseIf ds.Tables(0).Columns.Contains("Requisito" & CStr(i + 1)) Then
                                        .NameReq(i) = ds.Tables(0).Rows(0)("Requisito" & CStr(i + 1)).ToString()  'Nemonico
                                    Else
                                        If dsCons.Tables(0).Columns.Contains("I_XNMNCA" & CStr(i + 1)) Then
                                            .NameReq(i) = dsCons.Tables(0).Rows(0)("I_XNMNCA" & CStr(i + 1)).ToString()  'Nemonico
                                        Else
                                            .NameReq(i) = ""
                                        End If
                                    End If
                                    If ds.Tables(0).Columns.Contains("I_FINIVI" & CStr(i + 1)) Then
                                        .FechIniReq(i) = ds.Tables(0).Rows(0)("I_FINIVI" & CStr(i + 1)).ToString() 'Fecha Inicial de vigencia
                                    ElseIf ds.Tables(0).Columns.Contains("FechaInicialVigencia" & CStr(i + 1)) Then
                                        .FechIniReq(i) = ds.Tables(0).Rows(0)("FechaInicialVigencia" & CStr(i + 1)).ToString() 'Fecha Inicial de vigencia
                                    Else
                                        If dsCons.Tables(0).Columns.Contains("I_FINIVI" & CStr(i + 1)) Then
                                            .FechIniReq(i) = dsCons.Tables(0).Rows(0)("I_FINIVI" & CStr(i + 1)).ToString() 'Fecha Inicial de vigencia
                                        Else
                                            .FechIniReq(i) = "0"
                                        End If
                                    End If
                                    If ds.Tables(0).Columns.Contains("I_FFINVI" & CStr(i + 1)) Then
                                        .FechFinReq(i) = ds.Tables(0).Rows(0)("I_FFINVI" & CStr(i + 1)).ToString() 'Fecha Final de vigencia
                                    ElseIf ds.Tables(0).Columns.Contains("FechaFinalVigencia" & CStr(i + 1)) Then
                                        .FechFinReq(i) = ds.Tables(0).Rows(0)("FechaFinalVigencia" & CStr(i + 1)).ToString() 'Fecha Final de vigencia
                                    Else
                                        If dsCons.Tables(0).Columns.Contains("I_FFINVI" & CStr(i + 1)) Then
                                            .FechFinReq(i) = dsCons.Tables(0).Rows(0)("I_FFINVI" & CStr(i + 1)).ToString() 'Fecha Final de vigencia
                                        Else
                                            .FechFinReq(i) = "0"
                                        End If
                                    End If
                                    If ds.Tables(0).Columns.Contains("I_VNUMDA" & CStr(i + 1)) Then
                                        .DatNumReq(i) = ds.Tables(0).Rows(0)("I_VNUMDA" & CStr(i + 1)).ToString() 'Dato numerico
                                    ElseIf ds.Tables(0).Columns.Contains("DatoNumerico" & CStr(i + 1)) Then
                                        .DatNumReq(i) = ds.Tables(0).Rows(0)("DatoNumerico" & CStr(i + 1)).ToString() 'Dato numerico
                                    Else
                                        If dsCons.Tables(0).Columns.Contains("I_VNUMDA" & CStr(i + 1)) Then
                                            .DatNumReq(i) = dsCons.Tables(0).Rows(0)("I_VNUMDA" & CStr(i + 1)).ToString() 'Dato numerico
                                        Else
                                            .DatNumReq(i) = "0"
                                        End If
                                    End If
                                    If ds.Tables(0).Columns.Contains("I_XALFDA" & CStr(i + 1)) Then
                                        .DatAlfReq(i) = ds.Tables(0).Rows(0)("I_XALFDA" & CStr(i + 1)).ToString() 'Dato Alfabetico
                                    ElseIf ds.Tables(0).Columns.Contains("DatoAlfabetico" & CStr(i + 1)) Then
                                        .DatAlfReq(i) = ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString() 'Dato Alfabetico
                                    Else
                                        If dsCons.Tables(0).Columns.Contains("I_XALFDA" & CStr(i + 1)) Then
                                            .DatAlfReq(i) = dsCons.Tables(0).Rows(0)("I_XALFDA" & CStr(i + 1)).ToString() 'Dato Alfabetico
                                        Else
                                            .DatAlfReq(i) = ""
                                        End If
                                    End If
                                End If
                            Next
                            If ds.Tables(0).Columns.Contains("TipoIdentificacionTrabajadorResponsable") Then
                                .TipoIdentificacion1 = ds.Tables(0).Rows(0)("TipoIdentificacionTrabajadorResponsable").ToString()
                            ElseIf dsCons.Tables(0).Rows(0)("I_TRES001").ToString() <> "" Then
                                .TipoIdentificacion1 = dsCons.Tables(0).Rows(0)("I_TRES001").ToString()
                            Else
                                .TipoIdentificacion1 = dsCons.Tables(0).Rows(0)("I_XRES001").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("TrabajadorResponsable") Then
                                .Responsable1 = ds.Tables(0).Rows(0)("TrabajadorResponsable").ToString()
                            Else
                                .Responsable1 = dsCons.Tables(0).Rows(0)("I_NRES001").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("TipoIdentificacionEmpresaResponsable") Then
                                .TipoIdentificacion2 = ds.Tables(0).Rows(0)("TipoIdentificacionEmpresaResponsable").ToString()
                            ElseIf dsCons.Tables(0).Rows(0)("I_TRES001").ToString() <> "" Then
                                .TipoIdentificacion2 = dsCons.Tables(0).Rows(0)("I_TRES002").ToString()
                            Else
                                .TipoIdentificacion2 = dsCons.Tables(0).Rows(0)("I_XRES002").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("EmpresaResponsable") Then
                                .Responsable2 = ds.Tables(0).Rows(0)("EmpresaResponsable").ToString()
                            Else
                                .Responsable2 = dsCons.Tables(0).Rows(0)("I_NRES002").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("SucursalEmpresaResponsable") Then
                                .Responsable3 = ds.Tables(0).Rows(0)("SucursalEmpresaResponsable").ToString()
                            Else
                                .Responsable3 = dsCons.Tables(0).Rows(0)("I_NRES003").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("CentroCostoEmpresaResponsable") Then
                                .Responsable4 = ds.Tables(0).Rows(0)("CentroCostoEmpresaResponsable").ToString()
                            Else
                                .Responsable4 = dsCons.Tables(0).Rows(0)("I_NRES004").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("FechaInicioAfiliacion") Then
                                If ds.Tables(0).Rows(0)("FechaInicioAfiliacion").ToString() = dsCons.Tables(0).Rows(0)("I_FINIAFI").ToString() Then
                                    .FechaInicioAfiliacion = dsCons.Tables(0).Rows(0)("I_FINIAFI").ToString()
                                Else
                                    .FechaInicioAfiliacion = ds.Tables(0).Rows(0)("FechaInicioAfiliacion").ToString()
                                End If
                            Else
                                .FechaInicioAfiliacion = dsCons.Tables(0).Rows(0)("I_FINIAFI").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("FechaFinalAfiliacion") Then
                                If ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString() = dsCons.Tables(0).Rows(0)("I_FFINAFI").ToString() Then
                                    .FechaFinalAfiliacion = dsCons.Tables(0).Rows(0)("I_FFINAFI").ToString()
                                Else
                                    .FechaFinalAfiliacion = ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString()
                                End If
                            ElseIf ds.Tables(0).Columns.Contains("Requisito") Then
                                If ds.Tables(0).Rows(0).Item("Requisito").ToString.Equals("DE2400") Then
                                    .FechaFinalAfiliacion = "0"
                                End If
                            Else
                                .FechaFinalAfiliacion = dsCons.Tables(0).Rows(0)("I_FFINAFI").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("CausalRetiro") Then
                                If ds.Tables(0).Rows(0)("CausalRetiro").ToString() = dsCons.Tables(0).Rows(0)("I_CRETAFI").ToString() Then
                                    If ds.Tables(0).Columns.Contains("Operacion") Then
                                        If ds.Tables(0).Rows(0)("Operacion").ToString() = "ADD" Then
                                            .CausalRetiro = "0"
                                        Else
                                            .CausalRetiro = dsCons.Tables(0).Rows(0)("I_CRETAFI").ToString()
                                        End If
                                    Else
                                        .CausalRetiro = dsCons.Tables(0).Rows(0)("I_CRETAFI").ToString()
                                    End If
                                Else
                                    .CausalRetiro = ds.Tables(0).Rows(0)("CausalRetiro").ToString()
                                End If
                            Else
                                If ds.Tables(0).Rows(0)("Operacion").ToString() = "ADD" Then
                                    .CausalRetiro = "0"
                                Else
                                    .CausalRetiro = dsCons.Tables(0).Rows(0)("I_CRETAFI").ToString()
                                End If
                            End If
                            If ds.Tables(0).Columns.Contains("FechaFinalAfiliacion") Then
                                If dsCons.Tables(0).Columns.Contains("I_FRET") Then
                                    If ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString() = dsCons.Tables(0).Rows(0)("I_FRET").ToString() Then
                                        .FechaRetiro = dsCons.Tables(0).Rows(0)("I_FRET").ToString()
                                    Else
                                        .FechaRetiro = ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString()
                                    End If
                                Else
                                    .FechaRetiro = ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString()
                                End If
                            ElseIf ds.Tables(0).Columns.Contains("Requisito") Then
                                If ds.Tables(0).Rows(0).Item("Requisito").ToString.Equals("DE2400") Then
                                    .FechaRetiro = "0"
                                End If
                            Else
                                If dsCons.Tables(0).Columns.Contains("I_FRET") Then
                                    .FechaRetiro = dsCons.Tables(0).Rows(0)("I_FRET").ToString().ToString()
                                Else
                                    .FechaRetiro = "0"
                                End If
                            End If
                            If ds.Tables(0).Columns.Contains("FechaPrimerAporte") Then
                                If ds.Tables(0).Rows(0)("FechaPrimerAporte").ToString() = dsCons.Tables(0).Rows(0)("I_FVIN").ToString() Then
                                    .FechaPrimerAporte = dsCons.Tables(0).Rows(0)("I_FVIN").ToString()
                                Else
                                    .FechaPrimerAporte = ds.Tables(0).Rows(0)("FechaPrimerAporte").ToString()
                                End If
                            ElseIf ds.Tables(0).Columns.Contains("Requisito") Then
                                If ds.Tables(0).Rows(0).Item("Requisito").ToString.Equals("DE2400") Then
                                    .FechaPrimerAporte = DateTime.Now.ToString("yyyyMMdd")
                                End If
                            Else
                                .FechaPrimerAporte = dsCons.Tables(0).Rows(0)("I_FVIN").ToString().ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("FechaIngresoEmpresa") Then
                                If dsCons.Tables(0).Columns.Contains("I_FINGEMP") Then
                                    If ds.Tables(0).Rows(0)("FechaIngresoEmpresa").ToString() = dsCons.Tables(0).Rows(0)("I_FINGEMP").ToString() Then
                                        .FechaIngresoEmpresa = dsCons.Tables(0).Rows(0)("I_FINGEMP").ToString()
                                    Else
                                        .FechaIngresoEmpresa = ds.Tables(0).Rows(0)("FechaIngresoEmpresa").ToString()
                                    End If
                                Else
                                    .FechaIngresoEmpresa = ds.Tables(0).Rows(0)("FechaIngresoEmpresa").ToString()
                                End If
                            ElseIf ds.Tables(0).Columns.Contains("Requisito") Then
                                If ds.Tables(0).Rows(0).Item("Requisito").ToString.Equals("DE2400") Then
                                    .FechaIngresoEmpresa = DateTime.Now.ToString("yyyyMMdd")
                                End If
                            Else
                                If dsCons.Tables(0).Columns.Contains("I_FINGEMP") Then
                                    .FechaIngresoEmpresa = dsCons.Tables(0).Rows(0)("I_FINGEMP").ToString().ToString()
                                Else
                                    .FechaIngresoEmpresa = "0"
                                End If
                            End If
                            If dsCons.Tables(0).Columns.Contains("I_VVIN") Then
                                If ds.Tables(0).Columns.Contains("Valor") Then
                                    If ds.Tables(0).Rows(0)("Valor").ToString() = dsCons.Tables(0).Rows(0)("I_VVIN").ToString() Then
                                        .Valor = dsCons.Tables(0).Rows(0)("I_VVIN").ToString()
                                    Else
                                        .Valor = ds.Tables(0).Rows(0)("Valor").ToString()
                                    End If
                                Else
                                    If dsCons.Tables(0).Columns.Contains("I_VVIN") Then
                                        .Valor = dsCons.Tables(0).Rows(0)("I_VVIN").ToString()
                                    Else
                                        .Valor = "0"
                                    End If
                                End If
                                If ds.Tables(0).Columns.Contains("Cantidad") Then
                                    If ds.Tables(0).Rows(0)("Cantidad").ToString() = dsCons.Tables(0).Rows(0)("I_QVIN").ToString() Then
                                        .Cantidad = dsCons.Tables(0).Rows(0)("I_QVIN").ToString()
                                    Else
                                        .Cantidad = ds.Tables(0).Rows(0)("Cantidad").ToString()
                                    End If
                                Else
                                    If dsCons.Tables(0).Columns.Contains("I_QVIN") Then
                                        .Cantidad = dsCons.Tables(0).Rows(0)("I_QVIN").ToString()
                                    Else
                                        .Cantidad = "0"
                                    End If
                                End If
                                If ds.Tables(0).Columns.Contains("Cargo") Then
                                    If ds.Tables(0).Rows(0)("Cargo").ToString() = dsCons.Tables(0).Rows(0)("I_XVIN").ToString() Then
                                        .Cargo = dsCons.Tables(0).Rows(0)("I_XVIN").ToString()
                                    Else
                                        .Cargo = ds.Tables(0).Rows(0)("Cargo").ToString()
                                    End If
                                Else
                                    If dsCons.Tables(0).Columns.Contains("I_XVIN") Then
                                        .Cargo = dsCons.Tables(0).Rows(0)("I_XVIN").ToString()
                                    Else
                                        .Cargo = "0"
                                    End If
                                End If
                                If ds.Tables(0).Columns.Contains("NumeroDireccion") Then
                                    If ds.Tables(0).Rows(0)("NumeroDireccion").ToString() = dsCons.Tables(0).Rows(0)("I_NDIRVIN").ToString() Then
                                        .NumeroDireccion = dsCons.Tables(0).Rows(0)("I_NDIRVIN").ToString()
                                    Else
                                        .NumeroDireccion = ds.Tables(0).Rows(0)("NumeroDireccion").ToString()
                                    End If
                                Else
                                    If dsCons.Tables(0).Columns.Contains("I_NDIRVIN") Then
                                        .NumeroDireccion = dsCons.Tables(0).Rows(0)("I_NDIRVIN").ToString()
                                    Else
                                        .NumeroDireccion = "0"
                                    End If
                                End If
                                If ds.Tables(0).Columns.Contains("InfoAdicional") Then
                                    If ds.Tables(0).Rows(0)("InfoAdicional").ToString() = dsCons.Tables(0).Rows(0)("I_XALFADI").ToString() Then
                                        .InfoAdicional = dsCons.Tables(0).Rows(0)("I_XALFADI").ToString()
                                    Else
                                        .InfoAdicional = ds.Tables(0).Rows(0)("InfoAdicional").ToString()
                                    End If
                                Else
                                    If dsCons.Tables(0).Columns.Contains("I_XALFADI") Then
                                        .InfoAdicional = dsCons.Tables(0).Rows(0)("I_XALFADI").ToString()
                                    Else
                                        .InfoAdicional = ""
                                    End If
                                End If
                            End If
                            If ds.Tables(0).Columns.Contains("Requisito") Then
                                If ds.Tables(0).Rows(0).Item("Requisito").ToString.Equals("DE2400") Then
                                    .NameReq(0) = ds.Tables(0).Rows(0).Item("Requisito").ToString
                                End If
                            End If
                        End With
                        returnvalue = ObjVin.Vinculacion()
                        xmlDoc.LoadXml(returnvalue)
                        xmlNodo = xmlDoc.SelectSingleNode("//MESSAGE")
                        If returnvalue.Contains("MESSAGE") Then
                            If InStr(1, xmlNodo.InnerXml.ToString, "SUCCESSFUL ENTRY") > 0 Or InStr(1, xmlNodo.InnerXml.ToString, "INPUT REQUEST") > 0 Then
                                strRepuesta = "Resultado: MODIFICACIÓN A VINCULACIÓN ACTUAL EXITOSA"
                                'strRepuesta = xmlNodo.InnerXml.ToString()
                            Else
                                strRepuesta = xmlNodo.InnerXml.ToString()
                            End If
                        ElseIf returnvalue.Contains("ERROR") Then
                            xmlNodo = Nothing
                            strRepuesta = xmlNodo.InnerXml.ToString()
                        Else
                            xmlNodo = Nothing
                            strRepuesta = returnvalue
                        End If
                        returnvalue = strRepuesta
                    End If
                End If
            Else
                xmlDoc.LoadXml(ReturnConsulta)
                xmlNodo = xmlDoc.SelectSingleNode("//MESSAGE")
                If xmlNodo Is Nothing Then
                    returnvalue = ReturnConsulta
                Else
                    strRepuesta = xmlNodo.InnerXml.ToString()
                    returnvalue = strRepuesta
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function
    '0004808 ini
    <WebMethod(Description:="Metodo para modificar la informacion de estado de un requisito desde otros aplicativos")> _
    Public Function ActualizarEstadoRequisito(ByVal strApp As String, _
             ByVal strXml As String) As String
        Try
            Dim returnvalue As String, sAplicacion As String, fIniConsumo As DateTime, ffinConsumo As DateTime
            fIniConsumo = Now
            returnvalue = String.Empty
            sAplicacion = ConfigurationSettings.AppSettings("AfilPos_ProjectID")
            Dim strXMLDatosBasicos As String = ""
            'FN1
            strXMLDatosBasicos = Utilidades.CUtil.ValidarXSD(strXml, (System.AppDomain.CurrentDomain.BaseDirectory + "XSD\TramaActualizarEstadoRequisito.xsd"))
            If strXMLDatosBasicos.Length > 4 Then
                ActualizarEstadoRequisito = xmlDataSetResultado(0, strXMLDatosBasicos)
                Return ActualizarEstadoRequisito
            End If
            Dim dsDatosBasicos As DataSet, nombreRequisito As String

            Dim ds As DataSet = ObjUtil.GetDataSet(strXml)

            nombreRequisito = ds.Tables(0).Rows(0)("Requisito").ToString
            Try
                strXMLDatosBasicos = ConsultaCliente(sAplicacion, strXml)
                'FA1
                If strXMLDatosBasicos.Contains("CLIENTE NO EXISTE") Then
                    Return xmlDataSetResultado(0, "El afiliado no existe, No se pudo realizar la actualización de requisito")
                End If
            Catch
                Return xmlDataSetResultado(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento - ERROR: Consulta Cliente")
            End Try

            'CE2.4 
            Dim dstReq As DataSet       '4808 201400909 parametro requisitos prohibidas
            Dim dtr As DataRow()
            Try
                dstReq = ConsultaAF.dstConsultaEga18(sAplicacion, "UPDRQX")
                If dstReq.Tables.Count > 0 Then
                    dtr = dstReq.Tables(0).Select("XDES='" & nombreRequisito & "'")
                    If Not dtr Is Nothing Then
                        If dtr.Length > 0 Then
                            Return xmlDataSetResultado(0, "ERROR POS: Requisito inhabilitado para ser cambiado")
                        End If
                    End If
                End If
            Catch
                Return xmlDataSetResultado(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento - ERROR: Consulta EGA18")
            End Try
            Dim ObjTran As New Servicios.ClsServicio
            dsDatosBasicos = ObjUtil.GetDataSet(strXMLDatosBasicos)
            ' Dim obj As Compensar.SISPOS.ESL.Vinculacion.ConsultaAF

            Dim dstRequisitos As New DataSet, dtRequisitos() As DataTable = Nothing
            Try
                dstRequisitos = ConsultaAF.ConsultarRequisitoPorNautcli(sAplicacion, dsDatosBasicos.Tables(0).Rows(0)("I_NAUTCLI").ToString)
                'FA2
                dtr = dstRequisitos.Tables(0).Select("XNMNCAM='" & nombreRequisito & "' AND CMNT='A'")
                If dtr.Length = 0 Then
                    Return xmlDataSetResultado(0, "ERROR POS: Cliente no tiene asociado el requisito")
                End If
            Catch
                Return xmlDataSetResultado(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento - ERROR: Consulta Requisitos")
            End Try

            'strFecFin = dstFechas.Tables(0).Rows(0)("CDATTAB").ToString

            'If dtReq.Rows.Count > 0 Then

            'End If
            Dim sucursal As String, centroCosto As String

            If ds.Tables(0).Rows(0)("Sucursal").ToString() = "" Then
                sucursal = dsDatosBasicos.Tables(0).Rows(0)("I_NIDESUC").ToString()
            Else
                sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
            End If

            If ds.Tables(0).Rows(0)("Centrocosto").ToString() = "" Then
                centroCosto = dsDatosBasicos.Tables(0).Rows(0)("I_NIDECOS").ToString()
            Else
                centroCosto = ds.Tables(0).Rows(0)("Centrocosto").ToString()
            End If

            Dim xmlDoc As XmlDocument
            Dim xmlnodo As XmlNode, NTELCLI As String
            strXMLDatosBasicos = strXml
            'NTELCLI = dtr.Rows(0)("FULTNOV").ToString()  'String.Format("{0:yyyyMMdd}", DateTime.Parse(Datos(1))) '  'TODO:20140902 corresponde a la fecha de proceso del requisito
            NTELCLI = dtr(0).Item("FPRO")
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then

                    strXml = "<CLE15><CLE15>" & _
                                      "<_TOP_LINE_>CLE15T00001507JUN05</_TOP_LINE_>" & _
                                          "<I_MAINT>EST</I_MAINT>" & _
                                          "<I_XISP /> <I_XINDOPC />" & _
                                          "<I_CALT>" & ds.Tables(0).Rows(0)("Usuario").ToString() & "</I_CALT>" & _
                                          "<I_NAUTCLI>0</I_NAUTCLI>" & _
                                          "<I_NTELCLI>" & NTELCLI & "</I_NTELCLI>" & _
                                          "<I_NDIR>" & ds.Tables(0).Rows(0)("EstadoRequisito").ToString() & "</I_NDIR>" & _
                                          "<I_XNIVADI></I_XNIVADI>" & _
                                          "<I_CPRGSRV></I_CPRGSRV>" & _
                                          "<I_NCONAFI>0</I_NCONAFI>" & _
                                          "<I_NIDECLI>" & ds.Tables(0).Rows(0)("NroIdentificacion").ToString() & "</I_NIDECLI>" & _
                                          "<I_XIDECLI>" & ds.Tables(0).Rows(0)("TipoIdent").ToString() & "</I_XIDECLI>" & _
                                          "<I_XNUICLI>" & ds.Tables(0).Rows(0)("ParteAlfabetica").ToString() & "</I_XNUICLI>" & _
                                          "<I_NIDESUC>" & sucursal & "</I_NIDESUC>" & _
                                          "<I_NIDECOS>" & centroCosto & "</I_NIDECOS>" & _
                                          "<I_XNMNCA1>" & ds.Tables(0).Rows(0)("Requisito").ToString() & "</I_XNMNCA1>" & _
                                  "</CLE15></CLE15>"
                    'Se envia el XML con los valores del documento para la consulta

                    With ObjTran
                        .DtsInfoTran = ObjUtil.GetDataSet(strXml)
                        returnvalue = .EjecutaTransaccion(sAplicacion)
                    End With

                    ' returnvalue = ObjTran.EjecutaTransaccion(ConfigurationSettings.AppSettings("ProjectID"), "CLE15", strXml, "1")

                End If
            End If
            xmlDoc = New XmlDocument()
            Dim intExitoso As Integer
            Try
                xmlDoc.LoadXml(returnvalue)
                xmlnodo = xmlDoc.SelectSingleNode("//MESSAGE")
                If Not (xmlnodo Is Nothing) Then
                    If xmlnodo.InnerText.ToString.IndexOf("EL RADICADO HA SIDO ACTUALIZADO") = -1 Then
                        'DAC 20121019 CU-REGCLIENTES-029
                        If xmlnodo.InnerText.ToString.IndexOf("NPUT ") > 0 OrElse xmlnodo.InnerText.ToString.IndexOf("HA SIDO ACTUALIZADO") > 0 Then
                            returnvalue = "TRANSACCION EXITOSA"
                            intExitoso = 1
                        Else
                            returnvalue = xmlnodo.InnerText.ToString
                        End If
                    End If
                Else
                    xmlnodo = xmlDoc.SelectSingleNode("//Mensaje")
                    If Not (xmlnodo Is Nothing) Then
                        'DAC 20121019 CU-REGCLIENTES-029
                        If xmlnodo.InnerText.ToString.IndexOf("NPUT ") > 0 Then
                            returnvalue = "TRANSACCION EXITOSA"
                            intExitoso = 1
                        Else
                            returnvalue = xmlnodo.InnerText.ToString
                        End If

                    Else
                        returnvalue = "PROBLEMAS EN EL SERVICIO WEB"
                    End If
                End If
            Catch ex As Exception
                returnvalue = ex.Message
            End Try
            xmlDoc = Nothing
            xmlnodo = Nothing
            returnvalue = xmlDataSetResultado(intExitoso, returnvalue)
            ffinConsumo = Now
            ConsultaAF.InsertarBitacora(strApp, HttpContext.Current.Request.ServerVariables("REMOTE_ADDR"), "ActualizarEstadoRequisito", fIniConsumo, ffinConsumo, strXMLDatosBasicos, returnvalue, sAplicacion)
            Return returnvalue
            'Return returnvalue
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    '0004808 fin

    '0004807 ini

    <WebMethod(Description:="Metodo para modificar la informacion de datos basicos de afiliados desde otros aplicativos")> _
    Public Function ActualizarAfiliado(ByVal strApp As String, _
             ByVal strXml As String) As String
        Dim str_porDefecto As String = "NO REPORTA"
        Dim int_porDefecto As String = 0
        Try
            Dim returnvalue As String, sAplicacion As String, fIniConsumo As DateTime, ffinConsumo As DateTime
            fIniConsumo = Now
            returnvalue = String.Empty
            sAplicacion = ConfigurationSettings.AppSettings("AfilPos_ProjectID")
            Dim strXMLDatosBasicos As String = "", irad As String = "0"

            'VALIDAR LA TRAMA XML CON ARCHIVO XSD
            strXMLDatosBasicos = Utilidades.CUtil.ValidarXSD(strXml, (System.AppDomain.CurrentDomain.BaseDirectory + "XSD\TramaActualizarDatosBasicos.xsd"))
            If strXMLDatosBasicos.Length > 4 Then
                ActualizarAfiliado = xmlDataSetResultado(0, strXMLDatosBasicos)
                Return ActualizarAfiliado
            End If
            Dim dsDatosBasicos As DataSet

            Dim ds As DataSet = ObjUtil.GetDataSet(strXml)

            'SI EL TIPO DOC ENVIADO ES NUIP SE VALIDA QUE ENVIEN EL CAMPO PARTEALFABETICA
            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NU" And ds.Tables(0).Rows(0)("ParteAlfabetica").ToString() = "" Then
                ActualizarAfiliado = xmlDataSetResultado(0, "El elemento 'ParteAlfabetica' tiene un valor no válido según su tipo de datos.")
                Return ActualizarAfiliado
            End If

            'SE ARMA TRAMA PARA CONSULTAR CLIENTE
            Dim xmlConsulAfi As String = "<AFILIADO><TipoIdent>" & ds.Tables(0).Rows(0)("TipoIdent").ToString() & "</TipoIdent>" & _
                                        "<NroIdentificacion>" & ds.Tables(0).Rows(0)("NroIdentificacion").ToString() & "</NroIdentificacion>" & _
                                        "<ParteAlfabetica>" & ds.Tables(0).Rows(0)("ParteAlfabetica").ToString() & "</ParteAlfabetica>" & _
                                        "<Sucursal>0</Sucursal><CentroCosto>0</CentroCosto></AFILIADO>"

            Dim strXMLDatosBasicosAfil As String
            'SE CONSULTA CLIENTE
            Try
                strXMLDatosBasicosAfil = ConsultaCliente(sAplicacion, xmlConsulAfi)

                'SE VALIDA LA EXISTENCIA DEL CLIENTE CON LOS DATOS ENVIADOS
                If strXMLDatosBasicosAfil.Contains("CLIENTE NO EXISTE") Then
                    Return xmlDataSetResultado(0, "El afiliado no existe, No se pudo realizar la actualización")
                End If
            Catch ex As Exception
                Return xmlDataSetResultado(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento - ERROR: Consulta Cliente")
            End Try

            Dim dtr As DataRow()
            Dim ObjTran As New Servicios.ClsServicio
            dsDatosBasicos = ObjUtil.GetDataSet(strXMLDatosBasicosAfil)
            Dim dstRequisitos As New DataSet, dtRequisitos() As DataTable = Nothing



            'VALIDAR EL VALOR DEL CAMPO ZONA CON LAS DEFINIDAS EN LA EGA18
            Dim valCiudad As String
            Dim dstReq As DataSet
            Dim valError As Integer = 0
            Dim xciu As String = "11001"
            Try
                If ds.Tables(0).Rows(0)("Ciudad").ToString() = "" Then
                    xciu = dsDatosBasicos.Tables(0).Rows(0)("I_CCIUCLI").ToString()
                Else
                    xciu = ds.Tables(0).Rows(0)("Ciudad").ToString()
                End If
                If (xciu = "") Then
                    xciu = "11001"
                End If
                valCiudad = "ZO" + xciu '11018
                dstReq = ConsultaAF.dstConsultaEga18(sAplicacion, valCiudad)
                Dim rowZona() As DataRow = dstReq.Tables("EGA18").Select
                For Each dtReq As DataRow In rowZona
                    If (dtReq.Item("CDATTAB") = ds.Tables(0).Rows(0)("Zona").ToString()) Or ds.Tables(0).Rows(0)("Zona").ToString() = "" Then
                        valError = 0
                        Exit For
                    Else
                        valError = valError + 1
                    End If
                Next


                If valError > 0 Then
                    ActualizarAfiliado = xmlDataSetResultado(0, "El elemento 'Zona' tiene un valor no válido según su tipo de datos.")
                    Return ActualizarAfiliado
                End If
                'Fecha:     2016/06/28''
                'Mantis:    16536''
                'Autor:     EXTISSJCACERES'
                'Se elimino la validacion del archivo: TramaActualizarDatosBasicos.xsd para realizarla contra los valores encontrados en la tabla EGA18'
                'VALIDAR EL VALOR DEL NIVEL ESCOLAR
                dstReq = ConsultaAF.dstConsultaEga18(sAplicacion, "XNIVESC")
                Dim rowNivEscolaridad() As DataRow = dstReq.Tables("EGA18").Select
                For Each dtReq As DataRow In rowNivEscolaridad
                    If (dtReq.Item("CDATTAB") = ds.Tables(0).Rows(0)("NivelEscolaridad").ToString()) Or ds.Tables(0).Rows(0)("NivelEscolaridad").ToString() = "" Then
                        valError = 0
                        Exit For
                    Else
                        valError = valError + 1
                    End If
                Next

                If valError > 0 Then
                    ActualizarAfiliado = xmlDataSetResultado(0, "El elemento 'NivelEscolaridad' tiene un valor no válido según su tipo de datos.")
                    Return ActualizarAfiliado
                End If
                'Fin

                'VALIDAR EL VALOR DEL GRUPO POBLACION
                'Fecha:     2016/07/13''
                'Mantis:    16536''
                'Autor:     EXTISSJCACERES'
                'Se corrigue apuntamiento hacia el rowGruPoblacion asociado con la nota mantis: (0153185) 
                dstReq = ConsultaAF.dstConsultaEga18(sAplicacion, "CGRPOBL")
                Dim rowGruPoblacion() As DataRow = dstReq.Tables("EGA18").Select
                For Each dtReq As DataRow In rowGruPoblacion
                    If (dtReq.Item("CDATTAB") = ds.Tables(0).Rows(0)("GrupoPoblacion").ToString()) Or ds.Tables(0).Rows(0)("GrupoPoblacion").ToString() = "" Then
                        valError = 0
                        Exit For
                    Else
                        valError = valError + 1
                    End If
                Next

                If valError > 0 Then
                    ActualizarAfiliado = xmlDataSetResultado(0, "El elemento 'GrupoPoblacion' tiene un valor no válido según su tipo de datos.")
                    Return ActualizarAfiliado
                End If
                'Fin

                'VALIDAR EL VALOR DEL GRUPO ETNICO
                dstReq = ConsultaAF.dstConsultaEga18(sAplicacion, "CGRPETN")
                Dim rowGruEtnico() As DataRow = dstReq.Tables("EGA18").Select
                For Each dtReq As DataRow In rowGruEtnico
                    If (dtReq.Item("CDATTAB") = ds.Tables(0).Rows(0)("GrupoEtnico").ToString()) Or ds.Tables(0).Rows(0)("GrupoEtnico").ToString() = "" Then
                        valError = 0
                        Exit For
                    Else
                        valError = valError + 1
                    End If
                Next

                If valError > 0 Then
                    ActualizarAfiliado = xmlDataSetResultado(0, "El elemento 'GrupoEtnico' tiene un valor no válido según su tipo de datos.")
                    Return ActualizarAfiliado
                End If
                'Fin 16536'
                'VALIDAR EL VALOR DEL CAMPO CODOCUPACION CON LOS DEFINIDOS EN LA EGA18
                dstReq = ConsultaAF.dstConsultaEga18(sAplicacion, "COCUTRA")
                Dim rowOcu() As DataRow = dstReq.Tables("EGA18").Select
                For Each dtReq As DataRow In rowOcu
                    If (dtReq.Item("CDATTAB") = ds.Tables(0).Rows(0)("CodOcupacion").ToString()) Or ds.Tables(0).Rows(0)("CodOcupacion").ToString() = "" Then
                        valError = 0
                        Exit For
                    Else
                        valError = valError + 1
                    End If
                Next

                If valError > 0 Then
                    ActualizarAfiliado = xmlDataSetResultado(0, "El elemento 'CodOcupacion' tiene un valor no válido según su tipo de datos.")
                    Return ActualizarAfiliado
                End If

            Catch ex As Exception
                Return xmlDataSetResultado(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento - ERROR: Consulta Grupo Poblacional - Zona")
            End Try
            'SE VALIDA LA OBLIGATORIEDAD DEL CAMPO PARTEALFABETICA PARA EL NUIP

            If ds.Tables(0).Rows(0)("TipoIdentNew").ToString() = "NU" And ds.Tables(0).Rows(0)("ParteAlfabeticaNew").ToString() = "" Then
                ActualizarAfiliado = xmlDataSetResultado(0, "El elemento 'ParteAlfabeticaNew' tiene un valor no válido según su tipo de datos.")
                Return ActualizarAfiliado
            End If

            'SE CONSULTAN LOS REQUISITOS ASIGNADOS AL CLIENTE
            Try
                dstRequisitos = ConsultaAF.ConsultarRequisitoPorNautcli(sAplicacion, dsDatosBasicos.Tables(0).Rows(0)("I_NAUTCLI").ToString)
            Catch ex As Exception
                Return xmlDataSetResultado(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento - ERROR: Consulta Requisitos")
            End Try
            'SE CONSULTA SI EXISTE EL REQUISITO MODTRA, SI NO EXISTE NO HACE NINGUNA ACCION
            If dstRequisitos.Tables.Count > 0 Then
                dtr = dstRequisitos.Tables(0).Select("XNMNCAM='MODTRA' AND CMNT='A' AND CESTDAT = 5")
                If dtr.Length > 0 Then
                    'Inicio 006613
                    'Dim tramaRadicar As String = "<RADICACION><Radica><TipoIdent>" & ds.Tables(0).Rows(0)("TipoIdent").ToString() & "</TipoIdent>" & _
                    '                            "<NroIdentificacion>" & ds.Tables(0).Rows(0)("NroIdentificacion").ToString() & "</NroIdentificacion>" & _
                    '                            "<DigitoChequeo></DigitoChequeo><Detalleadicional></Detalleadicional><Accion1>A</Accion1>" & _
                    '                            "<Requisito1>MODTRA</Requisito1>" & _
                    '                            "<FechaInicialVigencia1></FechaInicialVigencia1>" & _
                    '                            "<FechaFinalVigencia1></FechaFinalVigencia1>" & _
                    '                            "<DatoNumerico1>6</DatoNumerico1><DatoAlfabetico1>RADICADO UNICO</DatoAlfabetico1>" & _
                    '                            "<Usuario>" & ds.Tables(0).Rows(0)("Usuario").ToString() & "</Usuario>" & _
                    '                            "</Radica></RADICACION>"
                    'Try
                    'strXMLDatosBasicos = Radicar(sAplicacion, tramaRadicar)
                    'Catch ex As Exception
                    'Return xmlDataSetResultado(0, ex.Message)
                    'End Try
                    'dsDatosBasicos = ObjUtil.GetDataSet(strXMLDatosBasicos)
                    'Try
                    '    If dsDatosBasicos.Tables(0).Columns.Contains("MESSAGE") Then
                    '        If dsDatosBasicos.Tables(0).Rows(0)("MESSAGE").ToString().Contains("INPUT REQUEST") Then
                    '            irad = dsDatosBasicos.Tables(0).Rows(0)("I_NRADDOC").ToString()
                    '        Else
                    '            Return xmlDataSetResultado(0, dsDatosBasicos.Tables(0).Rows(0)("MESSAGE").ToString())
                    '        End If
                    '    End If
                    'Catch ex As Exception
                    '    Return xmlDataSetResultado(0, ex.Message)
                    'End Try
                    'Fin 006613
                    irad = dtr(0).Item(12).ToString()
                End If
            End If

            Dim dsDatosBasicosAfil, dsDatosBasicosAfilRes As DataSet
            Dim tramaAfil As String = "<Actualiza>"
            Dim numDoc As String = "", tipoDoc As String = "", partAlf As String = "", strResAfi As String = "", sn_fideliza As Integer = 0, _
                strObsAfi As String = "", strResReq As String = "", strObsReq As String = ""

            dsDatosBasicosAfil = ObjUtil.GetDataSet(strXMLDatosBasicosAfil)
            'dst_resDatUbicacion = ObjUtil.GetDataSet(str_XMLDatUbicacion)
            'SE MANDA DESFIDELIZAR EL CLIENTE 
            If dsDatosBasicosAfil.Tables(0).Rows(0)("I_FVALCLI").ToString() <> "0" Then
                Try
                    sn_fideliza = -1
                    Dim tramaFidelizar As String = "<Fideliza_Radica_NAdicional>" & _
                                                "<TipoIdent>" & ds.Tables(0).Rows(0)("TipoIdent").ToString() & "</TipoIdent>" & _
                                                "<NroIdentificacion>" & ds.Tables(0).Rows(0)("NroIdentificacion").ToString() & "</NroIdentificacion>"
                    If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NU" Then
                        tramaFidelizar = tramaFidelizar & "<ParteAlfabetica>" & ds.Tables(0).Rows(0)("ParteAlfabetica").ToString() & "</ParteAlfabetica>"
                    Else
                        tramaFidelizar = tramaFidelizar & "<ParteAlfabetica />"
                    End If

                    tramaFidelizar = tramaFidelizar & "<DigitoChequeo />"

                    If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                        tramaFidelizar = tramaFidelizar & "<Sucursal>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NIDESUC").ToString() & "</Sucursal>"
                        tramaFidelizar = tramaFidelizar & "<Centrocosto>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NIDECOS").ToString() & "</Centrocosto>"
                    Else
                        tramaFidelizar = tramaFidelizar & "<Sucursal /><Centrocosto />"
                    End If

                    tramaFidelizar = tramaFidelizar & "<Usuario>" & ds.Tables(0).Rows(0)("Usuario").ToString() & "</Usuario></Fideliza_Radica_NAdicional>"

                    CambiarFidelizacion(sAplicacion, tramaFidelizar)
                    ObjReg = New Proceso.ClsRegistro
                Catch ex As Exception
                    Return xmlDataSetResultado(0, ex.Message)
                End Try
            End If

            'SE ARMA TRAMA DE ACTUALIZACION DE DATOS BASICOS
            If ds.Tables(0).Rows(0)("TipoIdentNew").ToString() = "" Then
                tramaAfil = tramaAfil & "<TipoIdent>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XIDECLI").ToString() & "</TipoIdent>"
            Else
                tramaAfil = tramaAfil & "<TipoIdent>" & ds.Tables(0).Rows(0)("TipoIdentNew").ToString() & "</TipoIdent>"
            End If

            If ds.Tables(0).Rows(0)("NroIdentificacionNew").ToString() = "" Then
                tramaAfil = tramaAfil & "<NroIdentificacion>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NIDECLI").ToString() & "</NroIdentificacion>"
            Else
                tramaAfil = tramaAfil & "<NroIdentificacion>" & ds.Tables(0).Rows(0)("NroIdentificacionNew").ToString() & "</NroIdentificacion>"
            End If

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NU" Then
                If ds.Tables(0).Rows(0)("PartealfabeticaNew").ToString() = "" Then
                    tramaAfil = tramaAfil & "<Partealfabetica>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XNUICLI").ToString() & "</Partealfabetica>"
                Else
                    tramaAfil = tramaAfil & "<Partealfabetica>" & ds.Tables(0).Rows(0)("PartealfabeticaNew").ToString() & "</Partealfabetica>"
                End If
            Else
                tramaAfil = tramaAfil & "<Partealfabetica />"
            End If

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                If ds.Tables(0).Rows(0)("DigitoChequeoNew").ToString() = "" Then
                    tramaAfil = tramaAfil & "<DigitoChequeo>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NDIGCHE").ToString() & "</DigitoChequeo>"
                Else
                    tramaAfil = tramaAfil & "<DigitoChequeo>" & ds.Tables(0).Rows(0)("DigitoChequeoNew").ToString() & "</DigitoChequeo>"
                End If
            Else
                tramaAfil = tramaAfil & "<DigitoChequeo />"
            End If

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                If ds.Tables(0).Rows(0)("SucursalNew").ToString() = "" Then
                    tramaAfil = tramaAfil & "<Sucursal>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NIDESUC").ToString() & "</Sucursal>"
                Else
                    tramaAfil = tramaAfil & "<Sucursal>" & ds.Tables(0).Rows(0)("SucursalNew").ToString() & "</Sucursal>"
                End If
            Else
                tramaAfil = tramaAfil & "<Sucursal />"
            End If

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                If ds.Tables(0).Rows(0)("CentroCostoNew").ToString() = "" Then
                    tramaAfil = tramaAfil & "<CentroCosto>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NIDECOS").ToString() & "</CentroCosto>"
                Else
                    tramaAfil = tramaAfil & "<CentroCosto>" & ds.Tables(0).Rows(0)("CentroCostoNew").ToString() & "</CentroCosto>"
                End If
            Else
                tramaAfil = tramaAfil & "<CentroCosto />"
            End If

            tramaAfil = tramaAfil & "<TipoIdentOrg>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XIDECLI").ToString() & "</TipoIdentOrg>"
            tramaAfil = tramaAfil & "<NroIdentificacionOrg>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NIDECLI").ToString() & "</NroIdentificacionOrg>"

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NU" Then
                tramaAfil = tramaAfil & "<ParteAlfabeticaOrg>" & ds.Tables(0).Rows(0)("ParteAlfabetica").ToString() & "</ParteAlfabeticaOrg>"
            Else
                tramaAfil = tramaAfil & "<ParteAlfabeticaOrg />"
            End If


            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                tramaAfil = tramaAfil & "<DigitoChequeoOrg>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NDIGCHE").ToString() & "</DigitoChequeoOrg>"
                tramaAfil = tramaAfil & "<SucursalOrg>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NIDESUC").ToString() & "</SucursalOrg>"
                tramaAfil = tramaAfil & "<CentroCostoOrg>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NIDECOS").ToString() & "</CentroCostoOrg>"
            Else
                tramaAfil = tramaAfil & "<DigitoChequeoOrg /><SucursalOrg /><CentroCostoOrg />"
            End If

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                tramaAfil = tramaAfil & "<Nombre>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XDESRAZ").ToString() & "</Nombre>"
            Else
                If ds.Tables(0).Rows(0)("PrimerNombre").ToString() = "" And ds.Tables(0).Rows(0)("SegundoNombre").ToString() = "" Then
                    tramaAfil = tramaAfil & "<Nombre>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XNOMTRAB").ToString() & " " & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XDESRAZ").ToString() & "</Nombre>"
                ElseIf ds.Tables(0).Rows(0)("PrimerNombre").ToString() = "" And ds.Tables(0).Rows(0)("SegundoNombre").ToString() <> "" Then
                    tramaAfil = tramaAfil & "<Nombre>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XNOMTRAB").ToString() & " " & ds.Tables(0).Rows(0)("SegundoNombre").ToString() & "</Nombre>"
                ElseIf ds.Tables(0).Rows(0)("PrimerNombre").ToString() <> "" And ds.Tables(0).Rows(0)("SegundoNombre").ToString() = "" Then
                    tramaAfil = tramaAfil & "<Nombre>" & ds.Tables(0).Rows(0)("PrimerNombre").ToString() & " " & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XDESRAZ").ToString() & "</Nombre>"
                Else
                    tramaAfil = tramaAfil & "<Nombre>" & ds.Tables(0).Rows(0)("PrimerNombre").ToString() & " " & ds.Tables(0).Rows(0)("SegundoNombre").ToString() & "</Nombre>"
                End If
            End If

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                tramaAfil = tramaAfil & "<Primerapellido />"
            ElseIf ds.Tables(0).Rows(0)("Primerapellido").ToString() = "" Then
                tramaAfil = tramaAfil & "<Primerapellido>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XPRIAPE").ToString() & "</Primerapellido>"
            Else
                tramaAfil = tramaAfil & "<Primerapellido>" & ds.Tables(0).Rows(0)("Primerapellido").ToString() & "</Primerapellido>"
            End If

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                tramaAfil = tramaAfil & "<Segundoapellido />"
            ElseIf ds.Tables(0).Rows(0)("SegundoApellido").ToString() = "" Then
                tramaAfil = tramaAfil & "<Segundoapellido>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XSEGAPE").ToString() & "</Segundoapellido>"
            Else
                tramaAfil = tramaAfil & "<Segundoapellido>" & ds.Tables(0).Rows(0)("SegundoApellido").ToString() & "</Segundoapellido>"
            End If

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                tramaAfil = tramaAfil & "<Fechanacimiento />"
            ElseIf ds.Tables(0).Rows(0)("FechaNacimiento").ToString() = "" Then
                tramaAfil = tramaAfil & "<Fechanacimiento>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_FNACCLI").ToString() & "</Fechanacimiento>"
            Else
                tramaAfil = tramaAfil & "<Fechanacimiento>" & ds.Tables(0).Rows(0)("FechaNacimiento").ToString() & "</Fechanacimiento>"
            End If

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                tramaAfil = tramaAfil & "<Genero />"
            ElseIf ds.Tables(0).Rows(0)("Genero").ToString() = "" Then
                tramaAfil = tramaAfil & "<Genero>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XGENCLI").ToString() & "</Genero>"
            Else
                tramaAfil = tramaAfil & "<Genero>" & ds.Tables(0).Rows(0)("Genero").ToString() & "</Genero>"
            End If

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                tramaAfil = tramaAfil & "<Estadocivil />"
            ElseIf ds.Tables(0).Rows(0)("EstadoCivil").ToString() = "" Then
                tramaAfil = tramaAfil & "<Estadocivil>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_CECICLI").ToString() & "</Estadocivil>"
            Else
                tramaAfil = tramaAfil & "<Estadocivil>" & ds.Tables(0).Rows(0)("EstadoCivil").ToString() & "</Estadocivil>"
            End If
            '20160315/EXTISSJCACERES'
            '[0013711: PRY-VRT-PAQ4 CU-INT- SWPR57-005 Actualizar información afiliado Mejora Usuario sin datos en CLA02]'
            '[Inicio del Mantis: 13711]: (1) Se ajusta para enviar el Tag <Direccion> con valor por defecto'
            If ds.Tables(0).Rows(0)("Direccion").ToString() = "" Then
                If (dsDatosBasicosAfil.Tables(0).Rows(0)("I_XDIRCLI").ToString() = "") Then
                    tramaAfil = tramaAfil & "<Direccion>" & str_porDefecto & "</Direccion>"
                Else
                    tramaAfil = tramaAfil & "<Direccion>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XDIRCLI").ToString() & "</Direccion>"
                End If
            Else
                tramaAfil = tramaAfil & "<Direccion>" & ds.Tables(0).Rows(0)("Direccion").ToString() & "</Direccion>"
            End If

            tramaAfil = tramaAfil & "<DireccionOrg>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XDIRCLI").ToString() & "</DireccionOrg>"

            '[Continuación del Mantis: 13711]: (2) Se ajusta para enviar el Tag <Telefono> con valor por defecto'
            If ds.Tables(0).Rows(0)("Telefono").ToString() = "" Then
                If dsDatosBasicosAfil.Tables(0).Rows(0)("I_NTELCLI").ToString() = "" Then
                    tramaAfil = tramaAfil & "<Telefono>" & int_porDefecto & "</Telefono>"
                Else
                    tramaAfil = tramaAfil & "<Telefono>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NTELCLI").ToString() & "</Telefono>"
                End If
            Else
                tramaAfil = tramaAfil & "<Telefono>" & ds.Tables(0).Rows(0)("Telefono").ToString() & "</Telefono>"
            End If

            '[Continuación del Mantis: 13711]: (3) Se ajusta para enviar el Tag <Extension> con valor por defecto'
            If dsDatosBasicosAfil.Tables(0).Rows(0)("I_NEXTCLI").ToString() = "" Then
                tramaAfil = tramaAfil & "<Extension>" & int_porDefecto & "</Extension>"
            Else
                tramaAfil = tramaAfil & "<Extension>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NEXTCLI").ToString() & "</Extension>"
            End If

            '[Continuación del Mantis: 13711]: (4) Se ajusta para enviar el Tag <TipoDireccion> por defecto'
            If ds.Tables(0).Rows(0)("TipoDireccion").ToString() = "" Then
                If (dsDatosBasicosAfil.Tables(0).Rows(0)("I_XTIPDIR").ToString() = "") Then
                    str_porDefecto = "U"
                    tramaAfil = tramaAfil & "<TipoDireccion>" & str_porDefecto & "</TipoDireccion>"
                Else
                    tramaAfil = tramaAfil & "<TipoDireccion>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XTIPDIR").ToString() & "</TipoDireccion>"
                End If

            Else
                tramaAfil = tramaAfil & "<TipoDireccion>" & ds.Tables(0).Rows(0)("TipoDireccion").ToString() & "</TipoDireccion>"
            End If

            '[Continuación del Mantis: 13711]: (5) Se ajusta para enviar el Tag <Barrio> con valor por defecto'
            If ds.Tables(0).Rows(0)("Barrio").ToString() = "" Then
                If dsDatosBasicosAfil.Tables(0).Rows(0)("I_XBARCLI").ToString() = "" Then
                    str_porDefecto = "NO REPORTA"
                    tramaAfil = tramaAfil & "<Barrio>" & str_porDefecto & "</Barrio>"
                Else
                    tramaAfil = tramaAfil & "<Barrio>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XBARCLI").ToString() & "</Barrio>"
                End If
            Else
                tramaAfil = tramaAfil & "<Barrio>" & ds.Tables(0).Rows(0)("Barrio").ToString() & "</Barrio>"
            End If
            '[Continuación del Mantis: 13711]: (6) Se Envia el Tag <Zona> vacio'            
            If ds.Tables(0).Rows(0)("Zona").ToString() = "" Then
                If dsDatosBasicosAfil.Tables(0).Rows(0)("I_CZONBTA").ToString() = "" Then
                    int_porDefecto = 999
                    tramaAfil = tramaAfil & "<Zona>" & int_porDefecto & "</Zona>"
                Else
                    tramaAfil = tramaAfil & "<Zona>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_CZONBTA").ToString() & "</Zona>"
                End If
            Else
                tramaAfil = tramaAfil & "<Zona>" & ds.Tables(0).Rows(0)("Zona").ToString() & "</Zona>"
            End If
            '[Continuación del Mantis: 13711]: (7) Se ajusta para enviar el Tag <Ciudad> con valor por defecto'            
            If ds.Tables(0).Rows(0)("Ciudad").ToString() = "" Then
                If dsDatosBasicosAfil.Tables(0).Rows(0)("I_CCIUCLI").ToString() = "" Then
                    str_porDefecto = xciu
                    tramaAfil = tramaAfil & "<Ciudad>" & str_porDefecto & "</Ciudad>"
                Else
                    tramaAfil = tramaAfil & "<Ciudad>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_CCIUCLI").ToString() & "</Ciudad>"
                End If
            Else
                tramaAfil = tramaAfil & "<Ciudad>" & ds.Tables(0).Rows(0)("Ciudad").ToString() & "</Ciudad>"
            End If

            tramaAfil = tramaAfil & "<Detalleadicional />"

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                If ds.Tables(0).Rows(0)("RazonSocial").ToString() = "" Then
                    tramaAfil = tramaAfil & "<Razonsocial>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XDESRAZ").ToString() & "</Razonsocial>"
                Else
                    tramaAfil = tramaAfil & "<Razonsocial>" & ds.Tables(0).Rows(0)("RazonSocial").ToString() & "</Razonsocial>"
                End If
            Else
                tramaAfil = tramaAfil & "<Razonsocial>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_XDESRAZ").ToString() & "</Razonsocial>"
            End If

            tramaAfil = tramaAfil & "<Usuario>" & ds.Tables(0).Rows(0)("Usuario").ToString() & "</Usuario>"

            tramaAfil = tramaAfil & "<ConfirmaApellNomb /><SenalConfirmaFonetico /><SenalConfirma />"

            If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                tramaAfil = tramaAfil & "<ConfirmaEstadoCivil />"
            Else
                tramaAfil = tramaAfil & "<ConfirmaEstadoCivil>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_CECICLI").ToString() & "</ConfirmaEstadoCivil>"
            End If

            tramaAfil = tramaAfil & "<Operacion>CHG</Operacion></Actualiza>"

            '20160309/EXTISSJCACERES'
            '[Continuacion del Mantis: 13711]: Se Ensambla la trama para Actualizar Datos de Ubicacion'
            'SE ENVIA A ACTUALIZAR 
            strXMLDatosBasicosAfil = ActualizaCliente(sAplicacion, tramaAfil)
            dsDatosBasicosAfilRes = ObjUtil.GetDataSet(strXMLDatosBasicosAfil)

            Try
                If dsDatosBasicosAfilRes.Tables(0).Columns.Contains("Mensaje") Then
                    If dsDatosBasicosAfilRes.Tables(0).Rows(0)("Mensaje").ToString().Contains("MODIFICACIÓN EXITOSA") Or _
                        dsDatosBasicosAfilRes.Tables(0).Rows(0)("Mensaje").ToString().Contains("617") Or _
                        dsDatosBasicosAfilRes.Tables(0).Rows(0)("Mensaje").ToString().Contains("8199") Then
                        strResAfi = "OK"
                        If (ds.Tables(0).Rows(0)("TipoIdent").ToString() = ds.Tables(0).Rows(0)("TipoIdentNew").ToString()) Or _
                            ds.Tables(0).Rows(0)("TipoIdentNew").ToString() = "" Then
                            tipoDoc = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        Else
                            tipoDoc = ds.Tables(0).Rows(0)("TipoIdentNew").ToString()
                        End If

                        If (ds.Tables(0).Rows(0)("NroIdentificacion").ToString() = ds.Tables(0).Rows(0)("NroIdentificacionNew").ToString()) Or _
                            ds.Tables(0).Rows(0)("NroIdentificacionNew").ToString() = "" Then
                            numDoc = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        Else
                            numDoc = ds.Tables(0).Rows(0)("NroIdentificacionNew").ToString()
                        End If

                        If (ds.Tables(0).Rows(0)("ParteAlfabetica").ToString() = ds.Tables(0).Rows(0)("ParteAlfabeticaNew").ToString()) Or _
                            ds.Tables(0).Rows(0)("ParteAlfabeticaNew").ToString() = "" Then
                            partAlf = ds.Tables(0).Rows(0)("ParteAlfabetica").ToString()
                        Else
                            partAlf = ds.Tables(0).Rows(0)("ParteAlfabeticaNew").ToString()
                        End If
                    Else
                        strResAfi = "ERROR"
                        strObsAfi = dsDatosBasicosAfilRes.Tables(0).Rows(0)("Mensaje").ToString()
                        tipoDoc = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        numDoc = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        partAlf = ds.Tables(0).Rows(0)("ParteAlfabetica").ToString()
                    End If
                Else
                    strResAfi = "ERROR"
                    strObsAfi = dsDatosBasicosAfilRes.Tables(0).Rows(0)("Mensaje_Id").ToString()
                    tipoDoc = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                    numDoc = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                    partAlf = ds.Tables(0).Rows(0)("ParteAlfabetica").ToString()
                End If
            Catch ex As Exception
                strObsAfi = ex.Message
                strResAfi = "ERROR"
                tipoDoc = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                numDoc = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                partAlf = ds.Tables(0).Rows(0)("ParteAlfabetica").ToString()
            End Try

            If sn_fideliza = -1 Then
                Try
                    Dim tramaFidelizar As String = "<Fideliza_Radica_NAdicional>" & _
                                                "<TipoIdent>" & tipoDoc & "</TipoIdent>" & _
                                                "<NroIdentificacion>" & numDoc & "</NroIdentificacion>"
                    If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NU" Then
                        tramaFidelizar = tramaFidelizar & "<ParteAlfabetica>" & partAlf & "</ParteAlfabetica>"
                    Else
                        tramaFidelizar = tramaFidelizar & "<ParteAlfabetica />"
                    End If

                    tramaFidelizar = tramaFidelizar & "<DigitoChequeo />"

                    If ds.Tables(0).Rows(0)("TipoIdent").ToString() = "NI" Then
                        tramaFidelizar = tramaFidelizar & "<Sucursal>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NIDESUC").ToString() & "</Sucursal>"
                        tramaFidelizar = tramaFidelizar & "<Centrocosto>" & dsDatosBasicosAfil.Tables(0).Rows(0)("I_NIDECOS").ToString() & "</Centrocosto>"
                    Else
                        tramaFidelizar = tramaFidelizar & "<Sucursal /><Centrocosto />"
                    End If

                    tramaFidelizar = tramaFidelizar & "<Usuario>" & ds.Tables(0).Rows(0)("Usuario").ToString() & "</Usuario></Fideliza_Radica_NAdicional>"

                    CambiarFidelizacion(sAplicacion, tramaFidelizar)
                    ObjReg = New Proceso.ClsRegistro
                Catch ex As Exception
                    xmlDataSetResultado(0, ex.Message)
                End Try
            End If


            'SE ARMA TRAMA PARA CONSULTAR CLIENTE CON BASE EN LOS DATOS DE IDENTIFICACION ACTUALIZADOS
            xmlConsulAfi = "<AFILIADO><TipoIdent>" & tipoDoc & "</TipoIdent>" & _
                                        "<NroIdentificacion>" & numDoc & "</NroIdentificacion>" & _
                                        "<ParteAlfabetica>" & partAlf & "</ParteAlfabetica>" & _
                                        "<Sucursal>0</Sucursal><CentroCosto>0</CentroCosto></AFILIADO>"

            strXMLDatosBasicosAfil = ConsultaCliente(sAplicacion, xmlConsulAfi)
            dsDatosBasicos = ObjUtil.GetDataSet(strXMLDatosBasicosAfil)

            dstRequisitos = ConsultaAF.ConsultarRequisitoPorNautcli(sAplicacion, dsDatosBasicos.Tables(0).Rows(0)("I_NAUTCLI").ToString)
            'PROCESO DE ACTUALIZACION DE DATOS DE CONTACTO - REQUISITOS
            Dim arrayReq(8) As String, arrayCampos() As String, arrayGrupo() As Integer
            '--2015/09/01--' '--MANTIS: 8934--'
            '''EXITSSJCACERES: Se cambio en Array de Requisitos "TIPIED" por "GRPOBL"'''
            'arrayReq = {"CODOCU", "GRPETN", "TIPIED", "NIVESC", "CORELE", "TELCEL", "AUTENC", "AUTENV"}
            arrayReq = {"CODOCU", "GRPETN", "GRPOBL", "NIVESC", "CORELE", "TELCEL", "AUTENC", "AUTENV"}
            arrayCampos = {"CodOcupacion", "GrupoEtnico", "GrupoPoblacion", "NivelEscolaridad", "CorreoElectronico", "Celular", "AutMsjTexto", "AutMsjCorreoElect"}
            arrayGrupo = {19, 2, 5, 2, 16, 16, 29, 29}

            Dim tramaRadicarReq As String = "<RADICACION><Radica><TipoIdent>" & tipoDoc & "</TipoIdent>" & _
                            "<NroIdentificacion>" & numDoc & "</NroIdentificacion>" & _
                            "<ParteAlfabetica>" & partAlf & "</ParteAlfabetica>" & _
                            "<DigitoChequeo></DigitoChequeo><Detalleadicional></Detalleadicional>"

            Dim i As Integer = 0, indice As Integer = 1, cont As Integer = 0, _
            strcad As String = "", accion As String = ""
            While i < 8
                If ds.Tables(0).Rows(0)(arrayCampos(i)).ToString() <> "" Then
                    dtr = dstRequisitos.Tables(0).Select("XNMNCAM='" & arrayReq(i) & "' AND CMNT='A'")
                    If arrayReq(i) = "CORELE" Then
                        strcad = "<DatoNumerico" & indice & " />" & _
                                    "<DatoAlfabetico" & indice & ">" & ds.Tables(0).Rows(0)(arrayCampos(i)).ToString() & "</DatoAlfabetico" & indice & ">"
                    Else
                        strcad = "<DatoNumerico" & indice & ">" & ds.Tables(0).Rows(0)(arrayCampos(i)).ToString() & "</DatoNumerico" & indice & ">" & _
                                    "<DatoAlfabetico" & indice & " />"
                    End If
                    If arrayGrupo(i) = 29 Then
                        accion = "A"
                    Else
                        accion = "C"
                    End If
                    If dtr.Length = 0 Then ' SI NO EXISTE SE ADICIONA EL REQUISITO Y SU VALOR
                        tramaRadicarReq = tramaRadicarReq & "<Accion" & indice & ">A</Accion" & indice & ">" & _
                                                            "<Requisito" & indice & ">" & arrayReq(i) & "</Requisito" & indice & ">" & _
                                                            "<FechaInicialVigencia" & indice & "></FechaInicialVigencia" & indice & ">" & _
                                                            "<FechaFinalVigencia" & indice & "></FechaFinalVigencia" & indice & ">" & _
                                                            strcad
                        indice = indice + 1
                    Else 'SI EXISTE SE ACTUALIZA EL VALOR DEL REQUISITO
                        tramaRadicarReq = tramaRadicarReq & "<Accion" & indice & ">" & accion & "</Accion" & indice & ">" & _
                                                            "<Requisito" & indice & ">" & arrayReq(i) & "</Requisito" & indice & ">" & _
                                                            "<FechaInicialVigencia" & indice & "></FechaInicialVigencia" & indice & ">" & _
                                                            "<FechaFinalVigencia" & indice & "></FechaFinalVigencia" & indice & ">" & _
                                                            strcad
                        indice = indice + 1
                    End If
                Else
                    cont = cont + 1
                End If
                i = i + 1
            End While

            If cont < 8 Then ' SE ENVIA TRAMA CON EL TOTAL DE REQUISITOS 
                tramaRadicarReq = tramaRadicarReq & "<Usuario>" & ds.Tables(0).Rows(0)("Usuario").ToString() & "</Usuario>" & _
                                               "</Radica></RADICACION>"
                strXMLDatosBasicos = Radicar(sAplicacion, tramaRadicarReq)
                dsDatosBasicos = ObjUtil.GetDataSet(strXMLDatosBasicos)

                Try
                    If dsDatosBasicos.Tables(0).Columns.Contains("MESSAGE") Then
                        If dsDatosBasicos.Tables(0).Rows(0)("MESSAGE").ToString().Contains("INPUT REQUEST") Then
                            strResReq = "OK"
                        Else
                            strResReq = "ERROR"
                            strObsReq = dsDatosBasicos.Tables(0).Rows(0)("MESSAGE").ToString()
                        End If
                    Else
                        strResReq = "ERROR"
                        strObsReq = dsDatosBasicos.Tables(0).Rows(0)("Mensaje").ToString()
                    End If
                Catch ex As Exception
                    strResReq = "ERROR"
                    strObsReq = ex.Message
                End Try
            Else
                strResReq = "OK"
            End If
            ' SE ENVIA A LOG LA TRAMA DE ENTRADA
            ffinConsumo = Now
            'Fecha:     2016/07/07'
            'Mantis:    16536''
            'Autor:     EXTISSJCACERES'
            'Se quita comentario que se puso provisionalmente, para ConsultaAF.InsertarBitacora debido a que generaba excepcion "Could not find the procedure: sp_POS_ADD_Log_WS" que afectaba el correcto funcionamiento de las pruebas. Se soluciono creando lo relacionado con procedimiento nuevamente en COMADMO y COMEPS.  
            ConsultaAF.InsertarBitacora(strApp, HttpContext.Current.Request.ServerVariables("REMOTE_ADDR"), "ActualizarAfiliado", fIniConsumo, ffinConsumo, strXml, strResAfi, sAplicacion)
            'DEVUELVE TRAMA RESPUESTA DE ACTUALIZACION
            If irad = "0" Then
                irad = ""
            End If

            Dim tramaSalida As String = ""
            If strResAfi = "OK" And strResReq = "OK" Then
                tramaSalida = "<NewDataSet><Table><Respuesta>El sistema realizo la actualización de datos básicos y actualización de requisitos o datos de contacto</Respuesta>" & _
                                "<Radicado>" & irad & "</Radicado><Observacion></Observacion></Table></NewDataSet>"
            ElseIf strResAfi = "OK" And strResReq = "ERROR" Then
                tramaSalida = "<NewDataSet><Table><Respuesta>El sistema realizo la actualización de datos básicos, pero no se pudo realizar la actualización de requisitos o datos de contacto</Respuesta>" & _
                                "<Radicado>" & irad & "</Radicado><Observacion>" & strObsReq & "</Observacion></Table></NewDataSet>"
            ElseIf strResAfi = "ERROR" And strResReq = "OK" Then
                tramaSalida = "<NewDataSet><Table><Respuesta>El sistema realizo la actualización de requisitos o datos de contacto, pero no se pudo realizar la actualización de datos básicos</Respuesta>" & _
                                "<Radicado>" & irad & "</Radicado><Observacion>" & strObsAfi & "</Observacion></Table></NewDataSet>"
            ElseIf strResAfi = "ERROR" And strResReq = "ERROR" Then
                tramaSalida = "<NewDataSet><Table><Respuesta>El sistema no pudo realizar la actualización de datos básicos ni la actualización de requisitos o datos de contacto</Respuesta>" & _
                                "<Radicado>" & irad & "</Radicado><Observacion>" & strObsAfi & ", " & strObsAfi & "</Observacion></Table></NewDataSet>"
            End If

            Return tramaSalida
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    '0007876 inicio
    '''EXTISSJCACERES
    <WebMethod(Description:="Metodo para modificar la informacion de los Datos Basicos y/o de Contacto para Empresas o Depencias")> _
    Public Function ActualizarEmpresa(ByVal strApp As String, _
                                                            ByVal strXML As String) As String
        Try
            Dim str_ConDireccion As String = "1"                    ' Consectutivo de la Direccion o NDIR
            Dim str_valRetorno As String                            ' VALor de RETORNO
            Dim str_aplicacion As String                            ' APLICACION
            str_valRetorno = String.Empty
            Dim dt_fecIniConsumo As DateTime                        ' FECha de INIcio del CONSUMO
            Dim dt_fecFinConsumo As DateTime                        ' FECha de FIN del CONSUMO
            Dim DsActDatBasEmpresa As DataSet                       ' DataSet de DatosBasicos del Afiliado
            Dim DsDatosBasicosAfilRes As DataSet                    '            
            Dim str_traActEmpresa As String = "<Actualiza>"         ' TRAma de la ACTualizacion de la EMPRESA
            Dim str_numDocumento As String = ""                     ' NUMero de DOCUMENTO
            Dim str_tipDocumento As String = ""                     ' TIPo de DOCUMENTO
            Dim str_parAlfabetica As String = ""                    ' PARte ALFABETICA
            Dim str_sucursal As String = ""                         ' sucursal
            Dim str_resAfiEmpresa As String = ""                    ' RESpuesta de la AFIliacion de la EMPRESA
            Dim sn_fideliza As Integer = 0
            Dim str_obsAfiEmpresa As String = ""                    ' OBSservacion de la AFIliacion de la EMPRESA
            Dim strresReq As String = ""                            ' RESpuesta del (los) REQUISITOS
            Dim str_obsRequisitos As String = ""                    ' OBServacion del (los) REQUISITOS
            Dim str_traSalida As String = ""                        ' TRAma de SALIDA
            dt_fecIniConsumo = Now

            Dim str_XMLInfEmpDependencia As String = ""             ' XML de INFormacion de la EMPresa O DEPENDENCIA
            str_aplicacion = ConfigurationSettings.AppSettings("AfilPos_ProjectID")
            Dim str_irad As String = "0"                            'Identificador del RADicado
            'VALIDAR LA TRAMA XML CON ARCHIVO XSD
            str_XMLInfEmpDependencia = Utilidades.CUtil.ValidarXSD(strXML, (System.AppDomain.CurrentDomain.BaseDirectory + "XSD\TramaActualizarEmpresa.xsd"))
            If str_XMLInfEmpDependencia.Length > 4 Then
                ActualizarEmpresa = xmlDataSetResultado(0, str_XMLInfEmpDependencia)
                Return ActualizarEmpresa
            End If

            Dim DsDatBasicos As DataSet                             ' DataSet de DATos Basicos   
            Dim DsDataSet As DataSet = ObjUtil.GetDataSet(strXML)   ' DataSet

            '''20150708 EXTISSJCACERES'''
            '--SI EL TIPO DOCUMENTO ENVIADO ES 'NUIP' SE VALIDA QUE ENVIEN EL CAMPO PARTE ALFABETICA--'
            If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NU" And DsDataSet.Tables(0).Rows(0)("ParAlfActual").ToString() = "" Then
                ActualizarEmpresa = xmlDataSetResultado(0, "El elemento 'ParteAlfabetica' tiene un valor no válido según su tipo de datos.")
                Return ActualizarEmpresa
            End If
            '''20150708 EXTISSJCACERES'''
            '--VALIDAR EL VALOR DEL CAMPO LOCALIDAD O ZONA CON LAS DEFINIDAS EN LA EGA18--'
            Dim dsRequisito As DataSet                              ' DataSet de los REQUISITOS
            Dim int_numError As Integer = 0                         ' NUMero de ERRORES
            Try
                dsRequisito = ConsultaAF.dstConsultaEga18(str_aplicacion, "ZO11001")
                Dim rowZona() As DataRow = dsRequisito.Tables("EGA18").Select
                For Each DtRequisito As DataRow In rowZona
                    If (DtRequisito.Item("CDATTAB") = DsDataSet.Tables(0).Rows(0)("LocZona").ToString()) Or DsDataSet.Tables(0).Rows(0)("LocZona").ToString() = "" Then
                        int_numError = 0
                        Exit For
                    Else
                        int_numError = int_numError + 1
                    End If
                Next
                If int_numError > 0 Then
                    ActualizarEmpresa = xmlDataSetResultado(0, "El elemento 'LocZona' tiene un valor no válido según su tipo de datos.")
                    Return ActualizarEmpresa
                End If
            Catch ex As Exception
                Return xmlDataSetResultado(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento - ERROR: Consulta Grupo Poblacional - Zona")
            End Try
            '''20150708 EXTISSJCACERES'''
            '--SE VALIDA LA OBLIGATORIEDAD DEL CAMPO DE LA PARTE ALFABETICA PARA EL NUIP--'
            If DsDataSet.Tables(0).Rows(0)("TipIdeNuevo").ToString() = "NU" And DsDataSet.Tables(0).Rows(0)("ParAlfNuevo").ToString() = "" Then
                ActualizarEmpresa = xmlDataSetResultado(0, "El elemento 'ParAlfNuevo' tiene un valor no válido según su tipo de datos.")
                Return ActualizarEmpresa
            End If
            '''20150708 EXTISSJCACERES'''
            '--SE ARMA TRAMA PARA CONSULTAR CLIENTE--'
            Dim str_XMLConAfiliado As String = "<AFILIADO><TipoIdent>" & DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() & "</TipoIdent>" & _
                                        "<NroIdentificacion>" & DsDataSet.Tables(0).Rows(0)("NumIdeActual").ToString() & "</NroIdentificacion>" & _
                                        "<ParteAlfabetica>" & DsDataSet.Tables(0).Rows(0)("ParAlfActual").ToString() & "</ParteAlfabetica>" & _
                                        "<Sucursal>" & DsDataSet.Tables(0).Rows(0)("NumDepActual").ToString() & "</Sucursal><CentroCosto>0</CentroCosto></AFILIADO>"
            Dim str_XMLDatBasAfiliado As String 'XML con los DATos BASicos de  un AFIliado
            '''20150708 EXTISSJCACERES'''
            '--SE CONSULTA CLIENTE--'
            Try
                str_XMLDatBasAfiliado = ConsultaCliente(str_aplicacion, str_XMLConAfiliado)
                '''20150709 EXTISSJCACERES'''
                '--SE VALIDA LA EXISTENCIA DEL CLIENTE CON LOS DATOS ENVIADOS--'
                If str_XMLDatBasAfiliado.Contains("CLIENTE NO EXISTE") Then
                    Return xmlDataSetResultado(0, "La empresa no existe, No se pudo realizar la actualización")
                End If
            Catch ex As Exception
                Return xmlDataSetResultado(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento - ERROR: Consulta Cliente")
            End Try
            Dim DrwDataRow As DataRow()
            Dim objTransaccion As New Servicios.ClsServicio
            DsDatBasicos = ObjUtil.GetDataSet(str_XMLDatBasAfiliado)
            Dim DsRequisitos As New DataSet ' DataSet con los REQUISITOS
            Dim DtRequisitos As New DataTable ' DataTable con los REQUISITOS
            '''20150709 EXTISSJCACERES'''
            '--SE CONSULTAN LOS REQUISITOS ASIGNADOS AL CLIENTE--'
            Try
                DsRequisitos = ConsultaAF.ConsultarRequisitoPorNautcli(str_aplicacion, DsDatBasicos.Tables(0).Rows(0)("I_NAUTCLI").ToString)
            Catch ex As Exception
                Return xmlDataSetResultado(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento - ERROR: Consulta Requisitos")
            End Try
            Dim boo_actDatBasicos As Boolean ' ACTualizar DATos BASICOS
            Dim boo_actReqDatContacto As Boolean ' ACTualizar REQuisitos y DATos de CONTACTO 
            '''20150709 EXTISSJCACERES'''
            'SE CONSULTA SI EXISTE EL REQUISITO "MOTREM" EN ESTADO TRAMITE
            If DsRequisitos.Tables.Count > 0 Then
                '
                DrwDataRow = DsRequisitos.Tables(0).Select("XNMNCAM='MOTREM' AND CMNT='A' AND CESTDAT = 5")
                '3.	Si se tiene un requisito en estado en trámite no debe permitir crear uno nuevo
                If DrwDataRow.Length > 0 Then
                    '3.	Para realizar la radicación de cambio de datos básicos
                    str_irad = DrwDataRow(0).Item(12).ToString()
                    boo_actDatBasicos = False
                    str_obsAfiEmpresa = "Empresa con requisito MOTREM en tramite; no se pueden actualizar Datos Basicos"
                Else
                    boo_actDatBasicos = True
                    str_obsAfiEmpresa = ""
                End If
            Else
                str_obsAfiEmpresa = "Empresa no tiene el requisito MOTREM"
            End If

            'SE CONSULTA SI EXISTE EL REQUISITO "MODCON"
            If DsRequisitos.Tables.Count > 0 Then                '
                DrwDataRow = DsRequisitos.Tables(0).Select("XNMNCAM='MODCON' AND CMNT='A' AND CESTDAT=5")
                '4.	Para realizar la radicación de cambio de datos de contacto y requisitos
                If DrwDataRow.Length > 0 Then
                    str_irad = DrwDataRow(0).Item(12).ToString()
                    boo_actReqDatContacto = False
                    str_obsRequisitos = "Empresa con requisito MODCOM en tramite, no se pueden actualizar Datos de Contacto o Requisitos"
                Else
                    boo_actReqDatContacto = True
                    str_obsRequisitos = ""
                End If
            Else
                str_obsRequisitos = "Empresa no tiene el requisito MODCOM"
            End If
            Dim str_afiemp As String
            '20150721 - SE PRUEBA SI EL CLIENTE EMPRESA ESTA REGISTRADO CON CEDULA CIUDADANIA O CEDULA DE EXTRANJERIA
            If (DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "CC" Or DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "CE") Then
                If (DsRequisitos.Tables.Count > 0) Then
                    DrwDataRow = DsRequisitos.Tables(0).Select("XNMNCAM='AFIEMP' AND CMNT='A'")
                    If DrwDataRow.Length > 0 Then
                        str_afiemp = DrwDataRow(0).Item(12).ToString()
                        str_ConDireccion = "2"

                    End If
                End If
            End If

            '''20150721 - SE PRUEBA SI SE ACTUALIZARAN LOS DATOS BASICOS Y LOS DATOS DE CONTACTO/REQUISITOS
            If boo_actDatBasicos = True And boo_actReqDatContacto = True Then
                str_traSalida = "<NewDataSet><Table><Respuesta>El sistema realizo la actualización de datos básicos y actualización de requisitos o datos de contacto</Respuesta>" & _
                                "<Radicado>" & str_irad & "</Radicado><Observacion></Observacion></Table></NewDataSet>"
            ElseIf boo_actDatBasicos = True And boo_actReqDatContacto = False Then
                str_traSalida = "<NewDataSet><Table><Respuesta>El sistema realizo la actualización de datos básicos, pero no se pudo realizar la actualización de requisitos o datos de contacto</Respuesta>" & _
                                "<Radicado>" & str_irad & "</Radicado><Observacion>" & str_obsRequisitos & "</Observacion></Table></NewDataSet>"
            ElseIf boo_actDatBasicos = False And boo_actReqDatContacto = True Then
                str_traSalida = "<NewDataSet><Table><Respuesta>El sistema realizo la actualización de requisitos o datos de contacto, pero no se pudo realizar la actualización de datos básicos</Respuesta>" & _
                                "<Radicado>" & str_irad & "</Radicado><Observacion>" & str_obsAfiEmpresa & "</Observacion></Table></NewDataSet>"
            ElseIf boo_actDatBasicos = False And boo_actReqDatContacto = False Then
                str_traSalida = "<NewDataSet><Table><Respuesta>El sistema no pudo realizar la actualización de datos básicos ni la actualización de requisitos o datos de contacto</Respuesta>" & _
                                "<Radicado>" & str_irad & "</Radicado><Observacion>" & str_obsAfiEmpresa & ", " & str_obsRequisitos & "</Observacion></Table></NewDataSet>"
            End If

            '20150709 - EXTISSJCACERES
            '''SE ACTUALIZAN LOS DATOS BÁSICOS'''
            If (boo_actDatBasicos = True) Then
                DsActDatBasEmpresa = ObjUtil.GetDataSet(str_XMLDatBasAfiliado)
                If DsActDatBasEmpresa.Tables(0).Rows(0)("I_FVALCLI").ToString() <> "0" Then
                    Try
                        sn_fideliza = -1
                        Dim tramaFidelizar As String = "<Fideliza_Radica_NAdicional>" & _
                                                    "<TipoIdent>" & DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() & "</TipoIdent>" & _
                                                    "<NroIdentificacion>" & DsDataSet.Tables(0).Rows(0)("NumIdeActual").ToString() & "</NroIdentificacion>"
                        If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NU" Then
                            tramaFidelizar = tramaFidelizar & "<ParteAlfabetica>" & DsDataSet.Tables(0).Rows(0)("ParAlfActual").ToString() & "</ParteAlfabetica>"
                        Else
                            tramaFidelizar = tramaFidelizar & "<ParteAlfabetica />"
                        End If
                        tramaFidelizar = tramaFidelizar & "<DigitoChequeo/>"
                        If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                            tramaFidelizar = tramaFidelizar & "<Sucursal>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NIDESUC").ToString() & "</Sucursal>"
                            tramaFidelizar = tramaFidelizar & "<Centrocosto>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NIDECOS").ToString() & "</Centrocosto>"
                        Else
                            tramaFidelizar = tramaFidelizar & "<Sucursal /><Centrocosto />"
                        End If
                        tramaFidelizar = tramaFidelizar & "<Usuario>" & DsDataSet.Tables(0).Rows(0)("Usuario").ToString() & "</Usuario></Fideliza_Radica_NAdicional>"
                        CambiarFidelizacion(str_aplicacion, tramaFidelizar)
                        ObjReg = New Proceso.ClsRegistro
                    Catch ex As Exception
                        Return xmlDataSetResultado(0, ex.Message)
                    End Try
                End If
                '20150709 - EXTISSJCACERES SE ARMA TRAMA DE ACTUALIZACION DE DATOS BASICOS
                If DsDataSet.Tables(0).Rows(0)("TipIdeNuevo").ToString() = "" Then
                    str_traActEmpresa = str_traActEmpresa & "<TipoIdent>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XIDECLI").ToString() & "</TipoIdent>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<TipoIdent>" & DsDataSet.Tables(0).Rows(0)("TipIdeNuevo").ToString() & "</TipoIdent>"
                End If

                If DsDataSet.Tables(0).Rows(0)("NumIdeNuevo").ToString() = "" Then
                    str_traActEmpresa = str_traActEmpresa & "<NroIdentificacion>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NIDECLI").ToString() & "</NroIdentificacion>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<NroIdentificacion>" & DsDataSet.Tables(0).Rows(0)("NumIdeNuevo").ToString() & "</NroIdentificacion>"
                End If

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NU" Then
                    If DsDataSet.Tables(0).Rows(0)("ParAlfNuevo").ToString() = "" Then
                        str_traActEmpresa = str_traActEmpresa & "<Partealfabetica>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XNUICLI").ToString() & "</Partealfabetica>"
                    Else
                        str_traActEmpresa = str_traActEmpresa & "<Partealfabetica>" & DsActDatBasEmpresa.Tables(0).Rows(0)("ParAlfNuevo").ToString() & "</Partealfabetica>"
                    End If
                Else
                    str_traActEmpresa = str_traActEmpresa & "<Partealfabetica/>"
                End If

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    If DsDataSet.Tables(0).Rows(0)("DigCheNuevo").ToString() = "" Then
                        str_traActEmpresa = str_traActEmpresa & "<DigitoChequeo>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NDIGCHE").ToString() & "</DigitoChequeo>"
                    Else
                        str_traActEmpresa = str_traActEmpresa & "<DigitoChequeo>" & DsDataSet.Tables(0).Rows(0)("DigCheNuevo").ToString() & "</DigitoChequeo>"
                    End If
                Else
                    str_traActEmpresa = str_traActEmpresa & "<DigitoChequeo/>"
                End If

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    If DsDataSet.Tables(0).Rows(0)("NumDepActual").ToString() = "" Then
                        str_traActEmpresa = str_traActEmpresa & "<Sucursal>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NIDESUC").ToString() & "</Sucursal>"
                    Else
                        str_traActEmpresa = str_traActEmpresa & "<Sucursal>" & DsDataSet.Tables(0).Rows(0)("numDepActual").ToString() & "</Sucursal>"
                    End If
                Else
                    str_traActEmpresa = str_traActEmpresa & "<Sucursal/>"
                End If

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    str_traActEmpresa = str_traActEmpresa & "<CentroCosto>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NIDECOS").ToString() & "</CentroCosto>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<CentroCosto/>"
                End If

                str_traActEmpresa = str_traActEmpresa & "<TipoIdentOrg>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XIDECLI").ToString() & "</TipoIdentOrg>"
                str_traActEmpresa = str_traActEmpresa & "<NroIdentificacionOrg>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NIDECLI").ToString() & "</NroIdentificacionOrg>"

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NU" Then
                    str_traActEmpresa = str_traActEmpresa & "<ParteAlfabeticaOrg>" & DsDataSet.Tables(0).Rows(0)("ParAlfActual").ToString() & "</ParteAlfabeticaOrg>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<ParteAlfabeticaOrg/>"
                End If

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    str_traActEmpresa = str_traActEmpresa & "<DigitoChequeoOrg>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NDIGCHE").ToString() & "</DigitoChequeoOrg>"
                    str_traActEmpresa = str_traActEmpresa & "<SucursalOrg>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NIDESUC").ToString() & "</SucursalOrg>"
                    str_traActEmpresa = str_traActEmpresa & "<CentroCostoOrg>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NIDECOS").ToString() & "</CentroCostoOrg>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<DigitoChequeoOrg /><SucursalOrg/><CentroCostoOrg/>"
                End If
                '20150713 - SE ENVIAN LOS DATOS CORRESPONDIENTES A LOS NOMBRES
                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    str_traActEmpresa = str_traActEmpresa & "<Nombre>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XDESRAZ").ToString() & "</Nombre>"
                Else
                    If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "CC" Or DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString = "CE" Then
                        If (DsDataSet.Tables(0).Rows(0)("PriNombre").ToString() = "" And DsDataSet.Tables(0).Rows(0)("SegNombre").ToString() = "") Then
                            str_traActEmpresa = str_traActEmpresa & "<Nombre>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XNOMTRAB").ToString() & " " & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XDESRAZ").ToString() & "</Nombre>"
                        ElseIf DsDataSet.Tables(0).Rows(0)("PriNombre").ToString() = "" And DsDataSet.Tables(0).Rows(0)("SegNombre").ToString() <> "" Then
                            str_traActEmpresa = str_traActEmpresa & "<Nombre>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XNOMTRAB").ToString() & " " & DsDataSet.Tables(0).Rows(0)("SegundoNombre").ToString() & "</Nombre>"
                        ElseIf DsDataSet.Tables(0).Rows(0)("PriNombre").ToString() <> "" And DsDataSet.Tables(0).Rows(0)("SegNombre").ToString() = "" Then
                            str_traActEmpresa = str_traActEmpresa & "<Nombre>" & DsDataSet.Tables(0).Rows(0)("PriNombre").ToString() & " " & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XDESRAZ").ToString() & "</Nombre>"
                        Else
                            str_traActEmpresa = str_traActEmpresa & "<Nombre>" & DsDataSet.Tables(0).Rows(0)("PriNombre").ToString() & " " & DsDataSet.Tables(0).Rows(0)("SegNombre").ToString() & "</Nombre>"
                        End If
                    End If
                End If
                '20150713 - SE ENVIAN LOS DATOS CORRESPONDIENTES A LOS APELLIDOS
                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    str_traActEmpresa = str_traActEmpresa & "<Primerapellido/>"
                ElseIf DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "CC" Or DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "CE" Then
                    If DsDataSet.Tables(0).Rows(0)("PriApellido").ToString() = "" Then
                        str_traActEmpresa = str_traActEmpresa & "<Primerapellido>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XPRIAPE").ToString() & "</Primerapellido>"
                    Else
                        str_traActEmpresa = str_traActEmpresa & "<Primerapellido>" & DsDataSet.Tables(0).Rows(0)("PriApellido").ToString() & "</Primerapellido>"
                    End If
                End If

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    str_traActEmpresa = str_traActEmpresa & "<Segundoapellido/>"
                ElseIf DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "CC" Or DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "CE" Then
                    If DsDataSet.Tables(0).Rows(0)("SegApellido").ToString() = "" Then
                        str_traActEmpresa = str_traActEmpresa & "<Segundoapellido>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XSEGAPE").ToString() & "</Segundoapellido>"
                    Else
                        str_traActEmpresa = str_traActEmpresa & "<Segundoapellido>" & DsDataSet.Tables(0).Rows(0)("SegApellido").ToString() & "</Segundoapellido>"
                    End If
                End If

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    str_traActEmpresa = str_traActEmpresa & "<Fechanacimiento/>"
                Else

                    str_traActEmpresa = str_traActEmpresa & "<Fechanacimiento>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_FNACCLI").ToString() & "</Fechanacimiento>"
                End If

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    str_traActEmpresa = str_traActEmpresa & "<Genero/>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<Genero>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XGENCLI").ToString() & "</Genero>"
                End If

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    str_traActEmpresa = str_traActEmpresa & "<Estadocivil/>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<Estadocivil>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_CECICLI").ToString() & "</Estadocivil>"
                End If

                If DsDataSet.Tables(0).Rows(0)("Direccion").ToString() = "" Then
                    str_traActEmpresa = str_traActEmpresa & "<Direccion>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XDIRCLI").ToString() & "</Direccion>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<Direccion>" & DsDataSet.Tables(0).Rows(0)("Direccion").ToString() & "</Direccion>"
                End If

                str_traActEmpresa = str_traActEmpresa & "<DireccionOrg>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XDIRCLI").ToString() & "</DireccionOrg>"

                If DsDataSet.Tables(0).Rows(0)("NumTelFijo").ToString() = "" Then
                    str_traActEmpresa = str_traActEmpresa & "<Telefono>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NTELCLI").ToString() & "</Telefono>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<Telefono>" & DsDataSet.Tables(0).Rows(0)("NumTelFijo").ToString() & "</Telefono>"
                End If

                str_traActEmpresa = str_traActEmpresa & "<Extension>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_NEXTCLI").ToString() & "</Extension>"

                If DsDataSet.Tables(0).Rows(0)("TipDireccion").ToString() = "" Then
                    str_traActEmpresa = str_traActEmpresa & "<TipoDireccion>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XTIPDIR").ToString() & "</TipoDireccion>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<TipoDireccion>" & DsDataSet.Tables(0).Rows(0)("TipDireccion").ToString() & "</TipoDireccion>"
                End If

                If DsDataSet.Tables(0).Rows(0)("Barrio").ToString() = "" Then
                    str_traActEmpresa = str_traActEmpresa & "<Barrio>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XBARCLI").ToString() & "</Barrio>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<Barrio>" & DsDataSet.Tables(0).Rows(0)("Barrio").ToString() & "</Barrio>"
                End If

                If DsDataSet.Tables(0).Rows(0)("LocZona").ToString() = "" Then
                    str_traActEmpresa = str_traActEmpresa & "<Zona>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_CZONBTA").ToString() & "</Zona>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<Zona>" & DsDataSet.Tables(0).Rows(0)("LocZona").ToString() & "</Zona>"
                End If

                If DsDataSet.Tables(0).Rows(0)("Ciudad").ToString() = "" Then
                    str_traActEmpresa = str_traActEmpresa & "<Ciudad>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_CCIUCLI").ToString() & "</Ciudad>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<Ciudad>" & DsDataSet.Tables(0).Rows(0)("Ciudad").ToString() & "</Ciudad>"
                End If

                str_traActEmpresa = str_traActEmpresa & "<Detalleadicional/>"

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    If DsDataSet.Tables(0).Rows(0)("RazSocial").ToString() = "" Then
                        str_traActEmpresa = str_traActEmpresa & "<Razonsocial>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XDESRAZ").ToString() & "</Razonsocial>"
                    Else
                        str_traActEmpresa = str_traActEmpresa & "<Razonsocial>" & DsDataSet.Tables(0).Rows(0)("RazSocial").ToString() & "</Razonsocial>"
                    End If
                Else
                    str_traActEmpresa = str_traActEmpresa & "<Razonsocial>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_XDESRAZ").ToString() & "</Razonsocial>"
                End If

                str_traActEmpresa = str_traActEmpresa & "<Usuario>" & DsDataSet.Tables(0).Rows(0)("Usuario").ToString() & "</Usuario>"

                str_traActEmpresa = str_traActEmpresa & "<ConfirmaApellNomb/><SenalConfirmaFonetico/><SenalConfirma/>"

                If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                    str_traActEmpresa = str_traActEmpresa & "<ConfirmaEstadoCivil/>"
                Else
                    str_traActEmpresa = str_traActEmpresa & "<ConfirmaEstadoCivil>" & DsActDatBasEmpresa.Tables(0).Rows(0)("I_CECICLI").ToString() & "</ConfirmaEstadoCivil>"
                End If

                str_traActEmpresa = str_traActEmpresa & "<Operacion>CHG</Operacion>"
                '20150715 - SE ADICIONA EL CONSECUTIVO DE LA DIRECCION
                str_traActEmpresa = str_traActEmpresa & "<ConsecutivoDir>" & str_ConDireccion & "</ConsecutivoDir></Actualiza>"

                '20150715 - SE ENVIA A ACTUALIZAR 
                str_XMLDatBasAfiliado = ActualizaCliente(str_aplicacion, str_traActEmpresa)
                DsDatosBasicosAfilRes = ObjUtil.GetDataSet(str_XMLDatBasAfiliado)

                Try
                    If DsDatosBasicosAfilRes.Tables(0).Columns.Contains("Mensaje") Then
                        If DsDatosBasicosAfilRes.Tables(0).Rows(0)("Mensaje").ToString().Contains("MODIFICACIÓN EXITOSA") Or _
                            DsDatosBasicosAfilRes.Tables(0).Rows(0)("Mensaje").ToString().Contains("8199") Then
                            str_resAfiEmpresa = "OK"
                            If (DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = DsDataSet.Tables(0).Rows(0)("TipIdeNuevo").ToString()) Or _
                                DsDataSet.Tables(0).Rows(0)("TipIdeNuevo").ToString() = "" Then
                                str_tipDocumento = DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString()
                            Else
                                str_tipDocumento = DsDataSet.Tables(0).Rows(0)("TipIdeNuevo").ToString()
                            End If

                            If (DsDataSet.Tables(0).Rows(0)("NumIdeActual").ToString() = DsDataSet.Tables(0).Rows(0)("NumIdeNuevo").ToString()) Or _
                                DsDataSet.Tables(0).Rows(0)("NumIdeNuevo").ToString() = "" Then
                                str_numDocumento = DsDataSet.Tables(0).Rows(0)("NumIdeActual").ToString()
                            Else
                                str_numDocumento = DsDataSet.Tables(0).Rows(0)("NumIdeNuevo").ToString()
                            End If

                            If (DsDataSet.Tables(0).Rows(0)("ParAlfActual").ToString() = DsDataSet.Tables(0).Rows(0)("ParAlfNuevo").ToString()) Or _
                                DsDataSet.Tables(0).Rows(0)("ParAlfNuevo").ToString() = "" Then
                                str_parAlfabetica = DsDataSet.Tables(0).Rows(0)("ParAlfActual").ToString()
                            Else
                                str_parAlfabetica = DsDataSet.Tables(0).Rows(0)("ParAlfNuevo").ToString()
                            End If

                            If (DsDataSet.Tables(0).Rows(0)("NumDepActual").ToString() = DsDataSet.Tables(0).Rows(0)("NumDepNuevo").ToString()) Or _
                                DsDataSet.Tables(0).Rows(0)("NumDepNuevo").ToString() = "" Then
                                str_sucursal = DsDataSet.Tables(0).Rows(0)("NumDepActual").ToString()
                            Else
                                str_sucursal = DsDataSet.Tables(0).Rows(0)("NumDepActual").ToString()
                            End If
                        Else
                            str_resAfiEmpresa = "ERROR"
                            str_obsAfiEmpresa = DsDatosBasicosAfilRes.Tables(0).Rows(0)("Mensaje").ToString()
                            str_tipDocumento = DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString()
                            str_numDocumento = DsDataSet.Tables(0).Rows(0)("NumIdeActual").ToString()
                            str_parAlfabetica = DsDataSet.Tables(0).Rows(0)("ParAlfActual").ToString()
                            str_sucursal = DsDataSet.Tables(0).Rows(0)("NumDepActual").ToString()
                        End If
                    Else
                        str_resAfiEmpresa = "ERROR"
                        str_obsAfiEmpresa = DsDatosBasicosAfilRes.Tables(0).Rows(0)("Mensaje_Id").ToString()
                        str_tipDocumento = DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString()
                        str_numDocumento = DsDataSet.Tables(0).Rows(0)("NumIdeActual").ToString()
                        str_parAlfabetica = DsDataSet.Tables(0).Rows(0)("ParAlfActual").ToString()
                        str_sucursal = DsDataSet.Tables(0).Rows(0)("NumDepActual").ToString()
                    End If
                Catch ex As Exception
                    str_obsAfiEmpresa = ex.Message
                    str_resAfiEmpresa = "ERROR"
                    str_tipDocumento = DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString()
                    str_numDocumento = DsDataSet.Tables(0).Rows(0)("NumIdeActual").ToString()
                    str_parAlfabetica = DsDataSet.Tables(0).Rows(0)("ParAlfActual").ToString()
                    str_sucursal = DsDataSet.Tables(0).Rows(0)("NumDepActual").ToString()
                End Try

                If sn_fideliza = -1 Then
                    Try
                        Dim str_traFidelizar As String = "<Fideliza_Radica_NAdicional>" & _
                                                    "<TipoIdent>" & str_tipDocumento & "</TipoIdent>" & _
                                                    "<NroIdentificacion>" & str_numDocumento & "</NroIdentificacion>"
                        If DsDataSet.Tables(0).Rows(0)("TipoIdent").ToString() = "NU" Then
                            str_traFidelizar = str_traFidelizar & "<ParteAlfabetica>" & str_parAlfabetica & "</ParteAlfabetica>"
                        Else
                            str_traFidelizar = str_traFidelizar & "<ParteAlfabetica />"
                        End If

                        str_traFidelizar = str_traFidelizar & "<DigitoChequeo />"

                        If DsDataSet.Tables(0).Rows(0)("TipIdeActual").ToString() = "NI" Then
                            str_traFidelizar = str_traFidelizar & "<Sucursal>" & str_sucursal & "</Sucursal>"
                            str_traFidelizar = str_traFidelizar & "<Centrocosto>0</Centrocosto>"
                        Else
                            str_traFidelizar = str_traFidelizar & "<Sucursal /><Centrocosto />"
                        End If

                        str_traFidelizar = str_traFidelizar & "<Usuario>" & DsDataSet.Tables(0).Rows(0)("Usuario").ToString() & "</Usuario></Fideliza_Radica_NAdicional>"

                        CambiarFidelizacion(str_aplicacion, str_traFidelizar)
                        ObjReg = New Proceso.ClsRegistro
                    Catch ex As Exception
                        xmlDataSetResultado(0, ex.Message)
                    End Try
                End If

                'SE ARMA TRAMA PARA CONSULTAR CLIENTE CON BASE EN LOS DATOS DE IDENTIFICACION ACTUALIZADOS
                str_XMLConAfiliado = "<AFILIADO><TipoIdent>" & str_tipDocumento & "</TipoIdent>" & _
                                    "<NroIdentificacion>" & str_numDocumento & "</NroIdentificacion>" & _
                                    "<ParteAlfabetica>" & str_parAlfabetica & "</ParteAlfabetica>" & _
                                    "<Sucursal>" & str_sucursal & "</Sucursal><CentroCosto>0</CentroCosto></AFILIADO>"

                str_XMLDatBasAfiliado = ConsultaCliente(str_aplicacion, str_XMLConAfiliado)
                DsDatBasicos = ObjUtil.GetDataSet(str_XMLDatBasAfiliado)
                DsRequisitos = ConsultaAF.ConsultarRequisitoPorNautcli(str_aplicacion, DsDatBasicos.Tables(0).Rows(0)("I_NAUTCLI").ToString)
            End If

            If (boo_actReqDatContacto = True) Then
                'SE ARMA TRAMA PARA CONSULTAR CLIENTE CON BASE EN LOS DATOS DE IDENTIFICACION ACTUALIZADOS
                str_XMLConAfiliado = "<AFILIADO><TipoIdent>" & str_tipDocumento & "</TipoIdent>" & _
                                            "<NroIdentificacion>" & str_numDocumento & "</NroIdentificacion>" & _
                                            "<ParteAlfabetica>" & str_parAlfabetica & "</ParteAlfabetica>" & _
                                            "<Sucursal>" & str_sucursal & "</Sucursal><CentroCosto>0</CentroCosto></AFILIADO>"
                str_XMLDatBasAfiliado = ConsultaCliente(str_aplicacion, str_XMLConAfiliado)
                DsDatBasicos = ObjUtil.GetDataSet(str_XMLDatBasAfiliado)

                DsRequisitos = ConsultaAF.ConsultarRequisitoPorNautcli(str_aplicacion, DsDatBasicos.Tables(0).Rows(0)("I_NAUTCLI").ToString)

                Const INT_NUMREQUISITOS As Integer = 10
                Dim str_arrRequisitos(INT_NUMREQUISITOS) As String
                Dim str_arrCampos(INT_NUMREQUISITOS) As String
                Dim int_arrGrupo(INT_NUMREQUISITOS) As Integer

                str_arrRequisitos = {"RAZONS", "CODOCU", "REPLEG", "CORELE", "COREL4", "COREL1", "COREL2", "COREL3", "TELCEL", "NUMFAX"}
                str_arrCampos = {"RazSocial", "ActEconomica", "NomRepLegal", "CorCorporativo", "CorNovPagos", "CorNovAfiliacion", "CorIncapacidades", "CorMovEPS", "NumTelMovil", "NumFax"}

                Dim tramaRadicarReq As String = "<RADICACION><Radica><TipoIdent>" & str_tipDocumento & "</TipoIdent>" & _
                                "<NroIdentificacion>" & str_numDocumento & "</NroIdentificacion>" & _
                                "<DigitoChequeo></DigitoChequeo><Detalleadicional></Detalleadicional>"

                int_arrGrupo = {2, 19, 1, 16, 16, 1, 16, 16, 16, 16}


                Dim int_i As Integer = 0
                Dim int_indice As Integer = 1
                Dim int_contador As Integer = 0
                Dim str_cadena As String = ""
                Dim str_accion As String = ""
                While int_i < INT_NUMREQUISITOS
                    If (DsDataSet.Tables(0).Rows(0)(str_arrCampos(int_i)).ToString() <> "") Then
                        'SE REVISA EL REQUISITO RAZON SOCIAL, REP. LEGAL, COR. ELECTRONICO,COR. ELECTRONICO 1, COR. ELECTRONICO 2,COR. ELECTRONICO 3,COR. ELECTRONICO 4,
                        DrwDataRow = DsRequisitos.Tables(0).Select("XNMNCAM='" & str_arrRequisitos(int_i) & "' AND CMNT='A'")
                        If str_arrRequisitos(int_i) = "RAZONS" Or _
                            str_arrRequisitos(int_i) = "REPLEG" Or _
                            str_arrRequisitos(int_i) = "CORELE" Or _
                            str_arrRequisitos(int_i) = "COREL1" Or _
                            str_arrRequisitos(int_i) = "COREL2" Or _
                            str_arrRequisitos(int_i) = "COREL3" Or _
                            str_arrRequisitos(int_i) = "COREL4" Then

                            If str_arrRequisitos(int_i) = "REPLEG" Then
                                str_cadena = "<DatoNumerico" & int_indice & ">5</DatoNumerico" & int_indice & ">" & _
                                            "<DatoAlfabetico" & int_indice & ">" & DsDataSet.Tables(0).Rows(0)(str_arrCampos(int_i)).ToString() & "</DatoAlfabetico" & int_indice & ">"
                            Else
                                str_cadena = "<DatoNumerico" & int_indice & " />" & _
                                        "<DatoAlfabetico" & int_indice & ">" & DsDataSet.Tables(0).Rows(0)(str_arrCampos(int_i)).ToString() & "</DatoAlfabetico" & int_indice & ">"
                            End If
                        Else
                            str_cadena = "<DatoNumerico" & int_indice & ">" & DsDataSet.Tables(0).Rows(0)(str_arrCampos(int_i)).ToString() & "</DatoNumerico" & int_indice & ">" & _
                                        "<DatoAlfabetico" & int_indice & " />"
                        End If

                        If int_arrGrupo(int_i) = 29 Then
                            str_accion = "A"
                        Else
                            str_accion = "C"
                        End If
                        If DrwDataRow.Length = 0 Then ' SI NO EXISTE SE ADICIONA EL REQUISITO Y SU VALOR
                            tramaRadicarReq = tramaRadicarReq & "<Accion" & int_indice & ">A</Accion" & int_indice & ">" & _
                                                                "<Requisito" & int_indice & ">" & str_arrRequisitos(int_i) & "</Requisito" & int_indice & ">" & _
                                                                "<FechaInicialVigencia" & int_indice & "></FechaInicialVigencia" & int_indice & ">" & _
                                                                "<FechaFinalVigencia" & int_indice & "></FechaFinalVigencia" & int_indice & ">" & _
                                                                str_cadena
                            int_indice = int_indice + 1
                        Else 'SI EXISTE SE ACTUALIZA EL VALOR DEL REQUISITO
                            tramaRadicarReq = tramaRadicarReq & "<Accion" & int_indice & ">" & str_accion & "</Accion" & int_indice & ">" & _
                                                                "<Requisito" & int_indice & ">" & str_arrRequisitos(int_i) & "</Requisito" & int_indice & ">" & _
                                                                "<FechaInicialVigencia" & int_indice & "></FechaInicialVigencia" & int_indice & ">" & _
                                                                "<FechaFinalVigencia" & int_indice & "></FechaFinalVigencia" & int_indice & ">" & _
                                                                str_cadena
                            int_indice = int_indice + 1
                        End If
                    Else
                        int_contador = int_contador + 1

                    End If
                    int_i = int_i + 1
                End While
                If int_contador < INT_NUMREQUISITOS Then ' SE ENVIA TRAMA CON EL TOTAL DE REQUISITOS 
                    tramaRadicarReq = tramaRadicarReq & "<Usuario>" & DsDataSet.Tables(0).Rows(0)("Usuario").ToString() & "</Usuario>" & _
                                                   "</Radica></RADICACION>"
                    str_XMLDatBasAfiliado = Radicar(str_aplicacion, tramaRadicarReq)
                    DsDatBasicos = ObjUtil.GetDataSet(str_XMLDatBasAfiliado)

                    Try
                        If DsDatBasicos.Tables(0).Columns.Contains("MESSAGE") Then
                            If DsDatBasicos.Tables(0).Rows(0)("MESSAGE").ToString().Contains("INPUT REQUEST") Then
                                strresReq = "OK"
                            Else
                                strresReq = "ERROR"
                                str_obsRequisitos = DsDatBasicos.Tables(0).Rows(0)("MESSAGE").ToString()
                            End If
                        Else
                            strresReq = "ERROR"
                            str_obsRequisitos = DsDatBasicos.Tables(0).Rows(0)("Mensaje").ToString()
                        End If
                    Catch ex As Exception
                        strresReq = "ERROR"
                        str_obsRequisitos = ex.Message
                    End Try
                Else
                    strresReq = "OK"
                End If
            End If
            ' SE ENVIA A LOG LA TRAMA DE ENTRADA
            dt_fecFinConsumo = Now
            ConsultaAF.InsertarBitacora(strApp, HttpContext.Current.Request.ServerVariables("REMOTE_ADDR"), "ActualizarEmpresa", dt_fecIniConsumo, dt_fecFinConsumo, strXML, str_resAfiEmpresa, str_aplicacion)
            'DEVUELVE TRAMA RESPUESTA DE ACTUALIZACION
            If str_irad = "0" Then
                str_irad = ""
            End If


            If str_resAfiEmpresa = "OK" And strresReq = "OK" Then
                str_traSalida = "<NewDataSet><Table><Respuesta>El sistema realizo la actualización de datos básicos y actualización de requisitos o datos de contacto</Respuesta>" & _
                                "<Radicado>" & str_irad & "</Radicado><Observacion></Observacion></Table></NewDataSet>"
            ElseIf str_resAfiEmpresa = "OK" And strresReq = "ERROR" Then
                str_traSalida = "<NewDataSet><Table><Respuesta>El sistema realizo la actualización de datos básicos, pero no se pudo realizar la actualización de requisitos o datos de contacto</Respuesta>" & _
                                "<Radicado>" & str_irad & "</Radicado><Observacion>" & str_obsRequisitos & "</Observacion></Table></NewDataSet>"
            ElseIf str_resAfiEmpresa = "ERROR" And strresReq = "OK" Then
                str_traSalida = "<NewDataSet><Table><Respuesta>El sistema realizo la actualización de requisitos o datos de contacto, pero no se pudo realizar la actualización de datos básicos</Respuesta>" & _
                                "<Radicado>" & str_irad & "</Radicado><Observacion>" & str_obsAfiEmpresa & "</Observacion></Table></NewDataSet>"
            ElseIf str_resAfiEmpresa = "ERROR" And strresReq = "ERROR" Then
                str_traSalida = "<NewDataSet><Table><Respuesta>El sistema no pudo realizar la actualización de datos básicos ni la actualización de requisitos o datos de contacto</Respuesta>" & _
                                "<Radicado>" & str_irad & "</Radicado><Observacion>" & str_obsAfiEmpresa & ", " & str_obsRequisitos & "</Observacion></Table></NewDataSet>"
            End If

            Return str_traSalida
        Catch ex As Exception
            'Return ex.Message
            Return xmlDataSetResultado(0, "El Sistema de información del Consorcio no se encuentra disponible en este momento")
        End Try
    End Function

End Class
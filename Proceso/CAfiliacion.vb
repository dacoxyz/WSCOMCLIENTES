#Region "Namespaces"
Imports EntidadesNegocio
Imports ManejoMensajes
''Imports Encripcion
Imports System.Data.SqlClient
Imports Microsoft.Win32
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.IO
Imports System.Xml.Serialization
Imports System.Data
Imports Microsoft.VisualBasic
Imports System.Threading
Imports Compensar.SISPOS
Imports Compensar.SISPOS.DAL.VinculacionPOS
Imports Compensar.Vincula.POS

#End Region
''' -----------------------------------------------------------------------------
''' Project	 : 
''' Class	 : CAfiliacion
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Provee los metodos necesarios para manipular la información de una afiliación 
''' de un cotizante o beneficiario en caja o POS en la base de datos de Transar.
''' </summary>
''' ''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCLOPEZB]	8/23/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Class CAfiliacion
    Dim ObjVin As New Proceso.ClsVinculacion
    Dim ObjUtil As New Utilidades.CUtil
    Dim ObjRad As New Proceso.ClsRadicacion
    Dim oAplicacion As Compensar.Vincula.POS.Helper.coleccion.clsParametros

#Region " Metodos "
    'control de cambios: es llamado desde ETL de actualizacion masiva de estado: sep 9 2009.
    Public Function actualizaEstado(ByVal sXmlIn As String, ByRef sRespuesta As String) As Boolean
        Dim xmlDoc As New Xml.XmlDocument()
        Dim xmlNodo As Xml.XmlNode
        Dim objLogTransaccional As New LogicaTransaccional.ManejadorTransacciones
        objLogTransaccional.CambioEstado(sXmlIn, sRespuesta)
        If sRespuesta.Contains("MESSAGE") Then
            xmlDoc.LoadXml(sRespuesta)
            xmlNodo = xmlDoc.SelectSingleNode("//MESSAGE")
            If InStr(1, xmlNodo.InnerXml.ToString, "SUCCESSFUL ENTRY") > 0 Or InStr(1, xmlNodo.InnerXml.ToString, "INPUT REQUEST") > 0 Then
                sRespuesta = "RADICACION EXITOSA"
                actualizaEstado = True
            Else
                sRespuesta = xmlNodo.InnerXml.ToString()
                actualizaEstado = False
            End If
        ElseIf sRespuesta.Contains("ERROR") Then
            xmlNodo = Nothing
            sRespuesta = sRespuesta
            actualizaEstado = False
        Else
            xmlNodo = Nothing
            sRespuesta = sRespuesta
            actualizaEstado = False
        End If
    End Function
    'control de cambios: es llamado desde ETL de actualizacion masiva de estrato: sep 9 2009.
    Public Function actualizaEstrato(ByVal sXmlIn As String, ByRef sRespuesta As String) As Boolean
        Dim objLogTransaccional As New LogicaTransaccional.ManejadorTransacciones
        If objLogTransaccional.CambioEstado(sXmlIn, sRespuesta) Then
            Return True
        Else
            Return False
        End If

    End Function

    ''' <summary>
    ''' Hace radicacion y cambio estado en una sola operacion.
    ''' </summary>
    ''' <param name="xmlIn"></param>
    ''' <param name="strRepuesta"></param>
    ''' <param name="sAplicacion"></param>
    ''' <returns></returns>
    ''' <remarks>control de cambios sep 09. WLM.</remarks>
    Public Function CambioEstado(ByVal xmlIn As String, ByRef strRepuesta As String, ByVal sAplicacion As String) As Boolean
        CambioEstado = False
        Dim sRadicarResp As String
        Dim xmlDoc As New Xml.XmlDocument()
        Dim xmlNodo As Xml.XmlNode
        Dim Valores As String()
        Dim intContar As Integer
        Dim intContador As Integer = 0
        Dim objVincula As New Compensar.Vincula.POS.LogicaEstados(sAplicacion)
        If objVincula.cambioEstado(xmlIn) Then
            'xmlIn = "<Radica><IndOption>V</IndOption><Estado>0</Estado><Usuario>999999999999</Usuario><TipoIdent>CC</TipoIdent><NroIdentificacion>7306962</NroIdentificacion><ParteAlfabetica></ParteAlfabetica><Sucursal></Sucursal><CentroCosto></CentroCosto><Condicion>0</Condicion><Programa>021203</Programa><TipoIdentEmpr></TipoIdentEmpr><NroIdentEmpr></NroIdentEmpr><Accion1>M</Accion1><Requisito1>ACTSER</Requisito1><FechaInicialVigencia1>0</FechaInicialVigencia1><FechaFinalVigencia1>0</FechaFinalVigencia1><DatoNumerico1></DatoNumerico1><DatoAlfabetico1></DatoAlfabetico1></Radica>,<Radica><IndOption>V</IndOption><Estado>0</Estado><Usuario>999999999999</Usuario><TipoIdent>CC</TipoIdent><NroIdentificacion>7306962</NroIdentificacion><ParteAlfabetica></ParteAlfabetica><Sucursal></Sucursal><CentroCosto></CentroCosto><Condicion>0</Condicion><Programa>021203</Programa><TipoIdentEmpr></TipoIdentEmpr><NroIdentEmpr></NroIdentEmpr><Accion1>M</Accion1><Requisito1>ACTSER</Requisito1><FechaInicialVigencia1>0</FechaInicialVigencia1><FechaFinalVigencia1>0</FechaFinalVigencia1><DatoNumerico1></DatoNumerico1><DatoAlfabetico1></DatoAlfabetico1></Radica>"
            Valores = Split(xmlIn, ",")
            intContar = Valores.Length
            strRepuesta = ""
            For intContador = 0 To intContar - 1
                sRadicarResp = Me.RadicarCambioEstado(sAplicacion, Valores(intContador))
                If sRadicarResp.Contains("MESSAGE") Then
                    xmlDoc.LoadXml(sRadicarResp)
                    xmlNodo = xmlDoc.SelectSingleNode("//MESSAGE")
                    If InStr(1, xmlNodo.InnerXml.ToString, "SUCCESSFUL ENTRY") > 0 Or InStr(1, xmlNodo.InnerXml.ToString, "INPUT REQUEST") > 0 Then
                        CambioEstado = True
                        If strRepuesta <> "" Then
                            strRepuesta = "RADICACION EXITOSA" & "," & strRepuesta
                        Else
                            strRepuesta = "RADICACION EXITOSA"
                        End If
                    Else
                        CambioEstado = False
                        If strRepuesta <> "" Then
                            strRepuesta = xmlNodo.InnerXml.ToString() & "," & strRepuesta
                        Else
                            strRepuesta = xmlNodo.InnerXml.ToString()
                        End If
                    End If
                ElseIf sRadicarResp.Contains("ERROR") Then
                    CambioEstado = False
                    xmlNodo = Nothing
                    If strRepuesta <> "" Then
                        strRepuesta = sRadicarResp & "," & strRepuesta
                    Else
                        strRepuesta = sRadicarResp
                    End If
                Else
                    CambioEstado = True
                    xmlNodo = Nothing
                    If strRepuesta <> "" Then
                        strRepuesta = sRadicarResp & "," & strRepuesta
                    Else
                        strRepuesta = sRadicarResp
                    End If
                End If
            Next
        End If
    End Function
    Public Function ConstruirDatoAfiliacion(ByVal xmlIn As String, ByRef pobjDatosAfiliacion As CDatosAfiliacion, ByRef boolPos As Boolean, ByRef boolCaj As Boolean, ByRef strRepuesta As String, ByVal sAplicacion As String) As Boolean
        'Llena la entidad de negocio de afiliación
        Dim objLogTransaccional As New LogicaTransaccional.ManejadorTransacciones
        If objLogTransaccional.ConstruirDatoAfiliacion(xmlIn, pobjDatosAfiliacion, boolPos, boolCaj, strRepuesta, sAplicacion) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function ConstruirDatoVinculacion(ByRef xmlIn As String, ByVal sAplicacion As String) As Boolean
        'Llena la entidad de negocio de afiliación
        Dim objLogTransaccional As New LogicaTransaccional.ManejadorTransacciones
        If objLogTransaccional.ConstruirDatoVinculacion(xmlIn, sAplicacion) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function ManejoErrorVinc(ByRef xmlIn As String, ByRef comando As String, ByRef fecha As String, ByVal esPos As Boolean) As Boolean
        'Llena la entidad de negocio de afiliación
        Dim objLogTransaccional As New LogicaTransaccional.ManejadorTransacciones
        If objLogTransaccional.ManejoErrorVinc(xmlIn, comando, fecha, esPos) Then
            Return True
        Else
            xmlIn = objLogTransaccional.DescripcionError
            Return False
        End If
    End Function
    'Public Function Requisitos(ByVal xmlIn As String, ByRef strRepuesta As String) As Boolean
    '    'Llena la entidad de negocio de afiliación
    '    Dim objLogTransaccional As New LogicaTransaccional.ManejadorTransacciones
    '    If objLogTransaccional.Requisitos(xmlIn, strRepuesta) Then
    '        Return True
    '    Else
    '        strRepuesta = objLogTransaccional.DescripcionError
    '        Return False
    '    End If
    'End Function
    Public Function Requisitos(ByVal xmlIn As String, ByRef strRepuesta As String, ByVal sAplicacion As String) As Boolean
        Dim returnvalue As String = ""
        Dim ds As DataSet = ObjUtil.GetDataSet(xmlIn)
        Dim pstrXMLDatosRespuesta As String
        If ds Is Nothing Then
            Return False
        Else
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    'En caso de ser un Certificado Escolar se consulta el NIDECLI del beneficiario
                    If ds.Tables(0).Rows(0)("I_XDESREQ").ToString() = "ESC" Then

                    End If
                    Dim dsVin As DataSet
                    If Not pstrXMLDatosRespuesta Is Nothing Then
                        dsVin = ObjUtil.GetDataSet(pstrXMLDatosRespuesta)
                    End If
                    With ObjRad
                        .Aplicacion = sAplicacion
                        If ds.Tables(0).Rows(0)("I_XDESREQ").ToString() = "ESC" Then
                            If Not dsVin Is Nothing Then
                                .TipoIdent = dsVin.Tables(0).Rows(0)("TIDECLI").ToString()
                            Else
                                .TipoIdent = Utilidades.CUtil.darTipoIdCodigo(ds.Tables(0).Rows(0)("I_TIDEBEN").ToString())
                            End If
                        Else
                            .TipoIdent = Utilidades.CUtil.darTipoIdCodigo(ds.Tables(0).Rows(0)("I_TIDEBEN").ToString())
                        End If
                        If ds.Tables(0).Rows(0)("I_XDESREQ").ToString() = "ESC" Then
                            If Not dsVin Is Nothing Then
                                .NroIdentificacion = dsVin.Tables(0).Rows(0)("NIDECLI").ToString()
                            Else
                                .NroIdentificacion = ds.Tables(0).Rows(0)("I_NIDEBEN").ToString()
                            End If
                        Else
                            .NroIdentificacion = ds.Tables(0).Rows(0)("I_NIDEBEN").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("Partealfabetica") Then
                            .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        End If
                        If Not ds.Tables(0).Columns.Contains("Usuario") Then
                            .Usuario = "999999999999"
                        Else
                            .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                        End If
                        .AccReq(0) = "C" 'Accion
                        If ds.Tables(0).Rows(0)("I_XDESREQ").ToString() = "ESC" Then
                            .NameReq(0) = "CERESC"  'Nemonico
                        Else
                            .NameReq(0) = "FDVIDA" 'Nemonico
                        End If
                        .FechIniReq(0) = ""
                        .FechFinReq(0) = ""
                        .DatNumReq(0) = ""
                        If ds.Tables(0).Rows(0)("I_XDESREQ").ToString() = "ESC" Then
                            .DatAlfReq(0) = ds.Tables(0).Rows(0)("I_CNIV") & Utilidades.CUtil.strCompObject(ds.Tables(0).Rows(0)("I_NCUR")) & Utilidades.CUtil.strCompObject(ds.Tables(0).Rows(0)("I_NHOREST")) & ds.Tables(0).Rows(0)("I_NJOR") & ds.Tables(0).Rows(0)("I_XCAL") 'Dato Alfabetico
                        Else
                            .DatAlfReq(0) = "1"  'Dato Alfabetico
                        End If
                    End With
                    strRepuesta = ObjRad.Radicacion()
                End If
            End If
            Return True
        End If
    End Function

    Public Function Afiliar(ByVal pobjDatosAfiliacion As CDatosAfiliacion, ByRef pstrDatosRespuesta As String, ByVal pblnCaja As Boolean, ByVal pblnPOS As Boolean) As Boolean
        Dim Proceso As CProceso
        ' 1. Determinar si es Afiliar Beneficiario o Trabajador 
        If pobjDatosAfiliacion.Persona.GetType.FullName = "EntidadesNegocio.CTrabajador" Then
            '2. Afiliar trabajador 
            Proceso = New CProcesoAfiliarCotizante
            '3. Delegar afiliacion
            If Proceso.Afiliar(pobjDatosAfiliacion, pblnCaja, pblnPOS, pstrDatosRespuesta) Then
                '4. Verificar Respuestas 
                Return True
            Else
                pstrDatosRespuesta = Proceso.DescripcionError
                Return False
            End If
        ElseIf pobjDatosAfiliacion.Persona.GetType.FullName = "EntidadesNegocio.CBeneficiario" Then
            '2. Afiliar beneficiario 
            Proceso = New CProcesoAfiliarBeneficiario
            '3. Delegar afiliacion
            If Proceso.Afiliar(pobjDatosAfiliacion, pblnCaja, pblnPOS, pstrDatosRespuesta) Then
                '4. Verificar Respuestas 
                Return True
            Else
                pstrDatosRespuesta = Proceso.DescripcionError
                Return False
            End If
        Else
            pstrDatosRespuesta = "ERROR Nombre NI:" & pobjDatosAfiliacion.Persona.GetType.FullName
            Return False
        End If ' Cierra pobjIndividuo.GetType.FullName = "BusinessEntities.CTrabajador" 
    End Function
    Public Function AfiliarW(ByVal pobjDatosAfiliacion As CDatosAfiliacion, ByRef pstrDatosRespuesta As String, ByVal pblnCaja As Boolean, ByVal pblnPOS As Boolean) As Boolean
        Dim objlogicatransaccional As New LogicaTransaccional.ManejadorTransacciones

        If objlogicatransaccional.TransacciontrCle15(pobjDatosAfiliacion, pstrDatosRespuesta) Then
            '' pstrDatosRespuesta = pstrDatosRespuesta
            Return True
        Else
            pstrDatosRespuesta = objlogicatransaccional.DescripcionError
            Return False
        End If
    End Function
    'Public Overridable Function leerConexionBDHost(ByRef objConnHost As cAF) As Boolean
    '    Try
    '        Dim strCode As String
    '        Dim strIP As String
    '        strCode = ConfigurationManager.AppSettings("COMDB_CODE")
    '        strIP = ConfigurationManager.AppSettings("COMDB_IP")
    '        objConnHost = New ComDB.cAF(strCode, strIP)
    '        Return True
    '    Catch objException As Exception
    '        ManejoMensajes.CManejadorMensajes.PublicarMensaje(objException.ToString, EventLogEntryType.Error)
    '        mstrError = objException.ToString
    '        Return False
    '    End Try
    'End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método maneja errores a nivel de detalle interno, es decir detalles tecnicos 
    ''' que solo serán relevantes al personal de soporte de TRANSAR 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub ManejarErrorInterno(ByVal pstrUbicacion As String, ByVal pstrCodigoMSG As String, ByVal pstrMensaje As String)
        Dim strBuilder As StringBuilder
        Dim strerror As String = ""
        strBuilder = New StringBuilder(pstrUbicacion)
        CManejadorMensajes.DarMensaje(pstrCodigoMSG, strerror)
        strBuilder.Append(strerror)
        strBuilder.Append(vbCrLf)
        strBuilder.Append(pstrMensaje)
        CManejadorMensajes.PublicarMensaje(strBuilder.ToString(), EventLogEntryType.Error)
    End Sub


    'Public Function Consultarbeneficiarios2(ByRef objdatos As Object, ByRef pstrXMLDatosRespuesta As String) As Boolean
    '    Try
    '        Dim objConnHost As cAF
    '        Dim strXMLResultado As String
    '        If leerConexionBDHost(objConnHost) Then
    '            Dim blnResp As Boolean
    '            Dim strSQL As String
    '            Dim documento As String = objdatos(2)
    '            Dim tipodocumento As String = objdatos(1)
    '            strSQL = " SELECT XNOMBEN + ' ' +XPRIAPE + ' ' +XSEGAPE as nombres,TIDEBEN,NIDEBEN,(CASE WHEN FINGEPS = 0 THEN ' ' ELSE SUBSTRING(CONVERT(VARCHAR,FINGEPS),7,2) + '/' +  SUBSTRING(CONVERT(VARCHAR,FINGEPS),5,2) + '/' + SUBSTRING(CONVERT(VARCHAR,FINGEPS),1,4) END) AS FINGEPS,CCAT,CPRN, (CASE WHEN CESTPOS = 1 AND FRETEPS >= CAST(CONVERT(VARCHAR, GETDATE(),112) AS INTEGER)  THEN '7' ELSE CESTPOS END) AS  CESTPOS ,CESTPCO,CESTSOC,(CASE WHEN FNACBEN = 0 THEN ' ' ELSE SUBSTRING(CONVERT(VARCHAR,FNACBEN),7,2) + '/' +  SUBSTRING(CONVERT(VARCHAR,FNACBEN),5,2) + '/' + SUBSTRING(CONVERT(VARCHAR,FNACBEN),1,4) END) AS FNACBEN, CTIPDOC,NDOCBEN, XSEX, CPRG, CTIPNUI   "
    '            strSQL = strSQL & "FROM AFILDB_EGA02 WHERE TIDETRA = " & tipodocumento & " AND NIDETRA = " & documento
    '            If objConnHost.Query(strSQL, strXMLResultado) Then
    '                pstrXMLDatosRespuesta = strXMLResultado
    '            Else
    '                ManejoMensajes.CManejadorMensajes.PublicarMensaje("CDatos::CAfiliacion.Consultarbeneficiarios:Error devuelto por COMDB: " & objConnHost.DescripcionError, EventLogEntryType.Error)
    '                mstrError = objConnHost.DescripcionError
    '                Return False
    '            End If
    '        Else
    '            ManejoMensajes.CManejadorMensajes.PublicarMensaje("CDatos::CAfiliacion.Consultarbeneficiarios:Error devuelto por COMDB: " & objConnHost.DescripcionError, EventLogEntryType.Error)
    '            mstrError = objConnHost.DescripcionError
    '            Return False
    '        End If
    '    Catch objException As Exception
    '        ManejoMensajes.CManejadorMensajes.PublicarMensaje(objException.ToString, EventLogEntryType.Error)
    '        mstrError = objException.ToString
    '        Return False
    '    End Try
    '    Return True
    'End Function

    'Public Function ConsVinculados(ByRef objdatos As DataSet, ByRef pstrXMLDatosRespuesta As String) As Boolean
    '    Try
    '        Dim objConnHost As cAF
    '        Dim strXMLResultado As String
    '        If leerConexionBDHost(objConnHost) Then
    '            Dim blnResp As Boolean
    '            Dim strSQL As String
    '            Dim documento As String = objdatos.Tables(0).Rows(0)("I_NIDETRA").ToString()
    '            Dim FechaNacimiento As String = objdatos.Tables(0).Rows(0)("I_NIDEBEN").ToString()
    '            strSQL = "SELECT  TIDECLI, NIDECLI FROM AFILDB_CLA00 WHERE NAUTCLI IN (SELECT NAUTCLI FROM AFILDB_VIM00 (NOLOCK) "
    '            strSQL = strSQL & " WHERE NAUTR01 IN (SELECT NAUTCLI FROM AFILDB_CLA00 (NOLOCK) WHERE NIDECLI=" & documento & ") "
    '            strSQL = strSQL & " AND CMNT='A') AND FNACCLI=" & FechaNacimiento
    '            If objConnHost.Query(strSQL, strXMLResultado) Then
    '                pstrXMLDatosRespuesta = strXMLResultado
    '            Else
    '                ManejoMensajes.CManejadorMensajes.PublicarMensaje("CDatos::CAfiliacion.ConsTrabajador:Error devuelto por COMDB: " & objConnHost.DescripcionError, EventLogEntryType.Error)
    '                mstrError = objConnHost.DescripcionError
    '                Return False
    '            End If
    '        Else
    '            ManejoMensajes.CManejadorMensajes.PublicarMensaje("CDatos::CAfiliacion.ConsTrabajador:Error devuelto por COMDB: " & objConnHost.DescripcionError, EventLogEntryType.Error)
    '            mstrError = objConnHost.DescripcionError
    '            Return False
    '        End If
    '    Catch objException As Exception
    '        ManejoMensajes.CManejadorMensajes.PublicarMensaje(objException.ToString, EventLogEntryType.Error)
    '        mstrError = objException.ToString
    '        Return False
    '    End Try
    '    Return True
    'End Function
    ' ''' <summary>
    ' ''' 
    ' ''' </summary>
    ' ''' <param name="objdatos"></param>
    ' ''' <param name="pstrXMLDatosRespuesta"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>

    'Public Function ConsultaTrabajadorBasico2(ByRef objdatos As Object, ByRef pstrXMLDatosRespuesta As String) As Boolean
    '    Try
    '        Dim objConnHost As cAF
    '        Dim strXMLResultado As String
    '        If leerConexionBDHost(objConnHost) Then
    '            Dim blnResp As Boolean
    '            Dim strSQL As String
    '            Dim idempresa As String = objdatos(0)
    '            Dim tipodocumento As String = objdatos(1)
    '            Dim documento As String = objdatos(2)
    '            Dim tipodocempresa As String = objdatos(3)
    '            Dim dependencia As String = objdatos(4)
    '            Dim strAlfaNuip As String = objdatos(5)
    '            strSQL = " SELECT TOP 1 AFILDB_EGA01.XNOMTRA,AFILDB_EGA01.XPRIAPE + ' ' + AFILDB_EGA01.XSEGAPE as Apellidos,AFILDB_EGA01.TIDETRA,AFILDB_EGA01.NIDETRA,AFILDB_EGA02.XNOMBEN,AFILDB_EGA02.XPRIAPE,AFILDB_EGA02.XSEGAPE,AFILDB_EGA02.CPRN,AFILDB_EGA02.NDOCBEN,AFILDB_EGA02.CTIPDOC,AFILDB_EGA01.CESTPOS, AFILDB_EGA01.CESTPCO, (CASE WHEN AFILDB_EGA01.FNACTRA = 0 THEN ' ' ELSE SUBSTRING(CONVERT(VARCHAR,AFILDB_EGA01.FNACTRA),7,2) + '/' +  SUBSTRING(CONVERT(VARCHAR,AFILDB_EGA01.FNACTRA),5,2) + '/' + SUBSTRING(CONVERT(VARCHAR,AFILDB_EGA01.FNACTRA),1,4) END) AS FNACTRA, AFILDB_EGA01.XSEXTRA, AFILDB_EGA01.CESTSOC, AFILDB_EGA01.CPRG, AFILDB_EGA01.FRETPOS FROM AFILDB_EGA01,AFILDB_EGA02 WHERE AFILDB_EGA02.TIDETRA=AFILDB_EGA01.TIDETRA AND AFILDB_EGA02.NIDETRA=AFILDB_EGA01.NIDETRA " & _
    '               " AND AFILDB_EGA02.NDOCBEN=" & documento & " AND AFILDB_EGA02.CTIPDOC=" & tipodocumento & " AND NOT (AFILDB_EGA02.CESTPOS = 3 AND AFILDB_EGA02.CESTPCO = 3)"
    '            If strAlfaNuip <> "" Then
    '                strSQL = strSQL & " AND AFILDB_EGA02.CTIPNUI = '" & strAlfaNuip & "'"
    '            ElseIf tipodocumento = "8" Then
    '                strSQL = strSQL & " AND AFILDB_EGA02.CTIPNUI = ''"
    '            End If
    '            If dependencia <> "" Then
    '                strSQL = strSQL & " AND AFILDB_EGA01.CDEP=" & dependencia
    '            End If
    '            If idempresa <> "" Then
    '                strSQL = strSQL & " And AFILDB_EGA01.TIDEEMP = " & tipodocempresa & " And AFILDB_EGA01.NIDEEMP = " & idempresa
    '            End If
    '            strSQL = strSQL & " ORDER BY  AFILDB_EGA02.CESTPOS, AFILDB_EGA02.CESTPCO"
    '            If objConnHost.Query(strSQL, strXMLResultado) Then
    '                pstrXMLDatosRespuesta = strXMLResultado
    '            Else
    '                ManejoMensajes.CManejadorMensajes.PublicarMensaje("CDatos::CConsultas.ConsultaTrabajadorBasico:Error devuelto por COMDB: " & objConnHost.DescripcionError, EventLogEntryType.Error)
    '                mstrError = objConnHost.DescripcionError
    '                Return False
    '            End If
    '        Else
    '            ManejoMensajes.CManejadorMensajes.PublicarMensaje("CDatos::CConsultas.ConsultaTrabajadorBasico:Error devuelto por COMDB: " & objConnHost.DescripcionError, EventLogEntryType.Error)
    '            mstrError = objConnHost.DescripcionError
    '            Return False
    '        End If
    '    Catch objException As Exception
    '        ManejoMensajes.CManejadorMensajes.PublicarMensaje(objException.ToString, EventLogEntryType.Error)
    '        mstrError = objException.ToString
    '        Return False
    '    End Try
    '    Return True
    'End Function
#End Region
#Region "Miembros Privados "
    Friend mFormatter As DateTimeFormatInfo
    Friend mstrError As String
#End Region

#Region " Constructores "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Se encarga de llamar al constructor de la super clase
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub New()
        MyBase.New()
    End Sub
#End Region
#Region "Propiedades "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Cadena que almacena la descripción del error ocurrido en cualquier momento, dado
    ''' que alguno de los métodos pertenecientes a la clase arroje FALSE luego de la 
    ''' ejecución de alguno de ellos 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property DescripcionError() As String
        Get
            Return mstrError
        End Get
        Set(ByVal pstrValue As String)
            mstrError = pstrValue
        End Set
    End Property
#End Region
#Region "Helper Methods"
    'WLM 24/09/2008: El proceso de cambio de estado SISPOS define una logica basada en requisitos, si este requisito esta asociado a una operacion vinculacion
    'Se usa este metodo el cual es el mismo expuesto en WSCOMCLIENTES.Afiliaciones:
    Private Function Vincular(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim returnvalue As String = ""
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim i As Integer
                    With ObjVin
                        .Aplicacion = Aplicacion
                        .Operacion = ds.Tables(0).Rows(0)("Operacion").ToString()
                        .Identity = ds.Tables(0).Rows(0)("Identity").ToString()
                        .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        .Nombre = ds.Tables(0).Rows(0)("Nombre").ToString()
                        .PrimerApellido = ds.Tables(0).Rows(0)("PrimerApellido").ToString()
                        .SegundoApellido = ds.Tables(0).Rows(0)("SegundoApellido").ToString()
                        .Fechanacimiento = ds.Tables(0).Rows(0)("Fechanacimiento").ToString()
                        .Genero = ds.Tables(0).Rows(0)("Genero").ToString()
                        .EstadoCivil = ds.Tables(0).Rows(0)("EstadoCivil").ToString()
                        .Direccion = ds.Tables(0).Rows(0)("Direccion").ToString()
                        .Telefono = ds.Tables(0).Rows(0)("Telefono").ToString()
                        .Extension = ds.Tables(0).Rows(0)("Extension").ToString()
                        .TipoDireccion = ds.Tables(0).Rows(0)("TipoDireccion").ToString()
                        .Barrio = ds.Tables(0).Rows(0)("Barrio").ToString()
                        .Zona = ds.Tables(0).Rows(0)("Zona").ToString()
                        .Ciudad = ds.Tables(0).Rows(0)("Ciudad").ToString()
                        .Detalleadicional = ds.Tables(0).Rows(0)("Detalleadicional").ToString()
                        .Razonsocial = ds.Tables(0).Rows(0)("Razonsocial").ToString()
                        .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                        .GrupoPrograma = ds.Tables(0).Rows(0)("GrupoPrograma").ToString()
                        .Programa = ds.Tables(0).Rows(0)("Programa").ToString()
                        .Condicion = ds.Tables(0).Rows(0)("Condicion").ToString()
                        For i = 0 To 11
                            .AccReq(i) = ds.Tables(0).Rows(0)("Accion" & CStr(i + 1)).ToString()  'Accion
                            .NameReq(i) = ds.Tables(0).Rows(0)("Requisito" & CStr(i + 1)).ToString()  'Nemonico
                            .FechIniReq(i) = ds.Tables(0).Rows(0)("FechaInicialVigencia" & CStr(i + 1)).ToString() 'Fecha Inicial de vigencia
                            .FechFinReq(i) = ds.Tables(0).Rows(0)("FechaFinalVigencia" & CStr(i + 1)).ToString()  'Fecha Final de vigencia
                            .DatNumReq(i) = ds.Tables(0).Rows(0)("DatoNumerico" & CStr(i + 1)).ToString()  'Dato numerico
                            .DatAlfReq(i) = ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString()  'Dato Alfabetico
                        Next
                        .TipoIdentificacion1 = ds.Tables(0).Rows(0)("TipoIdentificacionTrabajadorResponsable").ToString()
                        .Responsable1 = ds.Tables(0).Rows(0)("TrabajadorResponsable").ToString()
                        .TipoIdentificacion2 = ds.Tables(0).Rows(0)("TipoIdentificacionEmpresaResponsable").ToString()
                        .Responsable2 = ds.Tables(0).Rows(0)("EmpresaResponsable").ToString()
                        .Responsable3 = ds.Tables(0).Rows(0)("SucursalEmpresaResponsable").ToString()
                        .Responsable4 = ds.Tables(0).Rows(0)("CentroCostoEmpresaResponsable").ToString()
                        .FechaInicioAfiliacion = ds.Tables(0).Rows(0)("FechaInicioAfiliacion").ToString()
                        .FechaFinalAfiliacion = ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString()
                        .FechaRetiro = ds.Tables(0).Rows(0)("FechaFinalAfiliacion").ToString()
                        .CausalRetiro = ds.Tables(0).Rows(0)("CausalRetiro").ToString()
                        .FechaIngresoEmpresa = ds.Tables(0).Rows(0)("FechaIngresoEmpresa").ToString()
                        .Valor = ds.Tables(0).Rows(0)("Valor").ToString()
                        .Cantidad = ds.Tables(0).Rows(0)("Cantidad").ToString()
                        .Cargo = ds.Tables(0).Rows(0)("Cargo").ToString()
                        .NumeroDireccion = ds.Tables(0).Rows(0)("NumeroDireccion").ToString()
                        .InfoAdicional = ds.Tables(0).Rows(0)("InfoAdicional").ToString()
                    End With
                    returnvalue = ObjVin.Vinculacion()
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function
    Private Function RadicarCambioEstado(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim returnvalue As String = ""
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)
            Dim dstCausales As DataSet
            Dim dsAfiliacionCotizante As New DataSet
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    'Dim i As Integer
                    With ObjRad
                        .Aplicacion = Aplicacion
                        .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        If Not ds.Tables(0).Rows(0)("Partealfabetica") Is Nothing Then
                            .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        End If
                        If Not ds.Tables(0).Rows(0)("Usuario") Is Nothing Then
                            .Usuario = Utilidades.CUtil.strEvalObject(ds.Tables(0).Rows(0)("Usuario"))
                        End If
                        '// wlm:sep 09 2009:control de cambios:unificacion radicacion cambio estado.
                        Try
                            .Estado = CStr(ds.Tables(0).Rows(0)("Estado"))
                        Catch ex As Exception
                        End Try
                        Try
                            .IndOption = CStr(ds.Tables(0).Rows(0)("IndOption"))
                        Catch ex As Exception
                        End Try
                        '//solo si es retiro:
                        If .IndOption = "S" Then
                            .FechaRetiro = CStr(ds.Tables(0).Rows(0)("FechaRetiro"))
                            .FechaFinalAfiliacion = CStr(ds.Tables(0).Rows(0)("FechaFinalAfiliacion"))
                        End If
                        If .Estado = "1" And .IndOption = "V" Then
                            If Not ds.Tables(0).Rows(0)("FechaRetiro") Is Nothing Then
                                .FechaRetiro = CStr(ds.Tables(0).Rows(0)("FechaRetiro"))
                            End If
                            If Not ds.Tables(0).Rows(0)("FechaFinalAfiliacion") Is Nothing Then
                                .FechaFinalAfiliacion = CStr(ds.Tables(0).Rows(0)("FechaFinalAfiliacion"))
                            End If
                            If Not ds.Tables(0).Rows(0)("CausalRetiro") Is Nothing Then
                                .CausalRetiro = CStr(ds.Tables(0).Rows(0)("CausalRetiro"))

                            End If
                        End If

                        If Not ds.Tables(0).Rows(0)("NroIdentEmpr") Is Nothing Then
                            .NroIdentEmpr = CStr(ds.Tables(0).Rows(0)("NroIdentEmpr"))

                        End If
                        If Not ds.Tables(0).Rows(0)("TipoIdentEmpr") Is Nothing Then
                            .TipoIdentEmpr = CStr(ds.Tables(0).Rows(0)("TipoIdentEmpr"))
                        End If

                        If Not ds.Tables(0).Rows(0)("Programa") Is Nothing Then
                            .Programa = ds.Tables(0).Rows(0)("Programa").ToString()
                        End If
                        If Not ds.Tables(0).Rows(0)("Condicion") Is Nothing Then
                            .Condicion = ds.Tables(0).Rows(0)("Condicion").ToString()
                        End If
                        If Not ds.Tables(0).Rows(0)("Sucursal") Is Nothing Then
                            .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        End If
                        If Not ds.Tables(0).Rows(0)("CentroCosto") Is Nothing Then
                            .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        End If
                        .AccReq(0) = Utilidades.CUtil.strEvalObject(ds.Tables(0).Rows(0)("Accion1"))  'Accion
                        .NameReq(0) = Utilidades.CUtil.strEvalObject(ds.Tables(0).Rows(0)("Requisito1"))  'Nemonico
                        .FechIniReq(0) = Utilidades.CUtil.strEvalObject(ds.Tables(0).Rows(0)("FechaInicialVigencia1")) 'Fecha Inicial de vigencia
                        .FechFinReq(0) = Utilidades.CUtil.strEvalObject(ds.Tables(0).Rows(0)("FechaFinalVigencia1"))  'Fecha Final de vigencia
                        .DatNumReq(0) = Utilidades.CUtil.strEvalObject(ds.Tables(0).Rows(0)("DatoNumerico1"))  'Dato numerico
                        .DatAlfReq(0) = Utilidades.CUtil.strEvalObject(ds.Tables(0).Rows(0)("DatoAlfabetico1"))  'Dato Alfabetico
                    End With
                    If Not ObjRad.NameReq(0).Equals("") Then
                        If ObjRad.Estado = 1 And ObjRad.IndOption = "V" And _
                                                (Not ObjRad.NameReq(0).Equals("TAAAUT") And Not ObjRad.NameReq(0).Equals("TDANEG")) Then
                            returnvalue = Parametros
                        ElseIf ObjRad.NameReq(0).Equals("PLABOR") And ObjRad.Usuario <> "999999999999" Then
                            'Determina Tipo de Identificacion
                            Dim tipoId As String = Utilidades.CUtil.darCodigoTipoId(ds.Tables(0).Rows(0)("TipoIdent").ToString())

                            'Consulta el Nautcli a procesar
                            Dim nautcli As Decimal
                            nautcli = consultarNautcli(Aplicacion, ds.Tables(0).Rows(0)("NroIdentificacion").ToString(), tipoId)

                            dstCausales = ESL.Vinculacion.ConsultaAF.dstConsultaEga18(Aplicacion, "CPROLAB")
                            dsAfiliacionCotizante = ESL.Vinculacion.ConsultaAF.ConsultaVinculacionesTr(Aplicacion, nautcli)

                            Dim strCCAURET As String = dsAfiliacionCotizante.Tables(0).Rows(0)("CCAURET").ToString
                            Dim strNAUTR02 As String = dsAfiliacionCotizante.Tables(0).Rows(0)("NAUTR02").ToString

                            'MANTIS 7600 VALIDA SOBRE LAS CAUSALES QUE IMPIDEN LA PROTECCION LABORAL
                            For Each drC As DataRow In dstCausales.Tables(0).Rows
                                'SELECT XDES,XDATTAB,CDATTAB FROM AFILDB_EGA18 WHERE XDATTAB IN('CRETBEN' , 'CRETCOT')
                                If strCCAURET = drC("CDATTAB") Then 'Causales 5,8,9,12,16 y 23
                                    returnvalue = Parametros
                                End If
                            Next
                            oAplicacion = New Helper.coleccion.clsParametros()
                            Dim periodos As Integer = 0
                            Dim oComadmoPos As New DAL.Comadmo(oAplicacion.codigoComadmo)

                            'Valida los Periodos de Proteccion Laboral
                            periodos = oComadmoPos.consultaHistoricoVin(nautcli, strNAUTR02, strCCAURET, Utilidades.CUtil.strEvalObject(ds.Tables(0).Rows(0)("FechaInicialVigencia1")))

                            If periodos > 0 Then
                                returnvalue = ObjRad.Radicacion()
                            Else
                                returnvalue = Parametros
                            End If
                        Else
                            returnvalue = ObjRad.Radicacion() '12140
                        End If
                    Else
                        returnvalue = Parametros
                    End If
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function
    Private Function Radicar(ByVal Aplicacion As String, ByVal Parametros As String) As String
        Dim returnvalue As String = ""
        Try
            Dim ds As DataSet = ObjUtil.GetDataSet(Parametros)
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim i As Integer
                    With ObjRad
                        .Aplicacion = Aplicacion
                        .TipoIdent = ds.Tables(0).Rows(0)("TipoIdent").ToString()
                        .NroIdentificacion = ds.Tables(0).Rows(0)("NroIdentificacion").ToString()
                        .Partealfabetica = ds.Tables(0).Rows(0)("Partealfabetica").ToString()
                        .DigitoChequeo = ds.Tables(0).Rows(0)("DigitoChequeo").ToString()
                        .Sucursal = ds.Tables(0).Rows(0)("Sucursal").ToString()
                        .CentroCosto = ds.Tables(0).Rows(0)("CentroCosto").ToString()
                        .Nombre = ds.Tables(0).Rows(0)("Nombre").ToString()
                        .PrimerApellido = ds.Tables(0).Rows(0)("PrimerApellido").ToString()
                        .SegundoApellido = ds.Tables(0).Rows(0)("SegundoApellido").ToString()
                        .Fechanacimiento = ds.Tables(0).Rows(0)("Fechanacimiento").ToString()
                        .Genero = ds.Tables(0).Rows(0)("Genero").ToString()
                        .EstadoCivil = ds.Tables(0).Rows(0)("EstadoCivil").ToString()
                        .Detalleadicional = ds.Tables(0).Rows(0)("Detalleadicional").ToString()
                        .Razonsocial = ds.Tables(0).Rows(0)("Razonsocial").ToString()
                        .Usuario = ds.Tables(0).Rows(0)("Usuario").ToString()
                        .Programa = ds.Tables(0).Rows(0)("Programa").ToString()
                        .Condicion = ds.Tables(0).Rows(0)("Condicion").ToString()
                        For i = 0 To 11
                            .AccReq(i) = ds.Tables(0).Rows(0)("Accion" & CStr(i + 1)).ToString()  'Accion
                            .NameReq(i) = ds.Tables(0).Rows(0)("Requisito" & CStr(i + 1)).ToString()  'Nemonico
                            .FechIniReq(i) = ds.Tables(0).Rows(0)("FechaInicialVigencia" & CStr(i + 1)).ToString() 'Fecha Inicial de vigencia
                            .FechFinReq(i) = ds.Tables(0).Rows(0)("FechaFinalVigencia" & CStr(i + 1)).ToString()  'Fecha Final de vigencia
                            .DatNumReq(i) = ds.Tables(0).Rows(0)("DatoNumerico" & CStr(i + 1)).ToString()  'Dato numerico
                            .DatAlfReq(i) = ds.Tables(0).Rows(0)("DatoAlfabetico" & CStr(i + 1)).ToString()  'Dato Alfabetico
                        Next
                        .InfoAdicional = ds.Tables(0).Rows(0)("InfoAdicional").ToString()
                    End With
                    returnvalue = ObjRad.Radicacion()
                End If
            End If
        Catch ex As Exception
            returnvalue = ObjUtil.ConvertToXMLdoc(ex.Message)
        End Try
        Return returnvalue
    End Function
    Function consultarNautcli(ByVal codAplicacion As String, ByVal strIdentificacion As String, ByVal strTipoIdent As String) As String
        'MODIFICADO LCARDENAS 31-01-2012: SOLUCION INCIDENCIA 7402 LLEGA CON CERO CUANDO HAY ERROR REGISTRO FID INMODIFICABLE
        Dim objLogica As New Compensar.Vincula.POS.LogicaEstados
        Return objLogica.consultarNautcli(codAplicacion, strIdentificacion, strTipoIdent)
    End Function
#End Region



End Class




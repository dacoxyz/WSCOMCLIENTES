Imports Microsoft.VisualBasic
Imports System.Xml
Imports EntidadesNegocio

Imports Compensar.SISPOS.ESL
Imports System.Configuration
Imports ManejoMensajes
Imports System.Text.RegularExpressions
Imports Compensar.SISPOS.ESL.Vinculacion '4855 20150504

Public Class ClsVinculacion
    Inherits ClsCliente

#Region "Declaraciones"
    Dim ObjUtil As New Utilidades.CUtil
    Dim ObjTran As New Servicios.ClsServicio
    Dim ObjRegistro As New ClsRegistro
    Dim cle15 As New ObjTransferDatos.DtsCLE15
    Dim m_Programa As String
    Dim m_Condicion As String
    Dim m_Usuario As String
    Dim m_IdEps As String
    Dim m_Aplicacion As String
    Dim m_Operacion As String
    Dim m_GrupoPrograma As String
    Dim m_CondicionEspecial As String '1710 20130430
#End Region

#Region "Propiedades"
    Property Usuario() As String
        Get
            Return m_Usuario
        End Get
        Set(ByVal value As String)
            m_Usuario = value
        End Set
    End Property

    Property IdEps() As String
        Get
            Return m_IdEps
        End Get
        Set(ByVal value As String)
            m_IdEps = value
        End Set
    End Property

    Property Programa() As String
        Get
            Return m_Programa
        End Get
        Set(ByVal value As String)
            m_Programa = value
        End Set
    End Property

    Property Condicion() As String
        Get
            Return m_Condicion
        End Get
        Set(ByVal value As String)
            m_Condicion = value
        End Set
    End Property

    Property Operacion() As String
        Get
            Return m_Operacion
        End Get
        Set(ByVal value As String)
            m_Operacion = value
        End Set
    End Property

    Property Aplicacion() As String
        Get
            Return m_Aplicacion
        End Get
        Set(ByVal value As String)
            m_Aplicacion = value
        End Set
    End Property

    Property GrupoPrograma() As String
        Get
            Return m_GrupoPrograma
        End Get
        Set(ByVal value As String)
            m_GrupoPrograma = value
        End Set
    End Property
    '1710 20130430
    Property CondicionEspecial() As String
        Get
            Return m_CondicionEspecial
        End Get
        Set(ByVal value As String)
            m_CondicionEspecial = value
        End Set
    End Property

#End Region

#Region "Metodos"

    Public Function Vinculacion() As String
        Dim returnvalue As String = ""
        Dim i As Integer
        Dim dr As Data.DataRow
        Dim pattern As String = "[^A-Za-z0-9| |Ñ|n]"
        Dim pattern2 As String = "[^A-Za-z0-9| |Ñ|n|@|.]" '0010445
        Dim rgx As New Regex(pattern)
        Dim rgx2 As New Regex(pattern2)                 '0010445
        'rgx.Replace("", pattern)
        dr = cle15.Tables(0).NewRow

        Dim XmlResultadoTemp As String = ""  '2702

        Try  '2702
            With cle15.Tables(0)
                dr("_TOP_LINE_") = "CLE15T" & DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")
                dr("I_MAINT") = m_Operacion
                dr("I_CALT") = m_Usuario
                dr("I_NAUTCLI") = Identity
                If m_Usuario = "999999999999" Then
                    dr("I_XINDOPC") = "F"
                End If
                If TipoIdent IsNot Nothing Then
                    dr("I_XIDECLI") = TipoIdent.ToUpper
                End If
                dr("I_NIDECLI") = NroIdentificacion
                dr("I_XNUICLI") = Partealfabetica
                If DigitoChequeo IsNot Nothing Then
                    dr("I_NDIGCHE") = ObjUtil.strAsignarValor(DigitoChequeo.ToString, "0")
                End If
                dr("I_NIDESUC") = ObjUtil.strAsignarValor(Sucursal, "0")
                dr("I_NIDECOS") = ObjUtil.strAsignarValor(CentroCosto, "0")
                If Razonsocial IsNot Nothing Then
                    dr("I_XDESRAZ") = rgx.Replace(Razonsocial.ToUpper, "").ToString
                End If
                If PrimerApellido IsNot Nothing Then
                    dr("I_XPRIAPE") = PrimerApellido.ToUpper
                End If
                If SegundoApellido IsNot Nothing Then
                    dr("I_XSEGAPE") = SegundoApellido.ToUpper
                End If
                If Nombre IsNot Nothing Then
                    dr("I_XNOMTRAB") = rgx.Replace(Nombre.ToUpper, "").ToString

                    '004249 identifica 
                    If Nombre.Length > 30 Then
                        Dim iPos As Integer
                        iPos = Nombre.IndexOf(" ")
                        If iPos > 1 Then
                            dr("I_XDESRAZ") = LTrim(Nombre.Substring(iPos)).ToUpper
                            dr("I_XNOMTRAB") = Left(Nombre, iPos)
                            Nombre = dr("I_XNOMTRAB")
                            Razonsocial = dr("I_XDESRAZ")
                        End If
                    End If
                    ''004249

                End If
                dr("I_FNACCLI") = ObjUtil.strAsignarValor(Fechanacimiento, "0")
                dr("I_XGENCLI") = Genero
                dr("I_CECICLI") = EstadoCivil
                If Detalleadicional IsNot Nothing Then
                    dr("I_XNIVADI") = IIf(Detalleadicional <> "", Detalleadicional, "N")
                End If
                If TipoDireccion IsNot Nothing Then
                    dr("I_XTIPDIR") = ObjUtil.strAsignarValor(TipoDireccion.ToUpper, "U")
                End If
                If Direccion IsNot Nothing Then
                    dr("I_XDIRCLI") = rgx.Replace(Direccion, "").ToString
                Else
                    dr("I_XDIRCLI") = Direccion
                End If
                If Barrio IsNot Nothing Then
                    If Barrio.ToString.Length > 18 Then
                        dr("I_XBARCLI") = rgx.Replace(Barrio, "").ToString.Substring(0, 18)
                    Else
                        dr("I_XBARCLI") = rgx.Replace(Barrio, "").ToString
                    End If
                End If
                dr("I_NTELCLI") = ObjUtil.strAsignarValor(Telefono, "0")
                dr("I_NEXTCLI") = ObjUtil.strAsignarValor(Extension, "0")
                dr("I_CCIUCLI") = ObjUtil.strAsignarValor(Ciudad, "0")
                dr("I_CZONBTA") = IIf(ObjUtil.strAsignarValor(Zona, "0") <> "0", Zona, "999") '
                dr("I_CPRGSRV") = m_Programa
                dr("I_NCONAFI") = ObjUtil.strAsignarValor(m_Condicion, "0")
                For i = 0 To 11
                    dr("I_XACC" & CStr(i + 1)) = AccReq(i)
                    dr("I_XNMNCA" & CStr(i + 1)) = NameReq(i)
                    dr("I_FINIVI" & CStr(i + 1)) = ObjUtil.strAsignarValor(FechIniReq(i), "0")
                    dr("I_FFINVI" & CStr(i + 1)) = ObjUtil.strAsignarValor(FechFinReq(i), "0")
                    dr("I_VNUMDA" & CStr(i + 1)) = ObjUtil.strAsignarValor(DatNumReq(i), "0")
                    dr("I_XALFDA" & CStr(i + 1)) = DatAlfReq(i)
                Next
                dr("I_XESCVIN") = ObjUtil.strAsignarValor(Escala, "0")
                dr("I_CESTVIN") = ObjUtil.strAsignarValor(Estado, "0")
                dr("I_TRES001") = ObjUtil.strAsignarValor(TipoIdentificacion1, "0")
                dr("I_NRES001") = ObjUtil.strAsignarValor(Responsable1, "0")
                dr("I_TRES002") = ObjUtil.strAsignarValor(TipoIdentificacion2, "0")
                dr("I_NRES002") = ObjUtil.strAsignarValor(Responsable2, "0")
                dr("I_NRES003") = ObjUtil.strAsignarValor(Responsable3, "0")
                dr("I_NRES004") = ObjUtil.strAsignarValor(Responsable4, "0")
                dr("I_NDIRVIN") = ObjUtil.strAsignarValor(NumeroDireccion, "0")
                dr("I_FINIAFI") = ObjUtil.strAsignarValor(FechaInicioAfiliacion, "0")
                dr("I_FFINAFI") = ObjUtil.strAsignarValor(FechaFinalAfiliacion, "0")
                dr("I_FRET") = ObjUtil.strAsignarValor(FechaRetiro, "0")
                dr("I_CRETAFI") = ObjUtil.strAsignarValor(CausalRetiro, "0")
                dr("I_FVIN") = ObjUtil.strAsignarValor(FechaPrimerAporte, "0")
                'MODIFICADO LCARDENAS 12-10-2011: FECHA EN FORMATO INCORRECTO PARA SER ENVIADA AL METODO ISPEC
                Try
                    dr("I_FINGEMP") = CType(Convert.ToDateTime(FechaIngresoEmpresa, System.Globalization.DateTimeFormatInfo.CurrentInfo).ToString("yyyyMMdd"), String)
                Catch ex As Exception
                    dr("I_FINGEMP") = ObjUtil.strAsignarValor(FechaIngresoEmpresa, "0")
                End Try
                dr("I_QVIN") = ObjUtil.strAsignarValor(Cantidad, "0")
                dr("I_XVIN") = ObjUtil.strAsignarValor(Cargo, "0")
                dr("I_VVIN") = ObjUtil.strAsignarValor(Valor, "0")
                If InfoAdicional IsNot Nothing Then
                    dr("I_XALFADI") = rgx2.Replace(InfoAdicional.ToString, "").ToString
                Else
                    dr("I_XALFADI") = InfoAdicional
                End If
                '1710 20130430
                Try
                    If Not CondicionEspecial Is Nothing Then
                        dr("C_ESPECIAL") = CondicionEspecial
                    End If
                Catch ex As Exception

                End Try
                'dr.c()
                If m_Operacion = "ADD" And m_GrupoPrograma = "EP" And dr("I_FVIN") > DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") Then '7667 20150618
                    Dim dsAfiliacionCotizante As New DataSet
                    dsAfiliacionCotizante = ConsultaAF.ConsultaVinculacionesTr("WSPR04", Identity)
                    Dim drTmp() As DataRow
                    If dsAfiliacionCotizante.Tables.Count >= 1 Then
                        If dsAfiliacionCotizante.Tables(0).Rows.Count >= 0 Then
                            drTmp = dsAfiliacionCotizante.Tables(0).Select("CESTVIN<>1 AND XPRG='RS'")
                            If drTmp.Length > 0 Then
                                dr("I_CESTVIN") = "3"
                            End If
                        End If
                    End If
                End If
            End With
            cle15.Tables(0).Rows.Add(dr)
            XmlResultadoTemp = cle15.GetXml()
            CManejadorMensajes.PublicarMensaje("ClsVinculacion:Vinculacion1" & cle15.GetXml(), EventLogEntryType.SuccessAudit)
            With ObjTran
                .DtsInfoTran = cle15
                Dim xmlDoc As XmlDocument
                Dim xmlnodo As XmlNode
                Dim XmlResultado As String = ""  '2702
                'm_GrupoPrograma = IIf(m_GrupoPrograma = "", "CJ", m_GrupoPrograma) 'No existe el programa, se forza a guardar la excepcion
                Select Case m_GrupoPrograma
                    'Case "CJ" 'Caja
                    '    Dim ObjVinculacionCaja As New Compensar.Vincula.CAJA.ClsLogicaVinculacionCaja
                    '    'XmlResultado = ObjVinculacionCaja.FncValidarAfiliacionCaja(cle15.GetXml, m_Aplicacion, ConfigurationManager.AppSettings("ProjectID"), "", m_Usuario, "0")
                    '    XmlResultado = ObjVinculacionCaja.FncValidarAfiliacionCaja(cle15.GetXml, m_Aplicacion, "SWPR87_VS8", "", m_Usuario, "0")
                    '    ObjVinculacionCaja.Dispose()
                    Case "EP" 'Pos
                        Dim objVincula As New Compensar.Vincula.POS.VinculacionPOS
                        XmlResultado = objVincula.VincularPOS(cle15.GetXml)
                        cle15.Clear()
                    Case "RS" 'Regimen Subsidiado                                       '4855 20140808 
                        Dim objVincula As New Compensar.Vincula.POS.clsVinculacionRS    '4855 20140808 
                        XmlResultado = objVincula.VincularRS(cle15.GetXml)              '4855 20140808 
                        cle15.Clear()                                                   '4855 20140808 
                    Case Else
                        XmlResultado = cle15.GetXml
                        cle15.Clear()
                End Select
                'CManejadorMensajes.PublicarMensaje("ClsVinculacion:Vinculacion2" & cle15.GetXml() & " - " & XmlResultado, EventLogEntryType.SuccessAudit)
                xmlDoc = New XmlDocument()
                'xmlDoc.LoadXml(XmlResultado)
                'xmlnodo = xmlDoc.SelectSingleNode("//Mensaje")
                Dim valores As String()
                Dim intContar As Integer
                Dim intContador As Integer = 0
                'XmlResultado = "<DtsCLE15 xmlns=""http://tempuri.org/DtsCLE15.xsd""><CLE15><_TOP_LINE_>CLE15T2009/08/27 09:47:11</_TOP_LINE_><I_MAINT>CHG</I_MAINT><I_CSED>0</I_CSED><I_FVALCLI>0</I_FVALCLI><I_NAUTCLI>6211815167020081</I_NAUTCLI><I_NRADDOC>0</I_NRADDOC><I_CALT>91474978</I_CALT><I_XIDECLI>CC</I_XIDECLI><I_NIDECLI>80093044</I_NIDECLI><I_XNUICLI /><I_NDIGCHE>0</I_NDIGCHE><I_NIDESUC>0</I_NIDESUC><I_NIDECOS>0</I_NIDECOS><I_XGENCLI>M</I_XGENCLI><I_FNACCLI>19811002</I_FNACCLI><I_CECICLI>SO</I_CECICLI><I_XPRIAPE>ZARATE</I_XPRIAPE><I_XSEGAPE>PENA</I_XSEGAPE><I_XNOMTRAB>HENRY ALAN</I_XNOMTRAB><I_XNIVADI>N</I_XNIVADI><I_XDESRAZ /><I_NEDA>0</I_NEDA><I_CPRGSRV>01021</I_CPRGSRV><I_NCONAFI>2</I_NCONAFI><I_XTIPDIR>U</I_XTIPDIR><I_CGRUCAM>0</I_CGRUCAM><I_NDIR>1</I_NDIR><I_XDIRCLI>TV 57A N 104 67</I_XDIRCLI><I_NTELCLI>4178444</I_NTELCLI><I_NEXTCLI>0</I_NEXTCLI><I_CZONBTA>999</I_CZONBTA><I_CCIUCLI>11001</I_CCIUCLI><I_XACC1 /><I_XNMNCA1>CODOCU</I_XNMNCA1><I_FINIVI1>0</I_FINIVI1><I_FFINVI1>0</I_FFINVI1><I_VNUMDA1>0</I_VNUMDA1><I_XALFDA1 /><I_XACC2 /><I_XNMNCA2>CUEBAN</I_XNMNCA2><I_FINIVI2>0</I_FINIVI2><I_FFINVI2>0</I_FFINVI2><I_VNUMDA2>0</I_VNUMDA2><I_XALFDA2>A0</I_XALFDA2><I_XACC3 /><I_XNMNCA3>SERVIC</I_XNMNCA3><I_FINIVI3>0</I_FINIVI3><I_FFINVI3>0</I_FFINVI3><I_VNUMDA3>0</I_VNUMDA3><I_XALFDA3 /><I_XACC4 /><I_XNMNCA4>OTREMP</I_XNMNCA4><I_FINIVI4>0</I_FINIVI4><I_FFINVI4>0</I_FFINVI4><I_VNUMDA4>0</I_VNUMDA4><I_XALFDA4 /><I_XACC5 /><I_XNMNCA5>SCUATR</I_XNMNCA5><I_FINIVI5>0</I_FINIVI5><I_FFINVI5>0</I_FFINVI5><I_VNUMDA5>0</I_VNUMDA5><I_XALFDA5>1</I_XALFDA5><I_XACC6 /><I_XNMNCA6>SSEISS</I_XNMNCA6><I_FINIVI6>0</I_FINIVI6><I_FFINVI6>0</I_FFINVI6><I_VNUMDA6>0</I_VNUMDA6><I_XALFDA6 /><I_XACC7 /><I_XNMNCA7 /><I_FINIVI7>0</I_FINIVI7><I_FFINVI7>0</I_FFINVI7><I_VNUMDA7>0</I_VNUMDA7><I_XALFDA7 /><I_XACC8 /><I_XNMNCA8 /><I_FINIVI8>0</I_FINIVI8><I_FFINVI8>0</I_FFINVI8><I_VNUMDA8>0</I_VNUMDA8><I_XALFDA8 /><I_XACC9 /><I_XNMNCA9 /><I_FINIVI9>0</I_FINIVI9><I_FFINVI9>0</I_FFINVI9><I_VNUMDA9>0</I_VNUMDA9><I_XALFDA9 /><I_XACC10 /><I_XNMNCA10 /><I_FINIVI10>0</I_FINIVI10><I_FFINVI10>0</I_FFINVI10><I_VNUMDA10>0</I_VNUMDA10><I_XALFDA10 /><I_XACC11 /><I_XNMNCA11 /><I_FINIVI11>0</I_FINIVI11><I_FFINVI11>0</I_FFINVI11><I_VNUMDA11>0</I_VNUMDA11><I_XALFDA11 /><I_XACC12 /><I_XNMNCA12 /><I_FINIVI12>0</I_FINIVI12><I_FFINVI12>0</I_FFINVI12><I_VNUMDA12>0</I_VNUMDA12><I_XALFDA12 /><I_NIDENUE>0</I_NIDENUE><I_NSUCNUE>0</I_NSUCNUE><I_NCOSNUE>0</I_NCOSNUE><I_XBARCLI>NO REPORTA</I_XBARCLI><I_TRES001>0</I_TRES001><I_NRES001>0</I_NRES001><I_TRES002>NI</I_TRES002><I_NRES002>860039561</I_NRES002><I_NRES003>0</I_NRES003><I_NRES004>0</I_NRES004><I_VVIN>4584800</I_VVIN><I_XESCVIN>C</I_XESCVIN><I_CESTVIN>1</I_CESTVIN><I_NDIRVIN>0</I_NDIRVIN><I_FINIAFI>20040216</I_FINIAFI><I_FFINAFI>20090827</I_FFINAFI><I_CRETAFI>20</I_CRETAFI><I_FVIN>20040216</I_FVIN><I_QVIN>240</I_QVIN><I_XVIN>ADMINISTRATIVO</I_XVIN><I_FRET>20090827</I_FRET><I_XALFADI /></CLE15></DtsCLE15>"
                valores = Split(XmlResultado, ",")
                intContar = valores.Length
                For intContador = 0 To intContar - 1
                    xmlDoc.LoadXml(valores(intContador))
                    xmlnodo = xmlDoc.SelectSingleNode("//Mensaje")
                    '' transacciones
                    '' procesar 1
                    'MsgBox(valores(intContador).ToString)
                    '' si se procesa 1 puede procesarse el 2
                    If (xmlnodo Is Nothing) Then
                        .DtsInfoTran = ObjUtil.GetDataSet(valores(intContador))
                        returnvalue = .EjecutaTransaccion(m_Aplicacion)
                        If InStr(1, returnvalue, "MESSAGE") > 0 Then ' si contiene el tag
                            If Not InStr(1, returnvalue, "INPUT REQUEST") > 0 Then ' si no encuentra error
                                If intContar > 1 Then
                                    Dim xmlDocRta As New XmlDocument
                                    Dim xmlNodoRta As XmlNode
                                    xmlDocRta.LoadXml(returnvalue)
                                    xmlNodoRta = xmlDocRta.SelectSingleNode("//MESSAGE")
                                    Throw New Exception("ERROR: " + xmlNodoRta.InnerText)
                                End If
                            End If
                            '4855 20150504
                            'If m_Operacion = "ADD" And m_GrupoPrograma = "EP" And (Not (XmlResultado.Contains("<I_CESTVIN>3</I_CESTVIN>")) AndAlso returnvalue.Contains("INPUT REQUEST")) Then '4855 20150505
                            '    Dim dsAfiliacionCotizante As New DataSet
                            '    dsAfiliacionCotizante = ConsultaAF.ConsultaVinculacionesTr("WSPR04", Identity)
                            '    Dim drTmp() As DataRow
                            '    If dsAfiliacionCotizante.Tables.Count >= 1 Then
                            '        If dsAfiliacionCotizante.Tables(0).Rows.Count >= 0 Then
                            '            drTmp = dsAfiliacionCotizante.Tables(0).Select("CESTVIN<>1 AND XPRG='RS'")
                            '            If drTmp.Length > 0 Then                                       '4855 20140811 alguna vinculacion activa de RS
                            '                Estado = "3"    'oAfil.cestvin = 3                         '4855 20150504 No afiliado
                            '                m_Operacion = "EST"   'dsXmlEntrada.Tables(0).Rows(0).Item("I_CESTVIN") = "3"  '4855 20150504 No afiliado
                            '                Call Vinculacion()      '4855 20150504
                            '            End If
                            '        End If
                            '    End If

                            'End If
                        Else ' si no contiene el tag
                            'retona vacio
                            Return returnvalue ' = "<CLE15> <CLE15> <_TOP_LINE_>CLE15T00004510JUL30</_TOP_LINE_> <I_MAINT>INQ</I_MAINT> <I_XISP /> <I_XINDOPC /> <I_FVALCLI>20090625</I_FVALCLI> <I_NAUTCLI>6203917652504671</I_NAUTCLI> <I_XACC1 /> <I_XACC2 /> <I_XACC3 /> <I_XACC4 /> <I_XACC5 /> <I_XACC6 /> <I_XACC7 /> <I_XACC8 /> <I_XACC9 /> <I_XACC10 /> <I_XACC11 /> <I_XACC12 /> <I_NEDA>8</I_NEDA> <I_CPRN>HI</I_CPRN> <I_NRADDOC>59100652</I_NRADDOC> <I_CALT>52555323</I_CALT> <I_XIDECLI>NU</I_XIDECLI> <I_NIDECLI>252719</I_NIDECLI> <I_XNUICLI>A5H</I_XNUICLI> <I_NDIGCHE>0</I_NDIGCHE> <I_NIDESUC>0</I_NIDESUC> <I_NIDECOS>0</I_NIDECOS> <I_XGENCLI>M</I_XGENCLI> <I_FNACCLI>20010928</I_FNACCLI> <I_CECICLI>SO</I_CECICLI> <I_CGRUCAM>0</I_CGRUCAM> <I_XNIVADI>N</I_XNIVADI> <I_XPRIAPE>PEÑA</I_XPRIAPE> <I_XSEGAPE>CHAVEZ</I_XSEGAPE> <I_XNOMTRAB>SERGIO ANDRES</I_XNOMTRAB> <I_XVARRAN>DATFBEN</I_XVARRAN> <I_CCAUINA>0</I_CCAUINA> <I_XDESRAZ /> <I_XPRGEQU>EP</I_XPRGEQU> <I_XTIPDIR>U</I_XTIPDIR> <I_FRET>20100701</I_FRET> <I_CPRGSRV>021313</I_CPRGSRV> <I_NCONAFI>3</I_NCONAFI> <I_XNOMPRG>Hijos</I_XNOMPRG> <I_XBARCLI>NO ACTUALIZA</I_XBARCLI> <I_NDIR>1</I_NDIR> <I_XDIRCLI>NO ACTUALIZA</I_XDIRCLI> <I_XNMNCA1>EPSANT</I_XNMNCA1> <I_FINIVI1>0</I_FINIVI1> <I_FFINVI1>0</I_FFINVI1> <I_VNUMDA1>1</I_VNUMDA1> <I_XALFDA1>FECHA INICIO VINCULACIÓN POS</I_XALFDA1> <I_XNMNCA2>AFISSS</I_XNMNCA2> <I_FINIVI2>0</I_FINIVI2> <I_FFINVI2>0</I_FFINVI2> <I_VNUMDA2>1</I_VNUMDA2> <I_XALFDA2>FECHA INICIO DE COBERTURA</I_XALFDA2> <I_XNMNCA3>DISCAP</I_XNMNCA3> <I_FINIVI3>0</I_FINIVI3> <I_FFINVI3>0</I_FFINVI3> <I_VNUMDA3>0</I_VNUMDA3> <I_XALFDA3>REGIONALES EPS008</I_XALFDA3> <I_XNMNCA4 /> <I_FINIVI4>0</I_FINIVI4> <I_FFINVI4>0</I_FFINVI4> <I_VNUMDA4>0</I_VNUMDA4> <I_XALFDA4 /> <I_XNMNCA5 /> <I_FINIVI5>0</I_FINIVI5> <I_FFINVI5>0</I_FFINVI5> <I_VNUMDA5>0</I_VNUMDA5> <I_XALFDA5 /> <I_XNMNCA6 /> <I_FINIVI6>0</I_FINIVI6> <I_FFINVI6>0</I_FFINVI6> <I_VNUMDA6>0</I_VNUMDA6> <I_XALFDA6>CIUDADES EPS</I_XALFDA6> <I_XNMNCA7 /> <I_FINIVI7>0</I_FINIVI7> <I_FFINVI7>0</I_FFINVI7> <I_VNUMDA7>0</I_VNUMDA7> <I_XALFDA7 /> <I_XNMNCA8 /> <I_FINIVI8>0</I_FINIVI8> <I_FFINVI8>0</I_FFINVI8> <I_VNUMDA8>0</I_VNUMDA8> <I_XALFDA8 /> <I_XNMNCA9 /> <I_FINIVI9>0</I_FINIVI9> <I_FFINVI9>0</I_FFINVI9> <I_VNUMDA9>0</I_VNUMDA9> <I_XALFDA9 /> <I_XNMNCA10 /> <I_FINIVI10>0</I_FINIVI10> <I_FFINVI10>0</I_FFINVI10> <I_VNUMDA10>0</I_VNUMDA10> <I_XALFDA10 /> <I_XNMNCA11 /> <I_FINIVI11>0</I_FINIVI11> <I_FFINVI11>0</I_FFINVI11> <I_VNUMDA11>0</I_VNUMDA11> <I_XALFDA11 /> <I_XNMNCA12 /> <I_FINIVI12>0</I_FINIVI12> <I_FFINVI12>0</I_FFINVI12> <I_VNUMDA12>0</I_VNUMDA12> <I_XALFDA12 /> <I_XIDENUE /> <I_NIDENUE>0</I_NIDENUE> <I_XNUINUE /> <I_NSUCNUE>0</I_NSUCNUE> <I_NCOSNUE>0</I_NCOSNUE> <I_NRES003>0</I_NRES003> <I_NRES004>0</I_NRES004> <I_TRES001>CC</I_TRES001> <I_NRES001>79855875</I_NRES001> <I_XRES001 /> <I_TRES002 /> <I_NRES002>0</I_NRES002> <I_XRES002 /> <I_VVIN>0</I_VVIN> <I_XESCVIN>2</I_XESCVIN> <I_CESTVIN>1</I_CESTVIN> <I_NDIRVIN>11001</I_NDIRVIN> <I_FINIAFI>20080128</I_FINIAFI> <I_FFINAFI>20100701</I_FFINAFI> <I_CRETAFI>6</I_CRETAFI> <I_FVIN>0</I_FVIN> <I_QVIN>1</I_QVIN> <I_XVIN /> <I_XALFADI>PEÑA HERNANDEZ CESAR AUGUSTO</I_XALFADI> <I_NTELCLI>0</I_NTELCLI> <I_NEXTCLI>0</I_NEXTCLI> <I_CZONBTA>999</I_CZONBTA> <I_CCIUCLI>11001</I_CCIUCLI> <I_FINGEMP>0</I_FINGEMP> <I_CSED>1000</I_CSED> <I_CODEPS>8</I_CODEPS> <MESSAGE> 13:09:36:38 INPUT REQUEST 0.00</MESSAGE> </CLE15> </CLE15>"
                        End If
                    Else
                        returnvalue = XmlResultado
                    End If
                Next
            End With
            Return returnvalue
        Catch Ex As Exception
            'CREADO LCARDENAS 19-04-2012: SEGUIMIENTO INCIDENCIA 9430 INTERMITENCIA WSCOMCLIENTES
            If ConfigurationSettings.AppSettings("LogAuditoriaAfiliar") = "S" Then
                Dim objCache As New Compensar.Vincula.POS.ClsCache
                Dim sInnerExcepcion As String = ""
                If Not Ex.InnerException Is Nothing Then
                    sInnerExcepcion = Ex.InnerException.ToString()
                End If
                objCache.guardarErrorBitacora(ConfigurationSettings.AppSettings("ProjectID"), _
1, "Auditoria Afiliar: cVinculacion.Vinculacion: " & Ex.Message & sInnerExcepcion & "StackTrace: " & Ex.StackTrace, "", "", "", My.Computer.Name, 1, 1)
            End If
            'CManejadorMensajes.PublicarMensaje("ClsVinculacion:ERROR" & Ex.Message & " - " & Ex.Source, EventLogEntryType.Error)
            Dim strMensaje As String, strMensajeOrig As String, intPosIni As Integer, intPosFin As Integer
            strMensaje = Ex.Message
            strMensajeOrig = strMensaje
            If strMensaje.Contains("DtsCLE15") Then
                intPosIni = strMensaje.IndexOf(":")
                intPosFin = strMensaje.IndexOf("DtsCLE15")
                strMensaje = "Error: Validar caracteres especiales en Datos en Entrada"
                If intPosIni > 0 Then
                    Dim intLon As Integer
                    intLon = intPosFin - intPosIni - 1
                    'strMensaje = strMensaje + strMensajeOrig.Substring(intPosIni - 1, intLon)
                    Dim strCampo() As String
                    strCampo = strMensajeOrig.Split(",")
                    If strCampo.Length > 0 Then
                        strCampo = strCampo(0).ToString.Split(":")
                        If strCampo.Length > 1 Then
                            Dim dtsResultado As DataSet
                            dtsResultado = ObjUtil.GetDataSet(XmlResultadoTemp)

                            strMensaje = strMensaje + ". " + strCampo(1).ToString + ", " + dtsResultado.Tables(0).Rows(0).Item(LTrim(strCampo(1).ToString))
                        End If

                    End If
                End If
            End If

            Return ObjUtil.ConvertToXMLdoc(strMensaje)
        End Try
    End Function
    Public Function ValidaRepuesta(ByVal XmlResultado As String) As String
        Dim xmlDoc As XmlDocument
        Dim xmlnodo As XmlNode
        Dim returnvalue As String = ""
        CManejadorMensajes.PublicarMensaje("ClsVinculacion:Vinculacion2" & cle15.GetXml() & " - " & XmlResultado, EventLogEntryType.SuccessAudit)
        xmlDoc = New XmlDocument()
        xmlDoc.LoadXml(XmlResultado)
        xmlnodo = xmlDoc.SelectSingleNode("//Mensaje")
        returnvalue = XmlResultado
        If (xmlnodo Is Nothing) Then

            CManejadorMensajes.PublicarMensaje("ClsVinculacion:Vinculacion3" & cle15.GetXml(), EventLogEntryType.SuccessAudit)
        Else ' si no contiene el tag
            'retona vacio
            Return returnvalue
        End If
        Return returnvalue
    End Function

#End Region

    Public Sub New()

    End Sub
End Class

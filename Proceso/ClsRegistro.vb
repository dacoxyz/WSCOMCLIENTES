Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Configuration
Imports EntidadesNegocio

Public Class ClsRegistro
    Inherits ClsCliente

#Region "Declaraciones"
    Dim ObjUtil As New Utilidades.CUtil
    Dim ObjTran As New Servicios.ClsServicio
    Dim cle15 As New ObjTransferDatos.DtsCLE15
    Dim m_OperacionMod As String
    Dim m_TipoIdentOrg As String
    Dim m_NroIdentificacionOrg As String
    Dim m_PartealfabeticaOrg As String
    Dim m_DigitoChequeoOrg As String
    Dim m_sucursalOrg As String
    Dim m_CentroCostoOrg As String
    Dim m_ConfirmaApellNomb As String
    Dim m_señalConfirmaFonetico As String
    Dim m_señalConfirma As String
    Dim m_DireccionOrg As String
    Dim m_Usuario As String
    Dim m_Aplicacion As String
    Dim m_Programa As String
    Dim m_Condicion As String
    Dim m_Ndir As String   '4249 20140505
    Dim m_Grupo As String
    Dim m_ConfirmaEstadoCivil As String
#End Region

#Region "Propiedades"
    Property OperacionMod() As String
        Get
            Return m_OperacionMod
        End Get
        Set(ByVal value As String)
            m_OperacionMod = value
        End Set
    End Property
    Property TipoIdentOrg() As String
        Get
            Return m_TipoIdentOrg
        End Get
        Set(ByVal value As String)
            m_TipoIdentOrg = value
        End Set
    End Property

    Property NroIdentificacionOrg() As String
        Get
            Return m_NroIdentificacionOrg
        End Get
        Set(ByVal value As String)
            m_NroIdentificacionOrg = value
        End Set
    End Property

    Property PartealfabeticaOrg() As String
        Get
            Return m_PartealfabeticaOrg
        End Get
        Set(ByVal value As String)
            m_PartealfabeticaOrg = value
        End Set
    End Property

    Property DigitoChequeoOrg() As String
        Get
            Return m_DigitoChequeoOrg
        End Get
        Set(ByVal value As String)
            m_DigitoChequeoOrg = value
        End Set
    End Property

    Property sucursalOrg() As String
        Get
            Return m_sucursalOrg
        End Get
        Set(ByVal value As String)
            m_sucursalOrg = value
        End Set
    End Property

    Property CentroCostoOrg() As String
        Get
            Return m_CentroCostoOrg
        End Get
        Set(ByVal value As String)
            m_CentroCostoOrg = value
        End Set
    End Property

    Property DireccionOrg() As String
        Get
            Return m_DireccionOrg
        End Get
        Set(ByVal value As String)
            m_DireccionOrg = value
        End Set
    End Property

    Property señalConfirmaFonetico() As String
        Get
            Return m_señalConfirmaFonetico
        End Get
        Set(ByVal value As String)
            m_señalConfirmaFonetico = value
        End Set
    End Property

    Property señalConfirma() As String
        Get
            Return m_señalConfirma
        End Get
        Set(ByVal value As String)
            m_señalConfirma = value
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
    '4249 20140505
    Property Ndir() As String
        Get
            Return m_Ndir
        End Get
        Set(ByVal value As String)
            m_Ndir = value
        End Set
    End Property
    '4249 20140505

    Property Grupo() As String
        Get
            Return m_Grupo
        End Get
        Set(ByVal value As String)
            m_Grupo = value
        End Set
    End Property

    Property ConfirmaApellNomb() As String
        Get
            Return m_ConfirmaApellNomb
        End Get
        Set(ByVal value As String)
            m_ConfirmaApellNomb = value
        End Set
    End Property

    Property Usuario() As String
        Get
            Return m_Usuario
        End Get
        Set(ByVal value As String)
            m_Usuario = value
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

    Property ConfirmaEstadoCivil() As String
        Get
            Return m_ConfirmaEstadoCivil
        End Get
        Set(ByVal value As String)
            m_ConfirmaEstadoCivil = value
        End Set
    End Property
#End Region

#Region "Metodos"

    Public Function ConsultarCliente() As String
        Try
            Dim i As Integer
            Dim returnvalue As String = ""
            With ObjTran
                .DtsInfoTran = cle15
                '4249 20140505
                returnvalue = .ejecutaAldea(m_Aplicacion, TipoIdent.ToUpper, NroIdentificacion, Partealfabetica, _
                                            ObjUtil.strAsignarValor(Sucursal, "0"), ObjUtil.strAsignarValor(CentroCosto, "0"), _
                                            ObjUtil.strAsignarValor(m_Condicion, "0"), m_Programa, m_Ndir)
            End With
            Dim ds As DataSet = ObjUtil.GetDataSet(returnvalue)
            Dim dr As Data.DataRow
            Dim drDc As Data.DataRow()
            dr = cle15.Tables(0).NewRow
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 And ds.Tables(0).TableName = "Table" Then
                    With cle15.Tables(0)
                        dr("_TOP_LINE_") = "CLE15T" & DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")
                        dr("I_MAINT") = "INQ"
                        dr("I_CALT") = m_Usuario
                        dr("I_XIDECLI") = TipoIdent.ToUpper
                        If ds.Tables(0).Columns.Contains("NAUTCLI") Then
                            dr("I_NAUTCLI") = ds.Tables(0).Rows(0)("NAUTCLI").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("NIDECLI") Then
                            dr("I_NIDECLI") = ds.Tables(0).Rows(0)("NIDECLI").ToString()
                        End If
                        dr("I_XNUICLI") = Partealfabetica
                        'dr("I_NDIGCHE") = ObjUtil.strAsignarValor(DigitoChequeo.ToString, "0")
                        If ds.Tables(0).Columns.Contains("NVERCLI") Then
                            dr("I_NDIGCHE") = ds.Tables(0).Rows(0)("NVERCLI").ToString()
                        Else
                            dr("I_NDIGCHE") = ObjUtil.strAsignarValor(DigitoChequeo.ToString, "0")
                        End If
                        If ds.Tables(0).Columns.Contains("NSUCCLI") Then
                            dr("I_NIDESUC") = ds.Tables(0).Rows(0)("NSUCCLI").ToString()
                        Else
                            dr("I_NIDESUC") = ObjUtil.strAsignarValor(Sucursal, "0")
                        End If
                        If ds.Tables(0).Columns.Contains("NCOSCLI") Then
                            dr("I_NIDECOS") = ds.Tables(0).Rows(0)("NCOSCLI").ToString()
                        Else
                            dr("I_NIDECOS") = ObjUtil.strAsignarValor(CentroCosto, "0")
                        End If
                        '4249 20140505
                        If ds.Tables(0).Columns.Contains("I_NDIR") Then
                            dr("I_NDIR") = ds.Tables(0).Rows(0)("I_NDIR").ToString()
                        Else
                            dr("I_NDIR") = m_Ndir
                        End If
                        '4249 20140505
                        If ds.Tables(0).Columns.Contains("FVALCLI") Then
                            dr("I_FVALCLI") = ds.Tables(0).Rows(0)("FVALCLI").ToString()
                        Else
                            dr("I_FVALCLI") = "0"
                        End If

                        If Responsable2 <> "0" Then
                            If Responsable2 <> "" Then
                                drDc = ds.Tables(0).Select("XRESP2='" & "2" + "-" + Responsable2 & "'")
                                If drDc.Length > 0 Then
                                    dr("I_FINIAFI") = drDc(0).Item("FINIAFI")
                                    dr("I_FFINAFI") = drDc(0).Item("FFINAFI")
                                    If ds.Tables(0).Rows(0)("XRESP2").ToString() <> "" Then
                                        dr("I_TRES002") = ObjUtil.GetEquivTipoDoc(drDc(0).Item("XRESP2").ToString().Substring(0, 1))
                                        dr("I_NRES002") = drDc(0).Item("XRESP2").ToString().Substring(2)
                                    End If
                                Else
                                    drDc = ds.Tables(0).Select("XRESP2='" & "1" + "-" + Responsable2 & "'")
                                    If drDc.Length > 0 Then
                                        dr("I_FINIAFI") = drDc(0).Item("FINIAFI")
                                        dr("I_FFINAFI") = drDc(0).Item("FFINAFI")
                                        If ds.Tables(0).Rows(0)("XRESP2").ToString() <> "" Then
                                            dr("I_TRES002") = ObjUtil.GetEquivTipoDoc(drDc(0).Item("XRESP2").ToString().Substring(0, 1))
                                            dr("I_NRES002") = drDc(0).Item("XRESP2").ToString().Substring(2)
                                        End If
                                    Else
                                        If ds.Tables(0).Rows(0)("XRESP2").ToString() <> "" Then
                                            dr("I_TRES002") = ObjUtil.GetEquivTipoDoc(ds.Tables(0).Rows(0)("XRESP2").ToString().Substring(0, 1))
                                            '10272 DAC 20120910
                                            dr("I_NRES002") = Responsable2

                                        End If
                                    End If
                                End If
                            Else
                                If ds.Tables(0).Columns.Contains("FINIAFI") Then
                                    dr("I_FINIAFI") = ds.Tables(0).Rows(0)("FINIAFI").ToString()
                                End If
                                If ds.Tables(0).Columns.Contains("FFINAFI") Then
                                    dr("I_FFINAFI") = ds.Tables(0).Rows(0)("FFINAFI").ToString()
                                End If
                                If ds.Tables(0).Columns.Contains("XRESP2") Then
                                    If ds.Tables(0).Rows(0)("XRESP2").ToString() <> "" Then
                                        dr("I_TRES002") = ObjUtil.GetEquivTipoDoc(ds.Tables(0).Rows(0)("XRESP2").ToString().Substring(0, 1))
                                        dr("I_NRES002") = ds.Tables(0).Rows(0)("XRESP2").ToString().Substring(2)
                                    End If
                                Else
                                    dr("I_TRES002") = ObjUtil.strAsignarValor(TipoIdentificacion2, "0")
                                    dr("I_NRES002") = ObjUtil.strAsignarValor(Responsable2, "0")
                                End If
                            End If
                        Else
                            If ds.Tables(0).Columns.Contains("FINIAFI") Then
                                dr("I_FINIAFI") = ds.Tables(0).Rows(0)("FINIAFI").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("FFINAFI") Then
                                dr("I_FFINAFI") = ds.Tables(0).Rows(0)("FFINAFI").ToString()
                            End If
                            If ds.Tables(0).Columns.Contains("XRESP2") Then
                                If ds.Tables(0).Rows(0)("XRESP2").ToString() <> "" Then
                                    dr("I_TRES002") = ObjUtil.GetEquivTipoDoc(ds.Tables(0).Rows(0)("XRESP2").ToString().Substring(0, 1))
                                    dr("I_NRES002") = ds.Tables(0).Rows(0)("XRESP2").ToString().Substring(2)
                                End If
                            Else
                                dr("I_TRES002") = ObjUtil.strAsignarValor(TipoIdentificacion2, "0")
                                dr("I_NRES002") = ObjUtil.strAsignarValor(Responsable2, "0")
                            End If
                        End If
                        dr("I_CPRGSRV") = m_Programa
                        dr("I_NCONAFI") = ObjUtil.strAsignarValor(m_Condicion, "0")
                        dr("I_CGRUCAM") = ObjUtil.strAsignarValor(m_Grupo, "0")
                        If ds.Tables(0).Columns.Contains("XRESP1") Then
                            If ds.Tables(0).Rows(0)("XRESP1").ToString() <> "" Then
                                dr("I_TRES001") = ObjUtil.GetEquivTipoDoc(ds.Tables(0).Rows(0)("XRESP1").ToString().Substring(0, 1))
                                dr("I_NRES001") = ds.Tables(0).Rows(0)("XRESP1").ToString().Substring(2)
                            End If
                        Else
                            dr("I_TRES001") = ObjUtil.strAsignarValor(TipoIdentificacion1, "0")
                            dr("I_NRES001") = ObjUtil.strAsignarValor(Responsable1, "0")
                        End If
                        If ds.Tables(0).Columns.Contains("XGENCLI") Then
                            dr("I_XGENCLI") = ds.Tables(0).Rows(0)("XGENCLI").ToString()
                        Else
                            dr("I_XGENCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("FNACCLI") Then
                            dr("I_FNACCLI") = ds.Tables(0).Rows(0)("FNACCLI").ToString()
                        Else
                            dr("I_FNACCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XECICLI") Then
                            dr("I_CECICLI") = ds.Tables(0).Rows(0)("XECICLI").ToString()
                        Else
                            dr("I_CECICLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XNIVADI") Then
                            dr("I_XNIVADI") = ds.Tables(0).Rows(0)("XNIVADI").ToString()
                        Else
                            dr("I_XNIVADI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("CCAUINA") Then
                            dr("I_CCAUINA") = ds.Tables(0).Rows(0)("CCAUINA").ToString()
                        Else
                            dr("I_CCAUINA") = "0"
                        End If
                        If ds.Tables(0).Columns.Contains("XPRIAPE") Then
                            dr("I_XPRIAPE") = ds.Tables(0).Rows(0)("XPRIAPE").ToString()
                        Else
                            dr("I_XPRIAPE") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XSEGAPE") Then
                            dr("I_XSEGAPE") = ds.Tables(0).Rows(0)("XSEGAPE").ToString()
                        Else
                            dr("I_XSEGAPE") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XNOMCLI") Then
                            dr("I_XNOMTRAB") = ds.Tables(0).Rows(0)("XNOMCLI").ToString()
                        Else
                            dr("I_XNOMTRAB") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XRAZSEG") Then
                            dr("I_XDESRAZ") = ds.Tables(0).Rows(0)("XRAZSEG").ToString()
                        Else
                            dr("I_XDESRAZ") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XTIPDIR") Then
                            dr("I_XTIPDIR") = ds.Tables(0).Rows(0)("XTIPDIR").ToString()
                        Else
                            dr("I_XTIPDIR") = ""
                        End If

                        If ds.Tables(0).Columns.Contains("XBARRIO") Then
                            If ds.Tables(0).Rows(0)("XBARRIO").ToString.Length > 18 Then
                                dr("I_XBARCLI") = ds.Tables(0).Rows(0)("XBARRIO").ToString.Substring(0, 18)
                            Else
                                dr("I_XBARCLI") = ds.Tables(0).Rows(0)("XBARRIO").ToString()
                            End If
                        Else
                            dr("I_XBARCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XDIR") Then
                            dr("I_XDIRCLI") = ds.Tables(0).Rows(0)("XDIR").ToString()
                        Else
                            dr("I_XDIRCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("NTELCLI") Then
                            dr("I_NTELCLI") = ds.Tables(0).Rows(0)("NTELCLI").ToString()
                        Else
                            dr("I_NTELCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("NEXTCLI") Then
                            dr("I_NEXTCLI") = ds.Tables(0).Rows(0)("NEXTCLI").ToString()
                        Else
                            dr("I_NEXTCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("CZON") Then
                            If ds.Tables(0).Rows(0)("CZON").ToString.Length > 4 Then
                                dr("I_CZONBTA") = ds.Tables(0).Rows(0)("CZON").ToString.Substring(0, 4)
                            Else
                                dr("I_CZONBTA") = ds.Tables(0).Rows(0)("CZON").ToString()
                            End If
                        Else
                            dr("I_CZONBTA") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("CCIU") Then
                            dr("I_CCIUCLI") = ds.Tables(0).Rows(0)("CCIU").ToString()
                        Else
                            dr("I_CCIUCLI") = ""
                        End If
                        dr("MESSAGE") = "INPUT REQUEST"
                    End With
                    cle15.Tables(0).Rows.Add(dr)
                Else
                    If dr.Table.Columns.Count <> 0 Then
                        For i = 0 To dr.Table.Columns.Count - 1
                            If dr.Item(i).ToString = "" Then
                                dr(i) = ""
                            End If
                        Next
                    End If
                    dr("MESSAGE") = "CLIENTE NO EXISTE"
                    cle15.Tables(0).Rows.Add(dr)
                End If
            Else
                If dr.Table.Columns.Count <> 0 Then
                    For i = 0 To dr.Table.Columns.Count - 1
                        If dr.Item(i).ToString = "" Then
                            dr(i) = ""
                        End If
                    Next
                End If
                dr("MESSAGE") = "CLIENTE NO EXISTE"
                cle15.Tables(0).Rows.Add(dr)
            End If
            returnvalue = cle15.GetXml
            cle15.Clear()
            returnvalue = returnvalue.Replace("<DtsCLE15 xmlns=""http://tempuri.org/DtsCLE15.xsd"">", "<CLE15>")
            Return returnvalue.Replace("</DtsCLE15>", "</CLE15>")
        Catch Ex As Exception
            Return ObjUtil.ConvertToXMLdoc(Ex.Message)
        End Try
    End Function

    Public Function ModificarCliente() As String
        Dim returnvalue As String = ""
        Dim strRespuesta As String = ""
        Dim Operacion As String = ""
        Dim Accion As String = ""
        Dim dr As Data.DataRow
        Dim xDesRaz As String = ""
        Try
            dr = cle15.Tables(0).NewRow
            'Cambio de identificacion
            If (TipoIdent.ToUpper <> m_TipoIdentOrg.ToUpper Or _
                NroIdentificacion <> m_NroIdentificacionOrg Or _
                Partealfabetica <> m_PartealfabeticaOrg Or _
                DigitoChequeo <> m_DigitoChequeoOrg Or _
                Sucursal <> m_sucursalOrg Or _
                CentroCosto <> m_CentroCostoOrg) Then
                Accion = "CHG"
                Operacion = "I"
                'DigitoChequeo <> m_DigitoChequeoOrg Or _
                returnvalue = ModfDatosSilmul(Accion, Operacion, xDesRaz)
                strRespuesta = " - Mensaje Cambio Documento : " & leeXmlNodo(returnvalue) & "  " & strRespuesta
            End If
            'Nuevo nombre o apellido o fonetico o todos
            If TipoIdent.ToUpper <> "NI" Then
                returnvalue = Valida_fonetico_Nombres_apellidos(xDesRaz, Operacion)
                strRespuesta = returnvalue & "  " & strRespuesta
                If returnvalue <> "" Then
                    Return returnvalue
                End If
                '004249 identifica 
                If Nombre.Length > 30 Then
                    Dim iPos As Integer
                    iPos = Nombre.IndexOf(" ")
                    If iPos > 1 Then
                        xDesRaz = LTrim(Nombre.Substring(iPos))
                        Nombre = Left(Nombre, iPos)
                    End If
                End If
                ''004249
            Else
                xDesRaz = Razonsocial.ToUpper
            End If
            If OperacionMod <> "" Then
                Accion = OperacionMod
                returnvalue = ModfDatosSilmul(Accion, Operacion, xDesRaz)
                strRespuesta = " - Mensaje Modificacion : " & leeXmlNodo(returnvalue) & "  " & strRespuesta
                Accion = ""
            End If
            'Nueva direccion
            If m_DireccionOrg = "" And Direccion <> "" Then
                Accion = "ADD"
                returnvalue = ModfDatosSilmul(Accion, Operacion, xDesRaz)
                strRespuesta = " - Mensaje Direccion : " & leeXmlNodo(returnvalue) & "  " & strRespuesta
                Operacion = ""
                Accion = ""
            End If
            If EstadoCivil <> m_ConfirmaEstadoCivil Then
                Accion = "EST"
                Operacion = "C"
                returnvalue = ModfDatosSilmul(Accion, Operacion, xDesRaz)
                strRespuesta = " - Mensaje Estado Civil : " & leeXmlNodo(returnvalue) & "  " & strRespuesta
                Operacion = ""
                Accion = ""
            End If
           
            If strRespuesta <> "" Then
                returnvalue = "<Req><Mensaje>" & strRespuesta & "</Mensaje></Req>"
            Else
                returnvalue = "<Req><Error>No se realizo modificacion</Error></Req>"
            End If
        Catch Ex As Exception
            Return ObjUtil.ConvertToXMLdoc(Ex.Message)
            returnvalue = ObjUtil.ConvertToXMLdoc(Ex.Message)
        End Try
        Return returnvalue
    End Function
    Public Function leeXmlNodo(ByVal cadenaRetorna As String) As String
        Dim xmlDoc As New Xml.XmlDocument()
        Dim xmlNodo As Xml.XmlNode
        Dim strRepuesta As String
        xmlDoc.LoadXml(cadenaRetorna)
        xmlNodo = xmlDoc.SelectSingleNode("//MESSAGE")
        If cadenaRetorna.Contains("MESSAGE") Then
            If InStr(1, xmlNodo.InnerXml.ToString, "SUCCESSFUL ENTRY") > 0 Or InStr(1, xmlNodo.InnerXml.ToString, "INPUT REQUEST") > 0 Then
                strRepuesta = "MODIFICACIÓN EXITOSA"
                'strRepuesta = xmlNodo.InnerXml.ToString()
            Else
                strRepuesta = xmlNodo.InnerXml.ToString()
            End If
        ElseIf cadenaRetorna.Contains("ERROR") Then
            xmlNodo = Nothing
            strRepuesta = xmlNodo.InnerXml.ToString()
        Else
            xmlNodo = Nothing
            strRepuesta = cadenaRetorna
        End If
        Return strRepuesta
    End Function
    Public Function ModfDatosSilmul(ByVal Accion As String, ByVal Operacion As String, Optional ByVal xDesRaz As String = "") As String
        Try
            Dim returnvalue As String = ""
            Dim dr As Data.DataRow
            dr = cle15.Tables(0).NewRow
            With cle15.Tables(0)
                dr("_TOP_LINE_") = "CLE15T" & DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")
                dr("I_MAINT") = Accion
                dr("I_XINDOPC") = Operacion
                dr("I_CALT") = m_Usuario
                dr("I_XIDECLI") = m_TipoIdentOrg.ToUpper
                dr("I_NIDECLI") = m_NroIdentificacionOrg
                dr("I_XNUICLI") = m_PartealfabeticaOrg
                dr("I_NDIGCHE") = ObjUtil.strAsignarValor(DigitoChequeo.ToString, "0")
                dr("I_NIDESUC") = ObjUtil.strAsignarValor(m_sucursalOrg, "0")
                dr("I_NIDECOS") = ObjUtil.strAsignarValor(m_CentroCostoOrg, "0")
                dr("I_XDESRAZ") = xDesRaz
                dr("I_XPRIAPE") = PrimerApellido.ToUpper
                dr("I_XSEGAPE") = SegundoApellido.ToUpper
                If Nombre IsNot Nothing Then
                    If TipoIdent.ToString.ToUpper <> "NI" Then
                        dr("I_XNOMTRAB") = Nombre.ToUpper
                    End If
                End If
                dr("I_FNACCLI") = ObjUtil.strAsignarValor(Fechanacimiento, "0")
                dr("I_XGENCLI") = Genero
                dr("I_CECICLI") = EstadoCivil
                dr("I_XNIVADI") = IIf(Detalleadicional <> "", Detalleadicional, "N")
                dr("I_XTIPDIR") = ObjUtil.strAsignarValor(TipoDireccion.ToUpper, "U")
                dr("I_XDIRCLI") = Direccion
                '4249 20140505
                If Not m_Ndir Is Nothing AndAlso Ndir.Length > 0 Then
                    dr("I_NDIR") = Ndir
                End If
                '4249 20140505
                If Barrio IsNot Nothing Then
                    If Barrio.ToString.Length > 18 Then
                        dr("I_XBARCLI") = Barrio.ToString.Substring(0, 18)
                    Else
                        dr("I_XBARCLI") = Barrio.ToString()
                    End If
                End If
                dr("I_NTELCLI") = ObjUtil.strAsignarValor(Telefono, "0")
                dr("I_NEXTCLI") = ObjUtil.strAsignarValor(Extension, "0")
                dr("I_CCIUCLI") = ObjUtil.strAsignarValor(Ciudad, "0")
                dr("I_CZONBTA") = ObjUtil.strAsignarValor(Zona, "0")
                If Operacion = "I" Then  'Cambio de identificacion
                    dr("I_XIDENUE") = TipoIdent
                    dr("I_NIDENUE") = NroIdentificacion
                    dr("I_XNUINUE") = Partealfabetica
                    dr("I_NSUCNUE") = ObjUtil.strAsignarValor(Sucursal, "0")
                    dr("I_NCOSNUE") = ObjUtil.strAsignarValor(CentroCosto, "0")
                End If
            End With
            cle15.Tables(0).Rows.Add(dr)
            With ObjTran
                .DtsInfoTran = cle15
                returnvalue = .EjecutaTransaccion(m_Aplicacion)
                Dim XmlResultado As String = ""
                cle15.Clear()
            End With
            Return returnvalue
        Catch Ex As Exception
            Return ObjUtil.ConvertToXMLdoc(Ex.Message)
        End Try
    End Function

    Public Function CrearCliente() As String
        Try
            Dim returnvalue As String = ""
            Dim xDesRaz As String = ""
            Dim Fonetico As String = ""
            Dim dr As Data.DataRow
            dr = cle15.Tables(0).NewRow
            If TipoIdent.ToUpper <> "NI" Then
                returnvalue = Valida_fonetico_Nombres_apellidos(xDesRaz, Fonetico)
                If returnvalue <> "" Then
                    Return returnvalue
                End If
                '004249 identifica 
                If Nombre.Length > 30 Then
                    Dim iPos As Integer
                    iPos = Nombre.IndexOf(" ")
                    If iPos > 1 Then
                        xDesRaz = LTrim(Nombre.Substring(iPos))
                        Nombre = Left(Nombre, iPos)
                    End If
                End If
                ''004249
            Else
                xDesRaz = Razonsocial.ToUpper
            End If
            With cle15.Tables(0)
                dr("_TOP_LINE_") = "CLE15T" & DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")
                dr("I_MAINT") = "ADD"
                dr("I_XINDOPC") = Fonetico
                dr("I_CALT") = m_Usuario
                dr("I_XIDECLI") = TipoIdent.ToUpper
                dr("I_NIDECLI") = NroIdentificacion
                dr("I_XNUICLI") = Partealfabetica
                dr("I_NDIGCHE") = ObjUtil.strAsignarValor(DigitoChequeo.ToString, "0")
                dr("I_NIDESUC") = ObjUtil.strAsignarValor(Sucursal, "0")
                dr("I_NIDECOS") = ObjUtil.strAsignarValor(CentroCosto, "0")
                dr("I_XDESRAZ") = xDesRaz
                dr("I_XPRIAPE") = PrimerApellido.ToUpper
                dr("I_XSEGAPE") = SegundoApellido.ToUpper
                dr("I_XNOMTRAB") = Nombre.ToUpper
                dr("I_FNACCLI") = ObjUtil.strAsignarValor(Fechanacimiento, "0")
                dr("I_XGENCLI") = Genero
                dr("I_CECICLI") = EstadoCivil
                dr("I_XNIVADI") = IIf(Detalleadicional <> "", Detalleadicional, "N")
                dr("I_XTIPDIR") = ObjUtil.strAsignarValor(TipoDireccion.ToUpper, "U")
                dr("I_XDIRCLI") = Direccion
                dr("I_XBARCLI") = Barrio
                dr("I_NTELCLI") = ObjUtil.strAsignarValor(Telefono, "0")
                dr("I_NEXTCLI") = ObjUtil.strAsignarValor(Extension, "0")
                dr("I_CCIUCLI") = ObjUtil.strAsignarValor(Ciudad, "0")
                dr("I_CZONBTA") = ObjUtil.strAsignarValor(Zona, "0")
            End With
            cle15.Tables(0).Rows.Add(dr)
            With ObjTran
                .DtsInfoTran = cle15
                returnvalue = .EjecutaTransaccion(m_Aplicacion)
            End With
            Return returnvalue
        Catch Ex As Exception
            Return ObjUtil.ConvertToXMLdoc(Ex.Message)
        End Try
    End Function

    Private Function Valida_fonetico_Nombres_apellidos(ByRef xDesRaz As String, ByRef Fonetico As String) As String
        Dim returnvalue As String = ""
        If m_señalConfirma <> "" Then
            Select Case m_señalConfirma
                Case "1" 'PRIMER APELLIDO
                    xDesRaz = PrimerApellido.ToUpper
                    If PrimerApellido.ToUpper <> m_ConfirmaApellNomb.ToUpper Then
                        returnvalue = "Error en la confirmación del primer apellido"
                        Return ObjUtil.ConvertToXMLdoc(returnvalue)
                    End If
                Case "2" 'SEGUNDO APELLIDO
                    xDesRaz = SegundoApellido.ToUpper
                    If SegundoApellido.ToUpper <> m_ConfirmaApellNomb.ToUpper Then
                        returnvalue = "Error en la confirmación del segundo apellido"
                        Return ObjUtil.ConvertToXMLdoc(returnvalue)
                    End If
                Case "3", "4" 'PRIMER y SEGUNDO NOMBRE
                    Dim pos As Integer = IIf(Nombre.IndexOf(" ") > 0, Nombre.IndexOf(" "), Nombre.Length - 1)
                    If m_señalConfirma = "3" Then
                        xDesRaz = Nombre.ToUpper.Substring(0, pos + 1)
                    Else
                        xDesRaz = Nombre.ToUpper.Substring(pos + 1, Nombre.Length - (pos + 1))
                    End If
                    If xDesRaz.Trim.ToUpper <> m_ConfirmaApellNomb.ToUpper Then
                        returnvalue = "Error en la confirmación del primer nombre"
                        Return ObjUtil.ConvertToXMLdoc(returnvalue)
                    End If
            End Select
        End If
        If m_señalConfirmaFonetico = "5" Then
            Fonetico = "F"
        End If
        Return returnvalue
    End Function

    Public Function Fidelizar() As String
        Dim Accion As String = "FID"
        Return ExecOperationAdi(Accion)
    End Function

    Public Function NuevoRadicado() As String
        Dim Accion As String = "NEW"
        Return ExecOperationAdi(Accion)
    End Function

    Public Function NivelAdicional() As String
        Dim Accion As String = "NIV"
        Return ExecOperationAdi(Accion)
    End Function

    Private Function ExecOperationAdi(ByVal Accion As String) As String
        Try
            Dim returnvalue As String = ""
            Dim dr As Data.DataRow
            dr = cle15.Tables(0).NewRow
            With cle15.Tables(0)
                dr("_TOP_LINE_") = "CLE15T" & DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")
                dr("I_MAINT") = Accion
                dr("I_CALT") = m_Usuario
                dr("I_XIDECLI") = TipoIdent.ToUpper
                dr("I_NIDECLI") = NroIdentificacion
                dr("I_XNUICLI") = Partealfabetica
                dr("I_NDIGCHE") = ObjUtil.strAsignarValor(DigitoChequeo, "0")
                dr("I_NIDESUC") = ObjUtil.strAsignarValor(Sucursal, "0")
                dr("I_NIDECOS") = ObjUtil.strAsignarValor(CentroCosto, "0")
            End With
            cle15.Tables(0).Rows.Add(dr)
            With ObjTran
                .DtsInfoTran = cle15
                returnvalue = .EjecutaTransaccion(m_Aplicacion)
            End With
            Return returnvalue
        Catch Ex As Exception
            Return ObjUtil.ConvertToXMLdoc(Ex.Message)
        End Try
    End Function
#End Region

End Class

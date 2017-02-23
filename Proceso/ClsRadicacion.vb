Imports Microsoft.VisualBasic
Imports System.Xml
Imports EntidadesNegocio
Imports System.Text.RegularExpressions


Public Class ClsRadicacion
    Inherits ClsCliente

#Region "Declaraciones"
    Dim ObjUtil As New Utilidades.CUtil
    Dim ObjTran As New Servicios.ClsServicio
    Dim cle15 As New ObjTransferDatos.DtsCLE15
    Dim m_Programa As String
    Dim m_Grupo As String
    Dim m_Condicion As String
    Dim m_Usuario As String
    Dim m_Aplicacion As String


    'control de cambio: sep 9 2009: wlm: unificacion de radicacion y cambio estado.
    Dim m_indopcion As String
    Dim m_estadoVin As String

    Dim m_causalRetiro As String
    Dim m_fechaFinAfiliacion As String
    Dim m_FechaRetiro As String



#End Region

#Region "Propiedades"
    Property Grupo() As String
        Get
            Return m_Grupo
        End Get
        Set(ByVal value As String)
            m_Grupo = value
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

    'Property IndOpcion() As String
    '    Get
    '        Return m_indopcion
    '    End Get
    '    Set(ByVal value As String)
    '        m_indopcion = value
    '    End Set
    'End Property

    'Property estadoVin() As String
    '    Get
    '        Return m_estadoVin
    '    End Get
    '    Set(ByVal value As String)
    '        m_estadoVin = value

    '    End Set
    'End Property




#End Region

#Region "Metodos"
    Public Function Radicacion() As String
        Dim pattern As String = "[^A-Za-z0-9| |Ñ|n]"
        Dim rgx As New Regex(pattern)
        Try
            Dim returnvalue As String = ""
            Dim i As Integer
            Dim dr As Data.DataRow
            dr = cle15.Tables(0).NewRow
            With cle15.Tables(0)
                dr("_TOP_LINE_") = "CLE15T" & DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")
                If String.IsNullOrEmpty(Me.IndOption) = False Then
                    Dim blMaint As Boolean = False
                    For j As Integer = 0 To 11
                        If (NameReq(j) = "PLABOR") Or (NameReq(j) = "MODEST") Then
                            dr("I_MAINT") = "EST"
                            blMaint = True
                            Exit For
                        End If
                    Next
                    If (Not blMaint) Then
                        dr("I_MAINT") = "DAT"
                    End If
                Else
                    dr("I_MAINT") = "DAT"
                End If
                dr("I_CALT") = m_Usuario
                If TipoIdent IsNot Nothing Then
                    dr("I_XIDECLI") = TipoIdent.ToUpper
                End If
                dr("I_NIDECLI") = NroIdentificacion
                dr("I_XNUICLI") = Partealfabetica
                If DigitoChequeo IsNot Nothing Then
                    dr("I_NDIGCHE") = ObjUtil.strAsignarValor(DigitoChequeo.ToString, "0")
                End If
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
                    If TipoIdent <> "NI" Then
                        dr("I_XNOMTRAB") = rgx.Replace(Nombre.ToUpper, "").ToString
                    End If

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
                dr("I_XALFADI") = InfoAdicional
                '// wlm:sep 09:control de cambios: unificacion radicacion cambio estado.
                If String.IsNullOrEmpty(Me.IndOption) = False Then
                    dr("I_XINDOPC") = Me.IndOption
                End If
                If String.IsNullOrEmpty(Me.Programa) = False Then
                    dr("I_CPRGSRV") = Me.Programa
                End If
                If String.IsNullOrEmpty(Me.Condicion) = False Then
                    dr("I_NCONAFI") = Me.Condicion
                End If
                If String.IsNullOrEmpty(Me.FechaRetiro) = False Then
                    dr("I_FRET") = Me.FechaRetiro
                End If
                If String.IsNullOrEmpty(Me.CausalRetiro) = False Then
                    dr("I_CRETAFI") = Me.CausalRetiro
                End If
                If String.IsNullOrEmpty(Me.FechaFinalAfiliacion) = False Then
                    dr("I_FFINAFI") = Me.FechaFinalAfiliacion
                End If
                'Actualizacion Nilson Fuentes requisito manual estratificacion
                If String.IsNullOrEmpty(Me.Estado) = False Then
                    Dim blEstrato As Boolean = False
                    For j As Integer = 0 To 11
                        If (NameReq(j) = "MODEST") Then
                            dr("I_XESCVIN") = Me.Estado
                            blEstrato = True
                            Exit For
                        End If
                    Next
                    If (Not blEstrato) Then
                        dr("I_CESTVIN") = Me.Estado
                    End If
                End If
                dr("I_TRES002") = ObjUtil.strAsignarValor(TipoIdentEmpr, "0")
                dr("I_NRES002") = ObjUtil.strAsignarValor(NroIdentEmpr, "0")
                If String.IsNullOrEmpty(Me.Sucursal) = False Then
                    dr("I_NIDESUC") = Me.Sucursal
                End If
                If String.IsNullOrEmpty(Me.CentroCosto) = False Then
                    dr("I_NIDECOS") = Me.CentroCosto
                End If
            End With
            cle15.Tables(0).Rows.Add(dr)
            With ObjTran
                .DtsInfoTran = cle15
                Dim xmlDoc As XmlDocument
                Dim xmlnodo As XmlNode
                Dim XmlResultado As String = ""
                XmlResultado = cle15.GetXml
                xmlDoc = New XmlDocument()
                xmlDoc.LoadXml(XmlResultado)
                xmlnodo = xmlDoc.SelectSingleNode("//Mensaje")
                If (xmlnodo Is Nothing) Then
                    .DtsInfoTran = ObjUtil.GetDataSet(XmlResultado)
                    returnvalue = .EjecutaTransaccion(m_Aplicacion)
                    cle15.Clear()
                Else
                    returnvalue = XmlResultado
                End If
            End With
            Return returnvalue
        Catch Ex As Exception
            Return ObjUtil.ConvertToXMLdoc(Ex.Message)
        End Try
    End Function
#End Region

End Class

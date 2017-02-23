Imports System.Web.UI.WebControls
Imports System.Globalization
Imports System.Data
Imports System.Text
Imports System.IO
Imports ManejoMensajes
Imports EntidadesNegocio
Imports System.Web.Mail
Imports System.Configuration
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Xml.Schema

''' -----------------------------------------------------------------------------
''' Project	 : Transar
''' Class	 : CUtil
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Provee funciones utilitarias a ser utilizados en diferentes paginas de Transar
''' </summary>
''' <remarks> 
''' </remarks>
''' <history>
''' 	[CENCLOPEZB]	8/30/2004	Created
''' </history>
''' 
''' -----------------------------------------------------------------------------
Public Class CUtil
    Public Const sNewDataSet As String = "<NewDataSet></NewDataSet>"                ''4808  20140902
    Public Shared Function strEvalObject(ByVal oObjeto As Object) As String
        If oObjeto Is Nothing Then
            Return String.Empty
        Else
            Return CStr(oObjeto)
        End If
    End Function
    Public Shared Function strCompObject(ByVal oObjeto As String) As String
        If oObjeto.ToString.Length < "2" And oObjeto.ToString.Length >= "1" Then
            Return "0" & oObjeto
        Else
            Return CStr(oObjeto)
        End If
    End Function
#Region "ValidationCallBack"
    Public Shared Sub ValidationCallBack(ByVal sender As Object, ByVal args As ValidationEventArgs)
        Throw New Exception(args.Message)
    End Sub
#End Region
#Region "ValidarXSD"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' REaliza validacion de Xml vs XSD
    ''' </summary>
    ''' <param name="nXML">
    ''' contiene la trama xml a validar
    ''' </param>
    ''' <param name="RutaXSD">
    ''' Contiene la ruta donde se encuentra el XSD
    ''' el control.
    ''' </param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[DAC]	2/25/2013	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function ValidarXSD(ByVal nXML As String, ByVal RutaXSD As String) As String
        Dim respuesta As String = String.Empty
        Dim mtr As New StringReader(nXML)
        Dim tr As XmlTextReader = New XmlTextReader(mtr)
        Dim sc As XmlSchemaCollection = New XmlSchemaCollection()
        Dim vr As XmlValidatingReader = New XmlValidatingReader(tr)
        If Trim(nXML).Equals(String.Empty) Or Trim(RutaXSD).Equals(String.Empty) Then
            respuesta = "Por favor ingrese XML/XSD validos."
            Return respuesta
            Exit Function
        End If
        Try
            sc.Add(Nothing, RutaXSD)
            vr.ValidationType = ValidationType.Schema
            vr.Schemas.Add(sc)
            AddHandler vr.ValidationEventHandler, AddressOf ValidationCallBack
            While (vr.Read())
            End While
            Return respuesta
        Catch e As Exception
            respuesta = e.Message
            Return respuesta
        End Try
    End Function
#End Region


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Asigna los dias del mes de a un dropdownlist 
    ''' </summary>
    ''' <param name="lstDia">
    ''' Dropdown list a llenar
    ''' </param>
    ''' <param name="intValorSeleccionado">
    ''' valor del item que debe estar seleccionado por defecto al momento de mostrar
    ''' el control.
    ''' </param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/30/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownlistDia(ByRef lstDia As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        lstDia.DataTextField = "Descripcion"
        lstDia.DataValueField = "Codigo"
        For intCont = 0 To 31
            Dim nuevoListItem As New ListItem
            If intCont = 0 Then
                nuevoListItem.Value = intCont
                nuevoListItem.Text = "Dia"
            Else
                If intCont < 10 Then
                    nuevoListItem.Value = "0" & CStr(intCont)
                Else
                    nuevoListItem.Value = intCont
                End If
                nuevoListItem.Text = intCont
            End If
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If '
            lstDia.Items.Add(nuevoListItem)
        Next

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Devuelve el nombre del estado de afiliacion POS
    ''' </summary>
    ''' <param name="pstrEstado"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	10/14/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function DarNombreEstadoAfiliacion(ByVal pstrEstado As String) As String
        Select Case (pstrEstado)
            Case "0"
                Return "Afiliado"
            Case "1"
                Return "Retirado"
            Case "3"
                Return "No Afiliado"
            Case "4"
                Return "Carente"
            Case "5"
                Return "Suspendido"
            Case "6"
                Return "Cancelado"
            Case "7"
                Return "Protección laboral"
            Case Else

                Return ""
        End Select
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Asigna los meses del año a un dropdownlist 
    ''' </summary>
    ''' <param name="lstMes"></param>
    ''' <param name="intValorSeleccionado"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/30/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownlistMes(ByRef lstMes As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        Dim arrMeses() As String = {"Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"}

        lstMes.DataTextField = "Descripcion"
        lstMes.DataValueField = "Codigo"
        For intCont = 0 To 12
            Dim nuevoListItem As New ListItem
            If intCont = 0 Then
                nuevoListItem.Value = intCont
                nuevoListItem.Text = "Mes"
            Else
                If intCont < 10 Then
                    nuevoListItem.Value = "0" & CStr(intCont)
                Else
                    nuevoListItem.Value = intCont
                End If
                nuevoListItem.Text = arrMeses(intCont - 1)
            End If
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstMes.Items.Add(nuevoListItem)
        Next

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Convierte la fecha especificada en una variable tipo fecha bajo el formato
    ''' dd/mm/aaaa
    ''' </summary>
    ''' <param name="strFecha">
    ''' Fecha a convertir
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/30/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function aFecha(ByVal strFecha As String) As DateTime
        Dim MyCultureInfo As CultureInfo = New CultureInfo("")
        Dim dfi As DateTimeFormatInfo = New DateTimeFormatInfo
        Dim MyString As String = strFecha
        dfi.ShortDatePattern = "dd/MM/yyyy"

        Dim MyDateTime As DateTime = DateTime.ParseExact(MyString, dfi.ShortDatePattern, MyCultureInfo)
        Return MyDateTime
    End Function
    '''' -----------------------------------------------------------------------------
    '''' <summary>
    '''' Asigna los tipos de identificación manejados a un dropdownlist 
    '''' </summary>
    '''' <param name="lstControl"></param>
    '''' <param name="intValorSeleccionado">
    '''' valor del item que debe estar seleccionado por defecto al momento de mostrar
    '''' el control.
    '''' </param>
    '''' <remarks>
    '''' </remarks>
    '''' <history>
    '''' 	[CENCLOPEZB]	8/31/2004	Created
    '''' </history>
    '''' -----------------------------------------------------------------------------
    'Public Shared Sub LlenarDrowpDownTipoId(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
    '    Dim intCont As Integer
    '    Dim arrValores() As String = {"1", "4", "2", "8", "7", "3"}
    '    Dim arrEtiquetas() As String = {"Cédula de ciudadanía", "Cédula de extranjería", "Nit", "No unico de identificación personal", "Registro civil", "Tarjeta de identidad"}


    '    lstControl.DataTextField = "Descripcion"
    '    lstControl.DataValueField = "Codigo"
    '    For intCont = 0 To 5
    '        Dim nuevoListItem As New ListItem
    '        nuevoListItem.Value = arrValores(intCont)
    '        nuevoListItem.Text = arrEtiquetas(intCont)
    '        If intValorSeleccionado = intCont Then
    '            nuevoListItem.Selected = True
    '        End If
    '        lstControl.Items.Add(nuevoListItem)
    '    Next

    'End Sub


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' devuelve el nombre de un tipo de identificacion con base en un codigo dado
    ''' </summary>
    ''' <param name="intCodigo"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/3/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function darNombreTipoId(ByVal intCodigo As Integer) As String
        Dim strNombre As String

        If intCodigo = 1 Then strNombre = "CC"
        If intCodigo = 2 Then strNombre = "NIT"
        If intCodigo = 3 Then strNombre = "TI"
        If intCodigo = 4 Then strNombre = "CE"
        If intCodigo = 6 Then strNombre = "Fecha Nac."
        If intCodigo = 7 Then strNombre = "RC"
        If intCodigo = 8 Then strNombre = "NUIP"
        If intCodigo > 8 Then strNombre = "OTRO"

        Return strNombre

    End Function
    '''' -----------------------------------------------------------------------------
    '''' <summary>
    '''' devuelve el nombre de un clase de aportante con base en un codigo dado
    '''' </summary>
    '''' <param name="intCodigo"></param>
    '''' <returns></returns>
    '''' <remarks>
    '''' </remarks>
    '''' <history>
    '''' 	[CENCLOPEZB]	8/3/2005	Created
    '''' </history>
    '''' -----------------------------------------------------------------------------
    'Public Shared Function darNombreClaseAportante(ByVal intCodigo As String) As String
    '    Dim strNombre As String

    '    strNombre = intCodigo
    '    If intCodigo = "G" Then strNombre = "Grande"
    '    If intCodigo = "P" Then strNombre = "Pequeño"

    '    Return strNombre

    'End Function

    Public Function TransformaDE2400(ByVal sXmlParametros As String) As String
        ''<NewDataSet><row><tide_cliente>CC</tide_cliente><nide_cliente>26505236</nide_cliente>
        ''<nautcliEmpresa>9999803422935874</nautcliEmpresa><programa_servicio>091204</programa_servicio>
        ''<condic_afil>2</condic_afil><indic_opcion>V</indic_opcion><nuip_cliente></nuip_cliente>
        ''<nreq>DE2400</nreq><finireq>20091201</finireq><ffinreq></ffinreq><vreq></vreq>
        ''<vnumreq>800114857</vnumreq></row></NewDataSet>
        Dim dsReaderXml As New DataSet
        Dim sParametros As New StringBuilder
        Dim drVincula As DataRow
        dsReaderXml.ReadXml(New StringReader(sXmlParametros))
        sParametros.Append("<XMLClientes xmlns=""http://tempuri.org/XMLClientes.xsd"">")
        For Each drVincula In dsReaderXml.Tables(0).Rows
            sParametros.Append("<Parametros>")
            sParametros.Append("<Operacion>ADD</Operacion>")
            sParametros.Append("<TipoIdent>" & drVincula.Item("tide_cliente").ToString & "</TipoIdent>")
            sParametros.Append("<NroIdentificacion>" & drVincula.Item("nide_cliente") & "</NroIdentificacion>")
            sParametros.Append("<ParteAlfabetica />")
            sParametros.Append("<DigitoChequeo />")
            sParametros.Append("<Sucursal />")
            sParametros.Append("<Centrocosto />")
            sParametros.Append("<Programa>" & drVincula.Item("programa_servicio") & "</Programa>")
            sParametros.Append("<Condicion>" & drVincula.Item("condic_afil") & "</Condicion>")
            sParametros.Append("<Grupo />")
            sParametros.Append("<Usuario>999999999999</Usuario>")
            sParametros.Append("<EmpresaResponsable>" & drVincula.Item("vnumreq") & "</EmpresaResponsable>")
            sParametros.Append("<Requisito>" & drVincula.Item("nreq") & "</Requisito>")
            sParametros.Append("</Parametros>")
        Next
        sParametros.Append("</XMLClientes>")
        Return sParametros.ToString()
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="lstDiaNac"></param>
    ''' <param name="lstMesNac"></param>
    ''' <param name="txtAnnoNac"></param>
    ''' <param name="objPersona"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	16/09/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub SetFechaNacimientoFromList(ByRef lstDiaNac As DropDownList, ByRef lstMesNac As DropDownList, ByRef txtAnnoNac As TextBox, ByRef objPersona As CDatosPersona)
        Dim strBuilder As StringBuilder

        strBuilder = New StringBuilder(lstDiaNac.SelectedValue)
        strBuilder.Append("/")
        strBuilder.Append(lstMesNac.SelectedValue)
        strBuilder.Append("/")
        strBuilder.Append(txtAnnoNac.Text)
        objPersona.FechaNacimiento = strBuilder.ToString
    End Sub

    'Public Shared Sub LlenarDrowpDownConsultaAutoliqAportes(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
    '    Dim intCont As Integer
    '    Dim arrValores() As String = {"0", "1", "2", "3"}
    '    Dim arrEtiquetas() As String = {"Seleccione una", "Consolidado de Planillas por mes , año y estado", "Consolidado de planillas por año, mes, estado, número de planillas , total de trabajadores, valor.", "Detalle de planillas  procesadas por transar"}

    '    lstControl.DataTextField = "Descripcion"
    '    lstControl.DataValueField = "Codigo"
    '    For intCont = 0 To 3
    '        Dim nuevoListItem As New ListItem
    '        nuevoListItem.Value = arrValores(intCont)
    '        nuevoListItem.Text = arrEtiquetas(intCont)
    '        If intValorSeleccionado = intCont Then
    '            nuevoListItem.Selected = True
    '        End If
    '        lstControl.Items.Add(nuevoListItem)
    '    Next

    'End Sub


    Public Shared Sub SetFechaRetiroFromList(ByRef lstDiaRet As DropDownList, ByRef lstMesRet As DropDownList, ByRef txtAnnoRet As TextBox, ByRef objPersona As CTrabajador)
        Dim strBuilder As StringBuilder

        If lstDiaRet.SelectedValue <> "0" Then
            strBuilder = New StringBuilder(lstDiaRet.SelectedValue)
            strBuilder.Append("/")
            strBuilder.Append(lstMesRet.SelectedValue)
            strBuilder.Append("/")
            strBuilder.Append(txtAnnoRet.Text)
            objPersona.FechaRetiro = strBuilder.ToString
        Else
            objPersona.FechaRetiro = Now.ToString("dd/MM/yyyy")
        End If

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Asigna las horas del dia a un dropdownlist 
    ''' </summary>
    ''' <param name="lstHora">Dropdown list a llenar</param>
    ''' <param name="intValorSeleccionado">
    ''' valor del item que debe estar seleccionado por defecto al momento de mostrar
    ''' el control.
    ''' </param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cenclopezb]	3/29/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownlistHoras(ByRef lstHora As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        lstHora.DataTextField = "Descripcion"
        lstHora.DataValueField = "Codigo"
        For intCont = 1 To 24
            Dim nuevoListItem As New ListItem
            If intCont < 10 Then
                nuevoListItem.Value = "0" & CStr(intCont)
            Else
                nuevoListItem.Value = intCont
            End If
            nuevoListItem.Text = intCont
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstHora.Items.Add(nuevoListItem)
        Next

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Asigna las horas del dia a un dropdownlist 
    ''' </summary>
    ''' <param name="lstMinutos">Dropdown list a llenar</param>
    ''' <param name="intValorSeleccionado">
    ''' valor del item que debe estar seleccionado por defecto al momento de mostrar
    ''' el control.
    ''' </param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cenclopezb]	3/29/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownlistMinutos(ByRef lstMinutos As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        lstMinutos.DataTextField = "Descripcion"
        lstMinutos.DataValueField = "Codigo"
        For intCont = 1 To 60
            Dim nuevoListItem As New ListItem
            If intCont < 10 Then
                nuevoListItem.Value = "0" & CStr(intCont)
            Else
                nuevoListItem.Value = intCont
            End If
            nuevoListItem.Text = intCont
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstMinutos.Items.Add(nuevoListItem)
        Next

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' asigna a un drop los posibles estados de la cuenta de un usuario delegado
    ''' </summary>
    ''' <param name="lstControl"></param>
    ''' <param name="intValorSeleccionado"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	7/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownEstadoUsr(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        Dim arrValores() As String = {"1", "0"}
        Dim arrEtiquetas() As String = {"Activada", "Desactivada"}

        lstControl.DataTextField = "Descripcion"
        lstControl.DataValueField = "Codigo"
        For intCont = 0 To 1
            Dim nuevoListItem As New ListItem
            nuevoListItem.Value = arrValores(intCont)
            nuevoListItem.Text = arrEtiquetas(intCont)
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstControl.Items.Add(nuevoListItem)
        Next

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Devuelve el contenido de un dataset en una cadena de texto con un formato 
    ''' determinado.
    ''' </summary>
    ''' <param name="dsDatos">dataset con la informacion a convertir</param>
    ''' <param name="strSeparadorColumnas">caracter separador de las columnas del dataset</param>
    ''' <param name="strDatos">Cadena con la informacion del dataset y el formato aplicado</param>
    ''' <param name="generarHeader">si es true, devuelve en la cadena los nombres de las columnas
    ''' del dataset
    ''' </param>
    ''' <remarks>
    ''' Devuelve la informacion de la primera tabla del dataset.
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/16/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub datasetToFormatoTexto(ByVal dsDatos As DataSet, ByVal strSeparadorColumnas As String, ByRef strDatos As String, Optional ByVal generarHeader As Boolean = False)
        'Dim strSelected As String
        strSeparadorColumnas = ","
        Dim dt As DataTable = dsDatos.Tables(0)

        Dim sbHead As New System.Text.StringBuilder
        Dim sbBody As New System.Text.StringBuilder

        Dim columns As Object()

        If generarHeader Then
            'Write the Column Names  
            For Each dc As DataColumn In dt.Columns
                sbHead.Append(dc.ColumnName)
                sbHead.Append(strSeparadorColumnas)
            Next

            If (sbHead.Length > 0) Then
                sbHead.Remove(sbHead.Length - 1, 1).Append(vbCrLf)
            End If
        End If
        ' Write the Data Rows  
        For Each dr As DataRow In dt.Rows
            columns = dr.ItemArray
            For Each column As Object In columns
                sbBody.Append(column.ToString())
                sbBody.Append(strSeparadorColumnas)
            Next
            If (sbBody.Length > 0) Then
                sbBody.Remove(sbBody.Length - 1, 1).Append(vbCrLf)
            End If
        Next

        strDatos = sbHead.ToString() & sbBody.ToString()

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Devuelve el contenido de un dataset en una cadena de texto con un formato html
    ''' determinado entendible para excel. 
    ''' </summary>
    ''' <param name="dsDatos"></param>
    ''' <param name="strDatos"></param>
    ''' <param name="generarHeader"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cenclopezb]	11/10/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub datasetToFormatoExcel(ByVal dsDatos As DataSet, ByRef strDatos As String, Optional ByVal generarHeader As Boolean = False)
        'Dim strSelected As String
        Dim dt As DataTable = dsDatos.Tables(0)

        Dim sbHead As New System.Text.StringBuilder
        Dim sbBody As New System.Text.StringBuilder

        Dim columns As Object()

        If generarHeader Then
            'Write the Column Names  
            sbHead.Append("<tr>")
            For Each dc As DataColumn In dt.Columns
                sbHead.Append("<td  bgcolor=LightGrey>")
                sbHead.Append("<b>" & dc.ColumnName & "</b>")
                sbHead.Append("</td>")
            Next

            sbHead.Append("</tr>")

        End If
        ' Write the Data Rows  
        For Each dr As DataRow In dt.Rows
            columns = dr.ItemArray
            sbBody.Append("<tr>")
            For Each column As Object In columns
                sbBody.Append("<td>")
                sbBody.Append(column.ToString())
                sbBody.Append("</td>")
            Next
            If (sbBody.Length > 0) Then
                sbBody.Remove(sbBody.Length - 1, 1).Append(vbCrLf)
            End If
            sbBody.Append("</tr>")
        Next

        strDatos = "<table border=1>" & sbHead.ToString() & sbBody.ToString() & "</table>"

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Llena el dro de las formas de presentaciones
    ''' </summary>
    ''' <param name="lstControl"></param>
    ''' <param name="intValorSeleccionado"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	8/18/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownFormaPresentacion(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        Dim arrValores() As String = {"U", "C", "S"}
        Dim arrEtiquetas() As String = {"Uníco", "Consolidado", "Sucursal"}

        lstControl.DataTextField = "Descripcion"
        lstControl.DataValueField = "Codigo"
        For intCont = 0 To 2
            Dim nuevoListItem As New ListItem
            nuevoListItem.Value = arrValores(intCont)
            nuevoListItem.Text = arrEtiquetas(intCont)
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstControl.Items.Add(nuevoListItem)
        Next

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Consulta el tipo de afiliado
    ''' </summary>
    ''' <param name="lstControl"></param>
    ''' <param name="intValorSeleccionado"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	16/09/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownTipoAfiliado(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        Dim arrValores() As String = {"C", "B"}
        Dim arrEtiquetas() As String = {"Cotizante", "Beneficiario"}

        lstControl.DataTextField = "Descripcion"
        lstControl.DataValueField = "Codigo"
        For intCont = 0 To 1
            Dim nuevoListItem As New ListItem
            nuevoListItem.Value = arrValores(intCont)
            nuevoListItem.Text = arrEtiquetas(intCont)
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstControl.Items.Add(nuevoListItem)
        Next

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Asigna los  tipo de afiliacion a un dropdownlist
    ''' </summary>
    ''' <param name="lstControl"></param>
    ''' <param name="intValorSeleccionado"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	16/09/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownTipoAfiliacion(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        Dim arrValores() As String = {"T", "ADD", "AFI", "REA"}
        Dim arrEtiquetas() As String = {"Todos", "ADD", "AFI", "REA"}

        lstControl.DataTextField = "Descripcion"
        lstControl.DataValueField = "Codigo"
        For intCont = 0 To 3
            Dim nuevoListItem As New ListItem
            nuevoListItem.Value = arrValores(intCont)
            nuevoListItem.Text = arrEtiquetas(intCont)
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstControl.Items.Add(nuevoListItem)
        Next

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Asigna el tipo de autorizaciones a un dropdownlist
    ''' </summary>
    ''' <param name="lstControl"></param>
    ''' <param name="intValorSeleccionado"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	16/09/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownTipoAutorizaciones(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        Dim arrValores() As String = {"0", "1", "2", "3", "4", "21", "22"}
        Dim arrEtiquetas() As String = {"", "Incapacidades", "Licencias de Maternidad", "Licencias de paternidad", "Parto No viable", "Notas Débito", "Notas crédito"}

        lstControl.DataTextField = "Descripcion"
        lstControl.DataValueField = "Codigo"
        For intCont = 0 To 6
            Dim nuevoListItem As New ListItem
            nuevoListItem.Value = arrValores(intCont)
            nuevoListItem.Text = arrEtiquetas(intCont)
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstControl.Items.Add(nuevoListItem)
        Next

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Asigna el estado de una autorización a un dropdownlist
    ''' </summary>
    ''' <param name="lstControl"></param>
    ''' <param name="intValorSeleccionado"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	16/09/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownEstadoAutorizacion(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        Dim arrValores() As String = {"", "R", "E"}
        Dim arrEtiquetas() As String = {"", "Retirada", "Emitida"}

        lstControl.DataTextField = "Descripcion"
        lstControl.DataValueField = "Codigo"
        For intCont = 0 To 2
            Dim nuevoListItem As New ListItem
            nuevoListItem.Value = arrValores(intCont)
            nuevoListItem.Text = arrEtiquetas(intCont)
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstControl.Items.Add(nuevoListItem)
        Next


    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Asigna el las secciones de encuestas existentes a un dropdownlist
    ''' </summary>
    ''' <param name="lstControl"></param>
    ''' <param name="intValorSeleccionado"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	12/23/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownSeccionEncuestas(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        Dim arrValores() As String = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11"}
        Dim arrEtiquetas() As String = {"Turismo", "Crédito", "Deportes", "Salud", "Educación", "Cultura", "Recreación", "Vivienda", "Subsidio", "Empresas", "Eventos"}

        lstControl.DataTextField = "Descripcion"
        lstControl.DataValueField = "Codigo"
        For intCont = 0 To 10
            Dim nuevoListItem As New ListItem
            nuevoListItem.Value = arrValores(intCont)
            nuevoListItem.Text = arrEtiquetas(intCont)
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstControl.Items.Add(nuevoListItem)
        Next


    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Asigna los tipos de consulta de usuarios delegados a un dropdownlist
    ''' </summary>
    ''' <param name="lstControl"></param>
    ''' <param name="intValorSeleccionado"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	16/09/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownTipoConsulta(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        Dim arrValores() As String = {"0", "1", "2", "3", "4"}
        Dim arrEtiquetas() As String = {"Seleccione una", "Usuarios que no han usado la clave de transar", "Reporte  por mes, año estado  y cantidad.", "Detalle de Claves aprobadas por transar.", "Consolidado de claves por estado y cantidad."}

        lstControl.DataTextField = "Descripcion"
        lstControl.DataValueField = "Codigo"
        For intCont = 0 To 4
            Dim nuevoListItem As New ListItem
            nuevoListItem.Value = arrValores(intCont)
            nuevoListItem.Text = arrEtiquetas(intCont)
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstControl.Items.Add(nuevoListItem)
        Next
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Suminsitra el codigo del tipo de identificación
    ''' </summary>
    ''' <param name="tipodoc"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	14/10/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function darCodigoTipoId(ByVal tipodoc As String) As String
        Dim intcodigo As Integer

        If tipodoc = "CC" Then intcodigo = 1
        If tipodoc = "NI" Then intcodigo = 2
        If tipodoc = "NIT" Then intcodigo = 2
        If tipodoc = "Nit" Then intcodigo = 2
        If tipodoc = "TI" Then intcodigo = 3
        If tipodoc = "CE" Then intcodigo = 4
        If tipodoc = "Fecha Nac." Then intcodigo = 6
        If tipodoc = "NUIP" Then intcodigo = 8
        If tipodoc = "Nuip" Then intcodigo = 8
        If tipodoc = "NU" Then intcodigo = 8
        If tipodoc = "OTRO" Then intcodigo = 8
        If tipodoc = "Otro" Then intcodigo = 8
        If tipodoc = "U" Then intcodigo = 8
        If tipodoc = "N" Then intcodigo = 2
        If tipodoc = "C" Then intcodigo = 1
        If tipodoc = "T" Then intcodigo = 3
        If tipodoc = "E" Then intcodigo = 4
        If tipodoc = "C.C." Then intcodigo = 1
        If tipodoc = "RC" Then intcodigo = 7
        If tipodoc = "PA" Then intcodigo = 5
        If tipodoc = "MS" Then intcodigo = 9


        Return intcodigo

    End Function
    Public Shared Function darTipoIdCodigo(ByVal tipodoc As String) As String
        Dim codigo As String = ""

        If tipodoc = "1" Then codigo = "CC"
        If tipodoc = "2" Then codigo = "NI"
        If tipodoc = "3" Then codigo = "TI"
        If tipodoc = "4" Then codigo = "CE"
        If tipodoc = "7" Then codigo = "RC"
        If tipodoc = "8" Then codigo = "NU"
        If tipodoc = "5" Then codigo = "PA"
        If tipodoc = "9" Then codigo = "MS"

        Return codigo

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Devuelve el codigo de la opcion "nuevo tipo de usuario delegado" al momento
    ''' de crear o modificar un usuario delegado
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	1/19/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function darCodigoNuevoTipoUsudel() As String
        Return "0"
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Devuelve el codigo del perfil general definido para los usuarios delegados
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	1/19/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function darCodigoTipoUsuGeneral() As String
        Return "1"
    End Function
    ''' -----------------------------------------------------------------------------
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' devuelve la descripción de la suma de un valor (monto escrito)
    ''' </summary>
    ''' <param name="pValor"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[RMORERAR]	6/6/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function darMontoEscrito(ByVal pValor As String) As String
        Dim strNombre, wCadena As String
        Dim wTercio, wNumero As String
        Dim wNumcar, wPosicion, I As Integer
        Dim w16 As Boolean

        pValor = Trim(Str(Val(pValor)))
        wNumcar = Len(pValor) Mod 3
        If wNumcar = 1 Then pValor = "00" + pValor
        If wNumcar = 2 Then pValor = "0" + pValor
        wNumcar = Len(pValor) \ 3
        I = 1

        Do Until I > wNumcar
            'Primera Posición
            wTercio = ""
            wPosicion = Len(pValor) - (I * 3) + 1
            wNumero = fNumero(Mid$(pValor, wPosicion, 1))
            Select Case Mid$(pValor, wPosicion, 1)
                Case Is = "0"
                    wTercio = wTercio + ""
                Case Is = "1"
                    If Mid$(pValor, wPosicion + 1, 1) <> "0" Or Mid$(pValor, wPosicion + 2, 1) <> "0" Then
                        wTercio = "CIENTO " + wTercio
                    Else
                        wTercio = "CIEN " + wTercio
                    End If
                Case Is = "5"
                    wTercio = wTercio + "QUINIENTOS "
                Case Is = "9"
                    wTercio = wTercio + "NOVECIENTOS "
                Case Is = "7"
                    wTercio = wTercio + "SETECIENTOS "
                Case Else
                    wTercio = wTercio + wNumero + "CIENTOS "
            End Select

            'Segunda Posición
            w16 = False
            wNumero = fNumero(Mid$(pValor, wPosicion + 1, 1))
            Select Case Mid$(pValor, wPosicion + 1, 1)
                Case Is = "0"
                    wTercio = wTercio + ""
                Case Is = "1"
                    If Mid$(pValor, wPosicion + 2, 1) <= "5" Then
                        wNumero = fNumero(Mid$(pValor, wPosicion + 1, 2))
                        wTercio = wTercio + wNumero
                        w16 = True
                    Else
                        wTercio = wTercio + "DIECI"
                        w16 = False
                    End If
                Case Is = "2"
                    If Mid$(pValor, wPosicion + 2, 1) <> "0" Then
                        wTercio = wTercio + "VENTI"
                    Else
                        wNumero = fDecada(Mid$(pValor, wPosicion + 1, 1))
                        wTercio = wTercio + wNumero
                    End If
                Case Else
                    wNumero = fDecada(Mid$(pValor, wPosicion + 1, 1))
                    If Mid$(pValor, wPosicion + 2, 1) <> "0" Then
                        wTercio = wTercio + wNumero + " Y "
                    Else
                        wTercio = wTercio + wNumero
                    End If
            End Select

            'Tercera Posición
            If Not w16 Then
                Select Case Mid$(pValor, wPosicion + 2, 1)
                    Case Is = "0"
                        wTercio = wTercio + ""
                    Case Is = "1"
                        If I = 1 Then
                            wTercio = wTercio + "UNO"
                        Else
                            wTercio = wTercio + "UN"
                        End If
                    Case Else
                        wNumero = fNumero(Mid$(pValor, wPosicion + 2, 1))
                        wTercio = wTercio + wNumero
                End Select
            End If

            'Miles
            If I = 1 Or I = 3 Then
                If wPosicion > 3 Then
                    If Val(Mid$(pValor, wPosicion - 3, 3)) > 0 Then
                        wCadena = " MIL " + wTercio + wCadena
                    Else
                        wCadena = wTercio + wCadena
                    End If
                Else
                    wCadena = wTercio + wCadena
                End If
            End If

            'Millones
            If I = 2 Then
                If wPosicion > 2 Then
                    Select Case Val(Mid$(pValor, wPosicion - 2, 2))
                        Case Is = 1
                            wCadena = " MILLON " + wTercio + wCadena
                        Case Else
                            wCadena = " MILLONES " + wTercio + wCadena
                    End Select
                Else
                    wCadena = wTercio + wCadena
                End If
            End If

            'Billones
            If I = 4 Then
                If wPosicion > 2 Then
                    Select Case Val(Mid$(pValor, wPosicion - 2, 2))
                        Case Is = 1
                            wCadena = " BILLON " + wTercio + wCadena
                        Case Else
                            wCadena = " BILLONES " + wTercio + wCadena
                    End Select
                Else
                    wCadena = wTercio + wCadena
                End If
            End If
            If I = 5 Then
                wCadena = wTercio + wCadena
            End If
            I = I + 1
        Loop

        Return wCadena

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' devuelve la descripción de la Decena (monto escrito)
    ''' </summary>
    ''' <param name="pNumero"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[RMORERAR]	6/6/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function fDecada$(ByVal pNumero As String)

        Select Case pNumero
            Case Is = "1"
                fDecada$ = "DIEZ"
            Case Is = "2"
                fDecada$ = "VEINTE"
            Case Is = "3"
                fDecada$ = "TREINTA"
            Case Is = "4"
                fDecada$ = "CUARENTA"
            Case Is = "5"
                fDecada$ = "CINCUENTA"
            Case Is = "6"
                fDecada$ = "SESENTA"
            Case Is = "7"
                fDecada$ = "SETENTA"
            Case Is = "8"
                fDecada$ = "OCHENTA"
            Case Is = "9"
                fDecada$ = "NOVENTA"
        End Select
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' devuelve la descripción de las Unidades (monto escrito)
    ''' </summary>
    ''' <param name="pNumero"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[RMORERAR]	6/6/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function fNumero$(ByVal pNumero As String)

        Select Case pNumero
            Case Is = "0"
                fNumero$ = "CERO"
            Case Is = "1"
                fNumero$ = "UNO"
            Case Is = "2"
                fNumero$ = "DOS"
            Case Is = "3"
                fNumero$ = "TRES"
            Case Is = "4"
                fNumero$ = "CUATRO"
            Case Is = "5"
                fNumero$ = "CINCO"
            Case Is = "6"
                fNumero$ = "SEIS"
            Case Is = "7"
                fNumero$ = "SIETE"
            Case Is = "8"
                fNumero$ = "OCHO"
            Case Is = "9"
                fNumero$ = "NUEVE"
            Case Is = "10"
                fNumero$ = "DIEZ"
            Case Is = "11"
                fNumero$ = "ONCE"
            Case Is = "12"
                fNumero$ = "DOCE"
            Case Is = "13"
                fNumero$ = "TRECE"
            Case Is = "14"
                fNumero$ = "CATORCE"
            Case Is = "15"
                fNumero$ = "QUINCE"
        End Select

        Return fNumero$
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da el nombre del estado de un afiliado en la cooperativa
    ''' </summary>
    ''' <param name="pstrEstado"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[extcenjdcastrom]	28/12/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function DarNombreEstadoAfiliadoCooperativa(ByVal pstrEstado As String) As String
        Select Case (pstrEstado)
            Case "1"
                Return "Activo"
            Case "2"
                Return " En(prenotificación)"
            Case "3"
                Return "Rechazado"
            Case Else

                Return ""
        End Select
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Llena un drop con los tipos de persona para realizar el pago
    ''' </summary>
    ''' <param name="lstControl"></param>
    ''' <param name="intValorSeleccionado"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[EXTCENJDCASTROM]	04/01/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub LlenarDrowpDownTipoPersona(ByRef lstControl As DropDownList, Optional ByVal intValorSeleccionado As Integer = 0)
        Dim intCont As Integer
        Dim arrValores() As String = {"1", "0"}
        Dim arrEtiquetas() As String = {"Persona Juridica", "Persona natural"}

        lstControl.DataTextField = "Descripcion"
        lstControl.DataValueField = "Codigo"
        For intCont = 0 To 1
            Dim nuevoListItem As New ListItem
            nuevoListItem.Value = arrValores(intCont)
            nuevoListItem.Text = arrEtiquetas(intCont)
            If intValorSeleccionado = intCont Then
                nuevoListItem.Selected = True
            End If
            lstControl.Items.Add(nuevoListItem)
        Next

    End Sub

    Public Shared Function darNombreTipoPersona(ByVal intCodigo As Integer) As String
        Dim strNombre As String

        If intCodigo = 2 Then
            strNombre = "Persona Jurídica"
        Else
            strNombre = "Persona Natural"
        End If
        Return strNombre

    End Function

#Region "Declaraciones"
    Dim m_AccReq As String
    Dim m_NameReq As String
    Dim m_FechIniReq As String
    Dim m_FechFinReq As String
    Dim m_DatNumReq As String
    Dim m_DatAlfReq As String
    Dim m_InfoAlfa As String
    Dim m_pos As Integer
    Dim m_Aplicacion As String
#End Region

#Region "Propiedades"
    Property AccReq() As String
        Get
            Return m_AccReq
        End Get
        Set(ByVal value As String)
            m_AccReq = value
        End Set
    End Property

    Property NameReq() As String
        Get
            Return m_NameReq
        End Get
        Set(ByVal value As String)
            m_NameReq = value
        End Set
    End Property

    Property FechIniReq() As String
        Get
            Return m_FechIniReq
        End Get
        Set(ByVal value As String)
            m_FechIniReq = value
        End Set
    End Property

    Property FechFinReq() As String
        Get
            Return m_FechFinReq
        End Get
        Set(ByVal value As String)
            m_FechFinReq = value
        End Set
    End Property

    Property DatNumReq() As String
        Get
            Return m_DatNumReq
        End Get
        Set(ByVal value As String)
            m_DatNumReq = value
        End Set
    End Property

    Property DatAlfReq() As String
        Get
            Return m_DatAlfReq
        End Get
        Set(ByVal value As String)
            m_DatAlfReq = value
        End Set
    End Property

    Property InfoAlfa() As String
        Get
            Return m_InfoAlfa
        End Get
        Set(ByVal value As String)
            m_InfoAlfa = value
        End Set
    End Property

    Property Pos() As Integer
        Get
            Return m_pos
        End Get
        Set(ByVal value As Integer)
            m_pos = value
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
#End Region

#Region "Metodos"

    Public Function GetDataSet(ByVal xml As String) As DataSet
        Dim DataRF As New DataSet()
        Dim xmlData As String = xml
        If xmlData.Contains("encoding") = 0 Then    '4808 20140909
            xmlData = "<?xml version='1.0' encoding='ISO-8859-1'?>" & xmlData
        End If

        Dim s As System.IO.Stream = New System.IO.MemoryStream(System.Text.ASCIIEncoding.Default.GetBytes(xmlData))
        DataRF.ReadXml(s)
        s.Close()
        Return (DataRF)
    End Function

    Public Function EvalDataSet(ByVal pDs As DataSet) As Boolean
        Dim crrValues As String
        Dim ColumnName As String
        Dim ColumnData As String
        Dim vCont As Integer
        Dim vCtl As Integer
        vCont = 1
        crrValues = String.Empty
        ColumnName = "DatoAlfabetico"
        vCtl = 0

        For Each DatCol As DataColumn In pDs.Tables(0).Columns
            If DatCol.Caption.StartsWith(ColumnName) Then
                ColumnData = ColumnName + CStr(vCont)
                crrValues = pDs.Tables(0).Rows(0).Item(ColumnData).ToString

                If crrValues.Length > 30 Then
                    vCtl = vCtl + 1
                End If

                If vCtl > 1 Then
                    Return True
                End If

                vCont = vCont + 1
            End If
        Next
        Return False

    End Function

    Public Function strAsignarValor(ByVal strVariable As String, ByVal strValor As String) As String
        If strVariable = "" Then
            strAsignarValor = strValor
        Else
            strAsignarValor = strVariable
        End If
    End Function

    Public Function ConvertToXMLdoc(ByVal infodat As String) As String
        infodat = "<?xml version=""1.0"" encoding=""utf-8""?><string><NewDataSet><Table><Mensaje>" & infodat & "</Mensaje></Table></NewDataSet></string>"
        Return infodat
    End Function

    Public Function Buildxmlrequisito() As String
        Dim returnvalue As String = ""
        returnvalue = returnvalue & IIf(AccReq <> "", "<I_XACC" & CStr(Pos + 1) & ">" & AccReq & "</I_XACC" & CStr(Pos + 1) & ">", "<I_XACC" & CStr(Pos + 1) & " />") & _
                                    IIf(AccReq <> "", "<I_XNMNCA" & CStr(Pos + 1) & ">" & NameReq & "</I_XNMNCA" & CStr(Pos + 1) & ">", "<I_XNMNCA" & CStr(Pos + 1) & " />") & _
                                  "<I_FINIVI" & CStr(Pos + 1) & ">" & strAsignarValor(FechIniReq, "0") & "</I_FINIVI" & CStr(Pos + 1) & ">" & _
                                  "<I_FFINVI" & CStr(Pos + 1) & ">" & strAsignarValor(FechFinReq, "0") & "</I_FFINVI" & CStr(Pos + 1) & ">" & _
                                  "<I_VNUMDA" & CStr(Pos + 1) & ">" & strAsignarValor(DatNumReq, "0") & "</I_VNUMDA" & CStr(Pos + 1) & ">" & _
                                  IIf(DatAlfReq <> "", "<I_XALFDA" & CStr(Pos + 1) & ">" & DatAlfReq & "</I_XALFDA" & CStr(Pos + 1) & ">", "<I_XALFDA" & CStr(Pos + 1) & " />")
        Return returnvalue
    End Function

    Public Function BuildInfoAdirequisito() As String
        Dim returnvalue As String = ""
        If InfoAlfa.Length > 30 Then
            returnvalue = returnvalue & "<I_XALFADI>" & m_InfoAlfa.Substring(30, m_InfoAlfa.Length - 30) & "</I_XALFADI>"
            InfoAlfa = InfoAlfa.Substring(0, 29)
        End If
        Return returnvalue
    End Function

    Public Sub WriteLogEvent(ByVal mensaje As String)
        Try
            If m_Aplicacion Is Nothing Or String.IsNullOrEmpty(m_Aplicacion) Then
                m_Aplicacion = ConfigurationSettings.AppSettings("AfilPos_ProjectID")
            End If
            If Not System.Diagnostics.EventLog.SourceExists(m_Aplicacion) Then
                System.Diagnostics.EventLog.CreateEventSource(m_Aplicacion, ConfigurationSettings.AppSettings("NombreLog"))
            End If
            ' Create an EventLog instance and assign its source.
            Dim myLog As New System.Diagnostics.EventLog
            myLog.Source = m_Aplicacion
            ' Write an informational entry to the event log.    
            myLog.WriteEntry(mensaje)
        Catch ex As Exception
        End Try
    End Sub
    ''4808  20140902
#Region "DataSetResultado"
    Public Shared Function DataSetResultado(ByVal sXML As String) As String
        If sXML.Equals(String.Empty) Then
            Return sNewDataSet
        Else
            Return sXML
        End If
    End Function
    Public Shared Function xmlDataSetResultado(ByVal iopc As Integer, ByVal sObs As String) As String
        Dim sRes As String = "EXITOSA"
        If iopc = 0 Then
            sRes = "FALLIDA"
        End If
        xmlDataSetResultado = "<NewDataSet><Table><Respuesta>TRANSACCION " & sRes & "</Respuesta><Observacion>" & sObs & "</Observacion></Table></NewDataSet>"

        If xmlDataSetResultado.Equals(String.Empty) Then
            Return sNewDataSet
        End If
    End Function
    '4249 20150417
    Public Shared Function xmlDataSetResultadoMsje(ByVal iopc As Integer, ByVal sObs As String) As String
        Dim sRes As String = "EXITOSA"
        If iopc = 0 Then
            sRes = "FALLIDA"
        End If
        xmlDataSetResultadoMsje = "<NewDataSet><Table><Respuesta>TRANSACCION " & sRes & "</Respuesta><Mensaje>" & sObs & "</Mensaje></Table></NewDataSet>"

        If xmlDataSetResultadoMsje.Equals(String.Empty) Then
            Return sNewDataSet
        End If
    End Function

#End Region
    ''4808  20140902

#End Region

#Region "Validaciones Aldea"

    Public Function GetEquivTipoDoc(ByVal tipodoc As String) As String
        Dim returnvalue As String = ""
        Dim TipD As String = ""
        Dim TipSch As Integer = IIf(IsNumeric(tipodoc) = True, 0, 1)
        If TipSch = 0 Then 'dado un dato numerico obtiene el equivalente en letras
            TipD = darTipoIdCodigo(tipodoc)
            If TipD <> "" Then
                returnvalue = TipD
            Else
                returnvalue = tipodoc
            End If
        Else 'dado letras se obtiene el equivalente en numero
            TipD = darCodigoTipoId(tipodoc)
            If TipD <> "" Then
                returnvalue = TipD
            Else
                returnvalue = tipodoc
            End If
        End If
        Return returnvalue
    End Function

#End Region

#Region "Validaciones Xml"
    Public Function gValidaTags(ByVal pStrXml As String) As String
        'Dim pTag As String
        Dim strTags() As String = {"CargoTrabajador", "Nombres", "PrimerApellido", "SegundoApellido", "Direccion", "Barrio", _
                "DatoAlfabetico1", "DatoAlfabetico2", "DatoAlfabetico3", "DatoAlfabetico4", "DatoAlfabetico5", "DatoAlfabetico6", "DatoAlfabetico7", "DatoAlfabetico8", "DatoAlfabetico9", "DatoAlfabetico10", "DatoAlfabetico11", "DatoAlfabetico12"}
        gValidaTags = ""
        For i As Integer = 0 To strTags.Length - 1
            If gValidaTagEspeciales(pStrXml, strTags(i)) Then
                gValidaTags = gValidaTags & "Error: Validar caracteres especiales en TAG:" & strTags(i) & ". "
            End If
        Next i
    End Function
    '2702
    Public Function gValidaTagEspeciales(ByVal pStrXml As String, ByVal pTag As String) As Boolean
        '// From xml spec valid chars: 
        '// #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
        '// any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
        Dim re As String = "[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-u10FFFF|^<|^>]"  '@
        'Dim re As String = "[^A-Za-z0-9| |&|<|'|>]"  '@& < > " '
        'Dim pattern As String = "[^A-Za-z0-9| |Ñ|n]"
        Dim mValorIni As Integer, mValorFin As Integer, mValor As String, mValorClean As String, mLen As Integer

        mValorIni = pStrXml.IndexOf("<" & pTag & ">")
        mValorFin = pStrXml.IndexOf("</" & pTag & ">")
        mLen = Len("<" & pTag & ">")
        If mValorIni < 0 Then
            Return False
        End If
        mValor = pStrXml.Substring(mValorIni + mLen, mValorFin - mLen - mValorIni)
        mValorClean = mValor.Replace(">", "")
        mValorClean = mValorClean.Replace("<", "")
        'mValorClean = RegularExpressions.Regex.Replace(mValor, re, "")

        If mValorClean = mValor Then
            Return False
        End If

        Return True
    End Function
#End Region

End Class


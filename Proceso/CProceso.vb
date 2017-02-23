#Region "Namespaces"
Imports EntidadesNegocio
Imports ManejoMensajes
Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports System.Xml.Serialization
Imports System.Text
Imports System.Configuration

#End Region

''' -----------------------------------------------------------------------------
''' Project	 : Proceso
''' Class	 : CProceso
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Esta clase abstracta, provee la estrucutra básica que deben seguir los diferentes 
''' procesos que implementan la funcionalidad de Transar.
''' </summary>
''' <remarks>
''' Transar, se ha dividido como un conjunto de funcionalidades, que implementan los
''' requerimientos funcionales de los diferentes módulos. 
''' Para implementar dicha funcionalidad, cada uno de estos conjuntos se ha modelado 
''' como una clase orquestadora que expone los métodos y operaciones básicas que debe 
''' realizar cada módulo. 
''' Este método de desarrollo implementa el patrón Strategy.
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	1/17/2005	Created
''' </history>
''' -----------------------------------------------------------------------------
Public MustInherit Class CProceso
#Region " Constructores"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor por defecto de la super clase. 
    ''' Se encarga de inicializar el objeto estático lógicaTransaccional
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	1/17/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub New()
        Me.objlogicatransaccional = New LogicaTransaccional.ManejadorTransacciones
    End Sub

#End Region

#Region " Propiedades "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Mediante esta propiedad, las clases que hacen uso de las herederas de CProceso,
    ''' pueden determinar la descripción de un error que haya ocurrido al interior de 
    ''' de alguna de ellas.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	1/17/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public ReadOnly Property DescripcionError() As String
        Get
            Return mstrError
        End Get
    End Property

#End Region

#Region " Miembros Protegidos"
    Protected mstrError As String
    Protected objlogicatransaccional As LogicaTransaccional.ManejadorTransacciones
#End Region

#Region "Funciones con Implementacion"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta función trae la implementación básica para la consulta de un trabajador.
    ''' </summary>
    ''' <param name="trabajador">Objeto de tipo Ctrabajador que se quiere consultar,
    ''' y en el cuál se van a dejar los resultados de la consulta
    ''' </param>
    ''' <returns>
    ''' En las clases que extiendan a CProceso, es posible hacerle override, para que
    ''' efectúe las consultas que se requieran.
    ''' </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overridable Function Consultar(ByRef trabajador As CDatosPersona) As Boolean
        Dim strError As String = ""
        Try
            If trabajador.GetType.FullName = "EntidadesNegocio.CTrabajador" Then
                'Dim WS As New Servicios.AF.AFI
                Dim strRespuesta As String
                Dim objTemp_Tra As EntidadesNegocio.CTrabajador
                Dim strAplicacion As String
                Dim strbuilder As StringBuilder

                objTemp_Tra = CType(trabajador, EntidadesNegocio.CTrabajador)
                If objTemp_Tra.TipoIdentificacion Is Nothing Or objTemp_Tra.IdTrabajador Is Nothing Then
                    CManejadorMensajes.DarMensaje("2", strError)
                    strBuilder = New StringBuilder("Proceso::CProcesoAfiliarCotizante::Consultar")
                    strBuilder.Append(strError)
                    strBuilder.Append("objTemp_Be.NumeroIdentificacion,objTemp_Be.TipoIdentificacion ,objTemp_Be.IdTrabajador o objTemp_Be.TipoId son vacios ")
                    CManejadorMensajes.PublicarMensaje(strBuilder.ToString(), EventLogEntryType.Warning)
                    Me.mstrError = strError
                    Return False
                Else
                    If ConfigurationSettings.AppSettings.HasKeys() Then
                        strAplicacion = ConfigurationSettings.AppSettings("IdAplicacion")
                    Else
                        strAplicacion = "SWPR57"
                    End If

                    Dim objTrabAutenticado As New CTrabajador
                    objTrabAutenticado.EmpresaPrimaria.IdEmpresa = IIf(objTemp_Tra.EmpresaPrimaria.IdEmpresa Is Nothing, "", objTemp_Tra.EmpresaPrimaria.IdEmpresa)
                    objTrabAutenticado.EmpresaPrimaria.TipoIdentificacion = IIf(objTemp_Tra.EmpresaPrimaria.TipoIdentificacion Is Nothing, "", objTemp_Tra.EmpresaPrimaria.TipoIdentificacion)

                    strRespuesta = "" 'WS.Afiliado(strAplicacion, Double.Parse(objTemp_Tra.IdTrabajador), Integer.Parse(objTemp_Tra.TipoIdentificacion), 0)
                    If StringToCTrabajador(strRespuesta, objTemp_Tra) Then

                        'Valida que el trabajador consultado exista en la empresa autenticada en transar
                        If ValidarTrabajadorEmpresa(objTemp_Tra, objTrabAutenticado) Then
                            trabajador = objTemp_Tra
                            Return True
                        Else
                            CManejadorMensajes.DarMensaje("3", strError)
                            Me.mstrError = strError
                            Return False
                        End If

                    Else
                        CManejadorMensajes.DarMensaje("3", strError)
                        Me.mstrError = strError
                        Return False
                    End If
                End If ' objTemp_Tra.TipoIdentificacion Is Nothing Or objTemp_Tra.IdTrabajador Is Nothing
            Else 'trabajador.GetType.FullName = "EntidadesNegocio.CTrabajador" 
                Dim strBuilder As StringBuilder
                CManejadorMensajes.DarMensaje("1", strError)
                strBuilder = New StringBuilder("Proceso::CProcesoAfiliarCotizante::Consultar ")
                strBuilder.Append(strError)
                strBuilder.Append(" Tipo Recibido:")
                strBuilder.Append(trabajador.GetType.FullName)
                strBuilder.Append(" Tipo Esperado: EntidadesNegocio.CTrabajador")
                CManejadorMensajes.PublicarMensaje(strBuilder.ToString(), EventLogEntryType.Error)
                Me.mstrError = strError
                Return False
            End If
        Catch ex As Exception
            CManejadorMensajes.PublicarMensaje(ex.ToString(), EventLogEntryType.Error)
            CManejadorMensajes.DarMensaje("3", strError)
            Me.mstrError = strError
            Return False
        End Try
    End Function

    Public Overridable Function ConsultarAfiliaciones(ByRef trabajador As CTrabajador) As Boolean
        Dim strError As String = ""
        Try
            Dim strRespuesta, strAplicacion As String
            'Dim WS As New Servicios.AF.AFI
            If ConfigurationSettings.AppSettings.HasKeys() Then
                strAplicacion = ConfigurationSettings.AppSettings("IdAplicacion")
            Else
                strAplicacion = "SWPR57"
            End If
            Dim objTemp_Tra As EntidadesNegocio.CTrabajador
            objTemp_Tra = CType(trabajador, EntidadesNegocio.CTrabajador)
            strRespuesta = "" 'WS.Afiliado(strAplicacion, Double.Parse(objTemp_Tra.IdTrabajador), Integer.Parse(objTemp_Tra.TipoIdentificacion), 8)
            Dim row As DataRow
            Dim ds As DataSet = New DataSet
            Dim st = New System.IO.StringReader(strRespuesta)
            ds.ReadXml(st)

            If ds.Tables.Count > 0 Then
                For Each row In ds.Tables(0).Rows
                    'If row("CESTCAJ") = 0 Then
                    objTemp_Tra.EmpresaPrimaria.EstadoAfiliacionCaja = row("CESTCAJ")
                    objTemp_Tra.Categoria = row("CCAT")
                    If trabajador.Vacio Then
                        objTemp_Tra.Nombres = row("XNOM")
                        objTemp_Tra.PrimerApellido = row("XPRIAPE")
                        objTemp_Tra.SegundoApellido = row("XSEGAPE")
                        objTemp_Tra.FechaNacimiento = row("FNAC")
                    End If
                    'End If
                Next
            End If

            trabajador = objTemp_Tra
            Return True
        Catch ex As Exception
            CManejadorMensajes.PublicarMensaje(ex.ToString(), EventLogEntryType.Error)
            CManejadorMensajes.DarMensaje("3", strError)
            Me.mstrError = strError
            Return False
        End Try
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Se encarga de hacer un mapeo entre una cadena xml que tiene las propiedades
    ''' de un trabajador y un objeto CTrabajador como tal.
    ''' </summary>
    ''' <param name="p_strCadena">Cadena XML con los atributos del trabajador</param>
    ''' <param name="trabajador"> Objeto trabajador, de entrada y salida, en el que 
    ''' se van a mapear las propiedades especificadas en p_strCadena</param>
    ''' <returns>
    ''' True si se pudo hacer la conversión 
    ''' False si hubo errores
    ''' </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	19/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function StringToCTrabajador(ByVal p_strCadena As String, ByRef trabajador As CTrabajador) As Boolean

        Try

            Dim strError As String = ""
            Dim strBuilder As StringBuilder
            Dim dsRespuestaWS As DataSet
            Dim intI As Integer


            If p_strCadena Is Nothing Or p_strCadena = "" Then
                CManejadorMensajes.DarMensaje("4", strError)
                strBuilder = New StringBuilder(strError)
                strBuilder.Append("p_strCadena = Vacio")
                CManejadorMensajes.PublicarMensaje(strBuilder.ToString(), EventLogEntryType.Error)
                Return False
            Else

                dsRespuestaWS = New DataSet
                dsRespuestaWS.ReadXml(New StringReader(p_strCadena))
                If dsRespuestaWS.Tables.Count = 0 Then
                    trabajador.EmpresaPrimaria.EstadoAfiliacionCaja = -1
                    trabajador.EmpresaPrimaria.EstadoAfiliacionPOS = -1
                    trabajador.EmpresaPrimaria.TipoVinculo = "1"
                    Return True
                Else
                    trabajador.Genero = IIf(dsRespuestaWS.Tables(0).Columns.Contains("XSEXTRA"), dsRespuestaWS.Tables(0).Rows(0)("XSEXTRA"), "F")
                    trabajador.Nombres = IIf(dsRespuestaWS.Tables(0).Columns.Contains("XNOMTRA"), dsRespuestaWS.Tables(0).Rows(0)("XNOMTRA"), "")
                    trabajador.PrimerApellido = IIf(dsRespuestaWS.Tables(0).Columns.Contains("XPRIAPE"), dsRespuestaWS.Tables(0).Rows(0)("XPRIAPE"), "")
                    trabajador.SegundoApellido = IIf(dsRespuestaWS.Tables(0).Columns.Contains("XSEGAPE"), dsRespuestaWS.Tables(0).Rows(0)("XSEGAPE"), "")
                    trabajador.FechaNacimiento = IIf(dsRespuestaWS.Tables(0).Columns.Contains("FNACTRA"), dsRespuestaWS.Tables(0).Rows(0)("FNACTRA"), "")
                    trabajador.Edad = IIf(dsRespuestaWS.Tables(0).Columns.Contains("EDAD"), dsRespuestaWS.Tables(0).Rows(0)("EDAD"), 0)

                    trabajador.Ciudad = IIf(dsRespuestaWS.Tables(0).Columns.Contains("CCIU"), dsRespuestaWS.Tables(0).Rows(0)("CCIU"), "0")
                    trabajador.Direccion = IIf(dsRespuestaWS.Tables(0).Columns.Contains("XDIR"), dsRespuestaWS.Tables(0).Rows(0)("XDIR"), "")
                    trabajador.EstadoCivil = IIf(dsRespuestaWS.Tables(0).Columns.Contains("CESTCIV"), dsRespuestaWS.Tables(0).Rows(0)("CESTCIV"), "")
                    trabajador.CodigoOcupacion = IIf(dsRespuestaWS.Tables(0).Columns.Contains("COCUTRA"), dsRespuestaWS.Tables(0).Rows(0)("COCUTRA"), 0)

                    trabajador.FechaAfiliacionSistema = IIf(dsRespuestaWS.Tables(0).Columns.Contains("FAFISSS"), dsRespuestaWS.Tables(0).Rows(0)("FNACTRA"), "")
                    trabajador.CabezaFamilia = IIf(dsRespuestaWS.Tables(0).Columns.Contains("CCABFAM"), IIf(dsRespuestaWS.Tables(0).Columns.Contains("CCABFAM") = "1", True, False), False)
                    trabajador.CodigoEPSAnterior = IIf(dsRespuestaWS.Tables(0).Columns.Contains("CEPSANT"), dsRespuestaWS.Tables(0).Rows(0)("CEPSANT"), 0)

                    If dsRespuestaWS.Tables(0).Columns.Contains("I_NCEDCYG") Then
                        trabajador.NumeroIdentificacionConyuge = dsRespuestaWS.Tables(0).Rows(0)("I_NCEDCYG")
                    End If
                    If dsRespuestaWS.Tables(0).Columns.Contains("I_NFIC") Then
                        trabajador.TipoIdentificacionConyuge = dsRespuestaWS.Tables(0).Rows(0)("I_NFIC")
                    End If
                    If dsRespuestaWS.Tables(0).Columns.Contains("I_VSALCYG") Then
                        trabajador.SalarioConyuge = dsRespuestaWS.Tables(0).Rows(0)("I_VSALCYG")
                    End If
                    If (dsRespuestaWS.Tables(0).Columns.Contains("CCAT")) Then
                        trabajador.Categoria = dsRespuestaWS.Tables(0).Rows(0)("CCAT")
                    End If
                    If (dsRespuestaWS.Tables(0).Columns.Contains("NTELUNO")) Then
                        trabajador.Telefono = dsRespuestaWS.Tables(0).Rows(0)("NTEL")
                    End If
                    ' Empresa Primaria                   
                    If dsRespuestaWS.Tables(0).Columns.Contains("XCAR") Then
                        trabajador.EmpresaPrimaria.CargoTrabajador = dsRespuestaWS.Tables(0).Rows(0)("XCAR")
                    Else
                        trabajador.EmpresaPrimaria.CargoTrabajador = ""
                    End If
                    If dsRespuestaWS.Tables(0).Columns.Contains("CCENCOS") Then
                        trabajador.EmpresaPrimaria.CentroDeCostos = dsRespuestaWS.Tables(0).Rows(0)("CCENCOS")
                    End If
                    If dsRespuestaWS.Tables(0).Columns.Contains("CDEP") Then
                        trabajador.EmpresaPrimaria.Dependencia = dsRespuestaWS.Tables(0).Rows(0)("CDEP")
                    End If
                    trabajador.EmpresaPrimaria.EstadoAfiliacionCaja = IIf(dsRespuestaWS.Tables(0).Columns.Contains("CESTCAJ"), dsRespuestaWS.Tables(0).Rows(0)("CESTCAJ"), -1)
                    trabajador.EmpresaPrimaria.EstadoAfiliacionPOS = IIf(dsRespuestaWS.Tables(0).Columns.Contains("CESTPOS"), dsRespuestaWS.Tables(0).Rows(0)("CESTPOS"), -1)
                    trabajador.EmpresaPrimaria.EstadoMultiafiliacionPOS = IIf(dsRespuestaWS.Tables(0).Columns.Contains("CGEN001"), dsRespuestaWS.Tables(0).Rows(0)("CGEN001"), -1)

                    If dsRespuestaWS.Tables(0).Columns.Contains("FINGTRA") Then
                        trabajador.EmpresaPrimaria.FechaIngreso = dsRespuestaWS.Tables(0).Rows(0)("FINGTRA")
                    End If
                    If dsRespuestaWS.Tables(0).Columns.Contains("FINGPOS") Then
                        trabajador.EmpresaPrimaria.FechaIngresoPOS = dsRespuestaWS.Tables(0).Rows(0)("FINGPOS")
                    End If
                    If dsRespuestaWS.Tables(0).Columns.Contains("FRETTRA") Then
                        If Not Convert.IsDBNull(dsRespuestaWS.Tables(0).Rows(0)("FRETTRA")) Then
                            trabajador.EmpresaPrimaria.FechaRetiro = dsRespuestaWS.Tables(0).Rows(0)("FRETTRA")
                        End If
                    End If
                    If dsRespuestaWS.Tables(0).Columns.Contains("NHORCON") Then
                        trabajador.EmpresaPrimaria.HorasLaborMes = dsRespuestaWS.Tables(0).Rows(0)("NHORCON")
                    End If
                    trabajador.EmpresaPrimaria.IdEmpresa = IIf(dsRespuestaWS.Tables(0).Columns.Contains("NIDEEMP"), dsRespuestaWS.Tables(0).Rows(0)("NIDEEMP"), "")
                    trabajador.EmpresaPrimaria.Nombre = IIf(dsRespuestaWS.Tables(0).Columns.Contains("XNOM"), dsRespuestaWS.Tables(0).Rows(0)("XNOM"), "")
                    If dsRespuestaWS.Tables(0).Columns.Contains("VSUETRA") Then
                        trabajador.EmpresaPrimaria.SalarioCaja = dsRespuestaWS.Tables(0).Rows(0)("VSUETRA")
                        trabajador.EmpresaPrimaria.SalarioPOS = dsRespuestaWS.Tables(0).Rows(0)("VSUETRA")
                    End If
                    If dsRespuestaWS.Tables(0).Columns.Contains("TIDEEMP") Then
                        If dsRespuestaWS.Tables(0).Rows(0)("TIDEEMP") <> "" Then
                            trabajador.EmpresaPrimaria.TipoIdentificacion = dsRespuestaWS.Tables(0).Rows(0)("TIDEEMP")
                        Else
                            trabajador.EmpresaPrimaria.TipoIdentificacion = "2"
                        End If
                    Else : trabajador.EmpresaPrimaria.TipoIdentificacion = "2"
                    End If
                    trabajador.EmpresaPrimaria.TipoVinculo = "1"

                    If dsRespuestaWS.Tables(0).Rows.Count > 1 Then
                        trabajador.EmpresasAsociadas = New CEmpresaAfiliado(dsRespuestaWS.Tables(0).Rows.Count - 2) {}
                    End If

                    For intI = 1 To dsRespuestaWS.Tables(0).Rows.Count - 1
                        trabajador.EmpresasAsociadas(intI - 1) = New CEmpresaAfiliado
                        If dsRespuestaWS.Tables(0).Columns.Contains("XCAR") Then
                            trabajador.EmpresasAsociadas(intI - 1).CargoTrabajador = dsRespuestaWS.Tables(0).Rows(intI)("XCAR")
                        Else
                            trabajador.EmpresasAsociadas(intI - 1).CargoTrabajador = ""
                        End If
                        If dsRespuestaWS.Tables(0).Columns.Contains("CCENCOS") Then
                            trabajador.EmpresasAsociadas(intI - 1).CentroDeCostos = dsRespuestaWS.Tables(0).Rows(intI)("CCENCOS")
                        End If
                        If dsRespuestaWS.Tables(0).Columns.Contains("CDEP") Then
                            trabajador.EmpresasAsociadas(intI - 1).Dependencia = dsRespuestaWS.Tables(0).Rows(intI)("CDEP")
                        End If
                        trabajador.EmpresasAsociadas(intI - 1).EstadoAfiliacionCaja = IIf(dsRespuestaWS.Tables(0).Columns.Contains("CESTCAJ"), dsRespuestaWS.Tables(0).Rows(intI)("CESTCAJ"), -1)
                        trabajador.EmpresasAsociadas(intI - 1).EstadoAfiliacionPOS = IIf(dsRespuestaWS.Tables(0).Columns.Contains("CESTPOS"), dsRespuestaWS.Tables(0).Rows(intI)("CESTPOS"), -1)
                        If dsRespuestaWS.Tables(0).Columns.Contains("FINGTRA") Then
                            trabajador.EmpresasAsociadas(intI - 1).FechaIngreso = dsRespuestaWS.Tables(0).Rows(intI)("FINGTRA")
                        End If
                        If dsRespuestaWS.Tables(0).Columns.Contains("FINGPOS") Then
                            trabajador.EmpresasAsociadas(intI - 1).FechaIngresoPOS = dsRespuestaWS.Tables(0).Rows(intI)("FINGPOS")
                        End If
                        If dsRespuestaWS.Tables(0).Columns.Contains("FRETTRA") Then
                            If Not Convert.IsDBNull(dsRespuestaWS.Tables(0).Rows(intI)("FRETTRA")) Then
                                trabajador.EmpresasAsociadas(intI - 1).FechaRetiro = dsRespuestaWS.Tables(0).Rows(intI)("FRETTRA")
                            End If
                        End If
                        If dsRespuestaWS.Tables(0).Columns.Contains("NHORCON") Then
                            trabajador.EmpresasAsociadas(intI - 1).HorasLaborMes = dsRespuestaWS.Tables(0).Rows(intI)("NHORCON")
                        End If
                        trabajador.EmpresasAsociadas(intI - 1).IdEmpresa = IIf(dsRespuestaWS.Tables(0).Columns.Contains("NIDEEMP"), dsRespuestaWS.Tables(0).Rows(intI)("NIDEEMP"), "")
                        trabajador.EmpresasAsociadas(intI - 1).Nombre = IIf(dsRespuestaWS.Tables(0).Columns.Contains("XNOM"), dsRespuestaWS.Tables(0).Rows(intI)("XNOM"), "")
                        If dsRespuestaWS.Tables(0).Columns.Contains("VSUETRA") Then
                            trabajador.EmpresasAsociadas(intI - 1).SalarioCaja = dsRespuestaWS.Tables(0).Rows(intI)("VSUETRA")
                            trabajador.EmpresasAsociadas(intI - 1).SalarioPOS = dsRespuestaWS.Tables(0).Rows(intI)("VSUETRA")
                        End If
                        If dsRespuestaWS.Tables(0).Columns.Contains("TIDEEMP") Then
                            If dsRespuestaWS.Tables(0).Rows(0)("TIDEEMP") <> "" Then
                                trabajador.EmpresasAsociadas(intI - 1).TipoIdentificacion = dsRespuestaWS.Tables(0).Rows(intI)("TIDEEMP")
                            Else
                                trabajador.EmpresasAsociadas(intI - 1).TipoIdentificacion = "2"
                            End If
                        Else : trabajador.EmpresasAsociadas(intI - 1).TipoIdentificacion = "2"
                        End If

                        trabajador.EmpresasAsociadas(intI - 1).TipoVinculo = "1"
                    Next intI
                    Return True
                End If ' Cierra : If dsRespuestaWS.Tables.Count = 0 Then
            End If ' Cierra : If p_strCadena Is Nothing Or p_strCadena = "" Then

        Catch ex As Exception
            CManejadorMensajes.PublicarMensaje(ex.ToString(), EventLogEntryType.Error)
            Return False
        End Try

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este metodo se encarga de recibir una cadena de xml con varios beneficiarios
    ''' y de devolver un objeto de tipo Cbeneficiario que este identificado segun
    ''' el tipo de identificacion y número que se reciban en los atributos correspondientes
    ''' del parametro beneficiario.   
    ''' </summary>
    ''' <param name="p_strCadena"> Cadena XML con los beneficiarios </param>
    ''' <param name="beneficiario">Objeto beneficiario que será buscado</param>
    ''' <returns>
    ''' True si se encontró el beneficiario y no hubo error
    ''' False Si hubo error
    ''' </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	19/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function StringToCBeneficiario(ByVal p_strCadena As String, ByRef beneficiario As CBeneficiario) As Boolean
        Dim dsBeneficiarios As New DataSet
        Dim sr As StringReader
        'Dim tabla As DataTable
        Dim drfila As DataRow
        Dim blnFound As Boolean
        'Dim i As Integer

        blnFound = False

        Try
            sr = New StringReader(p_strCadena)
            dsBeneficiarios.ReadXml(sr)
            If dsBeneficiarios.Tables.Count > 0 Then
                For Each drfila In dsBeneficiarios.Tables(0).Rows
                    'Revisar identificacion del beneficiario
                    If drfila("NIDEBEN") = beneficiario.NumeroIdentificacion And drfila("TIDEBEN") = beneficiario.TipoIdentificacion And drfila("NIDETRA") = beneficiario.IdTrabajador And drfila("TIDETRA") = beneficiario.TipoId Then
                        beneficiario.TipoId = drfila("TIDETRA")
                        beneficiario.IdTrabajador = drfila("NIDETRA")
                        beneficiario.PrimerApellido = drfila("XPRIAPE")
                        beneficiario.SegundoApellido = drfila("XSEGAPE")
                        beneficiario.Nombres = drfila("XNOMBEN")
                        beneficiario.Genero = drfila("XSEX")
                        beneficiario.Parentezco = drfila("CPRN")
                        beneficiario.FechaNacimiento = drfila("FNACBEN")
                        beneficiario.EstadoAfiliacionCaja = drfila("CESTCAJ")
                        beneficiario.EstadoAfiliacionPOS = drfila("CESTPOS")
                        beneficiario.EstadoMultiafiliacionPOS = drfila("CCAUNEG")
                        beneficiario.Categoria = drfila("CCAT")
                        beneficiario.Direccion = drfila("XDIR")
                        beneficiario.Telefono = drfila("NTEL")
                        beneficiario.Ciudad = drfila("CCIU")
                        blnFound = True
                    End If
                Next
            End If
            If Not blnFound Then
                beneficiario.EstadoAfiliacionCaja = -1
                beneficiario.EstadoAfiliacionPOS = -1
            End If

            Return True
        Catch ex As Exception
            CManejadorMensajes.PublicarMensaje(ex.ToString(), EventLogEntryType.Error)
            Return False
        End Try
    End Function
#End Region

#Region " Funciones Basicas"
    Public Overridable Function ConsultarDatosCarta(ByRef pobjDatosAfiliacion As CDatosAfiliacion, ByRef p_strRespuesta As String) As Boolean
    End Function
    Public Overridable Function TraerGrupoFamiliar(ByRef p_Trabajador As CTrabajador, ByRef p_strRespuesta As String) As Boolean
    End Function
    Public Overridable Function Afiliar(ByRef pobjDatosAfiliacion As CDatosAfiliacion, ByVal pCaja As Boolean, ByVal pPOS As Boolean, ByRef pstrRespuesta As String) As Boolean
    End Function
    Public Overridable Function Modificar(ByRef pobjNovedad As CNovedad, ByRef pstrRespuesta As String) As Boolean
    End Function
    Public Overridable Function GuardarDatosTransar(ByRef p_objAfiliacion As CDatosAfiliacion) As Boolean
    End Function

#End Region

#Region "Funciones Manejo de Errores"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método maneja los errores de negocio que serán enviados hasta los clientes de 
    ''' transar. 
    ''' </summary>
    ''' <param name="pCodigoMensaje"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Protected Sub ManejarErrorSalida(ByVal pCodigoMensaje As Integer)
        Dim strError As String = ""
        CManejadorMensajes.DarMensaje(pCodigoMensaje, strError)
        Me.mstrError = strError

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Overloads.Este método maneja los errores de negocio que serán enviados hasta los clientes de 
    ''' transar. 
    ''' </summary>
    ''' <param name="pMensaje"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Protected Sub ManejarErrorSalida(ByVal pMensaje As String)
        Me.mstrError = pMensaje
    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método maneja errores a nivel de detalle interno, es decir detalles tecnicos 
    ''' que solo serán relevantes al personal de soporte de TRANSAR 
    ''' </summary>
    ''' <param name="Mensaje"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Protected Sub ManejarErrorInterno(ByVal pUbicacion As String, ByVal pCodigoMSG As String, ByVal Mensaje As String)
        Dim strBuilder As StringBuilder
        Dim strerror As String = ""

        strBuilder = New StringBuilder(pUbicacion)
        CManejadorMensajes.DarMensaje(pCodigoMSG, strerror)
        strBuilder.Append(strerror)
        strBuilder.Append(vbCrLf)
        strBuilder.Append(Mensaje)
        CManejadorMensajes.PublicarMensaje(strBuilder.ToString(), EventLogEntryType.Error)

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Overloads.Este método maneja los errores de negocio que serán enviados hasta 
    ''' los clientes de transar. 
    ''' </summary>
    ''' <param name="pCodigoMSG"></param>
    ''' <param name="Mensaje">Mensaje adicional de acuerdo a parametros de la logica</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/16/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Protected Sub ManejarErrorSalida(ByVal pCodigoMSG As String, ByVal Mensaje As String)
        Dim strBuilder As StringBuilder
        Dim strerror As String = ""

        CManejadorMensajes.DarMensaje(pCodigoMSG, strerror)
        strBuilder = New StringBuilder(strerror)
        strBuilder.Append(Mensaje)
        Me.mstrError = strBuilder.ToString
    End Sub

#End Region

#Region "Funciones Privadas"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Valida que el trabajador consultado exista en la empresa autenticada en transar
    ''' </summary>
    ''' <param name="objTrabConsultado">
    ''' Objeto que tiene la informacion del trabajador y sus empresas asociadas
    ''' </param>
    ''' <param name="objTrabAutenticado">
    ''' Objeto que tiene la informacion del trabajador autenticado en Transar
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cenclopezb]	2/10/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function ValidarTrabajadorEmpresa(ByRef objTrabConsultado As CTrabajador, ByVal objTrabAutenticado As CTrabajador) As Boolean
        'busca en todas las empresas del trabajador si la que esta autenticada existe en este grupo
        Dim strIdEmpresaAutenticada As String
        Dim strTipoIdEmpresaAutenticada As String

        strIdEmpresaAutenticada = objTrabAutenticado.EmpresaPrimaria.IdEmpresa
        strTipoIdEmpresaAutenticada = objTrabAutenticado.EmpresaPrimaria.TipoIdentificacion

        If Not strIdEmpresaAutenticada Is Nothing And strIdEmpresaAutenticada <> "" And Not strTipoIdEmpresaAutenticada Is Nothing And strTipoIdEmpresaAutenticada <> "" Then

            'revisa la empresa primaria
            If strIdEmpresaAutenticada <> objTrabConsultado.EmpresaPrimaria.IdEmpresa Or strTipoIdEmpresaAutenticada <> objTrabConsultado.EmpresaPrimaria.TipoIdentificacion Then

                'revisa las empresas secundarias
                Dim objEmpresa As CEmpresaAfiliado
                Dim blnEncontroEmpresa As Boolean = False
                For Each objEmpresa In objTrabConsultado.EmpresasAsociadas
                    If strIdEmpresaAutenticada = objEmpresa.IdEmpresa And strTipoIdEmpresaAutenticada = objEmpresa.TipoIdentificacion Then
                        blnEncontroEmpresa = True
                    End If
                Next

                If Not blnEncontroEmpresa Then 'El trabajador encontrado no pertenece a la empresa autenticada en Transar
                    objTrabConsultado.Nombres = "" 'asigna el valor a esta propiedad para que el objeto se considere vacio
                End If
            End If
        End If

        Return True

    End Function

#End Region


End Class



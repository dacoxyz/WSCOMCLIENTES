#Region " Namespaces"
Imports ObjTransferDatos
Imports EntidadesNegocio
Imports ManejoMensajes
Imports Servicios
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.IO
Imports System.Xml
Imports System.Threading
Imports System.Xml.Serialization

#End Region
''' -----------------------------------------------------------------------------
''' Project	 : LogicaTransaccional
''' Class	 : ManejadorTransacciones
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Este componente se encarga de manejar la lógica necesaria para 
''' realizar transacciones tales como Afiliaciones, Retiros y Modificaciones de 
''' cotizantes y beneficiarios al host ES7000
''' </summary>
''' <remarks>
''' Esta clase interactua con los ISPECS del Es7000 para realizar dichas transacciones
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	15/09/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Class ManejadorTransacciones
    Dim consCliente As New CConsultaClienteINQ

#Region "Constantes "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Rango de numeracion de planillas de autoliquidacion de aportes
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Const CNSRANGONUMERACIONPLANILLAS As Double = 500000000

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' valores de posiciones del ispec CTE32
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/15/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Const CNSPOSCTE32_FPAG As Integer = 0
    Private Const CNSPOSCTE32_CSUC As Integer = 1
    Private Const CNSPOSCTE32_NPLA As Integer = 2
    Private Const CNSPOSCTE32_NIDE As Integer = 3
    Private Const CNSPOSCTE32_VREC As Integer = 4
    Private Const CNSPOSCTE32_NDOC As Integer = 5
    Private Const CNSPOSCTE32_XMAR As Integer = 6


#End Region

#Region "Constructores"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor por defecto de la clase; se encarga de inicializar las propiedades
    ''' estáticas de la clase y de ajustar los valores del idioma y valores provinientes
    ''' de los archivos de configuración.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	1/17/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub New()
        'Dim strError As String
        Try
            If ConfigurationManager.AppSettings.HasKeys() Then
                strAplicacion = ConfigurationManager.AppSettings("IdAplicacion")
            Else
                strAplicacion = "SWPR57"
            End If
            If ConfigurationManager.AppSettings.HasKeys() Then
                nombAplicacion = ConfigurationManager.AppSettings("NombreAplicacion")
            Else
                nombAplicacion = "Transar"
            End If

            If Not ConfigurationManager.AppSettings("ParametroCaja") Is Nothing Then
                intConstanteCaja = ConfigurationManager.AppSettings("ParametroCaja")
            Else
                intConstanteCaja = 1
            End If
            If Not ConfigurationManager.AppSettings("ParametroPOS") Is Nothing Then
                intConstantePOS = ConfigurationManager.AppSettings("ParametroPOS")
            Else
                intConstantePOS = 2
            End If

            mFormatter = New CultureInfo("es-CO", True).DateTimeFormat
            Thread.CurrentThread.CurrentUICulture = New CultureInfo("es-CO", True)
            Thread.CurrentThread.CurrentCulture = New CultureInfo("es-CO", True)
            ws = New Servicios.Servicios.AFSYS


        Catch ex As Exception
            ManejarErrorInterno("CManajeadorTransacciones::Constructor", 6, ex.ToString())
            ManejarErrorSalida(3)
        End Try

    End Sub
#End Region

#Region " Propiedades"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Por medio de esta propiedad, los componentes que hagan uso de ManejadorTransacciones
    ''' pueden determinar qué error ocurrió, en caso de que alguno de los métodos devuelva 
    ''' False
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' Esta propiedad es de lectura exclusivamente. 
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public ReadOnly Property DescripcionError() As String
        Get
            Return mstrError
        End Get
    End Property
#End Region

#Region " Miembros Privados y Shared"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Miembro Privado que almacena el error que haya ocurrido dentro de la clase, en 
    ''' caso de haber ocurrido alguno. 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private mstrError As String
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Proxy al Web Service por medio del cual se van a enviar las transacciones al sistema ES7000
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private ws As Servicios.Servicios.AFSYS
    ''' -----------------------------------------------------------------------------
    Private mFormatter As DateTimeFormatInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Nombre de la aplicacion que se enviará al Web Service ws como parametro.
    ''' </summary>
    ''' <remarks>
    ''' Este nombre será buscado en el web.config o app.config, de la aplicación 
    ''' que haga uso del componente ManejadorTransacciones.
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private strAplicacion As String
    Private nombAplicacion As String
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Constante que se saca del web.config y corresponde al indicador Caja, que se le debe enviar al 
    ''' Web Service
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	17/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Shared intConstanteCaja As Integer
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Constante que se saca del web.config y corresponde al indicador POS, que se le debe enviar al 
    ''' Web Service
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	17/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Shared intConstantePOS As Integer

#End Region

#Region "Transacciones Trabajador CAJA"

#End Region

#Region "Transacciones Beneficiario CAJA"

#End Region




    ''#Region "Funciones Privadas "

#Region "Funciones Segundo Empleo EGE05"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' NO TOCAR Esta función permite determinar si un trabajador está o no afiliado por la misma
    ''' empresa especificada en el parámetro objEmpresa, o por otra empresa.
    ''' </summary>
    ''' <param name="p_Trabajador">Objeto de tipo Ctrabajador con la información
    ''' del trabajador que se quiere consultar</param>
    ''' <param name="objEmpresa">Objeto de tipo CEmpresa por medio de la cual se quiere saber 
    ''' si el trabajador está afiliado o no</param>
    ''' <returns>True si el trabajador se encuentra afiliado por la misma empresa 
    ''' False si se encuentra afiliado por otra Empresa</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function AfiliadoMismaEmpresa(ByVal p_Trabajador As CTrabajador, ByVal objEmpresa As CEmpresaAfiliado) As Boolean
        Try

            If p_Trabajador.EmpresaPrimaria.IdEmpresa = objEmpresa.IdEmpresa _
               And p_Trabajador.EmpresaPrimaria.TipoIdentificacion = objEmpresa.TipoIdentificacion Then
                Return True
            Else : Return False
            End If

        Catch ex As Exception
            ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones::AfiliadoMismaEmpresa", 6, ex.ToString())
            ManejarErrorSalida(3)
        End Try
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta función permite realizar la verificación de la lógica de segundo empleo,
    ''' para  Caja de Compensación de un trabajador. 
    ''' </summary>
    ''' <returns>True si el método fue ejecutado con éxito 
    ''' False de lo contrario</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	01/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function VerificacionesSegundoEmpleoCAJA(ByRef p_Afiliacion As CDatosAfiliacion, ByRef p_strRespuesta As String) As Boolean


        Dim intIndice As Integer
        Dim strMensaje As String = ""
        Dim objEmpresaEGE05 As CEmpresaAfiliado
        Dim objEmpresaQueAfilia As CEmpresa
        Dim objTrabajador As CTrabajador
        Dim dsEGE05 As New Ege05
        Dim drfilaEge05 As Ege05.EGE05Row
        Dim strRespuesta As String
        'Dim wsAF As New AF.AFI
        Dim dsSalario As New DataSet
        'Dim dblSMLV As Double

        Try

            objTrabajador = p_Afiliacion.Persona
            objEmpresaQueAfilia = p_Afiliacion.Empresa

            ' Consultar EGE05
            '1. Buscar empresa primaria  en EGE05
            If BuscarAfiliacionEGE05(objTrabajador, objEmpresaQueAfilia, intIndice) Then
                objEmpresaEGE05 = objTrabajador.EmpresasAsociadas(intIndice)

                Select Case objEmpresaEGE05.EstadoAfiliacionCaja
                    Case 0 ' Ya se encuentra Afiliado 

                        'FuncionINQAFE21(dsAFE21, drFila, objTrabajador, "1")
                        CManejadorMensajes.DarMensaje(9, strMensaje)
                        'Dim builder As System.Text.StringBuilder
                        'builder = New StringBuilder(strMensaje)
                        'builder = builder.Replace("{0}", DateTime.ParseExact(dsAFE21.AFE21(0).I_FINGCAJ, "yyyyMMdd", mFormatter))
                        'p_strRespuesta = builder.ToString
                        'Return True
                        p_strRespuesta = strMensaje
                        Return True
                    Case Else ' Esta por POS
                        '1. enviar afi a ege05
                        drfilaEge05 = dsEGE05.EGE05.NewEGE05Row()
                        FuncionINQEGE05(dsEGE05, drfilaEge05, objTrabajador, objEmpresaQueAfilia, intConstanteCaja)
                        dsEGE05.EGE05(0).I_MAINT = "CHG"
                        dsEGE05.EGE05(0).MESSAGE = Nothing
                        If LlenarDataRowCajaTrabajador(dsEGE05.EGE05(0), objTrabajador, p_Afiliacion.Empresa, p_Afiliacion) Then
                            'dsEGE05.EGE05.AddEGE05Row(drfilaEge05)
                            strRespuesta = ws.Ispec(Me.strAplicacion, "EGE05", dsEGE05.GetXml(), intConstanteCaja)
                            CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                            Me.ReadXml(dsEGE05, New StringReader(strRespuesta))
                            If BuscarManejarErrores(dsEGE05, "LogicaTransaccional::ManejadorTransacciones::VerificacionesSegundoEmpleoCaja") Then
                                'Hubo Errores 
                                Return False
                            Else
                                'Verificar la suma de los salarios reportados en: EGE05 y en AFE21 si es mayor a 4 smlm
                                'Consultar el WS para traer el SMLV, si no estaba ya en la variable estatica
                                'If Me.dblSMLV = 0 Then
                                '    strRespuesta = wsAF.SMLV(strAplicacion)
                                '    dsSalario.ReadXml(New StringReader(strRespuesta))
                                '    dblSMLV = CDbl(dsSalario.Tables(0).Rows(0)(0))
                                'End If
                                ''Traer del web.config el numero de SMLV con el que se va a hacer la comparacion
                                'If intNumSalariosMinimos = 0 Then
                                '    If Not ConfigurationSettings.AppSettings("NumSalariosMinimos") Is Nothing Then
                                '        intNumSalariosMinimos = ConfigurationSettings.AppSettings("NumSalariosMinimos")
                                '    Else
                                '        ManejarErrorInterno("ManejadorTransacciones::VerificacionesSegundoEmpleoCaja", 21, "")
                                '        ManejarErrorSalida(3)
                                '        Return False
                                '    End If
                                'End If

                                'If CInt((objTrabajador.EmpresaPrimaria.SalarioCaja + objEmpresaEGE05.SalarioCaja) / dblSMLV) > intNumSalariosMinimos Then
                                '    '3. Enviar LOE al AFE24
                                '    'If Not FuncionADDAFE24(objTrabajador, "LOE") Then
                                '    '    'Todo Esto se debe guardar en el "log", como un add que no se pudo procesar
                                '    'End If
                                'End If
                                'Guarda mensaje retornado por el Host para seguimiento posterior
                                p_Afiliacion.RespuestaHost = dsEGE05.EGE05(0).MESSAGE

                                GuardarDatosAfiliacionCAJA(p_Afiliacion, "ADD")
                                CManejadorMensajes.DarMensaje(10, p_strRespuesta)
                                Return True
                            End If

                        Else : Return False
                        End If
                End Select

            Else ' not BuscarAfiliacionEGE05(objTrabajador, intIndice)
                'En EGE05 hay otras empresas asociadas, pero no la empresa primaria
                'Buscar si de las empresas afiliadas alguna tiene cestcaj =1 
                If BuscarAfiliadasCajaEnEGE05(objTrabajador, intIndice) Then
                    ' Hay una que tiene cestcaj = 1, posible translado
                    '1.Enviar ADD a EGE05
                    drfilaEge05 = dsEGE05.EGE05.NewEGE05Row()
                    drfilaEge05.I_MAINT = "ADD"
                    If LlenarDataRowCajaTrabajador(drfilaEge05, objTrabajador, p_Afiliacion.Empresa, p_Afiliacion) Then
                        dsEGE05.EGE05.AddEGE05Row(drfilaEge05)
                        strRespuesta = ws.Ispec(Me.strAplicacion, "EGE05", dsEGE05.GetXml(), intConstanteCaja)
                        CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                        Me.ReadXml(dsEGE05, New StringReader(strRespuesta))
                        If BuscarManejarErrores(dsEGE05, "LogicaTransaccional::ManejadorTransacciones::VerificacionesSegundoEmpleoCaja") Then
                            'Hubo Errores 
                            Return False
                        Else

                            'Guarda mensaje retornado por el Host para seguimiento posterior
                            p_Afiliacion.RespuestaHost = dsEGE05.EGE05(0).MESSAGE

                            '2.Validar del formulario ingresado si es Segundo Empleo?
                            ' If objTrabajador.SegundoEmpleo Then
                            GuardarDatosAfiliacionCAJA(p_Afiliacion, "ADD")
                            CManejadorMensajes.DarMensaje(10, p_strRespuesta)
                            p_Afiliacion.TransladoCaja = False

                            Return True
                            'Else ' 2.2 No es segundo empleo 

                            '' Ponerlo en el "log" como un posible transalado 
                            'p_Afiliacion.TransladoCaja = True
                            'Return True
                            'End If
                        End If ' Cierra BuscarManejarErrores
                    End If 'LlenarDataRowCajaTrabajador ...
                Else 'NOT  BuscarAfiliadasCajaEnEGE05(objTrabajador, intIndice)
                    ' Todas estan con CESTCAJ =0 o no hay ninguna aún
                    '1. Enviar ADD a EGE 05 
                    drfilaEge05 = dsEGE05.EGE05.NewEGE05Row()
                    drfilaEge05.I_MAINT = "ADD"
                    If LlenarDataRowCajaTrabajador(drfilaEge05, objTrabajador, p_Afiliacion.Empresa, p_Afiliacion) Then
                        dsEGE05.EGE05.AddEGE05Row(drfilaEge05)
                        strRespuesta = ws.Ispec(Me.strAplicacion, "EGE05", dsEGE05.GetXml(), intConstanteCaja)
                        CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                        Me.ReadXml(dsEGE05, New StringReader(strRespuesta))
                        If BuscarManejarErrores(dsEGE05, "LogicaTransaccional::ManejadorTransacciones::VerificacionesSegundoEmpleoCaja") Then
                            'Hubo Errores 
                            Return False
                        Else
                            'Guarda mensaje retornado por el Host para seguimiento posterior
                            p_Afiliacion.RespuestaHost = dsEGE05.EGE05(0).MESSAGE

                            '2. Guardar Datos de la transaccion recien hecha
                            GuardarDatosAfiliacionCAJA(p_Afiliacion, "ADD")
                            CManejadorMensajes.DarMensaje(10, p_strRespuesta)
                            Return True
                        End If ' CIERRA : BuscarManejarErrores...
                    End If ' CIERRA : LlenarDataRowCajaTrabajador... 
                End If ' CIERRA: BuscarAfiliadasCajaEnEGE05(objTrabajador, intIndice)
            End If ' Cierra BuscarAfiliacionEGE05 ... 

        Catch ex As Exception
            ManejarErrorInterno("CManajeadorTransacciones::TransaccionAfiliarTrabajadorCaja", 6, ex.ToString())
            ManejarErrorSalida(3)
        End Try

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta función permite realizar la verificación de la lógica de segundo empleo,
    ''' para POS. 
    ''' </summary>
    ''' <param name="p_Afiliacion"></param>
    ''' <param name="p_strRespuesta"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/6/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function VerificacionesSegundoEmpleoPOS(ByRef p_Afiliacion As CDatosAfiliacion, ByRef p_strRespuesta As String) As Boolean


        Dim intIndice As Integer
        Dim strMensaje As String = ""
        Dim objEmpresaEGE05 As CEmpresaAfiliado
        Dim objTrabajador As CTrabajador
        Dim dsEGE05 As New Ege05
        Dim drfilaEge05 As Ege05.EGE05Row
        Dim strRespuesta As String
        Dim objEmpresaQueAfilia As CEmpresa

        Try

            objTrabajador = p_Afiliacion.Persona
            objEmpresaQueAfilia = p_Afiliacion.Empresa


            '1. Buscar empresa primaria  en EGE05
            If BuscarAfiliacionEGE05(objTrabajador, objEmpresaQueAfilia, intIndice) Then 'Encontro la empresa en EGE05
                objEmpresaEGE05 = objTrabajador.EmpresasAsociadas(intIndice)

                'Analizar indicador POS
                Select Case objEmpresaEGE05.EstadoAfiliacionPOS
                    Case 0 ' Ya se encuentra Afiliado 
                        CManejadorMensajes.DarMensaje(22, strMensaje)
                        p_strRespuesta = strMensaje
                        Return True

                    Case Else ' Esta por Caja
                        '1. enviar afi a ege05
                        drfilaEge05 = dsEGE05.EGE05.NewEGE05Row()
                        FuncionINQEGE05(dsEGE05, drfilaEge05, objTrabajador, objEmpresaQueAfilia, intConstanteCaja)
                        dsEGE05.EGE05(0).I_MAINT = "CHG"
                        dsEGE05.EGE05(0).MESSAGE = Nothing
                        'Se debe cambiar el indicador POS a 1

                        If LlenarDataRowPOSTrabajador(dsEGE05.EGE05(0), p_Afiliacion) Then
                            'dsEGE05.EGE05.ImportRow(drfilaEge05)
                            strRespuesta = ws.Ispec(Me.strAplicacion, "EGE05", dsEGE05.GetXml(), intConstantePOS)
                            CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                            Me.ReadXml(dsEGE05, New StringReader(strRespuesta))
                            If BuscarManejarErrores(dsEGE05, "LogicaTransaccional::ManejadorTransacciones::VerificacionesSegundoEmpleoPOS") Then
                                'Hubo Errores 
                                Return False
                            Else

                                'Guarda mensaje retornado por el Host para seguimiento posterior
                                p_Afiliacion.RespuestaHost = dsEGE05.EGE05(0).MESSAGE

                                '2. Guardar Datos de la transaccion recien hecha
                                GuardarDatosAfiliacionPOS(p_Afiliacion, "CHG")
                                CManejadorMensajes.DarMensaje(23, p_strRespuesta)

                                Return True
                            End If
                        Else
                            Return False
                        End If
                End Select

            Else
                'Enviar un add a ege05 porque no esta la empresa primaria en el conjunto de empresas asociadas
                drfilaEge05 = dsEGE05.EGE05.NewEGE05Row()
                drfilaEge05.I_MAINT = "ADD"
                If LlenarDataRowPOSTrabajador(drfilaEge05, p_Afiliacion) Then
                    dsEGE05.EGE05.AddEGE05Row(drfilaEge05)
                    strRespuesta = ws.Ispec(Me.strAplicacion, "EGE05", dsEGE05.GetXml(), intConstantePOS)
                    CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                    Me.ReadXml(dsEGE05, New StringReader(strRespuesta))
                    If BuscarManejarErrores(dsEGE05, "LogicaTransaccional::ManejadorTransacciones::VerificacionesSegundoEmpleoPOS") Then
                        'Hubo Errores 
                        Return False
                    Else
                        'Guarda mensaje retornado por el Host para seguimiento posterior
                        p_Afiliacion.RespuestaHost = dsEGE05.EGE05(0).MESSAGE

                        '2. Guardar Datos de la transaccion recien hecha
                        GuardarDatosAfiliacionPOS(p_Afiliacion, "ADD")
                        CManejadorMensajes.DarMensaje(23, p_strRespuesta)
                        Return True

                    End If ' CIERRA : BuscarManejarErrores...
                End If ' CIERRA : LlenarDataRowPOSTrabajador... 
            End If ' Cierra BuscarAfiliacionEGE05 ... 

        Catch ex As Exception
            ManejarErrorInterno("CManajeadorTransacciones::TransaccionAfiliarTrabajadorCaja", 6, ex.ToString())
            ManejarErrorSalida(3)
        End Try

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''  Esta función busca la empresa primaria de un trabajador (la encontrada en AFE21)
    ''' dentro del grupo de empresas de EGE05 (EmpresasAsociadas)
    ''' </summary>
    ''' <param name="p_objTrabajador">Trabajador para el cual se va a hacer la consulta</param>
    ''' <param name="p_intIndice">Si la empresa primaria se encuentra dentro del grupo de empresas
    ''' de EGE05, en este parametro se envía el indice dentro del arreglo de empresas asociadas 
    ''' en el cual se encuentra la empresa </param>
    ''' <returns>
    ''' True si la empresa primaria se encuentra dentro de las empresas asociadas 
    ''' False de lo contrario 
    ''' </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	31/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function BuscarAfiliacionEGE05(ByVal p_objTrabajador As CTrabajador, ByVal p_Empresa As CEmpresa, ByRef p_intIndice As Integer) As Boolean
        Dim empresa As CEmpresaAfiliado
        Dim intI As Integer

        intI = 0

        If p_objTrabajador.EmpresasAsociadas Is Nothing Then
            Return False
        Else
            For Each empresa In p_objTrabajador.EmpresasAsociadas
                If empresa.IdEmpresa = p_Empresa.IdEmpresa _
                    And empresa.TipoIdentificacion = p_Empresa.TipoIdentificacion Then
                    p_intIndice = intI
                    Return True
                End If
                intI = intI + 1
            Next
            Return False
        End If
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta función permite buscar dentro del grupo de empresas que se encuentran en EGE05
    ''' alguna que tenga el indicador CESTCAJ en 1
    ''' </summary>
    ''' <param name="p_objTrabajador">Trabajador para el que ejecuta la busqueda</param>
    ''' <param name="p_intIndice">Indice en el que se encuentra la empresa con cestcaja=0</param>
    ''' <returns>
    ''' True si alguna tiene CESTCAJ en 1
    ''' False de lo contrario
    ''' </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	31/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function BuscarAfiliadasCajaEnEGE05(ByVal p_objTrabajador As CTrabajador, ByRef p_intIndice As Integer) As Boolean

        Dim objEmpresaAfiliado As CEmpresaAfiliado
        Dim blnFlag As Boolean

        blnFlag = False
        For Each objEmpresaAfiliado In p_objTrabajador.EmpresasAsociadas
            If objEmpresaAfiliado.EstadoAfiliacionCaja = 0 Then
                blnFlag = True
            End If
        Next
        Return blnFlag
    End Function
    Private Function SumatoriaSalarios(ByRef objTrabajador As CTrabajador, ByRef objEmpresaQueAfilia As CEmpresaAfiliado) As Double
        Dim empresa As CEmpresaAfiliado
        Dim dblSumatoria As Double

        dblSumatoria = 0
        For Each empresa In objTrabajador.EmpresasAsociadas
            dblSumatoria += empresa.SalarioCaja
        Next
        dblSumatoria += objTrabajador.EmpresaPrimaria.SalarioCaja
        dblSumatoria += objEmpresaQueAfilia.SalarioCaja
        Return dblSumatoria
    End Function

#End Region

#Region "Funciones INQ "
  
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Overloads. Esta función simula la función INQ del ES7000 , hace una consulta en CLE15
    ''' especificado en el objeto pobjTrabajadador, con su cedula y su tipo de cedula.
    ''' Los resultados de la consulta son devueltos en el parámetro de salida pobjFila .
    ''' Este método no devuelve  indicadores de error, maneja dentro de la propia función
    '''   cualquier excepción que pueda ocurrir.
    ''' </summary>
    ''' <param name="pobj_cle15"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[WEMUNEVARM]	24/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function FuncionINQCLE15(ByRef pobj_cle15 As DtsCLE15) As Boolean
        Dim strRespuesta As String

        Try

            strRespuesta = pobj_cle15.GetXml()
            CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)
            strRespuesta = ws.Ispec(Me.strAplicacion, "CLE15", strRespuesta, "1")
            CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)
            Me.ReadXml(pobj_cle15, New StringReader(strRespuesta))

            If pobj_cle15._Error.Rows.Count > 0 Then
                ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones:FuncionINQ", 5, pobj_cle15._Error(0).Mensaje)
                ManejarErrorSalida(3)
            ElseIf pobj_cle15.CLE15.Rows.Count > 0 Then
                If InStr(pobj_cle15.CLE15(0).MESSAGE, "ERROR") Then
                    If pobj_cle15.CLE15(0).MESSAGE.Trim = "ERROR            CLIENTE NO EXISTE" Then
                        Return True
                    Else
                        ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones:FuncionINQ", 5, pobj_cle15.CLE15(0).MESSAGE)
                        ManejarErrorSalida(pobj_cle15.CLE15(0).MESSAGE)
                        Return True
                    End If
                Else
                    Return False
                End If
            Else
                ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones:FuncionINQ", 5, pobj_cle15.CLE15(0).MESSAGE)
                ManejarErrorSalida(pobj_cle15.CLE15(0).MESSAGE)
            End If
            Return False
        Catch ex As Exception
            ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones::FuncionINQ", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try
    End Function

    Private Function FuncionINQCLE15(ByRef pobj_cle15 As DtsCLE15, ByVal objDatosAfiliacion As CDatosPersona, ByVal tipo As String, ByVal Medio As String, ByVal COMANDO As String) As Boolean
        Dim strRespuesta As String

        Try

            strRespuesta = consCliente.ConsultarCliente(objDatosAfiliacion, tipo)
            CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)
            Me.ReadXml(pobj_cle15, New StringReader(strRespuesta))

            If pobj_cle15._Error.Rows.Count > 0 Then
                ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones:FuncionINQ", 5, pobj_cle15._Error(0).Mensaje)
                ManejarErrorSalida(3)
            ElseIf pobj_cle15.CLE15.Rows.Count > 0 Then
                If InStr(pobj_cle15.CLE15(0).MESSAGE, "ERROR") Then
                    If pobj_cle15.CLE15(0).MESSAGE.Trim = "ERROR            CLIENTE NO EXISTE" Then
                        Return True
                    Else
                        ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones:FuncionINQ", 5, pobj_cle15.CLE15(0).MESSAGE)
                        ManejarErrorSalida(pobj_cle15.CLE15(0).MESSAGE)
                        Return True
                    End If
                Else
                    Return False
                End If
            Else
                ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones:FuncionINQ", 5, pobj_cle15.CLE15(0).MESSAGE)
                ManejarErrorSalida(pobj_cle15.CLE15(0).MESSAGE)
            End If
            Return False
        Catch ex As Exception
            'CREADO LCARDENAS 19-04-2012: SEGUIMIENTO INCIDENCIA 9430 INTERMITENCIA WSCOMCLIENTES
            If ConfigurationSettings.AppSettings("LogAuditoriaAfiliar") = "S" Then
                Dim objCache As New Compensar.Vincula.POS.ClsCache
                Dim sInnerExcepcion As String = ""
                If Not ex.InnerException Is Nothing Then
                    sInnerExcepcion = ex.InnerException.ToString()
                End If
                objCache.guardarErrorBitacora(ConfigurationSettings.AppSettings("ProjectID"), _
1, "Auditoria Afiliar: ManejadorTransacciones.FuncionINQCLE15: " & ex.Message & sInnerExcepcion & "StackTrace: " & ex.StackTrace, "", "", "", My.Computer.Name, 1, 1)
            End If
            ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones::FuncionINQ", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try
    End Function

    Private Sub FuncionINQEGE05(ByRef pobj_EGE05 As Ege05, ByRef pobjFila As Ege05.EGE05Row, ByVal pobjTrabajador As CTrabajador, ByVal pEmpresa As CEmpresaAfiliado, ByVal strTipoAfiliacion As String)
        Dim strRespuesta As String
        Try
            '1. Hacer INQ  
            ' 1.1 Funcion de consulta
            pobjFila.I_MAINT = "INQ"
            ' 1.2 Ajustar Documento y Tipo de Documento 
            pobjFila.I_NIDETRA = pobjTrabajador.IdTrabajador
            pobjFila.I_TIDETRA = pobjTrabajador.TipoIdentificacion
            pobjFila.I_NIDESUC = pEmpresa.IdEmpresa
            pobjFila.I_TIDESUC = pEmpresa.TipoIdentificacion
            ' 1.3 Enviar INQ
            pobj_EGE05.EGE05.AddEGE05Row(pobjFila)
            strRespuesta = ws.Ispec(Me.strAplicacion, "EGE05", pobj_EGE05.GetXml(), strTipoAfiliacion)
            CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

            Me.ReadXml(pobj_EGE05, New StringReader(strRespuesta))

            If pobj_EGE05._Error.Rows.Count > 0 Then
                ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones:FuncionINQ", 5, pobj_EGE05._Error(0).Mensaje)
                ManejarErrorSalida(3)
            ElseIf pobj_EGE05.EGE05.Rows.Count > 0 Then
                If pobj_EGE05.EGE05(0).MESSAGE.StartsWith("ERROR") Then
                    'Error manejarlo 
                    ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones:FuncionINQ", 5, pobj_EGE05.EGE05(0).MESSAGE)
                    ManejarErrorSalida(pobj_EGE05.EGE05(0).MESSAGE)
                End If
            End If

        Catch ex As Exception
            ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones::FuncionINQ", 6, ex.ToString())
            ManejarErrorSalida(3)
        End Try
    End Sub


#End Region

#Region " Funciones ADD"
    ' ''' -----------------------------------------------------------------------------
    ' ''' <summary>
    ' ''' Esta función permite enviar la solicitud de requisitos a AFE24
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks>
    ' ''' </remarks>
    ' ''' <history>
    ' ''' 	[CENCXLOPEZC]	01/09/2004	Created
    ' ''' </history>
    ' ''' -----------------------------------------------------------------------------
    'Private Function FuncionADDAFE24(ByVal p_objPersona As CDatosPersona, ByVal p_strRequisito As String) As Boolean

    '    Dim dsAfe24 As New AFE24
    '    Dim drAfe24 As AFE24.AFE24Row
    '    Dim objTrabajador As CTrabajador
    '    Dim objBeneficiario As CBeneficiario
    '    Dim strRespuesta As String

    '    Try
    '        drAfe24 = dsAfe24.AFE24.NewAFE24Row()
    '        drAfe24.I_MAINT = "ADD"
    '        drAfe24.I_XDESREQ = p_strRequisito

    '        If p_objPersona.GetType.FullName = "BusinessEntities.CTrabajador" Then
    '            objTrabajador = p_objPersona
    '            drAfe24.I_NIDETRA = objTrabajador.IdTrabajador
    '            drAfe24.I_TIDETRA = objTrabajador.TipoIdentificacion

    '        ElseIf p_objPersona.GetType.FullName = "BusinessEntities.CBeneficiario" Then
    '            objBeneficiario = p_objPersona
    '            drAfe24.I_TIDETRA = objBeneficiario.TipoId
    '            drAfe24.I_NIDETRA = objBeneficiario.IdTrabajador
    '            drAfe24.I_NIDEBEN = objBeneficiario.TipoIdentificacion
    '            drAfe24.I_TIDEBEN = objBeneficiario.TipoIdentificacion
    '        End If

    '        dsAfe24.AFE24.AddAFE24Row(drAfe24)
    '        strRespuesta = ws.Ispec(Me.strAplicacion, "AFE24", dsAfe24.GetXml(), intConstanteCaja)
    '        Me.ReadXml(dsAfe24, New StringReader(strRespuesta))
    '        If dsAfe24._Error.Rows.Count > 0 Then
    '            'Error manejarlo 
    '            ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones:FuncionADDAFE24", 5, dsAfe24._Error(0).Mensaje)
    '            ManejarErrorSalida(3)
    '            Return False
    '        ElseIf dsAfe24.AFE24.Rows.Count > 0 Then
    '            If dsAfe24.AFE24(0).MESSAGE.StartsWith("ERROR") Then
    '                ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones:FuncionADDAFE24", 5, dsAfe24.AFE24(0).MESSAGE)
    '                ManejarErrorSalida(dsAfe24.AFE24(0).MESSAGE)
    '                Return False
    '            End If
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones::FuncionADDAFE24", 6, ex.ToString())
    '        ManejarErrorSalida(3)
    '        Return False
    '    End Try

    'End Function

#End Region

#Region " Funciones Manejo De Error"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método maneja errores a nivel de detalle interno, es decir detalles tecnicos 
    ''' que solo serán relevantes al personal de soporte de TRANSAR 
    ''' </summary>
    ''' <param name="Mensaje">Mensaje adicional que se guardarán en el log de eventos</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub ManejarErrorInterno(ByVal pUbicacion As String, ByVal pCodigoMSG As String, ByVal Mensaje As String)
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
    ''' Este método maneja warnings a nivel de detalle interno, es decir detalles tecnicos 
    ''' que solo serán relevantes al personal de soporte de TRANSAR 
    ''' </summary>
    ''' <param name="pUbicacion"></param>
    ''' <param name="pCodigoMSG"></param>
    ''' <param name="Mensaje"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/1/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub ManejarWarningInterno(ByVal pUbicacion As String, ByVal Mensaje As String, Optional ByVal pCodigoMSG As String = "")
        Dim strBuilder As StringBuilder
        Dim strerror As String = ""

        strBuilder = New StringBuilder(pUbicacion)
        If pCodigoMSG <> "" Then
            CManejadorMensajes.DarMensaje(pCodigoMSG, strerror)
            strBuilder.Append(strerror)
            strBuilder.Append(vbCrLf)
        End If
        strBuilder.Append(Mensaje)
        CManejadorMensajes.PublicarMensaje(strBuilder.ToString(), EventLogEntryType.Warning)

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método maneja los errores de negocio que serán enviados hasta los clientes de 
    ''' transar. 
    ''' </summary>
    ''' <param name="pCodigoMensaje">Código del mensaje que será enviado a los componentes
    ''' que hagan uso de ManejadorTransacciones</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub ManejarErrorSalida(ByVal pCodigoMensaje As Integer)
        Dim strError As String = ""
        CManejadorMensajes.DarMensaje(pCodigoMensaje, strError)
        Me.mstrError = strError

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Overloads. Este método maneja los errores de negocio que serán enviados hasta los clientes de 
    ''' transar. 
    ''' </summary>
    ''' <param name="pMensaje">Mensaje que será enviado a los componentes
    ''' que hagan uso de ManejadorTransacciones</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    ''' 
    Private Sub ManejarErrorSalida(ByVal pMensaje As String)
        Me.mstrError = pMensaje
    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta función busca y maneja errores en un dataset fuertemente tipado.
    ''' </summary>
    ''' <param name="dsEGE05"> DataSet en el que se van a buscar los errores</param>
    ''' <param name="pUbicacion">Ubicación del método que llama a este método </param>
    ''' <returns>True si hubo errores 
    ''' False de lo contrario </returns>
    ''' <remarks>
    ''' Estos errores son el resultado de enviar una transaccion al host ES7000 por medio del 
    ''' web service AF
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function BuscarManejarErrores(ByVal dsEGE05 As Ege05, ByVal pUbicacion As String) As Boolean

        If dsEGE05._Error.Rows.Count > 0 Then
            ManejarErrorInterno(pUbicacion, 5, dsEGE05._Error.Rows(0)("Mensaje"))
            ManejarErrorSalida(3)
            Return True
        ElseIf dsEGE05.EGE05.Rows.Count > 0 Then
            If InStr(dsEGE05.EGE05(0).MESSAGE, "ERROR") > 0 Then
                ManejarErrorInterno(pUbicacion, 5, dsEGE05.EGE05(0).MESSAGE)
                ManejarErrorSalida(dsEGE05.EGE05(0).MESSAGE)
                Return True
            End If
        Else : Return False
        End If

    End Function


    Private Function BuscarManejarErrores(ByVal dsCLE15 As DtsCLE15, ByVal pUbicacion As String) As Boolean
        Dim strerror As String = ""
        If dsCLE15._Error.Rows.Count > 0 Then
            ManejarErrorInterno(pUbicacion, 5, dsCLE15._Error.Rows(0)("Mensaje"))
            ManejarErrorSalida(3)
            Return True
        ElseIf dsCLE15.CLE15.Rows.Count > 0 Then
            If InStr(dsCLE15.CLE15(0).MESSAGE, "REGISTRO FID INMODIFICABLE") > 0 Then
                Return False
            ElseIf InStr(dsCLE15.CLE15(0).MESSAGE, "ERROR") > 0 Then
                ManejarErrorInterno(pUbicacion, 5, dsCLE15.CLE15(0).MESSAGE)
                If InStr(dsCLE15.CLE15(0).MESSAGE, "CLIENTE EXISTE FONETICO") > 0 Then
                    CManejadorMensajes.DarMensaje(178, strerror)
                    dsCLE15.CLE15(0).MESSAGE = dsCLE15.CLE15(0).MESSAGE + strerror
                ElseIf InStr(dsCLE15.CLE15(0).MESSAGE, "NO SE HIZO NINGUNA ACTUALIZACION") > 0 Then
                    Return False
                ElseIf InStr(dsCLE15.CLE15(0).MESSAGE, "REGISTRO FID INMODIFICABLE") > 0 Then
                    Return False
                ElseIf InStr(dsCLE15.CLE15(0).MESSAGE, "303") > 0 Then
                    CManejadorMensajes.DarMensaje(179, strerror)
                    dsCLE15.CLE15(0).MESSAGE = "Error Primer Apellido no es válido. " + strerror + ": " + dsCLE15.CLE15(0).I_XPRIAPE
                ElseIf InStr(dsCLE15.CLE15(0).MESSAGE, "304") > 0 Then
                    CManejadorMensajes.DarMensaje(179, strerror)
                    dsCLE15.CLE15(0).MESSAGE = "Error Segundo Apellido no es válido. " + strerror + ": " + dsCLE15.CLE15(0).I_XSEGAPE
                ElseIf InStr(dsCLE15.CLE15(0).MESSAGE, "302") > 0 Then
                    CManejadorMensajes.DarMensaje(179, strerror)
                    dsCLE15.CLE15(0).MESSAGE = "Error Primer Nombre no es válido. " + strerror + ": " + dsCLE15.CLE15(0).I_XNOMTRAB
                ElseIf InStr(dsCLE15.CLE15(0).MESSAGE, "3021") > 0 Then
                    CManejadorMensajes.DarMensaje(179, strerror)
                    dsCLE15.CLE15(0).MESSAGE = "Error Segundo Nombre no es válido. " + strerror + ": " + dsCLE15.CLE15(0).I_XNOMTRAB
                End If
                ManejarErrorSalida(dsCLE15.CLE15(0).MESSAGE)
                Return True
            Else ''No hay Error
                Return False
            End If
        Else
            Return True
        End If

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta función busca y maneja errores en un dataset fuertemente tipado.
    ''' </summary>
    ''' <param name="dsAFE23">DataSet en el que se van a buscar los errores</param>
    ''' <param name="pUbicacion">Ubicación del método que llama a este método </param>
    ''' <returns>True si hubo errores 
    ''' False de lo contrario </returns>
    ''' <remarks>
    ''' Estos errores son el resultado de enviar una transaccion al host ES7000 por medio del 
    ''' web service AF
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function BuscarManejarErrores(ByVal dsAFE23 As Afe23, ByVal pUbicacion As String) As Boolean

        If dsAFE23._Error.Rows.Count > 0 Then
            ManejarErrorInterno(pUbicacion, 5, dsAFE23._Error.Rows(0)("Mensaje"))
            ManejarErrorSalida(3)
            Return True
        ElseIf dsAFE23.AFE23.Rows.Count > 0 Then
            If InStr(dsAFE23.AFE23(0).MESSAGE, "ERROR") > 0 Then
                ManejarErrorInterno(pUbicacion, dsAFE23.AFE23(0).MESSAGE, 5)
                ManejarErrorSalida(dsAFE23.AFE23(0).MESSAGE)
                Return True
            End If
        Else : Return False
        End If

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método busca y maneja errores encontrados en un dateset tipado AFE21
    ''' </summary>
    ''' <param name="dsAFE21">DataSet en el que se van a buscar los errores</param>
    ''' <param name="pUbicacion">Ubicación del método que llama a este método </param>
    ''' <returns>True si hubo errores 
    ''' False de lo contrario </returns>
    ''' <remarks>
    ''' Estos errores son el resultado de enviar una transaccion al host ES7000 por medio del 
    ''' web service AF
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function BuscarManejarErrores(ByVal dsAFE21 As Afe21, ByVal pUbicacion As String) As Boolean

        If dsAFE21._Error.Rows.Count > 0 Then
            ManejarErrorInterno(pUbicacion, 5, dsAFE21._Error.Rows(0)("Mensaje"))
            ManejarErrorSalida(3)
            Return True
        ElseIf dsAFE21.AFE21.Rows.Count > 0 Then
            If InStr(dsAFE21.AFE21(0).MESSAGE, "ERROR") > 0 Then
                ManejarErrorInterno(pUbicacion, dsAFE21.AFE21(0).MESSAGE, 5)
                ManejarErrorSalida(dsAFE21.AFE21(0).MESSAGE)
                Return True
            End If
        Else : Return False
        End If
    End Function
 

#End Region

#Region "Enumeraciones"
    Public Enum TipoFuncionFCE04
        Pagar = 0
        Desagrupar = 1
        Bloquear = 2
    End Enum
#End Region

#Region "Funciones llenar DataRow"
   

   
  
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta función mapea los datos de una afiliación a Row de CLE15
    ''' </summary>
    ''' <param name="drFila"></param>
    ''' <param name="objDatosAfiliacion"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[WEMUNEVARM]	24/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function LlenarDataRowCLE15(ByRef drFila As DtsCLE15.CLE15Row, ByVal objDatosAfiliacion As CDatosPersona, ByVal tipo As String, ByVal Medio As String, ByVal COMANDO As String) As Boolean
        Dim objBeneficiario As CBeneficiario
        Dim objTrabajador As CTrabajador
        '  Dim objEmpresa As CEmpresaAfiliado
        Try
            drFila.I_MAINT = COMANDO  ''                                                                                                  A-3
            drFila.I_CALT = Medio  ''    Número de identificación del funcionario responsable del registro                                     N_12
            drFila.I_NDIGCHE = "0"    ''  Dígito de chequeo del Nit (solo para NITS)       
            drFila.I_XDESRAZ = ""
            If drFila.I_MAINT = "INQ" Then
                If tipo = "1" Then
                    objTrabajador = objDatosAfiliacion
                    drFila.I_XIDECLI = darNombreTipoId(objTrabajador.TipoIdentificacion)    ''           Tipo de identificación del cliente en formato alfa CC, NI, TI, CE, PA, RC, UN, MS             A_2                   "
                    drFila.I_NIDECLI = objTrabajador.IdTrabajador       ''           Número de identificación del cliente                                                                               N_12
                Else
                    objBeneficiario = objDatosAfiliacion
                    drFila.I_XIDECLI = darNombreTipoId(objBeneficiario.TideBen)    ''           Tipo de identificación del cliente en formato alfa CC, NI, TI, CE, PA, RC, UN, MS             A_2                   "
                    drFila.I_NIDECLI = objBeneficiario.NideBen       ''           Número de identificación del cliente                                                                               N_12
                    drFila.I_XNUICLI = objBeneficiario.NUIPAlfa     ''            Nuip o parte alfabética de la identificación del cliente                                                      A_3
                End If
                Return True
            ElseIf drFila.I_MAINT = "CHG" Then
                Dim bl_chg As Boolean = False
                Dim bl_Fid As Boolean = False
                If tipo = "1" Then
                    objTrabajador = objDatosAfiliacion
                    drFila.I_XIDECLI = darNombreTipoId(objTrabajador.TipoIdentificacion) ''    Tipo de identificación del cliente en formato alfa CC, NI, TI, CE, PA, RC, UN, MS          
                    drFila.I_NIDECLI = objTrabajador.IdTrabajador   ''  Número de identificación del cliente               
                    drFila.I_XNUICLI = ""   ''     Nuip o parte alfabética de la identificación del cliente              

                    If drFila.I_XPRIAPE <> objTrabajador.PrimerApellido Then
                        drFila.I_XPRIAPE = objTrabajador.PrimerApellido  ''  Primer apellido del cliente   
                        bl_chg = True
                        bl_Fid = True
                    End If
                    If drFila.I_XSEGAPE <> objTrabajador.SegundoApellido Then
                        drFila.I_XSEGAPE = objTrabajador.SegundoApellido  ''   Segundo apellido del cliente   
                        bl_chg = True
                        bl_Fid = True
                    End If
                    If drFila.I_XNOMTRAB <> objTrabajador.Nombres Then
                        drFila.I_XNOMTRAB = objTrabajador.Nombres  '' Nombres del cliente  
                        bl_chg = True
                        bl_Fid = True
                    End If
                    If drFila.I_XGENCLI <> objTrabajador.Genero Then
                        drFila.I_XGENCLI = objTrabajador.Genero  '' Género del cliente M, F    
                        bl_chg = True
                        bl_Fid = True
                    End If
                    If drFila.I_FNACCLI <> CType(Convert.ToDateTime(objTrabajador.FechaNacimiento, mFormatter).ToString("yyyyMMdd"), String) Then
                        drFila.I_FNACCLI = CType(Convert.ToDateTime(objTrabajador.FechaNacimiento, mFormatter).ToString("yyyyMMdd"), String)
                        bl_chg = True
                        bl_Fid = True
                    End If

                    If drFila.I_CECICLI <> objTrabajador.EstadoCivil Then
                        drFila.I_CECICLI = objTrabajador.EstadoCivil          ''           Estado Civil Cliente SO, CA, SE, UL, VI    
                        bl_chg = True
                        bl_Fid = True
                    End If
                    drFila.I_XTIPDIR = objTrabajador.TipoZona    ''                 Tipo de Dirección          Rural / Urbana
                    If drFila.I_XDIRCLI.ToUpper() <> objTrabajador.Direccion.ToUpper() Then
                        drFila.I_XDIRCLI = objTrabajador.Direccion.ToUpper()  ''      Dirección
                        bl_chg = True
                    End If
                    If drFila.I_XBARCLI <> objTrabajador.Barrio Then '                   Barrio
                        drFila.I_XBARCLI = objTrabajador.Barrio
                        bl_chg = True
                    End If
                    If drFila.I_NTELCLI <> objTrabajador.Telefono Then
                        drFila.I_NTELCLI = objTrabajador.Telefono
                        bl_chg = True
                    End If
                    ' MODIFICADO LCARDENAS 12-10-2011: GENERO EXCEPCION CUANDO LA VARIABLE .ExtensionTelef SE ENCONTRABA VACIA
                    drFila.I_NEXTCLI = IIf(objTrabajador.ExtensionTelef Is Nothing, String.Empty, objTrabajador.ExtensionTelef).ToString() ''               Extensión
                    If drFila.I_CCIUCLI <> objTrabajador.Ciudad Then      ''                     Código de Ciudad
                        drFila.I_CCIUCLI = objTrabajador.Ciudad
                        bl_chg = True
                    End If
                    drFila.I_CZONBTA = "999" ''objTrabajador.Zona''                Zona
                Else
                    objBeneficiario = objDatosAfiliacion
                    drFila.I_XIDECLI = darNombreTipoId(objBeneficiario.TideBen)    ''           Tipo de identificación del cliente en formato alfa CC, NI, TI, CE, PA, RC, UN, MS     
                    drFila.I_NIDECLI = objBeneficiario.NideBen       ''           Número de identificación del cliente                                         
                    drFila.I_XNUICLI = objBeneficiario.NUIPAlfa     ''            Nuip o parte alfabética de la identificación del cliente       
                    If drFila.I_XPRIAPE <> objBeneficiario.PrimerApellido Then
                        drFila.I_XPRIAPE = objBeneficiario.PrimerApellido        ''       Primer apellido del cliente        
                        bl_chg = True
                        bl_Fid = True
                    End If
                    If drFila.I_XSEGAPE <> objBeneficiario.SegundoApellido Then
                        drFila.I_XSEGAPE = objBeneficiario.SegundoApellido  ''         Segundo apellido del cliente      
                        bl_chg = True
                        bl_Fid = True
                    End If
                    If drFila.I_XNOMTRAB <> objBeneficiario.Nombres Then
                        drFila.I_XNOMTRAB = objBeneficiario.Nombres    ''       Nombres del cliente         
                        bl_chg = True
                        bl_Fid = True
                    End If
                    If drFila.I_XGENCLI <> objBeneficiario.Genero Then
                        drFila.I_XGENCLI = objBeneficiario.Genero     ''    Género del cliente M, F          
                        bl_chg = True
                        bl_Fid = True
                    End If
                    If drFila.I_FNACCLI <> CType(Convert.ToDateTime(objBeneficiario.FechaNacimiento, mFormatter).ToString("yyyyMMdd"), String) Then
                        drFila.I_FNACCLI = CType(Convert.ToDateTime(objBeneficiario.FechaNacimiento, mFormatter).ToString("yyyyMMdd"), String) '' Estado Civil Cliente SO, CA, SE, UL, VI   
                        bl_chg = True
                        bl_Fid = True
                    End If
                    ''dac 20110907 Se ingresa el dato faltante ??? I_CCIUCLI
                    If Not drFila.I_CCIUCLI Is Nothing Then
                        If drFila.I_CCIUCLI <> objBeneficiario.Ciudad And Not objBeneficiario.Ciudad Is Nothing Then
                            drFila.I_CCIUCLI = objBeneficiario.Ciudad
                            bl_chg = True
                            bl_Fid = True
                        End If
                    Else
                        drFila.I_CCIUCLI = objBeneficiario.Ciudad
                    End If
                    'DAC 20110907         
                End If
                If Not bl_chg Then
                    drFila.I_MAINT = ""
                Else
                    If bl_Fid = True Then
                        drFila.I_MAINT = "CHGF"
                    Else

                    End If
                    drFila.I_XISP = ""
                    drFila.I_XINDOPC = ""
                    drFila.I_CGRUCAM = "0"
                    drFila.I_XVARRAN = ""
                    drFila.I_XPRGEQU = ""
                    drFila.I_FRET = "0"
                    drFila.I_CPRGSRV = ""
                    drFila.I_NCONAFI = "0"
                    drFila.I_XNOMPRG = ""
                    drFila.I_XACC1 = ""
                    drFila.I_XNMNCA1 = ""
                    drFila.I_FINIVI1 = "0"
                    drFila.I_FFINVI1 = "0"
                    drFila.I_VNUMDA1 = "0"
                    drFila.I_XALFDA1 = ""
                    drFila.I_XACC2 = ""
                    drFila.I_XNMNCA2 = ""
                    drFila.I_FINIVI2 = "0"
                    drFila.I_FFINVI2 = "0"
                    drFila.I_VNUMDA2 = "0"
                    drFila.I_XALFDA2 = ""
                    drFila.I_XACC3 = ""
                    drFila.I_XNMNCA3 = ""
                    drFila.I_FINIVI3 = "0"
                    drFila.I_FFINVI3 = "0"
                    drFila.I_VNUMDA3 = "0"
                    drFila.I_XALFDA3 = ""
                    drFila.I_XACC4 = ""
                    drFila.I_XNMNCA4 = ""
                    drFila.I_FINIVI4 = "0"
                    drFila.I_FFINVI4 = "0"
                    drFila.I_VNUMDA4 = "0"
                    drFila.I_XALFDA4 = ""
                    drFila.I_XACC5 = ""
                    drFila.I_XNMNCA5 = ""
                    drFila.I_FINIVI5 = "0"
                    drFila.I_FFINVI5 = "0"
                    drFila.I_VNUMDA5 = "0"
                    drFila.I_XALFDA5 = ""
                    drFila.I_XACC6 = ""
                    drFila.I_XNMNCA6 = ""
                    drFila.I_FINIVI6 = "0"
                    drFila.I_FFINVI6 = "0"
                    drFila.I_VNUMDA6 = "0"
                    drFila.I_XALFDA6 = ""
                    drFila.I_XACC7 = ""
                    drFila.I_XNMNCA7 = ""
                    drFila.I_FINIVI7 = "0"
                    drFila.I_FFINVI7 = "0"
                    drFila.I_VNUMDA7 = "0"
                    drFila.I_XALFDA7 = ""
                    drFila.I_XACC8 = ""
                    drFila.I_XNMNCA8 = ""
                    drFila.I_FINIVI8 = "0"
                    drFila.I_FFINVI8 = "0"
                    drFila.I_VNUMDA8 = "0"
                    drFila.I_XALFDA8 = ""
                    drFila.I_XACC9 = ""
                    drFila.I_XNMNCA9 = ""
                    drFila.I_FINIVI9 = "0"
                    drFila.I_FFINVI9 = "0"
                    drFila.I_VNUMDA9 = "0"
                    drFila.I_XALFDA9 = ""
                    drFila.I_XACC10 = ""
                    drFila.I_XNMNCA10 = ""
                    drFila.I_FINIVI10 = "0"
                    drFila.I_FFINVI10 = "0"
                    drFila.I_VNUMDA10 = "0"
                    drFila.I_XALFDA10 = ""
                    drFila.I_XACC11 = ""
                    drFila.I_XNMNCA11 = ""
                    drFila.I_FINIVI11 = "0"
                    drFila.I_FFINVI11 = "0"
                    drFila.I_VNUMDA11 = "0"
                    drFila.I_XALFDA11 = ""
                    drFila.I_XACC12 = ""
                    drFila.I_XNMNCA12 = ""
                    drFila.I_FINIVI12 = "0"
                    drFila.I_FFINVI12 = "0"
                    drFila.I_VNUMDA12 = "0"
                    drFila.I_XALFDA12 = ""
                    drFila.I_XIDENUE = ""
                    drFila.I_NIDENUE = "0"
                    drFila.I_XNUINUE = ""
                    drFila.I_NSUCNUE = "0"
                    drFila.I_NCOSNUE = "0"
                    drFila.I_NRES003 = "0"
                    drFila.I_NRES004 = "0"
                    drFila.I_TRES001 = ""
                    drFila.I_NRES001 = "0"
                    drFila.I_TRES002 = ""
                    drFila.I_NRES002 = "0"
                    drFila.I_VVIN = "0"
                    drFila.I_XESCVIN = ""
                    drFila.I_CESTVIN = "0"
                    drFila.I_NDIRVIN = "0"
                    drFila.I_FINIAFI = "0"
                    'If drFila.I_FINGEMP = "" Then
                    '    drFila.I_FINGEMP = "0" ' DAc 20110907, se inicializa la variable
                    'End If
                    If drFila.I_CCAUINA = "" Then
                        drFila.I_CCAUINA = "0" ' DAc 20110907, se inicializa la variable
                    End If
                    If drFila.I_NTELCLI = "" Then
                        drFila.I_NTELCLI = "0" ' DAc 20110907, se inicializa la variable
                    End If
                    ' MODIFICADO LCARDENAS 12-10-2011: GENERO EXCEPCION CUANDO LA VARIABLE .ExtensionTelef SE ENCONTRABA VACIA
                    If IIf(drFila.I_NEXTCLI Is System.DBNull.Value, String.Empty, drFila.I_NEXTCLI).ToString.Equals(String.Empty) Then
                        drFila.I_NEXTCLI = "0" ' DAc 20110907, se inicializa la variable
                    End If
                    If drFila.I_CZONBTA = "" Then
                        drFila.I_CZONBTA = "999" ' DAc 20110907, se inicializa la variable
                    End If
                    If drFila.I_CCIUCLI = "" Then ' DAc 20110907, se inicializa la variable
                        drFila.I_CCIUCLI = "0" ' DAc 20110907, se inicializa la variable
                    End If
                    drFila.I_FFINAFI = "0"
                    drFila.I_CRETAFI = "0"
                    drFila.I_FVIN = "0"
                    drFila.I_QVIN = "0"
                    drFila.I_XVIN = ""
                    drFila.I_XALFADI = ""
                    drFila.MESSAGE = ""
                End If
                Return True
            ElseIf drFila.I_MAINT = "ADD" Then
                If tipo = "1" Then
                    objTrabajador = objDatosAfiliacion
                    drFila.I_XIDECLI = darNombreTipoId(objTrabajador.TipoIdentificacion)    ''           Tipo de identificación del cliente en formato alfa CC, NI, TI, CE, PA, RC, UN, MS             A_2                   "
                    drFila.I_NIDECLI = objTrabajador.IdTrabajador       ''           Número de identificación del cliente                                                                               N_12
                    drFila.I_XNUICLI = ""   ''            Nuip o parte alfabética de la identificación del cliente                                                      A_3
                    drFila.I_XPRIAPE = objTrabajador.PrimerApellido        ''       Primer apellido del cliente                                                                                            A_15
                    drFila.I_XSEGAPE = objTrabajador.SegundoApellido  ''         Segundo apellido del cliente                                                                                         A_15
                    drFila.I_XNOMTRAB = objTrabajador.Nombres    ''       Nombres del cliente                                                                                                     A_30
                    drFila.I_XGENCLI = objTrabajador.Genero     ''          Género del cliente M, F                                                                                               A_1
                    drFila.I_FNACCLI = CType(Convert.ToDateTime(objTrabajador.FechaNacimiento, mFormatter).ToString("yyyyMMdd"), String)
                    '' o()    ''         Fecha de nacimiento del cliente Formato AAAAMMDD                                                  N_8
                    drFila.I_CECICLI = objTrabajador.EstadoCivil    ''    Estado Civil Cliente SO, CA, SE, UL, VI                                                                        A-2
                    drFila.I_XTIPDIR = objTrabajador.TipoZona    ''    Tipo de Dirección          Rural / Urbana
                    drFila.I_XDIRCLI = objTrabajador.Direccion  ''    Dirección
                    drFila.I_XBARCLI = objTrabajador.Barrio     ''      Barrio
                    drFila.I_NTELCLI = objTrabajador.Telefono    ''     Teléfono
                    drFila.I_NEXTCLI = objTrabajador.ExtensionTelef      ''               Extensión
                    drFila.I_CCIUCLI = objTrabajador.Ciudad    ''       Código de Ciudad
                    drFila.I_CZONBTA = "999" ''objTrabajador.Zona''   Zona
                Else
                    objBeneficiario = objDatosAfiliacion
                    drFila.I_XIDECLI = darNombreTipoId(objBeneficiario.TideBen)    ''           Tipo de identificación del cliente en formato alfa CC, NI, TI, CE, PA, RC, UN, MS             A_2                   "
                    drFila.I_NIDECLI = objBeneficiario.NideBen       ''           Número de identificación del cliente                                                                               N_12
                    drFila.I_XNUICLI = objBeneficiario.NUIPAlfa     ''            Nuip o parte alfabética de la identificación del cliente                                                      A_3
                    drFila.I_XPRIAPE = objBeneficiario.PrimerApellido        ''       Primer apellido del cliente                                                                                            A_15
                    drFila.I_XSEGAPE = objBeneficiario.SegundoApellido  ''         Segundo apellido del cliente                                                                                         A_15
                    drFila.I_XNOMTRAB = objBeneficiario.Nombres    ''       Nombres del cliente                                                                                                     A_30
                    drFila.I_XGENCLI = objBeneficiario.Genero     ''          Género del cliente M, F                                                                                               A_1
                    drFila.I_FNACCLI = CType(Convert.ToDateTime(objBeneficiario.FechaNacimiento, mFormatter).ToString("yyyyMMdd"), String)
                    ''objBeneficiario.FechaNacimiento()    ''         Fecha de nacimiento del cliente Formato AAAAMMDD                                                  N_8
                    drFila.I_CECICLI = objBeneficiario.EstadoCivil     ''           Estado Civil Cliente SO, CA, SE, UL, VI                                                                        A-2
                    drFila.I_XTIPDIR = objBeneficiario.TipoZona    ''                 Tipo de Dirección          Rural / Urbana
                    drFila.I_XDIRCLI = objBeneficiario.Direccion                  ''      Dirección
                    drFila.I_XBARCLI = objBeneficiario.Barrio    ''                   Barrio
                    drFila.I_NTELCLI = objBeneficiario.Telefono   ''                  Teléfono
                    drFila.I_NEXTCLI = objBeneficiario.ExtensionTelef     ''               Extensión
                    drFila.I_CCIUCLI = objBeneficiario.Ciudad  ''                     Código de Ciudad
                    drFila.I_CZONBTA = "999" ''objTrabajador.Zona    ''                Zona
                End If
                Return True
            End If
            '' Return True
        Catch ex As Exception
            'CREADO LCARDENAS 19-04-2012: SEGUIMIENTO INCIDENCIA 9430 INTERMITENCIA WSCOMCLIENTES
            If ConfigurationSettings.AppSettings("LogAuditoriaAfiliar") = "S" Then
                Dim objCache As New Compensar.Vincula.POS.ClsCache
                Dim sInnerExcepcion As String = ""
                If Not ex.InnerException Is Nothing Then
                    sInnerExcepcion = ex.InnerException.ToString()
                End If
                objCache.guardarErrorBitacora(ConfigurationSettings.AppSettings("ProjectID"), _
1, "Auditoria Afiliar: ManejadorTransacciones.LlenarDataRowCLE15: " & ex.Message & sInnerExcepcion & "StackTrace: " & ex.StackTrace, "", "", "", My.Computer.Name, 1, 1)
            End If
            ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones::LlenarDataRowPOSBeneficiario", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Llena la información a enviar al Webservice de Compensar para hacer una 
    ''' afiliacion  POS de un beneficiario
    ''' </summary>
    ''' <param name="drFila"></param>
    ''' <param name="pobjDatosAfiliacion"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/9/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function LlenarDataRowPOSBeneficiario(ByRef drFila As Afe23.AFE23Row, ByRef pobjDatosAfiliacion As CDatosAfiliacion) As Boolean
        Dim objBeneficiario As CBeneficiario
        Try
            objBeneficiario = pobjDatosAfiliacion.Persona
            drFila.I_NIDETRA = objBeneficiario.IdTrabajador
            drFila.I_TIDETRA = objBeneficiario.TipoId
            drFila.I_NIDEBEN = objBeneficiario.NumeroIdentificacion
            drFila.I_TIDEBEN = objBeneficiario.TipoIdentificacion
            drFila.I_XPRIAPEB = objBeneficiario.PrimerApellido
            drFila.I_XSEGAPEB = objBeneficiario.SegundoApellido
            drFila.I_XNOMBENE = objBeneficiario.Nombres
            drFila.I_XSEX = objBeneficiario.Genero
            drFila.I_CPRN = objBeneficiario.Parentezco
            drFila.I_FNACBEN = CType(Convert.ToDateTime(objBeneficiario.FechaNacimiento, mFormatter).ToString("yyyyMMdd"), String)
            drFila.I_EDAT = CType(GetAge(Convert.ToDateTime(objBeneficiario.FechaNacimiento, mFormatter)), String)
            drFila.I_CINDDIS = IIf(objBeneficiario.Discapacidad, "1", "0")
            drFila.I_XTIPDIS = objBeneficiario.TipoDiscapacidad
            drFila.I_CDISPER = IIf(objBeneficiario.DiscapacidadPermanente, "1", "0")
            drFila.I_CCIU = objBeneficiario.Ciudad
            drFila.I_CEPSANT = CType(objBeneficiario.CodigoEPSAnterior, String)
            If objBeneficiario.FechaAfiliacionEPSAnterior <> "" Then
                drFila.I_FAFISSS = Convert.ToDateTime(objBeneficiario.FechaAfiliacionEPSAnterior, mFormatter).ToString("yyyyMMdd")
            Else ' se coloca la fecha actual
                drFila.I_FAFISSS = CType(Now.ToString("yyyyMMdd"), String)
                'se actualiza este valor en la entidad de negocio
                objBeneficiario.FechaAfiliacionEPSAnterior = CType(Now.ToString("yyyy/MM/dd"), String)
                pobjDatosAfiliacion.Persona = objBeneficiario
            End If
            drFila.I_CAFIBEN = pobjDatosAfiliacion.Clase
            drFila.I_NDOCBEN = objBeneficiario.NideBen
            drFila.I_TDOCBEN = objBeneficiario.TideBen
            drFila.I_CTIPNUI = objBeneficiario.NUIPAlfa
            drFila.I_CEPSACT = "008"
            drFila.I_CCAU = "00"
            drFila.I_CCAMCNF = "2"
            drFila.I_CCAM = "2"
            If objBeneficiario.FechaAfiliacionPOS <> "" Then
                drFila.I_FCOM = Convert.ToDateTime(objBeneficiario.FechaAfiliacionPOS, mFormatter).ToString("yyyyMMdd")
            Else
                drFila.I_FCOM = Now.ToString("yyyyMMdd")
            End If
            Return True
        Catch ex As Exception
            ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones::LlenarDataRowPOSBeneficiario", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta función mapea los datos de una afiliación POS que vienen en los objetos de tipo 
    ''' Ctrabajador y CEmpresa a un datarow del dataset afe21 para que luego sea enviado
    ''' este xml al método ISPEC del WS.
    ''' </summary>
    ''' <param name="drFila">Fila en la que se van a copiar los datos</param>
    ''' <param name="pobjDatosAfiliacion">Objeto trabajador del cual se va a copiar los datos</param>
    ''' <returns>Retorna True si la operación fue exitosa 
    ''' False de lo contrario.
    ''' </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function LlenarDataRowPOSTrabajador(ByRef drFila As Afe21.AFE21Row, ByVal pobjDatosAfiliacion As CDatosAfiliacion) As Boolean
        Dim objTrabajador As CTrabajador
        Dim objEmpresa As CEmpresaAfiliado
        Try
            objTrabajador = pobjDatosAfiliacion.Persona
            objEmpresa = pobjDatosAfiliacion.Empresa
            drFila.I_NIDETRA = objTrabajador.IdTrabajador
            drFila.I_TIDETRA = objTrabajador.TipoIdentificacion
            drFila.I_XPRIAPE = objTrabajador.PrimerApellido
            drFila.I_XSEGAPE = objTrabajador.SegundoApellido
            drFila.I_XNOMBRE = objTrabajador.Nombres
            drFila.I_XSEX = objTrabajador.Genero
            drFila.I_CESTCIV = objTrabajador.EstadoCivil
            drFila.I_FNACTRA = Convert.ToDateTime(objTrabajador.FechaNacimiento, mFormatter).ToString("yyyyMMdd")
            drFila.I_EDAT = Me.GetAge(Convert.ToDateTime(objTrabajador.FechaNacimiento, mFormatter))
            drFila.I_XDIRCAS = objTrabajador.Direccion
            drFila.I_XCLAZON = objTrabajador.TipoZona
            If objTrabajador.Telefono <> "" And Not objTrabajador.Telefono Is Nothing Then
                drFila.I_NTELCAS = objTrabajador.Telefono
            Else
                drFila.I_NTELCAS = "0"
            End If
            drFila.I_XBARRIO = objTrabajador.Barrio
            drFila.I_CCIU = objTrabajador.Ciudad
            drFila.I_COCUTRA = objTrabajador.CodigoOcupacion
            drFila.I_TIDEEMP = objEmpresa.TipoIdentificacion
            drFila.I_NIDEEMP = objEmpresa.IdEmpresa
            drFila.I_CDEP = objEmpresa.Dependencia
            drFila.I_FINGEMP = CType(Convert.ToDateTime(objEmpresa.FechaIngreso, mFormatter).ToString("yyyyMMdd"), String)
            drFila.I_CCOL = objEmpresa.CentroDeCostos
            drFila.I_XNOMCARG = objEmpresa.CargoTrabajador
            drFila.I_VSALTRA = objEmpresa.SalarioPOS
            drFila.I_CESTSAL = Math.Floor(objEmpresa.SalarioPOS / 100000)
            drFila.I_NIDEVER = objTrabajador.IdTrabajador
            drFila.I_CCAMCNF = "2"
            drFila.I_CCAM = "2"
            'Información por defecto de POS
            drFila.I_NHORCON = "240"
            drFila.I_CTIPVIN = pobjDatosAfiliacion.Clase  ''"1"
            drFila.I_XTIPDIS = "N"
            drFila.I_NDIRCOR = "1"
            drFila.I_CEPSACT = "008"
            drFila.I_CPROEPS = "999"
            drFila.I_NCUECON = "0"
            'Informacion propia de POS
            If objTrabajador.FechaAfiliacionPOS <> "" Then
                drFila.I_FCOM = Convert.ToDateTime(objTrabajador.FechaAfiliacionPOS, mFormatter).ToString("yyyyMMdd")
            Else
                drFila.I_FCOM = Now.ToString("yyyyMMdd")
            End If
            drFila.I_FAFISSS = Convert.ToDateTime(objTrabajador.FechaAfiliacionSistema, mFormatter).ToString("yyyyMMdd")
            drFila.I_CEPSANT = objTrabajador.CodigoEPSAnterior
            drFila.I_CCABFAM = IIf(objTrabajador.CabezaFamilia, "1", "0")
            If objTrabajador.ConyugeCotiza Then
                drFila.I_NCEDCYG = objTrabajador.NumeroIdentificacionConyuge
                drFila.I_NFIC = objTrabajador.TipoIdentificacionConyuge
                drFila.I_VSALCYG = objTrabajador.SalarioConyuge
            End If
            ''/////////////////////////////////////////////////
            If pobjDatosAfiliacion.novedadTDA Then  'es un traslado
                If pobjDatosAfiliacion.mesAport = 2 Then '' El caso especial...
                    drFila.I_CINDTDA = "2"
                    drFila.I_CAFITRA = "2"
                Else
                    If drFila.I_CTIPVIN = 2 Then ''ES UN INDEPENDIENTE
                        drFila.I_CINDTDA = "0"
                        drFila.I_CAFITRA = "2"
                    Else ''NO ES INDEPENDIENTE
                        drFila.I_CINDTDA = "1"
                        drFila.I_CAFITRA = "2"
                    End If
                End If
            Else
                If drFila.I_CTIPVIN = 2 Then ''ES UN INDEPENDIENTE
                    drFila.I_CINDTDA = "0"
                    drFila.I_CAFITRA = "1"
                Else ''NO ES INDEPENDIENTE
                    drFila.I_CINDTDA = "1"
                    drFila.I_CAFITRA = "1"
                End If
            End If
            ''///////////////////////////////////////////////////
            Return True
        Catch ex As Exception
            ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones::LlenarDataRowPOSTrabajador", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Overload. Guarda los datos de un trabajador en un dataset tipado EGE05
    ''' </summary>
    ''' <param name="drFila">Fila en la que se van a copiar los datos</param>
    ''' <param name="objTrabajador">Objeto bneficiario del cual se va a copiar los datos</param>
    ''' <param name="objEmpresa">Objeto Empresa del cual se van a copiar los datos</param>
    ''' <returns>Retorna True si la operación fue exitosa 
    ''' False de lo contrario.
    '''  </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	01/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function LlenarDataRowCajaTrabajador(ByRef drFila As Ege05.EGE05Row, ByVal objTrabajador As CTrabajador, ByVal objEmpresa As CEmpresaAfiliado, ByVal pobjDatosAfiliacion As CDatosAfiliacion) As Boolean

        Try
            drFila.I_TIDETRA = objTrabajador.TipoIdentificacion
            drFila.I_NIDETRA = objTrabajador.IdTrabajador
            drFila.I_TIDESUC = objEmpresa.TipoIdentificacion
            drFila.I_NIDESUC = objEmpresa.IdEmpresa
            drFila.I_CTIPVIN = pobjDatosAfiliacion.Clase
            If objEmpresa.Dependencia <> "" And Not objEmpresa.Dependencia Is Nothing Then
                drFila.I_CDEPSUC = objEmpresa.Dependencia
            End If
            If objEmpresa.CentroDeCostos <> "" And Not objEmpresa.CentroDeCostos Is Nothing Then
                drFila.I_CCOL = objEmpresa.CentroDeCostos
            End If

            If objEmpresa.FechaIngreso <> "" And Not objEmpresa.FechaIngreso Is Nothing Then
                drFila.I_FINGEMP = (CType(Convert.ToDateTime(objEmpresa.FechaIngreso, mFormatter).ToString("yyyyMMdd"), String))
            End If
            If objEmpresa.HorasLaborMes > 0 Then
                drFila.I_NHORSUC = objEmpresa.HorasLaborMes
            End If
            If objEmpresa.CargoTrabajador <> "" And Not objEmpresa.CargoTrabajador Is Nothing Then
                drFila.I_XCARSUC = objEmpresa.CargoTrabajador
            End If
            If objEmpresa.SalarioCaja > 0 Then
                drFila.I_VSALSUC = objEmpresa.SalarioCaja
                drFila.I_VSUETRA = objEmpresa.SalarioCaja
            End If
            drFila.I_TAFICAJ = "1"
            Return True
        Catch ex As Exception
            ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones::LlenarDataRowCajaTrabajador", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Overload. Guarda los datos de un trabajador en un dataset tipado EGE05
    ''' </summary>
    ''' <param name="drFila">Fila en la que se van a copiar los datos</param>
    ''' <param name="pobjDatosAfiliacion">Objeto trabajador del cual se va a copiar los datos</param>
    ''' <returns>Retorna True si la operación fue exitosa 
    ''' False de lo contrario.
    '''  </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/7/2004	Created
    ''' </history> 
    ''' -----------------------------------------------------------------------------
    Private Function LlenarDataRowPOSTrabajador(ByRef drFila As Ege05.EGE05Row, ByVal pobjDatosAfiliacion As CDatosAfiliacion) As Boolean
        Dim objTrabajador As CTrabajador
        Dim objEmpresa As CEmpresaAfiliado
        Try
            objTrabajador = pobjDatosAfiliacion.Persona
            objEmpresa = pobjDatosAfiliacion.Empresa
            drFila.I_TIDETRA = objTrabajador.TipoIdentificacion
            drFila.I_NIDETRA = objTrabajador.IdTrabajador
            drFila.I_TIDESUC = objEmpresa.TipoIdentificacion
            drFila.I_NIDESUC = objEmpresa.IdEmpresa
            If objEmpresa.Dependencia <> "" And Not objEmpresa.Dependencia Is Nothing Then
                drFila.I_CDEPSUC = objEmpresa.Dependencia
            End If
            If objEmpresa.CentroDeCostos <> "" And Not objEmpresa.CentroDeCostos Is Nothing Then
                drFila.I_CCOL = objEmpresa.CentroDeCostos
            End If

            If objEmpresa.FechaIngreso <> "" Then
                drFila.I_FINGEMP = CType(Convert.ToDateTime(objEmpresa.FechaIngreso, mFormatter).ToString("yyyyMMdd"), String)
            Else
                drFila.I_FINGEMP = ""
            End If
            drFila.I_XCARSUC = objEmpresa.CargoTrabajador
            drFila.I_VSALSUC = CType(objEmpresa.SalarioPOS, String)
            drFila.I_TAFIPOS = "1"

            If objTrabajador.FechaAfiliacionEPSAnterior <> "" Then
                drFila.I_FINGPOS = CType(Convert.ToDateTime(objTrabajador.FechaAfiliacionEPSAnterior, mFormatter).ToString("yyyyMMdd"), String)
            ElseIf objTrabajador.CodigoEPSAnterior = "999" Then 'El trabajador no tiene eps anterior
                drFila.I_FINGPOS = Now.ToString("yyyyMMdd")
            Else
                ManejoMensajes.CManejadorMensajes.PublicarMensaje("objTrabajador.FechaAfiliacionEPSAnterior=" & objTrabajador.FechaAfiliacionEPSAnterior & ", objTrabajador.CodigoEPSAnterior= " & objTrabajador.CodigoEPSAnterior, EventLogEntryType.Warning)
                drFila.I_FINGPOS = CType(Convert.ToDateTime(objTrabajador.FechaAfiliacionSistema, mFormatter).ToString("yyyyMMdd"), String)
                ''objTrabajador.FechaAfiliacionSistema
            End If
            drFila.I_NHORSUC = "240"
            drFila.I_FRETTRA = "0"
            drFila.I_CTIPVIN = pobjDatosAfiliacion.Clase
            If drFila.I_CTIPVIN = 2 Then
                If pobjDatosAfiliacion.novedadTDA Then 'es un traslado
                    drFila.I_CINDTDA = "2"
                Else
                    drFila.I_CINDTDA = "0"
                End If
            Else
                If pobjDatosAfiliacion.novedadTDA Then 'es un traslado
                    drFila.I_CINDTDA = "2"
                Else
                    drFila.I_CINDTDA = "1"
                End If
            End If
            Return True
        Catch ex As Exception
            ManejarErrorInterno("LogicaTransaccional::ManejadorTransacciones::LlenarDataRowPOSTrabajador", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try

    End Function

#End Region

#Region "Funciones ReadXml"
    Private Function ReadXml(ByRef p_dsAsilTl As Afiliacion, ByVal p_reader As TextReader) As Boolean

        Dim ds As New DataSet

        ds.ReadXml(p_reader)
        ds.Namespace = "http://tempuri.org/AfiliaTl.xsd"
        p_dsAsilTl.Clear()
        p_dsAsilTl.Merge(ds, False, System.Data.MissingSchemaAction.Add)

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Carga una cadena xml en un dataset fuertemente tipado
    ''' </summary>
    ''' <param name="p_dsAfe23"></param>
    ''' <param name="p_reader"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function ReadXml(ByRef p_dsAfe23 As Afe23, ByVal p_reader As TextReader) As Boolean

        Dim ds As New DataSet

        ds.ReadXml(p_reader)
        ds.Namespace = "http://tempuri.org/Afe23.xsd"
        p_dsAfe23.Clear()
        p_dsAfe23.Merge(ds, False, System.Data.MissingSchemaAction.Add)

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Carga una cadena xml en un dataset fuertemente tipado
    ''' </summary>
    ''' <param name="p_dsAfe21"></param>
    ''' <param name="p_reader"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	03/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function ReadXml(ByRef p_dsAfe21 As Afe21, ByVal p_reader As TextReader) As Boolean

        Dim ds As New DataSet

        ds.ReadXml(p_reader)
        ds.Namespace = "http://tempuri.org/Afe21.xsd"
        p_dsAfe21.Clear()
        p_dsAfe21.Merge(ds, False, System.Data.MissingSchemaAction.Add)

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="p_dsCle15"></param>
    ''' <param name="p_reader"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[WEMUNEVARM]	24/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function ReadXml(ByRef p_dsCle15 As DtsCLE15, ByVal p_reader As TextReader) As Boolean

        Dim ds As New DataSet

        ds.ReadXml(p_reader)
        ds.Namespace = "http://tempuri.org/Cle15.xsd"
        p_dsCle15.Clear()
        p_dsCle15.Merge(ds, False, System.Data.MissingSchemaAction.Add)

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Carga una cadena xml en un dataset fuertemente tipado
    ''' </summary>
    ''' <param name="p_dsEGE05"></param>
    ''' <param name="p_reader"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function ReadXml(ByRef p_dsEGE05 As Ege05, ByVal p_reader As TextReader) As Boolean

        Dim ds As New DataSet

        ds.ReadXml(p_reader)
        ds.Namespace = "http://tempuri.org/EGE05.xsd"
        p_dsEGE05.Clear()
        p_dsEGE05.Merge(ds, False, System.Data.MissingSchemaAction.Add)

    End Function

    




#Region " Funciones Guardar Datos"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Guarda los datos de una afiliación recién hecha.
    ''' </summary>
    ''' <param name="p_objAfiliacion"></param>
    ''' <param name="pstrComando"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	01/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function GuardarDatosAfiliacionCAJA(ByRef p_objAfiliacion As CDatosAfiliacion, ByVal pstrComando As String) As String
        Dim strMensaje As String = ""
        Dim objBeneficiario As New CBeneficiario
        Dim objTrabajador As New CTrabajador

        p_objAfiliacion.Comando = pstrComando
        p_objAfiliacion.Fecha = DateTime.Now.ToString(Me.mFormatter)
        p_objAfiliacion.Tipo = TipoAfiliacion.Caja

        If p_objAfiliacion.Persona.GetType.FullName = objBeneficiario.GetType.FullName Then
            CManejadorMensajes.DarMensaje("26", strMensaje)
        ElseIf p_objAfiliacion.Persona.GetType.FullName = objTrabajador.GetType.FullName Then
            CManejadorMensajes.DarMensaje("10", strMensaje)
        End If

        Return strMensaje
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Guarda los datos transaccionales de una afiliación POS recién hecha.
    ''' </summary>
    ''' <param name="p_objAfiliacion"></param>
    ''' <param name="pstrComando"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/7/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub GuardarDatosAfiliacionPOS(ByRef p_objAfiliacion As CDatosAfiliacion, ByVal pstrComando As String)
        'Dim strMensaje As String
        p_objAfiliacion.Comando = pstrComando
        p_objAfiliacion.Fecha = DateTime.Now.ToString(Me.mFormatter)
        p_objAfiliacion.Tipo = TipoAfiliacion.POS
    End Sub
#End Region

#Region "Funciones Utilitarias"
    Private Function GetAge(ByVal Birthdate As System.DateTime, Optional ByVal AsOf As System.DateTime = #1/1/1700#) As String


        Dim iMonths As Integer
        Dim iYears As Integer
        Dim dYears As Decimal
        Dim lDayOfBirth As Long
        Dim lAsOf As Long
        Dim iBirthMonth As Integer
        Dim iAsOFMonth As Integer

        If AsOf = "#1/1/1700#" Then
            AsOf = DateTime.Now
        End If
        lDayOfBirth = DatePart(DateInterval.Day, Birthdate)
        lAsOf = DatePart(DateInterval.Day, AsOf)

        iBirthMonth = DatePart(DateInterval.Month, Birthdate)
        iAsOFMonth = DatePart(DateInterval.Month, AsOf)

        iMonths = DateDiff(DateInterval.Month, Birthdate, AsOf)

        dYears = iMonths / 12

        iYears = Math.Floor(dYears)

        If iBirthMonth = iAsOFMonth Then
            If lAsOf < lDayOfBirth Then
                iYears = iYears - 1
            End If
        End If

        Return iYears
    End Function
    ''Tipo de identificación del cliente en formato alfa CC, NI, TI, CE, PA?, RC, UN?, MS?             A_2                   
    Public Function darNombreTipoId(ByVal intCodigo As Integer) As String
        Dim strNombre As String = ""
        If intCodigo = 1 Then strNombre = "CC" ''
        If intCodigo = 2 Then strNombre = "NI" ''
        If intCodigo = 3 Then strNombre = "TI" ''
        If intCodigo = 4 Then strNombre = "CE" ''
        If intCodigo = 7 Then strNombre = "RC"
        If intCodigo = 8 Then strNombre = "NU" ''
        If intCodigo = 9 Then strNombre = "MS" ''--8992 se agrega el tipo de identificacion
        Return strNombre
    End Function

#End Region
#End Region

    Public Function TransacciontrCle15(ByRef pobjAfiliacion As CDatosAfiliacion, ByRef p_strRespuesta As String) As Boolean
        'Dim intParametro As Integer
        Try
            If Not pobjAfiliacion Is Nothing Or pobjAfiliacion.EsVacio Then
                ''If True Then
                If pobjAfiliacion.Persona.GetType.FullName = "EntidadesNegocio.CTrabajador" Then
                    'MODIFICADO LCARDENAS 27-03-2012: SOLUCION INCIDENCIA REABIERTA 8127
                    'ENVIO LA CLASE DE AFILIACION RECIBIDA EN TRAMA ONBASE
                    Return trCle15(pobjAfiliacion.Persona, p_strRespuesta, "1", pobjAfiliacion.IdUsuarioEjecutor, pobjAfiliacion.Clase)
                ElseIf pobjAfiliacion.Persona.GetType.FullName = "EntidadesNegocio.CBeneficiario" Then
                    'MODIFICADO LCARDENAS 27-03-2012: SOLUCION INCIDENCIA REABIERTA 8127
                    'ENVIO LA CLASE DE AFILIACION RECIBIDA EN TRAMA ONBASE
                    Return trCle15(pobjAfiliacion.Persona, p_strRespuesta, "2", pobjAfiliacion.IdUsuarioEjecutor, pobjAfiliacion.Clase)

                End If

            Else ' La afiliacion esta vacia
                ManejarErrorInterno("CManajeadorTransacciones::TransaccionAfiliarTrabajadorPOS", 4, "pobjAfiliacion vacío")
                ManejarErrorSalida(4)
                Return False
            End If
        Catch ex As Exception
            'CREADO LCARDENAS 19-04-2012: SEGUIMIENTO INCIDENCIA 9430 INTERMITENCIA WSCOMCLIENTES
            If ConfigurationSettings.AppSettings("LogAuditoriaAfiliar") = "S" Then
                Dim objCache As New Compensar.Vincula.POS.ClsCache
                Dim sInnerExcepcion As String = ""
                If Not ex.InnerException Is Nothing Then
                    sInnerExcepcion = ex.InnerException.ToString()
                End If
                objCache.guardarErrorBitacora(ConfigurationSettings.AppSettings("ProjectID"), _
1, "Auditoria Afiliar: ManejadorTransacciones.TransacciontrCle15: " & ex.Message & sInnerExcepcion & "StackTrace: " & ex.StackTrace, "", "", "", My.Computer.Name, 1, 1)
            End If
            ManejarErrorInterno("CManejaadorTransacciones::TransaccionAfiliarTrabajadorPOS", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try


    End Function

#Region "Transacciones Trabajador POS"
    '' -----------------------------------------------------------------------------
    '' <summary> 
    '' NO TOCARContiene toda la logica transaccional de afiliación de un trabajador a la 
    '' EPS(POS) 
    '' </summary> 
    '' <param name="p_objAfiliacion"> 
    '' Informacion de la afiliación 
    '' </param> 
    '' <returns> 
    '' True si la operación fue exitosa 
    '' False de lo contrario 
    '' </returns> 
    '' <remarks>  
    '' </remarks> 
    '' <history> 
    '' 	[CENCLOPEZB]	8/27/2004	Created 
    '' </history> 
    '' ----------------------------------------------------------------------------- 
    Public Function TransaccionAfiliarTrabajadorPOS(ByRef pobjAfiliacion As CDatosAfiliacion, ByRef p_strRespuesta As String) As Boolean
        'Dim intParametro As Integer
        Try
            If Not pobjAfiliacion Is Nothing Or pobjAfiliacion.EsVacio Then
                ''If True Then
                If trCle15(pobjAfiliacion.Persona, p_strRespuesta, "1", pobjAfiliacion.IdUsuarioEjecutor) Then

                    Dim objTrabajador As CTrabajador
                    Dim objEmpresa As CEmpresaAfiliado
                    Dim dsAFE21 As New Afe21
                    'Dim blnFlag As Boolean
                    Dim drFila As Afe21.AFE21Row
                    Dim strRespuesta As String
                    Dim strMensaje As String = ""

                    objTrabajador = pobjAfiliacion.Persona
                    objEmpresa = pobjAfiliacion.Empresa
                    drFila = dsAFE21.AFE21.NewAFE21Row()
                    drFila.I_CTIPVIN = pobjAfiliacion.Clase
                    drFila.I_TRANSAR = pobjAfiliacion.AfiliacionTransar
                    Select Case objTrabajador.EmpresaPrimaria.EstadoAfiliacionPOS
                        Case -1 ' Vacio
                            drFila.I_MAINT = "ADD"
                            If LlenarDataRowPOSTrabajador(drFila, pobjAfiliacion) Then
                                ' Enviar ADD a AFE21
                                dsAFE21.AFE21.AddAFE21Row(drFila)
                                strRespuesta = ws.Ispec(Me.strAplicacion, "AFE21", Replace(UCase(dsAFE21.GetXml()), "XMLNS", "xmlns"), intConstantePOS)
                                CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                                Me.ReadXml(dsAFE21, New StringReader(strRespuesta))
                                If BuscarManejarErrores(dsAFE21, "LogicaTransaccional::ManejadorTransacciones::TransaccionAfiliarTrabajadorPOS") Then
                                    'Hubo Errores 
                                    Return False
                                Else
                                    'Guarda mensaje retornado por el Host para seguimiento posterior
                                    pobjAfiliacion.RespuestaHost = dsAFE21.AFE21(0).MESSAGE

                                    ' Guardar datos de la afiliación recién hecha 
                                    GuardarDatosAfiliacionPOS(pobjAfiliacion, "ADD")
                                    CManejadorMensajes.DarMensaje("23", p_strRespuesta)
                                    Return True
                                End If
                            Else
                                'Error llenando DataRow
                                Return False
                            End If

                        Case 0, 4, 5, 6 ' Afiliado, carente, suspendido, cancelado o Multiafiliado

                            'Analiza si esta multiafiliado
                            If objTrabajador.EmpresaPrimaria.EstadoAfiliacionPOS = 0 And objTrabajador.EmpresaPrimaria.EstadoMultiafiliacionPOS = 9 Then
                                ' Ya esta afiliado 
                                CManejadorMensajes.DarMensaje("132", strMensaje)
                                p_strRespuesta = strMensaje
                                ManejarErrorSalida("132")
                                Return True
                            Else
                                ' AFILIADO POR LA MISMA EMPRESA ?
                                If AfiliadoMismaEmpresa(objTrabajador, objEmpresa) Then
                                    If objTrabajador.EmpresaPrimaria.EstadoAfiliacionPOS = 6 Then
                                        ' Esta cancelado
                                        CManejadorMensajes.DarMensaje("15", strMensaje)
                                        ManejarErrorSalida("15")
                                    Else
                                        ' Ya esta afiliado 
                                        CManejadorMensajes.DarMensaje("22", strMensaje)
                                        ManejarErrorSalida("22")
                                    End If
                                    p_strRespuesta = strMensaje

                                    Return True

                                Else
                                    'ConsultarEGE05
                                    If VerificacionesSegundoEmpleoPOS(pobjAfiliacion, p_strRespuesta) Then
                                        Return True
                                    Else
                                        Return False
                                    End If
                                End If
                            End If

                        Case 1 ' Retirado
                            ' AFILIADO POR LA MISMA EMPRESA ?
                            If AfiliadoMismaEmpresa(objTrabajador, objEmpresa) Then

                                If LlenarDataRowPOSTrabajador(drFila, pobjAfiliacion) Then
                                    drFila.I_MAINT = "REA"
                                    dsAFE21.AFE21.AddAFE21Row(drFila)
                                    strRespuesta = ws.Ispec(Me.strAplicacion, "AFE21", Replace(UCase(dsAFE21.GetXml()), "XMLNS", "xmlns"), intConstantePOS)
                                    CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                                    Me.ReadXml(dsAFE21, New StringReader(strRespuesta))
                                    If BuscarManejarErrores(dsAFE21, "LogicaTransaccional::ManejadorTransacciones::TransaccionAfiliarTrabajadorPOS") Then
                                        Return False
                                    Else
                                        'Guarda mensaje retornado por el Host para seguimiento posterior
                                        pobjAfiliacion.RespuestaHost = dsAFE21.AFE21(0).MESSAGE

                                        ' Guardar datos de la afiliación recién hecha 
                                        GuardarDatosAfiliacionPOS(pobjAfiliacion, "REA")
                                        CManejadorMensajes.DarMensaje("23", p_strRespuesta)
                                        Return True
                                    End If
                                Else
                                    'Error llenando DataRow
                                    Return False
                                End If
                            End If

                        Case 3 ' No Afiliado
                            If AfiliadoMismaEmpresa(objTrabajador, objEmpresa) Then
                                '1.  Enviar AFI a AFE21
                                drFila.I_MAINT = "AFI"
                                If LlenarDataRowPOSTrabajador(drFila, pobjAfiliacion) Then
                                    dsAFE21.AFE21.AddAFE21Row(drFila)
                                    strRespuesta = ws.Ispec(Me.strAplicacion, "AFE21", Replace(UCase(dsAFE21.GetXml()), "XMLNS", "xmlns"), intConstantePOS)
                                    CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                                    Me.ReadXml(dsAFE21, New StringReader(strRespuesta))
                                    If BuscarManejarErrores(dsAFE21, "LogicaTransaccional::ManejadorTransacciones::TransaccionAfiliarTrabajadorPOS") Then
                                        Return False
                                    Else
                                        'Guarda mensaje retornado por el Host para seguimiento posterior
                                        pobjAfiliacion.RespuestaHost = dsAFE21.AFE21(0).MESSAGE

                                        'Guardar datos de la afiliación recién hecha 
                                        GuardarDatosAfiliacionPOS(pobjAfiliacion, "AFI")
                                        CManejadorMensajes.DarMensaje("23", p_strRespuesta)
                                        Return True
                                    End If
                                Else
                                    Return False 'Error llenando data row
                                End If
                        
                         
                            End If
                    End Select
                Else
                    Return False
                End If
            Else ' La afiliacion esta vacia
                ManejarErrorInterno("CManajeadorTransacciones::TransaccionAfiliarTrabajadorPOS", 4, "pobjAfiliacion vacío")
                ManejarErrorSalida(4)
                Return False
            End If

        Catch ex As Exception
            ManejarErrorInterno("CManejaadorTransacciones::TransaccionAfiliarTrabajadorPOS", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try


    End Function

#End Region

#Region "Transacciones Beneficiario POS "

    ''' <summary>
    '''  NO TOCAR  Este método permite enviar la (s) transacción (es) necesaria (s) para afiliar 
    ''' un nuevo beneficiario a la EPS (POS)
    ''' </summary>
    ''' <param name="pobjAfiliacion"></param>
    ''' <param name="p_strRespuesta"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TransaccionAfiliarBeneficiarioPOS(ByRef pobjAfiliacion As CDatosAfiliacion, ByRef p_strRespuesta As String) As Boolean
        Try
            If Not pobjAfiliacion Is Nothing Or pobjAfiliacion.EsVacio Then
                ''If True Then
                If trCle15(pobjAfiliacion.Persona, p_strRespuesta, "2", pobjAfiliacion.IdUsuarioEjecutor) Then

                    Dim objBeneficiario As CBeneficiario
                    Dim objEmpresa As CEmpresaAfiliado
                    Dim dsAFE23 As New Afe23
                    'Dim blnFlag As Boolean
                    Dim drFila As Afe23.AFE23Row
                    Dim strRespuesta As String
                    Dim strMensaje As String = ""

                    objBeneficiario = pobjAfiliacion.Persona
                    objEmpresa = pobjAfiliacion.Empresa
                    drFila = dsAFE23.AFE23.NewAFE23Row()
                    drFila.I_TRANSAR = pobjAfiliacion.AfiliacionTransar
                    Select Case objBeneficiario.EstadoAfiliacionPOS
                        Case -1 ' Vacio
                            drFila.I_MAINT = "ADD"
                            If LlenarDataRowPOSBeneficiario(drFila, pobjAfiliacion) Then
                                ' Enviar ADD a AFE23
                                dsAFE23.AFE23.AddAFE23Row(drFila)
                                strRespuesta = ws.Ispec(Me.strAplicacion, "AFE23", Replace(UCase(dsAFE23.GetXml()), "XMLNS", "xmlns"), intConstantePOS)
                                CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                                Me.ReadXml(dsAFE23, New StringReader(strRespuesta))
                                If BuscarManejarErrores(dsAFE23, "LogicaTransaccional::ManejadorTransacciones::TransaccionAfiliarBeneficiarioPOS") Then
                                    'Hubo Errores 
                                    Return False
                                Else
                                    'Guarda mensaje retornado por el Host para seguimiento posterior
                                    pobjAfiliacion.RespuestaHost = dsAFE23.AFE23(0).MESSAGE

                                    ' Guardar datos de la afiliación recién hecha 
                                    GuardarDatosAfiliacionPOS(pobjAfiliacion, "ADD")
                                    CManejadorMensajes.DarMensaje("26", p_strRespuesta)
                                    Return True
                                End If
                            Else
                                'Error llenando DataRow
                                Return False
                            End If

                        Case 0, 4 ' Afiliado o carente 

                            'Analiza si esta multiafiliado
                            If objBeneficiario.EstadoAfiliacionPOS = 0 And objBeneficiario.EstadoMultiafiliacionPOS = 99 Then
                                ' Ya esta afiliado 
                                CManejadorMensajes.DarMensaje("133", strMensaje)
                                p_strRespuesta = strMensaje
                                ManejarErrorSalida("132")
                                Return True
                            Else
                                CManejadorMensajes.DarMensaje("25", strMensaje)
                                p_strRespuesta = strMensaje
                                ManejarErrorSalida(25)
                                Return True
                            End If

                        Case 1 ' Retirado

                            'Enviar REA a AFE23
                            drFila.I_MAINT = "REA"
                            If LlenarDataRowPOSBeneficiario(drFila, pobjAfiliacion) Then
                                dsAFE23.AFE23.AddAFE23Row(drFila)
                                strRespuesta = ws.Ispec(Me.strAplicacion, "AFE23", Replace(UCase(dsAFE23.GetXml()), "XMLNS", "xmlns"), intConstantePOS)
                                CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                                Me.ReadXml(dsAFE23, New StringReader(strRespuesta))
                                If BuscarManejarErrores(dsAFE23, "LogicaTransaccional::ManejadorTransacciones::TransaccionAfiliarBeneficiarioPOS") Then
                                    Return False
                                Else
                                    'Guarda mensaje retornado por el Host para seguimiento posterior
                                    pobjAfiliacion.RespuestaHost = dsAFE23.AFE23(0).MESSAGE

                                    ' Guardar datos de la afiliación recién hecha 
                                    GuardarDatosAfiliacionPOS(pobjAfiliacion, "REA")
                                    CManejadorMensajes.DarMensaje("26", p_strRespuesta)

                                    Return True
                                End If

                            Else
                                'Error llenando DataRow
                                Return False
                            End If

                        Case 3 ' No Afiliado
                            'Enviar AFI a AFE21
                            drFila.I_MAINT = "AFI"
                            If LlenarDataRowPOSBeneficiario(drFila, pobjAfiliacion) Then
                                dsAFE23.AFE23.AddAFE23Row(drFila)
                                strRespuesta = ws.Ispec(Me.strAplicacion, "AFE23", Replace(UCase(dsAFE23.GetXml()), "XMLNS", "xmlns"), intConstantePOS)
                                CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)

                                Me.ReadXml(dsAFE23, New StringReader(strRespuesta))
                                If BuscarManejarErrores(dsAFE23, "LogicaTransaccional::ManejadorTransacciones::TransaccionAfiliarBeneficiarioPOS") Then
                                    Return False
                                Else
                                    'Guarda mensaje retornado por el Host para seguimiento posterior
                                    pobjAfiliacion.RespuestaHost = dsAFE23.AFE23(0).MESSAGE

                                    ' Guardar datos de la afiliación recién hecha 
                                    GuardarDatosAfiliacionPOS(pobjAfiliacion, "AFI")
                                    CManejadorMensajes.DarMensaje("26", p_strRespuesta)
                                    Return True
                                End If
                            Else
                                'Error llenando data row
                                Return False
                            End If
                        Case 5, 6 ' Cancelado o suspendido
                            ' No se puede afiliar, saca mensaje de error
                            CManejadorMensajes.DarMensaje("17", strMensaje)
                            p_strRespuesta = strMensaje
                            ManejarErrorSalida(17)
                            Return True

                    End Select
                Else
                    Return (False)
                End If
            Else ' La afiliacion esta vacia
                ManejarErrorInterno("CManejadorTransacciones::TransaccionAfiliarBeneficiarioPOS", 4, "pobjAfiliacion vacío")
                ManejarErrorSalida(4)
                Return False
            End If


        Catch ex As Exception
            ManejarErrorInterno("CManejadorTransacciones::TransaccionAfiliarBeneficiarioPOS", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try


    End Function
#End Region


#Region "tansacciones CLE15"
    ''' <summary>
    ''' Lógica para decición de CREACION - Actualización - Consulta.
    ''' </summary>
    ''' <param name="persona"></param>
    ''' <param name="p_strRespuesta"></param>
    ''' <param name="tipo"></param>
    ''' <param name="ejecutor"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function trCle15(ByRef persona As CDatosPersona, ByRef p_strRespuesta As String, ByRef tipo As String, ByVal ejecutor As String, Optional ByVal strClaseAfil As String = "0") As Boolean
        ''varificar la existencia en 
        Dim dsCLE15 As New DtsCLE15
        Dim drFila As DtsCLE15.CLE15Row
        '' Dim strRespuesta As String
        Try
            drFila = dsCLE15.CLE15.NewCLE15Row()
            'LlenarDataRowCLE15(drFila, persona, tipo, ejecutor, "INQ")
            'dsCLE15.CLE15.AddCLE15Row(drFila)
            '' YA SE ENCUENTRA??
            If FuncionINQCLE15(dsCLE15, persona, tipo, ejecutor, "INQ") Then
                ''NUEVO
                dsCLE15.CLE15.Clear()
                LlenarDataRowCLE15(drFila, persona, tipo, ejecutor, "ADD")
                dsCLE15.CLE15.AddCLE15Row(drFila)
                'p_strRespuesta = drFila.I_NAUTCLI
                Return aplicarCle15(p_strRespuesta, dsCLE15)
            Else
                'If ejecutor = "79283801" Then
                '    p_strRespuesta = dsCLE15.CLE15(0).I_NAUTCLI ''drFila.I_NAUTCLI
                '    Return True
                'Else
                LlenarDataRowCLE15(dsCLE15.CLE15(0), persona, tipo, ejecutor, "CHG")
                If dsCLE15.CLE15(0).I_MAINT = "CHGF" Then
                    If dsCLE15.CLE15(0).I_FVALCLI.ToString() = "0" Then
                        dsCLE15.CLE15(0).I_MAINT = "CHG"
                        'Return aplicarCle15(p_strRespuesta, dsCLE15) ''se realiza llamado en linea 3591
                    Else
                        '' desfidelizo()
                        dsCLE15.CLE15(0).I_MAINT = "FID"
                        aplicarCle15(p_strRespuesta, dsCLE15)
                        dsCLE15.CLE15(0).I_MAINT = "CHG"
                        ' Return aplicarCle15(p_strRespuesta, dsCLE15) ''se realiza llamado en linea 3591
                    End If
                    'INI DAC 20120207 Verificacion de NCONAFI=0 8127, datos fidelizados y no fidelizados
                    If Not pValidacionesAfiliar(dsCLE15, p_strRespuesta, strClaseAfil) Then
                        Return False
                    End If
                    Return aplicarCle15(p_strRespuesta, dsCLE15)
                    'FIN DAC 20120207 Verificacion de NCONAFI=0 8127

                ElseIf dsCLE15.CLE15(0).I_MAINT = "CHG" Then
                    'INI DAC 20120207 Verificacion de NCONAFI=0 8127, datos fidelizados y no fidelizados
                    If Not pValidacionesAfiliar(dsCLE15, p_strRespuesta, strClaseAfil) Then
                        Return False
                    End If
                    Return aplicarCle15(p_strRespuesta, dsCLE15)
                Else ''No hay cambios
                    p_strRespuesta = dsCLE15.CLE15(0).I_NAUTCLI ''drFila.I_NAUTCLI
                    Return True
                End If
                'End If
            End If
        Catch ex As Exception
            'CREADO LCARDENAS 19-04-2012: SEGUIMIENTO INCIDENCIA 9430 INTERMITENCIA WSCOMCLIENTES
            If ConfigurationSettings.AppSettings("LogAuditoriaAfiliar") = "S" Then
                Dim objCache As New Compensar.Vincula.POS.ClsCache
                Dim sInnerExcepcion As String = ""
                If Not ex.InnerException Is Nothing Then
                    sInnerExcepcion = ex.InnerException.ToString()
                End If
                objCache.guardarErrorBitacora(ConfigurationSettings.AppSettings("ProjectID"), _
1, "Auditoria Afiliar: ManejadorTransacciones.trCle15: " & ex.Message & sInnerExcepcion & "StackTrace: " & ex.StackTrace, "", "", "", My.Computer.Name, 1, 1)
            End If
            ManejarErrorInterno("CManejadorTransacciones::TransaccionCLE15:", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try
    End Function
    '' se crea funcion para realizar validaciones sobre las vinculacion de ONBASE, retorna falso, si no paso la validacion
    '' retorna true si esta ok
    Public Function pValidacionesAfiliar(ByVal dsCLE15 As DtsCLE15, ByRef p_strRespuesta As String, Optional ByVal strClaseAfil As String = "0") As Boolean
        pValidacionesAfiliar = True
        Dim pEps As String = ""
        Dim NAUTCLI As String = "0"

        Try
            If dsCLE15.CLE15(0).I_CPRGSRV.Length > 0 Then
                pEps = dsCLE15.CLE15(0).I_CPRGSRV.ToString().Substring(0, 2)
                'Else
                '   pEps = "02"
            End If
            If dsCLE15.CLE15(0).I_NAUTCLI.Length > 0 Then
                NAUTCLI = dsCLE15.CLE15(0).I_NAUTCLI.ToString()
            End If


            strAplicacion = ConfigurationSettings.AppSettings("ProjectID")
            Dim dsAfiliacionCotizante As New DataSet
            dsAfiliacionCotizante = Compensar.SISPOS.ESL.Vinculacion.ConsultaAF.ConsultaAfiliacionAProgramaPOS(strAplicacion, NAUTCLI)
            Dim oBL As New Compensar.Vincula.POS.ReglasVinculacion(strAplicacion, NAUTCLI, dsAfiliacionCotizante) 'Me.IdVinculacion

            'Dim oBL As New Compensar.Vincula.POS.ReglasVinculacion(strAplicacion, NAUTCLI, Nothing)
            'independientes 20120211, se crea la validacion especifica para el programa
            If dsCLE15.CLE15(0).I_NCONAFI.ToString() = "0" Then
                If oBL.esCotizanteIndependiente(strAplicacion, dsCLE15.CLE15(0).I_NAUTCLI.ToString()) Then
                    'MODIFICADO LCARDENAS 27-03-2012: SOLUCION INCIDENCIA REABIERTA 8127
                    If Not oBL.esBeneficiarioActivo(pEps + "13") Then
                        'MODIFICADO LCARDENAS 27-03-2012: SOLUCION INCIDENCIA REABIERTA 8127
                        'EN ESTE PUNTO NO TIENE EL PROGRAMA SINO LA CLASE DE AFILIACION ENVIADA EN TRAMA ONBASE
                        Dim strInde As String = String.Empty
                        If dsCLE15.CLE15(0).I_CPRGSRV.Length = 0 Then
                            Dim valores As String()
                            strInde = "3,8,16,20,21" 'CLASE AFILIACION: 3=1203,8=1213,16=1209,20=1215,21=1216
                            valores = strInde.Split(",")
                            For index As Integer = 0 To valores.Length - 1
                                If valores(index) = strClaseAfil Then
                                    p_strRespuesta = dsCLE15.CLE15(0).I_NAUTCLI ''drFila.I_NAUTCLI
                                    Me.mstrError = "ERROR POS-066 Cotizante ya tiene vinculacion como independiente"
                                    Return False
                                End If
                            Next
                        ElseIf dsCLE15.CLE15(0).I_CPRGSRV.Length > 0 Then
                            strInde = "1203,1209,1213,1215,1216"
                            If strInde.Contains(dsCLE15.CLE15(0).I_CPRGSRV.Substring(2)) Then
                                p_strRespuesta = dsCLE15.CLE15(0).I_NAUTCLI ''drFila.I_NAUTCLI
                                Me.mstrError = "ERROR POS-066 Cotizante ya tiene vinculacion como independiente"
                                Return False
                            End If
                        End If
                    End If
                    'ElseIf oBL.esBeneficiarioActivo(pEps + "13") Then '0008373: VINCULACION COMO BENEFICIARIO CON MAS DE UN COTIZANTE
                    '    If oBL.beneficiarioTieneVinculacion Then
                    '        p_strRespuesta = dsCLE15.CLE15(0).I_NAUTCLI ''drFila.I_NAUTCLI
                    '        'Me.ManejarErrorSalida(12)
                    '        Me.mstrError = "ERROR POS-012 El vinculado tiene vinculacion activa"
                    '        Return False
                    '    End If
                End If
                'beneficiarios 0008373: VINCULACION COMO BENEFICIARIO CON MAS DE UN COTIZANTE
            ElseIf dsCLE15.CLE15(0).I_NCONAFI.ToString() = "3" Then
                'Dim dsAfiliacionCotizante As New DataSet
                dsAfiliacionCotizante = Compensar.SISPOS.ESL.Vinculacion.ConsultaAF.ConsultaAfiliacionAProgramaPOS(strAplicacion, NAUTCLI)
                oBL = New Compensar.Vincula.POS.ReglasVinculacion(strAplicacion, NAUTCLI, dsAfiliacionCotizante) 'Me.IdVinculacion

                If oBL.esBeneficiarioActivo(pEps + "13") Then
                    If oBL.beneficiarioTieneVinculacion Then
                        Dim strBf As String = pEps + "13"
                        If strBf.Contains(dsCLE15.CLE15(0).I_CPRGSRV.Substring(0, 4)) Then
                            p_strRespuesta = dsCLE15.CLE15(0).I_NAUTCLI ''drFila.I_NAUTCLI
                            'Me.ManejarErrorSalida(12)
                            Me.mstrError = "ERROR POS-012 El vinculado tiene vinculacion activa"
                            Return False
                        End If
                        'Me.guardarInconsistencia(12)
                    End If

                End If
            End If
        Catch ex As Exception
            'CREADO LCARDENAS 19-04-2012: SEGUIMIENTO INCIDENCIA 9430 INTERMITENCIA WSCOMCLIENTES
            If ConfigurationSettings.AppSettings("LogAuditoriaAfiliar") = "S" Then
                Dim objCache As New Compensar.Vincula.POS.ClsCache
                Dim sInnerExcepcion As String = ""
                If Not ex.InnerException Is Nothing Then
                    sInnerExcepcion = ex.InnerException.ToString()
                End If
                objCache.guardarErrorBitacora(ConfigurationSettings.AppSettings("ProjectID"), _
1, "Auditoria Afiliar: ManejadorTransacciones.pValidacionesAfiliar: " & ex.Message & sInnerExcepcion & "StackTrace: " & ex.StackTrace, "", "", "", My.Computer.Name, 1, 1)
            End If
        End Try
    End Function
    ''' <summary>
    ''' Aplica en XML teniendo en cuanta validación de RAZDES
    ''' </summary>
    ''' <param name="p_strRespuesta"></param>
    ''' <param name="dsCLE15"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function aplicarCle15(ByRef p_strRespuesta As String, ByVal dsCLE15 As DtsCLE15) As Boolean
        Dim strRespuesta As String
        p_strRespuesta = p_strRespuesta.Replace(" ", "")
        If p_strRespuesta = "" Or InStr(p_strRespuesta, " ") > 0 Then
            dsCLE15.CLE15(0).I_XDESRAZ = ""
        Else
            strRespuesta = dsCLE15.GetXml()
            If InStr(strRespuesta.ToUpper(), ">" + p_strRespuesta.ToUpper() + "<") Or InStr(strRespuesta.ToUpper(), ">" + p_strRespuesta.ToUpper() + " ") Or InStr(strRespuesta.ToUpper(), " " + p_strRespuesta.ToUpper() + "<") Then
                dsCLE15.CLE15(0).I_XDESRAZ = p_strRespuesta
            Else
                dsCLE15.CLE15(0).I_XDESRAZ = ""
            End If
        End If
        Try '2xxx 20130809 se elimina del dataset el nuevo campo 
            dsCLE15.Tables(0).Columns.Remove("C_ESPECIAL")
        Catch ex As Exception

        End Try
        strRespuesta = dsCLE15.GetXml()
        strRespuesta = strRespuesta.Replace(" xml:space=""preserve""", "") '(" xml:Space=""preserve""", "")
        CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)
        
        strRespuesta = ws.Ispec(Me.strAplicacion, "CLE15", Replace(UCase(strRespuesta), "XMLNS", "xmlns"), "1")
        CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta, EventLogEntryType.Warning)
        Me.ReadXml(dsCLE15, New StringReader(strRespuesta))
        If BuscarManejarErrores(dsCLE15, "LogicaTransaccional::ManejadorTransacciones::CLE15") Then
            Return False
        Else
            p_strRespuesta = dsCLE15.CLE15(0).I_NAUTCLI
            Return True
        End If
    End Function
    ''' <summary>
    ''' Proxima a modifcarse para dejar únicmanete los datos del cliente
    ''' </summary>
    ''' <param name="xmlIn"></param>
    ''' <param name="objDatosAfiliacion"></param>
    ''' <param name="boolPos"></param>
    ''' <param name="boolCaj"></param>
    ''' <param name="repuesta"></param>
    ''' <param name="sAplicacion"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ConstruirDatoAfiliacion(ByVal xmlIn As String, ByRef objDatosAfiliacion As CDatosAfiliacion, ByRef boolPos As Boolean, ByRef boolCaj As Boolean, ByRef repuesta As String, ByVal sAplicacion As String) As Integer
        Try
            Dim dsAfiliacion As New Afiliacion
            Dim drFila As Afiliacion.AfiliacionRow
            drFila = dsAfiliacion.Afiliacion.NewAfiliacionRow()
            Me.ReadXml(dsAfiliacion, New StringReader(xmlIn))
            ''   objDatosAfiliacion(New EntidadesNegocio.CDatosAfiliacion)
            Dim objEmpresa As New EntidadesNegocio.CEmpresaAfiliado
            If drFila.GrupoPrograma.Contains("PE") Then
                boolPos = True
            Else
                boolPos = False

            End If

            'Asigna informacion adicional de caja
            If drFila.GrupoPrograma.Contains("CJ") Then
                boolCaj = True
            Else
                boolCaj = False
            End If
            objEmpresa.IdEmpresa = drFila.Responsable2_IdEmpresa
            objEmpresa.TipoIdentificacion = drFila.TipoIdentificacionResponsable2_TipoIdenEmp
            If (drFila.CargoTrabajador <> "" Or drFila.IsCargoTrabajadorNull) And (drFila.CodigoOcupacion <> "" Or drFila.IsCodigoOcupacionNull) Then
                Dim objTrabajador As New EntidadesNegocio.CTrabajador
                objEmpresa.CargoTrabajador = drFila.CargoTrabajador
                objEmpresa.CentroDeCostos = IIf(drFila.IsResponsable4_CentroDeCostosNull Or drFila.Responsable4_CentroDeCostos = "", "0", drFila.Responsable4_CentroDeCostos)
                If drFila.IsResponsable3_DependenciaNull Or drFila.Responsable3_Dependencia <> "" Then
                    objEmpresa.Dependencia = drFila.Responsable3_Dependencia
                Else
                    objEmpresa.Dependencia = "0"
                End If
                objEmpresa.FechaIngreso = drFila.FechaIngreso
                objTrabajador.TipoIdentificacion = drFila.TipoIdent
                objTrabajador.IdTrabajador = drFila.NroIdentificacion
                objTrabajador.Barrio = drFila.Barrio.ToUpper()
                objTrabajador.CabezaFamilia = drFila.CabezaFamilia
                objTrabajador.Ciudad = drFila.Ciudad
                'objEmpresa.HorasLaborMes = drFila.Regional
                objTrabajador.CodigoOcupacion = drFila.CodigoOcupacion
                objTrabajador.ConyugeCotiza = drFila.ConyugeCotiza
                objTrabajador.Direccion = drFila.Direccion.ToUpper()
                objTrabajador.Telefono = drFila.Telefono
                objTrabajador.EstadoCivil = drFila.EstadoCivil.ToUpper()
                objTrabajador.FechaNacimiento = drFila.FechaNacimiento
                objTrabajador.Edad = drFila.Edad
                objTrabajador.Genero = drFila.Genero.ToUpper()
                objTrabajador.Nombres = drFila.Nombres.ToUpper()
                objTrabajador.PrimerApellido = drFila.PrimerApellido.ToUpper()
                objTrabajador.SegundoApellido = IIf(drFila.IsSegundoApellidoNull, "", drFila.SegundoApellido)
                objTrabajador.SegundoApellido = objTrabajador.SegundoApellido.ToUpper()
                objTrabajador.NumeroIdentificacionConyuge = drFila.NumeroIdentificacionConyuge
                If drFila.IsSalarioCyNull Then
                    objTrabajador.SalarioConyuge = "0"
                ElseIf drFila.SalarioCy = "" Then
                    objTrabajador.SalarioConyuge = "0"
                Else
                    objTrabajador.SalarioConyuge = drFila.SalarioCy
                End If
                objTrabajador.TipoIdentificacionConyuge = drFila.TipoIdentificacionConyuge
                objTrabajador.TipoZona = drFila.TipoDireccion_Zona
                'Asigna informacion adicional de POS

                If boolPos Then

                    If drFila.IsEpsOrgNull Then

                        objDatosAfiliacion.EpsOrigen = "008"

                    Else

                        If drFila.EpsOrg = "" Then

                            objDatosAfiliacion.EpsOrigen = "008"

                        Else

                            objDatosAfiliacion.EpsOrigen = drFila.EpsOrg

                        End If

                    End If

                    If drFila.IsFechaAfiliacionPOSNull Then
                        objTrabajador.FechaAfiliacionPOS = ""
                    Else
                        objTrabajador.FechaAfiliacionPOS = drFila.FechaAfiliacionPOS
                    End If
                    objTrabajador.FechaAfiliacionEPSAnterior = drFila.FechaAfiliacionEPSAnterior
                    objEmpresa.SalarioPOS = drFila.Valor_Salario
                    objTrabajador.CodigoEPSAnterior = drFila.CodigoEPSAnterior
                    objTrabajador.FechaAfiliacionSistema = drFila.FechaAfiliacionSistema
                    objDatosAfiliacion.novedadTDA = drFila.novedadTDA
                    objDatosAfiliacion.mesAport = drFila.mesAporte
                End If
                objDatosAfiliacion.Clase = drFila.ClaseAfiliacion
                'Asigna informacion adicional de caja
                If boolCaj Then
                    objTrabajador.SegundoEmpleo = drFila.SegundoEmpleo
                    objEmpresa.HorasLaborMes = drFila.Cantidad_HorasLaborMes
                    objEmpresa.SalarioCaja = drFila.Valor_Salario
                End If
                objDatosAfiliacion.Persona = objTrabajador
            ElseIf (drFila.IsParentescoBnNull Or drFila.ParentescoBn <> "") And (drFila.Responsable1_cot <> "" Or drFila.IsResponsable1_cotNull) Then
                Dim objBeneficiario As New EntidadesNegocio.CBeneficiario
                objBeneficiario.TipoId = drFila.TipoIdentificacionResponsable1_cot
                objBeneficiario.IdTrabajador = drFila.Responsable1_cot
                objBeneficiario.Parentezco = drFila.ParentescoBn.ToUpper()
                objBeneficiario.NUIPAlfa = drFila.ParteAlfabetica.ToUpper()
                objBeneficiario.TipoIdentificacion = drFila.TipoIdent
                objBeneficiario.NumeroIdentificacion = drFila.NroIdentificacion
                objBeneficiario.TideBen = drFila.TipoIdent
                objBeneficiario.NideBen = drFila.NroIdentificacion
                objBeneficiario.FechaNacimiento = drFila.FechaNacimiento
                objBeneficiario.Edad = drFila.Edad
                objBeneficiario.Genero = drFila.Genero.ToUpper()
                objBeneficiario.Nombres = drFila.Nombres.ToUpper()
                objBeneficiario.PrimerApellido = drFila.PrimerApellido.ToUpper()
                objBeneficiario.SegundoApellido = IIf(drFila.IsSegundoApellidoNull, "", drFila.SegundoApellido)
                If drFila.EstadoCivil = "" Then
                    objBeneficiario.EstadoCivil = "SO"
                Else
                    objBeneficiario.EstadoCivil = drFila.EstadoCivil.ToUpper()
                End If
                'Asigna los datos de POS
                If boolPos Then

                    If drFila.IsEpsOrgNull Then

                        objDatosAfiliacion.EpsOrigen = "008"

                    Else

                        If drFila.EpsOrg = "" Then

                            objDatosAfiliacion.EpsOrigen = "008"

                        Else

                            objDatosAfiliacion.EpsOrigen = drFila.EpsOrg

                        End If

                    End If

                    If drFila.IsFechaAfiliacionPOSNull Then
                        objBeneficiario.FechaAfiliacionPOS = ""
                    Else
                        objBeneficiario.FechaAfiliacionPOS = drFila.FechaAfiliacionPOS
                    End If
                    objBeneficiario.CodigoEPSAnterior = drFila.CodigoEPSAnterior
                    If drFila.FechaAfiliacionEPSAnterior <> "" Then
                        objBeneficiario.FechaAfiliacionEPSAnterior = drFila.FechaAfiliacionEPSAnterior
                    End If

                    'datos de discapacidad
                    objBeneficiario.TipoDiscapacidad = drFila.TipoDiscapacidadBn
                    If objBeneficiario.TipoDiscapacidad <> "N" Then
                        objBeneficiario.Discapacidad = drFila.DiscapacidadBn
                    End If
                    objBeneficiario.DiscapacidadPermanente = drFila.DiscapacidadPermanenteBn
                    objDatosAfiliacion.Clase = drFila.ClaseAfiliacion
                End If

                'Asigna datos de caja si el beneficiario es conyuge
                If drFila.ParentescoBn = "CY" And boolCaj Then
                    objDatosAfiliacion.Empresa.Nombre = drFila.EmpresaCy()
                    objDatosAfiliacion.Empresa.SalarioCaja = drFila.SalarioCy
                    objDatosAfiliacion.Empresa.SalarioPOS = drFila.SalarioCy
                    objBeneficiario.RecibeSubsidio = drFila.RecibeSubsidioCy
                    objBeneficiario.NombreCaja = drFila.NombreCajaCy
                End If
                objDatosAfiliacion.Persona = objBeneficiario
            Else

            End If
            objDatosAfiliacion.Empresa = objEmpresa
            objDatosAfiliacion.IdUsuarioEjecutor = drFila.IdUsuarioEjecutor
            If drFila.IsRazDesNull Then
                repuesta = ""
            Else
                repuesta = drFila.RazDes
            End If
            If sAplicacion = "SWPR57" Or sAplicacion = "TRANSAR" Then
                objDatosAfiliacion.AfiliacionTransar = "1"
            Else
                objDatosAfiliacion.AfiliacionTransar = "0"
            End If


            Return True
        Catch ex As Exception
            'CREADO LCARDENAS 19-04-2012: SEGUIMIENTO INCIDENCIA 9430 INTERMITENCIA WSCOMCLIENTES
            If ConfigurationSettings.AppSettings("LogAuditoriaAfiliar") = "S" Then
                Dim objCache As New Compensar.Vincula.POS.ClsCache
                Dim sInnerExcepcion As String = ""
                If Not ex.InnerException Is Nothing Then
                    sInnerExcepcion = ex.InnerException.ToString()
                End If
                objCache.guardarErrorBitacora(ConfigurationSettings.AppSettings("ProjectID"), _
1, "Auditoria Afiliar: ManejadorTransacciones.ConstruirDatoAfiliacion: " & ex.Message & sInnerExcepcion & "StackTrace: " & ex.StackTrace, "", objDatosAfiliacion.EpsOrigen, objDatosAfiliacion.DocIdentificacion, My.Computer.Name, 1, 1)
            End If
            repuesta = ex.Message
            ManejarErrorInterno("CManejadorTransacciones::ConstruirDatoAfiliacion:", 6, ex.ToString())
            ManejarErrorSalida(ex.Message)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Funcion para Construir la TRAMA XML como la necesita VINCULAR
    ''' </summary>
    ''' <param name="xmlIn"></param>
    ''' <param name="sAplicacion"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ConstruirDatoVinculacion(ByRef xmlIn As String, ByVal sAplicacion As String) As Integer
        Try
            Dim dsAfiliacion As New Data.DataSet
            dsAfiliacion.ReadXml(New StringReader(xmlIn))
            Dim agregados As String = ""
            dsAfiliacion.Tables(0).Rows(0)("TipoIdent") = darNombreTipoId(dsAfiliacion.Tables(0).Rows(0)("TipoIdent"))
            If dsAfiliacion.Tables(0).Columns.Contains("TipoIdentificacionResponsable1_cot") Then ''OK
                If dsAfiliacion.Tables(0).Rows(0)("TipoIdentificacionResponsable1_cot") <> "" Then
                    dsAfiliacion.Tables(0).Rows(0)("TipoIdentificacionResponsable1_cot") = darNombreTipoId(dsAfiliacion.Tables(0).Rows(0)("TipoIdentificacionResponsable1_cot"))
                End If
            End If
            If dsAfiliacion.Tables(0).Columns.Contains("TipoIdentificacionResponsable2_TipoIdenEmp") Then ''OK
                If dsAfiliacion.Tables(0).Rows(0)("TipoIdentificacionResponsable2_TipoIdenEmp") <> "" Then
                    dsAfiliacion.Tables(0).Rows(0)("TipoIdentificacionResponsable2_TipoIdenEmp") = darNombreTipoId(dsAfiliacion.Tables(0).Rows(0)("TipoIdentificacionResponsable2_TipoIdenEmp"))
                End If
            End If
            If dsAfiliacion.Tables(0).Columns.Contains("FechaafiliacionPOS") Then ''ok FECHA DEL SELLO TANTO CCF como POS
                If dsAfiliacion.Tables(0).Rows(0)("FechaafiliacionPOS").ToString() <> "" Then
                    ''dsAfiliacion.Tables(0).Rows(0)("FechaInicioAfiliacion") = CType(Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaafiliacionPOS"), mFormatter).ToString("yyyyMMdd"), String)
                    agregados = agregados & "<FechaInicioAfiliacion>" & CType(Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaafiliacionPOS"), mFormatter).ToString("yyyyMMdd"), String) & "</FechaInicioAfiliacion>"
                End If
            End If
            If dsAfiliacion.Tables(0).Columns.Contains("Fechanacimiento") Then 'ok
                If dsAfiliacion.Tables(0).Rows(0)("Fechanacimiento").ToString() <> "" Then
                    dsAfiliacion.Tables(0).Rows(0)("Fechanacimiento") = CType(Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("Fechanacimiento"), mFormatter).ToString("yyyyMMdd"), String)
                End If
            End If
            If InStr(dsAfiliacion.Tables(0).Rows(0)("GrupoPrograma"), "EP") Or InStr(dsAfiliacion.Tables(0).Rows(0)("GrupoPrograma"), "PE") Then
                If dsAfiliacion.Tables(0).Rows(0)("Ciudad") = "" Then
                    dsAfiliacion.Tables(0).Rows(0)("Ciudad") = "11001"
                End If
                Dim fecha As DateTime
                If dsAfiliacion.Tables(0).Rows(0)("mesAporte") = "" Then
                    fecha = Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaafiliacionPOS"))
                    dsAfiliacion.Tables(0).Rows(0)("mesAporte") = CType(fecha.ToString("yyyyMMdd"), String)
                    dsAfiliacion.Tables(0).Rows(0)("FechaIngreso") = CType(fecha.ToString("yyyyMMdd"), String)

                Else
                    Dim mesAporte As Integer = dsAfiliacion.Tables(0).Rows(0)("mesAporte")
                    If mesAporte = 0 Then 'Para act.
                        fecha = Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaafiliacionPOS"))
                    ElseIf mesAporte = 1 Then 'Para 1 Mes
                        'fecha = Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaafiliacionPOS")).AddDays(30)
                        fecha = Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaafiliacionPOS")).AddMonths(1)
                    ElseIf mesAporte = 2 Then ' Para 2 Meses
                        fecha = Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaafiliacionPOS")).AddMonths(2)
                        fecha = fecha.AddDays(-(fecha.Day - 1))
                    End If
                    dsAfiliacion.Tables(0).Rows(0)("mesAporte") = CType(fecha.ToString("yyyyMMdd"), String)
                    fecha = Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaIngreso"))
                    dsAfiliacion.Tables(0).Rows(0)("FechaIngreso") = CType(fecha.ToString("yyyyMMdd"), String)
                End If

            Else
                'If dsAfiliacion.Tables(0).Columns.Contains("FechaIngreso") Then ''ok
                '    If dsAfiliacion.Tables(0).Rows(0)("FechaIngreso") <> "" Then
                '        dsAfiliacion.Tables(0).Rows(0)("FechaIngreso") = CType(Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaIngreso"), mFormatter).ToString("yyyyMMdd"), String)
                '    End If
                'End If
            End If

            'CType(Convert.ToDateTime(objTrabajador.FechaNacimiento, mFormatter).ToString("yyyyMMdd"), String)
            If InStr(dsAfiliacion.Tables(0).Rows(0)("GrupoPrograma"), "EP") Or InStr(dsAfiliacion.Tables(0).Rows(0)("GrupoPrograma"), "PE") Then ''POS
                dsAfiliacion.Tables(0).Rows(0)("GrupoPrograma") = "EP"
                ''Trabajadores
                If dsAfiliacion.Tables(0).Columns.Contains("CargoTrabajador") Then
                    If dsAfiliacion.Tables(0).Rows(0)("CargoTrabajador") <> "" Then

                        If dsAfiliacion.Tables(0).Columns.Contains("EpsOrg") Then
                            If dsAfiliacion.Tables(0).Rows(0)("EpsOrg") = "009" Then
                                agregados = agregados & "<Programa>09"
                            ElseIf dsAfiliacion.Tables(0).Rows(0)("EpsOrg") = "9" Then
                                agregados = agregados & "<Programa>09"
                            ElseIf dsAfiliacion.Tables(0).Rows(0)("EpsOrg") = "012" Then
                                agregados = agregados & "<Programa>12"
                            ElseIf dsAfiliacion.Tables(0).Rows(0)("EpsOrg") = "12" Then
                                agregados = agregados & "<Programa>12"
                            Else
                                agregados = agregados & "<Programa>02"
                            End If
                        Else
                            agregados = agregados & "<Programa>02"
                        End If
                        'agregados = agregados & "<Programa>2"
                        'agregados = agregados & "<Programa>9"
                        'agregados = agregados & "<Programa>12"
                        If dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 1 Then  '1:Dependiente()
                            agregados = agregados & "1201</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 2 Then '2:Servicio(Domestico)'Empleada Doméstica21202
                            agregados = agregados & "1202</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 3 Then '3:Independiente() '0:Independiente(21203) 
                            agregados = agregados & "1203</Programa>"
                            agregados = agregados & "<Condicion>0</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 4 Then '4Madre(Comunitaria)'Madre Comunitaria 21204 
                            agregados = agregados & "1204</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 5 Then '5:Contratista    
                            agregados = agregados & "</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 6 Then '7: Pensionado() 'Pensionado(21205)             
                            agregados = agregados & "1205</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 7 Then '7:pensionado x sust 'Pensionado por Sustitución    21206            
                            agregados = agregados & "1206</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 8 Then 'Aprendices Regimen Especial21213
                            agregados = agregados & "1213</Programa>" '
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 12 Then  'Aprendiz SENA Etapa Electiva 21207
                            agregados = agregados & "1207</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 19 Then  'Aprendiz SENA Etapa Productiva21212
                            agregados = agregados & "1212</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 18 Then  'Funcionarios Públicos sin tope IBC21210
                            agregados = agregados & "1210</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 16 Then '2:Agremiado(21209)
                            agregados = agregados & "1209</Programa>"
                            agregados = agregados & "<Condicion>1</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 20 Then ':Independiente Tipo 41
                            agregados = agregados & "1215</Programa>"
                            agregados = agregados & "<Condicion>0</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 21 Then '2:Independiente Tipo 42
                            agregados = agregados & "1216</Programa>"
                            agregados = agregados & "<Condicion>0</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 0 Then  '1Convenio COMFENALCO Antioquia 2142
                            agregados = agregados & "</Programa>"
                            agregados = agregados & "<Condicion>1</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 0 Then  'Convenio COMFENALCO Valle     2141
                            agregados = agregados & "</Programa>"
                            agregados = agregados & "<Condicion>1</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 15 Then  'FONEDE(21208)
                            agregados = agregados & "1208</Programa>"
                            agregados = agregados & "<Condicion>1</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 22 OrElse _
                                dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 23 OrElse _
                                dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 24 OrElse _
                                dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 25 Then
                            agregados = agregados & "13" & dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") & "</Programa>"
                            agregados = agregados & "<Condicion>3</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 26 Then
                            agregados = agregados & "1321" & "</Programa>"
                            agregados = agregados & "<Condicion>3</Condicion>"

                            'Inicio Incidente 0010444: Se requiere conocer los codigos de los programas nuevos       
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 44 Then 'Dependiente Emergencia Social
                            agregados = agregados & "1244" & "</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 47 Then  'Dependiente Aporte SGP
                            agregados = agregados & "1247" & "</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 48 Then  'Dependiente Ley de Primer Empleo 1429
                            agregados = agregados & "1248" & "</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 49 Then  'Aprendiz SENA Etapa Lectiva Ley 1429
                            agregados = agregados & "1249" & "</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        ElseIf dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 50 Then  'Aprendiz SENA Etapa Productiva Ley 1429
                            agregados = agregados & "1250" & "</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                            'Fin   Incidente 0010444: Se requiere conocer los codigos de los programas nuevos 
                        Else
                            agregados = agregados & "</Programa>"
                            agregados = agregados & "<Condicion>2</Condicion>"
                        End If

                        ''DATOS ADICIONALES NETAMENTE DE TRABAJADORES
                        If dsAfiliacion.Tables(0).Columns.Contains("CodigoEPSAnterior") Then
                            If dsAfiliacion.Tables(0).Rows(0)("CodigoEPSAnterior") <> "" Then
                                dsAfiliacion.Tables(0).Rows(0)("Accion4") = "A"
                                dsAfiliacion.Tables(0).Rows(0)("Requisito4") = "EPSANT"
                                dsAfiliacion.Tables(0).Rows(0)("FechaInicialVigencia4") = ""
                                dsAfiliacion.Tables(0).Rows(0)("FechaFinalVigencia4") = ""
                                dsAfiliacion.Tables(0).Rows(0)("DatoNumerico4") = CType(dsAfiliacion.Tables(0).Rows(0)("CodigoEPSAnterior"), String)
                                dsAfiliacion.Tables(0).Rows(0)("DatoAlfabetico4") = ""
                            End If
                        End If
                        If Not dsAfiliacion.Tables(0).Columns.Contains("FechaAfiliacionPOS") Then
                            If dsAfiliacion.Tables(0).Rows(0)("FechaAfiliacionPOS") <> "" Then
                                If dsAfiliacion.Tables(0).Rows(0)("FechaAfiliacionPOS") <> 0 Then
                                    dsAfiliacion.Tables(0).Rows(0)("Accion3") = "A"
                                    dsAfiliacion.Tables(0).Rows(0)("Requisito3") = "FINEMP"
                                    dsAfiliacion.Tables(0).Rows(0)("FechaInicialVigencia3") = "" '' 
                                    dsAfiliacion.Tables(0).Rows(0)("FechaFinalVigencia3") = ""
                                    dsAfiliacion.Tables(0).Rows(0)("DatoNumerico3") = CType(Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaIngreso"), mFormatter).ToString("yyyyMMdd"), String) ''""
                                    dsAfiliacion.Tables(0).Rows(0)("DatoAlfabetico3") = ""
                                End If
                            End If
                        End If
                        If Not dsAfiliacion.Tables(0).Columns.Contains("NumeroIdentificacionConyuge") Then
                            If dsAfiliacion.Tables(0).Rows(0)("NumeroIdentificacionConyuge") <> "" Then
                                If dsAfiliacion.Tables(0).Rows(0)("NumeroIdentificacionConyuge") <> 0 Then
                                    dsAfiliacion.Tables(0).Rows(0)("Accion2") = "A"
                                    dsAfiliacion.Tables(0).Rows(0)("Requisito2") = "IDCONY"
                                    dsAfiliacion.Tables(0).Rows(0)("FechaInicialVigencia2") = ""
                                    dsAfiliacion.Tables(0).Rows(0)("FechaFinalVigencia2") = ""
                                    dsAfiliacion.Tables(0).Rows(0)("DatoNumerico2") = dsAfiliacion.Tables(0).Rows(0)("NumeroIdentificacionConyuge") '""
                                    dsAfiliacion.Tables(0).Rows(0)("DatoAlfabetico2") = darNombreTipoId(dsAfiliacion.Tables(0).Rows(0)("TipoIdentificacionConyuge")) '""

                                End If
                            End If
                        End If
                    ElseIf dsAfiliacion.Tables(0).Columns.Contains("ParentescoBn") And dsAfiliacion.Tables(0).Columns.Contains("Responsable1_cot") Then
                        If dsAfiliacion.Tables(0).Rows(0)("ParentescoBn") <> "" And dsAfiliacion.Tables(0).Rows(0)("Responsable1_cot") <> "" Then
                            'VUNCULACION DE UPC
                            '	Padre o madre del cónyuge	21315
                            '	Padre o madre del cónyugue	21324
                            '	Segundo grado de consanguinidad	21322
                            '	Tercer grado de consanguinidad	21323
                            If dsAfiliacion.Tables(0).Columns.Contains("EpsOrg") Then
                                If dsAfiliacion.Tables(0).Rows(0)("EpsOrg") = "009" Then
                                    agregados = agregados & "<Programa>09"
                                ElseIf dsAfiliacion.Tables(0).Rows(0)("EpsOrg") = "9" Then
                                    agregados = agregados & "<Programa>09"
                                ElseIf dsAfiliacion.Tables(0).Rows(0)("EpsOrg") = "012" Then
                                    agregados = agregados & "<Programa>12"
                                ElseIf dsAfiliacion.Tables(0).Rows(0)("EpsOrg") = "12" Then
                                    agregados = agregados & "<Programa>12"
                                Else
                                    agregados = agregados & "<Programa>02"
                                End If
                            Else
                                agregados = agregados & "<Programa>02"
                            End If
                            'extiscurrea 18082010 Se agrega codigo EPS a los beneficiarios
                            If dsAfiliacion.Tables(0).Rows(0)("ParentescoBn") = "CY" Then 'CY	Cónyuge / Compañera permanente	21311
                                agregados = agregados & "1311</Programa>"
                                agregados = agregados & "<Condicion>3</Condicion>"
                                dsAfiliacion.Tables(0).Rows(0)("EstadoCivil") = "CA"
                            ElseIf dsAfiliacion.Tables(0).Rows(0)("ParentescoBn") = "PA" Then 'PA                Padres(21314)
                                If dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 24 Then '8992
                                    agregados = agregados & "1324</Programa>"
                                Else
                                    agregados = agregados & "1314</Programa>"
                                End If

                                agregados = agregados & "<Condicion>3</Condicion>"
                                dsAfiliacion.Tables(0).Rows(0)("EstadoCivil") = "CA"

                            ElseIf dsAfiliacion.Tables(0).Rows(0)("ParentescoBn") = "HI" Then 'HI                Hijos(21313)
                                If dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 25 Then '8992
                                    agregados = agregados & "1325</Programa>"
                                Else
                                    agregados = agregados & "1313</Programa>"
                                End If
                                agregados = agregados & "<Condicion>3</Condicion>"
                                dsAfiliacion.Tables(0).Rows(0)("EstadoCivil") = "SO"
                                'DATOS ADICIONALES NETAMENTE BENEFICIARIOS
                                If Not dsAfiliacion.Tables(0).Columns.Contains("DiscapacidadBn") Then
                                    If dsAfiliacion.Tables(0).Rows(0)("DiscapacidadBn") <> "" Then
                                        If dsAfiliacion.Tables(0).Rows(0)("DiscapacidadBn").ToString().ToUpper = "True" Then
                                            'DISCAP()
                                            dsAfiliacion.Tables(0).Rows(0)("Accion2") = "A"
                                            dsAfiliacion.Tables(0).Rows(0)("Requisito2") = "DISCAP"
                                            dsAfiliacion.Tables(0).Rows(0)("FechaInicialVigencia2") = ""
                                            dsAfiliacion.Tables(0).Rows(0)("FechaFinalVigencia2") = ""
                                            dsAfiliacion.Tables(0).Rows(0)("DatoNumerico2") = dsAfiliacion.Tables(0).Rows(0)("DiscapacidadBn")
                                            dsAfiliacion.Tables(0).Rows(0)("DatoAlfabetico2") = dsAfiliacion.Tables(0).Rows(0)("DiscapacidadPermanenteBn")
                                        End If
                                    End If
                                End If
                                If dsAfiliacion.Tables(0).Columns.Contains("CodigoEPSAnterior") Then
                                    If dsAfiliacion.Tables(0).Rows(0)("CodigoEPSAnterior") <> "" Then
                                        dsAfiliacion.Tables(0).Rows(0)("Accion3") = "A"
                                        dsAfiliacion.Tables(0).Rows(0)("Requisito3") = "EPSANT"
                                        dsAfiliacion.Tables(0).Rows(0)("FechaInicialVigencia3") = ""
                                        dsAfiliacion.Tables(0).Rows(0)("FechaFinalVigencia3") = ""
                                        dsAfiliacion.Tables(0).Rows(0)("DatoNumerico3") = CType(dsAfiliacion.Tables(0).Rows(0)("CodigoEPSAnterior"), String)
                                        dsAfiliacion.Tables(0).Rows(0)("DatoAlfabetico3") = ""
                                    End If
                                End If

                            ElseIf dsAfiliacion.Tables(0).Rows(0)("ParentescoBn") = "OT" Then '"OT" 
                                If dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 22 OrElse _
                                    dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 23 OrElse _
                                    dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 24 OrElse _
                                    dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") = 25 Then
                                    agregados = agregados & "13" & dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") & "</Programa>"  '8992
                                Else
                                    agregados = agregados & "1321" & "</Programa>"                                                      '8992
                                End If
                                agregados = agregados & "<Condicion>3</Condicion>"
                            ElseIf dsAfiliacion.Tables(0).Rows(0)("ParentescoBn") = "UO" Then '"OT" 
                                agregados = agregados & "1321" & "</Programa>"  '8992
                                agregados = agregados & "<Condicion>3</Condicion>"
                            ElseIf dsAfiliacion.Tables(0).Rows(0)("ParentescoBn") = "US" Then '"OT" 
                                agregados = agregados & "1322" & "</Programa>"  '8992
                                agregados = agregados & "<Condicion>3</Condicion>"
                            ElseIf dsAfiliacion.Tables(0).Rows(0)("ParentescoBn") = "UT" Then '"OT" 
                                agregados = agregados & "1323" & "</Programa>"  '8992
                                agregados = agregados & "<Condicion>3</Condicion>"
                            ElseIf dsAfiliacion.Tables(0).Rows(0)("ParentescoBn") = "UP" Then '"OT" 
                                agregados = agregados & "1324" & "</Programa>"  '8992
                                agregados = agregados & "<Condicion>3</Condicion>"
                            ElseIf dsAfiliacion.Tables(0).Rows(0)("ParentescoBn") = "UH" Then '"OT" 
                                agregados = agregados & "1325" & "</Programa>"  '8992
                                agregados = agregados & "<Condicion>3</Condicion>"
                                '22 Segundo grado de consanguinidad US
                                ' 23 Tercer grado de consaguinidad UT
                                '24:                             Padres(UP)
                                '25:                             Hijos(UH)
                                ' 26 Otros no parientes UO 
                            Else
                                '10502
                                With dsAfiliacion.Tables(0)
                                    If .Rows(0)("ClaseAfiliacion") = 22 OrElse _
                                                                       .Rows(0)("ClaseAfiliacion") = 23 OrElse _
                                                                        .Rows(0)("ClaseAfiliacion") = 24 OrElse _
                                                                        .Rows(0)("ClaseAfiliacion") = 25 Then
                                        agregados = agregados & "13" & dsAfiliacion.Tables(0).Rows(0)("ClaseAfiliacion") & "</Programa>"  '8992
                                        agregados = agregados & "<Condicion>3</Condicion>"
                                        'Segundo y Tercer Grado de consanguinidad
                                        If .Rows(0)("ClaseAfiliacion") = 22 OrElse _
                                            .Rows(0)("ClaseAfiliacion") = 23 Then
                                            'Valor(Descripcion)
                                            '1:                                          HERMANO()
                                            '2:                                          NIETO()
                                            '3:                                          ABUELO()

                                            'Tercer Grado de Consanguinidad (ParentescoBn = "UT" -> Programa: "1323")
                                            '                                            Valor(Descripcion)
                                            '1:                                          SOBRINO()
                                            '2:                                          TIO()

                                            Dim strValorParentesco As String
                                            strValorParentesco = 1
                                            If .Rows(0)("ParentescoBn") = "NI" OrElse .Rows(0)("ParentescoBn") = "TI" Then
                                                strValorParentesco = 2
                                            ElseIf .Rows(0)("ParentescoBn") = "AB" Then
                                                strValorParentesco = 3
                                            End If
                                            dsAfiliacion.Tables(0).Rows(0)("Valor_Salario") = strValorParentesco
                                            'agregados = agregados & "<Valor_Salario>" & strValorParentesco & "</Valor_Salario>"
                                        End If
                                        '10502
                                    End If
                                End With
                                'Tag Clase Afiliación	Tag ParentescoBn	Descripción del Parentesco	Descripción del Programa
                                '22	                    HE	                Hermano	                    Segundo grado de consanguinidad US
                                '22	                    NI	                Nieto	                    Segundo grado de consanguinidad US
                                '22	                    AB	                Abuelo	                    Segundo grado de consanguinidad US
                                '23	                    SO	                Sobrino	                    Tercer grado de consaguinidad UT
                                '23	                    TI	                Tio	                        Tercer grado de consaguinidad UT
                                '24	                    PA	                Padre	                    Padres(UP)
                                '25	                    HI	                Hijo	                    Hijos(UH)
                                '26	                    OT	                Otros	                    Otros no parientes UO


                            End If
                        End If
                    End If
                    'DATOS ADICIONALES DE TR Y BN
                    If dsAfiliacion.Tables(0).Columns.Contains("FechaAfiliacionSistema") Then
                        If dsAfiliacion.Tables(0).Rows(0)("FechaAfiliacionSistema") <> "" Then
                            'If dsAfiliacion.Tables(0).Rows(0)("FechaAfiliacionSistema") <> 0 Then
                            'AFISSS()   ''    dsAfiliacion.Tables(0).Rows(0)("Requisito1 = "EPSANT"
                            dsAfiliacion.Tables(0).Rows(0)("Accion1") = "A"
                            dsAfiliacion.Tables(0).Rows(0)("Requisito1") = "AFISSS"
                            dsAfiliacion.Tables(0).Rows(0)("FechaInicialVigencia1") = ""
                            dsAfiliacion.Tables(0).Rows(0)("FechaFinalVigencia1") = ""
                            dsAfiliacion.Tables(0).Rows(0)("DatoNumerico1") = CType(Convert.ToDateTime(dsAfiliacion.Tables(0).Rows(0)("FechaAfiliacionSistema"), mFormatter).ToString("yyyyMMdd"), String)
                            dsAfiliacion.Tables(0).Rows(0)("DatoAlfabetico1") = ""
                            ' objBeneficiario.FechaAfiliacionPOS = dsAfiliacion.Tables(0).Rows(0)("FechaAfiliacionPOS
                            'End If
                        End If
                    End If
                End If
            End If
            'Asigna informacion adicional de caja
            If dsAfiliacion.Tables(0).Rows(0)("GrupoPrograma").Contains("CJ") Then
                'If sAplicacion = "SWPR57" Or sAplicacion = "TRANSAR" Then
                '    dsAfiliacion.Tables(0).Rows(0)("Accion1") = "A"
                '    dsAfiliacion.Tables(0).Rows(0)("Requisito1") = "INDSUB"
                '    dsAfiliacion.Tables(0).Rows(0)("FechaInicialVigencia1") = ""
                '    dsAfiliacion.Tables(0).Rows(0)("FechaFinalVigencia1") = ""
                '    dsAfiliacion.Tables(0).Rows(0)("DatoNumerico1") = "6"
                '    dsAfiliacion.Tables(0).Rows(0)("DatoAlfabetico1") = ""
                'End If
            End If
            agregados = agregados & "<Operacion>ADD</Operacion>"
            agregados = "</FechaafiliacionPOS>" & agregados
            xmlIn = dsAfiliacion.GetXml()
            xmlIn = xmlIn.Replace("</FechaafiliacionPOS>", agregados)
            Return True
        Catch ex As Exception
            'CREADO LCARDENAS 19-04-2012: SEGUIMIENTO INCIDENCIA 9430 INTERMITENCIA WSCOMCLIENTES
            If ConfigurationSettings.AppSettings("LogAuditoriaAfiliar") = "S" Then
                Dim objCache As New Compensar.Vincula.POS.ClsCache
                Dim sInnerExcepcion As String = ""
                If Not ex.InnerException Is Nothing Then
                    sInnerExcepcion = ex.InnerException.ToString()
                End If
                objCache.guardarErrorBitacora(ConfigurationSettings.AppSettings("ProjectID"), _
1, "Auditoria Afiliar: ManejadorTransacciones.ConstruirDatoVinculacion: " & ex.Message & sInnerExcepcion & "StackTrace: " & ex.StackTrace, "", "", "", My.Computer.Name, 1, 1)
            End If
            ManejarErrorInterno("CManejadorTransacciones::ConstruirDatoVinculacion:", 6, ex.ToString())
            ManejarErrorSalida(ex.Message)
            'xmlIn = ex.Message
            Return False
        End Try
    End Function

    
    Public Function CambioEstado(ByVal xmlIn As String, ByRef strRespuesta As String) As Integer
        Try
            Dim dscle15 As New DtsCLE15
            Dim drFila As DtsCLE15.CLE15Row
            ''  Dim strRespuesta As String
            drFila = dscle15.CLE15.NewCLE15Row()
         
            strRespuesta = dscle15.GetXml()
            Dim strNowBefore As String = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") & ";"
            'CManejadorMensajes.PublicarMensaje("EntradaHost: " & xmlIn & Now.ToString, EventLogEntryType.Warning)
            strRespuesta = ws.Ispec(Me.strAplicacion, "CLE15", Replace(UCase(xmlIn), "XMLNS", "xmlns"), "1")
            ''strRespuesta.
            If ConfigurationManager.AppSettings("Activalog") = "S" Then
                CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta & ";" & _
                                                              strNowBefore & DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), EventLogEntryType.Information)
            End If


            Me.ReadXml(dscle15, New StringReader(strRespuesta))

            If BuscarManejarErrores(dscle15, "LogicaTransaccional::ManejadorTransacciones::Requisitos") Then
                'Hubo Errores 

                Return False
            Else
                strRespuesta = dscle15.CLE15(0).MESSAGE
                Return True
            End If
        Catch ex As Exception
            CManejadorMensajes.PublicarMensaje("RespuestaHost: " & strRespuesta & ";" & _
                                     DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), EventLogEntryType.Error)

            strRespuesta = ex.Message
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Funcion para devolver el resultado de uan apoeracion de afiliacion(Registro-Vinculacion)
    ''' </summary>
    ''' <param name="strRespuesta"></param>
    ''' <param name="comando"></param>
    ''' <param name="fecha"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ManejoErrorVinc(ByRef strRespuesta As String, ByRef comando As String, ByRef fecha As String, ByVal esPos As Boolean) As Boolean
        If strRespuesta.Length > 500 Then
            Dim dsCLE15 As New DtsCLE15
            Me.ReadXml(dsCLE15, New StringReader(strRespuesta))
            If BuscarManejarErrores(dsCLE15, "LogicaTransaccional::ManejadorTransacciones::CLE15") Then
                Return False
            Else
                CManejadorMensajes.PublicarMensaje("Respuesta: " & strRespuesta, EventLogEntryType.Warning)
                If InStr(strRespuesta, "SUCCESSFUL ENTRY") > 0 Or InStr(strRespuesta, "INPUT REQUEST") > 0 Then
                    fecha = DateTime.Now.ToString(Me.mFormatter)
                    comando = "CLA"
                    If esPos Then
                        CManejadorMensajes.DarMensaje("180", strRespuesta)
                    Else
                        CManejadorMensajes.DarMensaje("181", strRespuesta)
                    End If
                Else
                    strRespuesta = "ERROR"

                    Try
                        '0000246
                        strRespuesta = strRespuesta & " " & dsCLE15.CLE15(0).MESSAGE
                    Catch ex As Exception

                    End Try
                End If
                ''Or InStr(1, xmlnodo.InnerXml.ToString, "305 ") > 0 Then CLIENTE AGRAGADO
                Return True
            End If
        Else
            Dim dsError As New Data.DataSet
            dsError.ReadXml(New StringReader(strRespuesta))
            If dsError.Tables.Count > 0 Then
                strRespuesta = ""
                If dsError.Tables(1).Rows(0)(0) <> "0" Then
                    strRespuesta = dsError.Tables(1).Rows(0)(0) 'dac 20121018 00477, sin ERROR
                Else
                    For Each dr As DataRow In dsError.Tables(2).Rows
                        strRespuesta = strRespuesta & dr("Mensaje_Text").ToString() & " " 'dac 20121018 00477, sin ERROR
                    Next
                End If
            Else
                strRespuesta = dsError.Tables(1).Rows(0)(0)   'dac 20121018 00477, sin ERROR
            End If
            ManejarErrorSalida(strRespuesta)
        End If

    End Function
#End Region

End Class
' END CLASS DEFINITION ManejadorTransacciones



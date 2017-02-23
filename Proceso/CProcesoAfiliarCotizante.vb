#Region "Namespaces"
Imports LogicaTransaccional
Imports EntidadesNegocio
Imports Servicios
Imports ManejoMensajes


Imports System.Reflection
Imports System.Configuration
Imports System.Xml
Imports System.Xml.Serialization
Imports System.IO
Imports System.Text
#End Region

''' -----------------------------------------------------------------------------
''' Project	 : Proceso
''' Class	 : CProcesoAfiliarCotizante
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Este proceso modela los diferentes pasos y operaciones necesarias para realizar 
''' el procesos de afiliación de un Trabajador, tanto a Caja de Compensación, como a 
''' EPS
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	15/09/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Class CProcesoAfiliarCotizante
    Inherits CProceso

#Region "Constructores"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor de la clase. Inicializa la clase llamando al constructor de la 
    ''' clase base CProceso. 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	17/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub New()
        MyBase.New()
    End Sub
#End Region

    ''#Region "Funciones Basicas Que Heredan de CProceso "

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Mediante este método se implementa la afiliación de un trabajador, tanto a Caja 
    ''' de Compensación Familiar, como a EPS
    ''' </summary>
    ''' <param name="pobjDatosAfiliacion">Datos de la afiliación que se va a realizar</param>
    ''' <param name="pCaja">Este parametro booleano indica si se trata de una afiliación a  
    '''  Caja de Compensación Familiar</param>
    ''' <param name="pPOS">Este parametro booleano indica si se trata de una afiliación a  
    ''' EPS</param>
    ''' <param name="pstrRespuesta">Cadena con el XML con el resultado del proceso.
    ''' Si hubo errores, o si el proceso fue exitoso, en este parámetro se envía el 
    ''' mensaje correspondiente. </param>
    ''' <returns>True si la operación es exitosa
    ''' False de lo contrario</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overrides Function Afiliar(ByRef pobjDatosAfiliacion As CDatosAfiliacion, ByVal pCaja As Boolean, ByVal pPOS As Boolean, ByRef pstrRespuesta As String) As Boolean
        Try
            If Not pobjDatosAfiliacion.EsVacio And Not pobjDatosAfiliacion Is Nothing Then
                If pobjDatosAfiliacion.Persona.GetType.FullName = "EntidadesNegocio.CTrabajador" Then
                    Dim blnflag As Boolean = True
                    Dim objTrabajador As New EntidadesNegocio.CTrabajador
                    Dim objTrabajadorConsulta As New EntidadesNegocio.CTrabajador
                    Dim strTmpRespuesta As String

                    'Guarda los datos del trabajador a adicionar en una bussines entity auxiliar para que no se pierdan los datos que se van a adicionar
                    objTrabajador = pobjDatosAfiliacion.Persona

                    'Asigna al un bussiness entity de tipo CTrabajador las variables de consulta de acuerdo a los  parametros ingresados
                    objTrabajadorConsulta.TipoIdentificacion = objTrabajador.TipoIdentificacion
                    objTrabajadorConsulta.IdTrabajador = objTrabajador.IdTrabajador

                    'averigua si la afiliación a realizar es de POS o Caja o las 2
                    If pPOS Then
                        'Consulta el trabajador
                        If Me.Consultar(objTrabajadorConsulta) Then
                            'De acuerdo a lo consultado en el WS, se asignan las propiedades de estadoAfiliacion del trabajador consultado al que se va a afiliar
                            objTrabajador.EmpresaPrimaria = objTrabajadorConsulta.EmpresaPrimaria
                            objTrabajador.EmpresasAsociadas = objTrabajadorConsulta.EmpresasAsociadas

                            'Se asigna el trabajador original a la entidad de afiliación
                            pobjDatosAfiliacion.Persona = objTrabajador

                            If objlogicatransaccional.TransaccionAfiliarTrabajadorPOS(pobjDatosAfiliacion, pstrRespuesta) Then
                                strTmpRespuesta = pstrRespuesta
                                'Guardar datos afiliacion en transar  
                                If GuardarDatosTransar(pobjDatosAfiliacion) Then
                                    blnflag = True
                                Else 'Error guardando datos en transar 
                                    ManejarErrorSalida(24)
                                    blnflag = False
                                End If
                            Else
                                MyBase.ManejarErrorInterno("CProcesoAfiliarCotizante::Afiliar", 14, objlogicatransaccional.DescripcionError)
                                MyBase.ManejarErrorSalida(objlogicatransaccional.DescripcionError)
                                blnflag = False
                            End If
                        Else
                            blnflag = False
                        End If
                    End If
                 

                    Return blnflag

                Else 'esta afiliando una entidad que no es de que no es de tipo CTrabajador
                    ManejarErrorInterno("CProcesoAfiliarCotizante::Afiliar", 1, "Esperado: EntidadesNegocio.CTrabajador" & ", Recibido: " & pobjDatosAfiliacion.Persona.GetType.FullName)
                    ManejarErrorSalida(13)
                    Return False
                End If 'CIERRA: pobjDatosAfiliacion.Persona.GetType.FullName = "EntidadesNegocio.CTrabajador"

            Else ' La afiliacion esta vacia
                ManejarErrorInterno("CProcesoAfiliarCotizante::Afiliar", 4, "error de tipos: al afiliar esperaba un tipo EntidadesNegocio.CTrabajador")
                ManejarErrorSalida(4)
                Return False
            End If 'If Not p_objAfiliacion Is Nothing Or p_objAfiliacion.EsVacio Then


        Catch ex As Exception
            ManejarErrorInterno("CProcesoAfiliarCotizante::Afiliar", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''  Este método se encarga de almacenar en la base de datos de Transar los datos 
    '''  correspondientes a una afiliación realizada.
    ''' </summary>
    ''' <param name="p_objAfiliacion">Objeto con los datos de la afiliación que se va
    '''  a almacenar en la base de datos</param>
    ''' <returns>True si la operación fue exitosa
    ''' False de lo contrario</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	06/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overrides Function GuardarDatosTransar(ByRef p_objAfiliacion As CDatosAfiliacion) As Boolean
        Dim objDB As New CAfiliacion
        'Dim strError As String
        Try
            If p_objAfiliacion.Comando <> "" And Not p_objAfiliacion.Comando Is Nothing Then 'Si el comando es vacio, el individuo ya estaba afiliado y no se debe guardar ningun registro
                Return True
            Else : Return True
            End If
        Catch ex As Exception
            MyBase.ManejarErrorInterno("CProcesoAfiliarCotizante::GuardarDatosTransar", 6, ex.ToString)
            MyBase.ManejarErrorSalida(3)
            Return False
        End Try

    End Function


#Region " Funciones Propias de la Clase"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Recibe un objeto de tipo CEmpresa, con las propiedades Numero de identificacion,
    ''' tipo de identificacion y codigo de dependencia debidamente especificadas. 
    ''' Consulta, mediante el usodel paquete Servicios las dependencias asociadas a 
    ''' la empresa. 
    ''' </summary>
    ''' <param name="objEmpresa">Objeto de tipo CTrabajador sobre el cual se va a hacer la 
    ''' consulta según las propiedades Número y tipo de indentificación.
    ''' </param>
    ''' <param name="pstrXMLDatosRespuesta">cadena XML con las dependencias encontradas
    ''' </param>
    ''' <returns>true si no hubo ningun error, false de lo contrario</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/3/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function ConsultarDependenciasEmpresa(ByVal objEmpresa As CEmpresa, ByRef pstrXMLDatosRespuesta As String) As Boolean
        Dim strError As String = ""

        Try

            'Dim WS As New Servicios.AF.AFI
            Dim strAplicacion As String
            Dim strbuilder As StringBuilder
            Dim intCodDep As Integer
            Dim intOpcConsulta As Integer
            Dim strResp As String

            If objEmpresa.IdEmpresa Is Nothing Or objEmpresa.TipoIdentificacion Is Nothing Then

                CManejadorMensajes.DarMensaje("2", strError)
                strbuilder = New StringBuilder("Proceso::CProcesoAfiliarCotizante::ConsultarDependenciasEmpresa:")
                strbuilder.Append(strError)
                CManejadorMensajes.PublicarMensaje(strbuilder.ToString(), EventLogEntryType.Error)
                MyBase.mstrError = strError
                Return False
            Else
                If ConfigurationSettings.AppSettings.HasKeys() Then
                    strAplicacion = ConfigurationSettings.AppSettings("IdAplicacion")
                Else
                    strAplicacion = "SWPR57"
                End If

                'revisa si la empresa tiene o no dependencia
                If IsNumeric(objEmpresa.Dependencia) And objEmpresa.Dependencia <> "0" Then
                    intCodDep = CInt(objEmpresa.Dependencia)
                    intOpcConsulta = 0
                Else
                    intCodDep = 0
                    intOpcConsulta = 1
                End If

                'Invocacion del webservice
                ManejoMensajes.CManejadorMensajes.PublicarMensaje("intOpcConsulta = " & intOpcConsulta, EventLogEntryType.Information)
                ManejoMensajes.CManejadorMensajes.PublicarMensaje("objEmpresa.Dependencia = " & objEmpresa.Dependencia, EventLogEntryType.Information)
                ManejoMensajes.CManejadorMensajes.PublicarMensaje("objEmpresa.idempresa = " & objEmpresa.IdEmpresa, EventLogEntryType.Information)
                strResp = "" 'WS.Empresa(strAplicacion, Double.Parse(objEmpresa.IdEmpresa), Integer.Parse(objEmpresa.TipoIdentificacion), intCodDep, intOpcConsulta)

                'Verifica si hay error del WS
                If InStr(strResp, "<Error>") Then
                    pstrXMLDatosRespuesta = ""
                    ManejarErrorInterno("CProcesoAfiliarCotizante::ConsultarDependenciasEmpresa", 6, strResp)
                    ManejarErrorSalida(3)
                    Return False
                Else
                    pstrXMLDatosRespuesta = strResp
                    Return True
                End If

            End If ' objTemp_Tra.TipoIdentificacion Is Nothing Or objTemp_Tra.IdTrabajador Is Nothing

        Catch ex As Exception
            ManejarErrorInterno("CProcesoAfiliarCotizante::ConsultarDependenciasEmpresa", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Consulta mediante el uso del paquete Servicios las ocupaciones existentes 
    ''' en Compensar
    ''' </summary>
    ''' <param name="pstrXMLDatosRespuesta">cadena XML con las ocupaciones encontradas
    ''' </param>
    ''' <returns>true si no hubo ningun error, false de lo contrario</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/3/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function ConsultarOcupaciones(ByRef pstrXMLDatosRespuesta As String) As Boolean
        'Dim strError As String

        Try

            'Dim WS As New Servicios.AF.AFI
            Dim strAplicacion As String
            'Dim strbuilder As StringBuilder
            'Dim intCodDep As Integer
            'Dim intOpcConsulta As Integer

            If ConfigurationSettings.AppSettings.HasKeys() Then
                strAplicacion = ConfigurationSettings.AppSettings("IdAplicacion")
            Else
                strAplicacion = "SWPR57"
            End If

            'Invocacion del webservice
            pstrXMLDatosRespuesta = "" 'WS.DatosBasicos(strAplicacion, "COCUTRA")
            Return True

        Catch ex As Exception
            ManejarErrorInterno("CProcesoAfiliarCotizante::ConsultarOcupaciones", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Consulta mediante el uso del paquete Servicios las EPS existentes 
    ''' en Compensar
    ''' </summary>
    ''' <param name="pstrXMLDatosRespuesta">cadena XML con las EPS encontradas
    ''' </param>
    ''' <returns>true si no hubo ningun error, false de lo contrario</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/3/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------  
    Public Function ConsultarEPS(ByRef pstrXMLDatosRespuesta As String) As Boolean
        'Dim strError As String

        Try

            'Dim WS As New Servicios.AF.AFI
            Dim strAplicacion As String
            'Dim strbuilder As StringBuilder
            'Dim intCodDep As Integer
            'Dim intOpcConsulta As Integer

            If ConfigurationSettings.AppSettings.HasKeys() Then
                strAplicacion = ConfigurationSettings.AppSettings("IdAplicacion")
            Else
                strAplicacion = "SWPR57"
            End If

            'Invocacion del webservice
            pstrXMLDatosRespuesta = "" 'WS.EPS(strAplicacion)
            Return True

        Catch ex As Exception
            ManejarErrorInterno("CProcesoAfiliarCotizante::ConsultarEPS", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Consulta mediante el uso del paquete Servicios las clases de afiliación
    ''' en Compensar
    ''' </summary>
    ''' <param name="pstrXMLDatosRespuesta">cadena XML con las clases de afiliacion
    '''  encontradas
    ''' </param>
    ''' <returns>true si no hubo ningun error, false de lo contrario</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/15/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function ConsultarClasesAfiliacion(ByRef pstrXMLDatosRespuesta As String) As Boolean
        Dim strError As String = ""

        Try

            'Dim WS As New Servicios.AF.AFI
            Dim strAplicacion As String
            'Dim strbuilder As StringBuilder
            'Dim intCodDep As Integer
            'Dim intOpcConsulta As Integer

            If ConfigurationSettings.AppSettings.HasKeys() Then
                strAplicacion = ConfigurationSettings.AppSettings("IdAplicacion")
            Else
                strAplicacion = "SWPR57"
            End If

            'Invocacion del webservice
            pstrXMLDatosRespuesta = "" 'WS.DatosBasicos(strAplicacion, "CAFITRA")
            Return True

        Catch ex As Exception
            ManejarErrorInterno("CProcesoAfiliarCotizante::ConsultarClasesAfiliacion", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try

    End Function
 
   
  
  
    Public Function ConsultarBeneficiariosAfiliado(ByVal pobjPaciente As CTrabajador, ByRef pstrXMLDatosRespuesta As String) As Boolean
        Dim strError As String = ""

        Try

            'Dim WS As New Servicios.AF.AFI
            Dim strAplicacion As String
            'Dim strbuilder As StringBuilder
            'Dim intCodDep As Integer
            'Dim intOpcConsulta As Integer

            If ConfigurationSettings.AppSettings.HasKeys() Then
                strAplicacion = ConfigurationSettings.AppSettings("IdAplicacion")
            Else
                strAplicacion = "SWPR57"
            End If

            'Invocacion del webservice

            pstrXMLDatosRespuesta = "" 'WS.Afiliado(strAplicacion, pobjPaciente.IdTrabajador, 1, 3)

            Return True

        Catch ex As Exception
            ManejarErrorInterno("CProcesoAutorizaciones::ConsultarBeneficiariosAfiliado", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try
    End Function
#End Region


End Class ' END CLASS DEFINITION CProcesoAfiliarCotizante



#Region "Namespaces"

Imports LogicaTransaccional
Imports EntidadesNegocio
Imports ManejoMensajes

Imports System.Configuration
Imports System.Text

#End Region

''' -----------------------------------------------------------------------------
''' Project	 : Proceso
''' Class	 : CProcesoAfiliarBeneficiario
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Este proceso modela los diferentes pasos y operaciones necesarias para realizar 
''' el procesos de afiliación de un Beneficiario, tanto a Caja de Compensación, como a 
''' EPS
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	15/09/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Class CProcesoAfiliarBeneficiario
    Inherits CProceso

#Region "Funciones que Heredan de CProceso "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método se encarga de realizar la consulta de los datos básicos de un 
    ''' beneficiario en el ES - 7000 mediante el Web Service AF
    ''' </summary>
    ''' <param name="pBeneficiario">Objeto de entrada y salida que contiene los 
    ''' parámetros de búsqueda del beneficiario, y en el cual, será depositada la 
    ''' información una vez encontrada en el host.</param>
    ''' <returns>True si la operación fue exitosa
    ''' False de lo contrario</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overloads Overrides Function Consultar(ByRef pBeneficiario As CDatosPersona) As Boolean
        Dim strError As String = ""

        Try
            If pBeneficiario.GetType.FullName = "EntidadesNegocio.CBeneficiario" Then

                'Dim WS As New Servicios.AF.AFI
                Dim strRespuesta As String
                Dim objTemp_Be As EntidadesNegocio.CBeneficiario
                Dim strAplicacion As String

                objTemp_Be = CType(pBeneficiario, EntidadesNegocio.CBeneficiario)
                If objTemp_Be.TipoId Is Nothing Or objTemp_Be.IdTrabajador Is Nothing Then
                    Dim strbuilder As StringBuilder

                    CManejadorMensajes.DarMensaje("2", strError)
                    strbuilder = New StringBuilder("Proceso::CProcesoAfiliarBeneficiario::Consultar ")
                    strbuilder.Append(strError)
                    strbuilder.Append(" objTemp_Be.NumeroIdentificacion,objTemp_Be.TipoIdentificacion ,objTemp_Be.IdBeneficiario o objTemp_Be.TipoId son vacios ")
                    CManejadorMensajes.PublicarMensaje(strbuilder.ToString(), EventLogEntryType.Warning)
                    MyBase.mstrError = strError
                    Return False
                Else

                    strAplicacion = ConfigurationSettings.AppSettings("IdAplicacion")
                    strRespuesta = "" 'WS.Afiliado(strAplicacion, Double.Parse(objTemp_Be.IdTrabajador), Integer.Parse(objTemp_Be.TipoId), 1)
                    If StringToCBeneficiario(strRespuesta, pBeneficiario) Then
                        Return True
                    Else
                        CManejadorMensajes.DarMensaje("3", strError)
                        MyBase.mstrError = strError
                        Return False
                    End If
                End If ' objTemp_Tra.TipoIdentificacion Is Nothing Or objTemp_Tra.IdBeneficiario Is Nothing
            Else 'pBeneficiario.GetType.FullName = "EntidadesNegocio.CBeneficiario 
                Dim strBuilder As StringBuilder

                CManejadorMensajes.DarMensaje("1", strError)
                strBuilder = New StringBuilder("Proceso::CProcesoAfiliarBeneficiario::Consultar ")
                strBuilder.Append(strError)
                strBuilder.Append(" Tipo Recibido: ")
                strBuilder.Append(pBeneficiario.GetType.FullName)
                strBuilder.Append(" Tipo Esperado: EntidadesNegocio.CBeneficiario")
                CManejadorMensajes.PublicarMensaje(strBuilder.ToString(), EventLogEntryType.Error)
                MyBase.mstrError = strError
                Return False
            End If
        Catch ex As Exception
            CManejadorMensajes.PublicarMensaje(ex.ToString(), EventLogEntryType.Error)
            CManejadorMensajes.DarMensaje("3", strError)
            MyBase.mstrError = strError
            Return False
        End Try

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Mediante este método se implementa la afiliación de un beneficiario, tanto a Caja 
    ''' de Compensación Familiar, como a EPS
    ''' </summary>
    ''' <param name="pobjDatosAfiliacion">Datos de la afiliación que se va a realizar</param>
    ''' <param name="pCaja">Este parametro booleano indica si se trata de una afiliación a  
    ''' Caja de Compensación Familiar</param>
    ''' <param name="pPOS">Este parametro booleano indica si se trata de una afiliación a  
    '''  EPS
    ''' </param>
    ''' <param name="pstrRespuesta">Cadena con el XML con el resultado del proceso.
    ''' Si hubo errores, o si el proceso fue exitoso, en este parámetro se envía el 
    ''' mensaje correspondiente. </param>
    ''' <returns>True si la operación es exitosa
    ''' False de lo contrario</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/9/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overrides Function Afiliar(ByRef pobjDatosAfiliacion As CDatosAfiliacion, ByVal pCaja As Boolean, ByVal pPOS As Boolean, ByRef pstrRespuesta As String) As Boolean
        Dim objLogicaTransaccional As New LogicaTransaccional.ManejadorTransacciones
        Try
            If Not pobjDatosAfiliacion.EsVacio And Not pobjDatosAfiliacion Is Nothing Then
                If pobjDatosAfiliacion.Persona.GetType.FullName = "EntidadesNegocio.CBeneficiario" Then
                    Dim blnflag As Boolean = True
                    Dim strNideBen As String
                    Dim strTideBen As String

                    Dim objBeneficiario As New EntidadesNegocio.CBeneficiario
                    'Guarda los datos del Beneficiario a adicionar en una bussines entity auxiliar para que no se pierdan los datos que se van a adicionar
                    objBeneficiario = pobjDatosAfiliacion.Persona

                    'Guarda en memoria la identificacion original del beneficiario en caso que se vaya a afiliar para POS y Caja
                    strNideBen = objBeneficiario.NumeroIdentificacion
                    strTideBen = objBeneficiario.TipoIdentificacion

                    'averigua si la afiliación a realizar es de POS o Caja o las 2
                    If pPOS Then
                        Dim objBeneficiarioConsulta As New EntidadesNegocio.CBeneficiario

                        'Asigna la identificacion auxiliar del beneficiario antes de hacer la consulta
                        objBeneficiario.AsignarIdentificacionAuxiliar(1)

                        'Asigna al un bussiness entity de tipo CBeneficiario las variables de consulta de acuerdo a los  parametros ingresados
                        objBeneficiarioConsulta.TipoIdentificacion = objBeneficiario.TipoIdentificacion
                        objBeneficiarioConsulta.NumeroIdentificacion = objBeneficiario.NumeroIdentificacion
                        objBeneficiarioConsulta.IdTrabajador = objBeneficiario.IdTrabajador
                        objBeneficiarioConsulta.TipoId = objBeneficiario.TipoId
                        objBeneficiarioConsulta.TideBen = objBeneficiario.TideBen
                        objBeneficiarioConsulta.NideBen = objBeneficiario.NideBen

                        If Me.Consultar(objBeneficiarioConsulta) Then
                            'De acuerdo a lo consultado en el WS, se asignan las propiedades de estadoAfiliacion del Beneficiario consultado al que se va a afiliar
                            objBeneficiario.EstadoAfiliacionPOS = objBeneficiarioConsulta.EstadoAfiliacionPOS
                            objBeneficiario.EstadoAfiliacionCaja = objBeneficiarioConsulta.EstadoAfiliacionCaja

                            'Se asigna el Beneficiario original a la entidad de afiliación
                            pobjDatosAfiliacion.Persona = objBeneficiario

                            If objLogicaTransaccional.TransaccionAfiliarBeneficiarioPOS(pobjDatosAfiliacion, pstrRespuesta) Then
                                'Guardar datos afiliacion en transar  
                                If GuardarDatosTransar(pobjDatosAfiliacion) Then
                                    blnflag = True
                                Else 'Error guardando datos en transar 
                                    ManejarErrorSalida(17)
                                    blnflag = False
                                End If
                            Else
                                MyBase.ManejarErrorInterno("CProcesoAfiliarBeneficiario::Afiliar", 14, objLogicaTransaccional.DescripcionError)
                                MyBase.ManejarErrorSalida(objLogicaTransaccional.DescripcionError)
                                blnflag = False
                            End If
                        Else
                            'Error en la consulta
                            blnflag = False
                        End If
                    End If
                    If pCaja And blnflag Then
                        Dim strRespuestaCaja As String = pstrRespuesta
                        Dim objBeneficiarioConsulta As New EntidadesNegocio.CBeneficiario

                        'restaura la identificacion original del beneficiario en caso que se haya afiliado a caja
                        If pPOS Then
                            objBeneficiario.NumeroIdentificacion = strNideBen
                            objBeneficiario.TipoIdentificacion = strTideBen
                        End If

                        'Asigna la identificacion auxiliar del beneficiario antes de hacer la consulta
                        objBeneficiario.AsignarIdentificacionAuxiliar(0)

                        'Asigna al un bussiness entity de tipo CBeneficiario las variables de consulta de acuerdo a los  parametros ingresados
                        objBeneficiarioConsulta.TipoIdentificacion = objBeneficiario.TipoIdentificacion
                        objBeneficiarioConsulta.NumeroIdentificacion = objBeneficiario.NumeroIdentificacion
                        objBeneficiarioConsulta.IdTrabajador = objBeneficiario.IdTrabajador
                        objBeneficiarioConsulta.TipoId = objBeneficiario.TipoId
                        objBeneficiarioConsulta.TideBen = objBeneficiario.TideBen
                        objBeneficiarioConsulta.NideBen = objBeneficiario.NideBen

                        If Me.Consultar(objBeneficiarioConsulta) Then
                            'De acuerdo a lo consultado en el WS, se asignan las propiedades de estadoAfiliacion del Beneficiario consultado al que se va a afiliar
                            objBeneficiario.EstadoAfiliacionPOS = objBeneficiarioConsulta.EstadoAfiliacionPOS
                            objBeneficiario.EstadoAfiliacionCaja = objBeneficiarioConsulta.EstadoAfiliacionCaja

                            'Se asigna el Beneficiario original a la entidad de afiliación
                            pobjDatosAfiliacion.Persona = objBeneficiario

                            'If objLogicaTransaccional.TransaccionAfiliarBeneficiarioCaja(pobjDatosAfiliacion, strRespuestaCaja) Then
                            '    pstrRespuesta = pstrRespuesta.Concat(" ", strRespuestaCaja)
                            '    'Guardar datos afiliacion en transar  
                            '    If GuardarDatosTransar(pobjDatosAfiliacion) Then
                            '        blnflag = True
                            '    Else 'Error guardando datos en transar 
                            '        ManejarErrorSalida(17)
                            '        blnflag = False
                            '    End If

                            'Else
                            '    MyBase.ManejarErrorInterno("CProcesoAfiliarBeneficiario::Afiliar", 14, objLogicaTransaccional.DescripcionError)
                            '    MyBase.ManejarErrorSalida(objLogicaTransaccional.DescripcionError)
                            '    blnflag = False
                            'End If

                        Else
                            'Error en la consulta
                            blnflag = False
                        End If
                    End If

                    Return blnflag

                Else 'esta afiliando una entidad que no es de que no es de tipo CBeneficiario
                    ManejarErrorInterno("CProcesoAfiliarBeneficiario::Afiliar", 1, "Esperado: EntidadesNegocio.CBeneficiario" & ", Recibido: " & pobjDatosAfiliacion.Persona.GetType.FullName)
                    ManejarErrorSalida(13)
                    Return False

                End If 'CIERRA: pobjDatosAfiliacion.Persona.GetType.FullName = "EntidadesNegocio.CBeneficiario"
            Else ' La afiliacion esta vacia
                ManejarErrorInterno("CProcesoAfiliarBeneficiario::Afiliar", 4, "error de tipos: al afiliar esperaba un tipo EntidadesNegocio.CBeneficiario")
                ManejarErrorSalida(4)
                Return False

            End If 'If Not p_objAfiliacion Is Nothing Or p_objAfiliacion.EsVacio Then

        Catch ex As Exception
            ManejarErrorInterno("CProcesoAfiliarBeneficiario::Afiliar", 6, ex.ToString())
            ManejarErrorSalida(3)
            Return False
        End Try

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método se encarga de almacenar en la base de datos de Transar los datos 
    ''' correspondientes a una afiliación realizada.
    ''' </summary>
    ''' <param name="p_objAfiliacion">Objeto con los datos de la afiliación que se va
    ''' a almacenar en la base de datos</param>
    ''' <returns>True si la operación fue exitosa
    ''' False de lo contrario</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/9/2004	Created
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
            MyBase.ManejarErrorInterno("CProcesoAfiliarBeneficiario::GuardarDatosTransar", 6, ex.ToString)
            MyBase.ManejarErrorSalida(3)
            Return False
        End Try

    End Function
#End Region

End Class ' END CLASS DEFINITION CProcesoAfiliarBeneficiario



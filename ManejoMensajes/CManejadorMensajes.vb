Imports System.Configuration
Imports System.Xml

''' -----------------------------------------------------------------------------
''' Project	 : ManejoMensajes
''' Class	 : CManejadorMensajes
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Maneja los mensajes controlados del negocio. Esta clase va a leer la descripción
'''  de los mensajes del negocio codificados en un archivo XML.
''' Permite tambien publicar los mensajes de error e informativos de las diferentes 
''' capas de lógica en el log de eventos.
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCLOPEZB]	8/13/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Class CManejadorMensajes

#Region "Constantes"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Constantes de la clase:
    ''' cnsNombreLogDefecto: Nombre por defecto de el log de eventos
    ''' cnsNombreAppDefecto: nombre de la aplicación default por la cual van a quedar clasificados los mensajes en el
    ''' log de eventos.
    ''' cnsConfSetNombreApp: nombre del tag que guarda el nombre de la aplicacion en el archivo de configuracion.
    ''' cnsConfSetNombreLog = nombre del tag que guarda el nombre del log en el archivo de configuracion.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/13/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Const cnsNombreLogDefecto As String = "LOGWSCOMCLIENTES"
    Private Const cnsNombreAppDefecto As String = "TRANSAR"
    Private Const cnsConfSetNombreApp As String = "IdAplicacion"
    Private Const cnsConfSetNombreLog As String = "NombreLog"
#End Region

#Region "Metodos"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Retorna un mensaje del negocio existente en el archivo Mensajes.xml 
    ''' </summary>
    ''' <param name="pstrCodigo">Código del mensaje a buscar</param>
    ''' <param name="pstrMensaje">Descripción del mensaje</param>
    ''' <remarks>
    ''' Si el mensaje no existe en el parametro "pstrMensaje" se devolvera cadena vacia.
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/13/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub DarMensaje(ByVal pstrCodigo As String, _
         ByRef pstrMensaje As String)
        Try
            'Obtener assembly de la librería
            Dim objAssembly As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            'Obtener path completo (incluyendo nombre del archivo) y el nombre del archivo
            Dim strPathCompleto As String = objAssembly.CodeBase
            Dim strNomArchivo As String = objAssembly.GetName().Name & ".dll"

            'Obtener solo el path
            Dim strPathArchivoConfiguracion As String = strPathCompleto.Substring(0, strPathCompleto.Length - strNomArchivo.Length)

            'Abrir archivo de mensajes
            Dim objArchXML As New XmlDocument
            objArchXML.Load(strPathArchivoConfiguracion & "Mensajes.xml")

            'Devuelve valor del mensaje
            If Not objArchXML.GetElementById(pstrCodigo) Is Nothing Then
                pstrMensaje = objArchXML.GetElementById(pstrCodigo).InnerText
            Else
                Dim msgError As String = "No se encontró el mensaje con código " & pstrCodigo & " en el archivo de mensajes de la aplicación. Por favor verifique"
                PublicarMensaje(msgError, EventLogEntryType.Error)
                pstrMensaje = ""
            End If

        Catch objExcepcion As System.IO.FileNotFoundException
            Dim msgError As String = "No se encontró el archivo de mensajes de la aplicación. Por favor verifique"
            PublicarMensaje(msgError, EventLogEntryType.Error)
        End Try

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Publica un mensaje en el log de eventos especificado en el archivo de configuracion
    ''' (web.config o app.config) de la aplicacion cliente que invoca este assembly 
    ''' </summary>
    ''' <param name="pstrMensaje">Descripción del mensaje a publicar</param>
    ''' <param name="pobjTipoMensaje">tipo de mensaje: error, informativo, advertencia</param>
    ''' <remarks>
    ''' este metodo usa las siguientes variables del archivo de configuracion de la aplicación
    ''' cliente:
    ''' NombreAplicacion: nombre de la aplicación por la cual van a quedar clasificados los mensajes en el
    '''  log de eventos.
    ''' NombreLog: nombre del log de eventos 
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/13/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub PublicarMensaje(ByVal pstrMensaje As String, _
          ByVal pobjTipoMensaje As EventLogEntryType)
        Try

            Dim strLogName As String = cnsNombreLogDefecto
            Dim strApplicationName As String = cnsNombreAppDefecto
            'cargar variables de configuracion
            If Not ConfigurationManager.AppSettings(cnsConfSetNombreLog) Is Nothing And ConfigurationManager.AppSettings(cnsConfSetNombreLog) <> "" Then
                strLogName = ConfigurationManager.AppSettings(cnsConfSetNombreLog)
            End If
            If Not ConfigurationManager.AppSettings(cnsConfSetNombreApp) Is Nothing And ConfigurationManager.AppSettings(cnsConfSetNombreApp) <> "" Then
                strApplicationName = ConfigurationManager.AppSettings(cnsConfSetNombreApp)
            End If
            ' verificar la existencia del event source
            If Not EventLog.SourceExists(strApplicationName) Then
                EventLog.CreateEventSource(strApplicationName, strLogName)
            End If
            EventLog.WriteEntry(strApplicationName, pstrMensaje, pobjTipoMensaje)
        Catch ex As Exception

        End Try

    End Sub
#End Region
End Class
' END CLASS DEFINITION CManejadorMensajes


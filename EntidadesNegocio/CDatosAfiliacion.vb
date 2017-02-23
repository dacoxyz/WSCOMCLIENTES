#Region "Namespaces"
Imports System.Globalization
#End Region

''' -----------------------------------------------------------------------------
''' Project	 : BusinessEntities
''' Class	 : CDatosAfiliacion
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Esta clase modela la informacion asociada a una afiliación. 
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCLOPEZB]	8/19/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
<Serializable()> Public Class CDatosAfiliacion

#Region " Propiedades "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Indicador de si el mes de aporte para TDA es el siguiente  subsiguiente. 
    ''' </summary>
    ''' <value>
    ''' False = la afiliación no tiene novedad de ingreso
    ''' True  = la afiliación tiene novedad de ingreso
    ''' </value>
    ''' <remarks>
    ''' Aplica únicamente para afiliaciones a POS
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/19/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property mesAport() As Integer
        Get
            Return mblnmesAport
        End Get
        Set(ByVal Value As Integer)
            mblnmesAport = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Indica si la afiliacion es un translado de Caja de Compensacion Familiar
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	10/7/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TransladoCaja() As Boolean
        Get
            Return mblnTransladoCaja
        End Get
        Set(ByVal Value As Boolean)
            mblnTransladoCaja = True
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Indicador de si la afiliacion tiene novedad de traslado o no
    ''' </summary>
    ''' <value>
    ''' True   = la afiliación tiene novedad de traslado (TDA)
    ''' False  = la afiliación no tiene novedad de traslado (TDA)
    ''' </value>
    ''' <remarks>
    ''' Aplica únicamente para afiliaciones a POS
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/19/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property novedadTDA() As Boolean
        Get
            Return mblnNovedadTDA
        End Get
        Set(ByVal Value As Boolean)
            mblnNovedadTDA = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Comando utilizado en el envío de la afiliación a el ISPEC correspondiente
    ''' en host de Compensar
    ''' </summary>
    ''' <value>
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/19/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Comando() As String
        Get
            Return mstrComando
        End Get
        Set(ByVal Value As String)
            If Value.Length > 3 Then
                Throw New ApplicationException("valor comando máximo de 3 caracteres")
            Else
                mstrComando = Value
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de afiliación, correspondiente a la enumeración TipoAfiliacion, a la cual 
    ''' corresponden los datos que modela la entidad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Tipo() As TipoAfiliacion
        Get
            Return mshrTipo
        End Get
        Set(ByVal Value As TipoAfiliacion)
            mshrTipo = Value
        End Set
    End Property
    Public Property AfiliacionTransar() As String
        Get
            Return mstrAfiliacionTransar
        End Get
        Set(ByVal Value As String)
            mstrAfiliacionTransar = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha en que se realiza(ó) la afiliación 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Fecha() As String
        Get
            If mdtmFechaProceso = mdtmFechaProceso.MinValue Then
                Return ""
            Else
                Return mdtmFechaProceso.ToString(mFormatter.ShortDatePattern)
            End If
        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mdtmFechaProceso = Convert.ToDateTime(Value, mFormatter)
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Codigo de la clase de afiliación.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' Aplica únicamente para afiliaciones POS
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Clase() As Short
        Get
            Return mshrClase
        End Get
        Set(ByVal Value As Short)
            mshrClase = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Número de radicación de la afiliación
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/14/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property NumRadicacion() As Integer
        Get
            Return mintNumRadicacion
        End Get
        Set(ByVal Value As Integer)
            mintNumRadicacion = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Canal o aplicativo de por medio del cual fue realizada la afiliación. 
    ''' </summary>
    ''' <value>
    ''' Transar = 0
    ''' Otro = 1
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/19/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Canal() As TipoCanal
        Get
            Return mshrCanal
        End Get
        Set(ByVal Value As TipoCanal)
            mshrCanal = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Datos de la persona que se está afiliando, bien sea a Caja de Compensación o
    ''' a EPS. 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' Este tipo de persona puede ser, un beneficiario CBeneficiario o un trabajador 
    ''' CTrabajador
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Persona() As CDatosPersona
        Get
            If Not mobjPersona Is Nothing Then
                Return mobjPersona
            Else
                mobjPersona = New CDatosPersona
                Return mobjPersona
            End If
        End Get
        Set(ByVal Value As CDatosPersona)
            If Not Value Is Nothing Then
                mobjPersona = Value
            Else
                mobjPersona = New CDatosPersona
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Empresa que está realizando la afiliación
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Empresa() As CEmpresaAfiliado
        Get
            If Not mobjEmpresa Is Nothing Then
                Return mobjEmpresa
            Else
                mobjEmpresa = New CEmpresaAfiliado
                Return mobjEmpresa
            End If
        End Get
        Set(ByVal Value As CEmpresaAfiliado)
            If Not Value Is Nothing Then
                mobjEmpresa = Value
            Else
                mobjEmpresa = New CEmpresaAfiliado
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Identificacion del usuario autenticado en Transar que realizó la afiliacion
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	11/1/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property IdUsuarioEjecutor() As String
        Get
            Return mstrIdusuarioEjecutor
        End Get
        Set(ByVal Value As String)
            mstrIdusuarioEjecutor = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Contiene el mensaje de respuesta de la transaccion en el ES7000. 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	4/11/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property RespuestaHost() As String
        Get
            Return mstrRespuestaHost
        End Get
        Set(ByVal Value As String)
            mstrRespuestaHost = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Documento de estado de salud para la afiliacion
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	10/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property DocEstadoSalud() As Boolean
        Get
            Return mblnDocEstadoSalud
        End Get
        Set(ByVal Value As Boolean)
            mblnDocEstadoSalud = Value
        End Set

    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Documento de iddentificacion para la afiliacion
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	10/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property DocIdentificacion() As Boolean
        Get
            Return mblnDocIdentificacion
        End Get
        Set(ByVal Value As Boolean)
            mblnDocIdentificacion = Value
        End Set

    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Documento de Parentesco para la afiliacion
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	10/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property DocParentesco() As Boolean
        Get
            Return mblnDocParentesco
        End Get
        Set(ByVal Value As Boolean)
            mblnDocParentesco = Value
        End Set

    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Documento de Dependencia para la afiliacion
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	10/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property DocDependencia() As Boolean
        Get
            Return mblnDocDependencia
        End Get
        Set(ByVal Value As Boolean)
            mblnDocDependencia = Value
        End Set

    End Property
    Public Property EpsOrigen() As String

        Get

            Return mblnEpsOrigen

        End Get

        Set(ByVal Value As String)

            mblnEpsOrigen = Value

        End Set



    End Property

#End Region

#Region "Miembros Privados "
    Private mblnDocEstadoSalud As Boolean
    Private mblnDocIdentificacion As Boolean
    Private mblnDocParentesco As Boolean
    Private mblnDocDependencia As Boolean
    Private mblnmesAport As Integer
    Private mblnNovedadTDA As Boolean
    Private mstrComando As String
    Private mshrTipo As Short
    Private mdtmFechaProceso As Date
    Private mobjPersona As CDatosPersona
    Private mobjEmpresa As CEmpresaAfiliado
    Private mFormatter As DateTimeFormatInfo
    Private mshrClase As Short
    Private mintNumRadicacion As Integer
    Private mshrCanal As Short
    Private mblnTransladoCaja As Boolean
    Private mstrIdusuarioEjecutor As String
    Private mstrAfiliacionTransar As String
    Private mstrRespuestaHost As String
    Private mblnEpsOrigen As String

#End Region

#Region " Constructores "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' El constructor por defecto. Se encarga de inicializar el lenguage de la 
    ''' entidad a es-CO
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub New()
        mFormatter = New CultureInfo("es-CO", True).DateTimeFormat
    End Sub
#End Region

#Region " Metodos "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método informa si el objeto está o no vacío.
    ''' </summary>
    ''' <returns>True si el objeto está vacio 
    ''' False si el objeto no esta vacío</returns>
    ''' <remarks>
    ''' Un objeto de tipo CDatosAfiliación se define como vacío si la fecha de afiliación
    ''' se encuentra vacía y si los nombres de la persona que contiene esta entidad también
    ''' se encuentran vacíos.
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	20/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function EsVacio() As Boolean
        If IsDate(mdtmFechaProceso) And Me.Persona.Nombres <> "" Then
            Return False
        Else : Return True
        End If
    End Function
#End Region

End Class
' END CLASS DEFINITION CAfiliacionPOS



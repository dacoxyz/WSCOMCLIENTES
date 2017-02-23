#Region "Namespaces"
Imports System.Globalization
Imports System.Threading
Imports System.Xml.Serialization
#End Region
''' -----------------------------------------------------------------------------
''' Project	 : BusinessEntities
''' Class	 : CDatosPersona
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Esta clase define los atributos y operaciones basicas de los datos personales,
''' de los individuos que van a actuar dentro del sistema. 
''' Esta clase está asegurada mediante verificación de tipos y de contenido de 
''' sus propiedades, para evitar ataques como buffer overrun, sql injection, etc
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	8/10/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
'''
<Serializable(), _
XmlInclude(GetType(CBeneficiario)), _
XmlInclude(GetType(CTrabajador))> _
Public Class CDatosPersona

#Region "Propiedades"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta propiedad almacena el primer apellido de un individuo. Que puede ser,
    ''' beneficiario o cotizante.
    ''' Se hace validación de tipos de datos, si se encuentra un tipo incoherente,
    ''' se lanza una excepción de seguridad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property PrimerApellido() As String
        Get
            Return mstrPrimerApellido
        End Get
        Set(ByVal Value As String)
            If Value.Length > 50 Then
                ''muy grande lanzar excepcion
                Throw New ApplicationException("Un apellido debe ser de màximo 50 caracteres")
            Else
                mstrPrimerApellido = Value
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta propiedad almacena el segundo apellido de un individuo. Que puede ser,
    ''' beneficiario o cotizante.
    ''' Se hace validación de tipos de datos, si se encuentra un tipo incoherente,
    ''' se lanza una excepción de seguridad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property SegundoApellido() As String
        Get
            Return mstrSegundoApellido
        End Get
        Set(ByVal Value As String)
            If Value.Length > 50 Then
                Throw New ApplicationException("Un apellido debe ser de màximo 50 caracteres")
            Else
                mstrSegundoApellido = Value
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta propiedad almacena los nombres de un individuo. Que puede ser,
    ''' beneficiario o cotizante.
    ''' Se hace validación de tipos de datos, si se encuentra un tipo incoherente,
    ''' se lanza una excepción de seguridad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Nombres() As String
        Get
            Return mstrNombres
        End Get
        Set(ByVal Value As String)
            If Value.Length > 80 Then
                Throw New ApplicationException("Los nombres deben ser de máximo 50 caracteres")
            Else
                mstrNombres = Value
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta propiedad es un caractér que indica el género de un individuo que 
    ''' actúa en el sistma, cotizante o beneficiario.
    ''' Se hace validación de tipos y de contenido. Si se encuentran incoherencias
    ''' se lanza una excepción de seguridad
    ''' </summary>
    ''' <value>
    ''' F = Género Femenino 
    ''' M = Género Masculino
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Genero() As String
        Get
            Return mstrGenero
        End Get
        Set(ByVal Value As String)
            If Value = "M" Or Value = "F" Then
                mstrGenero = Value
            Else
                Throw New ApplicationException("El género solo puede ser M (masculino) o F (Femenino)")
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Alamcena la fecha de nacimiento de uno de los individuos que actúa en
    ''' el sistema.
    ''' Se hace validación de tipos. Si se encuentran incoherencias se lanza
    ''' una excepcion de seguridad.
    ''' </summary>
    ''' <value> aaaa/mm/dd </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechaNacimiento() As String
        Get
            If mstrFechaNacimiento = mstrFechaNacimiento.MinValue Then
                Return ""
            Else
                Return mstrFechaNacimiento.ToString(mFormatter.ShortDatePattern)
            End If
        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mstrFechaNacimiento = Convert.ToDateTime(Value, mFormatter)
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta propiedad almacena la dirección de residencia de un individuo que participe
    ''' en el sistema.
    ''' Se hace validación de tipos y de contenido. Si hay incoherencias se 
    ''' lanza una excepciòn de seguridad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Direccion() As String
        Get
            Return mstrDireccion
        End Get
        Set(ByVal Value As String)
            If Value.Length > 100 Then
                Throw New ApplicationException("Una direccion puede tener máximo 100 caracteres")
            Else
                mstrDireccion = Value
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Barrio en que habita el individuo
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Barrio() As String
        Get
            Return mstrBarrio
        End Get
        Set(ByVal Value As String)
            If Value.Length > 100 Then
                Throw New ApplicationException("Una barrio puede tener máximo 100 caracteres")
            Else
                mstrBarrio = Value
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Almacena el teléfono de un individuo que participa en el sistema.
    ''' Se hace validaciòn de tipos y de contenido, si se encuentran 
    ''' incoherencias se lanza una excepción de seguridad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Telefono() As String
        Get
            Return mstrTelefono
        End Get
        Set(ByVal Value As String)
            If Value.Length > 50 Then
                Throw New ApplicationException("El número de telefono no debe tener más de 50 caracteres")
            Else
                mstrTelefono = Value
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Almacena la ciudad de residencia de un individuo que participa en el sistema.
    ''' Se hace verificación de tipos y de contenido. Si hay incoherencias se 
    ''' lanza una excepción de seguridad
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Ciudad() As String
        Get
            Return mstrCiudad
        End Get
        Set(ByVal Value As String)
            mstrCiudad = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' En esta propiedad se almacena la categoria a la cual pertence un individuo
    ''' en caso de estar afiliado a la empresa.
    ''' </summary>
    ''' <value>
    ''' Categorias Posibles 
    ''' A
    ''' B
    ''' C
    ''' D
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Categoria() As String
        Get
            Return mstrCategoria
        End Get
        Set(ByVal Value As String)
            mstrCategoria = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de Zona en la cual habita el individuo.
    ''' </summary>
    ''' <value>
    ''' R = Rural 
    ''' U = Urbana
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TipoZona() As String
        Get
            Return mstrTipoZona
        End Get
        Set(ByVal Value As String)
            mstrTipoZona = Value

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Zona debe ser generalmente 999
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[WEMUNEVARM]	07/02/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Zona() As String
        Get
            Return mstrZona
        End Get
        Set(ByVal Value As String)
            mstrZona = Value

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Codigo de la EPS a la cual estaba afiliado anteriormente el trabajador
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property CodigoEPSAnterior() As Short
        Get
            Return mintCodigoEPSAnterior
        End Get
        Set(ByVal Value As Short)
            mintCodigoEPSAnterior = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha de afiliación a la EPS anterior, en caso de haber estado afiliado a 
    ''' alguna. 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechaAfiliacionEPSAnterior() As String
        Get
            If mdtmFechaAfiliacionEPSAnterior = mdtmFechaAfiliacionEPSAnterior.MinValue Then
                Return ""
            Else
                Return mdtmFechaAfiliacionEPSAnterior.ToString(mFormatter.ShortDatePattern)
            End If

        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mdtmFechaAfiliacionEPSAnterior = Convert.ToDateTime(Value, mFormatter)
            End If
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FechaAfiliacionPOS() As String
        Get
            If mdtmFechaAfiliacionPOS = mdtmFechaAfiliacionPOS.MinValue Then
                Return ""
            Else
                Return mdtmFechaAfiliacionPOS.ToString(mFormatter.ShortDatePattern)
            End If

        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mdtmFechaAfiliacionPOS = Convert.ToDateTime(Value, mFormatter)
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Edad de la persona que se está modelando en la entidad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Edad() As String
        Get
            Return mintEdad
        End Get
        Set(ByVal Value As String)
            mintEdad = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Correo Electrónico de la persona
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	10/13/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Email() As String
        Get
            Return mstrEmail
        End Get
        Set(ByVal Value As String)
            mstrEmail = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' identificacion de la persona
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	10/13/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property identificacion() As String
        Get
            Return mstridentificacion
        End Get
        Set(ByVal Value As String)
            mstridentificacion = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de identificacion de la persona
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[EXTCENJDCASTROM]	26/10/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TipoIdentificacion() As String
        Get
            Return mstrTipoIdentificacion
        End Get
        Set(ByVal Value As String)
            mstrTipoIdentificacion = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' personas a cargo  de la persona
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	10/13/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property personaCargo() As String
        Get
            Return mstrpersonaCargo
        End Get
        Set(ByVal Value As String)
            mstrpersonaCargo = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta propiedad almacena el segundo nombre de una persona
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	10/27/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property SegundoNombre() As String
        Get
            Return mstrSegundoNombre
        End Get
        Set(ByVal Value As String)
            mstrSegundoNombre = Value

        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Estrato de persona
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	1/13/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Estrato() As String
        Get
            Return mstrEstrato
        End Get
        Set(ByVal Value As String)
            mstrEstrato = Value
        End Set
    End Property
    
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fax de la persona
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[EXTCENJDCASTROM]	26/10/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Fax() As String
        Get
            Return mstrFax
        End Get
        Set(ByVal Value As String)
            mstrFax = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Celular de la persona
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[EXTCENJDCASTROM]	26/10/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TelefonoCelular() As String
        Get
            Return mstrTelefonoCelular
        End Get
        Set(ByVal Value As String)
            mstrTelefonoCelular = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Extension
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[WEMUNEVARM]	07/02/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property ExtensionTelef() As String
        Get
            Return mstrExtTelef
        End Get
        Set(ByVal Value As String)
            mstrExtTelef = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Estado civil del trabajador
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property EstadoCivil() As String
        Get
            Return mstrEstadoCivil
        End Get
        Set(ByVal Value As String)
            If Value.Length > 4 Then
                Throw New ApplicationException("Estado civil debe tener maximo 3 caracteres")
            Else
                mstrEstadoCivil = Value
            End If
        End Set
    End Property


#End Region

#Region " Miembros Privados "
    Private mstrEstadoCivil As String
    Private mstrpersonaCargo As String
    Private mstridentificacion As String
    Private mstrPrimerApellido As String
    Private mstrSegundoApellido As String
    Private mstrNombres As String
    Private mstrSegundoNombre As String
    Private mstrGenero As String
    Private mstrExtencionTel As String
    Private mstrFechaNacimiento As Date
    Private mstrDireccion As String
    Private mstrTelefono As String
    Private mstrExtTelef As String
    Private mstrCiudad As String
    Private mstrCategoria As String
    Private mstrBarrio As String
    Private mintEdad As Short
    Private mstrTipoZona As String
    Private mstrZona As String
    Private mintCodigoEPSAnterior As Short
    Private mdtmFechaAfiliacionEPSAnterior As Date
    Private mdtmFechaAfiliacionPOS As Date
    Private mstrEmail As String
    Private mstrEstrato As String
    Private mstrFax As String
    Private mstrTipoIdentificacion As String
    Private mstrTelefonoCelular As String


#End Region

#Region "Miembros Protegidos"
    Protected mFormatter As DateTimeFormatInfo
#End Region

#Region " Constructores "

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor por defecto. Se encarga de inicializar el lenguage de la entidad 
    ''' en es-CO, y de asignar como zona por defecto "U" (zona urbana).
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub New()
        mFormatter = New CultureInfo("es-CO", True).DateTimeFormat
        Thread.CurrentThread.CurrentUICulture = New CultureInfo("es-CO", True)
        mstrTipoZona = "U"c
    End Sub

#End Region

#Region " Metodos "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método informa si el objeto está o no vacío.
    ''' Un objeto de tipo CDatosPersona o de cualquiera de los tipos que extienden a este
    ''' se considera vacio, si los datos nombres, apellidos, numero y tipo de identificacion
    ''' están vacíos.
    ''' </summary>
    ''' <returns>True si el objeto está vacio 
    ''' False si el objeto no esta vacío</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	20/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Protected Function EsVacio() As Boolean
        If Me.Nombres = "" Or Me.Nombres Is Nothing _
        And Me.PrimerApellido = "" Or Me.PrimerApellido Is Nothing Then
            Return True
        Else
            Return False
        End If
    End Function
#End Region
End Class
' END CLASS DEFINITION CDatosPersona



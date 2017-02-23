#Region "Namespaces"
Imports System.Configuration
Imports System.Globalization
Imports ManejoMensajes

#End Region
''' -----------------------------------------------------------------------------
''' Project	 : BusinessEntities
''' Class	 : CTrabajador
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Esta clase, que hereda de CDatosPersona, modela los datos principales de 
''' un trabajador y los datos especializados para el mismo. 
''' Es una clase segura, mediante la verificación de tipos y contenidos, que 
''' evitan cualquier tipo de ataque de tipo injection o buffer overrun 
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	8/10/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
<Serializable()> Public Class CTrabajador
    Inherits CDatosPersona

#Region "Datos Trabajador"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Identificación del trabajador
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property IdTrabajador() As String
        Get
            Return mstrIdTrabajador
        End Get
        Set(ByVal Value As String)
            If Value.Length > 50 Then
                Throw New ApplicationException("La identificación debe tener menos de 50 caracteres")
            Else
                mstrIdTrabajador = Value
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de Identificación del trabajador 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shadows Property TipoIdentificacion() As String
        Get
            Return mstrTipoID
        End Get
        Set(ByVal Value As String)
            If Value.Length > 4 Then
                Throw New ApplicationException("Tipo de Identificación debe tener máximo 3 caracteres")
            Else
                mstrTipoID = Value
            End If

        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Lista de empresas a las que está vinculado el trabajador.
    ''' Cada uno de los item de esta lista debe ser un objeto del 
    ''' tipo EmpresaAfiliado
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property EmpresasAsociadas() As CEmpresaAfiliado()

        Get
            Return marrEmpresas

        End Get
        Set(ByVal Value As CEmpresaAfiliado())
            marrEmpresas = Value
        End Set

    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha de afiliación al sistema de seguridad social. (Primera vez que se 
    ''' afilió a una EPS)
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechaAfiliacionSistema() As String
        Get
            If mdtmFechaSistema = mdtmFechaSistema.MinValue Then
                Return ""
            Else
                Return mdtmFechaSistema.ToString(mFormatter.ShortDatePattern)
            End If

        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mdtmFechaSistema = Convert.ToDateTime(Value, MyBase.mFormatter)
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha de retiro del trabajador
    ''' Se hace validación de tipos. Si se encuentran incoherencias se lanza
    ''' una excepcion de seguridad.
    ''' </summary>
    ''' <value> aaaa/mm/dd </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cenclopezb]	3/17/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechaRetiro() As String
        Get
            If mstrFechaRetiro = mstrFechaRetiro.MinValue Then
                Return ""
            Else
                Return mstrFechaRetiro.ToString(mFormatter.ShortDatePattern)
            End If
        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mstrFechaRetiro = Convert.ToDateTime(Value, mFormatter)
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta propiedad representa la empresa primaria por la cual se encuentra registrado
    ''' el trabajador a Caja de Compensación Familiar o a EPS en AFE21
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	30/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property EmpresaPrimaria() As CEmpresaAfiliado
        Get
            If Not mobjEmpresaPrimaria Is Nothing Then
                Return mobjEmpresaPrimaria
            Else
                mobjEmpresaPrimaria = New CEmpresaAfiliado
                Return mobjEmpresaPrimaria
            End If
        End Get
        Set(ByVal Value As EntidadesNegocio.CEmpresaAfiliado)
            If Not Value Is Nothing Then
                mobjEmpresaPrimaria = Value
            Else
                mobjEmpresaPrimaria = New CEmpresaAfiliado
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Código de la ocupación del trabajador.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property CodigoOcupacion() As Short
        Get
            Return mintCodigoOcupacion
        End Get
        Set(ByVal Value As Short)
            mintCodigoOcupacion = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Booleano que indica si el trabajador es o cabeza de familia 
    ''' </summary>
    ''' <value>
    ''' True  = Es cabeza de familia 
    ''' False = No es cabeza de familia 
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property CabezaFamilia() As Boolean
        Get
            Return mblnCabezaFamilia
        End Get
        Set(ByVal Value As Boolean)
            mblnCabezaFamilia = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Indicador de si el trabajador tiene dos o mas empleos 
    ''' </summary>
    ''' <value>
    ''' True   = El trabajador tiene mas de un empleo 
    ''' False  = El trabajador solo tiene un empleo 
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property SegundoEmpleo() As Boolean
        Get
            Return mblSegundoEmpleo
        End Get
        Set(ByVal Value As Boolean)
            mblSegundoEmpleo = Value
        End Set

    End Property


#End Region

#Region "Datos Conyuge"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Booleano que indica si el conyuge también es cotizante.
    ''' </summary>
    ''' <value>
    ''' True  = El conyuge es cotizante
    ''' False = El conyuge no es cotizante 
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property ConyugeCotiza() As Boolean
        Get
            Return mblnConyugeCotiza
        End Get
        Set(ByVal Value As Boolean)

            mblnConyugeCotiza = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de Identifación del conyuge, en caso de haberlo.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TipoIdentificacionConyuge() As String
        Get
            Return mstrTipoIdentificacionConyuge
        End Get
        Set(ByVal Value As String)
            If Value.Length > 4 Then
                Throw New ApplicationException("Tipo de Identificación debe tener máximo 3 caracteres")
            Else
                mstrTipoIdentificacionConyuge = Value
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Número de identificación del conyuge en caso de haberlo
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property NumeroIdentificacionConyuge() As String
        Get
            Return mstrNumeroIdentificacionConyuge
        End Get
        Set(ByVal Value As String)
            If Value.Length >= 50 Then
                Throw New ApplicationException("La identificación debe tener menos de 50 caracteres")
            Else
                mstrNumeroIdentificacionConyuge = Value
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Salario del Conyuge 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property SalarioConyuge() As Double
        Get
            Return mdblSalarioConyuge
        End Get
        Set(ByVal Value As Double)
            mdblSalarioConyuge = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha de Registro en Venta de Servicios
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	18/12/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Registro() As String
        Get
            Return mstrFechaRegistro
        End Get
        Set(ByVal Value As String)
            mstrFechaRegistro = Value

        End Set
    End Property

#End Region

#Region "Miembros Privados"
    Private mstrFechaRegistro As String
    Private mstrIdTrabajador As String
    Private mstrTipoID As String

    Private mdtmFechaSistema As Date
    Private marrEmpresas As CEmpresaAfiliado()
    Private mobjEmpresaPrimaria As CEmpresaAfiliado
    Private mintCodigoOcupacion As Short
    Private mblnCabezaFamilia As Boolean
    Private mblnConyugeCotiza As Boolean
    Private mstrTipoIdentificacionConyuge As String
    Private mstrNumeroIdentificacionConyuge As String
    Private mdblSalarioConyuge As Double
    Private mblSegundoEmpleo As Boolean
    Private mstrFechaRetiro As Date

#End Region

#Region " Constructores "

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor por defecto de la clase. Se encarga de llamar el constructor de 
    ''' la super clase (CDatosPersona) y de inicializar el arreglo de empresas 
    ''' asociadas al trabajador, como un arreglo vacío.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub New()
        MyBase.New()
        Me.marrEmpresas = New CEmpresaAfiliado() {}
    End Sub

#End Region

#Region " Metodos "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método permite determinar si la entidad está o no vacía.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Una entidad de la clase CTrabajador se difine como vacía si el tipo de identificación
    ''' o el número de identificación son vacíos.
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Vacio() As Boolean

        If Me.IdTrabajador = "" Or Me.TipoIdentificacion = "" _
          Or Me.IdTrabajador Is Nothing Or Me.TipoIdentificacion Is Nothing Then
            Return True
        ElseIf MyBase.EsVacio Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta función, permite determinar si un trabajador está o no afiliado a Caja de 
    ''' Compensación Familiar, por una empresa determinada
    ''' </summary>
    ''' <param name="strIdEmpresa">Numero de Identificacion de la empresa con la que se quiere probar 
    ''' la afiliación </param>
    ''' <param name="strTipoIdEmpresa">Tipo de Identificacion de la empresa con la que se quiere 
    ''' probar la afiliacion </param>
    ''' <returns>True si el trabajador pertenece o pertenecio a esta empresa 
    ''' False de lo Contrario
    ''' </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	24/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function EstaAfiliado(ByVal strIdEmpresa As String, ByVal strTipoIdEmpresa As String) As Boolean

        Dim empresa As CEmpresaAfiliado


        If Me.EmpresaPrimaria.IdEmpresa = strIdEmpresa And Me.EmpresaPrimaria.TipoIdentificacion = strTipoIdEmpresa And Me.EmpresaPrimaria.EstadoAfiliacionCaja = 0 Then
            Return True
        End If

        For Each empresa In Me.EmpresasAsociadas
            If empresa.IdEmpresa = strIdEmpresa And empresa.TipoIdentificacion = strTipoIdEmpresa And empresa.EstadoAfiliacionCaja = 0 Then
                Return True
            End If
        Next
        Return False

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Esta función, permite determinar si un trabajador está o no afiliado a POS
    '''  por una empresa determinada
    ''' </summary>
    ''' <param name="strIdEmpresa">Numero de Identificacion de la empresa con la que se quiere probar 
    ''' la afiliación </param>
    ''' <param name="strTipoIdEmpresa">Tipo de Identificacion de la empresa con la que se quiere 
    ''' probar la afiliacion </param>
    ''' <returns>True si el trabajador pertenece o pertenecio a esta empresa 
    ''' False de lo Contrario
    ''' </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	24/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function EstaAfiliadoPOS(ByVal strIdEmpresa As String, ByVal strTipoIdEmpresa As String) As Boolean

        Dim empresa As CEmpresaAfiliado


        If Me.EmpresaPrimaria.IdEmpresa = strIdEmpresa And Me.EmpresaPrimaria.TipoIdentificacion = strTipoIdEmpresa And (Me.EmpresaPrimaria.EstadoAfiliacionPOS = 0 Or Me.EmpresaPrimaria.EstadoAfiliacionPOS = 4 Or (Me.EmpresaPrimaria.EstadoAfiliacionPOS = 3 And Me.EmpresaPrimaria.FechaIngresoPOS <> "")) Then
            Return True
        End If

        For Each empresa In Me.EmpresasAsociadas
            If empresa.IdEmpresa = strIdEmpresa And empresa.TipoIdentificacion = strTipoIdEmpresa And (empresa.EstadoAfiliacionPOS = 0 Or empresa.EstadoAfiliacionPOS = 4 Or (empresa.EstadoAfiliacionPOS = 3 And empresa.FechaIngresoPOS <> "")) Then
                Return True
            End If
        Next
        Return False

    End Function


#End Region

End Class
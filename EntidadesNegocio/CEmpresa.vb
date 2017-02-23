''' -----------------------------------------------------------------------------
''' Project	 : BusinessEntities
''' Class	 : CEmpresa
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Esta clase modela una empresa, que puede o no estar registrada como cliente
''' de Compensar.
''' 
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	8/10/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
<Serializable()> Public Class CEmpresa

#Region " Propiedades "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Almacena el n�mero de identificaci�n de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property IdEmpresa() As String
        Get
            Return mstrIdEmpresa
        End Get
        Set(ByVal Value As String)
            If Value.Length > 50 Then
                Throw New ApplicationException("Identificaci�n de la empresa debe ser m�ximo de 50 caracteres")
            Else
                mstrIdEmpresa = Value
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Nombre de la empresa 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Nombre() As String
        Get
            Return mstrNombre
        End Get
        Set(ByVal Value As String)

            mstrNombre = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de Identificacion de la Empresa 
    ''' </summary>
    ''' <value>
    ''' NIT para empresas 
    ''' CC  para independientes registrados en c�mara de comercio
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TipoIdentificacion() As String
        Get
            Return mstrTipoIdentificacion
        End Get
        Set(ByVal Value As String)
            If Value.Length > 4 Then
                Throw New ApplicationException("Tipo de Identificaci�n debe tener m�ximo 3 caracteres")
            Else
                mstrTipoIdentificacion = Value
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Dependencia de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' Algunas dependencias trabajan como empresas independientes, bajo un mismo NIT,
    ''' por esta raz�n, durante todo el proceso, desde que ingresan a la aplicaci�n,
    ''' y a lo largo de todas las operaciones que realizan las empresas en Transar, estas
    ''' son identificadas a trav�s de su tipo de identificaci�n, su n�mero de identificaci�n 
    ''' y en el caso que corresponda, su dependencia.
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Dependencia() As String
        Get
            Return mstrDependenciaEmpresa
        End Get
        Set(ByVal Value As String)
            mstrDependenciaEmpresa = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Email de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/29/2005	Created
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
    ''' Clasificacion de la empresa por tama�o.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/29/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property ClaseAportante() As String
        Get
            Return mstrClaseAportante
        End Get
        Set(ByVal Value As String)

            mstrClaseAportante = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Direccion de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/29/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Direccion() As String
        Get
            Return mstrDireccion
        End Get
        Set(ByVal Value As String)

            mstrDireccion = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Ciudad de la sede principal de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/29/2005	Created
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
    ''' Departamento de la sede principal de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/29/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Departamento() As String
        Get
            Return mstrDepartamento
        End Get
        Set(ByVal Value As String)

            mstrDepartamento = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Telefono principal de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/29/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Telefono() As String
        Get
            Return mstrTelefono
        End Get
        Set(ByVal Value As String)

            mstrTelefono = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fax principal de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	6/29/2005	Created
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
    ''' Tipo de empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/12/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Tipo() As TipoEmpresa
        Get
            Return menmTipoEmpresa
        End Get
        Set(ByVal Value As TipoEmpresa)

            menmTipoEmpresa = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Codigo DANE de la ciudad
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	07/10/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property CodCiudad() As String
        Get
            Return mCodCiudad
        End Get
        Set(ByVal Value As String)

            mCodCiudad = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' C�digo DANE del departamento
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	07/10/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property CodDepartamento() As String
        Get
            Return mCodDepartamento
        End Get
        Set(ByVal Value As String)

            mCodDepartamento = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Nombre del representante legal de la Empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	10/10/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property NombreRepresentanteLegal() As String
        Get
            Return mstrNombreRepresentanteLegal
        End Get
        Set(ByVal Value As String)
            mstrNombreRepresentanteLegal = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Identificacion del representante legal
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	10/10/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property IdRepresentanteLegal() As String
        Get
            Return mstrIDRepresentanteLegal
        End Get
        Set(ByVal Value As String)
            mstrIDRepresentanteLegal = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Nombre de la persona de contacto dentro de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	10/10/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Contacto() As String
        Get
            Return mstrContacto
        End Get
        Set(ByVal Value As String)
            mstrContacto = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Nombre del cargo de la persona de contacto dentro de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	10/10/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property CargoContacto() As String
        Get
            Return mstrCargoContacto
        End Get
        Set(ByVal Value As String)
            mstrCargoContacto = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha de ingreso del contacto a la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	3/30/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechaIngresoContacto() As String
        Get
            Return mstrFechaIngresoContacto
        End Get
        Set(ByVal Value As String)
            mstrFechaIngresoContacto = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha de aporte
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[WEMUNEVARM]	20/02/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechadeAporte() As String
        Get
            Return mstrFechadeAporte
        End Get
        Set(ByVal Value As String)
            mstrFechadeAporte = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Telefono opcional de la empresa
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	10/10/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TelefonoOpcional() As String
        Get
            Return mstrTelefonoOpcional
        End Get
        Set(ByVal Value As String)

            mstrTelefonoOpcional = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Valor del aporte en $$
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[WEMUNEVARM]	20/02/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property ValorAporte() As String
        Get
            Return mstrValorAporte
        End Get
        Set(ByVal Value As String)

            mstrValorAporte = Value
        End Set
    End Property



#End Region

#Region "Miembros Privados "
    Private mstrIdEmpresa As String
    Private mstrNombre As String
    Private mstrTipoIdentificacion As String
    Private mstrDependenciaEmpresa As String
    Private mstrEmail As String
    Private mstrClaseAportante As String
    Private mstrDireccion As String
    Private mstrCiudad As String
    Private mstrDepartamento As String
    Private mstrTelefono As String
    Private mstrFax As String
    Private menmTipoEmpresa As TipoEmpresa
    Private mCodCiudad As String
    Private mCodDepartamento As String
    Private mstrNombreRepresentanteLegal As String
    Private mstrIDRepresentanteLegal As String
    Private mstrContacto As String
    Private mstrCargoContacto As String
    Private mstrTelefonoOpcional As String
    Private mstrFechaIngresoContacto As String
    Private mstrFechadeAporte As String
    Private mstrValorAporte As String


#End Region

#Region " Metodos "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este m�todo informa si el objeto est� o no vac�o.
    ''' Un objeto de tipo CEmpresa o de cualquiera de los tipos que extienden a este
    ''' se considera vacio, si los datos nombres numero y tipo de identificacion
    ''' est�n vac�os.
    ''' </summary>
    ''' <returns>True si el objeto est� vacio 
    ''' False si el objeto no esta vac�o</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	20/08/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function EsVacio() As Boolean

        If Me.mstrIdEmpresa = "" Or Me.mstrIdEmpresa Is Nothing _
        Or Me.mstrTipoIdentificacion = "" Or Me.mstrTipoIdentificacion Is Nothing Then
            Return True
        Else
            Return False
        End If
    End Function
#End Region

End Class





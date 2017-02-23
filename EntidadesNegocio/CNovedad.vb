#Region "Namespaces"
Imports System.Globalization
Imports System.Threading
#End Region
''' -----------------------------------------------------------------------------
''' Project	 : BusinessEntities
''' Class	 : CNovedad
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Esta entidad modela los datos correspondientes a una novedad. 
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	12/23/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
<Serializable()> Public Class CNovedad

#Region " Miembros Privados"
    Private mobjPersona As CDatosPersona
    Private mobjEmpresa As CEmpresaAfiliado
    Private mstrComando As String
    Private mdtmFecha As DateTime
    Private mobjTipoNovedad As Short
    Private mFormatter As DateTimeFormatInfo
    Private mstrNovING As Boolean
    Private mstrNovRET As Boolean
    Private mstrNovTDA As Boolean
    Private mstrNovTAA As Boolean
    Private mstrNovVSP As Boolean
    Private mstrNovVST As Boolean
    Private mstrNovSLN As Boolean
    Private mstrNovIGE As Boolean
    Private mstrNovLMA As Boolean
    Private mstrNovVAC As Boolean


#End Region

#Region "Propiedades"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Empresa que realiza la novedad.
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
    ''' Afiliado que será modificado por la novedad. Bien sea, beneficiario o trabajador.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
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
    ''' Comando aplicado para ejecutar la novedad en el ISPEC
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Property Comando() As String
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
    ''' Fecha en que se realiza la modifiacación.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Fecha() As String
        Get
            If mdtmFecha = mdtmFecha.MinValue Then
                Return ""
            Else
                Return mdtmFecha.ToString(mFormatter.ShortDatePattern)
            End If

        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mdtmFecha = Convert.ToDateTime(Value, mFormatter)
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Entero modelado en la estructura TipoNovedad, que especifica si la novedad corresponde
    ''' a una modificación o a un retiro. 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TipoDeNovedad() As TipoNovedad
        Get
            Return mobjTipoNovedad
        End Get
        Set(ByVal Value As TipoNovedad)
            mobjTipoNovedad = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Novedades de la planilla de autoliquidacion de aportes 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	05/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property ING() As Boolean
        Get
            Return mstrNovING
        End Get
        Set(ByVal Value As Boolean)
            mstrNovING = Value
        End Set

    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Novedades de la planilla de autoliquidacion de aportes 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	05/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '''    
    Public Property RET() As Boolean
        Get
            Return mstrNovRET
        End Get
        Set(ByVal Value As Boolean)
            mstrNovRET = Value
        End Set

    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Novedades de la planilla de autoliquidacion de aportes 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	05/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '''    
    Public Property TDA() As Boolean
        Get
            Return mstrNovTDA
        End Get
        Set(ByVal Value As Boolean)
            mstrNovTDA = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Novedades de la planilla de autoliquidacion de aportes 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	05/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '''    
    Public Property TAA() As Boolean
        Get
            Return mstrNovTAA
        End Get
        Set(ByVal Value As Boolean)
            mstrNovTAA = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Novedades de la planilla de autoliquidacion de aportes 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	05/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '''    
    Public Property VSP() As Boolean
        Get
            Return mstrNovVSP
        End Get
        Set(ByVal Value As Boolean)
            mstrNovVSP = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Novedades de la planilla de autoliquidacion de aportes 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	05/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '''    
    Public Property VST() As Boolean
        Get
            Return mstrNovVST
        End Get
        Set(ByVal Value As Boolean)
            mstrNovVST = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Novedades de la planilla de autoliquidacion de aportes 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	05/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '''    
    Public Property SLN() As Boolean
        Get
            Return mstrNovSLN
        End Get
        Set(ByVal Value As Boolean)
            mstrNovSLN = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Novedades de la planilla de autoliquidacion de aportes 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	05/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '''    
    Public Property IGE() As Boolean
        Get
            Return mstrNovIGE
        End Get
        Set(ByVal Value As Boolean)
            mstrNovIGE = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Novedades de la planilla de autoliquidacion de aportes 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	05/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '''    
    Public Property LMA() As Boolean
        Get
            Return mstrNovLMA
        End Get
        Set(ByVal Value As Boolean)
            mstrNovLMA = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Novedades de la planilla de autoliquidacion de aportes 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENSALBAM]	05/11/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '''    
    Public Property VAC() As Boolean
        Get
            Return mstrNovVAC
        End Get
        Set(ByVal Value As Boolean)
            mstrNovVAC = Value
        End Set
    End Property

#End Region

#Region " Constructores "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor por defecto. Se encarga de inicializar el lenguage de la entidad 
    ''' a es-CO
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
    End Sub
#End Region

End Class

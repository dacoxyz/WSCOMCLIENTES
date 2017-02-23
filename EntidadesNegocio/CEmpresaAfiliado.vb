#Region "Namespaces"
Imports System.Globalization
Imports System.Xml.Serialization
Imports System.Xml
Imports System.Threading
#End Region
''' -----------------------------------------------------------------------------
''' Project	 : BusinessEntities
''' Class	 : EmpresaAfiliado
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Esta clase modela una empresa a la cual está o estuvo afiliado 
''' un trabajador. 
''' Extiende de la clase CEmpresa los atributos básicos, pero esta 
''' se hace necesaria para diferenciar una empresa como entidad de 
''' negocio dentro del sistema de una empresa con la cual esta relacionado
''' un trabajador
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	8/10/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
<Serializable()> Public Class CEmpresaAfiliado
    Inherits CEmpresa

#Region " Propiedades "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha de retiro del trabajador de esta empresa, en caso de estarlo.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechaRetiro() As String
        Get
            If mdtmFechaRetiro = mdtmFechaRetiro.MinValue Then
                Return ""
            Else
                Return mdtmFechaRetiro.ToString(mFormatter.ShortDatePattern)
            End If
        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mdtmFechaRetiro = Convert.ToDateTime(Value, mFormatter)
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha de afiliación a la EPS, de un trabajador empleado por la empresa que 
    ''' modelada por esta entidad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	10/14/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechaIngresoPOS() As String
        Get
            If mdtmFechaIngresoPOS = mdtmFechaIngresoPOS.MinValue Then
                Return ""
            Else
                Return mdtmFechaIngresoPOS.ToString(mFormatter.ShortDatePattern)
            End If
        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mdtmFechaIngresoPOS = Convert.ToDateTime(Value, mFormatter)
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''  Fecha de ingreso al Plan Complementario, de un trabajador empleado por la 
    ''' empresa que se modela en esta entidad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	10/14/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechaIngresoPC() As String
        Get
            If mdtmFechaIngresoPC = mdtmFechaIngresoPC.MinValue Then
                Return ""
            Else
                Return mdtmFechaIngresoPC.ToString(mFormatter.ShortDatePattern)
            End If
        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mdtmFechaIngresoPC = Convert.ToDateTime(Value, mFormatter)
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha en que se afilió a caja de compensación familiar, un trabajador empleado
    ''' por la empresa que modela esta entidad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	10/14/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechaAfiliacionCAJA() As String
        Get
            If mdtmFechaAfiliacionCaja = mdtmFechaAfiliacionCaja.MinValue Then
                Return ""
            Else
                Return mdtmFechaAfiliacionCaja.ToString(mFormatter.ShortDatePattern)
            End If
        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mdtmFechaAfiliacionCaja = Convert.ToDateTime(Value, mFormatter)
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Centro de costo asociado con la empresa modelada por esta entidad.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property CentroDeCostos() As String
        Get
            Return mstrCentroDeCostos
        End Get
        Set(ByVal Value As String)
            mstrCentroDeCostos = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fecha de ingreso del trabajador a la empresa 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property FechaIngreso() As String
        Get
            If mdtmFechaIngreso = mdtmFechaIngreso.MinValue Then
                Return ""
            Else
                Return mdtmFechaIngreso.ToString(mFormatter.ShortDatePattern)
            End If
        End Get
        Set(ByVal Value As String)
            If Not Value = "" Then
                mdtmFechaIngreso = Convert.ToDateTime(Value, mFormatter)
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Cargo que desempeña el trabajador en la empresa.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property CargoTrabajador() As String
        Get
            Return mstrCargo
        End Get
        Set(ByVal Value As String)
            mstrCargo = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Salario básico del trabajador para transacciones de Caja de compensación familiar 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property SalarioCaja() As Double
        Get
            Return mdblSalarioCaja
        End Get
        Set(ByVal Value As Double)
            mdblSalarioCaja = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Salario que se registra para afiliaciones a POS 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	23/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property SalarioPOS() As Double
        Get
            Return mdblSalarioPOS
        End Get
        Set(ByVal Value As Double)
            mdblSalarioPOS = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Horas laboradas al mes para la empresa por el trabajador
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property HorasLaborMes() As Short
        Get
            Return mintHorasLaborMes
        End Get
        Set(ByVal Value As Short)
            mintHorasLaborMes = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Almacena el estado de afiliaciòn de un individuo a Caja de Compensaciòn
    ''' Familiar
    ''' </summary>
    ''' <value>
    ''' 0 -> Afiliado
    ''' 1 -> Retirado
    ''' 3 -> No Afiliado
    '''  </value>
    '''  <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property EstadoAfiliacionCaja() As Short
        Get
            Return mshrEstadoAfiliacionCaja
        End Get
        Set(ByVal Value As Short)
            mshrEstadoAfiliacionCaja = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Almacena el estado de afiliaciòn de un individuo a Plan Obligatorio de 
    ''' Salud
    ''' </summary>
    ''' <value>
    ''' 0      = El individuo se encuentra afiliado
    ''' 1      = El individuo se encuentra retirado
    ''' 3      = El individuo no se encuentra afiliado 
    ''' 4      = El individuo se encuentra carente 
    ''' 5      = El individuo se encuentra suspendido
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property EstadoAfiliacionPOS() As Short
        Get
            Return mshrEstadoAfiliacionPOS
        End Get
        Set(ByVal Value As Short)
            mshrEstadoAfiliacionPOS = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Almacena el estado de afiliaciòn de un individuo a Plan Complementario de 
    ''' Salud
    ''' </summary>
    ''' <value>
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property EstadoAfiliacionPC() As Short
        Get
            Return mshrEstadoAfiliacionPC
        End Get
        Set(ByVal Value As Short)
            mshrEstadoAfiliacionPC = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Almacena el estado de multiafiliacion de un trabajador en POS</summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	4/25/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property EstadoMultiafiliacionPOS() As Short
        Get
            Return mshrEstadoMultiafiliacionPOS
        End Get
        Set(ByVal Value As Short)
            mshrEstadoMultiafiliacionPOS = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de vínculo que existe entre la empresa y el trabajador
    ''' </summary>
    ''' <value>1-> Dependiente</value>
    ''' <remarks>
    '''  A la version 1.0.1 de Transar solo se maneja el tipo 1 para el módulo de afiliaciones.
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TipoVinculo() As Short
        Get
            Return mshrTipoVinculo
        End Get
        Set(ByVal Value As Short)
            mshrTipoVinculo = Value
        End Set
    End Property
#End Region

#Region " Miembros Privados "
    Private mshrTipoVinculo As Short
    Private mdtmFechaRetiro As Date
    Private mstrCentroDeCostos As String
    Private mstrCargo As String
    Private mdblSalarioPOS As Double
    Private mdblSalarioCaja As Double
    Private mdtmFechaIngreso As Date
    Private mintHorasLaborMes As Short
    Private mshrEstadoAfiliacionCaja As Short
    Private mshrEstadoAfiliacionPOS As Short
    Private mshrEstadoAfiliacionPC As Short
    Private mdtmFechaAfiliacionCaja As Date
    Private mdtmFechaIngresoPOS As Date
    Private mdtmFechaIngresoPC As Date
    Private mshrEstadoMultiafiliacionPOS As Short

    Private mFormatter As DateTimeFormatInfo

#End Region

#Region "Constructores"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor por defecto de la clase. Se encarga de inicializar el lenguage de 
    ''' la entidad a es-CO
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

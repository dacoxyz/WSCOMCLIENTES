''' -----------------------------------------------------------------------------
''' Project	 : BusinessEntities
''' Class	 : CAfiliacionPOS
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Esta clase modela los datos de una afiliacion POS. Contiene la información 
''' transaccional resultado de hacer una afiliación de un cotizante POS en los 
''' sistemas internos de Compensar
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCLOPEZB]	8/19/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
<Serializable()> Public Class CAfiliacionPOS

#Region " Propiedades "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Indicador de si la afiliacion tiene novedad de ingreso o no
    ''' </summary>
    ''' <value>
    ''' True   = la afiliación tiene novedad de ingreso
    ''' False  = la afiliación no tiene novedad de ingreso
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/19/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property novedadIngreso() As Boolean
        Get
            Return mblnNovedadIngreso
        End Get
        Set(ByVal Value As Boolean)
            mblnNovedadIngreso = Value
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
    ''' Comando utilizado en el envio de la afiliación a el ISPEC correspondiente
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
#End Region

#Region "Miembros Privados "
    Private mblnNovedadIngreso As Boolean
    Private mblnNovedadTDA As Boolean
    Private mstrComando As String
#End Region

End Class
' END CLASS DEFINITION CAfiliacionPOS



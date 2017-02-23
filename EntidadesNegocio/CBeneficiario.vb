''' -----------------------------------------------------------------------------
''' Project	 : BusinessEntities
''' Class	 : CBeneficiario
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' Esta clase, que hereda de CDatosPersona, tiene los datos básicos de un 
''' beneficiario y adicionalmente los datos específicos de un usuario de este 
''' tipo. 
''' Es una clase segura. Se validan tipos de datos y contenido de los mismos.
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	8/10/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
<Serializable()> Public Class CBeneficiario
    Inherits CDatosPersona

#Region " Datos Trabajador Asociado "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Número de identificación del trabajador por medio del cual está afiliado 
    ''' el beneficiario.
    ''' Se verifica tamaño del campo. No mayor a 12 dígitos. De lo contrario se 
    ''' genera una excepción 
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
    ''' Tipo de Identificación del trabajador por medio del cual está afiliado 
    ''' el beneficiario.
    ''' Se verifican tamaños y tipos posibles.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TipoId() As String
        Get
            Return mstrTipoId
        End Get
        Set(ByVal Value As String)
            If Value.Length > 4 Then
                Throw New ApplicationException("Tipo de Identificación debe tener máximo 3 caracteres")
            Else
                mstrTipoId = Value
            End If
        End Set
    End Property
#End Region

#Region "Datos Beneficiario"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Parentezco del beneficiario con el trabajador por medio del cual se afilia.
    ''' Se verifican tipos de datos y contenidos.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Parentezco() As String
        Get
            Return mstrParentezco
        End Get
        Set(ByVal Value As String)
            If Value.Length > 10 Then
                Throw New ApplicationException("Parentezco debe tener máximo 10 caracteres")
            Else
                mstrParentezco = Value
            End If

        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Número de identificación del beneficiario
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property NumeroIdentificacion() As String
        Get
            Return mstrNumeroIdentificacion
        End Get
        Set(ByVal Value As String)
            If Value.Length > 50 Then
                Throw New ApplicationException("La identificación debe tener menos de 50 caracteres")
            Else
                mstrNumeroIdentificacion = Value
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de Identificación del Beneficiario
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
            Return mstrTipoIdentificacion
        End Get
        Set(ByVal Value As String)
            If (Value.Length > 4) Then
                Throw New ApplicationException("Tipo de Identificación debe tener máximo 3 caracteres")
            Else
                mstrTipoIdentificacion = Value
            End If

        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Número de identificación auxiliar del beneficiario. Solicitado por el ISPEC
    ''' de afiliación de beneficiario del sistema interno de Compensar.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/24/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property NideBen() As String
        Get
            Return mstrNideBen
        End Get
        Set(ByVal Value As String)
            If Not Value Is Nothing Then
                If Value.Length > 50 Then
                    Throw New ApplicationException("El NideBen debe tener menos de 50 caracteres")
                Else
                    If Value = Me.FechaNacimiento Then
                        mstrNideBen = Convert.ToDateTime(Value, Me.mFormatter).ToString("yyyyMMdd")
                    Else
                        mstrNideBen = Value
                    End If
                End If
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de identificación auxiliar del beneficiario. Solicitado por el ISPEC
    ''' de afiliación de beneficiario del sistema interno de Compensar.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/24/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TideBen() As String
        Get
            Return mstrTideBen
        End Get
        Set(ByVal Value As String)
            If Not Value Is Nothing Then
                If (Value.Length > 4) Then
                    Throw New ApplicationException("El TideBen debe tener máximo 3 caracteres")
                Else
                    mstrTideBen = Value
                End If
            End If
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Parte alfanumérica del numero de identificación personal del beneficiario.
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property NUIPAlfa() As String
        Get
            Return mstrNUIPAlfa
        End Get
        Set(ByVal Value As String)
            mstrNUIPAlfa = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de discapacidad del beneficiario en caso de tener alguna.
    ''' </summary>
    ''' <value>
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property TipoDiscapacidad() As String
        Get
            Return mstrTipoIncapacidad
        End Get
        Set(ByVal Value As String)
            mstrTipoIncapacidad = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Booleano que determina si la discapacidad del beneficiario, en caso de tenerla,
    ''' es permanente.
    ''' </summary>
    ''' <value>
    ''' True  = Discapacidad Permanente 
    ''' False = Discapacidad Temporal o Ausente 
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property DiscapacidadPermanente() As Boolean
        Get
            Return mblnDiscapacidadPermanente
        End Get
        Set(ByVal Value As Boolean)
            mblnDiscapacidadPermanente = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Booleano que determina si el beneficiario tiene discapacidad
    ''' </summary>
    ''' <value>
    ''' True  = tiene Discapacidad 
    ''' False = no tiene Discapacidad 
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property Discapacidad() As Boolean
        Get
            Return mblnIndDiscapacidad
        End Get
        Set(ByVal Value As Boolean)
            mblnIndDiscapacidad = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Booleano que determina si el beneficiario recibe subsidio
    ''' </summary>
    ''' <value>
    ''' True  = si recibe
    ''' False = no recibe
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property RecibeSubsidio() As Boolean
        Get
            Return mblnRecibeSubsidio
        End Get
        Set(ByVal Value As Boolean)
            mblnRecibeSubsidio = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Nombre de la caja de compensacion a la cual esta afiliado el beneficiario
    ''' </summary>
    ''' <value>
    ''' </value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	8/11/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property NombreCaja() As String
        Get
            Return mstrNombreCaja
        End Get
        Set(ByVal Value As String)
            mstrNombreCaja = Value
        End Set
    End Property
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Almacena el estado de afiliaciòn de un individuo a Caja de Compensaciòn
    ''' Familiar
    ''' </summary>
    ''' <value>
    ''' 0      = El individuo se encuentra afiliado
    ''' 1      = El individuo se encuentra retirado
    ''' 3      = El individuo no se encuentra afiliado 
    ''' </value>
    ''' <remarks>
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
    ''' 0      = El individuo se encuentra afiliado
    ''' 1      = El individuo se encuentra retirado
    ''' 3      = El individuo no se encuentra afiliado 
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
    ''' Almacena el estado de multiafiliacion de un beneficiario en POS
    ''' </summary>
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
    ''' Fecha en que el beneficiario se afilió por primera vez al Plan Obligatorio de Salud 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	12/23/2004	Created
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
    ''' Fecha de ingreso del beneficiario al Plan Complementario
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
    ''' Fecha en que se afilió el beneficiario a Caja de Compensación Familiar, en caso
    ''' de estar afiliado.
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
    ''' Nombre del programa de Plan Complementario al cual esta afiliado el beneficiario
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	1/16/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property ProgramaPC() As String
        Get
            Return mstrProgramaPC
        End Get
        Set(ByVal Value As String)
            mstrProgramaPC = Value
        End Set
    End Property


#End Region

#Region " Miembros Privados"
    Private mstrIdTrabajador As String
    Private mstrTipoId As String
    Private mstrParentezco As String
    Private mstrNumeroIdentificacion As String
    Private mstrTipoIdentificacion As String
    Private mstrNideBen As String
    Private mstrTideBen As String
    Private mstrNUIPAlfa As String
    Private mstrTipoIncapacidad As String
    Private mblnDiscapacidadPermanente As Boolean
    Private mblnIndDiscapacidad As Boolean
    Private mblnRecibeSubsidio As Boolean
    Private mstrNombreCaja As String
    Private mshrEstadoAfiliacionCaja As Short
    Private mshrEstadoAfiliacionPOS As Short
    Private mshrEstadoAfiliacionPC As Short
    Private mshrEstadoMultiafiliacionPOS As Short
    Private mdtmFechaAfiliacionCaja As Date
    Private mdtmFechaIngresoPOS As Date
    Private mdtmFechaIngresoPC As Date
    Private mstrProgramaPC As String


#End Region

#Region " Funciones "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Este método sirve para determinar si una entidad de este tipo es vacia o no.
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Se define como vacía una entidad del tipo Cbeneficiario si la super clase de 
    ''' la que ha heredado (CDatosPersona) es vacía y adicionalmente las propiedades 
    ''' IdTrabajador y TipoID son vacías.
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	15/09/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Vacio() As Boolean
        If MyBase.EsVacio And (Me.IdTrabajador Is Nothing Or Me.TipoId Is Nothing) Then
            Return True
        Else : Return False
        End If

    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Asigna los valores de la identificacion auxiliar del beneficiario(NDocBen,TDocBen)
    ''' a las propiedades correspondientes con base en el tipo, numero de identificacion,
    ''' y parentezco de este. Tiene en cuenta el tipo de afiliacion del beneficiario.
    ''' </summary>
    ''' <param name="shrTipoAfiliacion">
    ''' Beneficiario afiliado a Caja = 0
    ''' Beneficiario afiliado a POS = 1
    ''' </param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	9/23/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub AsignarIdentificacionAuxiliar(ByVal shrTipoAfiliacion As Short)
        Dim strTideBen As String
        Dim strNideBen As String
        Dim strTdocBen As String
        Dim strNdocBen As String

        If shrTipoAfiliacion <> 0 And shrTipoAfiliacion <> 1 Then
            Throw New ApplicationException("El tipo de afiliacion debe ser 0(Caja) ó 1(POS)")
        Else
            strTideBen = Me.TipoIdentificacion
            strNideBen = Me.NumeroIdentificacion
            strTdocBen = Me.TipoIdentificacion
            strNdocBen = Me.NumeroIdentificacion

            If shrTipoAfiliacion = 1 Then 'Afiliado a POS
                'Caso CC, TI, CE
                If (Me.TipoIdentificacion = "1" Or Me.TipoIdentificacion = "3" Or Me.TipoIdentificacion = "4") And Me.Parentezco = "HI" Then
                    strTideBen = "6"
                    strNideBen = Convert.ToDateTime(Me.FechaNacimiento, mFormatter).ToString("yyyyMMdd")
                End If

                'Caso Registro Civil y Numero unico de menores de edad
                If Me.TipoIdentificacion = "7" Or (Me.TipoIdentificacion = "8" And Me.Edad < 18) Then
                    strTideBen = "6"
                    strNideBen = Convert.ToDateTime(Me.FechaNacimiento, mFormatter).ToString("yyyyMMdd")
                    strTdocBen = Me.TipoIdentificacion
                    strNdocBen = Me.NumeroIdentificacion
                End If

            ElseIf shrTipoAfiliacion = 0 Then 'Afiliado a Caja
                'Caso CC, TI, CE
                If (Me.TipoIdentificacion = "1" Or Me.TipoIdentificacion = "3" Or Me.TipoIdentificacion = "4") And Me.Parentezco = "HI" Then
                    strTideBen = "6"
                    strNideBen = Convert.ToDateTime(Me.FechaNacimiento, mFormatter).ToString("yyyyMMdd")
                End If

                'Caso Registro Civil y Numero unico
                If Me.TipoIdentificacion = "7" Or Me.TipoIdentificacion = "8" Then
                    strTideBen = "6"
                    strNideBen = Convert.ToDateTime(Me.FechaNacimiento, mFormatter).ToString("yyyyMMdd")
                    strTdocBen = Me.TipoIdentificacion
                    strNdocBen = Me.NumeroIdentificacion
                End If
            End If

            'Asigna valores a las propiedades del objeto
            Me.TipoIdentificacion = strTideBen
            Me.NumeroIdentificacion = strNideBen
            Me.TideBen = strTdocBen
            Me.NideBen = strNdocBen
        End If

    End Sub
#End Region

    Public Sub New()

    End Sub
End Class
' END CLASS DEFINITION CBeneficiario



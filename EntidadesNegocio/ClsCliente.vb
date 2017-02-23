Imports Microsoft.VisualBasic

Public Class ClsCliente
    Inherits ClsDireccionCliente

#Region "Declaraciones"
    Dim m_TipoIdent As String
    Dim m_Identity As String
    Dim m_NroIdentificacion As String
    Dim m_Partealfabetica As String
    Dim m_DigitoChequeo As String
    Dim m_sucursal As String
    Dim m_CentroCosto As String
    Dim m_Nombre As String
    Dim m_PrimerApellido As String
    Dim m_SegundoApellido As String
    Dim m_Fechanacimiento As String
    Dim m_Genero As String
    Dim m_Estadocivil As String
    Dim m_Detalleadicional As String
    Dim m_Razonsocial As String
    Dim m_TipoIdentEmpr As String
    Dim m_NroIdentEmpr As String
#End Region

#Region "Propiedades"
    Property TipoIdent() As String
        Get
            Return m_TipoIdent
        End Get
        Set(ByVal value As String)
            m_TipoIdent = value
        End Set
    End Property

    Property Identity() As String
        Get
            Return m_Identity
        End Get
        Set(ByVal value As String)
            m_Identity = value
        End Set
    End Property

    Property NroIdentificacion() As String
        Get
            Return m_NroIdentificacion
        End Get
        Set(ByVal value As String)
            m_NroIdentificacion = value
        End Set
    End Property

    Property Partealfabetica() As String
        Get
            Return m_Partealfabetica
        End Get
        Set(ByVal value As String)
            m_Partealfabetica = value
        End Set
    End Property

    Property DigitoChequeo() As String
        Get
            Return m_DigitoChequeo
        End Get
        Set(ByVal value As String)
            m_DigitoChequeo = value
        End Set
    End Property

    Property Sucursal() As String
        Get
            Return m_sucursal
        End Get
        Set(ByVal value As String)
            m_sucursal = value
        End Set
    End Property

    Property CentroCosto() As String
        Get
            Return m_CentroCosto
        End Get
        Set(ByVal value As String)
            m_CentroCosto = value
        End Set
    End Property

    Property Nombre() As String
        Get
            Return m_Nombre
        End Get
        Set(ByVal value As String)
            m_Nombre = value
        End Set
    End Property

    Property PrimerApellido() As String
        Get
            Return m_PrimerApellido
        End Get
        Set(ByVal value As String)
            m_PrimerApellido = value
        End Set
    End Property

    Property SegundoApellido() As String
        Get
            Return m_SegundoApellido
        End Get
        Set(ByVal value As String)
            m_SegundoApellido = value
        End Set
    End Property

    Property Fechanacimiento() As String
        Get
            Return m_Fechanacimiento
        End Get
        Set(ByVal value As String)
            m_Fechanacimiento = value
        End Set
    End Property

    Property Genero() As String
        Get
            Return m_Genero
        End Get
        Set(ByVal value As String)
            m_Genero = value
        End Set
    End Property

    Property EstadoCivil() As String
        Get
            Return m_Estadocivil
        End Get
        Set(ByVal value As String)
            m_Estadocivil = value
        End Set
    End Property
    Property Detalleadicional() As String
        Get
            Return m_Detalleadicional
        End Get
        Set(ByVal value As String)
            m_Detalleadicional = value
        End Set
    End Property

    Property Razonsocial() As String
        Get
            Return m_Razonsocial
        End Get
        Set(ByVal value As String)
            m_Razonsocial = value
        End Set
    End Property

    Property TipoIdentEmpr() As String
        Get
            Return m_TipoIdentEmpr
        End Get
        Set(ByVal value As String)
            m_TipoIdentEmpr = value
        End Set
    End Property

    Property NroIdentEmpr() As String
        Get
            Return m_NroIdentEmpr
        End Get
        Set(ByVal value As String)
            m_NroIdentEmpr = value
        End Set
    End Property

#End Region

End Class

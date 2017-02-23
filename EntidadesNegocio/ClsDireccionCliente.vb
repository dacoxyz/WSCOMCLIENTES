Imports Microsoft.VisualBasic

Public Class ClsDireccionCliente
    Inherits ClsRequisitos

#Region "Declaraciones"
    Dim m_Direccion As String
    Dim m_Telefono As String
    Dim m_Extension As String
    Dim m_TipoDireccion As String
    Dim m_Barrio As String
    Dim m_Zona As String
    Dim m_Ciudad As String
#End Region

#Region "Propiedades"
    Property Direccion() As String
        Get
            Return m_Direccion
        End Get
        Set(ByVal value As String)
            m_Direccion = value
        End Set
    End Property

    Property Telefono() As String
        Get
            Return m_Telefono
        End Get
        Set(ByVal value As String)
            m_Telefono = value
        End Set
    End Property

    Property Extension() As String
        Get
            Return m_Extension
        End Get
        Set(ByVal value As String)
            m_Extension = value
        End Set
    End Property

    Property TipoDireccion() As String
        Get
            Return m_TipoDireccion
        End Get
        Set(ByVal value As String)
            m_TipoDireccion = value
        End Set
    End Property

    Property Barrio() As String
        Get
            Return m_Barrio
        End Get
        Set(ByVal value As String)
            m_Barrio = value
        End Set
    End Property

    Property Zona() As String
        Get
            Return m_Zona
        End Get
        Set(ByVal value As String)
            m_Zona = value
        End Set
    End Property

    Property Ciudad() As String
        Get
            Return m_Ciudad
        End Get
        Set(ByVal value As String)
            m_Ciudad = value
        End Set
    End Property
#End Region

End Class

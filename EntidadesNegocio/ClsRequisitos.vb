Imports Microsoft.VisualBasic

Public Class ClsRequisitos
    Inherits ClsDatosAdicionales

#Region "Declaraciones"
    Dim m_AccReq(12) As String
    Dim m_NameReq(12) As String
    Dim m_FechIniReq(12) As String
    Dim m_FechFinReq(12) As String
    Dim m_DatNumReq(12) As String
    Dim m_DatAlfReq(12) As String
    Dim m_InfoAdicional As String
#End Region

#Region "Propiedades"
    Property AccReq() As String()
        Get
            Return m_AccReq
        End Get
        Set(ByVal value As String())
            m_AccReq = value
        End Set
    End Property

    Property NameReq() As String()
        Get
            Return m_NameReq
        End Get
        Set(ByVal value As String())
            m_NameReq = value
        End Set
    End Property

    Property FechIniReq() As String()
        Get
            Return m_FechIniReq
        End Get
        Set(ByVal value As String())
            m_FechIniReq = value
        End Set
    End Property

    Property FechFinReq() As String()
        Get
            Return m_FechFinReq
        End Get
        Set(ByVal value As String())
            m_FechFinReq = value
        End Set
    End Property

    Property DatNumReq() As String()
        Get
            Return m_DatNumReq
        End Get
        Set(ByVal value As String())
            m_DatNumReq = value
        End Set
    End Property

    Property DatAlfReq() As String()
        Get
            Return m_DatAlfReq
        End Get
        Set(ByVal value As String())
            m_DatAlfReq = value
        End Set
    End Property

    Property InfoAdicional() As String
        Get
            Return m_InfoAdicional
        End Get
        Set(ByVal value As String)
            m_InfoAdicional = value
        End Set
    End Property

#End Region

End Class

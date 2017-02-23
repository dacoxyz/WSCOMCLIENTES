Imports Microsoft.VisualBasic
Imports System.data

Public Class ClsDatosAdicionales


#Region "Declaraciones"
    Dim m_TipoIdentificacion1 As String
    Dim m_Responsable1 As String
    Dim m_TipoIdentificacion2 As String
    Dim m_Responsable2 As String
    Dim m_Responsable3 As String
    Dim m_Responsable4 As String
    Dim m_FechaInicioAfiliacion As String
    Dim m_FechaFinalAfiliacion As String
    Dim m_FechaRetiro As String
    Dim m_CausalRetiro As String
    Dim m_FechaIngresoEmpresa As String
    Dim m_FechaPrimerAporte As String
    Dim m_Valor As String
    Dim m_Cantidad As String
    Dim m_Cargo As String
    Dim m_NumeroDireccion As String
    Dim m_Estado As String
    Dim m_escala As String
    'wlm: sep 9 /09  control de cambios:unificacion cambio de estado y radicacion.
    Dim m_IndOption As String
#End Region

#Region "Propiedades"
    'wlm: sep 9 /09  control de cambios:unificacion cambio de estado y radicacion.
    Property IndOption() As String
        Get
            Return m_IndOption
        End Get
        Set(ByVal value As String)
            m_IndOption = value

        End Set
    End Property

    Property TipoIdentificacion1() As String
        Get
            Return m_TipoIdentificacion1
        End Get
        Set(ByVal value As String)
            m_TipoIdentificacion1 = value
        End Set
    End Property

    Property Responsable1() As String
        Get
            Return m_Responsable1
        End Get
        Set(ByVal value As String)
            m_Responsable1 = value
        End Set
    End Property

    Property TipoIdentificacion2() As String
        Get
            Return m_TipoIdentificacion2
        End Get
        Set(ByVal value As String)
            m_TipoIdentificacion2 = value
        End Set
    End Property

    Property Responsable2() As String
        Get
            Return m_Responsable2
        End Get
        Set(ByVal value As String)
            m_Responsable2 = value
        End Set
    End Property

    Property Responsable3() As String
        Get
            Return m_Responsable3
        End Get
        Set(ByVal value As String)
            m_Responsable3 = value
        End Set
    End Property

    Property Responsable4() As String
        Get
            Return m_Responsable4
        End Get
        Set(ByVal value As String)
            m_Responsable4 = value
        End Set
    End Property

    Property FechaInicioAfiliacion() As String
        Get
            Return m_FechaInicioAfiliacion
        End Get
        Set(ByVal value As String)
            m_FechaInicioAfiliacion = value
        End Set
    End Property

    Property FechaFinalAfiliacion() As String
        Get
            Return m_FechaFinalAfiliacion
        End Get
        Set(ByVal value As String)
            m_FechaFinalAfiliacion = value
        End Set
    End Property

    Property FechaRetiro() As String
        Get
            Return m_FechaRetiro
        End Get
        Set(ByVal value As String)
            m_FechaRetiro = value
        End Set
    End Property

    Property CausalRetiro() As String
        Get
            Return m_CausalRetiro
        End Get
        Set(ByVal value As String)
            m_CausalRetiro = value
        End Set
    End Property

    Property FechaPrimerAporte() As String
        Get
            Return m_FechaPrimerAporte
        End Get
        Set(ByVal value As String)
            m_FechaPrimerAporte = value
        End Set
    End Property

    Property FechaIngresoEmpresa() As String
        Get
            Return m_FechaIngresoEmpresa
        End Get
        Set(ByVal value As String)
            m_FechaIngresoEmpresa = value
        End Set
    End Property

    Property Valor() As String
        Get
            Return m_Valor
        End Get
        Set(ByVal value As String)
            m_Valor = value
        End Set
    End Property

    Property Cantidad() As String
        Get
            Return m_Cantidad
        End Get
        Set(ByVal value As String)
            m_Cantidad = value
        End Set
    End Property

    Property Cargo() As String
        Get
            Return m_Cargo
        End Get
        Set(ByVal value As String)
            m_Cargo = value
        End Set
    End Property

    Property NumeroDireccion() As String
        Get
            Return m_NumeroDireccion
        End Get
        Set(ByVal value As String)
            m_NumeroDireccion = value
        End Set
    End Property


    Property Estado() As String
        Get
            Return m_Estado
        End Get
        Set(ByVal value As String)
            m_Estado = value
        End Set
    End Property


    Property Escala() As String
        Get
            Return m_escala
        End Get
        Set(ByVal value As String)
            m_escala = value
        End Set
    End Property

#End Region

End Class


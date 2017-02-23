Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Microsoft.VisualBasic
Imports System.ComponentModel
Imports System.Configuration
Imports System.Data

<WebService(Namespace:="http://WSCOMCLIENTES/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class ServicioCargueMasivo
    Inherits System.Web.Services.WebService
    Dim objComAdmo As New Proceso.ClsCargueMasivo
    Dim objUtil As New Utilidades.CUtil
    Dim sProyecto As String = ConfigurationSettings.AppSettings("ProjectID")
#Region "MetodosWeb"
#Region "ConsultaParametrosSemana"
    <WebMethod(Description:="Consulta parametrización de semana para el cargue de archivo")> _
   Public Function ConsultaParametrosSemana(ByVal sApl As String, ByVal iSN_Activo As Integer) As String
        Dim ds As DataTable
        If iSN_Activo >= 1 Then
            Try
                Dim respuesta As String
                respuesta = objComAdmo.ConsultaParametrosSemana(sProyecto, iSN_Activo).DataSet.GetXml()
                Return respuesta
            Catch ex As Exception
                Return objUtil.ConvertToXMLdoc(ex.Message)
            End Try
        Else
            Try
                ds = objComAdmo.ConsultaParametrosSemana(sProyecto, iSN_Activo)
                Return ds.Rows(0).Item(0)
            Catch ex As Exception
                Return objUtil.ConvertToXMLdoc(ex.Message)
            Finally
                ds = Nothing
            End Try
        End If
    End Function
#End Region
#Region "ActualizaParametrosSemana"
    <WebMethod(Description:="Actualiza parametrización de semana para el cargue de archivo")> _
   Public Function ActualizaParametrosSemana(ByVal sApl As String, ByVal sUsuario As String, ByVal sXmlParams As String) As String
        Try
            objComAdmo.ActualizaParametrosSemana(sProyecto, sUsuario, sXmlParams)
            Return objUtil.ConvertToXMLdoc("Se actualizo los parámetros de cargue de archivo.")
        Catch ex As Exception
            Return objUtil.ConvertToXMLdoc("Error al actualizar parámetros de cargue de archivo.")
        End Try
    End Function
#End Region
#Region "ActualizaParametros"
    <WebMethod(Description:="Actualiza parametrización de semana para el cargue de archivo")> _
    Public Function ActualizaParametros(ByVal sApl As String, ByVal sUsuario As String, ByVal sXmlParams As String) As String
        Try
            objComAdmo.ActualizaParametros(sProyecto, sUsuario, sXmlParams)
            Return objUtil.ConvertToXMLdoc("Se actualizo los parámetros de cargue de archivo.")
        Catch ex As Exception
            Return objUtil.ConvertToXMLdoc("Error al actualizar parámetros de cargue de archivo.")
        End Try
    End Function
#End Region
#Region "InsertarLogPC" '11057 20160114
    <WebMethod(Description:="Inserta Log de Consultas de Plan Complementario")> _
    Public Function insertaLogConsultasPC(ByVal IDProyecto As String, ByVal sXmlParams As String) As String
        Try
            objComAdmo.insertaLogConsultasPC(IDProyecto, sXmlParams)
            Return objUtil.ConvertToXMLdoc("Se inserto el registro.")
        Catch ex As Exception
            Return objUtil.ConvertToXMLdoc("Error al insertar el registro.")
        End Try
    End Function
#End Region
#End Region
End Class
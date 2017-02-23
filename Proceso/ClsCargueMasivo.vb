Imports Compensar.SISPOS.DAL
Imports System.Data
Public Class ClsCargueMasivo
#Region "ConsultaParametrosSemana"
    Public Function ConsultaParametrosSemana(ByVal IDProyecto As String, ByVal iSN_Activo As Integer) As DataTable
        Dim objCargue As New Comadmo(IDProyecto)
        ConsultaParametrosSemana = objCargue.ConsultaParametrosSemana(iSN_Activo)
        Return ConsultaParametrosSemana
    End Function
#End Region
#Region "ActualizaParametrosSemana"
    Public Sub ActualizaParametrosSemana(ByVal IDProyecto As String, ByVal sUsuario As String, ByVal sXmlParams As String)
        Dim objCargue As New Comadmo(IDProyecto)
        objCargue.ActualizaParametrosSemana(sUsuario, sXmlParams)
    End Sub

#End Region
    Public Sub ActualizaParametros(ByVal IDProyecto As String, ByVal sUsuario As String, ByVal sOpc As String)
        Dim objCargue As New Comadmo(IDProyecto)
        objCargue.ActualizaParametrosMasivo(sOpc)
    End Sub

    Public Sub insertaLogConsultasPC(ByVal IDProyecto As String, ByVal xmlParams As String) '11057 20160114
        Dim objCargue As New Comadmo(IDProyecto)
        objCargue.insertaLogConsultasPC(xmlParams)
    End Sub
End Class

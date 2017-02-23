Imports System.Configuration
Imports System.String
Public Class ClsServicio


#Region "Declaraciones"
    Dim ObjUtil As New Utilidades.CUtil
    Dim RegistroClientes As New Servicios.AFSYS
    Dim DatosAldea As New AF
    Dim DatosCaf As New Utilidades.CAF
    Dim m_DtsInfoTran As DataSet
#End Region

#Region "Propiedades"

    Property DtsInfoTran() As DataSet
        Get
            Return m_DtsInfoTran
        End Get
        Set(ByVal value As DataSet)
            m_DtsInfoTran = value
        End Set
    End Property

#End Region

#Region "Metodos"

    Public Function EjecutaTransaccion(ByVal m_Aplicacion As String) As String
        Dim strResultado As String = ""
        Try
            Dim strBeforeAfter As String = ""
            If ConfigurationManager.AppSettings("Activalog") = "S" Then
                strBeforeAfter = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")
                'ObjUtil.WriteLogEvent("Aplicacion : " & m_Aplicacion & " Envio : " & m_DtsInfoTran.GetXml)
            End If
            Try '1710 20130503 se elimina del dataset el nuevo campo 
                m_DtsInfoTran.Tables(0).Columns.Remove("C_ESPECIAL")
            Catch ex As Exception

            End Try
            strResultado = RegistroClientes.Ispec(m_Aplicacion, "CLE15", m_DtsInfoTran.GetXml, "1") '.Replace("&", "&amp;")
            If ConfigurationManager.AppSettings("Activalog") = "S" Then
                strBeforeAfter = strBeforeAfter & ";" & DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")
                ObjUtil.WriteLogEvent("Aplicacion : " & m_Aplicacion & " Respuesta : " & strResultado & " Tiempo : " & strBeforeAfter)
            End If
        Catch ex As Exception
            Dim MsgError As String = ObjUtil.ConvertToXMLdoc(ex.Message)
            Return MsgError
            ObjUtil.WriteLogEvent("Aplicacion : " & m_Aplicacion & " Excepcion : " & MsgError)
        End Try
        Return strResultado
    End Function

    'Se crea metodo para ejecutar los metodos de aldea extiscurrea 201104....
    Public Function ejecutaAldea(ByVal m_Aplicacion As String, ByVal TipoId As String, ByVal NumeroId As String, ByVal ParteAlfabetica As String, _
    ByVal Sucursal As String, ByVal centroCosto As String, ByVal Condicion As String, ByVal Programa As String, _
        Optional ByVal Ndir As String = "1") As String '4249 20140505
        Dim returnvalue As String = ""
        Dim Xml As String = "", pKeyCache As String
        If TipoId = "NI" Then
            Dim strParams As String
            strParams = "<Afiliado><Identificacion>" & NumeroId & "</Identificacion><TipoIdentificacion>" & _
                        ObjUtil.GetEquivTipoDoc(TipoId) & "</TipoIdentificacion><AlfaId>" & ParteAlfabetica & _
                        "</AlfaId><Sucursal>" & IIf(Sucursal <> "", Sucursal, "0") & "</Sucursal><CentroCosto>" & _
                        IIf(centroCosto <> "", centroCosto, "0") & "</CentroCosto></Afiliado>" '4249 20140505 <Direccion>" & Ndir & "</Direccion>


            Dim oCache As New Compensar.SISPOS.DAL.Compensar.SISPOS.DAL.clsBase(m_Aplicacion)
            pKeyCache = NumeroId + ObjUtil.GetEquivTipoDoc(TipoId) + ParteAlfabetica + IIf(centroCosto <> "", centroCosto, "0") + IIf(Sucursal <> "", Sucursal, "0") 'nidecli + tipo_id_cotizante + xidecli + dependencia + Sucursal
            If oCache.InquiryCacheByKey("ConsultaNautcliSegunIdentificacion" + pKeyCache) Then
                Xml = CType(oCache.GetObjectFromCache("ConsultaNautcliSegunIdentificacion" + pKeyCache), String)
            Else
                Xml = DatosAldea.Afiliado(ConfigurationManager.AppSettings("ProjectID"), strParams, 25)
                oCache.AddToCache("ConsultaNautcliSegunIdentificacion" + pKeyCache, Xml, DateInterval.Minute, 3)
            End If

            returnvalue = Xml
        Else
            If Not IsNullOrEmpty(Programa) Then
                Xml = DatosAldea.Afiliado(ConfigurationManager.AppSettings("ProjectID"), "<Afiliado><Identificacion>" & NumeroId & _
                                          "</Identificacion><TipoIdentificacion>" & ObjUtil.GetEquivTipoDoc(TipoId) & _
                                          "</TipoIdentificacion><AlfaId>" & ParteAlfabetica & "</AlfaId> <NCONAFI>" & _
                                           IIf(Condicion <> "", Condicion, "0") & "</NCONAFI><Programa>" & Programa & "</Programa><Sucursal>" & _
                                          IIf(Sucursal <> "", Sucursal, "0") & "</Sucursal><CentroDeCosto>" & _
                                          IIf(centroCosto <> "", centroCosto, "0") & "</CentroDeCosto><Direccion>" & _
                                          Ndir & "</Direccion></Afiliado>", 5) '4249 20140505 TODO 20140505 verificar el reemplazo de esta consulta

                returnvalue = Xml
            Else
                Xml = DatosCaf.Afiliado(ConfigurationManager.AppSettings("ProjectID"), ParteAlfabetica + NumeroId, ObjUtil.GetEquivTipoDoc(TipoId), Ndir, 22) '4249
                returnvalue = Xml
            End If
        End If
        Return returnvalue
    End Function

#End Region

End Class

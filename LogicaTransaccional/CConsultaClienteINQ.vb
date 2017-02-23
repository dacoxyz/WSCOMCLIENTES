Imports System.Configuration
Imports System.String
Public Class CConsultaClienteINQ

#Region "Declaraciones"
    Dim cle15 As New ObjTransferDatos.DtsCLE15
    Dim ObjUtil As New Utilidades.CUtil
    Dim DatosAldea As New Servicios.AF
    Dim DatosCaf As New Utilidades.CAF
#End Region


    'Se crea metodo para ejecutar los metodos de aldea extiscurrea 201104....
    Public Function ejecutaAldea(ByVal m_Aplicacion As String, ByVal TipoId As String, ByVal NumeroId As String, ByVal ParteAlfabetica As String, _
    ByVal Sucursal As String, ByVal centroCosto As String, ByVal Condicion As String, ByVal Programa As String) As String
        Dim returnvalue As String = ""
        Dim Xml As String = "", pKeyCache As String
        If TipoId = "NI" Then
            'Xml = DatosAldea.Afiliado(ConfigurationManager.AppSettings("ProjectID"), "<Afiliado><Identificacion>" & NumeroId & "</Identificacion><TipoIdentificacion>" & ObjUtil.GetEquivTipoDoc(TipoId) & "</TipoIdentificacion><AlfaId>" & ParteAlfabetica & "</AlfaId><Sucursal>" & IIf(Sucursal <> "", Sucursal, "0") & "</Sucursal><CentroCosto>" & IIf(centroCosto <> "", centroCosto, "0") & "</CentroCosto></Afiliado>", 25)
            Dim strParams As String
            strParams = "<Afiliado><Identificacion>" & NumeroId & "</Identificacion><TipoIdentificacion>" & ObjUtil.GetEquivTipoDoc(TipoId) & "</TipoIdentificacion><AlfaId>" & ParteAlfabetica & "</AlfaId><Sucursal>" & IIf(Sucursal <> "", Sucursal, "0") & "</Sucursal><CentroCosto>" & IIf(centroCosto <> "", centroCosto, "0") & "</CentroCosto></Afiliado>"
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
                Xml = DatosAldea.Afiliado(ConfigurationManager.AppSettings("ProjectID"), "<Afiliado><Identificacion>" & NumeroId & "</Identificacion><TipoIdentificacion>" & ObjUtil.GetEquivTipoDoc(TipoId) & "</TipoIdentificacion><AlfaId>" & ParteAlfabetica & "</AlfaId> <NCONAFI>" & Condicion & "</NCONAFI><Programa>" & Programa & "</Programa><Sucursal>" & IIf(Sucursal <> "", Sucursal, "0") & "</Sucursal><CentroDeCosto>" & IIf(centroCosto <> "", centroCosto, "0") & "</CentroDeCosto></Afiliado>", 5) '2261

                returnvalue = Xml
            Else
                Xml = DatosCaf.Afiliado(ConfigurationManager.AppSettings("ProjectID"), ParteAlfabetica + NumeroId, ObjUtil.GetEquivTipoDoc(TipoId), "", 22)
                returnvalue = Xml
            End If
        End If
        Return returnvalue
    End Function

    Public Function ConsultarCliente(ByVal objDatosAfiliacion As EntidadesNegocio.CDatosPersona, ByVal tipo As String) As String
        Dim m_Usuario As String = ""
        Dim m_Aplicacion As String = ""
        Dim TipoIdent As String = ""
        Dim NroIdentificacion As String = ""
        Dim Partealfabetica As String = ""
        Dim Sucursal As String = ""
        Dim CentroCosto As String = ""
        Dim m_Condicion As String = ""
        Dim m_Programa As String = ""
        Dim objBeneficiario As EntidadesNegocio.CBeneficiario
        Dim objTrabajador As EntidadesNegocio.CTrabajador
        Dim ManejaTrans As New ManejadorTransacciones
        If tipo = "1" Then
            objTrabajador = objDatosAfiliacion
            TipoIdent = ManejaTrans.darNombreTipoId(objTrabajador.TipoIdentificacion)    ''           Tipo de identificación del cliente en formato alfa CC, NI, TI, CE, PA, RC, UN, MS             A_2                   "
            NroIdentificacion = objTrabajador.IdTrabajador       ''           Número de identificación del cliente                                                                               N_12
        Else
            objBeneficiario = objDatosAfiliacion
            TipoIdent = ManejaTrans.darNombreTipoId(objBeneficiario.TideBen)    ''           Tipo de identificación del cliente en formato alfa CC, NI, TI, CE, PA, RC, UN, MS             A_2                   "
            NroIdentificacion = objBeneficiario.NideBen       ''           Número de identificación del cliente                                                                               N_12
            Partealfabetica = objBeneficiario.NUIPAlfa     ''            Nuip o parte alfabética de la identificación del cliente                                                      A_3
        End If
        Try
            Dim i As Integer
            Dim returnvalue As String = ""
            returnvalue = ejecutaAldea(m_Aplicacion, TipoIdent.ToUpper, NroIdentificacion, Partealfabetica, ObjUtil.strAsignarValor(Sucursal, "0"), ObjUtil.strAsignarValor(CentroCosto, "0"), ObjUtil.strAsignarValor(m_Condicion, "0"), m_Programa)
            Dim ds As DataSet = ObjUtil.GetDataSet(returnvalue)
            Dim dr As Data.DataRow
            dr = cle15.Tables(0).NewRow
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 And ds.Tables(0).TableName = "Table" Then
                    With cle15.Tables(0)
                        dr("_TOP_LINE_") = "CLE15T" & DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")
                        dr("I_MAINT") = "INQ"
                        dr("I_CALT") = m_Usuario
                        dr("I_XIDECLI") = TipoIdent.ToUpper
                        If ds.Tables(0).Columns.Contains("NAUTCLI") Then
                            dr("I_NAUTCLI") = ds.Tables(0).Rows(0)("NAUTCLI").ToString()
                        End If
                        If ds.Tables(0).Columns.Contains("NIDECLI") Then
                            dr("I_NIDECLI") = ds.Tables(0).Rows(0)("NIDECLI").ToString()
                        End If
                        dr("I_XNUICLI") = Partealfabetica
                        dr("I_NDIGCHE") = ObjUtil.strAsignarValor(Partealfabetica.ToString, "0")
                        If ds.Tables(0).Columns.Contains("NSUCCLI") Then
                            dr("I_NIDESUC") = ds.Tables(0).Rows(0)("NSUCCLI").ToString()
                        Else
                            dr("I_NIDESUC") = ObjUtil.strAsignarValor(Sucursal, "0")
                        End If
                        If ds.Tables(0).Columns.Contains("NCOSCLI") Then
                            dr("I_NIDECOS") = ds.Tables(0).Rows(0)("NCOSCLI").ToString()
                        Else
                            dr("I_NIDECOS") = ObjUtil.strAsignarValor(CentroCosto, "0")
                        End If
                        dr("I_CPRGSRV") = m_Programa
                        dr("I_NCONAFI") = ObjUtil.strAsignarValor(m_Condicion, "0")
                       If ds.Tables(0).Columns.Contains("XRESP1") Then
                            If ds.Tables(0).Rows(0)("XRESP1").ToString() <> "" Then
                                dr("I_TRES001") = ObjUtil.GetEquivTipoDoc(ds.Tables(0).Rows(0)("XRESP1").ToString().Substring(0, 1))
                                dr("I_NRES001") = ds.Tables(0).Rows(0)("XRESP1").ToString().Substring(2)
                            End If
                        End If
                        If ds.Tables(0).Columns.Contains("XGENCLI") Then
                            dr("I_XGENCLI") = ds.Tables(0).Rows(0)("XGENCLI").ToString()
                        Else
                            dr("I_XGENCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("FNACCLI") Then
                            dr("I_FNACCLI") = ds.Tables(0).Rows(0)("FNACCLI").ToString()
                        Else
                            dr("I_FNACCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XECICLI") Then
                            dr("I_CECICLI") = ds.Tables(0).Rows(0)("XECICLI").ToString()
                        Else
                            dr("I_CECICLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XNIVADI") Then
                            dr("I_XNIVADI") = ds.Tables(0).Rows(0)("XNIVADI").ToString()
                        Else
                            dr("I_XNIVADI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("CCAUINA") Then
                            dr("I_CCAUINA") = ds.Tables(0).Rows(0)("CCAUINA").ToString()
                        Else
                            dr("I_CCAUINA") = "0"
                        End If
                        If ds.Tables(0).Columns.Contains("XPRIAPE") Then
                            dr("I_XPRIAPE") = ds.Tables(0).Rows(0)("XPRIAPE").ToString()
                        Else
                            dr("I_XPRIAPE") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XSEGAPE") Then
                            dr("I_XSEGAPE") = ds.Tables(0).Rows(0)("XSEGAPE").ToString()
                        Else
                            dr("I_XSEGAPE") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XNOMCLI") Then
                            dr("I_XNOMTRAB") = ds.Tables(0).Rows(0)("XNOMCLI").ToString()
                        Else
                            dr("I_XNOMTRAB") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XRAZSEG") Then
                            dr("I_XDESRAZ") = ds.Tables(0).Rows(0)("XRAZSEG").ToString()
                        Else
                            dr("I_XDESRAZ") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XTIPDIR") Then
                            dr("I_XTIPDIR") = ds.Tables(0).Rows(0)("XTIPDIR").ToString()
                        Else
                            dr("I_XTIPDIR") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XBARRIO") Then
                            If ds.Tables(0).Rows(0)("XBARRIO").ToString.Length > 18 Then
                                dr("I_XBARCLI") = ds.Tables(0).Rows(0)("XBARRIO").ToString.Substring(0, 18)
                            Else
                                dr("I_XBARCLI") = ds.Tables(0).Rows(0)("XBARRIO").ToString()
                            End If
                        Else
                            dr("I_XBARCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("XDIR") Then
                            dr("I_XDIRCLI") = ds.Tables(0).Rows(0)("XDIR").ToString()
                        Else
                            dr("I_XDIRCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("NTELCLI") Then
                            dr("I_NTELCLI") = ds.Tables(0).Rows(0)("NTELCLI").ToString()
                        Else
                            dr("I_NTELCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("NEXTCLI") Then
                            dr("I_NEXTCLI") = ds.Tables(0).Rows(0)("NEXTCLI").ToString()
                        Else
                            dr("I_NEXTCLI") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("CZON") Then
                            If ds.Tables(0).Rows(0)("CZON").ToString.Length > 4 Then
                                dr("I_CZONBTA") = ds.Tables(0).Rows(0)("CZON").ToString.Substring(0, 4)
                            Else
                                dr("I_CZONBTA") = ds.Tables(0).Rows(0)("CZON").ToString()
                            End If
                        Else
                            dr("I_CZONBTA") = ""
                        End If
                        If ds.Tables(0).Columns.Contains("CCIU") Then
                            dr("I_CCIUCLI") = ds.Tables(0).Rows(0)("CCIU").ToString()
                        Else
                            dr("I_CCIUCLI") = ""
                        End If
                        dr("MESSAGE") = "INPUT REQUEST"
                    End With
                    cle15.Tables(0).Rows.Add(dr)
                Else
                    If dr.Table.Columns.Count <> 0 Then
                        For i = 0 To dr.Table.Columns.Count - 1
                            If dr.Item(i).ToString = "" Then
                                dr(i) = ""
                            End If
                        Next
                    End If
                    'MODIFICADO LCARDENAS 25-10-2011 INCIDENCIA 6768
                    If dr.Item("I_FINGEMP").ToString = "" Then
                        dr.Item("I_FINGEMP") = System.DBNull.Value
                    End If
                    dr("MESSAGE") = "CLIENTE NO EXISTE"
                    cle15.Tables(0).Rows.Add(dr)
                End If
            Else
                If dr.Table.Columns.Count <> 0 Then
                    For i = 0 To dr.Table.Columns.Count - 1
                        If dr.Item(i).ToString = "" Then
                            dr(i) = ""
                        End If
                    Next
                End If
                'MODIFICADO LCARDENAS 25-10-2011 INCIDENCIA 6768
                If dr.Item("I_FINGEMP").ToString = "" Then
                    dr.Item("I_FINGEMP") = System.DBNull.Value
                End If
                dr("MESSAGE") = "CLIENTE NO EXISTE"
                cle15.Tables(0).Rows.Add(dr)
            End If
            returnvalue = cle15.GetXml
            returnvalue = returnvalue.Replace("<DtsCLE15 xmlns=""http://tempuri.org/DtsCLE15.xsd"">", "<CLE15>")
            Return returnvalue.Replace("</DtsCLE15>", "</CLE15>")
        Catch Ex As Exception
            'CREADO LCARDENAS 19-04-2012: SEGUIMIENTO INCIDENCIA 9430 INTERMITENCIA WSCOMCLIENTES
            If ConfigurationSettings.AppSettings("LogAuditoriaAfiliar") = "S" Then
                Dim objCache As New Compensar.Vincula.POS.ClsCache
                Dim sInnerExcepcion As String = ""
                If Not Ex.InnerException Is Nothing Then
                    sInnerExcepcion = Ex.InnerException.ToString()
                End If
                objCache.guardarErrorBitacora(ConfigurationSettings.AppSettings("ProjectID"), _
1, "Auditoria Afiliar: CConsultaClienteINQ.ConsultarCliente: " & Ex.Message & sInnerExcepcion & "StackTrace: " & Ex.StackTrace, "", "", "", My.Computer.Name, 1, 1)
            End If
            Return ObjUtil.ConvertToXMLdoc(Ex.Message)
        End Try
    End Function

End Class

Imports Compensar.SISPOS.DAL
Imports Compensar.SISPOS.ESL
Imports Servicios
Imports Compensar.Vincula
Imports System.Data
Imports WsComClientes
Imports System.Configuration
Imports Compensar.SISPOS.ESL.Vinculacion

Partial Public Class frnTest
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then


            Dim LoginName As String = Me.User.Identity.Name
            'Dim tramaTrab As New FrameWorkNet2.AdminusProviderModel.AdusMembershipProvider()
            Dim DateNow As String = Nothing
            'Dim usuario As System.Web.Security.MembershipUser
            'usuario = tramaTrab.GetUser(LoginName, True)
            DateNow = Format(Date.Now, "dd/MM/yyyy")

            Label2.Text = DateNow
        End If
    End Sub


    Protected Sub btnTodo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTodo.Click
        Call btnconexiones_Click(sender, e)
        imgbtnTodo.Visible = True
        imgbtnTodo.ImageUrl = imgbtnconexiones.ImageUrl
        lblbtnTodo.Text = lblConexiones.Text

        Call btnWscomclientes_Click(sender, e)
        If imgbtnTodo.ImageUrl <> "./Imagenes/btnX.jpg" Then
            lblbtnTodo.Text = lblbtnWscomclientes.Text
            imgbtnTodo.ImageUrl = imgbtnWscomclientes.ImageUrl
        End If

        'Call btnWscomclientes_Click(sender, e)
        'If imgbtnTodo.ImageUrl <> "./Imagenes/btnX.jpg" Then
        '    lblbtnTodo.Text = lblbtnWscomclientes.Text
        '    imgbtnTodo.ImageUrl = imgbtnWscomclientes.ImageUrl
        'End If
        Call BtnSiam_Click(sender, e)
        If imgbtnTodo.ImageUrl <> "./Imagenes/btnX.jpg" Then
            lblbtnTodo.Text = lblBtnSiam.Text
            imgbtnTodo.ImageUrl = imgBtnSiam.ImageUrl
        End If

        Call btnMasivoConsultaT_Click(sender, e)
        If imgbtnTodo.ImageUrl <> "./Imagenes/btnX.jpg" Then
            lblbtnTodo.Text = lblbtnMasivoConsultaT.Text
            imgbtnTodo.ImageUrl = imgbtnMasivoConsultaT.ImageUrl
        End If

        Call btnWcompensar_Click(sender, e)
        If imgbtnTodo.ImageUrl <> "./Imagenes/btnX.jpg" Then
            lblbtnTodo.Text = lblWcompensar.Text
            imgbtnTodo.ImageUrl = imgWcompensar.ImageUrl
        End If
    End Sub

    Protected Sub btnCxComeps_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCxComeps.Click
        Try
            Dim oDal As New Compensar.SISPOS.DAL.VinculacionPOS.Afiliaciones(ConfigurationSettings.AppSettings("AfilPos_ProjectID"))
            Dim obj As DataSet

            obj = oDal.ConsultarParametrosAfiliacion()
            imgbtnCxComeps.Visible = True
            If obj.Tables.Count > 0 Then
                imgbtnCxComeps.ImageUrl = "./Imagenes/btnY.jpg"
                lblComeps.Text = ("Exitoso!!!")
            Else
                imgbtnCxComeps.ImageUrl = "./Imagenes/btnX.jpg"
                lblComeps.Text = ("Fallido!!!")
            End If
        Catch ex As Exception
            imgbtnCxComeps.ImageUrl = "./Imagenes/btnX.jpg"
            imgbtnCxComeps.Visible = True
            imgbtnCxComeps.ToolTip = ex.Message
            lblComeps.Text = ("Fallido!!!")
        End Try
    End Sub

    Protected Sub btnCxComadmo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCxComadmo.Click
        Try
            Dim oDal As New Compensar.SISPOS.DAL.Comadmo(ConfigurationSettings.AppSettings("AfilPos_codigoComadmo"))
            Dim obj As IDataReader

            obj = oDal.consultaTotalAportesPorPeriodo("201201", 9999803432625645)
            If obj.Read() Then
                imgbtnCxComadmo.ImageUrl = "./Imagenes/btnY.jpg"
                imgbtnCxComadmo.Visible = True
                lblComadmo.Text = "Exitoso!!!"
                obj.Close()
            End If

        Catch ex As Exception
            imgbtnCxComadmo.Visible = True
            imgbtnCxComadmo.ImageUrl = "./Imagenes/btnX.jpg"
            imgbtnCxComadmo.ToolTip = ex.Message
            lblComadmo.Text = "Fallido!!!"
        End Try
    End Sub

    Protected Sub btnconexiones_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnconexiones.Click
        Try
            Call btnCxComeps_Click(sender, e)
            imgbtnconexiones.ImageUrl = imgbtnCxComeps.ImageUrl
            lblConexiones.Text = lblComeps.Text
            imgbtnconexiones.Visible = True
            Call btnCxComadmo_Click(sender, e)
            If imgbtnconexiones.ImageUrl <> "./Imagenes/btnX.jpg" Then
                lblConexiones.Text = lblComadmo.Text
                imgbtnconexiones.ImageUrl = imgbtnCxComadmo.ImageUrl
            End If
        Catch ex As Exception
            imgbtnconexiones.Visible = True
            imgbtnconexiones.ImageUrl = "./Imagenes/btnX.jpg"
            imgbtnconexiones.ToolTip = ex.Message
            lblConexiones.Text = "Fallido!!!"
        End Try
    End Sub

    Protected Sub btnWscomclientes1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWscomclientes1.Click
        Try
            imgbtnWscomclientes1.Visible = True
            Dim oVIn As New WsComClientes.Afiliaciones
            Dim strConsulta As String
            strConsulta = oVIn.ConsultaCliente(ConfigurationSettings.AppSettings("AfilPos_ProjectID"), "<XmlClientes><TipoIdent>CC</TipoIdent><NroIdentificacion>9731800</NroIdentificacion></XmlClientes>")
            If strConsulta.IndexOf("9731800") > 0 Then
                imgbtnWscomclientes1.ImageUrl = "./Imagenes/btnY.jpg"
                lblbtnWscomclientes1.Text = "Exitoso!!!"

            Else
                imgbtnWscomclientes1.ImageUrl = "./Imagenes/btnX.jpg"
                lblbtnWscomclientes1.Text = "Fallido!!!"
                imgbtnWscomclientes1.ToolTip = strConsulta
            End If

        Catch ex As Exception
            imgbtnWscomclientes1.Visible = True
            imgbtnWscomclientes1.ImageUrl = "./Imagenes/btnX.jpg"
            imgbtnWscomclientes1.ToolTip = ex.Message
            lblbtnWscomclientes1.Text = "Fallido!!!"
        End Try
    End Sub

    Protected Sub btnWscomclientes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWscomclientes.Click
        Call btnWscomclientes1_Click(sender, e)
        imgbtnWscomclientes.Visible = True
        imgbtnWscomclientes.ImageUrl = imgbtnWscomclientes1.ImageUrl
        lblbtnWscomclientes.Text = lblbtnWscomclientes1.Text
        '4808 20140909
        Call btnActualizarReq_Click(sender, e)
        If imgbtnWscomclientes.ImageUrl <> "./Imagenes/btnX.jpg" Then
            imgbtnWscomclientes.ImageUrl = imgActEstReq.ImageUrl
            lblbtnWscomclientes.Text = lblActEstReq.Text
        End If
        '4807 20140925
        Call btnActualizarAfil_Click(sender, e)
        If imgbtnWscomclientes.ImageUrl <> "./Imagenes/btnX.jpg" Then
            imgbtnWscomclientes.ImageUrl = imgActualizarAfil.ImageUrl
            lblbtnWscomclientes.Text = lblActualizarAfil.Text
        End If
        '7876 20150716
        Call btnActEmpresa_Click(sender, e)
        If imgbtnWscomclientes.ImageUrl <> "./Imagenes/btnX.jpg" Then
            imgbtnWscomclientes.ImageUrl = imgActualizarAfil.ImageUrl
            lblbtnWscomclientes.Text = lblActualizarAfil.Text
        End If
    End Sub

    Protected Sub BtnSiam1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSiam1.Click

        Dim oSiam As New POS.wsSiamProxy
        Dim sXmlRes As String, UserNameWsSiam As String, PasswordWsSiam As String
        Try

            UserNameWsSiam = ConfigurationSettings.AppSettings("USRSIAM_VALLE")
            PasswordWsSiam = ConfigurationSettings.AppSettings("PWDSIAM_VALLE")

            sXmlRes = oSiam.ConsultaAfiliados(UserNameWsSiam, PasswordWsSiam, _
              2, 14572527, False)
            imgBtnSiam1.Visible = True
            If sXmlRes <> "" And Not sXmlRes.Contains("Error") Then
                imgBtnSiam1.ImageUrl = "./Imagenes/btnY.jpg"
                lblBtnSiam1.Text = "Exitoso!!!"
            Else
                imgBtnSiam1.Visible = True
                imgBtnSiam1.ImageUrl = "./Imagenes/btnX.jpg"
                imgBtnSiam1.ToolTip = sXmlRes
            End If


        Catch ex As Exception
            imgBtnSiam1.Visible = True
            imgBtnSiam1.ImageUrl = "./Imagenes/btnX.jpg"
            imgBtnSiam1.ToolTip = ex.Message
            lblBtnSiam1.Text = "Fallido!!!"
        End Try
    End Sub

    Protected Sub BtnSiam_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSiam.Click
        Call BtnSiam1_Click(sender, e)
        imgBtnSiam.Visible = True
        imgBtnSiam.ImageUrl = imgBtnSiam1.ImageUrl
        imgBtnSiam.ToolTip = imgBtnSiam1.ToolTip
        lblBtnSiam.Text = lblBtnSiam1.Text

    End Sub

    Protected Sub btnMasivoConsulta1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMasivoConsulta1.Click
        Dim sParams As String
        Dim objParamas As New WsComClientes.ServicioCargueMasivo
        Try


            sParams = objParamas.ConsultaParametrosSemana(ConfigurationSettings.AppSettings("AfilPos_ProjectID"), -1)
            With imgbtnMasivoConsulta1
                .Visible = True
                If Not sParams.Equals("</ NewDataSet>") And Not sParams.Contains("<?xml version=") Then
                    .ImageUrl = "./Imagenes/btnY.jpg"
                    lblbtnMasivoConsulta1.Text = "Exitoso!!!"
                Else
                    .Visible = True
                    .ImageUrl = "./Imagenes/btnX.jpg"
                    .ToolTip = sParams
                    lblbtnMasivoConsulta1.Text = "Fallido!!!"
                End If

            End With
        Catch ex As Exception
            With imgbtnMasivoConsulta1
                .Visible = True
                .ImageUrl = "./Imagenes/btnX.jpg"
                .ToolTip = ex.Message
                lblbtnMasivoConsulta1.Text = "Fallido!!!"
            End With
        End Try

    End Sub

    Protected Sub btnMasivoConsultaT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMasivoConsultaT.Click
        Try


            Call btnMasivoConsulta1_Click(sender, e)
            With imgbtnMasivoConsultaT
                .Visible = True
                .ImageUrl = imgbtnMasivoConsulta1.ImageUrl
                lblbtnMasivoConsultaT.Text = lblbtnMasivoConsulta1.Text
                Call btnLogWin_Click(sender, e)
                .ImageUrl = imgbtnLogWin.ImageUrl
                lblbtnMasivoConsultaT.Text = lblbtnLogWin.Text

            End With
        Catch ex As Exception
            With imgbtnMasivoConsultaT
                .Visible = True
                .ImageUrl = "./Imagenes/btnX.jpg"
                .ToolTip = ex.Message
                lblbtnMasivoConsultaT.Text = "Fallido!!!"
            End With
        End Try
    End Sub

    Protected Sub btnALDEA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnALDEA.Click
        'Dim objAldeaProxy As New Compensar.SISPOS.ESL.Compensar.SISPOS.ESL.ALDEA.AF
        Dim strConsulta As String
        Try

            strConsulta = Compensar.SISPOS.ESL.Vinculacion.ConsultaAF.ConsultaNautcliSegunIdentificacion(ConfigurationSettings.AppSettings("AfilPos_ProjectID"), 9731800, 1)
            With imgAldea
                .Visible = True
                If strConsulta.Contains("6212803760327591") Then
                    .ImageUrl = "./Imagenes/btnY.jpg"
                    lblAldea.Text = "Exitoso!!!"
                Else
                    .ImageUrl = "./Imagenes/btnX.jpg"
                    .ToolTip = strConsulta
                    lblAldea.Text = "Fallido!!!"

                End If
            End With
        Catch ex As Exception
            With imgAldea
                .Visible = True
                .ImageUrl = "./Imagenes/btnX.jpg"
                .ToolTip = ex.Message
                lblAldea.Text = "Fallido!!!"
            End With
        End Try



    End Sub

    Protected Sub btnCAF_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCAF.Click
        Dim objCafProxy As New Compensar.SISPOS.ESL.Compensar.SISPOS.ESL.CAF.CAF
        Dim dtsConsulta As DataSet
        Try
            dtsConsulta = Compensar.SISPOS.ESL.Vinculacion.ConsultaAF.ConsultaAfiliadoPOS(ConfigurationSettings.AppSettings("AfilPos_ProjectID"), 6039817748363231, "021206")
            With imgCAF
                .Visible = True
                If dtsConsulta.Tables.Count > 0 Then
                    .ImageUrl = "./Imagenes/btnY.jpg"
                    lblCAF.Text = "Exitoso!!!"
                Else
                    dtsConsulta = Compensar.SISPOS.ESL.Vinculacion.ConsultaAF.ConsultaAfiliadoPOS(ConfigurationSettings.AppSettings("AfilPos_ProjectID"), 9999803441430802, "021206")
                    If dtsConsulta.Tables.Count > 0 Then
                        .ImageUrl = "./Imagenes/btnY.jpg"
                        lblCAF.Text = "Exitoso!!!"
                    Else
                        .ImageUrl = "./Imagenes/btnX.jpg"
                        lblCAF.Text = "Fallido!!!"
                        .ToolTip = dtsConsulta.GetXml
                    End If
                End If
            End With

        Catch ex As Exception
            With imgCAF
                .Visible = True
                .ImageUrl = "./Imagenes/btnX.jpg"
                lblCAF.Text = ("Fallido!!!")
                .ToolTip = ex.Message

            End With

        End Try
    End Sub

    Protected Sub btnIspec_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnIspec.Click
        Try
            Dim objServicios As New Servicios.Servicios.AFSYS
            Dim strConsulta As String, strXml As String

            strXml = "<CLE15><CLE15><_TOP_LINE_>CLE15T00007511NOV11</_TOP_LINE_><I_MAINT>INQ</I_MAINT><I_XISP /><I_XINDOPC /><I_FVALCLI>0</I_FVALCLI><I_NAUTCLI>6212803760327591</I_NAUTCLI><I_XACC1 /><I_XACC2 /><I_XACC3 /><I_XACC4 /><I_XACC5 /><I_XACC6 /><I_XACC7 /><I_XACC8 /><I_XACC9 /><I_XACC10 /><I_XACC11 /><I_XACC12 /><I_NEDA>17</I_NEDA><I_CPRN /><I_NRADDOC>970400084</I_NRADDOC><I_CALT>830049724</I_CALT><I_XIDECLI>CC</I_XIDECLI><I_NIDECLI>9731800</I_NIDECLI><I_XNUICLI /><I_NDIGCHE>0</I_NDIGCHE><I_NIDESUC>0</I_NIDESUC><I_NIDECOS>0</I_NIDECOS><I_XGENCLI>F</I_XGENCLI><I_FNACCLI>19941110</I_FNACCLI><I_CECICLI>SO</I_CECICLI><I_CGRUCAM>0</I_CGRUCAM><I_XNIVADI>N</I_XNIVADI><I_XPRIAPE>MARIN</I_XPRIAPE><I_XSEGAPE>GALINDO</I_XSEGAPE><I_XNOMTRAB>ANDREA LUCIA</I_XNOMTRAB><I_XVARRAN /><I_CCAUINA>0</I_CCAUINA><I_XDESRAZ /><I_XPRGEQU /><I_XTIPDIR /><I_FRET>0</I_FRET><I_CPRGSRV /><I_NCONAFI>0</I_NCONAFI><I_XNOMPRG /><I_XBARCLI /><I_NDIR>1</I_NDIR><I_XDIRCLI /><I_CBARRIO>0</I_CBARRIO><I_XNMNCA1 /><I_FINIVI1>0</I_FINIVI1><I_FFINVI1>0</I_FFINVI1><I_VNUMDA1>0</I_VNUMDA1><I_XALFDA1 /><I_XNMNCA2 /><I_FINIVI2>0</I_FINIVI2><I_FFINVI2>0</I_FFINVI2><I_VNUMDA2>0</I_VNUMDA2><I_XALFDA2 /><I_XNMNCA3 /><I_FINIVI3>0</I_FINIVI3><I_FFINVI3>0</I_FFINVI3><I_VNUMDA3>0</I_VNUMDA3><I_XALFDA3 /><I_XNMNCA4 /><I_FINIVI4>0</I_FINIVI4><I_FFINVI4>0</I_FFINVI4><I_VNUMDA4>0</I_VNUMDA4><I_XALFDA4 /><I_XNMNCA5 /><I_FINIVI5>0</I_FINIVI5><I_FFINVI5>0</I_FFINVI5><I_VNUMDA5>0</I_VNUMDA5><I_XALFDA5 /><I_XNMNCA6 /><I_FINIVI6>0</I_FINIVI6><I_FFINVI6>0</I_FFINVI6><I_VNUMDA6>0</I_VNUMDA6><I_XALFDA6 /><I_XNMNCA7 /><I_FINIVI7>0</I_FINIVI7><I_FFINVI7>0</I_FFINVI7><I_VNUMDA7>0</I_VNUMDA7><I_XALFDA7 /><I_XNMNCA8 /><I_FINIVI8>0</I_FINIVI8><I_FFINVI8>0</I_FFINVI8><I_VNUMDA8>0</I_VNUMDA8><I_XALFDA8 /><I_XNMNCA9 /><I_FINIVI9>0</I_FINIVI9><I_FFINVI9>0</I_FFINVI9><I_VNUMDA9>0</I_VNUMDA9><I_XALFDA9 /><I_XNMNCA10 /><I_FINIVI10>0</I_FINIVI10><I_FFINVI10>0</I_FFINVI10><I_VNUMDA10>0</I_VNUMDA10><I_XALFDA10 /><I_XNMNCA11 /><I_FINIVI11>0</I_FINIVI11><I_FFINVI11>0</I_FFINVI11><I_VNUMDA11>0</I_VNUMDA11><I_XALFDA11 /><I_XNMNCA12 /><I_FINIVI12>0</I_FINIVI12><I_FFINVI12>0</I_FFINVI12><I_VNUMDA12>0</I_VNUMDA12><I_XALFDA12 /><I_XIDENUE /><I_NIDENUE>0</I_NIDENUE><I_XNUINUE /><I_NSUCNUE>0</I_NSUCNUE><I_NCOSNUE>0</I_NCOSNUE><I_NRES003>0</I_NRES003><I_NRES004>0</I_NRES004><I_TRES001 /><I_NRES001>0</I_NRES001><I_XRES001 /><I_TRES002 /><I_NRES002>0</I_NRES002><I_XRES002 /><I_VVIN>0</I_VVIN><I_XESCVIN /><I_CESTVIN>0</I_CESTVIN><I_NDIRVIN>0</I_NDIRVIN><I_FINIAFI>0</I_FINIAFI><I_FFINAFI>0</I_FFINAFI><I_CRETAFI>0</I_CRETAFI><I_FVIN>0</I_FVIN><I_QVIN>0</I_QVIN><I_XVIN /><I_XALFADI /><I_NTELCLI>0</I_NTELCLI><I_NEXTCLI>0</I_NEXTCLI><I_CZONBTA>0</I_CZONBTA><I_CCIUCLI>0</I_CCIUCLI><I_FINGEMP>0</I_FINGEMP><I_CSED>1007</I_CSED><I_CODEPS>0</I_CODEPS></CLE15></CLE15>"

            strConsulta = objServicios.Ispec(ConfigurationSettings.AppSettings("AfilPos_ProjectID"), "CLE15", strXml, 1)

            With imgInspec
                .Visible = True
                If strConsulta.IndexOf("INPUT REQUEST") > 0 Then
                    .ImageUrl = "./Imagenes/btnY.jpg"
                    lblInspec.Text = "Exitoso!!!"
                Else
                    .ImageUrl = "./Imagenes/btnX.jpg"
                    lblInspec.Text = "Fallido!!!"
                    .ToolTip = strConsulta
                End If

            End With
        Catch ex As Exception
            With imgInspec
                .Visible = True
                .ImageUrl = "./Imagenes/btnX.jpg"
                lblInspec.Text = "Fallido!!!"
                .ToolTip = ex.Message

            End With
        End Try
    End Sub

    Protected Sub btnWcompensar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWcompensar.Click

        Call btnALDEA_Click(sender, e)
        With imgWcompensar
            .Visible = True
            If .ImageUrl <> "./Imagenes/btnX.jpg" Then
                lblWcompensar.Text = lblAldea.Text
                .ImageUrl = imgAldea.ImageUrl
            End If

            Call btnCAF_Click(sender, e)
            If .ImageUrl <> "./Imagenes/btnX.jpg" Then
                lblWcompensar.Text = lblCAF.Text
                .ImageUrl = imgCAF.ImageUrl
            End If

            Call btnIspec_Click(sender, e)
            If .ImageUrl <> "./Imagenes/btnX.jpg" Then
                lblWcompensar.Text = lblInspec.Text
                .ImageUrl = imgInspec.ImageUrl
            End If
            Call btnLogWin_Click(sender, e)
            If .ImageUrl <> "./Imagenes/btnX.jpg" Then
                lblWcompensar.Text = lblbtnLogWin.Text
                .ImageUrl = imgbtnLogWin.ImageUrl
            End If
        End With

    End Sub

    Protected Sub btnLogWin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogWin.Click
        With imgbtnLogWin
            Try
                .Visible = True
                CManejadorMensajes.PublicarMensaje("clsTest:Test" & "Mensaje Prueba" & " - " & "XXXXXXX", EventLogEntryType.SuccessAudit)
                .ImageUrl = "./Imagenes/btnY.jpg"
                lblbtnLogWin.Text = "Exitoso!!!"
            Catch ex As Exception
                .ImageUrl = "./Imagenes/btnX.jpg"
                lblbtnLogWin.Text = "Fallido!!!"
                .ToolTip = ex.Message
            End Try
        End With


    End Sub
    '4808 20140909 TEst de Metodo ActualizarEstadoRequisito
    Protected Sub btnActualizarReq_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnActualizarReq.Click
        Dim oVIn As New WsComClientes.Afiliaciones
        Try
            Dim dstRequisitos As New DataSet, dtr As DataRow()
            imgActEstReq.Visible = True

            Dim strConsulta As String
            strConsulta = oVIn.ActualizarEstadoRequisito(ConfigurationSettings.AppSettings("AfilPos_ProjectID"), "<REQUISITO><TipoIdent>CC</TipoIdent><NroIdentificacion>9731800</NroIdentificacion><ParteAlfabetica /><Sucursal>0</Sucursal><Centrocosto>0</Centrocosto><Usuario>99999999999</Usuario><Requisito>CORELE</Requisito>  <EstadoRequisito>4</EstadoRequisito></REQUISITO>")
            If strConsulta.IndexOf("EXITO") > 0 Then
                imgActEstReq.ImageUrl = "./Imagenes/btnY.jpg"
                lblActEstReq.Text = "Exitoso!!!"

            Else
                imgActEstReq.ImageUrl = "./Imagenes/btnX.jpg"
                lblActEstReq.Text = "Fallido!!!"
                imgActEstReq.ToolTip = strConsulta
            End If

        Catch ex As Exception
            imgActEstReq.Visible = True
            imgActEstReq.ImageUrl = "./Imagenes/btnX.jpg"
            imgActEstReq.ToolTip = ex.Message
            lblActEstReq.Text = "Fallido!!!"
        Finally
            Try
                oVIn.ActualizarEstadoRequisito(ConfigurationSettings.AppSettings("AfilPos_ProjectID"), "<REQUISITO><TipoIdent>CC</TipoIdent><NroIdentificacion>9731800</NroIdentificacion><ParteAlfabetica /><Sucursal>0</Sucursal><Centrocosto>0</Centrocosto><Usuario>99999999999</Usuario><Requisito>CORELE</Requisito>  <EstadoRequisito>1</EstadoRequisito></REQUISITO>")
            Catch ex As Exception

            End Try

        End Try
    End Sub

    '4807 20140925 Test de Metodo ActualizarAfiliado
    Protected Sub btnActualizarAfil_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnActualizarAfil.Click
        Dim oVIn As New WsComClientes.Afiliaciones
        Try
            imgActualizarAfil.Visible = True

            Dim strConsulta As String
            strConsulta = oVIn.ActualizarAfiliado(ConfigurationSettings.AppSettings("AfilPos_ProjectID"), "<AFILIADO><TipoIdent>CC</TipoIdent><NroIdentificacion>1101752819</NroIdentificacion><ParteAlfabetica /><DigitoChequeo /><Sucursal>0</Sucursal><CentroCosto>0</CentroCosto><TipoIdentNew /><NroIdentificacionNew /><ParteAlfabeticaNew /><DigitoChequeoNew /><SucursalNew /><CentroCostoNew /><PrimerNombre>CAMILO</PrimerNombre><SegundoNombre>ANDRES</SegundoNombre><PrimerApellido>ARIZA</PrimerApellido><SegundoApellido>PARDO</SegundoApellido><FechaNacimiento /><Genero /><EstadoCivil /><Direccion /><Telefono /><TipoDireccion /><Barrio /><Zona>1</Zona><Ciudad /><RazonSocial /><CodOcupacion /><GrupoEtnico /><GrupoPoblacion /><NivelEscolaridad /><CorreoElectronico /><Celular /><AutMsjTexto /><AutMsjCorreoElect /><Usuario>1032409047</Usuario><ConsecutivoDir /></AFILIADO>")
            If strConsulta.IndexOf("El sistema realizo la actualización") > 0 Then
                imgActualizarAfil.ImageUrl = "./Imagenes/btnY.jpg"
                lblActualizarAfil.Text = "Exitoso!!!"

            Else
                imgActualizarAfil.ImageUrl = "./Imagenes/btnX.jpg"
                lblActualizarAfil.Text = "Fallido!!!"
                imgActualizarAfil.ToolTip = strConsulta
            End If

        Catch ex As Exception
            imgActualizarAfil.Visible = True
            imgActualizarAfil.ImageUrl = "./Imagenes/btnX.jpg"
            imgActualizarAfil.ToolTip = ex.Message
            lblActualizarAfil.Text = "Fallido!!!"
        End Try
    End Sub

    '7876 20150714 Test de Metodo ActualizarEmpresa
    Protected Sub btnActEmpresa_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnActEmpresa.Click

        Dim oVIn As New WsComClientes.Afiliaciones
        Try
            ImgActEmpresa.Visible = True

            Dim strConsulta As String
            strConsulta = oVIn.ActualizarEmpresa(ConfigurationSettings.AppSettings("AfilPos_ProjectID"), "<EMPRESA><TipIdeActual>CC</TipIdeActual><NumIdeActual>79826414</NumIdeActual><ParAlfActual></ParAlfActual><DigCheActual></DigCheActual><NumDepActual></NumDepActual><TipIdeNuevo>CC</TipIdeNuevo><NumIdeNuevo>79826414</NumIdeNuevo><ParAlfNuevo></ParAlfNuevo><DigCheNuevo></DigCheNuevo><NumDepNuevo></NumDepNuevo><PriNombre></PriNombre><SegNombre></SegNombre><PriApellido></PriApellido><SegApellido></SegApellido><RazSocial></RazSocial><ActEconomica>10</ActEconomica ><NomRepLegal>MARIA XIMENA DIAZ</NomRepLegal><CorCorporativo>corporativo@gmail.com</CorCorporativo><CorNovPagos>novPagos@yahoo.es</CorNovPagos><CorNovAfiliacion>novAfiliacion@hotmail.com</CorNovAfiliacion><CorIncapacidades>incapacidades@gmail.com</CorIncapacidades><CorMovEPS>movEPS@mymail.com</CorMovEPS><NumTelFijo>7852365</NumTelFijo><NumTelMovil>3101234567</NumTelMovil><Departamento>11</Departamento><Ciudad>11001</Ciudad><Direccion>CR 63 22 10 BL 4 CA 1</Direccion><NumFax>2345678</NumFax><TipDireccion>U</TipDireccion><Barrio>SALITE ALTO</Barrio><LocZona>98</LocZona><Usuario>1032409047</Usuario></EMPRESA>")
            If strConsulta.IndexOf("El sistema realizo la actualización") > 0 Then
                ImgActEmpresa.ImageUrl = "./Imagenes/btnY.jpg"
                lblActEmpresa.Text = "Exitoso!!!"

            Else
                ImgActEmpresa.ImageUrl = "./Imagenes/btnX.jpg"
                lblActEmpresa.Text = "Fallido!!!"
                ImgActEmpresa.ToolTip = strConsulta
            End If

        Catch ex As Exception
            ImgActEmpresa.Visible = True
            ImgActEmpresa.ImageUrl = "./Imagenes/btnX.jpg"
            ImgActEmpresa.ToolTip = ex.Message
            lblActEmpresa.Text = "Fallido!!!"
        End Try
    End Sub
End Class
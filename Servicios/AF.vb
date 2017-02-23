﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Este código fue generado por una herramienta.
'     Versión del motor en tiempo de ejecución:2.0.50727.3074
'
'     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
'     se vuelve a generar el código.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'This source code was auto-generated by wsdl, Version=2.0.50727.42.
'

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42"), _
 System.Diagnostics.DebuggerStepThroughAttribute(), _
 System.ComponentModel.DesignerCategoryAttribute("code"), _
 System.Web.Services.WebServiceBindingAttribute(Name:="AFSoap", [Namespace]:="http://tempuri.org/WCompensar/AF")> _
Partial Public Class AF
    Inherits System.Web.Services.Protocols.SoapHttpClientProtocol

    Private AfiliadoOperationCompleted As System.Threading.SendOrPostCallback

    Private DatosVinculacionOperationCompleted As System.Threading.SendOrPostCallback

    Private EmpresaOperationCompleted As System.Threading.SendOrPostCallback

    Private ConsultaDatosBasicosOperationCompleted As System.Threading.SendOrPostCallback

    Private AgendaOperationCompleted As System.Threading.SendOrPostCallback
    Private pCodigoApp As String
    '''<remarks/>
    Public Sub New()
        MyBase.New()
        Me.Url = System.Configuration.ConfigurationManager.AppSettings("URLWSALDEA")  '"http://172.22.145.17/wcompensar/aldea/af.asmx"
        pCodigoApp = System.Configuration.ConfigurationManager.AppSettings("AfilPos_ProjectID")
    End Sub

    '''<remarks/>
    Public Event AfiliadoCompleted As AfiliadoCompletedEventHandler

    '''<remarks/>
    Public Event DatosVinculacionCompleted As DatosVinculacionCompletedEventHandler

    '''<remarks/>
    Public Event EmpresaCompleted As EmpresaCompletedEventHandler

    '''<remarks/>
    Public Event ConsultaDatosBasicosCompleted As ConsultaDatosBasicosCompletedEventHandler

    '''<remarks/>
    Public Event AgendaCompleted As AgendaCompletedEventHandler

    '''<remarks/>
    <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/WCompensar/AF/Afiliado", RequestNamespace:="http://tempuri.org/WCompensar/AF", ResponseNamespace:="http://tempuri.org/WCompensar/AF", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)> _
    Public Function Afiliado(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer) As String
        Dim results() As Object = Me.Invoke("Afiliado", New Object() {sApl, sParametros, iOpc})
        Return CType(results(0), String)
    End Function

    '''<remarks/>
    Public Function BeginAfiliado(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
        Return Me.BeginInvoke("Afiliado", New Object() {sApl, sParametros, iOpc}, callback, asyncState)
    End Function

    '''<remarks/>
    Public Function EndAfiliado(ByVal asyncResult As System.IAsyncResult) As String
        Dim results() As Object = Me.EndInvoke(asyncResult)
        Return CType(results(0), String)
    End Function

    '''<remarks/>
    Public Overloads Sub AfiliadoAsync(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer)
        Me.AfiliadoAsync(sApl, sParametros, iOpc, Nothing)
    End Sub

    '''<remarks/>
    Public Overloads Sub AfiliadoAsync(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer, ByVal userState As Object)
        If (Me.AfiliadoOperationCompleted Is Nothing) Then
            Me.AfiliadoOperationCompleted = AddressOf Me.OnAfiliadoOperationCompleted
        End If
        Me.InvokeAsync("Afiliado", New Object() {sApl, sParametros, iOpc}, Me.AfiliadoOperationCompleted, userState)
    End Sub

    Private Sub OnAfiliadoOperationCompleted(ByVal arg As Object)
        If (Not (Me.AfiliadoCompletedEvent) Is Nothing) Then
            Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg, System.Web.Services.Protocols.InvokeCompletedEventArgs)
            RaiseEvent AfiliadoCompleted(Me, New AfiliadoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
        End If
    End Sub

    '''<remarks/>
    <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/WCompensar/AF/DatosVinculacion", RequestNamespace:="http://tempuri.org/WCompensar/AF", ResponseNamespace:="http://tempuri.org/WCompensar/AF", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)> _
    Public Function DatosVinculacion(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer) As String
        Dim results() As Object = Me.Invoke("DatosVinculacion", New Object() {sApl, sParametros, iOpc})
        Return CType(results(0), String)
    End Function

    '''<remarks/>
    Public Function BeginDatosVinculacion(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
        Return Me.BeginInvoke("DatosVinculacion", New Object() {sApl, sParametros, iOpc}, callback, asyncState)
    End Function

    '''<remarks/>
    Public Function EndDatosVinculacion(ByVal asyncResult As System.IAsyncResult) As String
        Dim results() As Object = Me.EndInvoke(asyncResult)
        Return CType(results(0), String)
    End Function

    '''<remarks/>
    Public Overloads Sub DatosVinculacionAsync(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer)
        Me.DatosVinculacionAsync(sApl, sParametros, iOpc, Nothing)
    End Sub

    '''<remarks/>
    Public Overloads Sub DatosVinculacionAsync(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer, ByVal userState As Object)
        If (Me.DatosVinculacionOperationCompleted Is Nothing) Then
            Me.DatosVinculacionOperationCompleted = AddressOf Me.OnDatosVinculacionOperationCompleted
        End If
        Me.InvokeAsync("DatosVinculacion", New Object() {sApl, sParametros, iOpc}, Me.DatosVinculacionOperationCompleted, userState)
    End Sub

    Private Sub OnDatosVinculacionOperationCompleted(ByVal arg As Object)
        If (Not (Me.DatosVinculacionCompletedEvent) Is Nothing) Then
            Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg, System.Web.Services.Protocols.InvokeCompletedEventArgs)
            RaiseEvent DatosVinculacionCompleted(Me, New DatosVinculacionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
        End If
    End Sub

    '''<remarks/>
    <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/WCompensar/AF/Empresa", RequestNamespace:="http://tempuri.org/WCompensar/AF", ResponseNamespace:="http://tempuri.org/WCompensar/AF", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)> _
    Public Function Empresa(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer) As String
        Dim results() As Object = Me.Invoke("Empresa", New Object() {pCodigoApp, sParametros, iOpc})
        Return CType(results(0), String)
    End Function

    '''<remarks/>
    Public Function BeginEmpresa(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
        Return Me.BeginInvoke("Empresa", New Object() {sApl, sParametros, iOpc}, callback, asyncState)
    End Function

    '''<remarks/>
    Public Function EndEmpresa(ByVal asyncResult As System.IAsyncResult) As String
        Dim results() As Object = Me.EndInvoke(asyncResult)
        Return CType(results(0), String)
    End Function

    '''<remarks/>
    Public Overloads Sub EmpresaAsync(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer)
        Me.EmpresaAsync(sApl, sParametros, iOpc, Nothing)
    End Sub

    '''<remarks/>
    Public Overloads Sub EmpresaAsync(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer, ByVal userState As Object)
        If (Me.EmpresaOperationCompleted Is Nothing) Then
            Me.EmpresaOperationCompleted = AddressOf Me.OnEmpresaOperationCompleted
        End If
        Me.InvokeAsync("Empresa", New Object() {sApl, sParametros, iOpc}, Me.EmpresaOperationCompleted, userState)
    End Sub

    Private Sub OnEmpresaOperationCompleted(ByVal arg As Object)
        If (Not (Me.EmpresaCompletedEvent) Is Nothing) Then
            Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg, System.Web.Services.Protocols.InvokeCompletedEventArgs)
            RaiseEvent EmpresaCompleted(Me, New EmpresaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
        End If
    End Sub

    '''<remarks/>
    <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/WCompensar/AF/ConsultaDatosBasicos", RequestNamespace:="http://tempuri.org/WCompensar/AF", ResponseNamespace:="http://tempuri.org/WCompensar/AF", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)> _
    Public Function ConsultaDatosBasicos(ByVal sApl As String, ByVal iOpc As Integer) As String
        Dim results() As Object = Me.Invoke("ConsultaDatosBasicos", New Object() {sApl, iOpc})
        Return CType(results(0), String)
    End Function

    '''<remarks/>
    Public Function BeginConsultaDatosBasicos(ByVal sApl As String, ByVal iOpc As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
        Return Me.BeginInvoke("ConsultaDatosBasicos", New Object() {sApl, iOpc}, callback, asyncState)
    End Function

    '''<remarks/>
    Public Function EndConsultaDatosBasicos(ByVal asyncResult As System.IAsyncResult) As String
        Dim results() As Object = Me.EndInvoke(asyncResult)
        Return CType(results(0), String)
    End Function

    '''<remarks/>
    Public Overloads Sub ConsultaDatosBasicosAsync(ByVal sApl As String, ByVal iOpc As Integer)
        Me.ConsultaDatosBasicosAsync(sApl, iOpc, Nothing)
    End Sub

    '''<remarks/>
    Public Overloads Sub ConsultaDatosBasicosAsync(ByVal sApl As String, ByVal iOpc As Integer, ByVal userState As Object)
        If (Me.ConsultaDatosBasicosOperationCompleted Is Nothing) Then
            Me.ConsultaDatosBasicosOperationCompleted = AddressOf Me.OnConsultaDatosBasicosOperationCompleted
        End If
        Me.InvokeAsync("ConsultaDatosBasicos", New Object() {sApl, iOpc}, Me.ConsultaDatosBasicosOperationCompleted, userState)
    End Sub

    Private Sub OnConsultaDatosBasicosOperationCompleted(ByVal arg As Object)
        If (Not (Me.ConsultaDatosBasicosCompletedEvent) Is Nothing) Then
            Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg, System.Web.Services.Protocols.InvokeCompletedEventArgs)
            RaiseEvent ConsultaDatosBasicosCompleted(Me, New ConsultaDatosBasicosCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
        End If
    End Sub

    '''<remarks/>
    <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/WCompensar/AF/Agenda", RequestNamespace:="http://tempuri.org/WCompensar/AF", ResponseNamespace:="http://tempuri.org/WCompensar/AF", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)> _
    Public Function Agenda(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer) As String
        Dim results() As Object = Me.Invoke("Agenda", New Object() {sApl, sParametros, iOpc})
        Return CType(results(0), String)
    End Function

    '''<remarks/>
    Public Function BeginAgenda(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
        Return Me.BeginInvoke("Agenda", New Object() {sApl, sParametros, iOpc}, callback, asyncState)
    End Function

    '''<remarks/>
    Public Function EndAgenda(ByVal asyncResult As System.IAsyncResult) As String
        Dim results() As Object = Me.EndInvoke(asyncResult)
        Return CType(results(0), String)
    End Function

    '''<remarks/>
    Public Overloads Sub AgendaAsync(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer)
        Me.AgendaAsync(sApl, sParametros, iOpc, Nothing)
    End Sub

    '''<remarks/>
    Public Overloads Sub AgendaAsync(ByVal sApl As String, ByVal sParametros As String, ByVal iOpc As Integer, ByVal userState As Object)
        If (Me.AgendaOperationCompleted Is Nothing) Then
            Me.AgendaOperationCompleted = AddressOf Me.OnAgendaOperationCompleted
        End If
        Me.InvokeAsync("Agenda", New Object() {sApl, sParametros, iOpc}, Me.AgendaOperationCompleted, userState)
    End Sub

    Private Sub OnAgendaOperationCompleted(ByVal arg As Object)
        If (Not (Me.AgendaCompletedEvent) Is Nothing) Then
            Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg, System.Web.Services.Protocols.InvokeCompletedEventArgs)
            RaiseEvent AgendaCompleted(Me, New AgendaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
        End If
    End Sub

    '''<remarks/>
    Public Shadows Sub CancelAsync(ByVal userState As Object)
        MyBase.CancelAsync(userState)
    End Sub
End Class

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")> _
Public Delegate Sub AfiliadoCompletedEventHandler(ByVal sender As Object, ByVal e As AfiliadoCompletedEventArgs)

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42"), _
 System.Diagnostics.DebuggerStepThroughAttribute(), _
 System.ComponentModel.DesignerCategoryAttribute("code")> _
Partial Public Class AfiliadoCompletedEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs

    Private results() As Object

    Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
        MyBase.New(exception, cancelled, userState)
        Me.results = results
    End Sub

    '''<remarks/>
    Public ReadOnly Property Result() As String
        Get
            Me.RaiseExceptionIfNecessary()
            Return CType(Me.results(0), String)
        End Get
    End Property
End Class

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")> _
Public Delegate Sub DatosVinculacionCompletedEventHandler(ByVal sender As Object, ByVal e As DatosVinculacionCompletedEventArgs)

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42"), _
 System.Diagnostics.DebuggerStepThroughAttribute(), _
 System.ComponentModel.DesignerCategoryAttribute("code")> _
Partial Public Class DatosVinculacionCompletedEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs

    Private results() As Object

    Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
        MyBase.New(exception, cancelled, userState)
        Me.results = results
    End Sub

    '''<remarks/>
    Public ReadOnly Property Result() As String
        Get
            Me.RaiseExceptionIfNecessary()
            Return CType(Me.results(0), String)
        End Get
    End Property
End Class

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")> _
Public Delegate Sub EmpresaCompletedEventHandler(ByVal sender As Object, ByVal e As EmpresaCompletedEventArgs)

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42"), _
 System.Diagnostics.DebuggerStepThroughAttribute(), _
 System.ComponentModel.DesignerCategoryAttribute("code")> _
Partial Public Class EmpresaCompletedEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs

    Private results() As Object

    Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
        MyBase.New(exception, cancelled, userState)
        Me.results = results
    End Sub

    '''<remarks/>
    Public ReadOnly Property Result() As String
        Get
            Me.RaiseExceptionIfNecessary()
            Return CType(Me.results(0), String)
        End Get
    End Property
End Class

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")> _
Public Delegate Sub ConsultaDatosBasicosCompletedEventHandler(ByVal sender As Object, ByVal e As ConsultaDatosBasicosCompletedEventArgs)

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42"), _
 System.Diagnostics.DebuggerStepThroughAttribute(), _
 System.ComponentModel.DesignerCategoryAttribute("code")> _
Partial Public Class ConsultaDatosBasicosCompletedEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs

    Private results() As Object

    Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
        MyBase.New(exception, cancelled, userState)
        Me.results = results
    End Sub

    '''<remarks/>
    Public ReadOnly Property Result() As String
        Get
            Me.RaiseExceptionIfNecessary()
            Return CType(Me.results(0), String)
        End Get
    End Property
End Class

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")> _
Public Delegate Sub AgendaCompletedEventHandler(ByVal sender As Object, ByVal e As AgendaCompletedEventArgs)

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42"), _
 System.Diagnostics.DebuggerStepThroughAttribute(), _
 System.ComponentModel.DesignerCategoryAttribute("code")> _
Partial Public Class AgendaCompletedEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs

    Private results() As Object

    Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
        MyBase.New(exception, cancelled, userState)
        Me.results = results
    End Sub

    '''<remarks/>
    Public ReadOnly Property Result() As String
        Get
            Me.RaiseExceptionIfNecessary()
            Return CType(Me.results(0), String)
        End Get
    End Property
End Class
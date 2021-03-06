﻿
Imports Bwl.Tools.RunMonitorPlatform

Public MustInherit Class CommonTaskCheck
    Implements ITaskCheck

    Protected _statusInfo As String = ""
    Protected _parametersInfo As String = ""
    Protected _lastCheck As New LastCall
    protected _checkInterval as integer =3
    protected _name as string

    Public ReadOnly Property CheckIntervalSeconds As Integer Implements ITaskCheck.CheckIntervalSeconds
            Get
            Return _checkInterval
        End Get
    End Property

    Public ReadOnly Property StatusInfo As String Implements ITaskCheck.StatusInfo
        Get
            Return _statusInfo
        End Get
    End Property

    Public ReadOnly Property LastCheck As LastCall Implements ITaskCheck.LastCheck
        Get
            Return _lastCheck
        End Get
    End Property

    Public ReadOnly Property Name As String Implements ITaskCheck.Name
        Get
            Return _name
        End Get
    End Property

    Public ReadOnly Property ParametersInfo As String Implements ITaskCheck.ParametersInfo
        Get
            Return _parametersInfo
        End Get
    End Property

    Public MustOverride Sub Check() Implements ITaskCheck.Check

    Public Sub New(name As String)
        _Name = name
    End Sub
End Class

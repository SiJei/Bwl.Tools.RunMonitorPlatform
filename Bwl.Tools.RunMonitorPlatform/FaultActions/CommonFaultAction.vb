
Public MustInherit Class CommonFaultAction
    Implements IFaultAction
    Protected _name As String

    Protected _lastCall As New LastCall

    Public Sub New(name As String, faultsToRun As Integer)
        _name = name
        _FaultsToRun = faultsToRun
    End Sub

    Public Property DelayBeforeActionSeconds As Single = 30 Implements IFaultAction.DelayBeforeActionSeconds

    Public Property FaultsToRun As Integer Implements IFaultAction.FaultsToRun

    Public Property CheckFaultsOnly As Boolean Implements IFaultAction.CheckFaultsOnly


    Public ReadOnly Property LastAttempt As LastCall Implements IFaultAction.LastAttempt
        Get
            Return _lastCall
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IFaultAction.Name
        Get
            Return _name
        End Get
    End Property

    Public MustOverride Sub Run() Implements IFaultAction.Run

End Class

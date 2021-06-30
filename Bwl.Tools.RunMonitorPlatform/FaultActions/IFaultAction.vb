
Public Interface IFaultAction
    ReadOnly Property Name As String
    ReadOnly Property LastAttempt As LastCall
    Property DelayBeforeActionSeconds As Single
    Property FaultsToRun As Integer
    Property CheckFaultsOnly As Boolean
    Sub Run()
End Interface
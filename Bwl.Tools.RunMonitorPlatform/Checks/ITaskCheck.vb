﻿Public Interface ITaskCheck
    ReadOnly Property Name As String
    ReadOnly Property LastCheck As LastCall
    ReadOnly Property CheckIntervalSeconds As Integer
    ReadOnly Property StatusInfo As String
    ReadOnly Property ParametersInfo As String
    Sub Check()
End Interface


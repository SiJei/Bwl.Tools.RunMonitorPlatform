﻿Public Class FaultActionsList
    Inherits List(Of IFaultAction)

    Public Function GetByType(type As Type) As IFaultAction()
        Dim list As New List(Of IFaultAction)
        For Each check In Me
            If TypeOf check Is Type Then list.Add(check)
        Next
        Return list.ToArray
    End Function

    Public Function GetByTypeSingle(type As Type) As IFaultAction
        Dim list = GetByType(type)
        If list.Count = 0 Then Throw New Exception("No objects with type " + type.Name + " found")
        If list.Count > 1 Then Throw New Exception("More than one objects with type " + type.Name + " found")
        Return list(0)
    End Function
End Class

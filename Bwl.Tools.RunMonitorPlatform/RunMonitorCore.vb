﻿Imports System.Threading
Imports Bwl.Framework

Public Class RunMonitorCore
    Private _logger As Logger
    Private _thread As Threading.Thread
    Public Property Tasks As New TasksList

    Public Sub New(logger As Logger)
        _logger = logger
    End Sub

    Public Sub Run()
        Do
            Try
                SingleCheck()
            Catch ex As Exception
                _logger.AddError("RunMonitorCore Run Error: " + ex.Message)
            End Try
            Thread.Sleep(1000)
        Loop
    End Sub

    Public Sub RunInThread()
        If _thread IsNot Nothing Then Throw New Exception
        _thread = New Thread(AddressOf Run)
        _thread.Start()
    End Sub

    Public Sub StopThread()
        If _thread Is Nothing Then Throw New Exception
        _thread.Abort()
    End Sub

    Public Function GetShortStatus() As String()()
        Dim states As New List(Of String())
        For Each task In Tasks.ToArray
            states.Add({task.ID, task.ShortName, task.State.ToString, task.ShortState})
        Next
        Return states.ToArray
    End Function

    Public Property AutomaticEnableTasks As Boolean = True

    Public Sub SingleCheck()
        Const timeToAutomaticEnable = 10
        For Each task In Tasks.ToArray
            If task.Checks.Count > 0 Then
                If task.State = TaskState.Disabled Then
                    If AutomaticEnableTasks Then
                        task.ExternalInfo = "Time to automatic enable: " + (timeToAutomaticEnable - (Now - task.Checks(0).LastCheck.Time).TotalMinutes).ToString("0.0") + "min"
                        Dim mins = (Now - task.Checks(0).LastCheck.Time).TotalMinutes
                        If mins > timeToAutomaticEnable And mins < 100000 Then
                            task.State = TaskState.Warning
                            task.ExternalInfo = ""
                        End If
                    End If
                    For Each check In task.Checks
                        Try
                            check.Check()
                        Catch ex As Exception
                        End Try
                    Next
                Else
                    _logger.AddDebug("Checking Task " + task.ID)

                    Dim maximumCheckFails As Integer = 0

                    For Each check In task.Checks
                        If (Now - check.LastCheck.Time).TotalSeconds > check.CheckIntervalSeconds Then
                            Try
                                check.Check()
                                check.LastCheck.SetNowOk("")
                                _logger.AddDebug("Check Task Ok " + task.ID + " - " + check.Name)
                            Catch ex As TaskCheckException
                                _logger.AddWarning("Check Failed " + task.ID + " - " + ex.MainMessage)
                                check.LastCheck.SetNowError("", ex.MainMessage)
                            Catch ex As Exception
                                _logger.AddError("Check Failed " + task.ID + " - " + ex.Message)
                                check.LastCheck.SetNowError("", ex.Message)
                            End Try
                        End If
                        If maximumCheckFails < check.LastCheck.FailedAttempts Then maximumCheckFails = check.LastCheck.FailedAttempts
                    Next

                    If maximumCheckFails = 0 Then task.State = TaskState.Ok Else task.State = TaskState.Warning

                    For Each action In task.FaultActions.Where(Function(f) maximumCheckFails >= f.FaultsToRun And If(f.CheckFaultsOnly, maximumCheckFails Mod f.FaultsToRun = 0, (Now - f.LastAttempt.Time).TotalSeconds > f.DelayBeforeActionSeconds))
                        task.State = TaskState.Fault
                        Try
                            _logger.AddMessage("Task FaultAction" + task.ID + " - " + action.Name)
                            action.Run()
                            action.LastAttempt.SetNowOk("")
                            _logger.AddDebug("Task FaultAction" + task.ID + " - " + action.Name + " - ok")
                            _logger.AddDebug(action.LastAttempt.Message)
                            _logger.AddDebug(action.LastAttempt.ErrorText)
                        Catch ex As FaultActionException
                            action.LastAttempt.SetNowError("", ex.MainMessage)
                            _logger.AddWarning("Task FaultAction Failed " + task.ID + " - " + ex.MainMessage)
                            _logger.AddDebug(action.LastAttempt.Message)
                            _logger.AddDebug(action.LastAttempt.ErrorText)
                        Catch ex As Exception
                            action.LastAttempt.SetNowError("", ex.Message)
                            _logger.AddWarning("Task FaultAction Failed " + task.ID + " - " + ex.Message)
                            _logger.AddDebug(action.LastAttempt.Message)
                            _logger.AddDebug(action.LastAttempt.ErrorText)
                        End Try
                    Next
                End If
            End If
        Next
        Thread.Sleep(1)
    End Sub
End Class

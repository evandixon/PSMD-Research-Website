Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class Ability
    <DatabaseGenerated(DatabaseGeneratedOption.None), Key> <Required> Public Property ID As Integer
    <Required> Public Property Name As String
End Class

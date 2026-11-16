Imports System
Imports System.Windows.Forms
Namespace StudentRegistrationSystem
    Friend Module Program
        <STAThread>
        Public Sub Main()
            ApplicationConfiguration.Initialize()
            Application.Run(New LoginForm())
        End Sub
    End Module
End Namespace

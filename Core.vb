Imports System.Security.Cryptography
Imports MySqlConnector
Namespace StudentRegistrationSystem
    Public Module Db
        Public Function OpenConnection() As MySqlConnection
            Dim password = Environment.GetEnvironmentVariable("STUDENT_DB_PASSWORD")
            If password Is Nothing Then password = ""
            Dim builder As New MySqlConnectionStringBuilder With {
                .Server = Environment.GetEnvironmentVariable("STUDENT_DB_HOST"),
                .Port = 3306,
                .Database = "student_registration_db",
                .UserID = "root",
                .Password = password,
                .SslMode = MySqlSslMode.None
            }
            If String.IsNullOrWhiteSpace(builder.Server) Then builder.Server = "localhost"
            Dim connection As New MySqlConnection(builder.ConnectionString)
            connection.Open()
            Return connection
        End Function
    End Module
    Public Module CurrentStudent
        Public Property UserId As Integer
        Public Property StudentNo As String = ""
        Public Property FullName As String = ""
        Public Sub Logout()
            UserId = 0
            StudentNo = ""
            FullName = ""
        End Sub
    End Module
    Public Module Passwords
        Private Const Iterations As Integer = 120000
        Public Function Hash(password As String) As String
            Dim salt(15) As Byte
            RandomNumberGenerator.Fill(salt)
            Dim hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32)
            Return Convert.ToBase64String(salt) & ":" & Convert.ToBase64String(hashBytes)
        End Function
        Public Function Verify(password As String, stored As String) As Boolean
            Try
                Dim parts = stored.Split(":"c)
                If parts.Length <> 2 Then Return False
                Dim salt = Convert.FromBase64String(parts(0))
                Dim expected = Convert.FromBase64String(parts(1))
                Dim actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, expected.Length)
                Return CryptographicOperations.FixedTimeEquals(actual, expected)
            Catch
                Return False
            End Try
        End Function
    End Module
End Namespace

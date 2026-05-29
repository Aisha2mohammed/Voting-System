Imports System.Security.Cryptography
Imports System.Text

Module PasswordHelper

    ' Computes the SHA-256 hash of the supplied plain-text password and
    ' returns it as a lowercase hexadecimal string (64 characters).
    '
    ' All stored passwords in the `users` table must be hashed with this
    ' function before being written, and verified via VerifyPassword on login.
    Public Function HashPassword(plain As String) As String
        If plain Is Nothing Then
            plain = String.Empty
        End If

        Using sha As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes(plain))
            Dim sb As New StringBuilder(bytes.Length * 2)

            For i As Integer = 0 To bytes.Length - 1
                sb.Append(bytes(i).ToString("x2"))
            Next

            Return sb.ToString()
        End Using
    End Function

    ' Compares a plain-text password entered by the user against the stored
    ' SHA-256 hash. Returns True only when they match.
    Public Function VerifyPassword(plain As String, storedHash As String) As Boolean
        If storedHash Is Nothing Then Return False

        Dim hashOfInput As String = HashPassword(plain)
        Return String.Equals(hashOfInput, storedHash, StringComparison.OrdinalIgnoreCase)
    End Function

End Module

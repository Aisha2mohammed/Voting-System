Imports MySql.Data.MySqlClient

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim username As String = TextBox1.Text.Trim()
        Dim password As String = TextBox2.Text.Trim()

        If username = "" Or password = "" Then
            MessageBox.Show("Please enter username and password")
            Exit Sub
        End If

        Dim connString As String = DatabaseHelper.ConnString

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                ' Fetch the stored role AND password hash for the given username.
                ' We then verify the entered password against the stored hash
                ' with PasswordHelper.VerifyPassword (SHA-256).
                Dim query As String = "SELECT role, password FROM users WHERE username=@user"
                Using cmd As New MySqlCommand(query, conn)

                    cmd.Parameters.AddWithValue("@user", username)

                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim role As String = reader("role").ToString().ToLower()
                            Dim storedHash As String = reader("password").ToString()

                            If PasswordHelper.VerifyPassword(password, storedHash) Then
                                If role = "admin" Then
                                    MessageBox.Show("Welcome Admin!")

                                    Dim adminForm As New Form3()
                                    adminForm.loggedUsername = username
                                    adminForm.Show()
                                    Me.Hide()

                                ElseIf role = "user" Then
                                    MessageBox.Show("Welcome User!")

                                    ' OPEN FORM5 INSTEAD
                                    Dim f As New Form5()
                                    f.loggedUser = username
                                    f.Show()
                                    Me.Hide()

                                Else
                                    MessageBox.Show("Unknown role detected!")
                                End If
                            Else
                                MessageBox.Show("Invalid Username or Password")
                            End If
                        Else
                            MessageBox.Show("Invalid Username or Password")
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim registerForm As New Form2()
        registerForm.Show()
        Me.Hide()
    End Sub

End Class

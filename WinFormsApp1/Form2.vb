Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Form2
    Private Sub Label3_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim loginForm As New Form1()
        loginForm.Show()
        Me.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim fullname As String = TextBox1.Text.Trim()
        Dim username As String = TextBox2.Text.Trim()
        Dim password As String = TextBox3.Text.Trim()

        If fullname = "" Or username = "" Or password = "" Then
            MessageBox.Show("Please fill all fields")
            Exit Sub
        End If

        Dim connString As String = DatabaseHelper.ConnString

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                ' 🔍 Check if username exists
                Dim checkQuery As String = "SELECT COUNT(*) FROM users WHERE username=@user"
                Using checkCmd As New MySqlCommand(checkQuery, conn)
                    checkCmd.Parameters.AddWithValue("@user", username)

                    Dim exists As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                    If exists > 0 Then
                        MessageBox.Show("Username already exists. Try another.")
                        Exit Sub
                    End If
                End Using

                ' ✅ Insert with fullname + role
                Dim insertQuery As String = "INSERT INTO users (fullname, username, password, role) VALUES (@fullname, @user, @pass, @role)"
                Using insertCmd As New MySqlCommand(insertQuery, conn)
                    insertCmd.Parameters.AddWithValue("@fullname", fullname)
                    insertCmd.Parameters.AddWithValue("@user", username)
                    insertCmd.Parameters.AddWithValue("@pass", PasswordHelper.HashPassword(password))
                    insertCmd.Parameters.AddWithValue("@role", "user")

                    insertCmd.ExecuteNonQuery()
                    MessageBox.Show("Registered Successfully!")

                    Dim loginForm As New Form1()
                    loginForm.Show()
                    Me.Hide()
                End Using

            End Using

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub
End Class
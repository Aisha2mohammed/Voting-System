Imports MySql.Data.MySqlClient

Public Class Form3

    ' 🎨 Base color and active color
    Private baseColor As Color = Color.DarkSlateGray
    Private activeColor As Color = Color.Teal

    ' 👤 Logged in username
    Public loggedUsername As String

    ' 🧠 Show only one panel at a time
    Private Sub ShowPanel(panel As Panel)
        Panel3.Visible = False
        Panel4.Visible = False
        Panel5.Visible = False

        panel.Visible = True
        panel.BringToFront()
    End Sub

    ' 🎨 Reset all buttons to base color
    Private Sub ResetButtons()
        Button1.BackColor = baseColor
        Button2.BackColor = baseColor
        Button3.BackColor = baseColor
    End Sub

    ' 📊 LOAD USERS FROM DATABASE
    Private Sub LoadUsers()
        Dim connString As String = DatabaseHelper.ConnString

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim query As String = "SELECT fullname AS 'Full Name', username AS 'Username', role AS 'Role' FROM users WHERE role = 'user'"

                Using cmd As New MySqlCommand(query, conn)
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim table As New DataTable()

                    adapter.Fill(table)
                    DataGridView1.DataSource = table
                End Using
            End Using

            ' 🎨 Styling
            With DataGridView1
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                .EnableHeadersVisualStyles = False

                .ColumnHeadersDefaultCellStyle.BackColor = Color.Teal
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11, FontStyle.Bold)

                .DefaultCellStyle.BackColor = Color.White
                .DefaultCellStyle.ForeColor = Color.Black
                .DefaultCellStyle.SelectionBackColor = Color.Teal
                .DefaultCellStyle.SelectionForeColor = Color.White

                .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray
                .RowTemplate.Height = 30
                .RowHeadersVisible = False
            End With

        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message)
        End Try
    End Sub

    ' 🧑‍💼 Manage Users - Button1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ResetButtons()
        Button1.BackColor = activeColor
        ShowPanel(Panel3)
        LoadUsers()
    End Sub

    ' 🗳 Manage Votes - Button2
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ResetButtons()
        Button2.BackColor = activeColor
        ShowPanel(Panel4)
        LoadVotes()
    End Sub

    ' 👤 Profile - Button3
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ResetButtons()
        Button3.BackColor = activeColor
        ShowPanel(Panel5)
        LoadProfile()
    End Sub

    ' 🚪 Logout - Button4
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim loginForm As New Form1()
        loginForm.Show()
        Me.Close()
    End Sub

    ' 🚀 Form Load
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Panel2.Visible = True
        Panel3.Visible = False
        Panel4.Visible = False
        Panel5.Visible = False

        ResetButtons()
        Button1.BackColor = activeColor
        ShowPanel(Panel3)
        LoadUsers()

        Button1.ForeColor = Color.White
        Button2.ForeColor = Color.White
        Button3.ForeColor = Color.White
        Button4.ForeColor = Color.Black
    End Sub

    ' ===============================
    ' 🧠 VOTING SYSTEM CODE
    ' ===============================

    ' 📥 Load Votes
    Private Sub LoadVotes()
        Dim connString As String = DatabaseHelper.ConnString

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim query As String = "SELECT * FROM votes"
                Dim cmd As New MySqlCommand(query, conn)
                Dim reader = cmd.ExecuteReader()

                ListBox1.Items.Clear()

                While reader.Read()
                    ListBox1.Items.Add(reader("id").ToString() & " - " & reader("description").ToString())
                End While
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading votes: " & ex.Message)
        End Try
    End Sub

    ' 📊 Load Candidates
    Private Sub LoadCandidates(voteId As Integer)
        Dim connString As String = DatabaseHelper.ConnString

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim query As String = "SELECT id, candidate_name AS 'Candidate', vote_count AS 'Votes' FROM candidates WHERE vote_id=@id"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", voteId)

                Dim adapter As New MySqlDataAdapter(cmd)
                Dim table As New DataTable()

                adapter.Fill(table)
                DataGridView2.DataSource = table
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading candidates: " & ex.Message)
        End Try
    End Sub

    ' ➕ Add Vote - Button5
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim desc As String = TextBox1.Text.Trim()

        If desc = "" Then
            MessageBox.Show("Enter vote description")
            Exit Sub
        End If

        Dim connString As String = DatabaseHelper.ConnString

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim query As String = "INSERT INTO votes (description) VALUES (@desc)"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@desc", desc)
                cmd.ExecuteNonQuery()
            End Using

            TextBox1.Clear()
            LoadVotes()
        Catch ex As Exception
            MessageBox.Show("Error adding vote: " & ex.Message)
        End Try
    End Sub

    ' 🖱 Select Vote → Load its Candidates
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedItem Is Nothing Then Exit Sub

        Dim selected As String = ListBox1.SelectedItem.ToString()
        Dim id As Integer = Convert.ToInt32(selected.Split("-")(0).Trim())

        LoadCandidates(id)
    End Sub

    ' ➕ Add Candidate - Button6
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If ListBox1.SelectedItem Is Nothing Then
            MessageBox.Show("Select a vote first")
            Exit Sub
        End If

        Dim name As String = TextBox2.Text.Trim()

        If name = "" Then
            MessageBox.Show("Enter candidate name")
            Exit Sub
        End If

        Dim selected As String = ListBox1.SelectedItem.ToString()
        Dim voteId As Integer = Convert.ToInt32(selected.Split("-")(0).Trim())

        Dim connString As String = DatabaseHelper.ConnString

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim query As String = "INSERT INTO candidates (vote_id, candidate_name) VALUES (@vid, @name)"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@vid", voteId)
                cmd.Parameters.AddWithValue("@name", name)
                cmd.ExecuteNonQuery()
            End Using

            TextBox2.Clear()
            LoadCandidates(voteId)
        Catch ex As Exception
            MessageBox.Show("Error adding candidate: " & ex.Message)
        End Try
    End Sub

    ' ❌ Delete Candidate - Button7
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If DataGridView2.CurrentRow Is Nothing Then Exit Sub

        Dim id As Integer = DataGridView2.CurrentRow.Cells("id").Value

        Dim connString As String = DatabaseHelper.ConnString

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim query As String = "DELETE FROM candidates WHERE id=@id"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.ExecuteNonQuery()
            End Using

            ListBox1_SelectedIndexChanged(Nothing, Nothing)
        Catch ex As Exception
            MessageBox.Show("Error deleting candidate: " & ex.Message)
        End Try
    End Sub

    ' ===============================
    ' 👤 PROFILE CODE (NEW)
    ' ===============================

    ' 📥 Load Profile
    Private Sub LoadProfile()
        Dim connString As String = DatabaseHelper.ConnString

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim query As String = "SELECT fullname, username FROM users WHERE username=@user"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@user", loggedUsername)

                Dim reader = cmd.ExecuteReader()

                If reader.Read() Then
                    Label3.Text = reader("fullname").ToString()
                    Label5.Text = reader("username").ToString()
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading profile: " & ex.Message)
        End Try
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub

End Class
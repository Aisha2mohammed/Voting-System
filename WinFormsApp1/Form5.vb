Imports MySql.Data.MySqlClient

Public Class Form5

    ' 🔐 store logged in username
    Public loggedUser As String

    ' ===============================
    ' 🧠 PANEL SWITCHING
    ' ===============================
    Private Sub ShowPanel(panel As Panel)
        Panel3.Visible = False ' Vote panel
        Panel4.Visible = False ' Profile panel

        panel.Visible = True
    End Sub

    ' ===============================
    ' 📥 LOAD VOTES INTO LISTBOX
    ' ===============================
    Private Sub LoadVotes()

        Dim connString As String = DatabaseHelper.ConnString

        Using conn As New MySqlConnection(connString)
            conn.Open()

            Dim cmd As New MySqlCommand("SELECT * FROM votes", conn)
            Dim reader = cmd.ExecuteReader()

            ListBox1.Items.Clear()

            While reader.Read()
                ListBox1.Items.Add(reader("id").ToString() & " - " & reader("description").ToString())
            End While

        End Using

    End Sub

    ' ===============================
    ' 📊 LOAD CANDIDATES
    ' ===============================
    Private Sub LoadCandidates(voteId As Integer, description As String)

        Label2.Text = description

        Dim connString As String = DatabaseHelper.ConnString

        Using conn As New MySqlConnection(connString)
            conn.Open()

            Dim query As String = "SELECT id, candidate_name AS 'Candidate', vote_count AS 'Votes' FROM candidates WHERE vote_id=@id"

            Using cmd As New MySqlCommand(query, conn)

                cmd.Parameters.AddWithValue("@id", voteId)

                Dim adapter As New MySqlDataAdapter(cmd)
                Dim table As New DataTable()

                adapter.Fill(table)
                DataGridView1.DataSource = table

                ' 🔥 hide ID column
                If DataGridView1.Columns.Contains("id") Then
                    DataGridView1.Columns("id").Visible = False
                End If

                DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                DataGridView1.RowHeadersVisible = False

            End Using

        End Using

    End Sub

    ' ===============================
    ' 👤 LOAD PROFILE (NEW)
    ' ===============================
    Private Sub LoadProfile()

        Dim connString As String = DatabaseHelper.ConnString

        Using conn As New MySqlConnection(connString)
            conn.Open()

            Dim query As String = "SELECT fullname, username FROM users WHERE username=@user"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@user", loggedUser)

                Dim reader = cmd.ExecuteReader()

                If reader.Read() Then
                    Label5.Text = reader("fullname").ToString()   ' FULLNAME VALUE
                    Label6.Text = reader("username").ToString()   ' USERNAME VALUE
                End If

            End Using

        End Using

    End Sub

    ' ===============================
    ' 🖱 SELECT VOTE
    ' ===============================
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged

        If ListBox1.SelectedItem Is Nothing Then Exit Sub

        Dim selected As String = ListBox1.SelectedItem.ToString()
        Dim parts = selected.Split("-")

        Dim voteId As Integer = Convert.ToInt32(parts(0).Trim())
        Dim desc As String = parts(1).Trim()

        LoadCandidates(voteId, desc)

    End Sub

    ' ===============================
    ' 🗳 VOTE BUTTON
    ' ===============================
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        If DataGridView1.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a candidate")
            Exit Sub
        End If

        If ListBox1.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a vote first")
            Exit Sub
        End If

        Dim voteId As Integer = Convert.ToInt32(ListBox1.SelectedItem.ToString().Split("-")(0).Trim())
        Dim candidateId As Integer = DataGridView1.CurrentRow.Cells("id").Value

        Dim connString As String = DatabaseHelper.ConnString

        Using conn As New MySqlConnection(connString)
            conn.Open()

            ' 🔍 check duplicate vote
            Dim checkQuery As String = "SELECT COUNT(*) FROM user_votes WHERE username=@user AND vote_id=@vote"
            Using checkCmd As New MySqlCommand(checkQuery, conn)

                checkCmd.Parameters.AddWithValue("@user", loggedUser)
                checkCmd.Parameters.AddWithValue("@vote", voteId)

                If Convert.ToInt32(checkCmd.ExecuteScalar()) > 0 Then
                    MessageBox.Show("You already voted!")
                    Exit Sub
                End If

            End Using

            ' ✅ insert vote
            Dim insertQuery As String = "INSERT INTO user_votes (username, vote_id) VALUES (@user, @vote)"
            Using insertCmd As New MySqlCommand(insertQuery, conn)

                insertCmd.Parameters.AddWithValue("@user", loggedUser)
                insertCmd.Parameters.AddWithValue("@vote", voteId)
                insertCmd.ExecuteNonQuery()

            End Using

            ' ➕ increase vote count
            Dim updateQuery As String = "UPDATE candidates SET vote_count = vote_count + 1 WHERE id=@cid"
            Using updateCmd As New MySqlCommand(updateQuery, conn)

                updateCmd.Parameters.AddWithValue("@cid", candidateId)
                updateCmd.ExecuteNonQuery()

            End Using

        End Using

        MessageBox.Show("Vote submitted successfully!")

        ListBox1_SelectedIndexChanged(Nothing, Nothing)

    End Sub

    ' ===============================
    ' 🔘 SIDEBAR BUTTONS
    ' ===============================

    ' 🗳 Vote
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ShowPanel(Panel3)
    End Sub

    ' 👤 Profile
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ShowPanel(Panel4)
        LoadProfile()
    End Sub

    ' 🚪 Logout
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to logout?", "Logout",
                                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Dim loginForm As New Form1()
            loginForm.Show()
            Me.Close()
        End If

    End Sub

    ' ===============================
    ' 🚀 FORM LOAD
    ' ===============================
    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ShowPanel(Panel3) ' default = vote
        LoadVotes()

    End Sub

End Class
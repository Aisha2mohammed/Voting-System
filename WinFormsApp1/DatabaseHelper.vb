Module DatabaseHelper

    ' Centralized database connection string.
    ' Previously this string was duplicated in every form (Form1, Form2, Form3, Form5),
    ' which made it hard to maintain and easy to introduce inconsistencies.
    ' All forms now reference DatabaseHelper.ConnString.

    Public Const ConnString As String = "server=localhost;user id=root;password=;database=login_db"

End Module

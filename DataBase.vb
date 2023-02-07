Imports System.Data.SqlClient
Imports System.IO

Public Class DataBase
    Private Sub database()
        coun.Open()
        ComboBox1.Items.Clear()
        Dim cmd As New SqlCommand("select * from sysdatabases order by name", coun)
        Dim myreadar As SqlDataReader = cmd.ExecuteReader
        While myreadar.Read
            ComboBox1.Items.Add(myreadar(0))
            ComboBox1.Text = "Select Database"
        End While
        coun.Close()
    End Sub
    Sub folder()
        Dim folderName As String = "D:\Data"
        Dim path As String = System.IO.Path.Combine(Directory.GetCurrentDirectory(), folderName)

        If Not Directory.Exists(path) Then
            Directory.CreateDirectory(path)
            Console.WriteLine("Folder created successfully.")
        Else
            Exit Sub
        End If
    End Sub
    Private Sub LoadDatabasesIntoDataGridViewWithID()
        coun.Open()
        DataGridView1.Rows.Clear()
        Dim cmd As New SqlCommand("SELECT [name] ,database_id FROM sys.databases Where [name] Not In('master','model','msdb','tempdb','ReportServer','ReportServerTempDB')", coun)
        Dim myreader As SqlDataReader = cmd.ExecuteReader
        Dim id As Integer = 1
        While myreader.Read
            Dim row As String() = New String() {id.ToString, myreader("name").ToString}
            DataGridView1.Rows.Add(row)
            id += 1
        End While
        coun.Close()
    End Sub

    Private Sub LoadtabelsIntoDataGridViewWithID()

        coun.Open()
        DataGridView3.Rows.Clear()
        Dim cmd As New SqlCommand("SELECT name, ROW_NUMBER() OVER (ORDER BY name) AS [ID] FROM sys.tables", coun)
        Dim myreader As SqlDataReader = cmd.ExecuteReader
        Dim id As Integer = 1
        While myreader.Read
            Dim row As String() = New String() {id.ToString, myreader("name").ToString}
            DataGridView3.Rows.Add(row)
            id += 1
        End While
        coun.Close()
    End Sub


 

    Private Sub DataBase_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'database()
        LoadDatabasesIntoDataGridViewWithID()
        folder()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        BackupDatabases()
        'If ComboBox1.Text = "Select Database" Then
        '    MessageBox.Show("Please Select Database")
        '    Return
        'Else
        '    Dim dbName As String = ComboBox1.SelectedItem.ToString()
        '    SaveFileDialog1.FileName = dbName
        '    If SaveFileDialog1.ShowDialog = DialogResult.OK Then
        '        Dim folder As String
        '        folder = SaveFileDialog1.FileName
        '        Dim cmd2 As New SqlCommand("BACKUP Database [" & dbName & "] To disk='" & folder & "'", coun)
        '        coun.Open()
        '        cmd2.ExecuteNonQuery()
        '        coun.Close()
        '        MessageBox.Show("database backed up succesfully")
        '    Else
        '        MessageBox.Show("Please save Database")
        '    End If
        'End If


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        delete()
        'If ComboBox1.Text = "Select Database" Then
        '    MessageBox.Show("Please Select Database")
        '    Return
        'Else
        '    Dim cmd3 As New SqlCommand("Drop Database " & ComboBox1.Text & "", coun)
        '    coun.Open()
        '    cmd3.ExecuteNonQuery()
        '    coun.Close()
        '    MessageBox.Show("database deleted succesfully")
        '    database()

        'End If
    End Sub
    Sub delete()
        coun.Open()

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            If DataGridView1.Rows(i).Cells(2).Value = True Then
                Dim cmd3 As New SqlCommand("DROP DATABASE " & DataGridView1.Rows(i).Cells(1).Value & "", coun)
                cmd3.ExecuteNonQuery()
            End If
        Next
        coun.Close()
        MessageBox.Show("Databases deleted successfully")
        LoadDatabasesIntoDataGridViewWithID()
    End Sub

    Private Sub BackupDatabases()
        coun.Open()

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            If DataGridView1.Rows(i).Cells(2).Value = True Then
                Dim cmd2 As New SqlCommand("BACKUP Database [" & DataGridView1.Rows(i).Cells(1).Value & "] TO DISK='" & pathdata & "\\" & DataGridView1.Rows(i).Cells(1).Value & ".bak'", coun)
                cmd2.ExecuteNonQuery()
            End If
        Next
        coun.Close()
        MessageBox.Show("Databases backed up successfully")
        LoadDatabasesIntoDataGridViewWithID()
        loadfiles()
        OpenDataFolder()
    End Sub
    Private Sub tablesclean()

        coun.Open()

        For i As Integer = 0 To DataGridView3.Rows.Count - 1
            If DataGridView3.Rows(i).Cells(2).Value = True Then
                Dim command As New SqlCommand("DELETE FROM " & DataGridView3.Rows(i).Cells(1).Value & ";", coun)
                command.ExecuteNonQuery()
            End If
        Next
        coun.Close()
        MessageBox.Show("All rows deleted from tables")
        LoadtabelsIntoDataGridViewWithID()
    End Sub
    Sub restore()
        coun.Open()

        For i As Integer = 0 To DataGridView2.Rows.Count - 1
            If DataGridView2.Rows(i).Cells(2).Value = True Then
                Dim cmd4 As New SqlCommand("Select * from sysdatabases where name = '" & DataGridView2.Rows(i).Cells(1).Value & "'", coun)
                Dim myreader2 As SqlDataReader = cmd4.ExecuteReader
                If myreader2.Read Then
                    MessageBox.Show("Database existe " & DataGridView2.Rows(i).Cells(1).Value)
                Else
                    Dim cmd3 As New SqlCommand("Restore database " & DataGridView2.Rows(i).Cells(1).Value & " from DISK='" & pathdata & "\\" & DataGridView2.Rows(i).Cells(1).Value & ".bak'", coun)
                    myreader2.Close()
                    cmd3.ExecuteNonQuery()
                End If
                myreader2.Close()
            End If

        Next

        coun.Close()

        MessageBox.Show("database restored succesfully")
        LoadDatabasesIntoDataGridViewWithID()
    End Sub
    Sub OpenDataFolder()
        'Code to perform actions here

        'Open the D:\Data folder after the subroutine is finished
        Shell("explorer.exe D:\Data", vbNormalFocus)
    End Sub


    Dim pathdata As String = "D:\Data"
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        restore()
        'Dim dbname As String
        'dbname = InputBox("Input Database name you want to be restore", "Database Name")
        'Dim cmd4 As New SqlCommand("Select * from sysdatabases where name = '" & dbname & "'", coun)
        'coun.Open()
        'Dim myreader2 As SqlDataReader = cmd4.ExecuteReader
        'If myreader2.Read Then
        '    MessageBox.Show("Database existe")
        'Else
        '    coun.Close()
        '    If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
        '        Dim cmd3 As New SqlCommand("Restore database " & dbname & " from disk='" & OpenFileDialog1.FileName & "'", coun)
        '        coun.Open()
        '        cmd3.ExecuteReader()
        '        coun.Close()
        '        database()
        '        MessageBox.Show("database restored succesfully")
        '    Else

        '        MessageBox.Show("database Select succesfully you want to restore")
        '    End If
        'End If

    End Sub
    Sub loadfiles()
        DataGridView2.Rows.Clear()
        Dim files() As String = IO.Directory.GetFiles(pathdata, "*.bak")
        Dim dt As New DataTable()
        For i As Integer = 0 To files.Length - 1
            DataGridView2.Rows.Add(i, System.IO.Path.GetFileNameWithoutExtension(files(i)))
        Next
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        loadfiles()
    End Sub


    Private Sub SaveFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles SaveFileDialog1.FileOk

    End Sub

    Private Sub DataGridView2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
LoadtabelsIntoDataGridViewWithID
    End Sub

    Private Sub DataGridView1_CellContentClick_1(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub DataGridView2_CellContentClick_1(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub DataGridView1_CellContentClick_2(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button6_Click_1(sender As Object, e As EventArgs) Handles Button6.Click
        tablesclean()
    End Sub

    Private Sub DataGridView1_CellContentClick_3(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub DataGridView3_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView3.CellContentClick

    End Sub

    Private Sub DataGridView2_CellContentClick_2(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick

    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox1.Text = "table1" Then
            For Each row As DataGridViewRow In DataGridView1.Rows
                If ComboBox2.Text = "CHECK ALL" Then
                    row.Cells("Column3").Value = True
                ElseIf ComboBox2.Text = "UNCHECKED ALL" Then
                    row.Cells("Column3").Value = False
                End If

            Next
        ElseIf ComboBox1.Text = "table2" Then
            For Each row As DataGridViewRow In DataGridView2.Rows
                If ComboBox2.Text = "CHECK ALL" Then
                    row.Cells("DataGridViewCheckBoxColumn1").Value = True
                ElseIf ComboBox2.Text = "UNCHECKED ALL" Then
                    row.Cells("DataGridViewCheckBoxColumn1").Value = False
                End If

            Next
        ElseIf ComboBox1.Text = "table3" Then
            For Each row As DataGridViewRow In DataGridView3.Rows
                If ComboBox2.Text = "CHECK ALL" Then
                    row.Cells("DataGridViewCheckBoxColumn2").Value = True
                ElseIf ComboBox2.Text = "UNCHECKED ALL" Then
                    row.Cells("DataGridViewCheckBoxColumn2").Value = False
                End If

            Next
        End If
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.Text = "table1" Then
            LoadDatabasesIntoDataGridViewWithID()
        ElseIf ComboBox1.Text = "table2" Then
            loadfiles()
        ElseIf ComboBox1.Text = "table3" Then
            LoadtabelsIntoDataGridViewWithID()
        End If
    End Sub
End Class
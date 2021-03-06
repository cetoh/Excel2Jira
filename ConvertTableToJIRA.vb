
' https://stackoverflow.com/q/11109832/5411712

Function IsInArray(stringToBeFound As String, arr As Variant) As Boolean
  IsInArray = (UBound(Filter(arr, stringToBeFound)) > -1)
End Function


' initial inspiration : https://gist.github.com/DBremen/0ba67c6ec894ee581d98
Sub ConvertToJiraTable()
    Dim workingRange As Range, currCol As Range, currRow As Range
    Dim rowIndex As Long, colIndex As Long
    Dim output As String, boldedCellStr As String, cellStr As String
    Dim cellWordsArr As Variant

    'Dim statusHash As Dictionary
    Dim cb As DataObject
    ' set up clipboard
    Set cb = New DataObject

    rowIndex = 1

    ' declare an array of strings for BOLD keywords
    Dim boldArray(20) As String
    boldArray(0) = "Click"
    boldArray(1) = "Navigate"
    boldArray(2) = "Open"
    boldArray(3) = "Save"
    boldArray(4) = "Load"
    boldArray(5) = "Delete"
    boldArray(6) = "Hello"
    boldArray(7) = "Set"
    boldArray(8) = "Fill"
    boldArray(9) = "Enter"
    boldArray(10) = "Drag"
    boldArray(11) = "Type"

    ' Make sure macro just works in the background
    Application.ScreenUpdating = False

    Set workingRange = Range("A1").CurrentRegion
    For Each currRow In workingRange.Rows
        colIndex = 1
        For Each currCol In currRow.Columns

            cellStr = currCol.Value

            ' according to https://techsupport.osisoft.com/Troubleshooting/KB/KB01372
            ' There can be weird non ASCII spaces due to copy-pasting from an HTML source
            ' The werid characters mess with the Split() function.
            ' The character is Char(160) or "%C2%A0"
            ' this replacement converts the space character to ASCII
            cellStr = Replace(cellStr, CStr(Chr(160)), CStr(Chr(32)))
            ' then use the space character as the delimiter
            ' because JIRA only formats words separated by space.
            cellWordsArr = Split(cellStr, " ")

            For i = LBound(cellWordsArr) To UBound(cellWordsArr)
                ' Debug.Print (cellWordsArr(i))

                ' the empty string exists in cellWordsArr
                ' avoid empty string and match the keywords
                If cellWordsArr(i) <> "" _
                    And IsInArray(CStr(cellWordsArr(i)), boldArray) Then
                    cellWordsArr(i) = "*" & cellWordsArr(i) & "*"
                End If

                ' Debug.Print (cellWordsArr(i))
            Next i

            boldedCellStr = Join(cellWordsArr)

            If (boldedCellStr = "") Then boldedCellStr = " "
            If rowIndex = 1 Then ' if it's the first row do the "||"
                'MsgBox "rowIndex : " & rowIndex & vbCrLf & "colIndex : " & colIndex
                output = output & "||" & boldedCellStr
            Else
                output = output & "|" & boldedCellStr
            End If
            colIndex = colIndex + 1
        Next currCol

        rowIndex = rowIndex + 1
        '====================================================='
        '============fix the end of each row=================='
        '====================================================='
        If rowIndex = 1 Then ' if it's the first row do the "||"
            'MsgBox "rowIndex : " & rowIndex & vbCrLf & "colIndex : " & colIndex
            output = output & " " & "||"
        Else
            output = output & " " & "|"
        End If
        ' put a new line
        output = output & vbCrLf
        '====================================================='

    Next currRow
    cb.SetText output
    cb.PutInClipboard
    MsgBox ("THE FOLLOWING HAS BEEN PUT INTO YOUR CLIPBOARD :" _
            & vbCrLf & vbCrLf & output)
End Sub

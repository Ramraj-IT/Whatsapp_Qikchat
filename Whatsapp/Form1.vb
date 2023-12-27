Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports Newtonsoft.Json


Public Class Form1
    Dim URL As String
    Dim tmpp As String
    Dim webClient As New System.Net.WebClient

    Dim ConDB As SqlConnection

    Dim ServerName As String = "192.168.0.30"
    Dim DbName As String = "RRLIVE"
    Dim Userid As String = "admin"
    Dim UserPwd As String = "AdminG5366"
    Dim CompanyCode As String = "RR"


    Dim RptName As String = ""
    Dim PdfName As String = ""
    Dim Location As String = ""
    Dim TDate As String = ""





    Public ConstrDB As String = ""
    Public Function customCertValidation(ByVal sender As Object,
                                           ByVal cert As X509Certificate,
                                           ByVal chain As X509Chain,
                                           ByVal errors As SslPolicyErrors) As Boolean

        Return True
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click


        'Dim client = New RestClient("https://{baseUrl}/whatsapp/1/message/template")
        'client.Timeout = -1
        'Dim request = New RestRequest(Method.POST)
        'request.AddHeader("Authorization", "{authorization}")
        'request.AddHeader("Content-Type", "application/json")
        'request.AddHeader("Accept", "application/json")
        'Dim body = "{""messages"":[{""from"":""441134960000"",""to"":""441134960001"",""messageId"":""a28dd97c-1ffb-4fcf-99f1-0b557ed381da"",""content"":{""templateName"":""template_name"",""templateData"":{""body"":{""placeholders"":[""Placeholder Value 1"",""Placeholder Value 2""]}},""language"":""en_GB""},""callbackData"":""Callback data"",""notifyUrl"":""https://www.example.com/whatsapp""}]}"
        'request.AddParameter("application/json", body, ParameterType.RequestBody)
        'Dim response As IRestResponse = client.Execute(request)
        'Console.WriteLine(response.Content)

        Dim baseurl As String = "https://we6wd.api.infobip.com/whatsapp/1/message/sample_order_confirmation"
        Dim client As HttpClient = New HttpClient()
        Dim body = "{""messages"":[{""from"":""919514752147"",""to"":""919976253147"",""messageId"":""a28dd97c-1ffb-4fcf-99f1-0b557ed381da"",""content"":{""templateName"":""sample_order_confirmation"",""templateData"":{""body"":{""placeholders"":[""Placeholder Value 1"",""Placeholder Value 2""]}},""language"":""en_GB""},""callbackData"":""Callback data"",""notifyUrl"":""https://we6wd.api.infobip.com/whatsapp""}]}"

        client.DefaultRequestHeaders.Add("Basic", "6ec05c61e51cb67b56a95c4ee15bee6c-a2a0cdf4-73df-4746-b1ff-349a697ff753E")
        client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
        '  Dim response As HttpResponseMessage = client.PostAsync(baseurl).Result
        'Dim json As String = response.Content.ReadAsStringAsync().Result.ToString()


        'MsgBox(response)
        'MsgBox(json)
        'Dim encoded As String = "6ec05c61e51cb67b56a95c4ee15bee6c-a2a0cdf4-73df-4746-b1ff-349a697ff753"
        'Dim client As HttpClient = New HttpClient()
        'client.BaseAddress = New Uri("https://api.infobip.com/")
        'client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", encoded)
        'client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))


        'Dim httpRequest As HttpRequestMessage = New HttpRequestMessage(HttpMethod.Post, "/auth/1/session")
        ' httpRequest.Content = New StringContent(body, System.Text.Encoding.UTF8, "application/json")

        'Dim response = client.SendAsync(httpRequest).GetAwaiter().GetResult()
        'Dim responseContent As String = response.Content.ReadAsStringAsync().GetAwaiter().GetResult()

        ' Dim responseObject = JsonSerializer.Deserialize(responseContent)

        ' Dim result As String = responseObject.Token


    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim rs As String
        Dim dt1 As DataTable = New DataTable
        ConstrDB = "Server = " + ServerName + ";Database=" + DbName + ";User Id=" + Userid + ";Password=" + UserPwd + ";"
        Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)

        Dim clda As New SqlDataAdapter, clds As New DataSet
        clda.SelectCommand = New SqlCommand
        If ConDB.State = ConnectionState.Closed Then
            ConDB.Open()
        End If
        clda.SelectCommand.Connection = ConDB
        clda.SelectCommand.CommandType = CommandType.Text
        clda.SelectCommand.CommandText = " EXEC [INSWADespatchDetailsForCustomer] 'PDF'"
        clda.SelectCommand.CommandTimeout = 1000
        clda.Fill(clds, "tbl1")
        dt1 = clds.Tables(0)
        ConDB.Close()
        For Each clrow As DataRow In dt1.Rows

            If Not clrow.Item(2) = "" Then

                Dim FileNameReplace As String = clrow.Item(1).ToString().Replace("/", "_")

                Dim cryptfile As String

                Dim client As HttpClient = New HttpClient()
                Dim WaUrl As String = "http://wa1.api.qikberry.com/api/v1/messages/"
                Dim body = clrow.Item(2)
                Dim json = object_to_Json(json_to_object(body))


                Dim cryRpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument()

                Dim Dockey As Integer = Convert.ToInt32(clrow.Item(0).ToString())
                cryptfile = "E:\TFS\Whatsapp\Whatsapp\Whatsapp\Cry\GSTINVOICE_RR.rpt"
                cryRpt.Load(cryptfile)
                CrystalReportLogOn(cryRpt, Trim(ServerName), Trim(DbName), Trim(Userid), Trim(UserPwd))
                cryRpt.SetParameterValue("dockey@", Dockey)
                Me.CrystalReportViewer1.ReportSource = cryRpt


                Dim PDFFile As String = "E:\TFS\Whatsapp\Whatsapp\Whatsapp\Cry\Invoice_" + FileNameReplace + ".pdf"

                Dim filename As String = "Invoice_" + FileNameReplace

                Dim CrExportOptions As ExportOptions
                Dim CrDiskFileDestinationOptions As New DiskFileDestinationOptions()
                Dim CrFormatTypeOptions As New PdfRtfWordFormatOptions
                CrDiskFileDestinationOptions.DiskFileName = PDFFile
                CrExportOptions = cryRpt.ExportOptions
                With CrExportOptions
                    .ExportDestinationType = ExportDestinationType.DiskFile
                    .ExportFormatType = ExportFormatType.PortableDocFormat
                    .DestinationOptions = CrDiskFileDestinationOptions
                    .FormatOptions = CrFormatTypeOptions
                End With
                cryRpt.Export()





                Dim byt As Byte() = System.IO.File.ReadAllBytes(PDFFile)
                Dim Imagestring As String = Convert.ToBase64String(byt)




                Dim imageurl As String = "https://notification.ramrajcotton.net/api/UploadFile"

                Dim waybilldata As FileuploadData = New FileuploadData() With {
                        .FileName = filename,
                        .Type = "pdf",
                        .Data = Imagestring
                        }



                Dim Bytes_Invoice As Byte() = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(waybilldata))
                Dim ImagestringContent = New StringContent(JsonConvert.SerializeObject(waybilldata), UnicodeEncoding.UTF8, "application/json")




                client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
                Dim filecreationwa = client.PostAsync(imageurl, ImagestringContent).Result
                If filecreationwa.IsSuccessStatusCode Then
                    Dim json1 As String = filecreationwa.Content.ReadAsStringAsync().Result
                End If



                Dim stringContent = New StringContent(json, UnicodeEncoding.UTF8, "application/json")
                client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
                client.DefaultRequestHeaders.Add("QIKCHAT-API-KEY", "ZPWJ-Bq8p-izLx")

                Dim Responsewa = client.PostAsync(WaUrl, stringContent).Result
                rs = Responsewa.ToString()
                If Responsewa.IsSuccessStatusCode Then
                    Dim json1 As String = Responsewa.Content.ReadAsStringAsync().Result
                End If

            End If
        Next



    End Sub

    Public Class FileuploadData
        Public Property FileName As String
        Public Property Type As String
        Public Property Data As String

    End Class
    Public Function object_to_Json(ByVal table) As String
        Dim JSONString As String = String.Empty
        JSONString = JsonConvert.SerializeObject(table, Formatting.Indented)
        Return JSONString
    End Function

    Public Shared Function json_to_object(ByVal table)
        Dim myjson_object = JsonConvert.DeserializeObject(table)
        Return myjson_object
    End Function
    Friend Shared Function ReadPublicToken() As String
        Dim PublicKey As String = ""
        Dim path As String = ""
        path = System.Configuration.ConfigurationManager.ConnectionStrings("PATH").ConnectionString

        Using _ReadData As StreamReader = New StreamReader(path)
            PublicKey = _ReadData.ReadToEnd()
        End Using

        Return PublicKey
    End Function
    Public Sub CrystalReportLogOn(ByVal reportParameters As ReportDocument, ByVal serverName As String, ByVal databaseName As String, ByVal userName As String, ByVal password As String)
        Dim logOnInfo As TableLogOnInfo
        Dim subRd As ReportDocument
        Dim sects As Sections
        Dim ros As ReportObjects
        Dim sro As SubreportObject

        If reportParameters Is Nothing Then
            Throw New ArgumentNullException("reportParameters")
        End If

        Try
            For Each t As CrystalDecisions.CrystalReports.Engine.Table In reportParameters.Database.Tables
                logOnInfo = t.LogOnInfo
                logOnInfo.ReportName = reportParameters.Name
                logOnInfo.ConnectionInfo.ServerName = serverName
                logOnInfo.ConnectionInfo.DatabaseName = databaseName
                logOnInfo.ConnectionInfo.UserID = userName
                logOnInfo.ConnectionInfo.Password = password
                logOnInfo.TableName = t.Name
                t.ApplyLogOnInfo(logOnInfo)
            Next
        Catch
            Throw
        End Try

        sects = reportParameters.ReportDefinition.Sections
        For Each sect As Section In sects
            ros = sect.ReportObjects
            For Each ro As ReportObject In ros
                If ro.Kind = ReportObjectKind.SubreportObject Then
                    sro = DirectCast(ro, SubreportObject)
                    subRd = sro.OpenSubreport(sro.SubreportName)
                    Try
                        For Each t As CrystalDecisions.CrystalReports.Engine.Table In subRd.Database.Tables
                            logOnInfo = t.LogOnInfo
                            logOnInfo.ReportName = reportParameters.Name
                            logOnInfo.ConnectionInfo.ServerName = serverName
                            logOnInfo.ConnectionInfo.DatabaseName = databaseName
                            logOnInfo.ConnectionInfo.UserID = userName
                            logOnInfo.ConnectionInfo.Password = password
                            logOnInfo.TableName = t.Name
                            t.ApplyLogOnInfo(logOnInfo)
                        Next
                    Catch
                        Throw
                    End Try
                End If
            Next
        Next
    End Sub


End Class

Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Net.Security
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports Apitron.Pdf.Rasterizer
Imports Apitron.Pdf.Rasterizer.Configuration
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports Microsoft.SqlServer
Imports Newtonsoft.Json
Imports QRCoder
Imports System.Drawing
Imports Spire.Pdf
Imports Spire.Pdf.Graphics
Imports SautinSoft.PdfVision

Public Class FairInvitation
    Dim URL As String
    Dim tmpp As String
    Dim webClient As New System.Net.WebClient

    Dim ConDB As SqlConnection

    Dim ServerName As String = "192.168.0.6"
    Dim DbName As String = "ENESLIVE"
    Dim Userid As String = "IIS"
    Dim UserPwd As String = "55aigvsNH1XFs_B_Digvos"
    Dim CompanyCode As String = "ENES"

    Dim filepath As String = ""
    Public ConstrDB As String = ""
    Public Function customCertValidation(ByVal sender As Object,
                                           ByVal cert As X509Certificate,
                                           ByVal chain As X509Chain,
                                           ByVal errors As SslPolicyErrors) As Boolean

        Return True
    End Function
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim rs As String
        Dim dt1 As DataTable = New DataTable
        ConstrDB = "Server = " + ServerName + ";Database=" + DbName + ";User Id=" + Userid + ";Password=" + UserPwd + ";"
        Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)



        Dim qrGen = New QRCoder.QRCodeGenerator()
        Dim qrCode = qrGen.CreateQrCode("https://portal.ramrajcotton.net//Home/DefaultHome?QRBPID=2703", QRCoder.QRCodeGenerator.ECCLevel.H)
        Dim qrBmp = New BitmapByteQRCode(qrCode)
        Dim dt As Byte() = qrBmp.GetGraphic(7)
        Dim pictureBytes As MemoryStream = New MemoryStream(dt)
        PictureBox1.Image = System.Drawing.Image.FromStream(pictureBytes)
        PictureBox1.Image.Save("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\qr.jpg")



        Dim clda As New SqlDataAdapter, clds As New DataSet
        clda.SelectCommand = New SqlCommand
        If ConDB.State = ConnectionState.Closed Then
            ConDB.Open()
        End If
        clda.SelectCommand.Connection = ConDB
        clda.SelectCommand.CommandType = CommandType.Text
        clda.SelectCommand.CommandText = " EXEC [INSWADespatchDetailsForCustomer_Fair_VIJAYAVADA] 'fair_nov_new'"
        clda.SelectCommand.CommandTimeout = 1000
        clda.Fill(clds, "tbl1")
        dt1 = clds.Tables(0)
        ConDB.Close()
        For Each clrow As DataRow In dt1.Rows

            If Not clrow.Item(2) = "" Then

                Dim FileNameReplace As String = clrow.Item(0).ToString().Replace("/", "_")

                Dim cryptfile As String

                Dim client As HttpClient = New HttpClient()
                Dim WaUrl As String = "http://wa1.api.qikberry.com/api/v1/messages/"
                Dim body = clrow.Item(1)
                Dim json = object_to_Json(json_to_object(body))




                Dim cryRpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument()

                Dim Dockey As String = clrow.Item(0).ToString()
                cryptfile = "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\fairinvitation.rpt"
                cryRpt.Load(cryptfile)
                CrystalReportLogOn(cryRpt, Trim(ServerName), Trim(DbName), Trim(Userid), Trim(UserPwd))
                cryRpt.SetParameterValue("dockey@", Dockey)
                cryRpt.SetParameterValue("Qrpath", "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\qr.jpg")
                Me.CrystalReportViewer1.ReportSource = cryRpt


                Dim PDFFile As String = "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\Visitor_" + FileNameReplace + ".PDF"

                Dim filename As String = "Visitor_" + FileNameReplace

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




                Dim fullPath As String = "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\Visitor_" + FileNameReplace + ".PDF"
                Dim filepath As String = fullPath


                Dim imgBytes As Byte()

                Using fs As FileStream = New FileStream(filepath, FileMode.Open), fsOut As FileStream = File.Create(filename + ".Jpeg")

                    Using document As Document = New Document(fs)
                        Dim tiffRenderingSettings As TiffRenderingSettings = New TiffRenderingSettings(TiffCompressionMethod.None, 144, 144)
                        tiffRenderingSettings.WhiteColorTolerance = 0.9F
                        document.SaveToTiff(fsOut, tiffRenderingSettings)


                    End Using



                End Using

                Dim Path = System.Environment.CurrentDirectory() + "\" + filename + ".Jpeg"



                Dim byt1 As Byte() = System.IO.File.ReadAllBytes(Path)



                Dim pictureBytes1 As MemoryStream = New MemoryStream(byt1)
                PictureBox2.Image = System.Drawing.Image.FromStream(pictureBytes1)
                PictureBox2.Image.Save("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\" + filename + ".Jpeg")


                Dim byt As Byte() = System.IO.File.ReadAllBytes("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\" + filename + ".Jpeg")
                Dim Imagestring As String = Convert.ToBase64String(byt)






                Dim imageurl As String = "https://notification.ramrajcotton.net/api/UploadFile"

                Dim waybilldata As FileuploadData = New FileuploadData() With {
                        .FileName = filename,
                        .Type = "Jpeg",
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


        MessageBox.Show(rs)
    End Sub
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
    Public Class FileuploadData
        Public Property FileName As String
        Public Property Type As String
        Public Property Data As String

    End Class
    Public Class ResponseResult
        Public Property Status As String
        Public Property Message As String
        Public Property BpDetails As ResponsebpdetailsResult

    End Class
    Public Class ResponsebpdetailsResult
        Public Property BPID As String
        Public Property BPCode As String
        Public Property BPName As String
        Public Property URL As String
    End Class
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

    Private Sub FairInvitation_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim rs As String
        Dim dt1 As DataTable = New DataTable
        Dim dt11 As DataTable = New DataTable
        ConstrDB = "Server = " + ServerName + ";Database=" + DbName + ";User Id=" + Userid + ";Password=" + UserPwd + ";"
        Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)

        Dim clda121 As New SqlDataAdapter, clds121 As New DataSet
        clda121.SelectCommand = New SqlCommand
        If ConDB.State = ConnectionState.Closed Then
            ConDB.Open()
        End If
        clda121.SelectCommand.Connection = ConDB
        clda121.SelectCommand.CommandType = CommandType.Text
        'clda121.SelectCommand.CommandText = "SELECT Fairname FROM Fair_source_data WHERE IsActive=1"
        clda121.SelectCommand.CommandText = "select Fairname from  Fair_header where isactive=1"

        clda121.SelectCommand.CommandTimeout = 1000
        clda121.Fill(dt11)
        ComboBox1.DataSource = dt11
        ComboBox1.DisplayMember = "Fairname"
        ComboBox1.ValueMember = "Fairname"

        ConDB.Close()


        If System.Configuration.ConfigurationManager.AppSettings("Scheduler").ToString = "Y" Then
            Button6_Click(sender, e)
            Button9_Click_1(sender, e)
            Application.Exit()
        End If





    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim rs As String
        Dim dt1 As DataTable = New DataTable
        Dim dt11 As DataTable = New DataTable
        ConstrDB = "Server = " + ServerName + ";Database=" + DbName + ";User Id=" + Userid + ";Password=" + UserPwd + ";"
        Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)



        Dim clda As New SqlDataAdapter, clds As New DataSet
        clda.SelectCommand = New SqlCommand
        If ConDB.State = ConnectionState.Closed Then
            ConDB.Open()
        End If
        clda.SelectCommand.Connection = ConDB
        clda.SelectCommand.CommandType = CommandType.Text
        'clda.SelectCommand.CommandText = " EXEC [INSWACustomer_Fair_Digital_Banglore_31] 'fair_digitial_invitation'"
        clda.SelectCommand.CommandText = " EXEC  INSWACustomer_Fair_Digital_Invitation  'fair_digital_invitation','" + ComboBox1.Text + "'"
        clda.SelectCommand.CommandTimeout = 1000
        clda.Fill(clds, "tbl1")
        dt1 = clds.Tables(0)
        ConDB.Close()
        For Each clrow As DataRow In dt1.Rows
            Try
                If Not clrow.Item(2) = "" Then

                    Dim bpid As String = ""
                    Dim client1 As HttpClient = New HttpClient()
                    client1.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
                    Dim getbpid = client1.GetAsync("http://oopadmin.ramrajcotton.net/FairVisitorRegistration/GetBpData?BPCode=" & clrow.Item(0).ToString()).Result
                    If getbpid.IsSuccessStatusCode Then
                        Dim json1 As String = getbpid.Content.ReadAsStringAsync().Result
                        Dim Jsondata As ResponseResult = JsonConvert.DeserializeObject(Of ResponseResult)(json1)
                        bpid = Jsondata.BpDetails.BPID
                    End If


                    Dim qrGen = New QRCoder.QRCodeGenerator()
                    Dim qrCode = qrGen.CreateQrCode("https://portal.ramrajcotton.net//Home/DefaultHome?QRBPID=" & bpid, QRCoder.QRCodeGenerator.ECCLevel.H)
                    Dim qrBmp = New BitmapByteQRCode(qrCode)
                    Dim dt As Byte() = qrBmp.GetGraphic(7)
                    Dim pictureBytes As MemoryStream = New MemoryStream(dt)
                    PictureBox1.Image = System.Drawing.Image.FromStream(pictureBytes)
                    PictureBox1.Image.Save("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\qr_" & bpid & ".jpg")

                    Dim cardcode = clrow.Item(0).ToString()

                    Dim FileNameReplace As String = clrow.Item(0).ToString().Replace("/", "_")

                    Dim cryptfile As String
                    Dim cryptfile1 As String

                    Dim client As HttpClient = New HttpClient()
                    'Dim WaUrl As String = "http://wa1.api.qikberry.com/api/v1/messages/"
                    Dim WaUrl As String = "https://api.qikchat.in/v1/messages/"
                    Dim body = clrow.Item(1)
                    Dim json = object_to_Json(json_to_object(body))


                    Dim cryRpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument()

                    Dim Dockey As String = clrow.Item(0).ToString()
                    cryptfile = "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\address_print.rpt"
                    cryRpt.Load(cryptfile)
                    CrystalReportLogOn(cryRpt, Trim(ServerName), Trim(DbName), Trim(Userid), Trim(UserPwd))
                    cryRpt.SetParameterValue("dockey@", Dockey)
                    cryRpt.SetParameterValue("Qrpath", "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\qr_" & bpid & ".jpg")
                    Me.CrystalReportViewer1.ReportSource = cryRpt


                    Dim PDFFile As String = "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\Visitor_" + FileNameReplace + ".PDF"

                    Dim filename As String = "Visitor_" + FileNameReplace

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

                    Dim fullPath As String = "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\Visitor_" + FileNameReplace + ".PDF"
                    filepath = fullPath

                    Using fs As FileStream = New FileStream(filepath, FileMode.Open), fsOut As FileStream = File.Create(filename + "_new.Jpeg")

                        Using document As Document = New Document(fs)
                            Dim tiffRenderingSettings As TiffRenderingSettings = New TiffRenderingSettings(TiffCompressionMethod.None, 50, 50)
                            tiffRenderingSettings.WhiteColorTolerance = 0.9F
                            document.SaveToTiff(fsOut, tiffRenderingSettings)
                        End Using
                    End Using

                    Dim Path = System.Environment.CurrentDirectory() + "\" + filename + "_new.Jpeg"

                    Dim byt1 As Byte() = System.IO.File.ReadAllBytes(Path)

                    Dim pictureBytes1 As MemoryStream = New MemoryStream(byt1)
                    PictureBox2.Image = System.Drawing.Image.FromStream(pictureBytes1)
                    'System.Threading.Thread.Sleep(10000)
                    If Not My.Computer.FileSystem.FileExists("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\" + filename + "_new.Jpeg") Then
                        PictureBox2.Image.Save("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\" + filename + "_new.Jpeg")
                    End If


                    Dim bData As Byte()
                    'Dim br As BinaryReader = New BinaryReader(System.IO.File.OpenRead("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\FairInvitaionDayOrgBanglore31.jpg"))
                    Dim br As BinaryReader = New BinaryReader(System.IO.File.OpenRead("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\" + ComboBox1.Text + ".jpg"))
                    bData = br.ReadBytes(br.BaseStream.Length)
                    Dim ms As MemoryStream = New MemoryStream(bData, 0, bData.Length)
                    Dim img As Image = Image.FromStream(ms)

                    Dim Address As Bitmap = New Bitmap("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\" + filename + "_new.Jpeg")
                    Dim Invitation As Bitmap = New Bitmap(img)
                    Dim g As Graphics = Graphics.FromImage(Invitation)
                    g.DrawImage(Address, New Point(350, 950))
                    PictureBox3.Image = Invitation
                    PictureBox3.Image.Save("\\192.168.0.54\d$\FairInvImage\" + filename + ".Jpeg", System.Drawing.Imaging.ImageFormat.Jpeg)
                    Dim imagepath As String = "\\192.168.0.54\d$\FairInvImage\" + filename + ".Jpeg"
                    ''FileSystemWatcher1.Path = imagepath
                    ''FileSystemWatcher1.EnableRaisingEvents = True

                    ''System.Threading.Thread.Sleep(2000)

                    If System.IO.File.Exists(imagepath) Then

                        Dim stringContent = New StringContent(json, UnicodeEncoding.UTF8, "application/json")
                        client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
                        client.DefaultRequestHeaders.Add("QIKCHAT-API-KEY", "ZPWJ-Bq8p-izLx")

                        Dim Responsewa = client.PostAsync(WaUrl, stringContent).Result
                        rs = Responsewa.ToString()
                        If Responsewa.IsSuccessStatusCode Then
                            Dim jsondata1 As String = Responsewa.Content.ReadAsStringAsync().Result
                            Dim Jsondatamb As returndata = JsonConvert.DeserializeObject(Of returndata)(jsondata1)
                            Dim Mobile As String = Jsondatamb.data(0).recipient
                            Dim clda1 As New SqlDataAdapter, clds1 As New DataSet
                            clda1.SelectCommand = New SqlCommand
                            If ConDB.State = ConnectionState.Closed Then
                                ConDB.Open()
                            End If
                            clda1.SelectCommand.Connection = ConDB
                            clda1.SelectCommand.CommandType = CommandType.Text
                            'clda1.SelectCommand.CommandText = "INSERT INTO [dbo].[Fair_Bangalore_Digital_31]  ([Contact]) VALUES ( " + Mobile.Substring(2, 10) + " )"
                            'clda1.SelectCommand.CommandText = "INSERT INTO [dbo].[" + inserttable + "]  ([Contact]) VALUES ( " + Mobile.Substring(2, 10) + " )"
                            clda1.SelectCommand.CommandText = "update Fair_invitation_data set response='" + Responsewa.StatusCode.ToString() + "' , iswasent=1 where cardcode='" + cardcode + "'"
                            clda1.SelectCommand.CommandTimeout = 1000
                            clda1.SelectCommand.ExecuteNonQuery()
                        End If
                        ' System.Threading.Thread.Sleep(10000) 

                    End If

                End If
            Catch ex As Exception
                MessageBox.Show(ex.ToString())

            End Try
            'Button1_Click(sender, e)
        Next


        MessageBox.Show(rs)
    End Sub

    Public Shared Sub ConvertImageFolderToPdf()
        Dim inpFolder As String = (New DirectoryInfo("D:\Whatsapp\images")).FullName
        Dim outFile As String = (New FileInfo("Result.pdf")).FullName

        Dim v As New PdfVision()
        Dim options As New ImageToPdfOptions()
        options.PageSetup.PaperType = PaperType.Auto
        options.FitImageToPageSize = True
        options.JpegQuality = 95

        Try
            v.ConvertImageToPdf(inpFolder, outFile, options)
            System.Diagnostics.Process.Start(New System.Diagnostics.ProcessStartInfo(outFile) With {.UseShellExecute = True})
        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
            Console.ReadLine()
        End Try
    End Sub




    Public Class Datum
        Public Property id As String
        Public Property channel As String
        Public Property from As String
        Public Property recipient As String
        Public Property credits As Double
        Public Property created_at As DateTime
        Public Property status As String
    End Class

    Public Class returndata
        Public Property message As String
        Public Property data As List(Of Datum)
    End Class

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'Dim rs As String, ProcedureName_Inv As String
        'Dim dt1 As DataTable = New DataTable
        'Dim dt11 As DataTable = New DataTable
        'ConstrDB = "Server = " + ServerName + ";Database=" + DbName + ";User Id=" + Userid + ";Password=" + UserPwd + ";"
        'Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)

        'Dim clda1 As New SqlDataAdapter
        'clda1.SelectCommand = New SqlCommand
        'If ConDB.State = ConnectionState.Closed Then
        '    ConDB.Open()
        'End If
        'clda1.SelectCommand.Connection = ConDB
        'clda1.SelectCommand.CommandType = CommandType.Text
        'clda1.SelectCommand.CommandText = " select ProcedureName_Inv from fair_source_data where IsActive=1 and fairname = '" + ComboBox1.Text + "' "
        'clda1.SelectCommand.CommandTimeout = 1000
        'clda1.Fill(dt11)
        'ConDB.Close()
        'ProcedureName_Inv = dt11.Rows(0).Item("ProcedureName_Inv").ToString()

        'Dim clda As New SqlDataAdapter, clds As New DataSet
        'clda.SelectCommand = New SqlCommand
        'If ConDB.State = ConnectionState.Closed Then
        '    ConDB.Open()
        'End If
        'clda.SelectCommand.Connection = ConDB
        'clda.SelectCommand.CommandType = CommandType.Text
        ''clda.SelectCommand.CommandText = " EXEC [INSWADespatchDetailsForCustomer_Fair_Digital_Chennai_23_Inv] 'fair_digitial_invitation'"
        'clda.SelectCommand.CommandText = " EXEC " + ProcedureName_Inv + "'fair_digital_invitation'"
        'clda.SelectCommand.CommandTimeout = 1000
        'clda.Fill(clds, "tbl1")
        'dt1 = clds.Tables(0)
        'ConDB.Close()
        'For Each clrow As DataRow In dt1.Rows

        '    If Not clrow.Item(2) = "" Then

        '        Dim client As HttpClient = New HttpClient()
        '        'Dim WaUrl As String = "https://wa.api.qikberry.com/api/v1/messages/"
        '        Dim WaUrl As String = "https://api.qikchat.in/v1/messages/"
        '        Dim body = clrow.Item(1)
        '        Dim json = object_to_Json(json_to_object(body))

        '        Dim stringContent = New StringContent(json, UnicodeEncoding.UTF8, "application/json")
        '        client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
        '        client.DefaultRequestHeaders.Add("QIKCHAT-API-KEY", "ZPWJ-Bq8p-izLx")

        '        Dim Responsewa = client.PostAsync(WaUrl, stringContent).Result
        '        rs = Responsewa.ToString()
        '        If Responsewa.IsSuccessStatusCode Then
        '            Dim json1 As String = Responsewa.Content.ReadAsStringAsync().Result
        '        End If

        '    End If

        'Next


        'MessageBox.Show(rs)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
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
        clda.SelectCommand.CommandText = " EXEC [INSWADespatchDetailsForCustomer_Fair_Tutorial_E] 'fair_tutorial_new_video'"
        clda.SelectCommand.CommandTimeout = 1000
        clda.Fill(clds, "tbl1")
        dt1 = clds.Tables(0)
        ConDB.Close()
        For Each clrow As DataRow In dt1.Rows

            If Not clrow.Item(2) = "" Then




                Dim client As HttpClient = New HttpClient()
                Dim WaUrl As String = "http://wa1.api.qikberry.com/api/v1/messages/"
                Dim body = clrow.Item(1)
                Dim json = object_to_Json(json_to_object(body))

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


        MessageBox.Show(rs)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim rs As String
        Dim dt1 As DataTable = New DataTable
        ConstrDB = "Server = 192.168.0.58;Database=OOPLive;User Id=admin;Password=AdminG5366;"
        Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)



        Dim clda As New SqlDataAdapter, clds As New DataSet
        clda.SelectCommand = New SqlCommand
        If ConDB.State = ConnectionState.Closed Then
            ConDB.Open()
        End If
        clda.SelectCommand.Connection = ConDB
        clda.SelectCommand.CommandType = CommandType.Text
        clda.SelectCommand.CommandText = " EXEC [Sp_GetPoPrint_Fair_DirectLink_test] '" + TextBox1.Text + "','" + TextBox2.Text + "'"
        clda.SelectCommand.CommandTimeout = 1000
        clda.Fill(clds, "tbl1")
        dt1 = clds.Tables(0)
        ConDB.Close()
        For Each clrow As DataRow In dt1.Rows






            Dim client As HttpClient = New HttpClient()
            Dim WaUrl As String = "http://wa1.api.qikberry.com/api/v1/messages/"
            Dim body = clrow.Item(0).ToString()
            Dim json = object_to_Json(json_to_object(body))

            Dim stringContent = New StringContent(json, UnicodeEncoding.UTF8, "application/json")
            client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
            client.DefaultRequestHeaders.Add("QIKCHAT-API-KEY", "ZPWJ-Bq8p-izLx")

            Dim Responsewa = client.PostAsync(WaUrl, stringContent).Result
            rs = Responsewa.ToString()
            If Responsewa.IsSuccessStatusCode Then
                Dim json1 As String = Responsewa.Content.ReadAsStringAsync().Result
            End If


        Next


        MessageBox.Show(rs)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim rs As String
        Dim dt1 As DataTable = New DataTable
        ConstrDB = "Server = " + ServerName + ";Database=" + DbName + ";User Id=" + Userid + ";Password=" + UserPwd + ";"
        Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)



        Dim clda1 As New SqlDataAdapter, clds As New DataSet
        clda1.SelectCommand = New SqlCommand
        If ConDB.State = ConnectionState.Closed Then
            ConDB.Open()
        End If
        clda1.SelectCommand.CommandText = ""
        clda1.SelectCommand.Connection = ConDB
        clda1.SelectCommand.CommandType = CommandType.Text
        clda1.SelectCommand.CommandText = "exec Fair_video_Document_daybefore_video 'fair_digital_invitation_video','" + ComboBox1.Text + "'"
        clda1.SelectCommand.CommandTimeout = 1000
        clda1.Fill(clds, "tbl1")
        dt1 = clds.Tables(0)
        ConDB.Close()

        For Each clrow As DataRow In dt1.Rows
            Dim cardcode = clrow.Item(0).ToString()
            If Not clrow.Item(2) = "" Then
                System.Threading.Thread.Sleep(50000)
                Dim client As HttpClient = New HttpClient()
                'Dim WaUrl As String = "http://wa1.api.qikberry.com/api/v1/messages/"
                Dim WaUrl As String = "https://api.qikchat.in/v1/messages/"
                Dim body = clrow.Item(1)
                Dim json = object_to_Json(json_to_object(body))

                Dim stringContent = New StringContent(json, UnicodeEncoding.UTF8, "application/json")
                client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
                client.DefaultRequestHeaders.Add("QIKCHAT-API-KEY", "ZPWJ-Bq8p-izLx")

                Dim Responsewa = client.PostAsync(WaUrl, stringContent).Result
                rs = Responsewa.ToString()
                If Responsewa.IsSuccessStatusCode Then
                    Dim jsondata1 As String = Responsewa.Content.ReadAsStringAsync().Result
                    Dim Jsondatamb As returndata = JsonConvert.DeserializeObject(Of returndata)(jsondata1)
                    Dim Mobile As String = Jsondatamb.data(0).recipient
                    Dim clda12 As New SqlDataAdapter, clds1 As New DataSet
                    clda12.SelectCommand = New SqlCommand
                    If ConDB.State = ConnectionState.Closed Then
                        ConDB.Open()
                    End If
                    clda12.SelectCommand.Connection = ConDB
                    clda12.SelectCommand.CommandType = CommandType.Text
                    'clda1.SelectCommand.CommandText = "INSERT INTO [dbo].[Fair_Bangalore_Digital_31]  ([Contact]) VALUES ( " + Mobile.Substring(2, 10) + " )"
                    'clda1.SelectCommand.CommandText = "INSERT INTO [dbo].[" + inserttable + "]  ([Contact]) VALUES ( " + Mobile.Substring(2, 10) + " )"
                    clda12.SelectCommand.CommandText = "update Fair_invitation_data set response='" + Responsewa.StatusCode.ToString() + "' , iswasent=1 where cardcode='" + cardcode + "'"
                    clda12.SelectCommand.CommandTimeout = 1000
                    clda12.SelectCommand.ExecuteNonQuery()
                End If

            End If
        Next

        MessageBox.Show(rs)
    End Sub

    Public Shared Function ResizeImage(ByVal InputImage As Image) As Image
        Return New Bitmap(InputImage, New Size(3000, 2000))
    End Function

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim rs As String
        Dim dt1 As DataTable = New DataTable
        Dim dt11 As DataTable = New DataTable
        'ConstrDB = "Server = " + ServerName + ";Database=" + DbName + ";User Id=" + Userid + ";Password=" + UserPwd + ";"
        'Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)


        Dim clda As New SqlDataAdapter, clds As New DataSet
        clda.SelectCommand = New SqlCommand
        If ConDB.State = ConnectionState.Closed Then
            ConDB.Open()
        End If
        clda.SelectCommand.Connection = ConDB
        clda.SelectCommand.CommandType = CommandType.Text
        'clda.SelectCommand.CommandText = " EXEC [INSWADespatchDetailsForCustomer_Fair_Digital_Chennai_23_Inv] 'fair_digitial_invitation'"
        clda.SelectCommand.CommandText = " EXEC  [INSWACustomer_Fair_Digital_Banglore_02_regret_msg] 'fair_digitial_invitation'"
        clda.SelectCommand.CommandTimeout = 1000
        clda.Fill(clds, "tbl1")
        dt1 = clds.Tables(0)
        ConDB.Close()
        For Each clrow As DataRow In dt1.Rows

            If Not clrow.Item(2) = "" Then


                'Dim client As HttpClient = New HttpClient()
                'Dim WaUrl As String = "http://wa1.api.qikberry.com/api/v1/messages/"
                'Dim body = clrow.Item(1)
                'Dim json = object_to_Json(json_to_object(body))

                'Dim stringContent = New StringContent(json, UnicodeEncoding.UTF8, "application/json")
                'client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
                'client.DefaultRequestHeaders.Add("QIKCHAT-API-KEY", "ZPWJ-Bq8p-izLx")

                'Dim Responsewa = client.PostAsync(WaUrl, stringContent).Result
                'rs = Responsewa.ToString()
                'If Responsewa.IsSuccessStatusCode Then
                'Dim json1 As String = Responsewa.Content.ReadAsStringAsync().Result
                'End If

            End If
        Next


        MessageBox.Show(rs)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim rs As String
        Dim dt1 As DataTable = New DataTable
        Dim dt11 As DataTable = New DataTable
        ConstrDB = "Server = " + ServerName + ";Database=" + DbName + ";User Id=" + Userid + ";Password=" + UserPwd + ";"
        Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)


        Dim clda As New SqlDataAdapter, clds As New DataSet
        clda.SelectCommand = New SqlCommand
        If ConDB.State = ConnectionState.Closed Then
            ConDB.Open()
        End If
        clda.SelectCommand.Connection = ConDB
        clda.SelectCommand.CommandType = CommandType.Text
        'clda.SelectCommand.CommandText = " EXEC [INSWADespatchDetailsForCustomer_Fair_Digital_Chennai_23_Inv] 'fair_digitial_invitation'"
        'clda.SelectCommand.CommandText = " EXEC  [Fair_video_Document_daybefore_video] 'fair_digital_invitation_video','DiwaliFair_Trichy_Day1'"
        clda.SelectCommand.CommandTimeout = 1000
        clda.Fill(clds, "tbl1")
        dt1 = clds.Tables(0)
        ConDB.Close()
        For Each clrow As DataRow In dt1.Rows

            If Not clrow.Item(2) = "" Then

                Dim client As HttpClient = New HttpClient()
                Dim WaUrl As String = "http://wa1.api.qikberry.com/api/v1/messages/"
                Dim body = clrow.Item(1)
                Dim json = object_to_Json(json_to_object(body))

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


        MessageBox.Show(rs)




    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged

    End Sub


    Private Sub FileSystemWatcher1_Changed(sender As Object, e As FileSystemEventArgs) Handles FileSystemWatcher1.Changed

    End Sub

    Private Sub FileSystemWatcher1_Created(sender As Object, e As FileSystemEventArgs) Handles FileSystemWatcher1.Created
        If e.Name = filepath Then
            If (System.IO.File.Exists(e.FullPath)) Then
                FileSystemWatcher1.EnableRaisingEvents = False
                PictureBox1.Load(e.FullPath)
            End If
        End If
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) 
        ConvertImageFolderToPdf()
    End Sub

    Private Sub Button9_Click_1(sender As Object, e As EventArgs) Handles Button9.Click
        Dim rs As String
        Dim dt1 As DataTable = New DataTable
        Dim dt11 As DataTable = New DataTable
        ConstrDB = "Server = " + ServerName + ";Database=" + DbName + ";User Id=" + Userid + ";Password=" + UserPwd + ";"
        Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)



        Dim clda As New SqlDataAdapter, clds As New DataSet
        clda.SelectCommand = New SqlCommand
        If ConDB.State = ConnectionState.Closed Then
            ConDB.Open()
        End If
        clda.SelectCommand.Connection = ConDB
        clda.SelectCommand.CommandType = CommandType.Text
        'clda.SelectCommand.CommandText = " EXEC [INSWACustomer_Fair_Digital_Banglore_31] 'fair_digitial_invitation'"
        clda.SelectCommand.CommandText = " EXEC  INSWACustomer_Fair_Digital_Invitation  'fair_digital_invitation','" + ComboBox1.Text + "'"
        clda.SelectCommand.CommandTimeout = 1000
        clda.Fill(clds, "tbl1")
        dt1 = clds.Tables(0)
        ConDB.Close()
        For Each clrow As DataRow In dt1.Rows
            Try
                If Not clrow.Item(2) = "" Then

                    Dim cardcode = clrow.Item(0).ToString()

                    Dim FileNameReplace As String = clrow.Item(0).ToString().Replace("/", "_")


                    Dim client As HttpClient = New HttpClient()
                    'Dim WaUrl As String = "http://wa1.api.qikberry.com/api/v1/messages/"
                    Dim WaUrl As String = "https://api.qikchat.in/v1/messages/"
                    Dim body = clrow.Item(1)
                    Dim json = object_to_Json(json_to_object(body))

                    Dim filename As String = "Visitor_" + FileNameReplace
                    Dim imagepath As String = "\\192.168.0.54\d$\FairInvImage\" + filename + ".Jpeg"

                    If System.IO.File.Exists(imagepath) Then

                        Dim stringContent = New StringContent(json, UnicodeEncoding.UTF8, "application/json")
                        client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
                        client.DefaultRequestHeaders.Add("QIKCHAT-API-KEY", "ZPWJ-Bq8p-izLx")

                        Dim Responsewa = client.PostAsync(WaUrl, stringContent).Result
                        rs = Responsewa.ToString()
                        If Responsewa.IsSuccessStatusCode Then
                            Dim jsondata1 As String = Responsewa.Content.ReadAsStringAsync().Result
                            Dim Jsondatamb As returndata = JsonConvert.DeserializeObject(Of returndata)(jsondata1)
                            Dim Mobile As String = Jsondatamb.data(0).recipient
                            Dim clda1 As New SqlDataAdapter, clds1 As New DataSet
                            clda1.SelectCommand = New SqlCommand
                            If ConDB.State = ConnectionState.Closed Then
                                ConDB.Open()
                            End If
                            clda1.SelectCommand.Connection = ConDB
                            clda1.SelectCommand.CommandType = CommandType.Text
                            clda1.SelectCommand.CommandText = "update Fair_invitation_data set response='" + Responsewa.StatusCode.ToString() + "' , iswasent=1 where cardcode='" + cardcode + "'"
                            clda1.SelectCommand.CommandTimeout = 1000
                            clda1.SelectCommand.ExecuteNonQuery()
                        End If
                        ' System.Threading.Thread.Sleep(10000) 

                    End If

                End If
            Catch ex As Exception
                MessageBox.Show(ex.ToString())

            End Try
            'Button1_Click(sender, e)
        Next


        MessageBox.Show(rs)
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        'Dim rs As String
        'Dim dt1 As DataTable = New DataTable
        'Dim dt11 As DataTable = New DataTable
        'ConstrDB = "Server = " + ServerName + ";Database=" + DbName + ";User Id=" + Userid + ";Password=" + UserPwd + ";"
        'Dim ConDB As SqlConnection = New SqlConnection(ConstrDB)

        'Dim clda As New SqlDataAdapter, clds As New DataSet
        'clda.SelectCommand = New SqlCommand
        'If ConDB.State = ConnectionState.Closed Then
        '    ConDB.Open()
        'End If
        'clda.SelectCommand.Connection = ConDB
        'clda.SelectCommand.CommandType = CommandType.Text
        'clda.SelectCommand.CommandText = " EXEC  [INSWACustomer_LetterPad]  'LetterPad'"
        'clda.SelectCommand.CommandTimeout = 1000
        'clda.Fill(clds, "tbl1")
        'dt1 = clds.Tables(0)
        'ConDB.Close()
        'For Each clrow As DataRow In dt1.Rows
        '    Try
        '        If Not clrow.Item(2) = "" Then
        '            Dim cardcode = clrow.Item(0).ToString()

        '            Dim FileNameReplace As String = clrow.Item(0).ToString().Replace("/", "_")

        '            Dim cryptfile As String

        '            Dim client As HttpClient = New HttpClient()
        '            'Dim WaUrl As String = "http://wa1.api.qikberry.com/api/v1/messages/"
        '            Dim WaUrl As String = "https://api.qikchat.in/v1/messages/"
        '            Dim body = clrow.Item(1)
        '            Dim json = object_to_Json(json_to_object(body))


        '            Dim cryRpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument()

        '            Dim Dockey As String = clrow.Item(0).ToString()
        '            cryptfile = "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\address_print_letterpad.rpt"
        '            cryRpt.Load(cryptfile)
        '            CrystalReportLogOn(cryRpt, Trim(ServerName), Trim(DbName), Trim(Userid), Trim(UserPwd))
        '            cryRpt.SetParameterValue("dockey@", Dockey)

        '            Dim PDFFile As String = "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\Visitor_" + FileNameReplace + ".PDF"

        '            Dim filename As String = "Visitor_" + FileNameReplace

        '            Dim CrExportOptions As ExportOptions
        '            Dim CrDiskFileDestinationOptions As New DiskFileDestinationOptions()
        '            Dim CrFormatTypeOptions As New PdfRtfWordFormatOptions
        '            CrDiskFileDestinationOptions.DiskFileName = PDFFile
        '            CrExportOptions = cryRpt.ExportOptions
        '            With CrExportOptions
        '                .ExportDestinationType = ExportDestinationType.DiskFile
        '                .ExportFormatType = ExportFormatType.PortableDocFormat
        '                .DestinationOptions = CrDiskFileDestinationOptions
        '                .FormatOptions = CrFormatTypeOptions
        '            End With
        '            cryRpt.Export()

        '            Dim fullPath As String = "D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\Visitor_" + FileNameReplace + ".PDF"
        '            filepath = fullPath

        '            Using fs As FileStream = New FileStream(filepath, FileMode.Open), fsOut As FileStream = File.Create(filename + "_new.Jpeg")

        '                Using document As Document = New Document(fs)
        '                    Dim tiffRenderingSettings As TiffRenderingSettings = New TiffRenderingSettings(TiffCompressionMethod.None, 50, 50)
        '                    tiffRenderingSettings.WhiteColorTolerance = 0.9F
        '                    document.SaveToTiff(fsOut, tiffRenderingSettings)
        '                End Using
        '            End Using

        '            Dim Path = System.Environment.CurrentDirectory() + "\" + filename + "_new.Jpeg"

        '            Dim byt1 As Byte() = System.IO.File.ReadAllBytes(Path)

        '            Dim pictureBytes1 As MemoryStream = New MemoryStream(byt1)
        '            PictureBox2.Image = System.Drawing.Image.FromStream(pictureBytes1)
        '            'System.Threading.Thread.Sleep(10000)
        '            If Not My.Computer.FileSystem.FileExists("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\" + filename + "_new.Jpeg") Then
        '                PictureBox2.Image.Save("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\" + filename + "_new.Jpeg")
        '            End If


        '            Dim bData As Byte()
        '            'Dim br As BinaryReader = New BinaryReader(System.IO.File.OpenRead("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\FairInvitaionDayOrgBanglore31.jpg"))
        '            Dim br As BinaryReader = New BinaryReader(System.IO.File.OpenRead("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\DitributorLetterPad.jpg"))
        '            bData = br.ReadBytes(br.BaseStream.Length)
        '            Dim ms As MemoryStream = New MemoryStream(bData, 0, bData.Length)
        '            Dim img As Image = Image.FromStream(ms)

        '            Dim Address As Bitmap = New Bitmap("D:\Sources\Gowtham\whatsapp\Whatsapp\Cry\" + filename + "_new.Jpeg")
        '            Dim Invitation As Bitmap = New Bitmap(img)
        '            Dim g As Graphics = Graphics.FromImage(Invitation)
        '            g.DrawImage(Address, New Point(270, 400))
        '            PictureBox3.Image = Invitation
        '            PictureBox3.Image.Save("\\192.168.0.54\d$\FairInvImage\LetterPad\" + filename + ".Jpeg", System.Drawing.Imaging.ImageFormat.Jpeg)
        '            Dim imagepath As String = "\\192.168.0.54\d$\FairInvImage\LetterPad\" + filename + ".Jpeg"

        '            If System.IO.File.Exists(imagepath) Then

        '                Dim stringContent = New StringContent(json, UnicodeEncoding.UTF8, "application/json")
        '                client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
        '                client.DefaultRequestHeaders.Add("QIKCHAT-API-KEY", "ZPWJ-Bq8p-izLx")

        '                Dim Responsewa = client.PostAsync(WaUrl, stringContent).Result
        '                rs = Responsewa.ToString()
        '                If Responsewa.IsSuccessStatusCode Then
        '                    Dim jsondata1 As String = Responsewa.Content.ReadAsStringAsync().Result
        '                    Dim Jsondatamb As returndata = JsonConvert.DeserializeObject(Of returndata)(jsondata1)
        '                    Dim Mobile As String = Jsondatamb.data(0).recipient
        '                    Dim clda1 As New SqlDataAdapter, clds1 As New DataSet
        '                    clda1.SelectCommand = New SqlCommand
        '                    If ConDB.State = ConnectionState.Closed Then
        '                        ConDB.Open()
        '                    End If
        '                    clda1.SelectCommand.Connection = ConDB
        '                    clda1.SelectCommand.CommandType = CommandType.Text
        '                    clda1.SelectCommand.CommandText = "update [LetterPad_details] set response='" + Responsewa.StatusCode.ToString() + "' where cardcode='" + cardcode + "'"
        '                    clda1.SelectCommand.CommandTimeout = 1000
        '                    clda1.SelectCommand.ExecuteNonQuery()
        '                End If

        '            End If

        '        End If
        '    Catch ex As Exception
        '        MessageBox.Show(ex.ToString())

        '    End Try
        '    'Button1_Click(sender, e)
        'Next


        'MessageBox.Show(rs)
    End Sub
End Class
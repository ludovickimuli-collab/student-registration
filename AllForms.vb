Imports System.Drawing
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports MySqlConnector
Namespace StudentRegistrationSystem
 Public Module Colours
  Public ReadOnly Blue As Color=Color.FromArgb(25,55,109)
  Public ReadOnly Maroon As Color=Color.FromArgb(112,25,48)
  Public ReadOnly Gold As Color=Color.FromArgb(212,175,55)
 End Module
 Public Class UiForm
  Inherits Form
  Public Sub New()
   BackColor=Color.WhiteSmoke : Font=New Font("Segoe UI",9.5F) : StartPosition=FormStartPosition.CenterScreen
   FormBorderStyle=FormBorderStyle.FixedSingle : MaximizeBox=False
  End Sub
  Protected Sub Heading(t As String)
   Controls.Add(New Label With {.Text=t,.Font=New Font("Segoe UI",18,FontStyle.Bold),.ForeColor=Colours.Blue,.Location=New Point(25,20),.AutoSize=True})
  End Sub
  Protected Function Group(t As String,x As Integer,y As Integer,w As Integer,h As Integer) As GroupBox
   Dim g As New GroupBox With {.Text=t,.Location=New Point(x,y),.Size=New Size(w,h),.ForeColor=Colours.Maroon} : Controls.Add(g) : Return g
  End Function
  Protected Function Field(parent As Control,caption As String,n As String,x As Integer,y As Integer,w As Integer) As TextBox
   parent.Controls.Add(New Label With {.Text=caption,.Location=New Point(x,y),.AutoSize=True})
   Dim t As New TextBox With {.Name=n,.Location=New Point(x,y+22),.Size=New Size(w,27)} : parent.Controls.Add(t) : Return t
  End Function
  Protected Function ButtonOf(caption As String,x As Integer,y As Integer,c As Color) As Button
   Dim b As New Button With {.Text=caption,.Location=New Point(x,y),.Size=New Size(180,40),.BackColor=c,.ForeColor=If(c=Colours.Gold,Color.Black,Color.White),.FlatStyle=FlatStyle.Flat}
   b.FlatAppearance.BorderSize=0 : Controls.Add(b) : Return b
  End Function
  Protected Sub ErrorMessage(s As String)
   MessageBox.Show(s,"Student Registration System",MessageBoxButtons.OK,MessageBoxIcon.Error)
  End Sub
 End Class
 Public Class LoginForm
  Inherits UiForm
  Private txtEmail,txtPassword As TextBox
  Public Sub New()
   Text="Student Portal - Login" : ClientSize=New Size(490,440) : Heading("STUDENT REGISTRATION")
   Dim g=Group("Account credentials",30,90,430,210)
   txtEmail=Field(g,"Email address","txtEmail",22,35,370) : txtPassword=Field(g,"Password","txtPassword",22,105,370) : txtPassword.UseSystemPasswordChar=True
   Dim login=ButtonOf("LOGIN",30,325,Colours.Blue), signup=ButtonOf("CREATE ACCOUNT",245,325,Colours.Maroon)
   AddHandler login.Click, AddressOf DoLogin
   AddHandler signup.Click, Sub()
                               Using signupWindow As New SignUpForm()
                                   signupWindow.ShowDialog(Me)
                               End Using
                           End Sub
   Controls.Add(New Label With {.Text="Registration fee: UGX 30,000",.Location=New Point(32,385),.ForeColor=Colours.Maroon,.AutoSize=True})
 End Sub
 Private Sub DoLogin(sender As Object,e As EventArgs)
  If txtEmail.Text.Trim()="" OrElse txtPassword.Text="" Then MessageBox.Show("Enter your email and password.") : Return
  Try
   Using c=Db.OpenConnection(),cmd As New MySqlCommand("SELECT user_id,student_number,full_name,password_hash FROM users WHERE email=@e AND is_active=1 LIMIT 1",c)
    cmd.Parameters.AddWithValue("@e",txtEmail.Text.Trim())
    Using r=cmd.ExecuteReader()
     If r.Read() AndAlso Passwords.Verify(txtPassword.Text,r.GetString("password_hash")) Then
      CurrentStudent.UserId=r.GetInt32("user_id") : CurrentStudent.StudentNo=r.GetString("student_number") : CurrentStudent.FullName=r.GetString("full_name")
      Dim f As New DashboardForm() : f.Show() : Hide()
     Else
      MessageBox.Show("Incorrect email or password.","Login failed",MessageBoxButtons.OK,MessageBoxIcon.Warning)
     End If
    End Using
   End Using
  Catch ex As Exception
   ErrorMessage("Login/database error: " & ex.Message)
  End Try
 End Sub
 End Class
 Public Class SignUpForm
  Inherits UiForm
  Private sn,nm,em,pw,confirm As TextBox
  Public Sub New()
   Text="Create Student Account" : ClientSize=New Size(510,535) : Heading("CREATE STUDENT ACCOUNT")
   Dim g=Group("Personal and account details",25,75,460,370)
   sn=Field(g,"Student number","txtStudentNumber",20,30,400) : nm=Field(g,"Full name","txtFullName",20,90,400)
   em=Field(g,"Email address","txtEmail",20,150,400) : pw=Field(g,"Password (minimum 8 characters)","txtPassword",20,210,400) : pw.UseSystemPasswordChar=True
   confirm=Field(g,"Confirm password","txtConfirmPassword",20,270,400) : confirm.UseSystemPasswordChar=True
   Dim create=ButtonOf("CREATE ACCOUNT",25,465,Colours.Blue), cancel=ButtonOf("CANCEL",285,465,Colours.Maroon)
   AddHandler create.Click,AddressOf CreateAccount : AddHandler cancel.Click,Sub() Close()
 End Sub
 Private Sub CreateAccount(sender As Object,e As EventArgs)
  If sn.Text.Trim()="" OrElse nm.Text.Trim()="" OrElse em.Text.Trim()="" OrElse pw.Text="" Then MessageBox.Show("Complete all required fields.") : Return
  If Not Regex.IsMatch(em.Text.Trim(),"^[^@\s]+@[^@\s]+\.[^@\s]+$") Then MessageBox.Show("Enter a valid email address.") : Return
  If pw.Text.Length<8 OrElse pw.Text<>confirm.Text Then MessageBox.Show("Check password length and confirmation.") : Return
  Try
   Using c=Db.OpenConnection(),cmd As New MySqlCommand("INSERT INTO users(student_number,full_name,email,password_hash) VALUES(@s,@n,@e,@p)",c)
    cmd.Parameters.AddWithValue("@s",sn.Text.Trim()) : cmd.Parameters.AddWithValue("@n",nm.Text.Trim()) : cmd.Parameters.AddWithValue("@e",em.Text.Trim()) : cmd.Parameters.AddWithValue("@p",Passwords.Hash(pw.Text)) : cmd.ExecuteNonQuery()
   End Using
   MessageBox.Show("Account created. Return to Login.","Success",MessageBoxButtons.OK,MessageBoxIcon.Information) : Close()
  Catch ex As MySqlException When ex.Number=1062
   MessageBox.Show("Student number or email already exists.")
  Catch ex As Exception
   ErrorMessage(ex.Message)
  End Try
 End Sub
 End Class
 Public Class DashboardForm
  Inherits UiForm
  Public Sub New()
   Text="Student Portal - Dashboard" : ClientSize=New Size(660,440) : Heading("STUDENT PORTAL")
   Controls.Add(New Label With {.Text="Welcome, " & CurrentStudent.FullName,.Location=New Point(30,70),.AutoSize=True,.ForeColor=Colours.Maroon})
   Dim g=Group("Available services",30,115,600,210)
   Dim reg As New Button With {.Text="STUDENT REGISTRATION",.Location=New Point(35,65),.Size=New Size(235,75),.BackColor=Colours.Blue,.ForeColor=Color.White}
   Dim pay As New Button With {.Text="REGISTRATION PAYMENT",.Location=New Point(320,65),.Size=New Size(235,75),.BackColor=Colours.Maroon,.ForeColor=Color.White}
   g.Controls.Add(reg) : g.Controls.Add(pay)
   AddHandler reg.Click, Sub()
                             If CurrentStudent.UserId > 0 Then
                                 Using registrationWindow As New StudentRegistrationForm()
                                     registrationWindow.ShowDialog(Me)
                                 End Using
                             Else
                                 ErrorMessage("Please log in before opening student registration.")
                             End If
                         End Sub
   AddHandler pay.Click, Sub()
                             If CurrentStudent.UserId > 0 Then
                                 Using paymentWindow As New PaymentForm()
                                     paymentWindow.ShowDialog(Me)
                                 End Using
                             Else
                                 ErrorMessage("Please log in before opening payments.")
                             End If
                         End Sub
   Dim logout = ButtonOf("LOG OUT",430,365,Colours.Gold)
   AddHandler logout.Click, Sub()
                                CurrentStudent.Logout()
                                Dim loginWindow As New LoginForm()
                                loginWindow.Show()
                                Close()
                            End Sub
  End Sub
 End Class
 Public Class StudentRegistrationForm
  Inherits UiForm
  Private firstName,lastName,phone,address As TextBox
  Private dob As DateTimePicker
  Private male,female As RadioButton
  Private programme,faculty As ComboBox
  Private declaration As CheckBox
  Public Sub New()
   Text="Student Registration" : ClientSize=New Size(780,650) : Heading("STUDENT REGISTRATION FORM")
   Controls.Add(New Label With {.Text="Signed in: " & CurrentStudent.FullName & " (" & CurrentStudent.StudentNo & ")",.Location=New Point(28,58),.AutoSize=True,.ForeColor=Colours.Maroon})
   Dim a=Group("SECTION A: PERSONAL INFORMATION",25,90,725,190)
   firstName=Field(a,"First name *","txtFirstName",20,28,285) : lastName=Field(a,"Last name *","txtLastName",380,28,285)
   a.Controls.Add(New Label With {.Text="Date of birth",.Location=New Point(20,105),.AutoSize=True})
   dob=New DateTimePicker With {.Name="dtpDateOfBirth",.Location=New Point(20,128),.Size=New Size(285,27),.Format=DateTimePickerFormat.Short} : a.Controls.Add(dob)
   a.Controls.Add(New Label With {.Text="Gender *",.Location=New Point(380,105),.AutoSize=True})
   male=New RadioButton With {.Name="radMale",.Text="Male",.Location=New Point(380,132),.AutoSize=True}
   female=New RadioButton With {.Name="radFemale",.Text="Female",.Location=New Point(470,132),.AutoSize=True} : a.Controls.AddRange(New Control(){male,female})
   Dim b=Group("SECTION B: CONTACT AND ACADEMIC INFORMATION",25,295,725,245)
   phone=Field(b,"Phone number *","txtPhone",20,25,285)
   b.Controls.Add(New Label With {.Text="Programme *",.Location=New Point(380,25),.AutoSize=True})
   programme=New ComboBox With {.Name="cboProgramme",.Location=New Point(380,48),.Size=New Size(300,27),.DropDownStyle=ComboBoxStyle.DropDownList}
   programme.Items.AddRange(New Object(){"Bachelor of Information Technology","Bachelor of Arts with Education","Bachelor of Business Administration","Bachelor of Nursing Science","Diploma Programme","Other"})
   b.Controls.Add(New Label With {.Text="Faculty",.Location=New Point(20,105),.AutoSize=True})
   faculty=New ComboBox With {.Name="cboFaculty",.Location=New Point(20,128),.Size=New Size(285,27),.DropDownStyle=ComboBoxStyle.DropDownList}
   faculty.Items.AddRange(New Object(){"Faculty of Business and ICT","Faculty of Education","Faculty of Nursing and Midwifery","Faculty of Social Sciences"})
   address=Field(b,"Residential address","txtAddress",380,105,300)
   declaration=New CheckBox With {.Name="chkDeclaration",.Text="I confirm that the information provided is correct.",.Location=New Point(20,185),.AutoSize=True}
   b.Controls.AddRange(New Control(){programme,faculty,declaration})
   Dim save = ButtonOf("SAVE",25,570,Colours.Blue)
   Dim clear = ButtonOf("CLEAR",275,570,Colours.Maroon)
   Dim btnExit = ButtonOf("EXIT",525,570,Colours.Gold)
   AddHandler save.Click, AddressOf SaveStudent
   AddHandler clear.Click, AddressOf ClearFields
   AddHandler btnExit.Click, Sub() Me.Close()
  End Sub
  Private Sub SaveStudent(sender As Object,e As EventArgs)
   If CurrentStudent.UserId<=0 Then ErrorMessage("Login is required to access registration.") : Close() : Return
   If firstName.Text.Trim()="" Then MessageBox.Show("First name is required.") : firstName.Focus() : Return
   If lastName.Text.Trim()="" Then MessageBox.Show("Last name is required.") : lastName.Focus() : Return
   If phone.Text.Trim()="" Then MessageBox.Show("Phone number is required.") : phone.Focus() : Return
   If programme.SelectedIndex<0 OrElse (Not male.Checked AndAlso Not female.Checked) OrElse Not declaration.Checked Then MessageBox.Show("Select a programme and gender, and accept the declaration.") : Return
   Try
    Using c=Db.OpenConnection(),cmd As New MySqlCommand("INSERT INTO student_profiles(user_id,first_name,last_name,date_of_birth,gender,phone,programme,faculty,address) VALUES(@u,@f,@l,@d,@g,@p,@pr,@fa,@a) ON DUPLICATE KEY UPDATE first_name=VALUES(first_name),last_name=VALUES(last_name),date_of_birth=VALUES(date_of_birth),gender=VALUES(gender),phone=VALUES(phone),programme=VALUES(programme),faculty=VALUES(faculty),address=VALUES(address)",c)
     cmd.Parameters.AddWithValue("@u",CurrentStudent.UserId) : cmd.Parameters.AddWithValue("@f",firstName.Text.Trim()) : cmd.Parameters.AddWithValue("@l",lastName.Text.Trim())
     cmd.Parameters.AddWithValue("@d",dob.Value.Date) : cmd.Parameters.AddWithValue("@g",If(male.Checked,"Male","Female")) : cmd.Parameters.AddWithValue("@p",phone.Text.Trim())
     cmd.Parameters.AddWithValue("@pr",programme.Text) : cmd.Parameters.AddWithValue("@fa",faculty.Text) : cmd.Parameters.AddWithValue("@a",address.Text.Trim()) : cmd.ExecuteNonQuery()
    End Using
    MessageBox.Show("Student registration details saved successfully.","Saved",MessageBoxButtons.OK,MessageBoxIcon.Information)
   Catch ex As Exception
    ErrorMessage("Could not save registration: " & ex.Message)
   End Try
  End Sub
  Private Sub ClearFields(sender As Object,e As EventArgs)
   firstName.Clear() : lastName.Clear() : phone.Clear() : address.Clear() : dob.Value=Date.Today
   programme.SelectedIndex=-1 : faculty.SelectedIndex=-1 : male.Checked=False : female.Checked=False : declaration.Checked=False
  End Sub
 End Class
 Public Class PaymentForm
  Inherits UiForm
  Private method As ComboBox, confirm As CheckBox
  Public Sub New()
   Text="Registration Payment" : ClientSize=New Size(600,465) : Heading("REGISTRATION PAYMENT")
   Controls.Add(New Label With {.Text="Student: " & CurrentStudent.FullName & " | " & CurrentStudent.StudentNo,.Location=New Point(30,70),.AutoSize=True})
   Dim g=Group("Fee details",30,110,535,220)
   g.Controls.Add(New Label With {.Text="AMOUNT DUE: UGX 30,000",.Font=New Font("Segoe UI",17,FontStyle.Bold),.ForeColor=Colours.Maroon,.Location=New Point(25,40),.AutoSize=True})
   g.Controls.Add(New Label With {.Text="Payment method",.Location=New Point(25,100),.AutoSize=True})
   method=New ComboBox With {.Location=New Point(25,125),.Size=New Size(470,28),.DropDownStyle=ComboBoxStyle.DropDownList}
   method.Items.AddRange(New Object(){"Cash at accounts office","Bank deposit","Mobile money - manual confirmation"}) : method.SelectedIndex=0
   confirm=New CheckBox With {.Text="I confirm that I have made / will present this payment.",.Location=New Point(25,175),.AutoSize=True}
   g.Controls.AddRange(New Control(){method,confirm})
   Dim pay=ButtonOf("RECORD DEMO PAYMENT",30,355,Colours.Blue), cancel=ButtonOf("CANCEL",340,355,Colours.Maroon)
   AddHandler pay.Click,AddressOf RecordPayment : AddHandler cancel.Click,Sub() Close()
   Controls.Add(New Label With {.Text="DEMO ONLY: records a database entry; does not transfer money.",.Location=New Point(32,410),.AutoSize=True,.ForeColor=Colours.Maroon})
  End Sub
  Private Sub RecordPayment(sender As Object,e As EventArgs)
   If Not confirm.Checked Then MessageBox.Show("Confirm the payment declaration first.") : Return
   Try
    Using c=Db.OpenConnection(),cmd As New MySqlCommand("INSERT INTO payments(user_id,amount,currency,payment_method,reference_number,payment_status) VALUES(@u,30000,'UGX',@m,@r,'SUCCESS')",c)
     cmd.Parameters.AddWithValue("@u",CurrentStudent.UserId) : cmd.Parameters.AddWithValue("@m",method.Text) : cmd.Parameters.AddWithValue("@r","DEMO-" & Guid.NewGuid().ToString("N").Substring(0,10).ToUpperInvariant()) : cmd.ExecuteNonQuery()
    End Using
    Using successWindow As New PaymentSuccessForm()
        successWindow.ShowDialog(Me)
    End Using
    Close()
   Catch ex As Exception
    Using errorWindow As New PaymentErrorForm(ex.Message)
        errorWindow.ShowDialog(Me)
    End Using
   End Try
  End Sub
 End Class
 Public Class PaymentSuccessForm
  Inherits UiForm
  Public Sub New()
   Text="Payment Success" : ClientSize=New Size(560,280) : Heading("PAYMENT RECORDED")
   Controls.Add(New Label With {.Text="Your UGX 30,000 payment entry was saved successfully.",.Location=New Point(30,100),.AutoSize=True,.Font=New Font("Segoe UI",12)})
   Dim b=ButtonOf("CLOSE",30,185,Colours.Gold) : AddHandler b.Click,Sub() Close()
  End Sub
 End Class
 Public Class PaymentErrorForm
  Inherits UiForm
  Public Sub New(details As String)
   Text="Payment Error" : ClientSize=New Size(620,320) : Heading("PAYMENT NOT COMPLETED")
   Controls.Add(New Label With {.Text="We could not save the payment record.",.Location=New Point(30,95),.AutoSize=True})
   Controls.Add(New Label With {.Text=details,.Location=New Point(30,130),.Size=New Size(550,80)})
   Dim b=ButtonOf("CLOSE",30,235,Colours.Maroon) : AddHandler b.Click,Sub() Close()
  End Sub
 End Class
End Namespace

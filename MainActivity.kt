package com.unik.studentregistration

import android.app.Activity
import android.os.Bundle
import android.graphics.Color
import android.view.Gravity
import android.view.View
import android.widget.*

class MainActivity : Activity() {
    private val navy = Color.rgb(25,55,109)
    private val maroon = Color.rgb(112,25,48)
    private val gold = Color.rgb(212,175,55)
    private var loggedIn = false
    private var studentName = "Student"
    private lateinit var page: LinearLayout
    private fun dp(v:Int)= (v*resources.displayMetrics.density).toInt()
    override fun onCreate(savedInstanceState: Bundle?) { super.onCreate(savedInstanceState); showLogin() }
    private fun base(title:String) {
        val scroll=ScrollView(this)
        page=LinearLayout(this).apply { orientation=LinearLayout.VERTICAL; setPadding(dp(22),dp(24),dp(22),dp(24)); setBackgroundColor(Color.rgb(248,249,252)) }
        scroll.addView(page); setContentView(scroll)
        page.addView(TextView(this).apply { text=title; textSize=24f; setTextColor(navy); setTypeface(null,1); setPadding(0,0,0,dp(18)) })
    }
    private fun field(label:String, hint:String=label, password:Boolean=false): EditText {
        page.addView(TextView(this).apply { text=label; textSize=14f; setTextColor(Color.DKGRAY); setPadding(0,dp(8),0,dp(4)) })
        return EditText(this).apply { this.hint=hint; textSize=16f; setSingleLine(true); if(password) inputType=129; page.addView(this, LinearLayout.LayoutParams(-1,dp(52))) }
    }
    private fun button(label:String, color:Int, action:()->Unit) { page.addView(Button(this).apply { text=label; setTextColor(if(color==gold) Color.BLACK else Color.WHITE); setBackgroundColor(color); setOnClickListener { action() } }, LinearLayout.LayoutParams(-1,dp(54)).apply { topMargin=dp(12) }) }
    private fun note(text:String) { page.addView(TextView(this).apply { this.text=text; textSize=13f; setTextColor(maroon); setPadding(0,dp(12),0,dp(8)) }) }
    private fun showLogin() {
        loggedIn=false; base("STUDENT REGISTRATION")
        val email=field("Email address"); val pass=field("Password",password=true)
        button("LOGIN",navy) { if(email.text.isNotBlank() && pass.text.isNotBlank()) { loggedIn=true; studentName=email.text.toString().substringBefore('@'); showDashboard() } else note("Enter your email and password.") }
        button("CREATE ACCOUNT",maroon) { showSignup() }; note("Registration fee: UGX 30,000
Android preview uses local demo data; it is not connected to your XAMPP database.")
    }
    private fun showSignup() {
        base("CREATE STUDENT ACCOUNT")
        val number=field("Student number"); val name=field("Full name"); val email=field("Email address"); val pass=field("Password",password=true); val confirm=field("Confirm password",password=true)
        button("CREATE ACCOUNT",navy) { if(number.text.isBlank()||name.text.isBlank()||email.text.isBlank()||pass.text.length<8||pass.text.toString()!=confirm.text.toString()) note("Complete all fields. Passwords must match and contain at least 8 characters.") else { studentName=name.text.toString(); loggedIn=true; showDashboard() } }
        button("BACK TO LOGIN",maroon) { showLogin() }
    }
    private fun showDashboard() {
        if(!loggedIn) { showLogin(); return }; base("STUDENT PORTAL")
        note("Welcome, $studentName")
        button("STUDENT REGISTRATION",navy) { showRegistration() }
        button("REGISTRATION PAYMENT",maroon) { showPayment() }
        button("LOG OUT",gold) { loggedIn=false; showLogin() }
    }
    private fun showRegistration() {
        if(!loggedIn) { showLogin(); return }; base("STUDENT REGISTRATION FORM")
        val first=field("First name *"); val last=field("Last name *"); val phone=field("Phone number *"); val address=field("Residential address")
        val programme=Spinner(this); programme.adapter=ArrayAdapter(this,android.R.layout.simple_spinner_dropdown_item,listOf("Select programme","Bachelor of Information Technology","Bachelor of Arts with Education","Bachelor of Business Administration","Bachelor of Nursing Science","Diploma Programme","Other")); page.addView(programme)
        val gender=RadioGroup(this).apply { orientation=RadioGroup.HORIZONTAL; addView(RadioButton(this@MainActivity).apply { text="Male" }); addView(RadioButton(this@MainActivity).apply { text="Female" }) }; page.addView(gender)
        val agree=CheckBox(this).apply { text="I confirm the information is correct."; setTextColor(Color.DKGRAY) }; page.addView(agree)
        button("SAVE REGISTRATION",navy) { if(first.text.isBlank()||last.text.isBlank()||phone.text.isBlank()||programme.selectedItemPosition==0||gender.checkedRadioButtonId==-1||!agree.isChecked) note("Complete all required fields and accept the declaration.") else note("Demo preview: registration details accepted on this device. Connect a backend for permanent database storage.") }
        button("BACK TO DASHBOARD",maroon) { showDashboard() }
    }
    private fun showPayment() {
        if(!loggedIn) { showLogin(); return }; base("REGISTRATION PAYMENT")
        note("AMOUNT DUE: UGX 30,000")
        val methods=Spinner(this); methods.adapter=ArrayAdapter(this,android.R.layout.simple_spinner_dropdown_item,listOf("Cash at accounts office","Bank deposit","Mobile money - manual confirmation")); page.addView(methods)
        val confirm=CheckBox(this).apply { text="I confirm that I have made / will present this payment." }; page.addView(confirm)
        button("RECORD DEMO PAYMENT",navy) { if(confirm.isChecked) { base("PAYMENT RECORDED"); note("A demo payment confirmation was displayed. No money has been transferred."); button("BACK TO DASHBOARD",gold) { showDashboard() } } else note("Please confirm the payment declaration first.") }
        button("BACK TO DASHBOARD",maroon) { showDashboard() }
    }
}

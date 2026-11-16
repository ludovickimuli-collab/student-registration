# Windows VB.NET application

This is the Windows desktop version. It requires Windows and .NET 8 SDK with the **.NET desktop development** workload in Visual Studio 2022.

1. Start MySQL in XAMPP.
2. Import `Database/student_registration_db.sql` in phpMyAdmin.
3. Open `StudentRegistrationSystem.vbproj` in Visual Studio.
4. Restore NuGet packages, then Build > Rebuild Solution.
5. Run with F5.

Database defaults: host `localhost`, port `3306`, database `student_registration_db`, user `root`, blank password. If your XAMPP root account has a password, set the Windows environment variable `STUDENT_DB_PASSWORD` before launching Visual Studio. Optional host override: `STUDENT_DB_HOST`.

The UGX 30,000 payment screen is a demonstration database record only. It does not process Mobile Money, bank or card payments. Integrate an authorised payment provider before accepting real payments.

The forms are created in VB code, not Visual Studio drag-and-drop `.Designer.vb` files.

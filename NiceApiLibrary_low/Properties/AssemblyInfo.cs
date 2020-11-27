using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("NiceApiLibrary_low")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("NiceApiLibrary_low")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("3ce043b7-ea8e-493d-afd8-bec5c97978dc")]

//
//	1.0.0. 1	11/10/2017	initial version for NiceApi.net
//  1.0.0. 2	16/10/2017	some changes before going online
//  1.0.0. 3    16/10       Data_TrayUserFile removed
//  1.0.0. 4    16/10       Email credentials
//  1.0.0. 5    20/10       Dont log FileNotFoundException for admin_at_niceapi_dot_net
//  1.0.0. 6    24/10       missing Base64_URLEncoding added
//  1.0.0. 7    25/10       allow shorter username
//  1.0.0. 8    02/11       support for group send
//  1.0.0. 9    02/11       store all mobile numbers in one string
//  1.0.0.10    08/11       Changes to support latest Desktop App (DB config)
//  1.0.0.11    08/11       Support email tracking in database
//  1.0.0.12    22/11       Changed JustRegisterd and LittleActivity email (now contins all the details)
//  1.0.0.13    27/11       Data_MessageFile with FailedCounter (V3)
//  1.0.0.14    27/11       Data_MessageFile with FailedCounter (V3) bug fix
//  1.0.0.15    28/11       IsDevMachine added / Data_MessageFile.NoCounterUpdate
//  1.0.0.16    29/11       So many improvments like NoCounterUpdate on welcome message
//  1.0.0.17    06/12       Little activity email: double line removed
//  1.0.0.18    21/12       User file with Comment and SendFooter
//  1.0.0.19    31/01/2018  User.AccountStatusExplained added
//  1.0.0.20    10/03/2018  Usage% added to Data_AppUserFile
//  1.0.0.21    10/03/2018  Hardware upgrade 3.2018 email added
//  1.0.0.22    21/04/2018  trayASPURL_Local chaged
//  1.0.0.23    24/04/2018  user.WelcomeCounter added / user.MobileNumberCount added /  SortByUsage added
//  1.0.0.24    25/04/2018  added wrong tel email / comment to user
//  1.0.0.25    02/05/2018  BinBast64StreamHelper class added
//  1.0.0.26    14/05/2018  SendUpgradeRequestOnHighPercent email added
//  1.0.0.27    29/05/2018  Changes to support commercial accounts
//  1.0.0.28    29/06/2018  Wallet supported
//  1.0.0.29    10/07/2018  RemainingMessages -1 to disable the feature (For pay-monthly accounts)
//  1.0.0.30    10/07/2018  TelListController added
//  1.0.0.31    07/08/2018  Account Split for Free, Monthly and PayAsSent
//  1.0.1.0     28/08/2018  ASP-Tray communication recoded
//  1.0.1.1     27/09/2018  TelListController improvements
//  1.0.1.2     05/10/2018  TelCheck files added to ASP-Tray communication and more
//  1.0.1.3     16/10/2018  Priv, Add tel to free account bugfix.
//  1.0.1.4     17/10/2018  Comm, Add tel to commercial accounts.
//  1.0.1.5     17/10/2018  Comm, Add tel to commercial accounts. small fix where we dont want to send an empty check file to the tray.
//  1.0.1.6     27/10/2018  Comm, Add tel to commersial accounts. FundManagement should now deduct on Add Tel
//  1.0.1.7     07/11/2018  Allow free account tel add if account is blocked
//  1.0.1.8     16/11/2018  ...
//  1.0.1.9     16/11/2018  s_MsgFile_GetPartsFromMessageFile works also on old files ("Msg_")
//  1.0.1.10    28/11/2018  Limit message size to 10K
//  1.0.1.11    11/02/2019  Changed Reg_TelWrong_EmailText 
//	1.0.2.0		06/12/2018	Library to support sqlite
//	1.0.2.1		07/12/2018	Final amendments while testing
//	1.0.2.2		07/12/2018	File handling with _ in file tried to improve.
//	1.0.2.3		16/04/2019  Sql query support
//  1.0.2.4     24/04/2019  Further improvements before testing
//  1.0.2.5     30.04.2019  DisplayStatus for Tray status added.
//  1.0.2.6     01.05.2019  Tampering with DSSwitch.StartUp
//  1.0.2.7     01.05.2019  ProcessSql implemented for SQLite
//  1.0.2.8     03.05.2019  DataTable2WebControlsTable added
//  1.0.2.9     15.05.2019  Lib split to NiceApiLibrary_low and NiceApiLibrary
//  1.0.2.10    19.07.2019  Support for SystemDuplication upgrades / IData_AppUserWalletHandling.DeleteOne / AccountImportance
//  1.0.2.11    22.07.2019  ASPTrayBase.GetNiceStatus added
//  1.0.2.12    29.08.2019  SysDup Text added for Invoice issueing
//  1.0.2.13    17.10.2019  Support for monthly account with levels
//  1.0.2.14    31.01.2020  New pricing for SystemDuplication
//  1.0.3.00    17.03.2020  So Many changes, full support for MultipleSystems ....
//  1.0.3.01    27.03.2020  trying getting Loopback to work
//  1.0.3.02    30.09.2020  With DirectTel
[assembly: AssemblyVersion("1.0.3.02")]
[assembly: AssemblyFileVersion("1.0.3.02")]

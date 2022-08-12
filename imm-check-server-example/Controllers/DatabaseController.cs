using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using imm_check_server_example.Models;

namespace imm_check_server_example.Controllers
{
	[Route("api/[Controller]")]
	public class DatabaseController : Controller {
	[HttpGet]
	public List<ImmDocI20> GetData() {

		// Populate with the result of the queries.
		List<ImmDocI20> I20s = new List<ImmDocI20>();
		List<ImmDocDS2019info> DS2019s = new List<ImmDocDS2019info>();
		List<ImmDocI94> I94s = new List<ImmDocI94>();
		List<ImmDocPassport> Passports = new List<ImmDocPassport>();
		List<ImmDocVisaInfo> VisaInfos = new List<ImmDocVisaInfo>();
		List<SponsoredStudentInformation> SponsoredStudentInformation = new List<SponsoredStudentInformation>();

		// GetFullPath will complete the path for the file named passed in as a string.
		
		string dataSource = "Data Source=" + Path.GetFullPath("imm-doc-check.db");
		//string dataSource = $"Data Source=C:\\Users\\sevic\\Code\\github\\open-source-immigration-ssi\\imm-check-server-example\\imm-doc-check.db";
		// using will make sure that the resource is cleaned from memory after it exists
		// conn initializes the connection to the .db file.
		using(SqliteConnection conn = new SqliteConnection(dataSource)) {

			conn.Open();

			// sql is the string that will be run as an sql command
			string sqlI20Program = $"SELECT * FROM I20Program;";
			string sqlDS2019Info = $"SELECT * FROM ds2019info;";
			string sqlImmDocI94 = $"SELECT * FROM I94;";
			string sqlImmDocPassport = $"SELECT * FROM passport;";
			string sqlVisaInfo = $"SELECT * FROM sponsoredStudentInformation;";
			string sqlSponsoredStudentInformation = $"SELECT * FROM sponsoredStudentInformation;";

			// command combines the connection and the command string and creates the query
			using(SqliteCommand command = new SqliteCommand(sqlI20Program, conn)) {
				// reader allows you to read each value that comes back and do something to it.
				using(SqliteDataReader reader = command.ExecuteReader()) {
					// Read returns true while there are more rows to advance to. then false when done.
					while (reader.Read()) {
						ImmDocI20 newI20 = new ImmDocI20() {
                            recnum = reader.GetInt32(0),
//							idnumber = reader.GetValue(1).ToString(),
							idnumber = reader.GetInt32(1),
							sevisid = reader.GetString(2),
							status = reader.GetString(3),
							eduLevel = reader.GetString(4),
							eduComments = reader.GetString(5),
							primaryMajor = reader.GetString(6),
							secondMajor = reader.GetString(7),
							minor = reader.GetString(8),
							lengthOfStudy = reader.GetString(9),
							prgStartDate = reader.GetString(10),
							prgEndDate = reader.GetString(11),
							engRequired = reader.GetString(12),
							engRequirementsMet = reader.GetString(13),
							engNotRequired = reader.GetString(14),
							datestamp = reader.GetString(15),
							institutionalKey = reader.GetString(16),
							issDate = reader.GetString(17)
						};
						// add each one to the list.
						I20s.Add(newI20);
					}
				}
			}
			// command combines the connection and the command string and creates the query
			using(SqliteCommand command = new SqliteCommand(sqlDS2019Info, conn)) {
				// reader allows you to read each value that comes back and do something to it.
				using(SqliteDataReader reader = command.ExecuteReader()) {
					// Read returns true while there are more rows to advance to. then false when done.
					while (reader.Read()) {
						ImmDocDS2019info newDS2019 = new ImmDocDS2019info() {
                            recnum = reader.GetInt32(0),
							idnumber = reader.GetInt32(1),
							sponsor = reader.GetString(2),
							orgCode = reader.GetString(3),
							sevisid = reader.GetString(4),
							startProgram = reader.GetString(5),
							endProgram = reader.GetString(6),
							official = reader.GetString(7),
							street1 = reader.GetString(8),
							street2 = reader.GetString(9),
							city = reader.GetString(10),
							state = reader.GetString(11),
							postal = reader.GetString(12),
							phone = reader.GetString(13),
							email = reader.GetString(14),
							directBill = reader.GetString(15),
							datestamp = reader.GetString(16)
						};
						DS2019s.Add(newDS2019);
					}
				}
			}						
			// command combines the connection and the command string and creates the query
			using(SqliteCommand command = new SqliteCommand(sqlImmDocI94, conn)) {
				// reader allows you to read each value that comes back and do something to it.
				using(SqliteDataReader reader = command.ExecuteReader()) {
					// Read returns true while there are more rows to advance to. then false when done.
					while (reader.Read()) {
						// map the data to the model.
						ImmDocI94 newI94 = new ImmDocI94() {
                            recnum = reader.GetInt32(0),
							idnumber = reader.GetInt32(1),
							i94num = reader.GetString(2),
							i94poe = reader.GetString(3),
							i94iss = reader.GetString(4),
							i94exp = reader.GetString(5),
							i94expds = reader.GetString(6),
							datestamp = reader.GetString(7)
						};
						// add each one to the list.
						I94s.Add(newI94);
					}
				}
			}
			// command combines the connection and the command string and creates the query
			using(SqliteCommand command = new SqliteCommand(sqlImmDocPassport, conn)) {
				// reader allows you to read each value that comes back and do something to it.
				using(SqliteDataReader reader = command.ExecuteReader()) {
					// Read returns true while there are more rows to advance to. then false when done.
					while (reader.Read()) {
						ImmDocPassport newPassport = new ImmDocPassport() {
                            recnum = reader.GetInt32(0),
							idnumber = reader.GetInt32(1),
							givenName = reader.GetString(2),
							lastname = reader.GetString(3),
							cpass = reader.GetString(4),
							passnum = reader.GetString(5),
							passiss = reader.GetString(6),
							passexp = reader.GetString(7),
							datestamp = reader.GetString(8)
						};
						Passports.Add(newPassport);
					}
				}
			}
			// command combines the connection and the command string and creates the query
			using(SqliteCommand command = new SqliteCommand(sqlVisaInfo, conn)) {
				// reader allows you to read each value that comes back and do something to it.
				using(SqliteDataReader reader = command.ExecuteReader()) {
					// Read returns true while there are more rows to advance to. then false when done.
					while (reader.Read()) {
						ImmDocVisaInfo newVisaInfo = new ImmDocVisaInfo() {
                            recnum = reader.GetInt32(0),
							idnumber = reader.GetInt32(1),
							status = reader.GetString(2),
							visastamp = reader.GetString(3),
							stampnum = reader.GetString(4),
							controlnumber = reader.GetString(5),
							stampplace = reader.GetString(6),
							stampent = reader.GetString(7),
							stampiss = reader.GetString(8),
							stampexp = reader.GetString(8),
							datestamp = reader.GetString(8)
						};
						VisaInfos.Add(newVisaInfo);
					}
				}
			}						
			// command combines the connection and the command string and creates the query
			using(SqliteCommand command = new SqliteCommand(sqlSponsoredStudentInformation, conn)) {
				// reader allows you to read each value that comes back and do something to it.
				using(SqliteDataReader reader = command.ExecuteReader()) {
					// Read returns true while there are more rows to advance to. then false when done.
					while (reader.Read()) {
						SponsoredStudentInformation newSponsoredStudentInformation = new SponsoredStudentInformation() {
                            recnum = reader.GetInt32(0),
							idnumber = reader.GetInt32(1),
							sponsor = reader.GetString(2),
							fundingSponsor = reader.GetString(3),
							advisorName = reader.GetString(4),
							advisorEmail = reader.GetString(5),
							advisorPhone = reader.GetString(6),
							isSponsoredStudent = reader.GetString(7),
							datestamp = reader.GetString(8),
							isFeeExempt = reader.GetString(8),
							isUSGovSponsored = reader.GetString(8),
							needsFinancialDocs = reader.GetString(8),
							financialDocsExpDate = reader.GetString(8)
						};
						SponsoredStudentInformation.Add(newSponsoredStudentInformation);
					}
				}
			}
			conn.Close();
		}
		// List<ImmDocI20> I20s = new List<ImmDocI20>();
		// var immigrationDocuments = new ImmDocCollection[]
		// {

		// };
//I20s
		foreach (var i20s in I20s)
		{
			//Do Something
			Console.WriteLine("i20s.sevisid: ");
			Console.WriteLine(i20s.sevisid);
		}

/*
https://stackoverflow.com/questions/24999363/add-level-to-json-object-in-c-sharp

https://www.softwaretestinghelp.com/create-json-structure-using-c/

        public ImmDocI20? immDocI20 { get; set; }
        public ImmDocDS2019info? immDocDS2019info { get; set; }
        public ImmDocI94? immDocI94 { get; set; }
        public ImmDocPassport? immDocPassport { get; set; }
        public ImmDocVisaInfo? immDocVisaInfo { get; set; }
        public SponsoredStudentInformation?

*/
		return I20s;
	}
}

}
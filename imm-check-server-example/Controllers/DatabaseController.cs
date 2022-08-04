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

		// tracks will be populated with the result of the query.
		List<ImmDocI20> I20s = new List<ImmDocI20>();

		// GetFullPath will complete the path for the file named passed in as a string.
		string dataSource = "Data Source=" + Path.GetFullPath("../imm-doc-check.db");

		// using will make sure that the resource is cleaned from memory after it exists
		// conn initializes the connection to the .db file.
		using(SqliteConnection conn = new SqliteConnection(dataSource)) {

			conn.Open();

			// sql is the string that will be run as an sql command
			string sql = $"SELECT * FROM I20Program limit 200;";

			// command combines the connection and the command string and creates the query
			using(SqliteCommand command = new SqliteCommand(sql, conn)) {

				// reader allows you to read each value that comes back and do something to it.
				using(SqliteDataReader reader = command.ExecuteReader()) {

					// Read returns true while there are more rows to advance to. then false when done.
					while (reader.Read()) {
	/*I20Program:
	-------------
		[recnum] [int] IDENTITY(1,1) NOT NULL,
		[idnumber] [int] NOT NULL,
		[sevisid] [nvarchar](11) NULL,
		[status] [nvarchar](2) NULL,
		[eduLevel] [nvarchar](2) NULL,
		[eduComments] [nvarchar](500) NULL,
		[primaryMajor] [nvarchar](10) NULL,
		[secondMajor] [nvarchar](10) NULL,
		[minor] [nvarchar](10) NULL,
		[lengthOfStudy] [nvarchar](2) NULL,
		[prgStartDate] [datetime] NULL,
		[prgEndDate] [datetime] NULL,
		[engRequired] [bit] NOT NULL,
		[engRequirementsMet] [bit] NOT NULL,
		[engNotRequired] [nvarchar](500) NULL,
		[datestamp] [datetime] NOT NULL,
		[institutionalKey] [nvarchar](100) NULL,
		[issDate] [datetime] NULL)


		[I94]
		*****
		[recnum] [int] IDENTITY(1,1) NOT NULL,
		[idnumber] [int] NOT NULL,
		[i94num] [nvarchar](11) NULL,
		[i94poe] [nvarchar](5) NULL,
		[i94iss] [datetime] NULL,
		[i94exp] [datetime] NULL,
		[i94expds] [bit] NOT NULL,
		[datestamp] [datetime] NOT NULL)


	[ds2019info]
	************
		[recnum] [int] IDENTITY(1,1) NOT NULL,
		[idnumber] [int] NOT NULL,
		[sponsor] [nvarchar](200) NULL,
		[orgCode] [nvarchar](20) NULL,
		[sevisid] [nvarchar](11) NULL,
		[startProgram] [datetime] NULL,
		[endProgram] [datetime] NULL,
		[official] [nvarchar](200) NULL,
		[street1] [nvarchar](255) NULL,
		[street2] [nvarchar](255) NULL,
		[city] [nvarchar](60) NULL,
		[state] [nvarchar](2) NULL,
		[postal] [nvarchar](20) NULL,
		[phone] [nvarchar](20) NULL,
		[email] [nvarchar](255) NULL,
		[directBill] [bit] NULL,
		[datestamp] [datetime] NOT NULL)


	[passport]
	**********
		[recnum] [int] IDENTITY(1,1) NOT NULL,
		[idnumber] [int] NOT NULL,
		[givenName] [nvarchar](101) NULL,
		[lastname] [nvarchar](50) NULL,
		[cpass] [nvarchar](5) NULL,
		[passnum] [nvarchar](255) NULL,
		[passiss] [datetime] NULL,
		[passexp] [datetime] NULL,
		[datestamp] [datetime] NOT NULL)


	[visaInfo]
	**********
		[recnum] [int] IDENTITY(1,1) NOT NULL,
		[idnumber] [int] NOT NULL,
		[status] [nvarchar](10) NULL,
		[visastamp] [nvarchar](10) NULL,
		[stampnum] [nvarchar](255) NULL,
		[controlnumber] [nvarchar](255) NULL,
		[stampplace] [nvarchar](50) NULL,
		[stampent] [nvarchar](5) NULL,
		[stampiss] [datetime] NULL,
		[stampexp] [datetime] NULL,
		[datestamp] [datetime] NOT NULL)


	[sponsoredStudentInformation]
	*****************************
		[recnum] [int] IDENTITY(1,1) NOT NULL,
		[idnumber] [int] NOT NULL,
		[sponsor] [nvarchar](200) NOT NULL,
		[fundingSponsor] [nvarchar](200) NULL,
		[advisorName] [nvarchar](100) NULL,
		[advisorEmail] [nvarchar](255) NULL,
		[advisorPhone] [nvarchar](20) NULL,
		[isSponsoredStudent] [bit] NOT NULL,
		[datestamp] [datetime] NOT NULL,
		[isFeeExempt] [bit] NULL,
		[isUSGovSponsored] [bit] NULL,
		[needsFinancialDocs] [bit] NULL,
		[financialDocsExpDate] [datetime] NULL)
	*/
						// map the data to the model.
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
			conn.Close();
		}
		return I20s;
	}
}

}
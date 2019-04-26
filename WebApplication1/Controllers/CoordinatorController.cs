using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CoordinatorController : ApiController
    {
        public Coordinator Get(string algo, int NoOfParty = 0)
        {

            int myInitiator = 0;
            string str = "Election has started, ";
            Random r = new Random();
            int rInt = r.Next(1, 50);
            
            SqlConnection strcon3 = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString.ToString());
            if (algo.Equals("random"))
            {
                //generate election#
                myInitiator = r.Next(1, NoOfParty); //generate initiator
                str += "Algorithm in use is = Random ,";

            }
            else
            {
                str += "Algorithm in use is = High Availibilty ,";
                SqlCommand cmdHigh = new SqlCommand("SELECT TOP 1 availibility, participant FROM electTable ORDER BY availibility DESC", strcon3);
                strcon3.Open();
                SqlDataReader rdHIgh = cmdHigh.ExecuteReader();
                while (rdHIgh.Read())
                {
                    myInitiator = Convert.ToInt32(rdHIgh["participant"]);

                }
                rdHIgh.Close();
                strcon3.Close();
            }

            str += "The Initiator is " + myInitiator + ", ";
            str += "The Election Number is " + rInt + ", ";


            RunAsync(myInitiator, algo, rInt, NoOfParty).Wait();
            str += "Votes Received, Coordinator is counting ,";
            int newWinner = 0;
            int checkFlag = 0;
            Boolean checkWinner;
            // string strcon3 = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString.ToString();
            //SqlConnection strcon3 = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString.ToString());
            SqlCommand cmd5 = new SqlCommand("SELECT COUNT(*) countValue FROM (SELECT vote, COUNT(vote) counting, MAX(prevWinner) as prewinner " +
                    "FROM electTable GROUP BY vote " +
                    "HAVING COUNT(vote) = (" +
                    "SELECT MAX(mycount) " +
                    "FROM (" +
                    "SELECT vote, COUNT(*) mycount " +
                    "FROM [electTable] " +
                    "GROUP BY vote) AS c)) AS d;", strcon3);
            strcon3.Open();
            SqlDataReader rd3 = cmd5.ExecuteReader();
            while (rd3.Read())
            {
                checkFlag = Convert.ToInt32(rd3["countValue"]);


            }
            rd3.Close();
            strcon3.Close();


            SqlCommand cmd4 = new SqlCommand("SELECT vote, COUNT(vote), MAX(prevWinner) as prewinner " +
                "FROM electTable GROUP BY vote " +
                "HAVING COUNT(vote) = (" +
                "SELECT MAX(mycount) " +
                "FROM (" +
                "SELECT vote, COUNT(*) mycount " +
                "FROM [electTable] " +
                "GROUP BY vote) AS c);", strcon3);
            strcon3.Open();
            SqlDataReader rdWin = cmd4.ExecuteReader();
            while (rdWin.Read())
            {
                //check if tie
                if (checkFlag > 1)
                {
                    if ((Convert.ToInt32(rdWin["prewinner"])) == 0)
                        newWinner = Convert.ToInt32(rdWin["vote"]);
                    else
                    { }
                }
                else if(checkFlag == 1)
                    newWinner = Convert.ToInt32(rdWin["vote"]);

            }
            rdWin.Close();
            strcon3.Close();

            strcon3.Open();
            //Update table with preVwinner Flag for next election
            string delSqlCotrol = "UPDATE electTable " +
                "SET prevWinner = " +
                "CASE participant " +
                "WHEN " + newWinner + " THEN 1 " +
                "ELSE 0 " +
                " END ";
            SqlCommand cmd = new SqlCommand(delSqlCotrol, strcon3);
            cmd.ExecuteNonQuery();

            //Create a string to display on UI
            string getSqlCotrol = "SELECT vote, COUNT(*) mycount " +
                                    "FROM [electTable] " +
                                    "GROUP BY vote";
            SqlCommand cmdSqlCotrol = new SqlCommand(getSqlCotrol, strcon3);
            SqlDataReader rd5 = cmdSqlCotrol.ExecuteReader();
            while (rd5.Read())
            {
                str += "Participant " + Convert.ToInt32(rd5["vote"]) + "= Vote Count is " + Convert.ToInt32(rd5["mycount"]) + ",";

            }
            rd5.Close();
            strcon3.Close();
            str += "Final Winner is " + newWinner;
            return new Coordinator
            {
                Winner = newWinner,
                ResponseMsg = str

            };

        }

        public static async Task RunAsync(int myInitiator, string algo, int rInt, int NoOfParty)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:50221");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    Console.WriteLine("GET");
                    string url = "api/Participant" + myInitiator + "?ElecNo=" + rInt + "&algo=" + algo + "&NoOfParty=" + NoOfParty;
                    var response = client.GetAsync(url);
                    response.Wait();
                    
                }
                catch (Exception)
                {
                    throw;
                }


            }

        }
    }
}

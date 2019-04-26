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
    public class Participant2Controller : ApiController
    {
        // GET: api/Participant2

        public Participant2 Get(string algo)
        {
            int vote = 6;
            if (algo == "random")
            {
                Random r = new Random();
                int rInt = r.Next(1, 4); //for ints
                vote = rInt;
            }
            else
            {
                vote = 1;
            }
            // RunAsync().Wait();

            return new Participant2
            {
                Winner = vote,
                ResponseTime = 20,
                Availability = 10,

            };

        }
        public Participant2 Get(int ElecNo, string algo, int NoOfParty=0)
        {
            int vote = 1;
            //int availibility = 5;
            SqlConnection strcon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString.ToString());
            if (algo == "random")
            {
                Random r = new Random();
                int rInt = r.Next(1, 4); //for ints
                vote = rInt;
            }
            else
            {
                SqlCommand cmdHigh = new SqlCommand("SELECT TOP 1 availibility, participant FROM electTable ORDER BY availibility DESC", strcon);
                strcon.Open();
                SqlDataReader rdHIgh = cmdHigh.ExecuteReader();
                while (rdHIgh.Read())
                {
                    vote = Convert.ToInt32(rdHIgh["participant"]);

                }
                rdHIgh.Close();
                strcon.Close();
            }
               // vote = availibility;

            RunAsync(ElecNo, algo, NoOfParty).Wait();

            
            strcon.Open();

          
            //add new entry
            string sql = "UPDATE electTable set electionNo =" + ElecNo + ",vote =" + vote + " WHERE participant=2";
            SqlCommand cmdInsert = new SqlCommand(sql, strcon);
            cmdInsert.ExecuteNonQuery();
            strcon.Close();

            return new Participant2
            {
                Winner = 2,
                ResponseTime = 20,
                Availability = 10,
                ElectionNo = ElecNo,

            };

        }
        public Participant2 Get()
        {
            int myString = 0;
            int myVote = 0;
            SqlConnection strcon1 = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString.ToString());

            SqlCommand cmdNew = new SqlCommand("SELECT * FROM [electionTable] WHERE participant=2", strcon1);
            strcon1.Open();
            SqlDataReader rd = cmdNew.ExecuteReader();
            while (rd.Read())
            {
                myString = Convert.ToInt32(rd["electionNo"]);
                myVote = Convert.ToInt32(rd["vote"]);

            }
            rd.Close();
            strcon1.Close();

            return new Participant2
            {
                Winner = myVote,
                ResponseTime = 20,
                Availability = 10,
                ElectionNo = myString,

            };

        }
        public static async Task RunAsync(int ElecNo, string algo, int NoOfParty)
        {
            using (var client2 = new HttpClient())
            {
                client2.BaseAddress = new Uri("http://localhost:50221");
                client2.DefaultRequestHeaders.Accept.Clear();
                client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                for (int i = 1; i <= NoOfParty; i++)
                {
                    try
                    {
                        Console.WriteLine("GET");
                        string url = "api/Participant" + i + "?ElecNo=" + ElecNo + "&algo=" + algo;
                        var response = client2.GetAsync(url);
                        response.Wait();


                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

            }

        }
        /* public IEnumerable<string> Get()
         {
             return new string[] { "value1", "value2" };
         }

         // GET: api/Participant2/5
         public string Get(int id)
         {
             return "value";
         }

         // POST: api/Participant2
         public void Post([FromBody]string value)
         {
         }

         // PUT: api/Participant2/5
         public void Put(int id, [FromBody]string value)
         {
         }

         // DELETE: api/Participant2/5
         public void Delete(int id)
         {
         }*/
    }
}

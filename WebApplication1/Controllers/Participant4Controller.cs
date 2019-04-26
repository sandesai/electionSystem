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
    public class Participant4Controller : ApiController
    {
        //Acting as the initiator
        public static async Task RunAsync(int ElecNo, string algo, int NoOfParty)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:50221");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                for (int i = 1; i <= NoOfParty; i++)
                {
                    try
                    {
                        Console.WriteLine("GET");
                        string url = "api/Participant" + i + "?ElecNo=" + ElecNo + "&algo=" + algo;
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
        //for initiator
        public Participant3 Get(int ElecNo, string algo, int NoOfParty=0)
        {
            int vote = 1;
            int availibility = 100;
            SqlConnection strcon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString.ToString());
            if (algo == "random")
            {
                Random r = new Random();
                int rInt = r.Next(1, 4); //for ints
                vote = rInt;
            }
            else
            {
                SqlCommand cmdHighP3 = new SqlCommand("SELECT TOP 1 availibility, participant FROM electTable ORDER BY availibility DESC", strcon);
                strcon.Open();
                SqlDataReader rdHIghP3 = cmdHighP3.ExecuteReader();
                while (rdHIghP3.Read())
                {
                    vote = Convert.ToInt32(rdHIghP3["participant"]);

                }
                rdHIghP3.Close();
                strcon.Close();
            }

            RunAsync(ElecNo, algo, NoOfParty).Wait();
          
            strcon.Open();

            //clear table
            string delSql = "DELETE FROM electionTable WHERE participant=3";
            SqlCommand cmd = new SqlCommand(delSql, strcon);
            cmd.ExecuteNonQuery();

            //add new entry
            string sql = "UPDATE electTable set electionNo =" + ElecNo + ",vote =" + vote + " WHERE participant=4";
            SqlCommand cmdInsert = new SqlCommand(sql, strcon);
            cmdInsert.ExecuteNonQuery();
            strcon.Close();

            return new Participant3
            {
                Winner = 2,
                ResponseTime = 20,
                Availability = 10,
                ElectionNo = ElecNo,

            };

        }
        //Acting as participant 
        public Participant4 Get()
        {
            int myString = 0;
            int myVote = 0;
            SqlConnection strcon1 = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString.ToString());

            SqlCommand cmdNew = new SqlCommand("SELECT * FROM [electionTable] WHERE participant=3", strcon1);
            strcon1.Open();
            SqlDataReader rd = cmdNew.ExecuteReader();
            while (rd.Read())
            {
                myString = Convert.ToInt32(rd["electionNo"]);
                myVote = Convert.ToInt32(rd["vote"]);

            }
            rd.Close();
            strcon1.Close();

            return new Participant4
            {
                Winner = myVote,
                ResponseTime = 20,
                Availability = 10,
                ElectionNo = myString,

            };

        }
    }
}

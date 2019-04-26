using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class Participant1Controller : ApiController
    {
       /* public Participant1 Get(string algo)
        {
            int vote=6;
            if (algo == "random")
            {
                Random r = new Random();
                int rInt = r.Next(1, 4); //for ints
                vote = rInt;
            }
            else {
                vote = 1;
            }
            // RunAsync().Wait();

            return new Participant1
            {
                Winner = vote,
                ResponseTime = 20,
                Availability = 10,
                
            };

        }*/
        public Participant1 Get(int ElecNo, string algo, int NoOfParty=0)
        {
            int vote = 1;
            int availibility = 30;
            SqlConnection strcon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString.ToString());
            if (algo == "random")
            {
                Random r = new Random();
                int rInt = r.Next(1, 4); //for ints
                vote = rInt;
            }
            else
            {
                SqlCommand cmdHighP1 = new SqlCommand("SELECT TOP 1 availibility, participant FROM electTable ORDER BY availibility DESC", strcon);
                strcon.Open();
                SqlDataReader rdHIghP1 = cmdHighP1.ExecuteReader();
                while (rdHIghP1.Read())
                {
                    vote = Convert.ToInt32(rdHIghP1["participant"]);

                }
                rdHIghP1.Close();
                strcon.Close();
            }

            RunAsync(ElecNo, algo, NoOfParty).Wait();

           
            strcon.Open();

            //clear table
            string delSql = "UPDATE electTable set electionNo =" + ElecNo+",vote =" + vote+ " WHERE participant=1";
            SqlCommand cmd = new SqlCommand(delSql, strcon);
            cmd.ExecuteNonQuery();

            /*string delSql = "DELETE FROM electionTable WHERE participant=1";
            SqlCommand cmd = new SqlCommand(delSql, strcon);
            cmd.ExecuteNonQuery();*/

            //add new entry
           /* string sql = "insert into electionTable(electionNo,availibility,participant,vote) values (" + ElecNo + ","+ availibility+ ",1,"+vote+")";
            SqlCommand cmdInsert = new SqlCommand(sql, strcon);
            cmdInsert.ExecuteNonQuery();*/
            strcon.Close();

            return new Participant1
            {
                Winner = 2,
                ResponseTime = 20,
                Availability = 10,
                ElectionNo = ElecNo,

            };

        }
        public Participant1 Get()
        {

          
           int myString=0;
            int myVote = 0;
           SqlConnection strcon1 = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString.ToString());
            
            SqlCommand cmdNew = new SqlCommand("SELECT * FROM [electionTable] WHERE participant=1", strcon1);
            strcon1.Open();
            SqlDataReader rd = cmdNew.ExecuteReader();
            while (rd.Read())
            {
                myString = Convert.ToInt32(rd["electionNo"]);
                myVote = Convert.ToInt32(rd["vote"]);

            }
            rd.Close();
            strcon1.Close();

            return new Participant1
            {
                Winner = myVote,
                ResponseTime = 20,
                Availability = 10,
               ElectionNo = myString,

            };

        }
        
        public static async Task RunAsync(int ElecNo, string algo, int NoOfParty)
        {
            using (var client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri("http://localhost:50221");
                client1.DefaultRequestHeaders.Accept.Clear();
                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                for (int i = 1; i <= NoOfParty; i++)
                {
                    try
                    {
                        Console.WriteLine("GET");
                        
                        string url = "api/Participant" + i + "?ElecNo=" + ElecNo + "&algo=" +algo;
                        var response = client1.GetAsync(url);
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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class WeatherController : ApiController
    {
        // GET: api/Weather
       
       /* public IEnumerable<WeatherInfo> Get()
        {
            var weatherInfoList = new List<WeatherInfo>();
            for (int i = 0; i < 10; i++)
            {
                var weatherInfo = new WeatherInfo
                {
                    Location = $"Location{i}",
                    Degree = i * 23 / 17,
                    ResponseTime = 2,
                    Availability = 1,
                    DateTime = DateTime.Now.ToUniversalTime()
                  
                };
               
            weatherInfoList.Add(weatherInfo);

            }
            return weatherInfoList;
        }*/

        // GET: api/Weather/5

        public WeatherInfo Get( )
        {

            Random r = new Random();
            int rInt = r.Next(1,4); //for ints

           // RunAsync().Wait();

            return new WeatherInfo
            {
               
                ResponseTime = 2,
                Availability = rInt,
                DateTime = DateTime.Now.ToUniversalTime()
    
               

            };

        }
        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:50221");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("GET");
                HttpResponseMessage response = await client.GetAsync("api/Weather/1");
                if (response.IsSuccessStatusCode)
                {
                    WeatherInfo weather = await response.Content.ReadAsAsync<WeatherInfo>();
                    Console.WriteLine("{0}\t{1}\t{2}", weather.Location, weather.Degree, weather.DateTime);
                }

            }

        }

    }
}

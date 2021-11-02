using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TodoApi.Models;

namespace IOT_R8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherDataController : ControllerBase
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "qsC0IMLN21r9iaGEIeXAQIrhHnBExpZCNTzw1qof",
            BasePath = "https://iot-ohjelmointi-default-rtdb.europe-west1.firebasedatabase.app/"
        };

        private static FirebaseClient client;

        public static FirebaseClient Client
        {
            get
            {
                if (client == null)
                {
                    IFirebaseConfig config = new FirebaseConfig
                    {
                        AuthSecret = "qsC0IMLN21r9iaGEIeXAQIrhHnBExpZCNTzw1qof",
                        BasePath = "https://iot-ohjelmointi-default-rtdb.europe-west1.firebasedatabase.app/"
                    };

                    client = new FirebaseClient(config);
                }

                return client;
            }
        }

      

        [HttpGet]
        public async Task<ActionResult<List<WeatherData>>> GetItems()
        {
            FirebaseResponse response = await Client.GetAsync("data");
            object resp = response.ResultAs<object>();

            Console.WriteLine(resp);

            var firebaseLookup = JsonConvert.DeserializeObject<Dictionary<string, WeatherData>>(resp.ToString());
            var data = firebaseLookup.Values.ToList(); // or FirstOrDefault();


            if (data == null)
            {
                return NotFound();
            }

            return data;
        }

        // GET: api/TodoItems/5
        [HttpGet("{timestamp}")]
        public async Task<ActionResult<WeatherData>> GetItem(long timestamp)
        {
            FirebaseResponse response = await Client.GetAsync("data/" + timestamp);
            WeatherData data = response.ResultAs<WeatherData>();

            if (data == null)
            {
                return NotFound();
            }

            return data;
        }

        [HttpPut("{timestamp}")]
        public async Task<IActionResult> UpdateItem(long timestamp, WeatherData data)
        {
            if (timestamp != data.Timestamp)
            {
                return BadRequest();
            }

            FirebaseResponse response = await Client.GetAsync("data/" +timestamp);
            WeatherData getresp = response.ResultAs<WeatherData>();

            if (getresp == null)
            {
                return NotFound();
            }

            getresp.Temperature = data.Temperature;
            getresp.Light = data.Light;
            getresp.Humidity = data.Humidity;

            FirebaseResponse responseUpdate = await Client.UpdateAsync("data/"+timestamp, getresp);
            WeatherData updateresp = response.ResultAs<WeatherData>();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<WeatherData>> SaveItem(WeatherData data)
        {
            Random r = new Random();

            var date = DateTime.Now;

            // truncate milliseconds off, so they dont come around and mess things up
            date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);

            var newItem = new WeatherData
            {
                Timestamp = date.Ticks,
                Temperature = r.Next(0, 10),
                Humidity = r.Next(0, 10),
                Light = r.Next(0, 10)
            };
            SetResponse response = await Client.SetAsync("data/"+newItem.Timestamp, newItem);

            return CreatedAtAction(nameof(GetItem), new { Timestamp = newItem.Timestamp }, newItem);
        }

        [HttpDelete("{timestamp}")]
        public async Task<IActionResult> DeleteItem(long timestamp)
        {
            FirebaseResponse response = await Client.DeleteAsync("data/"+timestamp);

            return NoContent();
        }
    }
}

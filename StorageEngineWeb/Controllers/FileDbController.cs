// using System;

// namespace StorageEngineWeb.Controllers
// {

//     [ApiController]
//     [Route("api")]
//     public class FileDbController : ControllerBase
//     {
//         [HttpGet]
//         public async IEnumerable<WeatherForecast> Get(string collection, string key)
//         {
//             var rng = new Random();
//             return Enumerable.Range(1, 5).Select(index => new WeatherForecast
//             {
//                 Date = DateTime.Now.AddDays(index),
//                 TemperatureC = rng.Next(-20, 55),
//                 Summary = Summaries[rng.Next(Summaries.Length)]
//             })
//             .ToArray();
//         }
//     }
// }

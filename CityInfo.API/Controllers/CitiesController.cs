using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")] // Set route at controller level
    public class CitiesController : Controller
    {
        [HttpGet] // Provide separate methods here.
        public IActionResult GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public IActionResult GetCityById(int id)
        {
            var returnCity = CitiesDataStore.Current.Cities.FirstOrDefault(p => p.Id == id);

            if (returnCity == null)
            {
                return NotFound();
            }

            return Ok(returnCity);
        }
    }
}

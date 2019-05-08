using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")] // Set route at controller level
    public class CitiesController : Controller
    {
        // TODO: Use AutoMapper for this.
        private readonly ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet] // Provide separate methods here.
        public IActionResult GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();

            var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities); // use automapper to create the mapping....

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCityById(int id, bool includePointsOfInterest = false)
        {

            var returnCity = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (returnCity == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                var resultDto = Mapper.Map<CityDto>(returnCity);

                return Ok(resultDto);
            }
            else
            {
                var resultDto = Mapper.Map<CityWithoutPointsOfInterestDto>(returnCity);

                return Ok(resultDto);
            }
        }
    }
}

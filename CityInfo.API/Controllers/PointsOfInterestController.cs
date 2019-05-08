using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"The city {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterest(cityId);

                var resultsList = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);

                return Ok(resultsList);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestResult = Mapper.Map<PointOfInterestDto>(pointOfInterestEntity);

            return Ok(pointOfInterestResult);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var mappedPointOfInterest = Mapper.Map<PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, mappedPointOfInterest);

            if (!_cityInfoRepository.Save())
            {
                _logger.LogError("Changes were not saved to the database.");
                return StatusCode(500, "A problem happened while handling your request");
            }

            var createdPointOfInterestForReturn = Mapper.Map<PointOfInterestDto>(mappedPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new
                {cityId = cityId, id = createdPointOfInterestForReturn.Id}, createdPointOfInterestForReturn);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")] // You must update the entire resource here.
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                Debug.WriteLine("no poi in body");
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                Debug.WriteLine("no city");
                return NotFound();
            }

            var pointOfInterestFromRepo = _cityInfoRepository.GetPointOfInterest(cityId, id);

            if (pointOfInterestFromRepo == null)
            {
                Debug.WriteLine("no poi from repo");
                return NotFound();
            }

            Mapper.Map(pointOfInterest, pointOfInterestFromRepo); // This effectively puts the entity that is being tracked by our context into a modified state.

            if (!_cityInfoRepository.Save())
            {
                _logger.LogError("Unable to save changes to an updated point of interest.");
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent(); //request completed, but nothing to return. typical for updates.
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")] // partial updates
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided name and description should differ.");
            }

            TryValidateModel(pointOfInterestToPatch); // You have to validate the internal model here. The ModelState is validating the JsonPatchDocument.

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            if (!_cityInfoRepository.Save())
            {
                _logger.LogError("Unable to save changes to an updated point of interest.");
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent(); //request completed, but nothing to return. typical for updates.
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestFromRepo = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestFromRepo == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestFromRepo);

            _mailService.Send("Point of interest deleted",
                $"Point of interest {pointOfInterestFromRepo.Name} with id {pointOfInterestFromRepo.Id} was deleted.");


            if (!_cityInfoRepository.Save())
            {
                _logger.LogError("Unable to save changes to an updated point of interest.");
                return StatusCode(500, "A problem happened while handling your request.");
            } 

            return NoContent();
        }
    }
}

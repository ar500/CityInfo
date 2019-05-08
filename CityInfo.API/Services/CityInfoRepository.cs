using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public IEnumerable<City> GetCities()
        {
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int id, bool includePointOfInterest)
        {
            if (includePointOfInterest)
            {
                return _context.Cities
                    .Where(c => c.Id == id)
                    .Include(p => p.PointsOfInterest)
                    .FirstOrDefault();
            }
            else
            {
                return _context.Cities
                    .FirstOrDefault(c => c.Id == id);
            }
            
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId)
        {
            return _context.PointOfInterests
                .Where(p => p.CityId == cityId)
                .ToList();
        }

        public PointOfInterest GetPointOfInterest(int cityId, int id)
        {
            return _context.PointOfInterests
                .FirstOrDefault(p => p.CityId == cityId && p.Id == id);
        }

        public bool CityExists(int cityId)
        {
            return _context.Cities.Any(c => c.Id == cityId);
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }

        public bool Save()
        {
            // True if the SaveChanges method returned more than zero changes.
            return (_context.SaveChanges() >= 0);
        }
    }
}

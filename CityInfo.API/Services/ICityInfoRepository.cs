using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        IEnumerable<City> GetCities();

        City GetCity(int id, bool includePointsOfInterest);

        IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId);

        PointOfInterest GetPointOfInterest(int cityId, int id);

        bool CityExists(int cityId);

        void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        bool Save();
    }
}

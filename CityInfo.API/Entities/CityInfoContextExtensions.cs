using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public static class CityInfoContextExtensions
    {
        public static void EnsureSeedDataForContext(this CityInfoContext context)
        {
            if (context.Cities.Any())
            {
                return;
            }

            var cities = new List<City>()
            {
                new City()
                {
                    Name = "New York City",
                    Description = "The one with the big park.",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "Central Park",
                            Description = "The most visited urban park in the U.S."
                        },
                        new PointOfInterest()
                        {
                            Name = "Empire State Building",
                            Description = "A 102-story skyscraper located in Midtown Manhattan."
                        }
                    }
                },
                new City()
                {
                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never finished.",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "Cathedral of our Lady",
                            Description =
                                "A Gothic style cathedral, conceived by the architects Jan and Pieter Appelmans."
                        },
                        new PointOfInterest()
                        {
                            Name = "Antwerp Central Station",
                            Description = "The finest example of railway architecture in Belgium."
                        }
                    }

                },
                new City()
                {
                    Name = "Paris",
                    Description = "The one with the big tower.",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "Eiffel Tower",
                            Description = "A wrought iron lattice tower on the Champ de Mars."
                        },
                        new PointOfInterest()
                        {
                            Name = "The Louvre",
                            Description = "The world's largest museum"
                        }
                    }
                }
            };

            context.Cities.AddRange(cities);
            context.SaveChanges();
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using AirlinePlanner.Models;

namespace AirlinePlanner.Tests
{
  [TestClass]
  public class flightTests : IDisposable
  {

    [TestMethod]
    public void AddCity_AddCityToFlight_City()
    {
      Flight testFlight = new Flight(DateTime.Now, "Seattle", "Chicago", "OnTime");
      City testCity = new City("Seattle");
      testFlight.Save();
      testCity.Save();

      testFlight.AddCity(testCity);

      List<City> expected = new List<City>{testCity};
      List<City> actual = testFlight.GetCities();

      CollectionAssert.AreEqual(expected,actual);

    }

    public void Dispose()
    {
      City.DeleteAll();
      Flight.DeleteAll();
    }
  }
}

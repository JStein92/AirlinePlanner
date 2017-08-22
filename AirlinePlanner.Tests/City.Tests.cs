using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using AirlinePlanner.Models;

namespace AirlinePlanner.Tests
{
  [TestClass]
  public class CityTest : IDisposable
  {
    public CityTest()
    {
        DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=3306;database=airline_planner_test;";
    }

    [TestMethod]
    public void GetAll_GetAllItemsInDatabase_CityList()
    {
      int count = City.GetAll().Count;
      Assert.AreEqual(0,count);
    }
    [TestMethod]
    public void Save_SaveRecordInTheDB_City()
    {
      City city = new City("seattle");
      city.Save();

      List<City> result = City.GetAll();

      List<City> testList = new List<City>{city};

      CollectionAssert.AreEqual(testList,result);
    }

    [TestMethod]
    public void AddFlight_AddFlightToCity_Flight()
    {
      Flight testFlight = new Flight(DateTime.Now, "Seattle", "Chicago", "OnTime");
      City testCity = new City("Chicago");
      testFlight.Save();
      testCity.Save();

      testCity.AddFlight(testFlight);

      List<Flight> expected = new List<Flight>{testFlight};
      List<Flight> actual = testCity.GetFlights();

      CollectionAssert.AreEqual(expected,actual);

    }

    public void Dispose()
    {
      Flight.DeleteAll();
      City.DeleteAll();
    }

  }

}

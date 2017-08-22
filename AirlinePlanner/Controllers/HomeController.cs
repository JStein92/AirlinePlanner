using Microsoft.AspNetCore.Mvc;
using AirlinePlanner.Models;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace AirlinePlanner.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet("/")]
    public ActionResult Index()
    {
      return View();
    }
    [HttpGet("/flights")]
    public ActionResult Flights()
    {
      Dictionary<string, object> model = new Dictionary<string,object>{};
      model.Add("allFlights", Flight.GetAll());
      model.Add("allCities", City.GetAll());
      return View(model);
    }
    [HttpPost("/flights")]
    public ActionResult FlightsPost()
    {
      DateTime newDepartTime = DateTime.Parse(Request.Form["departure-time"]);
      string newDepartCity = City.Find(int.Parse(Request.Form["departure-city"])).GetName();
      string newArrivalCity = City.Find(int.Parse(Request.Form["arrival-city"])).GetName();
      string newStatus = Request.Form["status"];

      Flight newFlight = new Flight(newDepartTime,newDepartCity,newArrivalCity,newStatus);
      newFlight.Save();

      newFlight.AddCity(City.Find((int.Parse(Request.Form["departure-city"]))));
      newFlight.AddCity(City.Find((int.Parse(Request.Form["arrival-city"]))));

      Dictionary<string, object> model = new Dictionary<string,object>{};
      model.Add("allFlights", Flight.GetAll());
      model.Add("allCities", City.GetAll());

      return View("flights",model);
    }
    [HttpPost("/flights/newStop")]
    public ActionResult FlightsNewStop()
    {

      Flight foundFlight = Flight.Find(int.Parse(Request.Form["flightId"]));
      foundFlight.AddCity(City.Find(int.Parse(Request.Form["cityId"])));

      Dictionary<string, object> model = new Dictionary<string,object>{};
      model.Add("allFlights", Flight.GetAll());
      model.Add("allCities", City.GetAll());

      return View("flights",model);
    }
    [HttpGet("/cities")]
    public ActionResult Cities()
    {
      return View(City.GetAll());
    }
    [HttpPost("/cities")]
    public ActionResult CitiesPost()
    {
      string newCityName = Request.Form["name"];
      City newCity = new City(newCityName);
      newCity.Save();
      return View("Cities",City.GetAll());
    }
    [HttpGet("/FlightForm")]
    public ActionResult FlightForm()
    {
      return View(City.GetAll());
    }
    [HttpGet("/CityForm")]
    public ActionResult CityForm()
    {
      return View(Flight.GetAll());
    }
  }
}

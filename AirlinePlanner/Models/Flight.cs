using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace AirlinePlanner.Models

{
  public class Flight
  {
    private int _id;
    private DateTime _departureTime;
    private string _departureCity;
    private string _arrivalCity;
    private string _status;

    public Flight(DateTime departureTime, string departureCity, string arrivalCity, string status, int id = 0)
    {
      _id = id;
      _departureTime = departureTime;
      _departureCity  = departureCity;
      _arrivalCity = arrivalCity;
      _status = status;
    }
    public int GetId()
    {
      return _id;
    }
    public DateTime GetDepartureTime()
    {
      return _departureTime;
    }
    public string GetDepartureCity()
    {
      return _departureCity;
    }
    public string GetArrivalCity()
    {
      return _arrivalCity;
    }
    public string GetStatus()
    {
      return _status;
    }

    public override bool Equals(System.Object otherFlight)
    {
      if(!(otherFlight is Flight))
      {
        return false;
      }
      else
      {
        Flight newFlight = (Flight) otherFlight;
        return this.GetId().Equals(newFlight.GetId());
      }
    }
    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }
    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText= @"DELETE FROM flights; DELETE FROM cities_flights";

      cmd.ExecuteNonQuery();
      conn.Close();
    }
    public static List<Flight> GetAll()
    {
      List<Flight> allFlights = new List<Flight>();
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        DateTime departureTime = rdr.GetDateTime(1);
        string departureCity = rdr.GetString(2);
        string arrivalCity = rdr.GetString(3);
        string status = rdr.GetString(4);

        Flight flight = new Flight(departureTime,departureCity,arrivalCity,status, id);
        allFlights.Add(flight);
      }
      conn.Close();
      return allFlights;
    }

    public void AddCity(City city)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText= @"INSERT INTO cities_flights (city_id,flight_id) VALUES (@cityId,@flightId);";

      MySqlParameter cityId = new MySqlParameter();
      cityId.ParameterName = "@cityId";
      cityId.Value = city.GetId();
      cmd.Parameters.Add(cityId);

      MySqlParameter flightId = new MySqlParameter();
      flightId.ParameterName = "@flightId";
      flightId.Value = _id;
      cmd.Parameters.Add(flightId);

      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public static Flight Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      int newId = 0;
      DateTime departureTime = DateTime.Now;
      string departureCity = "";
      string arrivalCity ="";
      string status = "";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      while(rdr.Read())
      {
         newId = rdr.GetInt32(0);
         departureTime = rdr.GetDateTime(1);
         departureCity = rdr.GetString(2);
         arrivalCity = rdr.GetString(3);
         status = rdr.GetString(4);
      }

      Flight flight = new Flight(departureTime,departureCity,arrivalCity,status, newId);

      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }

      return flight;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText= @"INSERT INTO flights(departure_time,departure_city,arrival_city,status) VALUES(@departureTime,@departureCity,@arrivalCity,@status);";

      MySqlParameter departureTime = new MySqlParameter();
      departureTime.ParameterName = "@departureTime";
      departureTime.Value = this._departureTime;
      cmd.Parameters.Add(departureTime);

      MySqlParameter departureCity = new MySqlParameter();
      departureCity.ParameterName = "@departureCity";
      departureCity.Value = this._departureCity;
      cmd.Parameters.Add(departureCity);

      MySqlParameter arrivalCity = new MySqlParameter();
      arrivalCity.ParameterName = "@arrivalCity";
      arrivalCity.Value = this._arrivalCity;
      cmd.Parameters.Add(arrivalCity);

      MySqlParameter status = new MySqlParameter();
      status.ParameterName = "@status";
      status.Value = this._status;
      cmd.Parameters.Add(status);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
    }

    public List<City> GetCities()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT city_id FROM cities_flights WHERE flight_id = @thisId;";

      MySqlParameter flightIdParameter = new MySqlParameter();
      flightIdParameter.ParameterName = "@thisId";
      flightIdParameter.Value = _id;
      cmd.Parameters.Add(flightIdParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      List<int> cityIds = new List<int> {};
      while(rdr.Read())
      {
        int cityId = rdr.GetInt32(0);
        cityIds.Add(cityId);
      }
      rdr.Dispose();

      List<City> cities = new List<City> {};
      foreach (int cityId in cityIds)
      {
        var cityQuery = conn.CreateCommand() as MySqlCommand;
        cityQuery.CommandText = @"SELECT * FROM cities WHERE id = @CityId;";

        MySqlParameter cityIdParameter = new MySqlParameter();
        cityIdParameter.ParameterName = "@CityId";
        cityIdParameter.Value = cityId;
        cityQuery.Parameters.Add(cityIdParameter);

        var cityQueryRdr = cityQuery.ExecuteReader() as MySqlDataReader;
        while(cityQueryRdr.Read())
        {
          int thisCityId = cityQueryRdr.GetInt32(0);
          string cityDescription = cityQueryRdr.GetString(1);
          City foundCity = new City(cityDescription, thisCityId);
          cities.Add(foundCity);
        }
        cityQueryRdr.Dispose();
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return cities;
    }
  }

}

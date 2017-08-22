using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace AirlinePlanner.Models
{
  public class City
  {
    private int _id;
    private string _name;

    public City(string name, int id = 0)
    {
      _id = id;
      _name = name;
    }

    public string GetName()
    {
      return _name;
    }
    public int GetId()
    {
      return _id;
    }
    public override bool Equals(System.Object otherCity)
    {
      if(!(otherCity is City))
      {
        return false;
      }
      else
      {
        City newCity = (City) otherCity;
        return this.GetId().Equals(newCity.GetId());
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
      cmd.CommandText= @"DELETE FROM cities; DELETE FROM cities_flights";

      cmd.ExecuteNonQuery();
      conn.Close();
    }
    public static List<City> GetAll()
    {
      List<City> allCities = new List<City>();
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM cities";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        var city = new City(name,id);
        allCities.Add(city);
      }
      conn.Close();
      return allCities;
    }
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText= @"INSERT INTO cities(name) VALUES(@name);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
    }

    public static City Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM cities WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      int newId = 0;
      string name ="";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
         newId = rdr.GetInt32(0);
         name = rdr.GetString(1);

      }
      City city = new City(name, newId);

      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }

      return city;
    }

    public void AddFlight(Flight flight)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText= @"INSERT INTO cities_flights (city_id,flight_id) VALUES (@cityId,@flightId);";

      MySqlParameter cityId = new MySqlParameter();
      cityId.ParameterName = "@cityId";
      cityId.Value = _id;
      cmd.Parameters.Add(cityId);

      MySqlParameter flightId = new MySqlParameter();
      flightId.ParameterName = "@flightId";
      flightId.Value = flight.GetId();
      cmd.Parameters.Add(flightId);

      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public List<Flight> GetFlights()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT flight_id FROM cities_flights WHERE city_id = @thisId;";

      MySqlParameter cityIdParameter = new MySqlParameter();
      cityIdParameter.ParameterName = "@thisId";
      cityIdParameter.Value = _id;
      cmd.Parameters.Add(cityIdParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      List<int> flightIds = new List<int> {};
      while(rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        flightIds.Add(flightId);
      }
      rdr.Dispose();

      List<Flight> flights = new List<Flight> {};
      foreach (int flightId in flightIds)
      {
        var flightQuery = conn.CreateCommand() as MySqlCommand;
        flightQuery.CommandText = @"SELECT * FROM flights WHERE id = @flightId;";

        MySqlParameter flightIdParameter = new MySqlParameter();
        flightIdParameter.ParameterName = "@flightId";
        flightIdParameter.Value = flightId;
        flightQuery.Parameters.Add(flightIdParameter);

        var flightQueryRdr = flightQuery.ExecuteReader() as MySqlDataReader;

        while(flightQueryRdr.Read())
        {
          int id = flightQueryRdr.GetInt32(0);
          DateTime departureTime = flightQueryRdr.GetDateTime(1);
          string departureCity = flightQueryRdr.GetString(2);
          string arrivalCity = flightQueryRdr.GetString(3);
          string status = flightQueryRdr.GetString(4);

          Flight flight = new Flight(departureTime,departureCity,arrivalCity,status, id);
          flights.Add(flight);
        }

        flightQueryRdr.Dispose();

      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return flights;
    }

  }
}

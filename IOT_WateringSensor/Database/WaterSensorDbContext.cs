using System;
using IOT_WateringSensor.Data;
using Microsoft.EntityFrameworkCore;

namespace IOT_WateringSensor.Database;

public class WaterSensorDbContext : DbContext
{
    public DbSet<SensorData> SensorData { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString =
            @"Server=" + (Environment.GetEnvironmentVariable("MSSQL_HOST")?? "[::1]") + ","
            + (Environment.GetEnvironmentVariable("MSSQL_PORT")?? "1433") + ";" 
            + "Database=SensorDataDb;User Id=SA;Password="
            + (Environment.GetEnvironmentVariable("SA_PASSWORD") ?? "yourStrong(!)Password") + ";"
            + "Trusted_Connection=false;";
        Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond + " - "+connectionString);
        optionsBuilder.UseSqlServer(connectionString);
        
        Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond +" - connected to db");
    }
}
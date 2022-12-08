using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOT_WateringSensor.Data;

public class SensorData : IEntityTypeConfiguration<SensorData>
{
    [Key] public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public DateTime TimeStamp { get; set; }
    public int Moisture { get; set; }

    public void Configure(EntityTypeBuilder<SensorData> builder)
    {
    }
}

public class SensorDataDto
{
    public Guid ClientId { get; set; }
    public DateTime TimeStamp { get; set; }
    public int Moisture { get; set; }
}
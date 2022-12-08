using System.ComponentModel.DataAnnotations;

namespace IOT_WateringSensor.Data;

public class UserToDeviceBinding
{
    [Key] public int Id { get; set; }
    public string? UserId { get; set; }
    public string DeviceId { get; set; }
    public bool isBound { get; set; } = false;
    public string bindingKey { get; set; }
}
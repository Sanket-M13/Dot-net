using System.ComponentModel.DataAnnotations;

namespace EVChargerAPI.Models
{
    public class VehicleBrand
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Type { get; set; } = string.Empty; // "Car" or "Bike"
        
        public ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();
    }
    
    public class VehicleModel
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public int VehicleBrandId { get; set; }
        public VehicleBrand VehicleBrand { get; set; } = null!;
    }
}
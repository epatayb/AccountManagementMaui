namespace AccountManagementMaui.Api.Models.CityModels
{
    public class CityDetailDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CityCode { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public List<CityDistrictDto> Districts { get; set; } = [];
    }

    public class CityDistrictDto
    {
        public int Id { get; set; }

        public string DistrictCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
namespace AccountManagementMaui.Api.Models.CityModels
{
    public class CityListDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CityCode { get; set; } = string.Empty;

        public int DistrictCount { get; set; }
    }
}

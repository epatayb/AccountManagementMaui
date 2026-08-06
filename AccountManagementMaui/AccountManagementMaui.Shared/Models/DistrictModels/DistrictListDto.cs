using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagementMaui.Shared.Models.DistrictModels
{
    public class DistrictListDto
    {
        public int Id { get; set; }

        public string DistrictCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int CityId { get; set; }

        public string CityCode { get; set; } = string.Empty;

        public string CityName { get; set; } = string.Empty;
    }
}

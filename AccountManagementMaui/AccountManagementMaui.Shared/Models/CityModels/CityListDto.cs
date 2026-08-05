using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagementMaui.Shared.Models.CityModels
{
    public class CityListDto
    {
        public int Id { get; set; }

        public string CityCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int DistrictCount { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;

namespace Pardis.Domain.Dto
{
    public partial class Dtos
    {

        public class DashboardStatsDto
        {
            public Dictionary<string, object> Stats { get; set; }
            public List<object> RecentActivity { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdministrationApp
{
    class ReservationFilter
    {
        public string NameFilter { get; set; }
        public int? SpotFilter { get; set; }
        public string EmailFilter { get; set; }
        public string FromDateFilter { get; set; }
        public string ToDateFilter { get; set; }

    }
}

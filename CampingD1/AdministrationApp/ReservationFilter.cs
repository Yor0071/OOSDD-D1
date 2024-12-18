using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdministrationApp
{
    class ReservationFilter
    {
        public int? ReservationIDFilter { get; set; }
        public string SpotNameFilter { get; set; }
        public string NameFilter { get; set; }
        public string EmailFilter { get; set; }
        public string FromDateFilter { get; set; }
        public string ToDateFilter { get; set; }
    }
}

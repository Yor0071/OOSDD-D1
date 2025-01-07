using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Types
{
    public class Activity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }

        public Activity() { }

        public Activity(int id, string title, string description, string location, DateTime date)
        {
            Id = id;
            Title = title;
            Description = description;
            Location = location;
            Date = date;
        }
    }
}

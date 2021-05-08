using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SRManagementApplication.Models
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceTimePeriod { get; set; }
    }
}

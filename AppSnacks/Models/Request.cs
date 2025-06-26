using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSnacks.Models
{
    public class Request
    {
        public string? Address { get; set; }
        public decimal Total { get; set; }
        public int UserId { get; set; }
    }
}

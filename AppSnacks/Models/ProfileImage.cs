using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSnacks.Models
{
    public class ProfileImage
    {
        public string? ImageUrl { get; set; }
        public string? ImagePath => AppConfig.BaseUrl + ImageUrl;

    }
}

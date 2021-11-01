using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialBrothers.Data
{
    public class AdressDTO
    {
        public List<string> resourceSets { get; set; }
        public string FilmId { get; set; }
        public string Title { get; set; }
        public int ScreenId { get; set; }
        public DateTime PreShowStartTime { get; set; }
    }

    public class RootObject
    {
        public List<AdressDTO> AdressDTOs { get; set; }
    }
}

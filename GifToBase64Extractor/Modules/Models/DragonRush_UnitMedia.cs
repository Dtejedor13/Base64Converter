using System.Collections.Generic;

namespace GifToBase64Extractor.Modules.Models
{
    class DragonRush_UnitMedia
    {
        public int id { get; set; }
        public List<DragonRush_UnitMediaModel> media { get; set; } = new List<DragonRush_UnitMediaModel>();
    }
}

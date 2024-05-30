using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Media.Imaging;

namespace Diplom;

public class AllDTO
{
    public class ChurchDto
    {
        public int Id { get; set; }
        public string Churchname { get; set; }
        public List<string>? Photos { get; set; }
        public Bitmap? FirstPhoto { get; set; }
        public string BuildDate { get; set; }
        public string Description { get; set; }
        public string ChurchnameEng { get; set; }
        public string BuildDateEng { get; set; }
        public string DescriptionEng { get; set; }
        
        // Дополнительные свойства для отображения
        public string ChurchnameDisplay { get; set; }
        public string BuildDateDisplay { get; set; }

        public string GetChurchname(bool isRussian)
        {
            return isRussian ? Churchname : ChurchnameEng;
        }

        public string GetBuildDate(bool isRussian)
        {
            return isRussian ? BuildDate : BuildDateEng;
        }

        public string GetDescription(bool isRussian)
        {
            return isRussian ? Description : DescriptionEng;
        }
    }





    public class PhotoDto
    {
        public string NamePhoto { get; set; }
    }

    public class Region
    {
        public int Id { get; set; }
        public string Nameofregion { get; set; }
        public string NameofregionEng { get; set; }
    }
    
}
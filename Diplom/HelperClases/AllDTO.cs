using System.Collections.Generic;
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
    }

    public class PhotoDto
    {
        public string NamePhoto { get; set; }
    }

    public class Region
    {
        public int Id { get; set; }
        public string Nameofregion { get; set; }
    }
    
}
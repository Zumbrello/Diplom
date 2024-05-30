using System.ComponentModel;

public class Region 
{
    public int Id { get; set; }
    public string NameofregionEng { get; set; }
    public string Nameofregion { get; set; }  // Сделаем свойство публичным и автоматическим
    public object Tag { get; set; }
    
    public string IsRussian(bool isRu)
    {
        if (isRu)
        {
            return Nameofregion;
        }
        else
        {
            Nameofregion = NameofregionEng;
            return Nameofregion;
        }
    }
}
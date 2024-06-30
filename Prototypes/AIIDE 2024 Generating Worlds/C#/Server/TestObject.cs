using System.Xml.Linq;

public class TestObject
{
    public bool groundRulesExist { get; set; }
    public String? filepath { get; set; }
    public String? filename { get; set; }
    public String? extension { get; set; }
    public int tilesize { get; set; }
    public String? name { get; set; }
    public int width { get; set; }
    public bool isTileset { get; set; }
    public XElement rules { get; set; }
    public List<String> files { get; set; }
}

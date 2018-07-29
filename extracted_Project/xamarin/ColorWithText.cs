using Android.Graphics;

namespace xamarin
{
    //class containing all 4 colors, name and id (to check with ListView item id)
    public class ColorWithText
    {
        public string schemeName;
        public Color color_First;
        public Color color_Second;
        public Color color_Third;
        public Color color_Fourth;
        public int id;

        public ColorWithText (Color color_First, Color color_Second, Color color_Third, Color color_Fourth, string schemeName, int idScheme)
        {
            this.color_First = color_First;
            this.color_Second = color_Second;
            this.color_Third = color_Third;
            this.color_Fourth = color_Fourth;
            this.schemeName = schemeName;
            this.id = idScheme;
        }
        
        private string ConvertToHex(Color color)
        {
            var hex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            return hex;
        }

        private string ConvertToRGB(Color color)
        {
            return "(" + color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString() + ")";
        }
        
        public string GetHex()
        {
            return ConvertToHex(color_First) + " , " + ConvertToHex(color_Second) + " , " + ConvertToHex(color_Third) + " , " + ConvertToHex(color_Fourth); 
        }

        public string GetRGB()
        {
            return ConvertToRGB(color_First) + "\n" + ConvertToRGB(color_Second) + "\n" + ConvertToRGB(color_Third) + "\n" + ConvertToRGB(color_Fourth);
        }
    }
}
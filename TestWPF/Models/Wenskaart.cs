using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestWPF.Models
{
    public class Wenskaart
    {
        public Wenskaart()
        {
            FontSize = 24;
            Lettertype = new FontFamily("Arial");
        }

        public BitmapImage Background { get; set; }
        public string Boodschap { get; set; }
        public FontFamily Lettertype { get; set; }
        public int FontSize { get; set; }
        public string Status { get; set; }
    }
}

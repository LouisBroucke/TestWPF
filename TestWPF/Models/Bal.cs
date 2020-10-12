using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TestWPF.Models
{
    public class Bal
    {
        public Bal()
        {

        }

        public Bal(double x, double y, Brush kleur)
        {
            XPositie = x;
            YPositie = y;
            Kleur = kleur;
        }

        public double XPositie { get; set; }
        public double YPositie { get; set; }
        public Brush Kleur { get; set; }
    }
}

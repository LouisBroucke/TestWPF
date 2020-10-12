using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestWPF.Models;

namespace TestWPF.ViewModel
{
    public class WenskaartVM : ViewModelBase
    {
        private Wenskaart wenskaart;
        private ObservableCollection<FontFamily> fontLijst = new ObservableCollection<FontFamily>();
        private ObservableCollection<Kleur> kleurLijst = new ObservableCollection<Kleur>();
        private ObservableCollection<Models.Bal> ballen = new ObservableCollection<Models.Bal>();

        public WenskaartVM(Wenskaart wenskaart)
        {
            this.wenskaart = wenskaart;

            var fonts = Fonts.SystemFontFamilies.OrderBy(font => font.ToString());

            foreach (var font in fonts)
            {
                FontLijst.Add(font);
            }

            foreach (PropertyInfo info in typeof(Colors).GetProperties())
            {
                BrushConverter bc = new BrushConverter();
                SolidColorBrush deKleur = (SolidColorBrush)bc.ConvertFromString(info.Name);
                Kleur kleur = new Kleur();
                kleur.Brush = deKleur;
                kleur.Naam = info.Name;
                kleur.Hex = deKleur.ToString();
                kleur.Rood = deKleur.Color.R;
                kleur.Groen = deKleur.Color.G;
                kleur.Blauw = deKleur.Color.B;
                KleurLijst.Add(kleur);
            }
        }        

        public ObservableCollection<FontFamily> FontLijst
        {
            get
            {
                return fontLijst;
            }
            set
            {
                fontLijst = value;
                RaisePropertyChanged("SystemFonts");
            }
        }        

        public ObservableCollection<Kleur> KleurLijst
        {
            get
            {
                return kleurLijst;
            }
            set
            {
                kleurLijst = value;
                RaisePropertyChanged("KleurLijst");
            }
        }        

        public ObservableCollection<Bal> Ballen
        {
            get
            {
                return ballen;
            }
            set
            {
                ballen = value;
                RaisePropertyChanged("Ballen");
            }
        }

        public BitmapImage Achtergrond
        {
            get
            {
                return wenskaart.Background;
            }
            set
            {
                wenskaart.Background = value;
                RaisePropertyChanged("Achtergrond");
            }
        }

        public string Wens
        {
            get
            {
                return wenskaart.Boodschap;
            }
            set
            {
                wenskaart.Boodschap = value;
                RaisePropertyChanged("Wens");
            }
        }

        public FontFamily Lettertype
        {
            get
            {
                return wenskaart.Lettertype;
            }
            set
            {
                wenskaart.Lettertype = value;
                RaisePropertyChanged("Lettertype");
            }
        }

        public int Fontsize
        {
            get
            {
                return wenskaart.FontSize;
            }
            set
            {
                wenskaart.FontSize = value;
                RaisePropertyChanged("Fontsize");
            }
        }

        public string Status
        {
            get
            {
                return wenskaart.Status;
            }
            set
            {
                wenskaart.Status = value;
                RaisePropertyChanged("Status");
            }
        }

        public RelayCommand NieuwCommand
        {
            get
            {
                return new RelayCommand(NieuweKaart);
            }
        }

        private void NieuweKaart()
        {
            Achtergrond = null;
            Wens = string.Empty;
            Fontsize = 24;
            Lettertype = new FontFamily("Arial");
            Ballen.Clear();
            Status = "nieuw";
        }

        public RelayCommand OpenenCommand
        {
            get
            {
                return new RelayCommand(OpenenBestand);
            }
        }

        private void OpenenBestand()
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = "Wenskaart";
                dlg.DefaultExt = ".kaart";
                dlg.Filter = "Wenskaarten |*.kaart";

                if (dlg.ShowDialog() == true)
                {
                    using (StreamReader bestand = new StreamReader(dlg.FileName))
                    {
                        Achtergrond = new BitmapImage(new Uri(bestand.ReadLine()));
                        Wens = bestand.ReadLine();
                        Lettertype = new FontFamily(bestand.ReadLine());
                        Fontsize = Convert.ToInt16(bestand.ReadLine());

                        Ballen.Clear();

                        Brush kleur;
                        double x, y;

                        while (double.TryParse(bestand.ReadLine(), out x))
                        {
                            double.TryParse(bestand.ReadLine(), out y);
                            BrushConverter bc = new BrushConverter();
                            kleur = (Brush)bc.ConvertFromString(bestand.ReadLine());

                            Models.Bal bal = new Models.Bal(x, y, kleur);
                            Ballen.Add(bal);

                            x = 0;
                            y = 0;
                            kleur = null;
                        }
                    }
                }

                Status = dlg.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public RelayCommand OpslaanCommand
        {
            get
            {
                return new RelayCommand(OpslaanBestand);
            }
        }

        private void OpslaanBestand()
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "Wenskaart";
                dlg.DefaultExt = ".kaart";
                dlg.Filter = "Wenskaarten |*.kaart";

                if (dlg.ShowDialog() == true)
                {
                    using (StreamWriter bestand = new StreamWriter(dlg.FileName))
                    {
                        bestand.WriteLine(Achtergrond.ToString());
                        bestand.WriteLine(Wens);
                        bestand.WriteLine(Lettertype.ToString());
                        bestand.WriteLine(Fontsize.ToString());

                        foreach (var bal in ballen)
                        {
                            bestand.WriteLine(bal.XPositie.ToString());
                            bestand.WriteLine(bal.YPositie.ToString());
                            bestand.WriteLine(bal.Kleur.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public RelayCommand PreviewCommand
        {
            get
            {
                return new RelayCommand(Afdrukvoorbeeld);
            }
        }

        private void Afdrukvoorbeeld()
        {
            Views.Preview preview = new Views.Preview();
            preview.printpreview.Document = AfdrukSamenstellen();
            preview.ShowDialog();
        }

        private FixedDocument AfdrukSamenstellen()
        {
            FixedDocument doc = new FixedDocument();
            doc.DocumentPaginator.PageSize = new Size(500, 500);

            PageContent inhoud = new PageContent();
            doc.Pages.Add(inhoud);

            FixedPage page = new FixedPage();
            inhoud.Child = page;

            Canvas canvas = new Canvas();
            ImageBrush kaart = new ImageBrush();
            kaart.ImageSource = Achtergrond;
            canvas.Width = 500;
            canvas.Height = 400;
            canvas.Background = kaart;

            TextBlock tekst = new TextBlock();
            tekst.Text = Wens.ToString();
            tekst.FontFamily = Lettertype;
            tekst.FontSize = Fontsize;
            tekst.Width = 500;
            tekst.Margin = new Thickness(0, 400, 0, 0);
            tekst.TextAlignment = TextAlignment.Center;

            page.Children.Add(canvas);
            page.Children.Add(tekst);

            foreach (Models.Bal bal in Ballen)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = bal.Kleur;
                ellipse.Margin = new Thickness(bal.XPositie, bal.YPositie, 0, 0);
                page.Children.Add(ellipse);
            }

            return doc;
        }

        public RelayCommand AfsluitenCommand
        {
            get
            {
                return new RelayCommand(AfsluitenApp);
            }
        }

        private void AfsluitenApp()
        {
            Application.Current.MainWindow.Close();
        }

        public RelayCommand<CancelEventArgs> ClosingCommand
        {
            get
            {
                return new RelayCommand<CancelEventArgs>(OnClosingWindow);
            }
        }

        private void OnClosingWindow(CancelEventArgs e)
        {
            if (MessageBox.Show("Wilt u het programma sluiten?", "Afsluiten", 
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) 
                == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        public RelayCommand KerstkaartCommand
        {
            get
            {
                return new RelayCommand(KerstKaart);
            }
        }

        private void KerstKaart()
        {
            NieuweKaart();
            Achtergrond = new BitmapImage(new Uri("pack://application:,,,/images/kerstkaart.jpg"));
        }

        public RelayCommand GeboortekaartCommand
        {
            get
            {
                return new RelayCommand(GeboorteKaart);
            }
        }

        private void GeboorteKaart()
        {
            NieuweKaart();
            Achtergrond = new BitmapImage(new Uri("pack://application:,,,/images/geboortekaart.jpg"));
        }

        public RelayCommand GroterCommand
        {
            get
            {
                return new RelayCommand(GroterLettertype);
            }
        }

        private void GroterLettertype()
        {
            Fontsize++;
        }

        public RelayCommand KleinerCommand
        {
            get
            {
                return new RelayCommand(KleinerLettertype);
            }
        }

        private void KleinerLettertype()
        {
            Fontsize--;
        }

        public RelayCommand<MouseEventArgs> DragCommand
        {
            get
            {
                return new RelayCommand<MouseEventArgs>(sleepBal);
            }
        }

        private void sleepBal(MouseEventArgs e)
        {
            Ellipse sleepEllipse = (Ellipse)e.Source;
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DataObject ellipse = new DataObject("ellipse", sleepEllipse);
                    DragDrop.DoDragDrop(sleepEllipse, ellipse, DragDropEffects.Move);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public RelayCommand<DragEventArgs> DropCommand
        {
            get
            {
                return new RelayCommand<DragEventArgs>(dropBal);
            }
        }

        private void dropBal(DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent("ellipse"))
                {
                    Ellipse ellipse = (Ellipse)e.Data.GetData("ellipse");
                    Canvas canvas = (Canvas)e.Source;
                    Point pos = e.GetPosition(canvas);
                    Models.Bal bal = new Models.Bal(pos.X - 20, pos.Y - 20, ellipse.Fill);
                    Ballen.Add(bal);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}

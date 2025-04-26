using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_PlanningTool_Ambulance_Icket_Michael
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Label> labelKalender = new List<Label>();
        private List<ComboBox> cbxKalender = new List<ComboBox>();
        bool toggle = true;
        
        public MainWindow()
        {
            InitializeComponent();
            GenereerTime();
            BasisGrid();
        }
        private void BasisGrid()
        {
            // Genereer Opmaak Week Nummer en Uren Shift
            int p = 0;
            for (int r = 0; r < 6; r++)
            {
                GridWeek.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                TextBlock txb = new TextBlock();
                txb.Text = (r+1).ToString();
                txb.Background = Brushes.LightGray;
                txb.FontSize = 20;
                txb.VerticalAlignment = VerticalAlignment.Top;
                txb.HorizontalAlignment = HorizontalAlignment.Stretch;
                
                Grid.SetRow(txb, r);
                GridWeek.Children.Add(txb);

                Label[] lblarray = new Label[5];
                for (int c = 0; c < 5; c++)
                {
                    GridUren.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Label lbl = new Label();
                    lbl.Background = Brushes.LightGray;
                    lbl.BorderBrush = Brushes.Black;
                    lbl.BorderThickness = new Thickness(0.5);
                    lbl.FontSize = 15;
                    Grid.SetRow(lbl, p);
                    p++;
                    GridUren.Children.Add(lbl);
                    lblarray[c] = lbl;
                }
                lblarray[0].Content = "Day";
                lblarray[1].Content = "00u00 - 06u30";
                lblarray[2].Content = "06u30 - 12u30";
                lblarray[3].Content = "12u30 - 18u30";
                lblarray[4].Content = "18u30 - 24u00";
            }
            // Genereer het hoofdgrid dynamisch (6 rijen, 7 kolommen)

            for (int i = 0; i < 6; i++)
            {
                GridKalender.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
            for (int j = 0; j < 7; j++)
            {
                GridKalender.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
        }
        private void GenereerTime()
        {
            // Huidig jaartal, cbx vullen: twee jaar ervoor en twee jaar erna
            // Huidige maand, cbx vullen met een enum
            int huidigJaar = DateTime.Now.Year;
            int huidigeMaand = DateTime.Now.Month;
            List<string> jaren = new List<string>();
                        
            for (int i = huidigJaar - 2; i <= huidigJaar + 2; i++)
            {
                jaren.Add(i.ToString());
            }
            CmbJaar.ItemsSource = jaren;
            CmbMaand.ItemsSource = Enum.GetValues(typeof(Maanden));
            CmbJaar.SelectedIndex = 2;
            CmbMaand.SelectedIndex = huidigeMaand-1;
        }
        
        private void CmbJaar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridKalender.Children.Clear();
            GenereerKalender();
        }

        private void CmbMaand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridKalender.Children.Clear();
            GenereerKalender();
        }
        private void GenereerKalender()
        {
            if (CmbMaand.SelectedItem != null && CmbJaar.SelectedItem != null)
            {
                int maand = CmbMaand.SelectedIndex + 1;
                int jaar = int.Parse(CmbJaar.SelectedItem.ToString());

                DateTime eersteVanDeMaand = new DateTime(jaar, maand, 1);

                //dag van de week: zondag = 0 // zondag (0+6) // 6/7= rest 6 % modulus operator zorgt ervoor dat de waarden binnen het bereik 0–6 blijven
                int dagVanDeWeek = ((int)eersteVanDeMaand.DayOfWeek + 6) % 7;

                // Houd rij en kolom bij voor plaatsing in het hoofdgrid
                int huidigeRij = 0;
                int huidigeKolom = dagVanDeWeek;

                // Haal het aantal dagen in de maand op
                int dagenInDeMaand = DateTime.DaysInMonth(jaar, maand);

                // Start met het plaatsen van de dagen
                for (int dag = 1; dag <= dagenInDeMaand; dag++)
                {
                    // Creëer een genest grid voor elke dag
                    Grid dagGrid = new Grid
                    {
                        Margin = new Thickness(1),
                        Background = Brushes.LightGray
                    };
                    // Voeg 5 rijen toe aan het geneste grid (1 voor dag/maand, 4 voor lege labels)
                    for (int r = 0; r < 5; r++)
                    {
                        dagGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }
                    // Voeg 2 kolommen toe aan het geneste grid
                    for (int c = 0; c < 2; c++)
                    {
                        dagGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    }
                    // Voeg bovenste label toe (dag en maand)
                    Label lblDagMaand = new Label
                    {
                        Content = $"{dag} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(maand)}",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 12,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(0.5)
                    };
                    Grid.SetRow(lblDagMaand, 0); // Eerste rij van 5
                    Grid.SetColumnSpan(lblDagMaand, 2); // Beslaat beide kolommen
                    dagGrid.Children.Add(lblDagMaand);

                    // Voeg lege labels toe aan de overige 4 rijen
                    for (int subRij = 1; subRij <= 4; subRij++) // Start bij rij 1
                    {
                        for (int subKolom = 0; subKolom < 2; subKolom++)
                        {
                            Label lbl = new Label
                            {
                                Background = Brushes.White, // Optioneel
                                BorderBrush = Brushes.Black,
                                BorderThickness = new Thickness(0.5),
                            };
                            ComboBox cbx = new ComboBox
                            {
                                Visibility = Visibility.Hidden
                            };
                            Grid.SetRow(lbl, subRij);
                            Grid.SetColumn(lbl, subKolom);
                            dagGrid.Children.Add(lbl);
                            labelKalender.Add(lbl);
                            Grid.SetRow(cbx, subRij);
                            Grid.SetColumn(cbx, subKolom);
                            dagGrid.Children.Add(cbx);
                            cbxKalender.Add(cbx);
                        }
                    }
                    // Voeg het geneste grid toe aan het hoofdgrid op de juiste rij/kolom
                    Grid.SetRow(dagGrid, huidigeRij);
                    Grid.SetColumn(dagGrid, huidigeKolom);
                    GridKalender.Children.Add(dagGrid);

                    // Bereken de volgende cel (rij en kolom)
                    huidigeKolom++;
                    if (huidigeKolom >= 7) // Ga naar de volgende week
                    {
                        huidigeKolom = 0;
                        huidigeRij++;
                    }
                }
            }
        }
        private void Plan_Click(object sender, RoutedEventArgs e)
        {
            if (toggle)
            {
                foreach (Label lbl in labelKalender)
                {
                    lbl.Visibility = Visibility.Hidden;
                }
                foreach (ComboBox cbx in cbxKalender)
                {
                    cbx.Visibility = Visibility.Visible;
                }
            }
            else 
            {
                foreach (Label lbl in labelKalender)
                {
                    lbl.Visibility = Visibility.Visible;
                }
                foreach (ComboBox cbx in cbxKalender)
                {
                    cbx.Visibility = Visibility.Hidden;
                }
            }
            toggle = !toggle;
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            NewPerson newPerson = new NewPerson();
            newPerson.ShowDialog();
        }
    }
}
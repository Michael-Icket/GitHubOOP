using Newtonsoft.Json;
using System.Globalization;
using System.IO;
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
        private List<int> IndexNr = new List<int>();

        bool toggle = true;
        public List<Ambulancier> ambulanciers { get; set; }
        public List<MaandPlanAmbu> maandPlanAmbus { get; set; } = new List<MaandPlanAmbu>();
        public List<MaandPlanning> maandPlannings { get; set; } = new List<MaandPlanning>();

        string bestandspadAmbu;
        string bestandspadPlanAmbu;
        string bestandspadMaandPlan;
        string jsonMap;

        public MainWindow()
        {
            InitializeComponent();
            GenereerTime();
            BasisGrid();
            jsonMap = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(jsonMap))
            {
                Directory.CreateDirectory(jsonMap);
            }
            bestandspadAmbu = System.IO.Path.Combine(jsonMap, "ambulanciers.json");
            openJSON();
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
            CmbMaand.SelectedIndex = huidigeMaand - 1;
        }
        private void BasisGrid()
        {
            // Genereer Opmaak Week Nummer en Uren Shift
            int p = 0;
            for (int r = 0; r < 6; r++)
            {
                GridWeek.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                TextBlock txb = new TextBlock();
                txb.Text = (r + 1).ToString();
                txb.Background = Brushes.LightGray;
                txb.FontSize = 20;
                txb.VerticalAlignment = VerticalAlignment.Top;
                txb.HorizontalAlignment = HorizontalAlignment.Stretch;
                txb.TextAlignment = TextAlignment.Center;

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
            // Genereer het grid dynamisch voor de kalender (6 rijen, 7 kolommen)

            for (int i = 0; i < 6; i++)
            {
                GridKalender.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
            for (int j = 0; j < 7; j++)
            {
                GridKalender.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
        }
        private void openJSON()
        {
            try
            {
                string json = File.ReadAllText(bestandspadAmbu);
                ambulanciers = JsonConvert.DeserializeObject<List<Ambulancier>>(json);
                //MessageBox.Show($"Gegevens geladen {ambulanciers.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gegevens konden niet worden geladen van de database: {ex.Message}");
            }
        }
        private void CmbJaar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridKalender.Children.Clear();
            labelKalender.Clear();
            IndexNr.Clear();
            GenereerKalender();
        }

        private void CmbMaand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridKalender.Children.Clear();
            labelKalender.Clear();
            IndexNr.Clear();
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

                            lbl.MouseLeftButtonDown += Kliklabel_MouseLeftButtonDown;

                            Grid.SetRow(lbl, subRij);
                            Grid.SetColumn(lbl, subKolom);
                            dagGrid.Children.Add(lbl);
                            labelKalender.Add(lbl);
                            //lbl.Content = labelKalender.IndexOf(lbl);

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
        private void Kliklabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtLogin.Text))
            {
                MessageBox.Show("Gelieve eerst in te loggen");
            }
            else
            {
                Ambulancier searchAmbu = ZoekAmbulancier();
                Label klikLabel = sender as Label;

                if (klikLabel == null) return;

                int index = labelKalender.IndexOf(klikLabel);
                bool CRijbewijs = searchAmbu.Crijbewijs;
                bool checkIndex = IndexNr.Contains(index);

                // Bepaal groepen van indexen
                List<int> indexGroep = new List<int>();

                if (index <= 1)
                {
                    MessageBox.Show("Deze shift is vorige maand al ingevuld");
                }
                else if (index == labelKalender.Count - 1 || index == labelKalender.Count - 2) // Laatste twee horen bij elkaar met eerste twee volgende maand
                {
                    indexGroep.Add(labelKalender.Count - 1);
                    indexGroep.Add(labelKalender.Count - 2);
                }
                // Bekijk of dit een nachtshift(12u) is: laatste periode van de dag of eerste periode van de dag (linker en rechterkolom)
                else if (index % 8 >= 6 || index % 8 < 2)
                {
                    int groepStart = ((index - 6) / 8) * 8 + 6; // Startindex van de groep (6, 14, 22, ...)
                    indexGroep.AddRange(new int[] { groepStart, groepStart + 1, groepStart + 2, groepStart + 3 });
                }
                // Dit zijn dagshiften(6u), linker en rechterkolom
                else if (index % 8 >= 2 && index % 8 <= 5)
                {
                    indexGroep.Add(index);
                    indexGroep.Add(index % 2 == 0 ? index + 1 : index - 1); // Voeg buurindex toe (2 met 3, 4 met 5, enz.)
                }
                // Indien je geen chauffeur bent, kan je enkel op de rechterkolom staan. Linkerkolom is altijd een chauffeur.
                if (!CRijbewijs)
                {
                    indexGroep.RemoveAll(i => i % 2 == 0);
                }
                if (checkIndex)
                {   //Klikken om je shift te verwijderen
                    foreach (var i in indexGroep)
                    {
                        labelKalender[i].Background = Brushes.White;
                        IndexNr.Remove(i);
                    }
                }
                else
                {   //Klikken om je shift toe te voegen
                    foreach (var i in indexGroep)
                    {
                        labelKalender[i].Background = Brushes.LightGreen;
                        IndexNr.Add(i);
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
            NewPerson newPerson = new NewPerson(ambulanciers, bestandspadAmbu, jsonMap);
            newPerson.ShowDialog();
            ambulanciers = newPerson.ambulanciers;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            GridKalender.Children.Clear();
            labelKalender.Clear();
            IndexNr.Clear();
            GenereerKalender();
            Ambulancier searchAmbu = ZoekAmbulancier();
            if (searchAmbu != null)
            {
                VulGegevensIn(searchAmbu);
            }
            else { MessageBox.Show("Vul je AmbuID in"); }
        }
        private Ambulancier ZoekAmbulancier()
        {
            try
            {
                int AmbuID = int.Parse(TxtLogin.Text);
                var searchAmbu = ambulanciers.FirstOrDefault(a => a.WerknemerNummer == AmbuID);
                if (searchAmbu != null)
                {
                    return searchAmbu;
                }
                else{MessageBox.Show($"Werknemersnummer {AmbuID} is niet gekend.");
                    return null;
                }
            }
            catch (Exception er){MessageBox.Show($"Er ging iets fout {er.Message}");
                return null;
            }
        }
        private void VulGegevensIn(Ambulancier ambulancier)
        {
            LblGegevens1.Content = ($"{ambulancier.Naam} {ambulancier.Voornaam} - tel: {ambulancier.Telefoonnr}");
            LblGegevens2.Content = ($"Jaren ambulancier: {ambulancier.JarenErvaring} - {ambulancier.TypeAmbu}");
            cbPlanner.IsChecked = ambulancier.Planner;
            cbCRijbewijs.IsChecked = ambulancier.Crijbewijs;
            
            if (ambulancier.Planner == true)
            {
                GbPlanning.Visibility = Visibility.Visible;
                BtnNew.Visibility = Visibility.Visible;
            }
            else
            {
                GbPlanning.Visibility = Visibility.Hidden;
                BtnNew.Visibility = Visibility.Hidden;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Ambulancier searchAmbu = ZoekAmbulancier();
            if (searchAmbu != null)
            {
                string PlanningNr = CmbJaar.SelectedItem.ToString() + (CmbMaand.SelectedIndex + 1).ToString("D2") + searchAmbu.WerknemerNummer.ToString("D2");
                try
                {
                    int MaxD = int.Parse(TxtMaxDag.Text);
                    int MinN = int.Parse(TxtMinDag.Text);
                    int aantalS = IndexNr.Count();
                    MaandPlanAmbu planAmbu = new MaandPlanAmbu(MaxD, MinN, aantalS, PlanningNr, IndexNr);
                    maandPlanAmbus.Add(planAmbu);
                    saveJSONP(planAmbu, searchAmbu.WerknemerNummer);
                    MessageBox.Show($"Planning {planAmbu.PlanningNr} is toegevoegd");
                }
                catch (Exception er) { MessageBox.Show($"Vul de vakjes voor het maximum aantal nachten en dagen deze maand. {er.Message}"); }
            }
            else { MessageBox.Show("Vul je AmbuID in"); }
        }
        private void saveJSONP(MaandPlanAmbu planAmbu, int AmbuID)
        {
            try
            {
                bestandspadPlanAmbu = System.IO.Path.Combine(jsonMap, $"planning{AmbuID}.json");
                string json = JsonConvert.SerializeObject(planAmbu, Formatting.Indented);
                File.WriteAllText(bestandspadPlanAmbu, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Planning niet toegevoegd aan de database: {ex.Message}");
            }
        }
    }
}
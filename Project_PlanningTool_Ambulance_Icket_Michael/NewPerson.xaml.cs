using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;

namespace Project_PlanningTool_Ambulance_Icket_Michael
{
    /// <summary>
    /// Interaction logic for NewPerson.xaml
    /// </summary>
    public partial class NewPerson : Window
    {
        public List<Ambulancier> ambulanciers { get; private set; }
        string bestandspad;
        string jsonMap;
        public NewPerson(List<Ambulancier> ambu, string bestandspadM, string jsonMapM)
        {
            InitializeComponent();
            CmbTypeA.ItemsSource = Enum.GetValues(typeof(TypeAmbulancier));

            jsonMap = jsonMapM;
            bestandspad = bestandspadM;
            ambulanciers = ambu;
        }
        
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            bool CheckOK = true;
            CheckGegevens(ref CheckOK);
            if (CheckOK)
            {
                Ambulancier searchAmbu = ZoekAmbulancier();
                if (searchAmbu != null)
                {
                    MessageBox.Show($"Werknemersnummer {searchAmbu.WerknemerNummer} is reeds gekend, de gegevens worden overschreven.");
                    ChangeAmbulancier(searchAmbu);
                }
                else 
                {
                    NewAmbulancier();
                    JSONSaveA();
                    setDefault();
                }
            }
        }
        private bool CheckGegevens (ref bool check)
        {
            foreach (var child in SpNewP.Children)
            {
                if (child is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
                {
                    MessageBox.Show($"De TextBox '{textBox.Name}' is leeg. Vul deze in.");
                    check = false;
                    return check;
                }
                else if (child is ComboBox comboBox && comboBox.SelectedItem == null)
                {
                    MessageBox.Show($"De ComboBox '{comboBox.Name}' is niet geselecteerd. Kies een optie.");
                    check = false;
                    return check;
                }
            }

            return check;
        }
        private void ChangeAmbulancier(Ambulancier searchAmbu)
        {
            searchAmbu.Naam = TxtNaam.Text;
            searchAmbu.Voornaam = TxtVNaam.Text;
            searchAmbu.Telefoonnr = TxtTel.Text;
            searchAmbu.Crijbewijs = cbCrijbewijs.IsChecked ?? false;
            searchAmbu.Planner = cbPlanner.IsChecked ?? false;
            searchAmbu.JarenErvaring = DateTime.Now.Year - int.Parse(TxtJaar.Text);
            searchAmbu.TypeAmbu = (TypeAmbulancier)CmbTypeA.SelectedItem;

            string jsonOutput = JsonConvert.SerializeObject(ambulanciers, Formatting.Indented);
            File.WriteAllText(bestandspad, jsonOutput);
            setDefault();
        }
        private void NewAmbulancier()
        {
            try
            {
                Ambulancier ambulancier = new Ambulancier
                (
                    int.Parse(TxtID.Text),
                    TxtNaam.Text,
                    TxtVNaam.Text,
                    TxtTel.Text,
                    cbCrijbewijs.IsChecked ?? false,
                    cbPlanner.IsChecked ?? false,
                    DateTime.Now.Year - int.Parse(TxtJaar.Text),
                    (TypeAmbulancier)CmbTypeA.SelectedItem
                );
                ambulanciers.Add( ambulancier );
                //MessageBox.Show($"Ambulancier {ambulancier.Naam} {ambulancier.Voornaam} succesvol aangemaakt!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Er is een fout opgetreden: {ex.Message}");
            }
        }
        private void JSONSaveA()
        {
            try
            {
                // Serialiseer de lijst naar JSON
                string json = JsonConvert.SerializeObject(ambulanciers, Formatting.Indented);

                // Schrijf naar bestand
                File.WriteAllText(bestandspad, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gegevens niet toegevoegd aan database: {ex.Message}");
            }
        }
        private void setDefault()
        {
            foreach (var child in SpNewP.Children)
            {
                if (child is TextBox textbox)
                {
                    textbox.Clear();
                }
                if (child is ComboBox combobox)
                {
                    combobox.SelectedIndex = -1;
                }
                if (child is CheckBox checkbox)
                {
                    checkbox.IsChecked = false;
                }
            }
        }
        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            Ambulancier searchAmbu = ZoekAmbulancier();
            if(searchAmbu!=null)
            { 
            VulGegevensIn(searchAmbu);
            }
        }
        private Ambulancier ZoekAmbulancier()
        {
            try
            {
                int AmbuID = int.Parse(TxtID.Text);
                var searchAmbu = ambulanciers.FirstOrDefault(a => a.WerknemerNummer == AmbuID);
                if (searchAmbu != null)
                {
                    return searchAmbu;
                }
                else
                {
                    MessageBox.Show($"Werknemersnummer {AmbuID} is niet gekend.");
                    return null;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show($"Er ging iets fout {er.Message}");
                return null;
            }
        }
        private void VulGegevensIn(Ambulancier ambulancier) 
        {
            setDefault();
            TxtID.Text = ambulancier.WerknemerNummer.ToString();
            TxtNaam.Text = ambulancier.Naam;
            TxtVNaam.Text = ambulancier.Voornaam;
            TxtTel.Text = ambulancier.Telefoonnr;
            TxtJaar.Text = (DateTime.Now.Year - ambulancier.JarenErvaring).ToString(); 
            CmbTypeA.SelectedItem = ambulancier.TypeAmbu;
            cbCrijbewijs.IsChecked = ambulancier.Crijbewijs;
            cbPlanner.IsChecked = ambulancier.Planner;
        }
    }
}

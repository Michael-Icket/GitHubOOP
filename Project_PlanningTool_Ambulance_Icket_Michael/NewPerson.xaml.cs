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

namespace Project_PlanningTool_Ambulance_Icket_Michael
{
    /// <summary>
    /// Interaction logic for NewPerson.xaml
    /// </summary>
    public partial class NewPerson : Window
    {
        public NewPerson()
        {
            InitializeComponent();
            CmbTypeA.ItemsSource = Enum.GetValues(typeof(TypeAmbulancier));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool CheckOK = true;
            CheckGegevens(ref CheckOK);
            if (CheckOK)
            {
                NewAmbulancier();
            }
        }
        private bool CheckGegevens (ref bool check)
        {
            foreach (var child in SpNewP.Children)
            {
                if (child is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
                {
                    MessageBox.Show($"De TextBox '{textBox.Name}' is leeg. Vul deze in!");
                    check = false;
                    return check;
                }
                else if (child is ComboBox comboBox && comboBox.SelectedItem == null)
                {
                    MessageBox.Show($"De ComboBox '{comboBox.Name}' is niet geselecteerd. Kies een optie!");
                    check = false;
                    return check;
                }
            }
            return check;
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
                    DateTime.Now.Month - int.Parse(TxtJaar.Text),
                    (TypeAmbulancier)CmbTypeA.SelectedItem
                );
                MessageBox.Show($"Ambulancier {ambulancier.Naam} {ambulancier.Voornaam} succesvol aangemaakt!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Er is een fout opgetreden: {ex.Message}");
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int AmbuID = int.Parse(TxtID.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Geen correcte ID: {TxtID.Text}");
            }

            
        }
    }
}

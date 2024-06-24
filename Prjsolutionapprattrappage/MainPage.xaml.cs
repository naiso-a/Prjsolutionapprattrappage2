using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Prjsolutionapprattrappage
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel viewModel = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = viewModel; // Définir le modèle de vue pour la liaison des données
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                int resultat = await viewModel.GetEnfantNaissance();
                LabelResultat.Text = $"Nombre d'enfants nés à Nantes avec le prénom {viewModel.Prenom} en {viewModel.AnneeSelectionnee.Year}: {resultat}";
            }
            catch (Exception ex)
            {
                LabelResultat.Text = $"Erreur: {ex.Message}";
            }
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Prenom { get; set; }

        private DateTime anneeSelectionnee = DateTime.Now;
        public DateTime AnneeSelectionnee
        {
            get { return anneeSelectionnee; }
            set
            {
                anneeSelectionnee = value;
                OnPropertyChanged("AnneeSelectionnee"); // Notify about property change
            }
        }

        public async Task<int> GetEnfantNaissance()
        {


            string apiUrl = "https://data.nantesmetropole.fr/api/explore/v2.1/catalog/datasets/244400404_prenoms-enfants-nes-nantes/records?limit=20";
            string prenom = Prenom.ToLower();
            int annee = AnneeSelectionnee.Year;
            string formattedUrl = string.Format(apiUrl, prenom, annee);


            using (var client = new HttpClient())
            {

                var response = await client.GetAsync(formattedUrl);


                if (response.IsSuccessStatusCode)
                {

                    string json = await response.Content.ReadAsStringAsync();


                    dynamic data = JsonSerializer.Deserialize<dynamic>(json);
                    int nombreEnfants = data.nombreEnfants;

                    return nombreEnfants;
                }
                else
                {

                    throw new Exception($"Erreur API: {response.StatusCode}");
                }
            }
        
    }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Util;
using Android.Runtime;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Xml;

namespace RetoGeolocalizacion01
{
    [Activity(Label = "RetoGeolocalizacion01", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener
    {
        // UI
        TextView addressText;
        TextView lugarText;
        TextView autorText;

        // Location
        Location currentLocation;
        LocationManager locationManager;
        string locationProvider;

        string countryCode;
        Country country;
            
        private void InitializeLocationManager()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };

            IList<string> acceptableLocationProviders =
                locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
            }
            Log.Debug("Reto01 Geolocalizacion", "Using " + locationProvider + ".");
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            addressText = FindViewById<TextView>(Resource.Id.textViewLocation);
            lugarText = FindViewById<TextView>(Resource.Id.textViewLugar);
            autorText = FindViewById<TextView>(Resource.Id.textViewAuthor);
            InitializeLocationManager();

            countryCode = "";
            country = new Country();
            addressText.Text = "";
            lugarText.Text = "";
            autorText.Text = "\n\nName: Germán Santa Cruz\nEmail: german.scf@hotmail.com";
        }

        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
            countryCode = "";
            country.AreaInSqKm = "";
            country.Capital = "";
            country.ContinentName = "";
            country.CountryCode = "";
            country.CountryName = "";
            country.CurrencyCode = "";
            country.Languages = "";
            country.Population = "";
        }

        public void OnLocationChanged(Location location)
        {
            currentLocation = location;
            if (currentLocation == null)
            {
                addressText.Text = "Unable to determine your location. Try again in a short while.";
            }
            //else if(countryCode.Trim().Length == 0) 
            else
            {
                addressText.Text = string.Format("Location: {0:f6},{1:f6}", currentLocation.Latitude, currentLocation.Longitude);
                DownloadDataAsync(currentLocation);               
            }
        }

        public async void DownloadDataAsync(Location current)
        {          
            // Obtener el Country Code
            string url = "http://api.geonames.org/countryCode?lat=" +
            currentLocation.Latitude.ToString().Replace(",", ".") +
            "&lng=" + currentLocation.Longitude.ToString().Replace(",", ".") +
            "&username=germansantacruz";

            var httpClient = new HttpClient();
            Task<string> downloadTask = httpClient.GetStringAsync(url);
            string content = await downloadTask;
            content = content.Replace("\r", "").Replace("\n", "");
            countryCode = content;

            // Obtener datos del país
            url = "http://api.geonames.org/countryInfo?lang=es&country=" +
                countryCode + "&username=germansantacruz";
            Task<string> downloadTask2 = httpClient.GetStringAsync(url);
            content = await downloadTask2;
           
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);
            Country country = new Country();
           
            XmlNodeList xnList = xmlDoc.SelectNodes("geonames/country");
            foreach (XmlNode xn in xnList)
            {
                country.CountryCode = xn["countryCode"].InnerText;
                country.CountryName = xn["countryName"].InnerText;
                country.ContinentName = xn["continentName"].InnerText;
                country.CurrencyCode = xn["currencyCode"].InnerText;
                country.Capital = xn["capital"].InnerText;
                country.AreaInSqKm = xn["areaInSqKm"].InnerText;
                country.Languages = xn["languages"].InnerText;
                country.Population = xn["population"].InnerText;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Country code:  ");
            sb.Append(country.CountryCode);
            sb.Append("\n");
            sb.Append("Country:  ");
            sb.Append(country.CountryName);
            sb.Append("\n");
            sb.Append("Continent:  ");
            sb.Append(country.ContinentName);
            sb.Append("\n");
            sb.Append("Capital:  ");
            sb.Append(country.Capital);
            sb.Append("\n");
            sb.Append("Currency:  ");
            sb.Append(country.CurrencyCode);
            sb.Append("\n");
            sb.Append("Languages:  ");
            sb.Append(country.Languages);
            sb.Append("\n");
            sb.Append("Area in sq. Km:  ");
            sb.Append(country.AreaInSqKm);
            sb.Append("\n");
            sb.Append("Population:  ");
            sb.Append(country.Population);

            lugarText.Text = sb.ToString();            
        }

        public void OnProviderDisabled(string provider)
        {}
        public void OnProviderEnabled(string provider)
        {}
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {}
    }
}
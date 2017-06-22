using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RetoGeolocalizacion01
{
    public class Country
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string ContinentName { get; set; }
        public string Capital { get; set; }
        public string AreaInSqKm { get; set; }
        public string Population { get; set; }
        public string CurrencyCode { get; set; }
        public string Languages { get; set; }      
    }
}
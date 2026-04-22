using System;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;


namespace MultiMediaCenter.Controls
{
    public partial class MapForm : Form
    {
        public MapForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            System.Net.HttpWebRequest.DefaultWebProxy = null; // opcjonalnie

            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.Net.WebRequest.DefaultWebProxy = null;

            // 🔑 TO JEST KLUCZ
            //System.Net.HttpWebRequest.DefaultUserAgent = "MultiMediaCenter/1.0 (kontakt: mail@example.com)";

            // ✅ tylko raz i poprawnie
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            gMapControl1.MapProvider = GMapProviders.OpenStreetMap;

            gMapControl1.MinZoom = 2;
            gMapControl1.MaxZoom = 18;
            gMapControl1.Zoom = 10;
            gMapControl1.Position = new PointLatLng(52.2297, 21.0122);
        }
    }
}

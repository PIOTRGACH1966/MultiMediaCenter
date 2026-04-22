using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;


namespace MultiMediaCenter.Controls
{
    public partial class MapForm : UserControl
    {
        private bool isInitialized = false;
        private bool isResizing = false;
        private Point resizeStartPoint;
        private Size resizeStartSize;
        private const int ResizeHandleSize = 12; // rozmiar obszaru resize'a w rogu

        public MapForm()
        {
            InitializeComponent();
        }   

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // OnLoad na UserControl może się nie wywoła zaraz po InitializeComponent
            // dlatego inicjalizacja będzie w ShowLocation()
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(EventArgs.Empty);
            // Daj focus do gMapControl1, żeby otrzymywał mouse events
            if (gMapControl1 != null && !gMapControl1.Focused)
            {
                gMapControl1.Focus();
            }
        }

        // Deleguj mouse events do gMapControl1, żeby drag działał
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Sprawdź czy klikasz w rogu (resize handle)
            if (IsInResizeHandle(e.Location))
            {
                isResizing = true;
                resizeStartPoint = e.Location;
                resizeStartSize = this.Size;
                this.Cursor = Cursors.SizeNWSE;
            }
            else if (gMapControl1 != null)
            {
                gMapControl1.Focus();
                // Przekaż event do gMapControl1
                gMapControl1.PerformLayout();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isResizing)
            {
                // Oblicz nowy rozmiar
                int deltaX = e.X - resizeStartPoint.X;
                int deltaY = e.Y - resizeStartPoint.Y;

                int newWidth = Math.Max(100, resizeStartSize.Width + deltaX);
                int newHeight = Math.Max(100, resizeStartSize.Height + deltaY);

                this.Size = new Size(newWidth, newHeight);
            }
            else if (IsInResizeHandle(e.Location))
            {
                // Pokaż resize cursor
                this.Cursor = Cursors.SizeNWSE;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isResizing = false;
            this.Cursor = Cursors.Default;
        }

        private bool IsInResizeHandle(Point location)
        {
            // Sprawdź czy punkt jest w prawym dolnym rogu (resize handle)
            return location.X >= this.Width - ResizeHandleSize && 
                   location.Y >= this.Height - ResizeHandleSize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Narysuj resize handle w prawym dolnym rogu
            int handleSize = ResizeHandleSize;
            Point[] resizeHandleTriangle = new Point[]
            {
                new Point(this.Width - handleSize, this.Height),
                new Point(this.Width, this.Height - handleSize),
                new Point(this.Width, this.Height)
            };

            using (Brush brush = new SolidBrush(Color.FromArgb(180, 100, 100, 100)))
            {
                e.Graphics.FillPolygon(brush, resizeHandleTriangle);
            }

            using (Pen pen = new Pen(Color.Gray, 1))
            {
                e.Graphics.DrawPolygon(pen, resizeHandleTriangle);
            }
        }

        /// <summary>
        /// Inicjalizuj mapę (wywoła się na demand z ShowLocation)
        /// </summary>
        private void EnsureInitialized()
        {
            if (isInitialized)
                return;

            try
            {
                // Ustaw proxy i protokół
                HttpWebRequest.DefaultWebProxy = null;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebRequest.DefaultWebProxy = null;
                ServicePointManager.DefaultConnectionLimit = 10;

                // Użyj GoogleMap
                gMapControl1.MapProvider = GMapProviders.GoogleMap;

                gMapControl1.MinZoom = 1;
                gMapControl1.MaxZoom = 18;
                gMapControl1.CanDragMap = true;
                gMapControl1.DragButton = MouseButtons.Left;

                isInitialized = true;
                System.Diagnostics.Debug.WriteLine("MapForm: Mapa zainicjalizowana");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MapForm: Błąd inicjalizacji: {ex.Message}");
                MessageBox.Show($"Błąd inicjalizacji mapy: {ex.Message}", "Błąd mapy");
            }
        }

        /// <summary>
        /// Pokaż mapę na wskazanych współrzędnych GPS
        /// </summary>
        public void ShowLocation(double latitude, double longitude, string toolTipText = "Lokalizacja ze zdjęcia")
        {
            System.Diagnostics.Debug.WriteLine($"MapForm.ShowLocation: lat={latitude}, lon={longitude}, tooltip={toolTipText}");

            try
            {
                EnsureInitialized();

                var location = new PointLatLng(latitude, longitude);
                gMapControl1.Position = location;
                gMapControl1.Zoom = 12;

                // Usuń stare markery
                gMapControl1.Overlays.Clear();

                // Dodaj nowy marker
                var markers = new GMapOverlay("markers");
                var marker = new GMarkerGoogle(location, GMarkerGoogleType.blue_dot);
                marker.ToolTipText = toolTipText;
                markers.Markers.Add(marker);
                gMapControl1.Overlays.Add(markers);

                // Upewnij się że gMapControl ma focus i jest na wierzchu
                gMapControl1.Focus();
                this.BringToFront();

                System.Diagnostics.Debug.WriteLine("MapForm.ShowLocation: Marker dodany");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MapForm.ShowLocation: Błąd: {ex.Message}");
                MessageBox.Show($"Błąd wyświetlania lokalizacji: {ex.Message}", "Błąd mapy");
            }
        }

        /// <summary>
        /// Wyczyść mapę
        /// </summary>
        public void Clear()
        {
            try
            {
                gMapControl1.Overlays.Clear();
            }
            catch { }
        }
    }
}

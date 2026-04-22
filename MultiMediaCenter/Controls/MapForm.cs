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
        private Point dragOffset;

        private enum ResizeDirection { None, Left, Right, Top, Bottom, BottomRight }
        private ResizeDirection _currentResizeDir = ResizeDirection.None;
        private const int EdgeMargin = 5; // Szerokość czułego obszaru krawędzi

        public MapForm()
        {
            InitializeComponent();

            pnlDragHandle.MouseDown += DragHandle_MouseDown;
            pnlDragHandle.MouseMove += DragHandle_MouseMove;
        }   

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int targetWidth = 200;
            int targetHeight = 300;

            // Oblicz różnicę szerokości, aby zachować prawą krawędź
            int widthDifference = this.Width - targetWidth;

            // Ustaw nowy rozmiar i przesuń w prawo o różnicę
            this.Size = new Size(targetWidth, targetHeight);
            this.Left += widthDifference;

            // Opcjonalnie: upewnij się, że kontrolka jest na wierzchu
            this.BringToFront();

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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_currentResizeDir != ResizeDirection.None)
            {
                isResizing = true;
                resizeStartPoint = Cursor.Position; // Globalna pozycja myszy
                resizeStartSize = this.Size;
                // Zapamiętaj startową pozycję kontrolki dla resize'u lewej strony
                _resizeStartLocation = this.Location;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isResizing)
            {
                PerformResize(e);
                return;
            }

            // Wykrywanie nad którą krawędzią jest mysz, aby zmienić kursor
            if (e.X >= this.Width - ResizeHandleSize && e.Y >= this.Height - ResizeHandleSize)
            {
                this.Cursor = Cursors.SizeNWSE;
                _currentResizeDir = ResizeDirection.BottomRight;
            }
            else if (e.X <= EdgeMargin)
            {
                this.Cursor = Cursors.SizeWE;
                _currentResizeDir = ResizeDirection.Left;
            }
            else if (e.X >= this.Width - EdgeMargin)
            {
                this.Cursor = Cursors.SizeWE;
                _currentResizeDir = ResizeDirection.Right;
            }
            else if (e.Y >= this.Height - EdgeMargin)
            {
                this.Cursor = Cursors.SizeNS;
                _currentResizeDir = ResizeDirection.Bottom;
            }
            else
            {
                this.Cursor = Cursors.Default;
                _currentResizeDir = ResizeDirection.None;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isResizing = false;
            this.Cursor = Cursors.Default;
        }

        private Point _resizeStartLocation;

        private void PerformResize(MouseEventArgs e)
        {
            Point currentMousePos = Cursor.Position;
            int deltaX = currentMousePos.X - resizeStartPoint.X;
            int deltaY = currentMousePos.Y - resizeStartPoint.Y;

            switch (_currentResizeDir)
            {
                case ResizeDirection.Right:
                    this.Width = Math.Max(100, resizeStartSize.Width + deltaX);
                    break;
                case ResizeDirection.Bottom:
                    this.Height = Math.Max(100, resizeStartSize.Height + deltaY);
                    break;
                case ResizeDirection.BottomRight:
                    this.Width = Math.Max(100, resizeStartSize.Width + deltaX);
                    this.Height = Math.Max(100, resizeStartSize.Height + deltaY);
                    break;
                case ResizeDirection.Left:
                    // Resize lewej krawędzi wymaga zmiany szerokości I pozycji X
                    int newWidth = Math.Max(100, resizeStartSize.Width - deltaX);
                    if (newWidth > 100)
                    {
                        this.Left = _resizeStartLocation.X + deltaX;
                        this.Width = newWidth;
                    }
                    break;
            }
        }

        private void DragHandle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Zapamiętaj gdzie kliknąłeś względem lewego górnego rogu kontrolki
                dragOffset = e.Location;
            }
        }

        private void DragHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Oblicz nową pozycję całej kontrolki MapForm na formie-rodzicu
                this.Left += e.X - dragOffset.X;
                this.Top += e.Y - dragOffset.Y;
            }
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

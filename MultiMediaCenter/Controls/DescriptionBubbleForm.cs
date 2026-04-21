using System;
using System.Drawing;
using System.Windows.Forms;

namespace MultiMediaCenter
{
    public class DescriptionBubbleForm : Form
    {
        // --- globalny przelacznik (wlacza/wylacza wszystkie dymki w aplikacji) ---
        private static bool _globalEnabled = true;
        private static event EventHandler GlobalEnabledChanged;

        public static bool GlobalEnabled
        {
            get { return _globalEnabled; }
            set
            {
                if (_globalEnabled == value) return;
                _globalEnabled = value;
                if (GlobalEnabledChanged != null)
                    GlobalEnabledChanged(null, EventArgs.Empty);
            }
        }

        public static readonly Size BubbleSize = new Size(160, 160);

        // --- stan instancji ---
        private readonly TextBox textBox;
        private string currentKey;
        private string currentDescription;
        private Form ownerForm;
        private Control anchorControl;

        public DescriptionBubbleForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.FromArgb(255, 255, 204); // jasnozolty
            this.Size = BubbleSize;
            this.Padding = new Padding(6);

            textBox = new TextBox();
            textBox.Multiline = true;
            textBox.ReadOnly = true;
            textBox.WordWrap = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Color.FromArgb(255, 255, 204);
            textBox.Font = new Font("Segoe UI", 9f);
            textBox.Dock = DockStyle.Fill;
            textBox.TabStop = false;
            this.Controls.Add(textBox);

            GlobalEnabledChanged += HandleGlobalEnabledChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                GlobalEnabledChanged -= HandleGlobalEnabledChanged;
            base.Dispose(disposing);
        }

        private void HandleGlobalEnabledChanged(object s, EventArgs e)
        {
            if (!_globalEnabled) { if (this.Visible) this.Hide(); return; }
            TryShow();
        }

        // Wywolywane z Play / PlayFile dla kazdego pliku
        public void ShowFor(string _key, string _description, Form _owner, Control _anchor)
        {
            currentKey = _key;
            currentDescription = _description;

            // Przepnij subskrypcje na nowy owner/anchor
            if (ownerForm != null)
            {
                ownerForm.LocationChanged -= OwnerOrAnchorChanged;
                ownerForm.SizeChanged -= OwnerOrAnchorChanged;
            }
            if (anchorControl != null)
            {
                anchorControl.LocationChanged -= OwnerOrAnchorChanged;
                anchorControl.SizeChanged -= OwnerOrAnchorChanged;
                anchorControl.VisibleChanged -= OwnerOrAnchorChanged;
            }
            ownerForm = _owner;
            anchorControl = _anchor;
            if (ownerForm != null)
            {
                ownerForm.LocationChanged += OwnerOrAnchorChanged;
                ownerForm.SizeChanged += OwnerOrAnchorChanged;
            }
            if (anchorControl != null)
            {
                anchorControl.LocationChanged += OwnerOrAnchorChanged;
                anchorControl.SizeChanged += OwnerOrAnchorChanged;
                anchorControl.VisibleChanged += OwnerOrAnchorChanged;
            }

            TryShow();
        }

        private void TryShow()
        {
            if (!_globalEnabled
                || ownerForm == null
                || anchorControl == null
                /*|| !anchorControl.Visible*/
                || String.IsNullOrWhiteSpace(currentDescription))
            {
                if (this.Visible) this.Hide();
                return;
            }
            textBox.Text = currentDescription;
            textBox.SelectionStart = 0;
            textBox.SelectionLength = 0;
            RepositionToAnchor();
            if (!this.Visible)
            {
                if (this.Owner != ownerForm) this.Owner = ownerForm;
                this.Show(ownerForm);
            }
            this.BringToFront();
        }

        private void OwnerOrAnchorChanged(object s, EventArgs e) { TryShow(); }

        private void RepositionToAnchor()
        {
            if (ownerForm == null || anchorControl == null) return;
            Rectangle screenRect = anchorControl.RectangleToScreen(anchorControl.ClientRectangle);
            int x = screenRect.Right - this.Width - 8;
            int y = screenRect.Top + 8;
            if (x < screenRect.Left + 8) x = screenRect.Left + 8;
            this.Location = new Point(x, y);
        }

        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TOOLWINDOW = 0x00000080;
                const int WS_EX_NOACTIVATE = 0x08000000;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
                return cp;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DescriptionBubbleForm
            // 
            this.ClientSize = new System.Drawing.Size(180, 261);
            this.Name = "DescriptionBubbleForm";
            this.ResumeLayout(false);

        }
    }
}
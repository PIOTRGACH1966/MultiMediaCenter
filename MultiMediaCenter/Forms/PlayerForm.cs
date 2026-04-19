using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiMediaCenter
{
    public partial class PlayerForm : Form
    {
        public List<PlayableObject> objectsToPlay;
        public int startNdx = -1;
        private int currentNdx = 0;
        private Size fullScreenSize;
        private int fullScreenWidth;
        private int fullScreenHeight;
        private Utils utils = new Utils();
        private double zoomFactor = 1;
        private double zoomFactorCoeff;
        private double moveX = 0;
        private double moveY = 0;
        private double moveDelta;

        public PlayerForm(double _initialZoomFactorCoeff, double _initialMoveDelta)
        {
            InitializeComponent();
            zoomFactorCoeff = _initialZoomFactorCoeff;
            moveDelta = _initialMoveDelta;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            fullScreenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            fullScreenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            fullScreenSize = new Size(fullScreenWidth, fullScreenHeight);
            currentNdx = startNdx;

            this.MouseWheel += new MouseEventHandler(FormFullScreenPlayer_MouseWheel);
            this.PlayFile();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            AVPlayerBox.close();
        }

        private void FormFullScreenPlayer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Home)
                this.PlayFirstFile();
            else if (e.Modifiers != Keys.Control && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.PageUp))
                this.PlayPrevFile();
            else if (e.Modifiers != Keys.Control && (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.PageDown))
                this.PlayNextFile();
            else if (e.KeyCode == Keys.End)
                this.PlayLastFile();
            //else if (e.KeyCode == Keys.OemMinus)
            else if ((int)(e.KeyCode) == 109)
                this.PlayZoomOut();
            //else if (e.KeyCode == Keys.Oemplus)
            else if ((int)(e.KeyCode) == 107)
                this.PlayZoomIn();
            //else if (e.KeyCode == Keys.OemClear)
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Left)
                this.PlayMoveLeft();
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Right)
                this.PlayMoveRight();
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Up)
                this.PlayMoveUp();
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Down)
                this.PlayMoveDown();
            else if ((int)(e.KeyCode) == 106)
                this.ResetAll();
            else if (e.KeyCode == Keys.Escape)
            {
                if (zoomFactor == 1)
                    this.Close();
                else
                    this.ResetAll();
            }
        }
        void FormFullScreenPlayer_MouseWheel(object sender, MouseEventArgs e)
        {
            if(e.Delta>0)
                this.PlayZoomOut();
            else if (e.Delta < 0)
                this.PlayZoomIn();
        }
        private void AVPlayerBox_KeyUpEvent(object sender, AxWMPLib._WMPOCXEvents_KeyUpEvent e)
        {
            if (e.nKeyCode == 9)
                this.PlayPrevFile();
            if (e.nKeyCode == 33)
                this.PlayPrevFile();
            else if (e.nKeyCode == 34)
                this.PlayNextFile();
            else if (e.nKeyCode == 35)
                this.PlayLastFile();
            else if (e.nKeyCode == 27)
                this.Close();
        }

        private void PlayFirstFile()
        {
            currentNdx = 0;
            this.PlayFile();
        }
        private void PlayPrevFile()
        {
            if (currentNdx > 0)
            {
                currentNdx--;
                this.PlayFile();
            }
        }
        private void PlayNextFile()
        {
            if (currentNdx < objectsToPlay.Count-1)
            {
                currentNdx++;
                this.PlayFile();
            }
        }
        private void PlayLastFile()
        {
            currentNdx = objectsToPlay.Count - 1;
            this.PlayFile();
        }

        private void PlayZoomOut()
        {
            if(!this.isPicture())
                return;
            zoomFactor *= zoomFactorCoeff;
            this.PlayFile();
        }
        private void PlayZoomIn()
        {
            if (!this.isPicture())
                return;
            zoomFactor *= (1 / zoomFactorCoeff);
            this.PlayFile();
        }
        private void PlayZoomReset()
        {
            if (!this.isPicture())
                return;
            zoomFactor = 1;
            //this.PlayFile();
        }
        private void PlayMoveLeft()
        {
            if (!this.isPicture())
                return;
            moveX -= moveDelta;
            this.PlayFile();
        }
        private void PlayMoveRight()
        {
            if (!this.isPicture())
                return;
            moveX += moveDelta;
            this.PlayFile();
        }
        private void PlayMoveUp()
        {
            if (!this.isPicture())
                return;
            moveY -= moveDelta;
            this.PlayFile();
        }
        private void PlayMoveDown()
        {
            if (!this.isPicture())
                return;
            moveY += moveDelta;
            this.PlayFile();
        }
        private void PlayMoveReset()
        {
            if (!this.isPicture())
                return;
            moveX = 0;
            moveY = 0;
            //this.PlayFile();
        }

        private void ResetAll()
        {
            this.PlayZoomReset();
            this.PlayMoveReset();
            this.PlayFile();
        }

        private bool isPicture()
        {
            if (currentNdx < 0 || currentNdx >= objectsToPlay.Count)
                return false;
            string fSpec = objectsToPlay[currentNdx].fSpec;
            if (utils.ComputeContentType(fSpec) != ContentType.Picture)
                return false;
            return true;
        }

        private void PlayFile()
        {
            if (currentNdx < 0 || currentNdx >= objectsToPlay.Count)
                return;
            string fSpec = objectsToPlay[currentNdx].fSpec;
            fileNameLabel.Text = System.IO.Path.GetFileName(fSpec);

            ContentType contentType = utils.ComputeContentType(fSpec);
            if (contentType == ContentType.Picture)
            {
                textBox.Visible = false;

                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }
                pictureBox.ImageLocation = null;
                Image img = utils.GetImageFromFile(fSpec);
                if (img == null)
                    return;
                pictureBox.Image = img;
                //Size imageSize = new Size(img.Width, img.Height);
                int x = 0, y = 0;
                if (zoomFactor == 1)
                {
                    Size imageSize = new Size(img.Width, img.Height);
                    if (imageSize.Width < fullScreenSize.Width && imageSize.Height < fullScreenSize.Height)
                    {
                        pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                        x = (fullScreenWidth - imageSize.Width) / 2;
                        if (x < 0) x = 0;
                        y = (fullScreenHeight - imageSize.Height) / 2;
                        if (y < 0) y = 0;                        
                    }
                    else
                    {
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        pictureBox.Size = fullScreenSize;
                    }                    
                }
                else
                {
                    Size imageSize;
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    if (img.Width > fullScreenSize.Width || img.Height > fullScreenSize.Height)
                        imageSize = new Size(Convert.ToInt32(zoomFactor * fullScreenSize.Width), Convert.ToInt32(zoomFactor * fullScreenSize.Height));
                    else
                        imageSize = new Size(Convert.ToInt32(zoomFactor * img.Width), Convert.ToInt32(zoomFactor * img.Height));
                    if (zoomFactor < 1 && (imageSize.Width > fullScreenSize.Width || imageSize.Height > fullScreenSize.Height))
                        pictureBox.Size = fullScreenSize;
                    else
                    {
                        pictureBox.Size = imageSize;
                        x = (fullScreenWidth - imageSize.Width) / 2;
                        y = (fullScreenHeight - imageSize.Height) / 2;
                    }
                }
                if (y == 0)
                    y += 15;
                x += Convert.ToInt32(moveX);
                y += Convert.ToInt32(moveY);
                pictureBox.Location = new Point(x, y);
                pictureBox.Visible = true;
                pictureBox.Show();
                AVPlayerBox.Visible = false;
                AVPlayerBox.close();
            }
            else if (contentType == ContentType.Audio || contentType == ContentType.Video)
            {
                textBox.Visible = false;
                pictureBox.Visible = false;
                AVPlayerBox.Size = fullScreenSize;
                AVPlayerBox.URL = fSpec;
                AVPlayerBox.Visible = true;                
            }
            else if (contentType == ContentType.Text)
            {
                textBox.Size = fullScreenSize;
                textBox.Text = System.IO.File.ReadAllText(fSpec, Encoding.GetEncoding(1250));
                textBox.Visible = true;
                pictureBox.Visible = false;
                AVPlayerBox.Visible = false;
                AVPlayerBox.close();
            }
            else
            {
                textBox.Visible = false;
                pictureBox.Visible = false;
                AVPlayerBox.Visible = false;
            }
        }
    }
}
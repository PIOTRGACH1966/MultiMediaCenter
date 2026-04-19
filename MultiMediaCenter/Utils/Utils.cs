using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using IWshRuntimeLibrary;

namespace MultiMediaCenter
{
    public class Utils
    {
        public Utils()
        {
        }

        public ContentType ComputeContentType(string _fSpec)
        {
            ContentType retVal = ContentType.Unknown;
            string ext = System.IO.Path.GetExtension(_fSpec).ToUpper();
            switch (ext)
            {
                //case "":
                case ".DOC":
                case ".ME":
                case ".TXT":
                    {
                        retVal = ContentType.Text;
                        break;
                    }
                case ".BMP":
                case ".GIF":
                case ".JPG":
                case ".JPEG":
                    {
                        retVal = ContentType.Picture;
                        break;
                    }
                case ".MP3":
                case ".MP4":
                case ".WAV":
                case ".WMA":
                    {
                        retVal = ContentType.Audio;
                        break;
                    }
                case ".AVI":
                case ".MPG":
                case ".WMV":
                case ".MOV":
                    {
                        retVal = ContentType.Video;
                        break;
                    }
                case ".LNK":
                    {
                        string trgFile = this.ShortcutTargetFile(_fSpec);
                        retVal = this.ComputeContentType(trgFile);
                        break;
                    }
            }
            return retVal;
        }

        public string GetThumbSpec(string _fSpec)
        {
            //return System.IO.Path.GetDirectoryName(_fSpec) + "\\" + System.IO.Path.GetFileNameWithoutExtension(_fSpec) +
            //    "_thumb" + System.IO.Path.GetExtension(_fSpec);
            string retVal = String.Empty;
            //D:\AAA\1.jpg
            int secondBckSlashPos = _fSpec.IndexOf("\\", 3);
            retVal = _fSpec.Substring(0, secondBckSlashPos) + ".Thumbs" + _fSpec.Substring(secondBckSlashPos);
            return retVal;
        }
        public bool IsThumbSpec(string _fSpec)
        {
            bool retVal = false;
            //string fName = System.IO.Path.GetFileNameWithoutExtension(_fSpec);
            //if (fName.Length > "_thumb".Length && fName.Substring(fName.Length - "_thumb".Length, "_thumb".Length).ToLower() == "_thumb")
            //    retVal = true;
            retVal = _fSpec.Contains(".Thumbs\\");
            return retVal;
        }
        public Image SaveThumb(string _fSpec)
        {
            Image retVal = this.GetImageFromFile(_fSpec);
            this.SaveThumb(retVal, this.GetThumbSpec(_fSpec));
            return retVal;
        }
        public void SaveThumb(System.Drawing.Image _image, string _thumbSpec)
        {
            if (System.IO.File.Exists(_thumbSpec))
                System.IO.File.Delete(_thumbSpec);
            else
            {
                string thumbDir = System.IO.Path.GetDirectoryName(_thumbSpec);
                if (!System.IO.Directory.Exists(thumbDir))
                {
                    string[] dirs = thumbDir.Split('\\');
                    string createdDir = String.Empty;
                    foreach (string dir in dirs)
                    {
                        if (String.IsNullOrEmpty(createdDir))
                        {
                            createdDir = dir;
                            continue;
                        }
                        else
                            createdDir += ("\\" + dir);
                        if (!System.IO.Directory.Exists(createdDir))
                            System.IO.Directory.CreateDirectory(createdDir);
                    }
                }
            }
            try
            {
                _image.Save(_thumbSpec);
            }
            catch
            {
                if (System.IO.File.Exists(_thumbSpec))
                    System.IO.File.Delete(_thumbSpec);
                //_image.Save(_thumbSpec);
            }
        }
        public Image GetImageFromFile(string _fSpec)
        {
            Image retVal = null;
            try
            {
                if (!System.IO.File.Exists(_fSpec))
                    return retVal;
                //retVal = Image.FromFile(_fSpec);
                //- w/w nie zwalnia uchwytu do pliku ipotem nie można skasować thumba w danej sesji
                using (System.IO.FileStream fs = new System.IO.FileStream(_fSpec,
                    System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    retVal = Image.FromStream(fs);
                }
                ApplyExifOrientation(retVal);
            }
            catch { }
            return retVal;
        }

        private const int ExifOrientationTagId = 0x0112;
        private const int ExifGpsLatitudeRefTagId = 0x0001;
        private const int ExifGpsLatitudeTagId = 0x0002;
        private const int ExifGpsLongitudeRefTagId = 0x0003;
        private const int ExifGpsLongitudeTagId = 0x0004;

        public static void ApplyExifOrientation(Image _image)
        {
            if (_image == null)
                return;
            try
            {
                if (!_image.PropertyIdList.Contains(ExifOrientationTagId))
                    return;
                int orientation = _image.GetPropertyItem(ExifOrientationTagId).Value[0];
                switch (orientation)
                {
                    case 2: _image.RotateFlip(RotateFlipType.RotateNoneFlipX); break;
                    case 3: _image.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
                    case 4: _image.RotateFlip(RotateFlipType.Rotate180FlipX); break;
                    case 5: _image.RotateFlip(RotateFlipType.Rotate90FlipX); break;
                    case 6: _image.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                    case 7: _image.RotateFlip(RotateFlipType.Rotate270FlipX); break;
                    case 8: _image.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                    default: return;
                }
                _image.RemovePropertyItem(ExifOrientationTagId);
            }
            catch { }
        }

        public bool TryGetGpsCoordinates(string _fSpec, out double _latitude, out double _longitude)
        {
            _latitude = 0.0;
            _longitude = 0.0;
            if (String.IsNullOrEmpty(_fSpec) || !System.IO.File.Exists(_fSpec))
                return false;
            if (this.ComputeContentType(_fSpec) != ContentType.Picture)
                return false;
            try
            {
                using (FileStream fs = new FileStream(_fSpec, FileMode.Open, FileAccess.Read))
                using (Image img = Image.FromStream(fs, false, false))
                {
                    int[] ids = img.PropertyIdList;
                    if (!ids.Contains(ExifGpsLatitudeRefTagId) || !ids.Contains(ExifGpsLatitudeTagId)
                        || !ids.Contains(ExifGpsLongitudeRefTagId) || !ids.Contains(ExifGpsLongitudeTagId))
                        return false;
                    char latRef = (char)img.GetPropertyItem(ExifGpsLatitudeRefTagId).Value[0];
                    char lonRef = (char)img.GetPropertyItem(ExifGpsLongitudeRefTagId).Value[0];
                    double lat = ReadRational3AsDegrees(img.GetPropertyItem(ExifGpsLatitudeTagId).Value);
                    double lon = ReadRational3AsDegrees(img.GetPropertyItem(ExifGpsLongitudeTagId).Value);
                    if (latRef == 'S' || latRef == 's') lat = -lat;
                    if (lonRef == 'W' || lonRef == 'w') lon = -lon;
                    if (lat < -90.0 || lat > 90.0 || lon < -180.0 || lon > 180.0)
                        return false;
                    _latitude = lat;
                    _longitude = lon;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static double ReadRational3AsDegrees(byte[] _bytes)
        {
            if (_bytes == null || _bytes.Length < 24)
                return 0.0;
            double deg = ReadUnsignedRational(_bytes, 0);
            double min = ReadUnsignedRational(_bytes, 8);
            double sec = ReadUnsignedRational(_bytes, 16);
            return deg + (min / 60.0) + (sec / 3600.0);
        }

        private static double ReadUnsignedRational(byte[] _bytes, int _offset)
        {
            uint numerator = BitConverter.ToUInt32(_bytes, _offset);
            uint denominator = BitConverter.ToUInt32(_bytes, _offset + 4);
            if (denominator == 0)
                return 0.0;
            return (double)numerator / (double)denominator;
        }

        public string FileSizeDisplay(string _fSpec)
        {
            double fSize = 0;
            string unit = String.Empty;
            try
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(_fSpec);
                fSize = fi.Length;
                if (fSize < 1024)
                    unit = "B";
                else if (fSize < 1024 * 1024)
                {
                    fSize /= 1024.00;
                    unit = "KB";
                }
                else
                {
                    fSize /= (1024.00 * 1024.00);
                    unit = "MB";
                }
            }
            catch { }
            return (fSize.ToString("#.##") + unit);
        }

        public bool IsShortcut(string _fSpec)
        {
            return (System.IO.Path.GetExtension(_fSpec).ToUpper() == ".LNK");
        }
        public string MakeShortcutFileSpec(string _fSpec)
        {
            return _fSpec + ".lnk";
        }
        public string ShortcutTargetFile(string _shortcutSpec)
        {
            WshShellClass shell = new WshShellClass();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(_shortcutSpec);
            return shortcut.TargetPath;
        }
        public void CreateShortcut(string _shortcutSpec, string _targetFileSpec)
        {
            WshShellClass shell = new WshShellClass();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(_shortcutSpec);
            shortcut.TargetPath = _targetFileSpec;
            shortcut.Save();
        }
        public void ChangeShortcutTargetFile(string _shortcutSpec, string _targetFileSpec)
        {
            WshShellClass shell = new WshShellClass();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(_shortcutSpec);
            shortcut.TargetPath = _targetFileSpec;
            shortcut.Save();
            return;
        }

        public void SetItemQuality(ref Item _item, object _itemQuality)
        {
            if (_item == null || _itemQuality == null)
                return;
            _item.Quality = this.GetItemQuality(_itemQuality);
        }
        public ItemQuality GetItemQuality(object _itemQuality)
        {
            ItemQuality retVal = ItemQuality.Normal;
            string itemQualityStr = Convert.ToString(_itemQuality);
            if (String.IsNullOrEmpty(itemQualityStr))
                return retVal;
            switch (itemQualityStr)
            {
                case "Low":
                    {
                        retVal = ItemQuality.Low;
                        break;
                    }
                case "Normal":
                    {
                        retVal = ItemQuality.Normal;
                        break;
                    }
                case "Good":
                    {
                        retVal = ItemQuality.Good;
                        break;
                    }
                case "Best":
                    {
                        retVal = ItemQuality.Best;
                        break;
                    }
                case "Extra":
                    {
                        retVal = ItemQuality.Extra;
                        break;
                    }
            }
            return retVal;
        }
        public string ItemQualityDescription(ItemQuality _iq, bool _shortForm, bool _withBrackets)
        {
            string retVal = String.Empty;
            switch (_iq)
            {
                case ItemQuality.Low:
                    {
                        retVal = (_shortForm ? "L" : "Low");
                        break;
                    }
                case ItemQuality.Normal:
                    {
                        retVal = (_shortForm ? "N" : "Normal");
                        break;
                    }
                case ItemQuality.Good:
                    {
                        retVal = (_shortForm ? "G" : "Good");
                        break;
                    }
                case ItemQuality.Best:
                    {
                        retVal = (_shortForm ? "B" : "Best");
                        break;
                    }
                case ItemQuality.Extra:
                    {
                        retVal = (_shortForm ? "E" : "Extra");
                        break;
                    }
            }
            if (_withBrackets && !String.IsNullOrEmpty(retVal))
                retVal = "(" + retVal + ")";
            return retVal;
        }
        public string ItemStateSign(Item _item, bool _shortForm)
        {
            string retVal = String.Empty;
            if (_item.IsHidden)
                retVal += "H";
            if (_item.IsImportant)
                retVal += "!";
            retVal += ItemQualityDescription(_item.Quality, true, false);
            if (_item.IsArt)
                retVal += "*";
            if (!String.IsNullOrEmpty(retVal))
                retVal = "(" + retVal + ")";
            return retVal;
        }

        public List<Item> GetFileSpecItems(string _fSpec, string _connectString, Item _currentItem)
        {
            List<Item>  retVal = new List<Item>();
            SqlUtils su = new SqlUtils(_connectString);
            string sqlQuery = "select * from dbo.Items " +
                              "inner join dbo.Views on V_id = I_VID " +
                              "inner join dbo.ViewsLinks on VL_VID = V_id " +
                              "inner join dbo.albums on a_id = VL_ParentAID " +
                              "where I_filespec = '" + _fSpec + "' " +
                              "order by a_Name, VL_Name";
            SqlDataReader rd = su.GetSqlReader(sqlQuery);
            while (rd.Read())
            {
                int aID = Convert.ToInt32(rd["A_ID"]);
                Album a = new Album(aID, _connectString);
                a.LoadFromReader(rd, 0);
                int vID = Convert.ToInt32(rd["V_ID"]);
                View v = new View(vID, _connectString);
                v.Name = Convert.ToString(rd["V_Name"]);
                int lID = Convert.ToInt32(rd["VL_ID"]);
                ViewLink l = new ViewLink(a, v, _connectString);                
                l.LoadFromReader(rd, 0, false);
                l.ID = lID;
                int iID = Convert.ToInt32(rd["I_ID"]);
                Item i = new Item(v, iID, _connectString);
                i.LoadFromReader(rd);
                ItemQuality iq = i.Quality;
                bool isCurrent = false;
                if(_currentItem != null)
                    isCurrent = (i.ParentView.ID == _currentItem.ParentView.ID);
                i.CurrentNotes = (isCurrent ? " -> " : "     ") + this.ItemStateSign(i, true) + l.ParentAlbum.Name + "\\" + l.ViewPath;
                retVal.Add(i);
            }
            rd.Close();
            su.Close();
            return retVal;
        }
    }
}

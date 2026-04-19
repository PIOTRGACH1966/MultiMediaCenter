using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MultiMediaCenter
{
    public struct PlayableObject
    {
        public string fSpec;
        public ItemQuality Quality;

        public PlayableObject(string _fSpec, ItemQuality _quality)
        {
            this.fSpec = _fSpec;
            this.Quality = _quality;
        }
    }
}

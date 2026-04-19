using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiMediaCenter
{
    public enum ContentType
    {
        Unknown = 0,
        Text = 1,
        Picture = 2,
        Audio = 3,
        Video = 4
    }

    public enum ItemQuality
    {
        Default = -2,
        Low = -1,
        Normal = 0,
        Good = 1,
        Best = 2,
        Extra = 3
    }

    public enum InsertMode
    {
        ByName = 0,
        OnFirst = 1,
        OnSelected = 2,
        OnLast = 3
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Peasmod
{
    class InspectorMode
    {
        public static PlayerControl Inspector1;
        public static PlayerControl Inspector2;

        public static Color InspectorColor { get; } = new Color(58f / 255f, 255f / 255f, 127f / 255f);
    }
}

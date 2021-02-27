using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Peasmod.GameModes
{
    class InspectorMode
    {
        public static List<PlayerControl> Inspectors = new List<PlayerControl>();

        public static Color InspectorColor { get; } = new Color(58f / 255f, 255f / 255f, 127f / 255f);
    }
}

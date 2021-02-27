using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Hazel;
using Reactor.Extensions;

namespace Peasmod.GameModes
{
    class MayorMode
    {
        public static List<PlayerControl> Mayors = new List<PlayerControl>();

        public static Color MayorColor { get; } = new Color(17f / 255f, 49f / 255f, 255f / 255f);
    }
}

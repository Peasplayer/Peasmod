using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Peasmod.GameModes
{
    class JesterMode
    {
        public static List<PlayerControl> Jesters = new List<PlayerControl>();

        public static bool JesterWon = false;
        public static PlayerControl Winner;

        public static float MaxJesters = 0;
        public static Color JesterColor { get; } = new Color(136f / 256f, 31f / 255f, 136f / 255f);

        public static void HandleTasks()
        {
            
            foreach(var jester in Jesters)
            {
                var removeTask = new List<PlayerTask>();
                foreach (PlayerTask task in jester.myTasks)
                    if (task.TaskType != TaskTypes.FixComms && task.TaskType != TaskTypes.FixLights && task.TaskType != TaskTypes.ResetReactor && task.TaskType != TaskTypes.ResetSeismic && task.TaskType != TaskTypes.RestoreOxy)
                        removeTask.Add(task);
                foreach (PlayerTask task in removeTask)
                    jester.RemoveTask(task);
            }
        }
    }
}

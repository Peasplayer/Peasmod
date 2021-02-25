using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Peasmod
{
    class JesterMode
    {
        public static PlayerControl Jester1;
        public static PlayerControl Jester2;

        public static bool JesterWon = false;
        public static PlayerControl Winner;

        public static float MaxJesters = 0;
        public static Color JesterColor { get; } = new Color(136f / 256f, 31f / 255f, 136f / 255f);

        public static void HandleTasks()
        {
            var removeTask = new List<PlayerTask>();
            foreach (PlayerTask task in Jester1.myTasks)
                if (task.TaskType != TaskTypes.FixComms && task.TaskType != TaskTypes.FixLights && task.TaskType != TaskTypes.ResetReactor && task.TaskType != TaskTypes.ResetSeismic && task.TaskType != TaskTypes.RestoreOxy)
                    removeTask.Add(task);
            foreach (PlayerTask task in removeTask)
                Jester1.RemoveTask(task);
            removeTask.Clear();
            if (Jester2 != null)
            {
                foreach (PlayerTask task in Jester2.myTasks)
                    if (task.TaskType != TaskTypes.FixComms && task.TaskType != TaskTypes.FixLights && task.TaskType != TaskTypes.ResetReactor && task.TaskType != TaskTypes.ResetSeismic && task.TaskType != TaskTypes.RestoreOxy)
                        removeTask.Add(task);
                foreach (PlayerTask task in removeTask)
                    Jester2.RemoveTask(task);
            }
        }
    }
}

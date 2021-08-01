using System;
using System.Reflection;
using BepInEx.IL2CPP;
using HarmonyLib;
using PeasAPI.Roles;

namespace Peasmod.ApiExtension.Gamemodes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterCustomGameModeAttribute : Attribute
    {
        public static void Register(BasePlugin plugin)
        {
            Register(Assembly.GetCallingAssembly(), plugin);
        }

        public static void Register(Assembly assembly, BasePlugin plugin)
        {
            foreach (var type in assembly.GetTypes())
            {
                var attribute = type.GetCustomAttribute<RegisterCustomGameModeAttribute>(); 

                if (attribute != null)
                {
                    if (!type.IsSubclassOf(typeof(GameMode)))
                    {
                        throw new InvalidOperationException($"Type {type.FullDescription()} must extend {nameof(GameMode)}.");
                    }
                    
                    Activator.CreateInstance(type, plugin);
                }
            }
        }
    }
}
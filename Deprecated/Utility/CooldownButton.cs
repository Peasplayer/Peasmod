﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnhollowerBaseLib;
using Reactor.Extensions;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Reactor.Unstrip;

namespace Peasmod.Utility
{
    public class CooldownButton
    {
        public static List<CooldownButton> buttons = new List<CooldownButton>();
        public KillButtonManager killButtonManager;
        private Color startColorButton = new Color(255, 255, 255);
        private Color startColorText = new Color(255, 255, 255);
        public Vector2 PositionOffset = Vector2.zero;
        public float MaxTimer = 0f;
        public float Timer = 0f;
        public float EffectDuration = 0f;
        public bool isEffectActive;
        public bool hasEffectDuration;
        public bool enabled = true;
        public bool Visibile = true;
        public Category category;
        private Sprite ButtonSprite;
        public Action OnClick;
        private Action OnEffectEnd;
        private HudManager hudManager;
        private float pixelsPerUnit;
        private bool canUse;
        public PassiveButton button;

        public CooldownButton(Action OnClick, float Cooldown, string Image, float PixelsPerUnit, Vector2 PositionOffset, Category category, HudManager hudManager, float EffectDuration, Action OnEffectEnd)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.OnEffectEnd = OnEffectEnd;
            this.PositionOffset = PositionOffset;
            this.EffectDuration = EffectDuration;
            this.category = category;
            pixelsPerUnit = PixelsPerUnit;
            MaxTimer = Cooldown;
            Timer = MaxTimer;
            Texture2D tex = GUIExtensions.CreateEmptyTexture();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream(Image);
            byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
            ImageConversion.LoadImage(tex, buttonTexture, false);
            ButtonSprite = GUIExtensions.CreateSprite(tex);
            hasEffectDuration = true;
            isEffectActive = false;
            buttons.Add(this);
            Start();
        }

        public CooldownButton(Action OnClick, float Cooldown, string Image, float pixelsPerUnit, Vector2 PositionOffset, Category category, HudManager hudManager)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.pixelsPerUnit = pixelsPerUnit;
            this.PositionOffset = PositionOffset;
            this.category = category;
            MaxTimer = Cooldown;
            Timer = MaxTimer; 
            Texture2D tex = GUIExtensions.CreateEmptyTexture();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream(Image);
            byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
            ImageConversion.LoadImage(tex, buttonTexture, false);
            ButtonSprite = GUIExtensions.CreateSprite(tex);
            hasEffectDuration = false;
            buttons.Add(this);
            Start();
        }

        private void Start()
        {
            killButtonManager = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.transform);
            startColorButton = killButtonManager.renderer.color;
            startColorText = killButtonManager.TimerText.color;
            killButtonManager.gameObject.SetActive(true);
            killButtonManager.renderer.enabled = true;
            killButtonManager.renderer.sprite = ButtonSprite;
            button = killButtonManager.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction)listener);
            void listener()
            {
                if (Timer <= 0f && canUse && enabled && killButtonManager.gameObject.active && PlayerControl.LocalPlayer.moveable)
                {
                    killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                    OnClick();
                    Timer = MaxTimer;
                    if (hasEffectDuration)
                    {
                        isEffectActive = true;
                        Timer = EffectDuration;
                        killButtonManager.TimerText.color = new Color(0, 255, 0);
                    }
                }
            }
        }
        public bool CanUse()
        {
            if (PlayerControl.LocalPlayer == null) return false;
            if (PlayerControl.LocalPlayer.Data == null) return false;
            switch (category)
            {
                case Category.Everyone:
                    {
                        canUse = !PlayerControl.LocalPlayer.Data.IsDead;
                        break;
                    }
                case Category.OnlyCrewmate:
                    {
                        canUse = !PlayerControl.LocalPlayer.Data.IsImpostor && !PlayerControl.LocalPlayer.Data.IsDead;
                        break;
                    }
                case Category.OnlyImpostor:
                    {
                        canUse = PlayerControl.LocalPlayer.Data.IsImpostor && !PlayerControl.LocalPlayer.Data.IsDead;
                        break;
                    }
                case Category.OnlyDoctor:
                    {
                        canUse = PlayerControl.LocalPlayer.IsRole(Role.Doctor) && !PlayerControl.LocalPlayer.Data.IsDead;
                        break;
                    }
                case Category.OnlySheriff:
                    {
                        canUse = PlayerControl.LocalPlayer.IsRole(Role.Sheriff) && !PlayerControl.LocalPlayer.Data.IsDead;
                        break;
                    }
                case Category.OnlyThanos:
                    {
                        canUse = PlayerControl.LocalPlayer.IsRole(Role.Thanos) && !PlayerControl.LocalPlayer.Data.IsDead;
                        break;
                    }
            }
            return true;
        }
        public static void HudUpdate()
        {
            buttons.RemoveAll(item => item.killButtonManager == null);
            for (int i = 0; i < buttons.Count; i++)
            {
                
                buttons[i].killButtonManager.renderer.sprite = buttons[i].ButtonSprite;
                buttons[i].killButtonManager.gameObject.SetActive(buttons[i].Visibile);
                buttons[i].killButtonManager.renderer.enabled = buttons[i].Visibile;
                buttons[i].killButtonManager.enabled = buttons[i].Visibile;
                buttons[i].killButtonManager.gameObject.active = buttons[i].Visibile;
                if (buttons[i].CanUse() && buttons[i].Visibile)
                    buttons[i].Update();
            }
        }
        private void Update()
        {
            if (killButtonManager.transform.localPosition.x > 0f)
                killButtonManager.transform.localPosition = new Vector3((killButtonManager.transform.localPosition.x + 1.3f) * -1, killButtonManager.transform.localPosition.y, killButtonManager.transform.localPosition.z) + new Vector3(PositionOffset.x, PositionOffset.y);
            if (Timer < 0f && PlayerControl.LocalPlayer.moveable)
            {
                killButtonManager.renderer.color = new Color(1f, 1f, 1f, 1f);
                if (isEffectActive)
                {
                    killButtonManager.TimerText.color = startColorText;
                    Timer = MaxTimer;
                    isEffectActive = false;
                    OnEffectEnd();
                }
            }
            else
            {
                if (canUse)
                    Timer -= Time.deltaTime;
                killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
            }
            killButtonManager.gameObject.SetActive(canUse);
            killButtonManager.renderer.enabled = canUse;
            if (canUse)
            {
                killButtonManager.renderer.material.SetFloat("_Desat", 0f);
                killButtonManager.SetCoolDown(Timer, MaxTimer);
            }
        }
        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
        public static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            if (iCall_LoadImage == null)
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");

            var il2cppArray = (Il2CppStructArray<byte>)data;

            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }

        public void SetTexture(String resource)
        {
            Texture2D tex = GUIExtensions.CreateEmptyTexture();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream(resource);
            byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
            ImageConversion.LoadImage(tex, buttonTexture, false);
            ButtonSprite = GUIExtensions.CreateSprite(tex);
        }
        public enum Category
        {
            Everyone,
            OnlyCrewmate,
            OnlyImpostor,
            OnlyDoctor,
            OnlySheriff,
            OnlyThanos
        }
    }
}

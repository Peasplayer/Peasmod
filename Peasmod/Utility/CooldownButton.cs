using System;
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

namespace Peasmod
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
        public Category category;
        private string ResourceName;
        public Action OnClick;
        private Action OnEffectEnd;
        private HudManager hudManager;
        private float pixelsPerUnit;
        private bool canUse;
        public PassiveButton button;

        public CooldownButton(Action OnClick, float Cooldown, string ImageEmbededResourcePath, float PixelsPerUnit, Vector2 PositionOffset, Category category, HudManager hudManager, float EffectDuration, Action OnEffectEnd)
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
            ResourceName = ImageEmbededResourcePath;
            hasEffectDuration = true;
            isEffectActive = false;
            buttons.Add(this);
            Start();
        }

        public CooldownButton(Action OnClick, float Cooldown, string ImageEmbededResourcePath, float pixelsPerUnit, Vector2 PositionOffset, Category category, HudManager hudManager)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.pixelsPerUnit = pixelsPerUnit;
            this.PositionOffset = PositionOffset;
            this.category = category;
            MaxTimer = Cooldown;
            Timer = MaxTimer;
            ResourceName = ImageEmbededResourcePath;
            hasEffectDuration = false;
            buttons.Add(this);
            Start();
        }

        private void Start()
        {
            killButtonManager = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.transform);
            startColorButton = killButtonManager.renderer.color;
            startColorText = killButtonManager.TimerText.Color;
            killButtonManager.gameObject.SetActive(true);
            killButtonManager.renderer.enabled = true;
            Texture2D tex = GUIExtensions.CreateEmptyTexture();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream(ResourceName);
            byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
            ImageConversion.LoadImage(tex, buttonTexture, false);
            var test = Utils.CreateSprite(ResourceName);
            killButtonManager.renderer.sprite = GUIExtensions.CreateSprite(tex);
            //killButtonManager.renderer.size = test.GetComponent<SpriteRenderer>().size;
            killButtonManager.transform.localScale = test.GetComponent<SpriteRenderer>().transform.localScale;
            test.Destroy();
            //Utils.Log(killButtonManager.renderer.size.x + " | " + killButtonManager.renderer.size.y);
            button = killButtonManager.GetComponent<PassiveButton>();
            foreach(var collider in button.Colliders)
            {
                collider.transform.localScale = killButtonManager.transform.localScale;
            }
            button.transform.localScale = killButtonManager.transform.localScale;
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
                        killButtonManager.TimerText.Color = new Color(0, 255, 0);
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
                        canUse = true;
                        break;
                    }
                case Category.OnlyCrewmate:
                    {
                        canUse = !PlayerControl.LocalPlayer.Data.IsImpostor;
                        break;
                    }
                case Category.OnlyImpostor:
                    {
                        canUse = PlayerControl.LocalPlayer.Data.IsImpostor;
                        break;
                    }
                case Category.Doctor:
                    {
                        if (DoctorMode.Doctor1 != null)
                            if(DoctorMode.Doctor1.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                canUse = !PlayerControl.LocalPlayer.Data.IsDead;
                            else if (DoctorMode.Doctor2 != null)
                                if(DoctorMode.Doctor2.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                    canUse = !PlayerControl.LocalPlayer.Data.IsDead;
                                else
                                    canUse = false;
                            else
                                canUse = false;
                        break;
                    }
                case Category.Sheriff:
                    {
                        if (SheriffMode.Sheriff1 != null)
                            if (SheriffMode.Sheriff1.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                canUse = !PlayerControl.LocalPlayer.Data.IsDead;
                            else if (SheriffMode.Sheriff2 != null)
                                if (SheriffMode.Sheriff2.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                    canUse = !PlayerControl.LocalPlayer.Data.IsDead;
                                else
                                    canUse = false;
                            else
                                canUse = false;
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
                if (buttons[i].CanUse())
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
                    killButtonManager.TimerText.Color = startColorText;
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
            ResourceName = resource;
            Texture2D tex = GUIExtensions.CreateEmptyTexture();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream(ResourceName);
            byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
            ImageConversion.LoadImage(tex, buttonTexture, false);
            killButtonManager.renderer.sprite = GUIExtensions.CreateSprite(tex);
        }
        public enum Category
        {
            Everyone,
            OnlyCrewmate,
            OnlyImpostor,
            Doctor,
            Sheriff
        }
    }
}

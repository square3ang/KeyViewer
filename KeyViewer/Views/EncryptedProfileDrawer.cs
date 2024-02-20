﻿using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using System.Threading.Tasks;
using UnityEngine;
using TEP = KeyViewer.Core.Translation.TranslationKeys.EncryptedProfile;

namespace KeyViewer.Views
{
    public class EncryptedProfileDrawer : ModelDrawable<EncryptedProfile>
    {
        private byte[] encProfile;
        private Metadata meta;
        private string key;
        private bool tryDecrypting;
        private string resultMessage;
        public EncryptedProfileDrawer(byte[] encProfile) : base(null, L(TEP.Prefix))
        {
            this.encProfile = encProfile;
            Open().Await();
        }
        public override void Draw()
        {
            if (encProfile == null)
            {
                Drawer.ButtonLabel(L(TEP.Damaged), KeyViewerUtils.OpenDiscordUrl);
                return;
            }
            if (meta == null)
            {
                Drawer.ButtonLabel(L(TEP.Opening), KeyViewerUtils.OpenDiscordUrl);
                return;
            }
            GUILayout.BeginHorizontal();
            Drawer.ButtonLabel(L(TEP.Name) + ": ", KeyViewerUtils.OpenDiscordUrl);
            Drawer.ButtonLabel(meta.Name, KeyViewerUtils.OpenDiscordUrl);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Drawer.ButtonLabel(L(TEP.Author) + ": ", KeyViewerUtils.OpenDiscordUrl);
            Drawer.ButtonLabel(meta.Author, KeyViewerUtils.OpenDiscordUrl);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Drawer.ButtonLabel(L(TEP.Description) + ": ", KeyViewerUtils.OpenDiscordUrl);
            Drawer.ButtonLabel(meta.Description, KeyViewerUtils.OpenDiscordUrl);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Drawer.ButtonLabel(L(TEP.CreationTime) + ": ", KeyViewerUtils.OpenDiscordUrl);
            var d = meta.CreationTime;
            Drawer.ButtonLabel($"{d.Year}/{d.Month}/{d.Day} {d.Hour}:{d.Minute}:{d.Second}", KeyViewerUtils.OpenDiscordUrl);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (tryDecrypting)
                    Drawer.ButtonLabel(L(TEP.Decrypting), KeyViewerUtils.OpenDiscordUrl);
                else
                {
                    Drawer.ButtonLabel(L(TEP.Key), KeyViewerUtils.OpenDiscordUrl);
                    key = GUILayout.TextField(key);
                    if (GUILayout.Button(L(TEP.Import)))
                    {
                        tryDecrypting = true;
                        Decrypt().Await();
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(resultMessage))
                Drawer.ButtonLabel(resultMessage, KeyViewerUtils.OpenDiscordUrl);
        }
        private async Task Open()
        {
            model = await KeyViewerWebAPI.OpenEncryptedProfile(encProfile);
            meta = model?.Metadata;
            if (meta == null) encProfile = null;
        }
        private async Task Decrypt()
        {
            var profile = await KeyViewerWebAPI.DecryptProfile(model.RawProfile, key);
            if (profile == null) resultMessage = L(TEP.KeyMismatch);
            else
            {
                StaticCoroutine.Queue(StaticCoroutine.SyncRunner(() => Main.AddManagerImmediate(meta.Name, profile, key)));
                resultMessage = L(TEP.Success);
            }
            tryDecrypting = false;
        }
    }
}

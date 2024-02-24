using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Utils;
using SFB;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using TKEP = KeyViewer.Core.Translation.TranslationKeys.EncryptedProfile;

namespace KeyViewer.Views
{
    public class EncryptedProfileSaveDrawer : ModelDrawable<EncryptedProfile>
    {
        private Profile profile;
        private string key;
        private bool tryEncrypting;
        private string resultMessage;
        private byte[] encProfile;
        public EncryptedProfileSaveDrawer(Profile profile) : base(new EncryptedProfile(), L(TKEP.Prefix))
        {
            this.profile = profile;
            model.Metadata = new Metadata();
        }
        public override void Draw()
        {
            Drawer.DrawString(L(TKEP.Name), ref model.Metadata.Name);
            Drawer.DrawString(L(TKEP.Author), ref model.Metadata.Author);
            Drawer.DrawString(L(TKEP.Description), ref model.Metadata.Description);

            GUILayout.BeginHorizontal();
            {
                if (tryEncrypting)
                    Drawer.ButtonLabel(L(TKEP.Encrypting), KeyViewerUtils.OpenDiscordUrl);
                else
                {
                    if (encProfile != null)
                    {
                        Drawer.ButtonLabel(L(TKEP.Key) + key, KeyViewerUtils.OpenDiscordUrl);
                        if (GUILayout.Button(L(TKEP.Save)))
                        {
                            var path = StandaloneFileBrowser.SaveFilePanel(L(TKEP.Prefix), Main.Mod.Path, model.Metadata.Name + ".encryptedProfile", "encryptedProfile");
                            if (!string.IsNullOrWhiteSpace(path)) File.WriteAllBytes(path, encProfile);
                        }
                    }
                    else
                    {
                        Drawer.ButtonLabel(L(TKEP.Key), KeyViewerUtils.OpenDiscordUrl);
                        key = GUILayout.TextField(key);
                        if (GUILayout.Button(L(TKEP.Encrypt)) &&
                            !string.IsNullOrWhiteSpace(model.Metadata.Name) &&
                            !string.IsNullOrWhiteSpace(model.Metadata.Author) &&
                            !string.IsNullOrWhiteSpace(key))
                        {
                            tryEncrypting = true;
                            Encrypt().Await();
                        }
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(resultMessage))
                Drawer.ButtonLabel(resultMessage, KeyViewerUtils.OpenDiscordUrl);
        }
        private async Task Encrypt()
        {
            encProfile = await KeyViewerWebAPI.EncryptProfile(profile, model.Metadata, key);
            if (encProfile == null) resultMessage = L(TKEP.InternalError);
            else resultMessage = L(TKEP.Success);
            tryEncrypting = false;
        }
    }
}

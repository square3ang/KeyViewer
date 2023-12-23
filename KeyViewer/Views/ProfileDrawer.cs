using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Unity;

namespace KeyViewer.Views
{
    public class ProfileDrawer : ModelDrawable
    {
        public Profile profile;
        public ProfileDrawer(KeyManager manager, Profile profile) : base(manager)
        {
            this.profile = profile;
        }
        public override void Draw(IDrawer drawer)
        {
            
        }
    }
}

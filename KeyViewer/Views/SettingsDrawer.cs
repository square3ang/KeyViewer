using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyViewer.Views
{
    public class SettingsDrawer : ModelDrawable<Settings>
    {
        public Settings settings;
        public SettingsDrawer(Settings settings)
        {
            this.settings = settings;
        }
        public override void Draw(IDrawer drawer)
        {

        }
    }
}

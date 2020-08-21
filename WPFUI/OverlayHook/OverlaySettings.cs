using System;
using System.Collections.Generic;
using System.Text;

namespace WPFUI.OverlayHook
{
    // Example settings
    public class DemoOverlaySettings
    {
        // 60 frames/sec roughly
        public int UpdateRate { get; set; }

        public string Author { get; set; }
        public string Description { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
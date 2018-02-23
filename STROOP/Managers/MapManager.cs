﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STROOP.Structs;
using STROOP.Utilities;
using System.Windows.Forms;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using STROOP.Structs.Configurations;
using STROOP.Controls.Map.Graphics;

namespace STROOP.Managers
{
    public class MapManager
    {        
        public bool IsLoaded { get; private set; }
        public bool Visible { get => _mapGraphics.Control.Visible; set => _mapGraphics.Control.Visible = value; }

        MapGui _mapGui;
        MapGraphics _mapGraphics;

        public MapManager(MapAssociations mapAssoc, MapGui mapGui)
        {
            _mapGui = mapGui;
        }

        public void Load()
        {
            // Create new graphics control
            _mapGraphics = new MapGraphics(_mapGui.GLControl);
            _mapGraphics.Load();

            IsLoaded = true;
        }

        public void Update()
        {
            // Make sure the control has successfully loaded
            if (!IsLoaded)
                return;
            
            // Update gui by drawing images (invokes _mapGraphics.OnPaint())
            _mapGraphics.Control.Invalidate();
        }
    }
}
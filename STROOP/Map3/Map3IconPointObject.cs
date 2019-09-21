﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using STROOP.Controls.Map;
using OpenTK.Graphics.OpenGL;
using STROOP.Utilities;
using STROOP.Structs.Configurations;
using STROOP.Structs;
using OpenTK;

namespace STROOP.Map3
{
    public abstract class Map3IconPointObject : Map3IconObject
    {
        public Map3IconPointObject(Map3Graphics graphics, Func<Image> imageFunction)
            : base(graphics, imageFunction)
        {
        }

        public override void DrawOnControl()
        {
            UpdateImage();

            // Update map object
            (double x, double y, double z, double angle) = GetPositionAngle();
            float relX = (float)PuUtilities.GetRelativeCoordinate(x);
            float relY = (float)PuUtilities.GetRelativeCoordinate(y);
            float relZ = (float)PuUtilities.GetRelativeCoordinate(z);
            float angleDegrees = (float)MoreMath.AngleUnitsToDegrees(angle);

            // Additional stats
            int iconSize = 50;
            float alpha = 1;

            float xOffsetInGameUnits = relX - Graphics.XMin;
            float xOffsetPixels = xOffsetInGameUnits * Graphics.ConversionScale;
            float xPosPixels = Graphics.MapView.X + xOffsetPixels;

            float zOffsetInGameUnits = relZ - Graphics.ZMin;
            float zOffsetPixels = zOffsetInGameUnits * Graphics.ConversionScale;
            float zPosPixels = Graphics.MapView.Y + zOffsetPixels;

            SizeF size = Graphics.ScaleImageSize(Image.Size, iconSize);

            DrawTexture(TextureId, new PointF(xPosPixels, zPosPixels), size, angleDegrees, alpha);
        }

        protected abstract (double x, double y, double z, double angle) GetPositionAngle();
    }
}

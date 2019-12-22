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
using System.Drawing.Imaging;
using STROOP.Models;

namespace STROOP.Map3
{
    public class Map3CustomCeilingObject : Map3HorizontalTriangleObject
    {
        private readonly List<uint> _triAddressList;

        public Map3CustomCeilingObject(List<uint> triAddressList)
            : base()
        {
            _triAddressList = triAddressList;

            Opacity = 0.5;
            Color = Color.Red;
        }

        public static Map3CustomCeilingObject Create(string text)
        {
            if (text == null) return null;
            List<uint?> nullableUIntList = ParsingUtilities.ParseStringList(text)
                .ConvertAll(word => ParsingUtilities.ParseHexNullable(word));
            if (nullableUIntList.Any(nullableUInt => !nullableUInt.HasValue))
            {
                return null;
            }
            List<uint> uintList = nullableUIntList.ConvertAll(nullableUInt => nullableUInt.Value);
            return new Map3CustomCeilingObject(uintList);
        }

        protected override List<List<(float x, float y, float z)>> GetVertexLists()
        {
            return Map3Utilities.GetTriangleVertexLists(_triAddressList);
        }

        public override string GetName()
        {
            return "Custom Ceiling Tris";
        }

        public override Image GetImage()
        {
            return Config.ObjectAssociations.TriangleCeilingImage;
        }
    }
}

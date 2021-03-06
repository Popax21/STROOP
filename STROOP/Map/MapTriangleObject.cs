﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using STROOP.Utilities;
using STROOP.Structs.Configurations;
using STROOP.Structs;
using OpenTK;
using System.Drawing.Imaging;
using STROOP.Models;

namespace STROOP.Map
{
    public abstract class MapTriangleObject : MapObject
    {
        public MapTriangleObject()
            : base()
        {
        }

        protected List<List<(float x, float y, float z)>> GetVertexLists()
        {
            return GetTriangles().ConvertAll(tri => tri.Get3DVertices());
        }

        protected abstract List<TriangleDataModel> GetTriangles();

        protected static (float x, float y, float z) OffsetVertex(
            (float x, float y, float z) vertex, float xOffset, float yOffset, float zOffset)
        {
            return (vertex.x + xOffset, vertex.y + yOffset, vertex.z + zOffset);
        }

        public override MapDrawType GetDrawType()
        {
            return MapDrawType.Perspective;
        }
    }
}

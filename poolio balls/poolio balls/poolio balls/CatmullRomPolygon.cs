using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace poolio_balls
{
    public class CatmullRomPolygon : Polygon
    {
        public CatmullRomPolygon(float curviness, params Vector2[] vertices)
            //: base(Geometry.CreateCatmullBlob(curviness, vertices))
            : base(Geometry.CreateCatmullBlob(curviness, vertices))
        {
        }
    }
}

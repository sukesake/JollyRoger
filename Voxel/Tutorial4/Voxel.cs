﻿using System;
using SharpDX;
using SharpHelper;

namespace Tutorial4
{
    class Voxel
    {
        public SharpMesh Mesh { get; private set; }

        public Voxel(SharpDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            var indices = new int[] 
                { 
                    0,1,2,0,2,3,
                    4,6,5,4,7,6,
                    8,9,10,8,10,11,
                    12,14,13,12,15,14,
                    16,18,17,16,19,18,
                    20,21,22,20,22,23
                };

            float length = 1.0f;
            float height = 1.0f;
            float width = 1.0f;

            var vertices = new[] 
                {
                    new TexturedVertex(new Vector3(-length, height, width),new Vector2(1,1)),
                    new TexturedVertex(new Vector3( length, height, width),new Vector2(0,1)),
                    new TexturedVertex(new Vector3( length, height,-width),new Vector2(0,0)),
                    new TexturedVertex(new Vector3(-length, height,-width),new Vector2(1,0)),
               
                    new TexturedVertex(new Vector3(-length,-height ,width),new Vector2(1,1)),
                    new TexturedVertex(new Vector3( length,-height ,width),new Vector2(0,1)),
                    new TexturedVertex(new Vector3( length,-height,-width),new Vector2(0,0)),
                    new TexturedVertex(new Vector3(-length,-height,-width),new Vector2(1,0)),
               
                    new TexturedVertex(new Vector3(-length,-height, width),new Vector2(0,1)),
                    new TexturedVertex(new Vector3(-length, height, width),new Vector2(0,0)),
                    new TexturedVertex(new Vector3(-length, height,-width),new Vector2(1,0)),
                    new TexturedVertex(new Vector3(-length,-height,-width),new Vector2(1,1)),
               
                    new TexturedVertex(new Vector3( length,-height, width),new Vector2(1,1)),
                    new TexturedVertex(new Vector3( length, height, width),new Vector2(1,0)),
                    new TexturedVertex(new Vector3( length, height,-width),new Vector2(0,0)),
                    new TexturedVertex(new Vector3( length,-height,-width),new Vector2(0,1)),
              
                    new TexturedVertex(new Vector3(-length, height, width),new Vector2(1,0)),
                    new TexturedVertex(new Vector3( length, height, width),new Vector2(0,0)),
                    new TexturedVertex(new Vector3( length,-height, width),new Vector2(0,1)),
                    new TexturedVertex(new Vector3(-length,-height, width),new Vector2(1,1)),
               
                    new TexturedVertex(new Vector3(-length, height,-width),new Vector2(0,0)),
                    new TexturedVertex(new Vector3( length, height,-width),new Vector2(1,0)),
                    new TexturedVertex(new Vector3( length,-height,-width),new Vector2(1,1)),
                    new TexturedVertex(new Vector3(-length,-height,-width),new Vector2(0,1))
                };

            Mesh = SharpMesh.Create<TexturedVertex>(device, vertices, indices);
        }
    }
}

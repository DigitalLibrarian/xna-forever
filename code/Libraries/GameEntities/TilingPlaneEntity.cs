using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Forever.Physics;
using Forever.Physics.Collide;
using Forever.Interface;
using Forever.Render;

namespace Forever.GameEntities
{
    public class TilingPlaneEntity : ICollideable, IHasRigidBody, IRenderable, IGameEntity
    {
        IGeometryData geometryData = null;
        public IGeometryData GeometryData { get { return geometryData; } }

        IRigidBody body = null;
        public IRigidBody Body { get { return body; } }

        public bool Awake { get { return false; } }

        #region Rendering Properties
        private Texture2D Texture { get; set; }

        VertexPositionNormalTexture[] Vertices = new VertexPositionNormalTexture[4];
        short[] Indices = new short[6];

        #endregion
        public TilingPlaneEntity(IRigidBody body, IGeometryData geometryData)
        {
            if (geometryData.Prim.PrimType != CollideType.Plane) {throw new InvalidOperationException("You may not use this class with a CollideType other than CollideType.Plane");}

            this.body = body;
            this.geometryData = geometryData;
            //this.Texture = texture;


        }



        public void LoadContent(ContentManager content) 
        {
            this.Texture = content.Load<Texture2D>("tile02");
         

            Forever.Physics.Collide.Plane plane = this.GeometryData.Prim as Forever.Physics.Collide.Plane;
            Vector3 origin = Vector3.Zero ;

            Vector3 normal = plane.Normal;

            float height = 1f;
            float width = 1f;

            // Calculate the quad corners
            Vector3 Left = plane.Right * -1f;
            Vector3 Up = plane.Forward;
            Vector3 uppercenter = (Up * height / 2) + origin;
            Vector3 UpperLeft = uppercenter + (Left * width / 2);
            Vector3 UpperRight = uppercenter - (Left * width / 2);
            Vector3 LowerLeft = UpperLeft - (Up * height);
            Vector3 LowerRight = UpperRight - (Up * height);

            // Fill in texture coordinates to display full texture
            // on quad
            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            // Provide a normal for each vertex
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal = normal;
            }

            // Set the position and texture coordinate for each
            // vertex
            Vertices[0].Position = LowerLeft;
            Vertices[0].TextureCoordinate = textureLowerLeft;
            Vertices[1].Position = UpperLeft;
            Vertices[1].TextureCoordinate = textureUpperLeft;
            Vertices[2].Position = LowerRight;
            Vertices[2].TextureCoordinate = textureLowerRight;
            Vertices[3].Position = UpperRight;
            Vertices[3].TextureCoordinate = textureUpperRight;

            // Set the index buffer for each vertex, using
            // clockwise winding
            Indices[0] = 0;
            Indices[1] = 1;
            Indices[2] = 2;
            Indices[3] = 2;
            Indices[4] = 1;
            Indices[5] = 3;

        }

        public void Render(RenderContext renderContext, GameTime gameTime)
        {
            //renderContext.GraphicsDevice.Clear(Color.Black);
            // Render a set of quads tiled with a texture centered at the place where the
            // cam position is projected on to our plane
            Vector3 camPosition = renderContext.Camera.Position;

            BasicEffect quadEffect = renderContext.BasicEffect;

            quadEffect.EnableDefaultLighting();


            quadEffect.View = renderContext.Camera.View;
            quadEffect.Projection = renderContext.Camera.Projection;
            quadEffect.TextureEnabled = true;
            quadEffect.Texture = Texture;


            int numTilesPerSide = 10;
            Matrix instancingOffset;

            Vector3 quadCenter;

            Forever.Physics.Collide.Plane plane = geometryData.Prim as Forever.Physics.Collide.Plane;

            float tileSize = 10f;
            Matrix scale = Matrix.CreateScale(tileSize);

            // back, left
            Vector3 renderOrigin = plane.PrinciplePoint;
                //+ plane.Right * -1f * ((numTilesPerSide * tileSize * 0.5f))
                //+ plane.Forward * -1f * ((numTilesPerSide * tileSize * 0.5f));

            float visableRegionSize = numTilesPerSide * tileSize;

            Vector3 camOnPlane = plane.ClosestPoint(camPosition);

            Vector3 planarDiff = camOnPlane - renderOrigin;

            int originRegionX = 0;// (int)(renderOrigin.X / visableRegionSize);
            int originRegionZ = 0;// (int)(renderOrigin.Z / visableRegionSize);

            float camRegionX = (camOnPlane.X / visableRegionSize);
            float camRegionZ = (camOnPlane.Z / visableRegionSize);



            if (camRegionX < originRegionX || camRegionX > originRegionX)
            {
                int regionsX = ((int)planarDiff.X / (int)visableRegionSize) + -1;
                renderOrigin += new Vector3(regionsX * visableRegionSize, 0f, 0f);
                
            }

            if (camRegionZ < originRegionZ || camRegionZ > originRegionZ)
            {
                int regionsZ = ((int)planarDiff.Z / (int)visableRegionSize) + -1;
                renderOrigin += new Vector3(0f, 0f, regionsZ * visableRegionSize);

            }



            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {

                for (int i = -1; i < 3; i++)
                {
                    for (int j = -1; j < 3; j++)
                    {
                        Vector3 regionOffset = new Vector3(i * visableRegionSize, 0f, j * visableRegionSize);

                        for (int x = 0; x < numTilesPerSide; x++)
                        {
                            float localX = tileSize * x;
                            for (int z = 0; z < numTilesPerSide; z++)
                            {
                                float localZ = tileSize * z;
                                quadCenter = renderOrigin + regionOffset + (plane.Right * localX) + (plane.Forward * localZ);
                                
                                quadCenter = new Vector3(quadCenter.X, plane.PrinciplePoint.Y, quadCenter.Z);


                                instancingOffset = Matrix.CreateTranslation(
                                        quadCenter
                                    );
                                quadEffect.World = scale * instancingOffset;
                                pass.Apply();

                                renderContext.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                                    PrimitiveType.TriangleList,
                                    Vertices, 0, 4,
                                    Indices, 0, 2
                                );
                            }
                        }
                    }
                }

            }
        }


        public Matrix World
        {
            get { return body.World; }
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void addForce(Vector3 force)
        {
            body.addForce(force);
        }

        public void addForce(Vector3 force, Vector3 point)
        {
            body.addForce(force, point);
        }

        public void integrate(float duration)
        {
            body.integrate(duration);
        }

        public Vector3 CenterOfMass
        {
            get { return body.CenterOfMass; }
        }

        public void Translate(Vector3 translation)
        {
            body.Translate(translation);
        }
    }
}

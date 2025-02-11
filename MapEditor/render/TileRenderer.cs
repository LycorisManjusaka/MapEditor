﻿/*
 * MapEditor
 * Пользователь: AngryKirC
 * Дата: 29.11.2015
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using NoxShared;
using System.Windows.Forms;
using static NoxShared.ThingDb;
using static NoxShared.Map.Tile;

namespace MapEditor.render
{
	/// <summary>
	/// Responsible for drawing map tiles
	/// </summary>
	internal class TileRenderer
	{
		private readonly MapViewRenderer mapRenderer;
		private readonly List<TileDrawData> visibleTiles;
		const int squareSize = MapView.squareSize;
        
		private struct TileDrawData
		{
			public Bitmap Texture;
			public Map.Tile Tile;
			
			public TileDrawData(Map.Tile tile, Bitmap tex)
			{
				Texture = tex;
				Tile = tile;
			}
		}
		
		public TileRenderer(MapViewRenderer mapRenderer)
		{
			this.mapRenderer = mapRenderer;
			this.visibleTiles = new List<TileDrawData>();
		}

        public void UpdateCanvas(Rectangle clip)
		{
            
			// free cached tile bitmaps
			foreach (TileDrawData tdd in visibleTiles)
			{
				if (tdd.Texture != null)
					tdd.Texture.Dispose();
			}
            
			visibleTiles.Clear();
			// update tile list
            bool prevMode = EditorSettings.Default.Edit_PreviewMode;
			foreach (Map.Tile tile in mapRenderer.Map.Tiles.Values)
            {
				int x = tile.Location.X * squareSize;
                int y = tile.Location.Y * squareSize;
                
                // filter seen tiles
                if (clip.Contains(x, y))
                {
                	Bitmap texture = null;
                    if (prevMode)
	                {
	                	// generate texture
	                	texture = TileGenTexture(tile);
	                }
	                visibleTiles.Add(new TileDrawData(tile, texture));
                }
			}
		}
        public void UpdateCanvasWithFakeTiles(Rectangle clip)
        {
            // update tile list
            bool prevMode = EditorSettings.Default.Edit_PreviewMode;
            foreach (Map.Tile tile in mapRenderer.FakeTiles.Values)
            {
                int x = tile.Location.X * squareSize;
                int y = tile.Location.Y * squareSize;

                // filter seen tiles
                if (clip.Contains(x, y))
                {
                    Bitmap texture = null;
                    if (prevMode)
                    {
                        // generate texture
                        texture = TileGenTexture(tile, true);
                    }
                    visibleTiles.Add(new TileDrawData(tile, texture));
                }
            }
        }

        private Bitmap TileGenTexture(Map.Tile tile, bool transparent = false)
        {
        	if (tile == null) return null;
        	int index = (int)FloorTiles[(int)tile.graphicId].Variations[tile.Variation];
            // duplicate tile image
            Bitmap image = (Bitmap) mapRenderer.VideoBag.GetBitmap(index).Clone();
            if (image == null) return null;
            // paint edges
            if (EditorSettings.Default.Draw_PreviewTexEdges)
            {
	            for (int i = 0; i < tile.EdgeTiles.Count; i++)
	            {
                   
	            	var edge = (EdgeTile) tile.EdgeTiles[i];
					Tile edgeEdge = EdgeTiles[(int)edge.Edge];
					Tile edgeTile = FloorTiles[(int)edge.Graphic];
					byte dirNum = (byte) edge.Dir;
					// fix for MudEdge
                    
					if (dirNum < edgeEdge.Variations.Count)
					{
			    		int indexedge = (int)edgeEdge.Variations[(byte)edge.Dir];
			    		int indextile = (int)edgeTile.Variations[edge.Variation];
                        
			    		mapRenderer.VideoBag.ApplyEdgeMask(image, indexedge, indextile);
					}
                     
	            }
            }

            if (transparent)
            {
                var bmpShader = new BitmapShader(image);
                bmpShader.LockBitmap();
                bmpShader.MakeSemitransparent();
                image = bmpShader.UnlockBitmap();
            }

			return image;
        }
		
		public void Render(Graphics g)
		{
			PointF nwCorner, neCorner, seCorner, swCorner, center;
			Pen pen;
			Map.Tile tile;
			foreach (TileDrawData tdd in visibleTiles)
            {
				tile = tdd.Tile;
                int x = tile.Location.X * squareSize;
                int y = tile.Location.Y * squareSize;
                pen = mapRenderer.ColorLayout.Tiles;

                center = new PointF(x + squareSize / 2f, y + (3 / 2f) * squareSize);
                nwCorner = new PointF(x - squareSize / 2f, y + (3 / 2f) * squareSize);
                neCorner = new PointF(nwCorner.X + squareSize, nwCorner.Y - squareSize);
                swCorner = new PointF(nwCorner.X + squareSize, nwCorner.Y + squareSize);
                seCorner = new PointF(neCorner.X + squareSize, neCorner.Y + squareSize);

                if ((EditorSettings.Default.Edit_PreviewMode && tdd.Tile.graphicId != TileId.Black)
                    || (MainWindow.Instance.imgMode))
                {
                	// draw tile+edges texture
                    if (tdd.Texture != null)
                	{
                        //MessageBox.Show(tdd.Tile.graphicId.ToString());
	                	float gX = tile.Location.X * 23F - 11F;
	            		float gY = tile.Location.Y * 23F + 11F;
	            		g.DrawImage(tdd.Texture, gX, gY, 46F, 46F);
                	}
                }
                else
                {
                    Brush tembrush = new SolidBrush(tile.col);
                   
                    if (MainWindow.Instance.mapView.EdgeMakeNewCtrl.ignoreAllBox.Checked && EditorSettings.Default.Edit_PreviewMode)
                    {
                        Color temColor = Color.FromArgb(160, 10, 10, 10);
                        tembrush = new SolidBrush(temColor);
                    }
                    g.FillPolygon(tembrush, new PointF[] { nwCorner, neCorner, seCorner, swCorner });
                    if (EditorSettings.Default.Edit_PreviewMode)
                        continue;
                    //draw the center dot
                    const int diam = 2;
                    PointF ellTL = new PointF(center.X - diam / 2f, center.Y - diam / 2f);

                    if (tile.EdgeTiles.Count > 0)
                        g.FillEllipse(Brushes.YellowGreen, ellTL.X, ellTL.Y, diam, diam);
                    else
                        g.DrawEllipse(Pens.Brown, ellTL.X, ellTL.Y, diam, diam);
                }

                PointF nwCorner2 = nwCorner;
                PointF neCorner2 = neCorner;
                PointF swCorner2 = swCorner;
                PointF seCorner2 = seCorner;
                PointF neCornerB = neCorner;
                PointF seCornerB = seCorner;
                // Draw edges
                int i = 0; EdgeTile edge;
                
                for (i = 0; i < tile.EdgeTiles.Count; i++)
                {
                    edge = (EdgeTile)tile.EdgeTiles[i];
                    TileId graphId = edge.Graphic;
                    Color col;
                    const int diam = 4;
                    PointF ellTL = new PointF(center.X - diam / 2f, center.Y - diam / 2f);
                    col = Color.FromArgb(unchecked((int)Map.tilecolors[(int)graphId]));
                    
                    if (graphId == tile.graphicId)
                        col = Color.Aqua;

                    // Skip this part if preview mode is enabled
                    if (EditorSettings.Default.Edit_PreviewMode)
                    	continue;
                    var m = new System.Drawing.Drawing2D.Matrix();
                    // Draw Edges (Normal)
                    switch (edge.Dir)
                    {
                        case EdgeTile.Direction.North_08:
                        case EdgeTile.Direction.North_0A:
                        case EdgeTile.Direction.North:
                            nwCorner2.X += 2;
                            nwCorner2.Y += 2;
                            neCorner2.X += 2;
                            neCorner2.Y += 2;
                            g.DrawLine(new Pen(col, 4), nwCorner2, neCorner2);
                            break;
                        case EdgeTile.Direction.South_07:
                        case EdgeTile.Direction.South_09:
                        case EdgeTile.Direction.South:
                            swCorner2.X -= 2;
                            swCorner2.Y -= 2;
                            seCorner2.X -= 2;
                            seCorner2.Y -= 2;
                            g.DrawLine(new Pen(col, 4), swCorner2, seCorner2);
                            break;
                        case EdgeTile.Direction.East_D:
                        case EdgeTile.Direction.East_E:
                        case EdgeTile.Direction.East:
                            neCorner.X -= 2;
                            neCorner.Y += 2;
                            seCorner.X -= 2;
                            seCorner.Y += 2;
                            g.DrawLine(new Pen(col, 4), neCorner, seCorner);
                            break;
                        case EdgeTile.Direction.West_02:
                        case EdgeTile.Direction.West_03:
                        case EdgeTile.Direction.West:
                            nwCorner2.X += 2;
                            nwCorner2.Y -= 2;
                            swCorner2.X += 2;
                            swCorner2.Y -= 2;
                            g.DrawLine(new Pen(col, 4), nwCorner2, swCorner2);
                            break;
                        case EdgeTile.Direction.NE_Sides:
                            neCorner.X -= 2;
                            neCorner.Y += 2;
                            seCorner.X -= 2;
                            seCorner.Y += 2;
                            g.DrawLine(new Pen(col, 4), neCorner, seCorner);
                            nwCorner2 = nwCorner;
                            neCorner2 = neCorner;
                            swCorner2 = swCorner;
                            seCorner2 = seCorner;
                            nwCorner2.X += 2;
                            nwCorner2.Y += 2;
                            neCorner2.X += 2;
                            neCorner2.Y += 2;
                            g.DrawLine(new Pen(col, 4), nwCorner2, neCorner2);
                            break;
                        case EdgeTile.Direction.NW_Sides:
                            nwCorner2.X += 2;
                            nwCorner2.Y -= 2;
                            swCorner2.X += 2;
                            swCorner2.Y -= 2;
                            g.DrawLine(new Pen(col, 4), nwCorner2, swCorner2);
                            nwCorner2 = nwCorner;
                            neCorner2 = neCorner;
                            swCorner2 = swCorner;
                            seCorner2 = seCorner;
                            nwCorner2.X += 2;
                            nwCorner2.Y += 2;
                            neCorner2.X += 2;
                            neCorner2.Y += 2;
                            g.DrawLine(new Pen(col, 4), nwCorner2, neCorner2);
                            break;
                        case EdgeTile.Direction.SE_Sides:
                            swCorner2.X -= 2;
                            swCorner2.Y -= 2;
                            seCorner2.X -= 2;
                            seCorner2.Y -= 2;
                            g.DrawLine(new Pen(col, 4), swCorner2, seCorner2);
                            nwCorner2 = nwCorner;
                            neCorner2 = neCorner;
                            swCorner2 = swCorner;
                            seCorner2 = seCorner;
                            neCorner.X -= 2;
                            neCorner.Y += 2;
                            seCorner.X -= 2;
                            seCorner.Y += 2;
                            g.DrawLine(new Pen(col, 4), neCorner, seCorner);
                            break;

                        case EdgeTile.Direction.SW_Sides:
                            swCorner2.X -= 2;
                            swCorner2.Y -= 2;
                            seCorner2.X -= 2;
                            seCorner2.Y -= 2;
                            g.DrawLine(new Pen(col, 4), swCorner2, seCorner2);
                            nwCorner2 = nwCorner;
                            neCorner2 = neCorner;
                            swCorner2 = swCorner;
                            seCorner2 = seCorner;
                            nwCorner2.X += 2;
                            nwCorner2.Y -= 2;
                            swCorner2.X += 2;
                            swCorner2.Y -= 2;
                            g.DrawLine(new Pen(col, 4), nwCorner2, swCorner2);
                            break;
                        case EdgeTile.Direction.NE_Tip:
                            using (m)
                            {
                                PointF centerTR = new PointF(ellTL.X + 2, ellTL.Y - (squareSize - 4));

                                m.RotateAt(45, centerTR);
                                g.Transform = m;

                                g.DrawRectangle(new Pen(col, 3), centerTR.X, centerTR.Y, 2, 2);
                                g.ResetTransform();
                            }
                            
                            break;
                        case EdgeTile.Direction.SE_Tip:
                            using (m)
                            {
                                PointF centerTR = new PointF(ellTL.X + (squareSize-(float)1.5), ellTL.Y+(float)0.5);

                                m.RotateAt(45, centerTR);
                                g.Transform = m;

                                g.DrawRectangle(new Pen(col, 2), centerTR.X, centerTR.Y, 2, 2);
                                g.ResetTransform();
                            }
                            
                            break;

                        case EdgeTile.Direction.SW_Tip:
                            using (m)
                            {
                                PointF centerTR = new PointF(ellTL.X + 2, ellTL.Y + (squareSize - 2));

                                m.RotateAt(45, centerTR);
                                g.Transform = m;

                                g.DrawRectangle(new Pen(col, 3), centerTR.X, centerTR.Y, 2, 2);
                                g.ResetTransform();
                            }
                            break;
                        case EdgeTile.Direction.NW_Tip:
                            using (m)
                            {
                                PointF centerTR = new PointF(ellTL.X - (squareSize-5), ellTL.Y+(float)0.5);

                                m.RotateAt(45, centerTR);
                                g.Transform = m;

                                g.DrawRectangle(new Pen(col, 3), centerTR.X, centerTR.Y, 2, 2);
                                g.ResetTransform();
                            }
                            break;
                        default: break;
                    }
                }
            }
		}
	}
}

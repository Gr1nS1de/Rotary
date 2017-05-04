using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	public static partial class D2dFloodfill
	{
		private class Spread
		{
			public int i;
			public int x;
			public int y;
		}
		
		public static List<Island> TempIslands = new List<Island>();
		
		private static byte[] cells;
		
		private static List<Spread> spreads = new List<Spread>();
		
		private static Island currentIsland;
		
		private static int spreadCount;
		
		private static int sizeX;
		
		private static int sizeY;

		private static int minX;

		private static int maxX;

		private static int minY;

		private static int maxY;
		
		private static int total;
		
		private const byte CELL_EMPTY = 0;
		
		private const byte CELL_SOLID = 1;
		
		private const byte CELL_CLAIM = 2;
		
		public static Island SpawnTempIsland(bool addToTempList = true)
		{
			var island = D2dPool<Island>.Spawn() ?? new Island();

			if (addToTempList == true)
			{
				TempIslands.Add(island);
			}

			return island;
		}

		public static void DespawnTempIslands(Island borderIsland = null)
		{
			if (borderIsland != null)
			{
				D2dPool<Island>.Despawn(borderIsland, j => j.Clear());
			}

			if (TempIslands.Count > 0)
			{
				for (var i = TempIslands.Count - 1; i >= 0; i--)
				{
					D2dPool<Island>.Despawn(TempIslands[i], j => j.Clear());
				}
			
				TempIslands.Clear();
			}
		}

		public static Island FindLocal(byte[] alphaData, int alphaWidth, int alphaHeight, D2dRect findRect)
		{
			if (D2dHelper.AlphaIsValid(alphaData, alphaWidth, alphaHeight) == false)
			{
				Debug.LogError("Alpha data invalid"); return null;
			}

			var oldRect = new D2dRect(0, alphaWidth, 0, alphaHeight);
			var lapRect = D2dRect.CalculateOverlap(oldRect, findRect);
				
			if (lapRect.IsSet == false)
			{
				return null;
			}
				
			minX        = lapRect.MinX;
			maxX        = lapRect.MaxX;
			minY        = lapRect.MinY;
			maxY        = lapRect.MaxY;
			sizeX       = lapRect.SizeX;
			sizeY       = lapRect.SizeY;
			total       = sizeX * sizeY;
			spreadCount = 0;
			
			if (cells == null || cells.Length < total)
			{
				cells = new byte[total];
			}

			DespawnTempIslands();

			// Write solid or empty cells
			for (var y = minY; y < maxY; y++)
			{
				var c = (y - minY) * sizeX - minX;
				var d = y * alphaWidth;
				
				for (var x = minX; x < maxX; x++)
				{
					cells[c + x] = alphaData[d + x] > 127 ? CELL_SOLID : CELL_EMPTY;
				}
			}
					
			var borderIsland = default(Island);
			
			// Border coming from left?
			if (minX > 0)
			{
				for (var y = 0; y < sizeY; y++)
				{
					var i = y * sizeX;

					if (cells[i] == CELL_SOLID)
					{
						if (borderIsland == null) borderIsland = currentIsland = SpawnTempIsland(false);

						BeginFloodFill(i, i % sizeX, i / sizeX);
					}
				}
			}

			// Border coming from right?
			if (maxX < alphaWidth)
			{
				for (var y = 0; y < sizeY; y++)
				{
					var i = y * sizeX + sizeX - 1;

					if (cells[i] == CELL_SOLID)
					{
						if (borderIsland == null) borderIsland = currentIsland = SpawnTempIsland(false);

						BeginFloodFill(i, i % sizeX, i / sizeX);
					}
				}
			}

			// Border coming from bottom?
			if (minY > 0)
			{
				for (var x = 0; x < sizeX; x++)
				{
					var i = x;

					if (cells[i] == CELL_SOLID)
					{
						if (borderIsland == null) borderIsland = currentIsland = SpawnTempIsland(false);

						BeginFloodFill(i, i % sizeX, i / sizeX);
					}
				}
			}

			// Border coming from top?
			if (maxY < alphaHeight)
			{
				for (var x = 0; x < sizeX; x++)
				{
					var i = x + sizeX * (sizeY - 1);

					if (cells[i] == CELL_SOLID)
					{
						if (borderIsland == null) borderIsland = currentIsland = SpawnTempIsland(false);

						BeginFloodFill(i, i % sizeX, i / sizeX);
					}
				}
			}

			for (var i = 0; i < total; i++)
			{
				if (cells[i] == CELL_SOLID)
				{
					currentIsland = SpawnTempIsland();
							
					BeginFloodFill(i, i % sizeX, i / sizeX);
				}
			}
					
			return borderIsland;
		}

		public static void Find(byte[] alphaData, int alphaWidth, int alphaHeight)
		{
			if (D2dHelper.AlphaIsValid(alphaData, alphaWidth, alphaHeight) == false)
			{
				throw new System.ArgumentException("Invalid alpha");
			}

			minX  = 0;
			maxX  = alphaWidth;
			minY  = 0;
			maxY  = alphaHeight;
			sizeX = alphaWidth;
			sizeY = alphaHeight;
			total = sizeX * sizeY;
			
			if (cells == null || cells.Length < total)
			{
				cells = new byte[total];
			}
			
			DespawnTempIslands();
			
			// Find all solid pixels
			for (var i = 0; i < total; i++)
			{
				cells[i] = alphaData[i] > 127 ? CELL_SOLID : CELL_EMPTY;
			}

			for (var i = 0; i < total; i++)
			{
				if (cells[i] == CELL_SOLID)
				{
					currentIsland = SpawnTempIsland();
					
					BeginFloodFill(i, i % sizeX, i / sizeX);
				}
			}
		}
		
		public static void Feather(Island island)
		{
			for (var i = island.Pixels.Count - 1; i >= 0; i--)
			{
				var pixel = island.Pixels[i];
				var x     = pixel.X;
				var y     = pixel.Y;
				
				TryFeather(island, x - 1, y    );
				TryFeather(island, x + 1, y    );
				TryFeather(island, x    , y - 1);
				TryFeather(island, x    , y + 1);
			}
		}
		
		private static void TryFeather(Island island, int x, int y)
		{
			if (x >= 0 && y >= 0 && x < sizeX && y < sizeY)
			{
				var i = x + y * sizeX;
				
				if (cells[i] == CELL_EMPTY)
				{
					cells[i] = CELL_CLAIM;
					
					island.AddPixel(x, y);
				}
			}
			else
			{
				island.AddPixel(x, y);
			}
		}
		
		private static void BeginFloodFill(int i, int x, int y)
		{
			var oldSpreadCount = 0; spreadCount = 0;
			
			SpreadTo(i, x, y);
			
			// Non-recursive floodfill
			while (spreadCount != oldSpreadCount)
			{
				var start = oldSpreadCount;
				var end   = oldSpreadCount = spreadCount;
				
				for (var j = start; j < end; j++)
				{
					var spread = spreads[j];
					
					FloodFill(spread.i, spread.x, spread.y);
				}
			}
		}
		
		private static void SpreadTo(int i, int x, int y)
		{
			if (x != (i%sizeX))
			{
				throw new System.Exception();
			}
			if (y != (i/sizeX))
			{
				throw new System.Exception();
			}

			cells[i] = CELL_CLAIM;
			
			var spread = default(Spread);
			
			if (spreadCount >= spreads.Count)
			{
				spread = new Spread(); spreads.Add(spread);
			}
			else
			{
				spread = spreads[spreadCount];
			}
			
			spreadCount += 1;
			
			spread.i = i;
			spread.x = x;
			spread.y = y;
			
			currentIsland.AddPixel(minX + x, minY + y);
		}
		
		private static void FloodFill(int i, int x, int y)
		{
			// Left
			if (x > 0)
			{
				var n = i - 1;
				
				if (cells[n] == CELL_SOLID)
				{
					SpreadTo(n, x - 1, y);
				}
			}
			
			// Right
			if (x < sizeX - 1)
			{
				var n = i + 1;
				
				if (cells[n] == CELL_SOLID)
				{
					SpreadTo(n, x + 1, y);
				}
			}
			
			// Bottom
			if (y > 0)
			{
				var n = i - sizeX;
				if (n < 0 || n >= cells.Length)
				{
					Debug.Log(n + " - " + i + " - " + (i%sizeX) + " - " + (i/sizeX) + " - " + x + " - " + y + " - " + sizeX + " - " + sizeY + " - " + cells.Length);
				}
				if (cells[n] == CELL_SOLID)
				{
					SpreadTo(n, x, y - 1);
				}
			}
			
			// Top
			if (y < sizeY - 1)
			{
				var n = i + sizeX;
				
				if (cells[n] == CELL_SOLID)
				{
					SpreadTo(n, x, y + 1);
				}
			}
		}
	}
}
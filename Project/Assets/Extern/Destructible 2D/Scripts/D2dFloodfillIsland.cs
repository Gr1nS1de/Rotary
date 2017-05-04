using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	public static partial class D2dFloodfill
	{
		public class Island
		{
			public List<D2dFloodfillPixel> Pixels = new List<D2dFloodfillPixel>();
		
			public void AddPixel(int x, int y)
			{
				var pixel = D2dPool<D2dFloodfillPixel>.Spawn() ?? new D2dFloodfillPixel();
			
				pixel.X = x;
				pixel.Y = y;
			
				Pixels.Add(pixel);
			}
		
			public void Clear()
			{
				for (var i = Pixels.Count - 1; i >= 0; i--)
				{
					D2dPool<D2dFloodfillPixel>.Despawn(Pixels[i]);
				}
			
				Pixels.Clear();
			}

			public D2dSplitGroup CreateSplitGroup(bool feather, bool allowExpand)
			{
				if (feather == true)
				{
					D2dFloodfill.Feather(this);
				}

				var group = D2dPool<D2dSplitGroup>.Spawn() ?? new D2dSplitGroup();

				for (var j = Pixels.Count - 1; j >= 0; j--)
				{
					var pixel = Pixels[j];

					group.AddPixel(pixel.X, pixel.Y);
				}

				// Feather will automatically add a transparent border
				if (allowExpand == true && feather == false)
				{
					group.Rect.MinX -= 1;
					group.Rect.MaxX += 1;
					group.Rect.MinY -= 1;
					group.Rect.MaxY += 1;
				}

				return group;
			}
		}
	}
}
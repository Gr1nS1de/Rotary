using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dPolygonCollider))]
	public class D2dPolygonCollider_Editor : D2dCollider_Editor<D2dPolygonCollider>
	{
		protected override void OnInspector()
		{
			var destroyChild = false;
			
			DrawDefault("CellSize", ref destroyChild);
			DrawDefault("Detail", ref destroyChild);

			if (destroyChild == true) DirtyEach(t => t.DestroyChild());
			
			base.OnInspector();
		}
	}
}
#endif

namespace Destructible2D
{
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Polygon Collider")]
	public class D2dPolygonCollider : D2dCollider
	{
		[Tooltip("The size of each collider cell")]
		[D2dPopup(8, 16, 32, 64, 128, 256)]
		public int CellSize = 64;
		
		[Tooltip("How many vertices should remain in the collider shapes")]
		[Range(0.5f, 1.0f)]
		public float Detail = 0.9f;
		
		[SerializeField]
		private int width;
		
		[SerializeField]
		private int height;
		
		[SerializeField]
		private List<D2dPolygonColliderCell> cells = new List<D2dPolygonColliderCell>();
		
		[System.NonSerialized]
		private List<PolygonCollider2D> unusedColliders = new List<PolygonCollider2D>();
		
		public override void UpdateColliderSettings()
		{
			for (var i = cells.Count - 1; i >= 0; i--)
			{
				cells[i].UpdateColliderSettings(IsTrigger, Material);
			}
		}
		
		protected override void OnAlphaDataReplaced()
		{
			base.OnAlphaDataReplaced();
			
			Rebuild();
		}
		
		protected override void OnAlphaDataModified(D2dRect rect)
		{
			base.OnAlphaDataModified(rect);
			
			if (CellSize > 0)
			{
				var cellXMin = rect.MinX / CellSize;
				var cellYMin = rect.MinY / CellSize;
				var cellXMax = (rect.MaxX + 1) / CellSize;
				var cellYMax = (rect.MaxY + 1) / CellSize;
				
				// Mark
				for (var cellY = cellYMin; cellY <= cellYMax; cellY++)
				{
					var offset = cellY * width;
					
					for (var cellX = cellXMin; cellX <= cellXMax; cellX++)
					{
						var index = cellX + offset;
						
						if (index >= 0 && index < cells.Count)
						{
							Mark(cells[index]);
						}
						else
						{
							Regenerate();
						}
					}
				}
				
				// Generate
				for (var cellY = cellYMin; cellY <= cellYMax; cellY++)
				{
					var offset = cellY * width;
					
					for (var cellX = cellXMin; cellX <= cellXMax; cellX++)
					{
						var index = cellX + offset;
						
						if (index >= 0 && index < cells.Count)
						{
							RebuildCell(cells[index], cellX, cellY);
						}
					}
				}
				
				Sweep();
			}
			else
			{
				Rebuild();
			}
		}
		
		protected override void OnAlphaDataSubset(D2dRect rect)
		{
			base.OnAlphaDataSubset(rect);
			
			Rebuild();
		}
		
		protected override void OnStartSplit()
		{
			base.OnStartSplit();
			
			Mark();
			Sweep();
		}
		
		private void Mark()
		{
			for (var i = cells.Count - 1; i >= 0; i--)
			{
				D2dPool<D2dPolygonColliderCell>.Despawn(cells[i], c => c.Clear(unusedColliders));
			}
			
			cells.Clear();
		}
		
		private void Mark(D2dPolygonColliderCell cell)
		{
			cell.Clear(unusedColliders);
		}
		
		private void Sweep()
		{
			for (var i = unusedColliders.Count - 1; i >= 0; i--)
			{
				D2dHelper.Destroy(unusedColliders[i]);
			}
			
			unusedColliders.Clear();
		}
		
		private void Rebuild()
		{
			Mark();
			{
				if (CellSize > 0)
				{
					width  = (destructible.AlphaWidth  + CellSize - 1) / CellSize;
					height = (destructible.AlphaHeight + CellSize - 1) / CellSize;
					
					for (var y = 0; y < height; y++)
					{
						for (var x = 0; x < width; x++)
						{
							var cell = D2dPool<D2dPolygonColliderCell>.Spawn() ?? new D2dPolygonColliderCell();
							
							RebuildCell(cell, x, y);
							
							cells.Add(cell);
						}
					}
					
					UpdateColliderSettings();
				}
			}
			Sweep();
		}
		
		private void RebuildCell(D2dPolygonColliderCell cell, int x, int y)
		{
			var xMin = CellSize * x;
			var yMin = CellSize * y;
			var xMax = Mathf.Min(CellSize + xMin, destructible.AlphaWidth );
			var yMax = Mathf.Min(CellSize + yMin, destructible.AlphaHeight);
			
			D2dColliderBuilder.CalculatePoly(destructible.AlphaData, destructible.AlphaWidth, xMin, xMax, yMin, yMax);
			
			D2dColliderBuilder.BuildPoly(cell, unusedColliders, child, Detail);
			
			cell.UpdateColliderSettings(IsTrigger, Material);
		}
	}
}
using UnityEngine;

namespace SplatterSystem.Platformer {
	
	public class GroundState {
		private readonly GameObject player;
		private readonly Transform pt;
		private readonly float width;
		private readonly float height;
		private const float RaycastDistance = 0.05f;

		public RaycastHit2D raycastDown;
		public RaycastHit2D raycastUp;
		public RaycastHit2D raycastLeft;
		public RaycastHit2D raycastRight;

		public GroundState(GameObject playerGameObject) {
			player = playerGameObject;
			pt = player.transform;

			var colliderExtents = player.GetComponent<Collider2D>().bounds.extents;
			width = colliderExtents.x + 0.1f;
			height = colliderExtents.y + 0.2f;
		}

		/// <summary>
		/// Returns whether player is touching a wall.
		/// </summary>
		public bool IsWall() {
			raycastLeft = Physics2D.Raycast(pt.position - width*Vector3.right, -Vector2.right, RaycastDistance);
			raycastRight = Physics2D.Raycast(pt.position + width*Vector3.right, Vector2.right, RaycastDistance);

			return (raycastLeft || raycastRight);
		}

		/// <summary>
		/// Returns whether player is touching the ceiling.
		/// </summary>
		public bool IsCeiling() {
			raycastUp = Physics2D.Raycast(pt.position + width*Vector3.up, Vector2.up, RaycastDistance);
			return raycastUp;
		}

		/// <summary>
		/// Returns whether player is touching ground.
		/// </summary>
		public bool IsGround() {
			var vecDown = new Vector2(pt.position.x, pt.position.y - height);
			var vecLeft = new Vector2(pt.position.x - (width - 0.2f), pt.position.y - height);
			var vecRight = new Vector2(pt.position.x + (width - 0.2f), pt.position.y - height);
			raycastDown = Physics2D.Raycast(vecDown, -Vector2.up, RaycastDistance);
			bool bottom2 = Physics2D.Raycast(vecRight, -Vector2.up, RaycastDistance);
			bool bottom3 = Physics2D.Raycast(vecLeft, -Vector2.up, RaycastDistance);
			return raycastDown || bottom2 || bottom3;
		}

		/// <summary>
		/// Returns whether player is touching wall or ground.
		/// </summary>
		public bool IsTouching() {
			return (IsGround() || IsWall() || IsCeiling());
		}

		/// <summary>
		/// Returns direction of the touching wall.
		/// </summary>
		public int WallDirection() {
			bool left = Physics2D.Raycast(pt.position - width*Vector3.right, -Vector2.right, RaycastDistance);
			bool right = Physics2D.Raycast(pt.position + width*Vector3.right, Vector2.right, RaycastDistance);

			if (left) return -1;
			else if (right) return 1;
			else return 0;
		}
	}


}

using UnityEngine;

namespace SplatterSystem {
    public abstract class AbstractSplatterManager : MonoBehaviour {
        public abstract void SetDefaultSettings(SplatterSettings settings);
        public abstract void Spawn(Vector3 position);
        public abstract void Spawn(Vector3 position, Vector3 direction);
        public abstract void Spawn(Vector3 position, Color color);
        public abstract void Spawn(Vector3 position, Vector3? direction, Color? color);
        public abstract void Spawn(SplatterSettings settings, Vector3 position, Vector3? direction, Color? color);
        public abstract void Clear();
    }
}
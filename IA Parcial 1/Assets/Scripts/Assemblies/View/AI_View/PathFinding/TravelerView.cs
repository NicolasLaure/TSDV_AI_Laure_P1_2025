using System;
using UnityEngine;

namespace AI_View.Pathfinding
{
    public class TravelerView : MonoBehaviour
    {
        public Vector3 scale;
        public Vector3 position;
        
        private void Awake()
        {
            scale = transform.localScale;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
            transform.position = position;
        }
    }
}
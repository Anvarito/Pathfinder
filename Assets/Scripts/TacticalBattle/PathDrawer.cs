using System.Collections.Generic;
using UnityEngine;

namespace TacticalBattle
{
    public class PathDrawer : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        public Color StartColor = Color.yellow;
        public Color EndColor = Color.yellow;
        public float _height = 0.5f;

        private void Start()
        {
            // Добавляем LineRenderer для визуализации пути
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = StartColor;
            lineRenderer.endColor = EndColor;
        }
        
        
        public void UpdatePathVisualization(List<PathNode> path)
        {
            lineRenderer.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                Vector3 position = path[i].GridPosition;
                position.y = transform.position.y + _height; // Немного выше земли
                lineRenderer.SetPosition(i, position);
            }
        }

        public void ClearPathVisualization()
        {
            lineRenderer.positionCount = 0;
        }
    }
}
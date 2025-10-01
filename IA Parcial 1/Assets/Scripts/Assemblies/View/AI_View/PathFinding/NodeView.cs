using System;
using System.Collections.Generic;
using AI_Model.Pathfinding;
using RTS.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace AI_View.Pathfinding
{
    public class NodeView : MonoBehaviour, IPointerClickHandler
    {
        public MapNode node;

        [SerializeField] private Material blockedMaterial;
        [SerializeField] private Color pathColor;
        [SerializeField] private List<Material> typeColors = new List<Material>();
        [SerializeField] private TextMeshPro text;
        [SerializeField] private SpriteRenderer frontSprite;
        [SerializeField] private SpriteRenderer areaSprite;
        [SerializeField] private float areaAlpha;

        private Material areaMaterial;

        private Dictionary<int, Transitability> typeToWeight;
        private int currentType;

        private Material currentMaterial;
        public Vector3 position;

        public Material CurrentMaterial => areaMaterial != null ? areaMaterial : currentMaterial;

        public void Init(MapNode node, Dictionary<int, Transitability> typeToWeight)
        {
            this.typeToWeight = typeToWeight;
            this.node = node;
            currentType = node.GetTileType();
            UpdateMaterial();
            UpdateNumberText(typeToWeight[node.GetTileType()].weight);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int enumInt = (int)currentType;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                currentType = enumInt + 1 < Enum.GetValues(typeof(TileType)).Length ? enumInt + 1 : 0;
                node.SetTileType(currentType);
                //UpdateWeight();
                UpdateMaterial();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                currentType = enumInt - 1 > 0 ? enumInt - 1 : Enum.GetValues(typeof(TileType)).Length - 1;
                node.SetTileType(currentType);
                //UpdateWeight();
                UpdateMaterial();
            }
            else if (eventData.button == PointerEventData.InputButton.Middle)
                ToggleBlock();
        }

        private void UpdateWeight()
        {
            UpdateNumberText(typeToWeight[node.GetTileType()].weight);
        }

        private void ToggleBlock()
        {
            node.ToggleBlock();
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            if (node.IsBlocked())
                currentMaterial = blockedMaterial;
            else
                currentMaterial = typeColors[(int)currentType];
        }

        private void UpdateNumberText(int newWeight)
        {
            text.text = newWeight.ToString();
        }

        public void SetAreaMaterial(Material material)
        {
            areaMaterial = material;
        }
    }
}
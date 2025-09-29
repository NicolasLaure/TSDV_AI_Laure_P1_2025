using System;
using System.Collections.Generic;
using AI_Model.Pathfinding;
using RTS.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.WSA;

namespace AI_View.Pathfinding
{
    public class NodeView : MonoBehaviour, IPointerClickHandler
    {
        public MapNode node;

        [SerializeField] private Color blockedColor;
        [SerializeField] private Color pathColor;
        [SerializeField] private List<Color> typeColors = new List<Color>();
        [SerializeField] private TextMeshPro text;
        [SerializeField] private SpriteRenderer frontSprite;
        [SerializeField] private SpriteRenderer areaSprite;
        [SerializeField] private float areaAlpha;

        private Color prevColor;
        private Color areaColor;

        private Dictionary<Enum, Transitability> typeToWeight;
        private TileType currentType;

        public void Init(MapNode node, Dictionary<Enum, Transitability> typeToWeight)
        {
            this.typeToWeight = typeToWeight;
            this.node = node;
            currentType = node.GetTileType<TileType>();
            UpdateMaterial();
            UpdateNumberText(typeToWeight[node.GetTileType<TileType>()].weight);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int enumInt = (int)currentType;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                currentType = (TileType)(enumInt + 1 < Enum.GetValues(typeof(TileType)).Length ? enumInt + 1 : 0);
                node.SetTileType(currentType);
                //UpdateWeight();
                UpdateMaterial();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                currentType = (TileType)(enumInt - 1 > 0 ? enumInt - 1 : Enum.GetValues(typeof(TileType)).Length - 1);
                node.SetTileType(currentType);
                //UpdateWeight();
                UpdateMaterial();
            }
            else if (eventData.button == PointerEventData.InputButton.Middle)
                ToggleBlock();
        }

        private void UpdateWeight()
        {
            UpdateNumberText(typeToWeight[node.GetTileType<TileType>()].weight);
        }

        private void ToggleBlock()
        {
            node.ToggleBlock();
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            if (node.IsBlocked())
                frontSprite.color = blockedColor;
            else
                frontSprite.color = typeColors[(int)currentType];
        }

        private void UpdateNumberText(int newWeight)
        {
            text.text = newWeight.ToString();
        }

        public void SetPath(bool shouldDraw)
        {
            if (shouldDraw)
            {
                prevColor = frontSprite.color;
                frontSprite.color = pathColor;
            }
            else
                frontSprite.color = prevColor;
        }

        public void SetAreaColor(Color color)
        {
            areaColor = color;
            areaColor.a = areaAlpha;
            areaSprite.color = areaColor;
        }
    }
}
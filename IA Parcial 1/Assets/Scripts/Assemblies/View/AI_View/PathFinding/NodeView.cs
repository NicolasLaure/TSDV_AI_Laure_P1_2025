using AI_Model.Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AI_View.Pathfinding
{
    public class NodeView : MonoBehaviour, IPointerClickHandler
    {
        public INode node;

        [SerializeField] private Color clearColor;
        [SerializeField] private Color blockedColor;
        [SerializeField] private Color pathColor;

        [SerializeField] private TextMeshPro text;
        [SerializeField] private SpriteRenderer frontSprite;

        private Color prevColor;
        private Color areaColor;

        public void Init(INode node)
        {
            this.node = node;
            UpdateMaterial();
            UpdateNumberText((node as IWeightedNode).GetWeight());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                UpdateWeight(true);
            else if (eventData.button == PointerEventData.InputButton.Right)
                UpdateWeight(false);
            else if (eventData.button == PointerEventData.InputButton.Middle)
                ToggleBlock();
        }

        private void UpdateWeight(bool goesUp)
        {
            IWeightedNode weightedNode = (node as IWeightedNode);
            int newWeight = goesUp ? weightedNode.GetWeight() + 1 : weightedNode.GetWeight() - 1;
            weightedNode.SetWeight(newWeight);
            UpdateNumberText(weightedNode.GetWeight());
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
                frontSprite.color = clearColor;
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
            if (node.IsBlocked())
                areaColor = color * blockedColor;

            frontSprite.color = areaColor;
        }
    }
}
using AI_Model.Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AI_View.Pathfinding
{
    public class NodeView : MonoBehaviour, IPointerClickHandler
    {
        public INode node;

        [SerializeField] private Material clearMat;
        [SerializeField] private Material blockedMat;
        [SerializeField] private Material pathMat;

        [SerializeField] private TextMeshPro text;
        private MeshRenderer _meshRenderer;

        private Material prevMat;

        public void Init(INode node)
        {
            this.node = node;
            _meshRenderer ??= GetComponent<MeshRenderer>();
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
                _meshRenderer.material = blockedMat;
            else
                _meshRenderer.material = clearMat;
        }

        private void UpdateNumberText(int newWeight)
        {
            text.text = newWeight.ToString();
        }

        public void SetPath(bool shouldDraw)
        {
            if (shouldDraw)
            {
                prevMat = _meshRenderer.material;
                _meshRenderer.material = pathMat;
            }
            else
                _meshRenderer.material = prevMat;
        }
    }
}
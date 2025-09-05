using Pathfinder;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeView : MonoBehaviour, IPointerClickHandler
{
    public INode node;

    [SerializeField] private Material clearMat;
    [SerializeField] private Material blockedMat;

    private MeshRenderer _meshRenderer;

    public void Init(INode node)
    {
        this.node = node;
        _meshRenderer ??= GetComponent<MeshRenderer>();
        UpdateMaterial();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleBlock();
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
}
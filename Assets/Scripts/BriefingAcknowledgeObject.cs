using UnityEngine;
using UnityEngine.InputSystem;

public class BriefingAcknowledgeObject : MonoBehaviour
{
    [Header("Connections")]
    public BriefingManager briefingManager;

    private Collider _myCollider3D;
    private Collider2D _myCollider2D;
    private Camera _cam;

    void Awake()
    {
        _myCollider3D = GetComponent<Collider>();
        _myCollider2D = GetComponent<Collider2D>();
        _cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null || _cam == null || briefingManager == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            bool clickedMe = false;

            if (_myCollider3D != null)
            {
                Ray ray = _cam.ScreenPointToRay(mouseScreenPos);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == _myCollider3D)
                {
                    clickedMe = true;
                }
            }
            else if (_myCollider2D != null)
            {
                if (_myCollider2D.OverlapPoint(_cam.ScreenToWorldPoint(mouseScreenPos)))
                {
                    clickedMe = true;
                }
            }

            if (clickedMe)
            {
                // --- THIS IS THE ONLY CHANGED LINE ---
                briefingManager.ToggleMessage();
            }
        }
    }
}
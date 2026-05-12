using UnityEngine;
using UnityEngine.InputSystem;

public class BriefingAcknowledgeObject : MonoBehaviour
{
    [Header("Connections")]
    [Tooltip("Drag your Mission Manager here!")]
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

        // Listen for the click even while Time.timeScale is 0!
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            bool clickedMe = false;

            // Check for 3D Colliders
            if (_myCollider3D != null)
            {
                Ray ray = _cam.ScreenPointToRay(mouseScreenPos);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == _myCollider3D)
                {
                    clickedMe = true;
                }
            }
            // Check for 2D Colliders (just in case!)
            else if (_myCollider2D != null)
            {
                if (_myCollider2D.OverlapPoint(_cam.ScreenToWorldPoint(mouseScreenPos)))
                {
                    clickedMe = true;
                }
            }

            // If we clicked the speaker, tell the manager to start the game!
            if (clickedMe)
            {
                briefingManager.AcknowledgeAndStart();
            }
        }
    }
}
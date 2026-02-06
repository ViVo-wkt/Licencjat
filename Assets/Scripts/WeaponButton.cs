using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponButton : MonoBehaviour
{
    public enum ButtonType { SemiActive, FireAndForget }
    public ButtonType type;
    public WeaponSelector selectorManager; // Reference to the brain

    private Collider2D _myCollider;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            
            if (_myCollider.OverlapPoint(mousePos))
            {
                // We were clicked! Tell the manager.
                if (type == ButtonType.SemiActive)
                {
                    selectorManager.SelectSemiActive();
                }
                else
                {
                    selectorManager.SelectActive();
                }
            }
        }
    }
}
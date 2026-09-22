using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;

public class MenuButton : MonoBehaviour
{
    [Header("2D Visuals")]
    public Sprite unpressedSprite;
    public Sprite pressedSprite;
    private SpriteRenderer _spriteRenderer;

    [Header("3D Visuals (Optional)")]
    [Tooltip("Drag your 3D button model here")]
    public Renderer buttonMeshRenderer; 
    
    [Tooltip("Drag your raw .jpg / .png textures here")]
    public Texture2D unpressedTexture;
    public Texture2D pressedTexture;

    [Header("Audio")]
    [Tooltip("Leave blank to use the default click, or drag a custom SFX here!")]
    public AudioClip customButtonSound;

    [Header("Settings")]
    public float actionDelay = 0.2f;
    
    [Header("Action")]
    public UnityEvent onClickAction;

    private Collider2D _col2D;
    private Collider _col3D; 
    private Camera _cam;
    private bool _isPressed = false;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _col2D = GetComponent<Collider2D>();
        _col3D = GetComponent<Collider>();
        _cam = Camera.main;
        
        if (_spriteRenderer != null && unpressedSprite != null)
            _spriteRenderer.sprite = unpressedSprite;

        if (buttonMeshRenderer != null && unpressedTexture != null)
            buttonMeshRenderer.material.mainTexture = unpressedTexture;
    }

    void Update()
    {
        if (_isPressed || Mouse.current == null || _cam == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            bool hit = false;

            if (_col2D != null) 
            {
                if (_col2D.OverlapPoint(_cam.ScreenToWorldPoint(mouseScreenPos))) hit = true;
            }
            else if (_col3D != null) 
            {
                Ray ray = _cam.ScreenPointToRay(mouseScreenPos);
                if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.collider == _col3D) hit = true;
            }

            if (hit) StartCoroutine(PressRoutine());
        }
    }

    IEnumerator PressRoutine()
    {
        _isPressed = true;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound(customButtonSound);
        
        if (_spriteRenderer != null && pressedSprite != null)
            _spriteRenderer.sprite = pressedSprite;

        if (buttonMeshRenderer != null && pressedTexture != null)
            buttonMeshRenderer.material.mainTexture = pressedTexture;

        yield return new WaitForSeconds(actionDelay);

        onClickAction.Invoke();

        if (_spriteRenderer != null && unpressedSprite != null)
            _spriteRenderer.sprite = unpressedSprite;

        if (buttonMeshRenderer != null && unpressedTexture != null)
            buttonMeshRenderer.material.mainTexture = unpressedTexture;
            
        _isPressed = false;
    }
}
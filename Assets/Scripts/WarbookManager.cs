using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class WarbookManager : MonoBehaviour
{
    [System.Serializable]
    public class WarbookEntry
    {
        public string title;
        [TextArea(8, 20)]
        public string description;
    }

    [Header("Hardware Switch Link")]
    public SliderSwitch sliderSwitch;

    [Header("Screen Panels")]
    public GameObject targetInfoScreen;   // Default target info HUD
    public GameObject warbookMasterPanel; // Root parent of all Warbook UI
    public GameObject listSubPanel;       // Panel displaying index list & scroll arrows
    public GameObject detailSubPanel;     // Panel displaying title & description

    [Header("List View Elements")]
    public Button[] listRowButtons;       // The visible rows on screen (e.g. 9 buttons)
    public TMP_Text[] listRowTexts;       // The text components on those buttons
    public Button scrollUpButton;
    public Button scrollDownButton;

    [Header("Detail View Elements")]
    public TMP_Text detailTitleText;
    public TMP_Text detailBodyText;
    public Button backButton;

    [Header("Database")]
    public List<WarbookEntry> database = new List<WarbookEntry>()
    {
        new WarbookEntry { 
            title = "SARH MISSILE", 
            description = "SEMI-ACTIVE RADAR HOMING\n\nRelies on continuous radar illumination from the ground station. Highly effective against medium-range targets but requires uninterrupted beam tracking." 
        },
        new WarbookEntry { 
            title = "ARH MISSILE", 
            description = "ACTIVE RADAR HOMING\n\nCarries an onboard seeker head for terminal guidance. Once launched toward the designated target bearing, it operates autonomously." 
        },
        new WarbookEntry { 
            title = "AUTO MISSILE", 
            description = "INFRARED SHORT-RANGE\n\nPassive thermal detection intended for close-in interception. Rapidly tracks targets acquired within the optical targeting reticle." 
        },
        new WarbookEntry { 
            title = "BEAM RADAR", 
            description = "DIRECTIONAL ILLUMINATOR\n\nHigh-power, narrow-cone antenna manually steered to designate hostiles and guide semi-active munitions." 
        },
        new WarbookEntry { 
            title = "SWEEP RADAR", 
            description = "SURVEILLANCE RADAR\n\nContinuously rotating search beam providing early warning and periodic positional updates of airspace contacts." 
        },
        new WarbookEntry { 
            title = "DRONE", 
            description = "LOW-ALTITUDE RECON / LOITERING MUNITION\n\nSlow-moving, small radar cross-section. Low radar reflectivity makes early detection challenging." 
        },
        new WarbookEntry { 
            title = "CRUISE MISSILE", 
            description = "STAND-OFF STRIKE WEAPON\n\nHigh-speed, low-altitude ingress designed to evade detection. Requires rapid interception upon identification." 
        },
        new WarbookEntry { 
            title = "AIRLINER", 
            description = "CIVILIAN FLIGHT TRAFFIC\n\nNon-combatant air traffic maintaining steady speed and altitude along predetermined corridors. Destruction results in severe command penalties." 
        },
        new WarbookEntry { 
            title = "FIGHTER-BOMBER", 
            description = "FAST ATTACK CRAFT\n\nHeavily armed hostile platform navigating directly toward base infrastructure. Employs speed and altitude variations." 
        }
    };

    private int _topVisibleIndex = 0;
    private bool _isWarbookActive = false;

    void Start()
    {
        // Wire up list button clicks
        for (int i = 0; i < listRowButtons.Length; i++)
        {
            int slotIndex = i;
            listRowButtons[i].onClick.AddListener(() => OnRowClicked(slotIndex));
        }

        if (scrollUpButton != null) scrollUpButton.onClick.AddListener(ScrollUp);
        if (scrollDownButton != null) scrollDownButton.onClick.AddListener(ScrollDown);
        if (backButton != null) backButton.onClick.AddListener(ShowListView);

        if (sliderSwitch != null)
        {
            sliderSwitch.OnSwitchToggled += HandleSwitchToggled;
            // Align initial layout
            HandleSwitchToggled(sliderSwitch.isOnRightSide);
        }
        else
        {
            CloseWarbook();
        }
    }

    void OnDestroy()
    {
        if (sliderSwitch != null)
        {
            sliderSwitch.OnSwitchToggled -= HandleSwitchToggled;
        }
    }

    void HandleSwitchToggled(bool isOnRight)
    {
        // Assuming sliding to the right activates the Warbook
        if (isOnRight)
        {
            OpenWarbook();
        }
        else
        {
            CloseWarbook();
        }
    }

    public void OpenWarbook()
    {
        _isWarbookActive = true;
        Time.timeScale = 0f; // Pauses gameplay simulation

        if (targetInfoScreen != null) targetInfoScreen.SetActive(false);
        if (warbookMasterPanel != null) warbookMasterPanel.SetActive(true);

        ShowListView();
    }

    public void CloseWarbook()
    {
        _isWarbookActive = false;
        Time.timeScale = 1f; // Resumes gameplay simulation

        if (warbookMasterPanel != null) warbookMasterPanel.SetActive(false);
        // Note: targetInfoScreen visibility returns to normal radar selection behavior
    }

    public void ShowListView()
    {
        if (listSubPanel != null) listSubPanel.SetActive(true);
        if (detailSubPanel != null) detailSubPanel.SetActive(false);

        RefreshListUI();
    }

    public void ShowDetailView(WarbookEntry entry)
    {
        if (listSubPanel != null) listSubPanel.SetActive(false);
        if (detailSubPanel != null) detailSubPanel.SetActive(true);

        if (detailTitleText != null) detailTitleText.text = entry.title;
        if (detailBodyText != null) detailBodyText.text = entry.description;
    }

    void RefreshListUI()
    {
        int totalRows = listRowButtons.Length;

        for (int i = 0; i < totalRows; i++)
        {
            int entryIndex = _topVisibleIndex + i;

            if (entryIndex < database.Count)
            {
                listRowButtons[i].gameObject.SetActive(true);
                if (listRowTexts[i] != null)
                {
                    listRowTexts[i].text = database[entryIndex].title;
                }
            }
            else
            {
                listRowButtons[i].gameObject.SetActive(false);
            }
        }

        // Enable or disable scroll arrows based on list bounds
        if (scrollUpButton != null)
            scrollUpButton.interactable = _topVisibleIndex > 0;

        if (scrollDownButton != null)
            scrollDownButton.interactable = (_topVisibleIndex + totalRows) < database.Count;
    }

    public void ScrollUp()
    {
        if (_topVisibleIndex > 0)
        {
            _topVisibleIndex--;
            RefreshListUI();
        }
    }

    public void ScrollDown()
    {
        if (_topVisibleIndex + listRowButtons.Length < database.Count)
        {
            _topVisibleIndex++;
            RefreshListUI();
        }
    }

    void OnRowClicked(int slotIndex)
    {
        int actualIndex = _topVisibleIndex + slotIndex;
        if (actualIndex >= 0 && actualIndex < database.Count)
        {
            ShowDetailView(database[actualIndex]);
        }
    }
}
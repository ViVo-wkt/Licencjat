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
    public GameObject targetInfoScreen;
    public GameObject warbookMasterPanel;
    public GameObject listSubPanel;
    public GameObject detailSubPanel;

    [Header("List View Elements")]
    public Button[] listRowButtons;
    public TMP_Text[] listRowTexts;

    [Header("Detail View Elements")]
    public TMP_Text detailTitleText;
    public TMP_Text detailBodyText;
    public Button backButton;

    [Header("Radar UI Link")]
    public RadarUIManager radarUIManager;

    [Header("Database")]
    public List<WarbookEntry> database = new List<WarbookEntry>()
    {
        new WarbookEntry { 
            title = "SARH MISSILE", 
            description = "SEMI-ACTIVE RADAR HOMING\n\nRelies on continuous radar illumination from the station. Highly effective against medium-range targets but requires continuous beam tracking, else it loses focus. Only one missile can be guided at a time." 
        },
        new WarbookEntry { 
            title = "ARH MISSILE", 
            description = "ACTIVE RADAR HOMING\n\nCarries an onboard seeker head for terminal guidance. Once launched toward the designated target bearing, it operates autonomously. The seeker cone is narrow, so the target must be within the optical reticle for successful acquisition." 
        },
        new WarbookEntry { 
            title = "AUTO MISSILE", 
            description = "INFRARED SHORT-RANGE\n\nPassive thermal detection intended for close-in interception. Rapidly tracks targets acquired within the optical targeting reticle. Not modeled realistically in-game, the X/Y aiming axis mechanic aims to bring more variety to the gameplay." 
        },
        new WarbookEntry { 
            title = "BEAM RADAR", 
            description = "DIRECTIONAL ILLUMINATOR\n\nHigh-power, narrow-cone antenna manually steered to designate hostiles and guide semi-active munitions. Provides precise range and bearing data but only in a narrow field of view. Requires operator input to maintain target lock." 
        },
        new WarbookEntry { 
            title = "SWEEP RADAR", 
            description = "SURVEILLANCE RADAR\n\nContinuously rotating search beam providing early warning and periodic positional updates of airspace contacts. Doesn't require operator input, but lacks the precision of a directional illuminator. Only updates data periodically, so fast-moving targets may be missed between sweeps."
        },
        new WarbookEntry { 
            title = "DRONE", 
            description = "LOW-ALTITUDE LOITERING MUNITION\n\nSlow-moving, small radar cross-section. Not a particularly dangerous threat on its own, but can be used to distract or overwhelm defenses, when used in large numbers. Multiple impacts are required to destroy the base." 
        },
        new WarbookEntry { 
            title = "CRUISE MISSILE", 
            description = "STAND-OFF STRIKE WEAPON\n\nHigh-speed, low-altitude ingress designed to evade detection. Requires rapid interception upon identification, as it can reach the base in seconds. Can be launched from a variety of platforms, including aircraft and ships." 
        },
        new WarbookEntry { 
            title = "AIRLINER", 
            description = "CIVILIAN FLIGHT TRAFFIC\n\nNon-combatant air traffic maintaining steady speed and altitude along predetermined corridors. Destruction results in penalties, represented in-game by a hitpoint reduction to the base." 
        },
        new WarbookEntry { 
            title = "FIGHTER-BOMBER", 
            description = "FAST ATTACK CRAFT\n\nHeavily armed hostile platform navigating directly toward base infrastructure. Employs speed and altitude variations, as well as maneuvering techniques, aimed to evade incoming fire. A single bomb load is sufficient to destroy the base, so interception is critical." 
        }
    };

    void Start()
    {
        for (int i = 0; i < listRowButtons.Length; i++)
        {
            int slotIndex = i;
            if (listRowButtons[i] != null)
            {
                listRowButtons[i].onClick.AddListener(() => OnRowClicked(slotIndex));
            }
        }

        if (backButton != null) backButton.onClick.AddListener(ShowListView);

        if (sliderSwitch != null)
        {
            sliderSwitch.OnSwitchToggled += HandleSwitchToggled;
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

        Time.timeScale = 1f;
    }

    void HandleSwitchToggled(bool isOnRight)
    {
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
        Time.timeScale = 0f;

        if (radarUIManager != null)
        {
            radarUIManager.HideForWarbook();
        }
        else if (targetInfoScreen != null)
        {
            targetInfoScreen.SetActive(false);
        }

        if (warbookMasterPanel != null) warbookMasterPanel.SetActive(true);

        ShowListView();
    }

    public void CloseWarbook()
    {
        Time.timeScale = 1f;

        if (warbookMasterPanel != null) warbookMasterPanel.SetActive(false);

        if (radarUIManager != null)
        {
            radarUIManager.RestoreFromWarbook();
        }
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
        for (int i = 0; i < listRowButtons.Length; i++)
        {
            if (i < database.Count)
            {
                listRowButtons[i].gameObject.SetActive(true);
                if (listRowTexts[i] != null)
                {
                    listRowTexts[i].text = database[i].title;
                }
            }
            else
            {
                listRowButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnRowClicked(int index)
    {
        if (index >= 0 && index < database.Count)
        {
            ShowDetailView(database[index]);
        }
    }
}
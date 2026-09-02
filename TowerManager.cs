using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TowerManager : MonoBehaviour
{
    // references to my tower prefabs that can be bought and placed
    [Header("Towers")]
    [SerializeField] private GameObject pistolTower;
    [SerializeField] private GameObject sniperTower;
    [SerializeField] private GameObject shotgunTower;

    [SerializeField] private LayerMask towerLayer;

    // ui panel and text elements for showing stats when a tower is clicked
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI towerName;
    [SerializeField] private TextMeshProUGUI towerLevel;
    [SerializeField] private TextMeshProUGUI UpgradeCost;
    [SerializeField] private TextMeshProUGUI towerTargetting;

    private GameObject selectedTower;
    private GameObject placingTower;

    void Start()
    {
        // hiding my upgrade/info panel right when the game starts
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        // canceling tower placement if i press the escape key
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClearSelected();
        }

        // clearing the placing tower variable once it finishes locking into place
        if (placingTower)
        {
            if (!placingTower.GetComponent<TowerPlacement>().isPlacing)
            {
                placingTower = null;
            }
        }

        // handling mouse clicks to select or deselect towers on the map
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // stopping world selection clicks if my mouse is hovering over ui buttons
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            // ignoring selection raycasts while i am currently placing a new tower
            if (placingTower == null)
            {
                Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
                Vector3 worldPos3D = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                Vector2 worldPos2D = new Vector2(worldPos3D.x, worldPos3D.y);

                RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero, Mathf.Infinity, towerLayer);

                // if i hit a tower, select it, turn on its range circle, and fill out the ui text
                if (hit.collider != null)
                {
                    DeselectCurrentTower();
                    selectedTower = hit.collider.gameObject;
                    SetRangeSpriteState(selectedTower, true);

                    if (panel != null) panel.SetActive(true);
                    if (towerName != null) towerName.text = selectedTower.name.Replace("(Clone)", "").Trim();

                    TowerUpgrades upgradesComp = selectedTower.GetComponent<TowerUpgrades>();
                    if (upgradesComp != null)
                    {
                        if (towerLevel != null) towerLevel.text = "Tower LVL: " + upgradesComp.currentlevel.ToString();
                        if (UpgradeCost != null) UpgradeCost.text = upgradesComp.currentCost;
                    }

                    Tower towerComp = selectedTower.GetComponent<Tower>();
                    if (towerComp != null && towerTargetting != null)
                    {
                        if (towerComp.first)
                        {
                            towerTargetting.text = "First";
                        }
                        else if (towerComp.last)
                        {
                            towerTargetting.text = "Last";
                        }
                        else if (towerComp.strong)
                        {
                            towerTargetting.text = "Strong";
                        }
                    }
                }
                else
                {
                    // clicking empty space deselects whatever tower i had active
                    DeselectCurrentTower();
                }
            }
        }

        // selling the selected tower if i press the 'i' key
        if (Keyboard.current.iKey.wasPressedThisFrame && selectedTower != null)
        {
            SellSelectedTower();
        }
    }

    // triggered by shop buttons to spawn a new tower following my mouse
    public void setTower(GameObject towerPrefab)
    {
        ClearSelected();
        DeselectCurrentTower();

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mousePos.z = 0;

        placingTower = Instantiate(towerPrefab, mousePos, Quaternion.identity);
    }

    // upgrading my currently selected tower and refreshing the ui text
    public void UpgradeSelected()
    {
        if (selectedTower)
        {
            selectedTower.GetComponent<TowerUpgrades>().Upgrade();
            if (towerLevel != null) towerLevel.text = "Tower LVL: " + selectedTower.GetComponent<TowerUpgrades>().currentlevel.ToString();
            if (UpgradeCost != null) UpgradeCost.text = selectedTower.GetComponent<TowerUpgrades>().currentCost;
        }
    }

    // cycling through targeting modes (first -> last -> strong -> first) when clicking the button
    public void ChangeTargetting()
    {
        if (selectedTower)
        {
            Tower tower = selectedTower.GetComponent<Tower>();

            if (tower.first)
            {
                tower.first = false;
                tower.last = true;
                tower.strong = false;
                if (towerTargetting != null) towerTargetting.text = "Last";
            }
            else if (tower.last)
            {
                tower.first = false;
                tower.last = false;
                tower.strong = true;
                if (towerTargetting != null) towerTargetting.text = "Strong";
            }
            else if (tower.strong)
            {
                tower.first = true;
                tower.last = false;
                tower.strong = false;
                if (towerTargetting != null) towerTargetting.text = "First";
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && selectedTower)
        {
            panel.SetActive(false);
        }
    }

    // destroying a tower that is still being placed if cancelled
    private void ClearSelected()
    {
        if (placingTower)
        {
            Destroy(placingTower);
            placingTower = null;
        }
    }

    // turning off range visuals and hiding the info panel when deselecting
    private void DeselectCurrentTower()
    {
        if (selectedTower != null)
        {
            SetRangeSpriteState(selectedTower, false);
            selectedTower = null;
            if (panel != null) panel.SetActive(false);
        }
    }

    // deleting the selected tower from the game world completely
    private void SellSelectedTower()
    {
        Destroy(selectedTower);
        selectedTower = null;
        if (panel != null) panel.SetActive(false);
    }

    // helper function to turn the range circle sprite on or off for a tower object
    private void SetRangeSpriteState(GameObject towerObj, bool enabled)
    {
        if (towerObj == null) return;

        Transform rangeTransform = towerObj.transform.Find("Range");
        if (rangeTransform != null && rangeTransform.GetComponent<SpriteRenderer>() != null)
        {
            rangeTransform.GetComponent<SpriteRenderer>().enabled = enabled;
        }
    }
}
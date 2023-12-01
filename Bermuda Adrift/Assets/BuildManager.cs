using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static event Action<Tower> OnTowerPicked;
    public static event Action<BarrierScriptable> OnBarrierPicked;
    public static event Action OnTwoTowersPlaced;

    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;

    private int selectedTower = 0;  //Not sure what this is for - JD

    private GameManager gameManager;
    private GameObject mostRecent;
    private Vector3[] positions;
    private int activeIndex;
    private float towerRange;

    bool twoTowersPlacedEventCalled;

    [SerializeField] private Tower[] towers;
    [SerializeField] private BarrierScriptable[] barriers;

    public GameObject GetSelectedTower() { 
        return towerPrefabs[selectedTower];
    }

    public void placeTower(Tower scriptable)
    {
        if (gameManager.cost(scriptable.getCost()) && recentWasPlaced())
        {
            OnTowerPicked?.Invoke(scriptable);
            mostRecent = Instantiate(towerPrefabs[0]);

            mostRecent.SendMessage("place", scriptable);
            if (scriptable.getRange() > 0)
                towerRange = scriptable.getRange();
            else
                towerRange = 1;

            StartCoroutine(positionTracker());
        }
    }

    public void placeBarrier(BarrierScriptable scriptable)
    {
        if (gameManager.cost(scriptable.getCost()) && recentWasPlaced())
        {
            OnBarrierPicked?.Invoke(scriptable);
            mostRecent = Instantiate(towerPrefabs[1]);
            mostRecent.SendMessage("setBarrier", scriptable);

            StartCoroutine(positionTracker());
        }
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        positions = new Vector3[48];

        instantiateBuyables();
    }

    private void instantiateBuyables()
    {
        //Dividing + vectors to make the menu consistent with different resolutions
        float xPadding = (3 + 21.33f) / (21.33f * 2);
        float yPadding = (3 + 12) / 24f;
        float initialX = (17 + 21.33f) / (21.33f * 2);
        float initialY = (5 + 12) / 24f;
        Vector3 initial = Camera.main.ViewportToWorldPoint(new Vector3(initialX, initialY));
        Vector3 padding = Camera.main.ViewportToWorldPoint(new Vector3(xPadding, yPadding));

        int xMult = 0;
        int yMult = 0;
        foreach (Tower t in towers)
        {
            GameObject tower = new GameObject();
            Button button = tower.AddComponent<Button>();
            Image image = tower.AddComponent<Image>();
            ButtonHover buttonHover = tower.AddComponent<ButtonHover>();
            ButtonDescription buttonDescription = tower.AddComponent<ButtonDescription>();
            buttonDescription.SendMessage("setTower", t);
            buttonHover.tower = t;
            image.sprite = t.getImage();
            button.targetGraphic = image;
            tower.transform.parent = this.transform;
            button.onClick.AddListener(() => TowerButtonClick(t));

            if (xMult % 2 == 0)
            {
                tower.transform.position = new Vector3(initial.x, initial.y - padding.y * yMult, transform.position.z);
            }
            else
            {
                tower.transform.position = new Vector3(initial.x + padding.x, initial.y - padding.y * yMult, transform.position.z);
                yMult++;
            }

            tower.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            xMult++;
        }

        foreach (BarrierScriptable b in barriers)
        {
            GameObject barrier = new GameObject();
            Button button = barrier.AddComponent<Button>();
            Image image = barrier.AddComponent<Image>();
            ButtonHover buttonHover = barrier.AddComponent<ButtonHover>();
            buttonHover.barrier = b;
            ButtonDescription buttonDescription = barrier.AddComponent<ButtonDescription>();
            buttonDescription.SendMessage("setBarrier", b);
            image.sprite = b.getStartingSprite();
            button.targetGraphic = image;
            barrier.transform.parent = this.transform;
            button.onClick.AddListener(() => TowerButtonClick(b));

            if (xMult % 2 == 0)
            {
                barrier.transform.position = new Vector3(initial.x, initial.y - padding.y * yMult, transform.position.z);
            }
            else
            {
                barrier.transform.position = new Vector3(initial.x + padding.x, initial.y - padding.y * yMult, transform.position.z);
                yMult++;
            }

            barrier.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            xMult++;
        }
    }

    private void TowerButtonClick(Tower t)
    {
        placeTower(t);
    }

    private void TowerButtonClick(BarrierScriptable b)
    {
        placeBarrier(b);
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown("5")) { PlaceLightningRod(); }
    }

    #region Tower/Barrier Placements
    public void PlaceMachineGun()
    {
        placeTower(towers[1]);
    }

    public void PlaceTricannon()
    {
        placeTower(towers[0]);
    }
    public void PlaceLightningRod() { placeTower(towers[2]); }

    public void PlaceBarrier()
    {
        placeBarrier(barriers[0]);
    }

    public void PlaceFishNet()
    {
        placeBarrier(barriers[1]);
    }
    #endregion
    */

    private bool recentWasPlaced()  //Checks if previous tower selected was placed. If cancelled, both will be null and it will return true
    {
        if (mostRecent == null)
            return true;

        if (mostRecent.GetComponent<TowerAI>() != null)
            return mostRecent.GetComponent<TowerAI>().getPlaced();
        else if (mostRecent.GetComponent<Barriers>())
            return mostRecent.GetComponent<Barriers>().getPlaced();
        return true;
    }

    private IEnumerator positionTracker()
    {
        int i = 0;
        for (; i < positions.Length; i++)   //Find next spot to insert a position
        {
            if (positions[i] == Vector3.zero)
                break;
        }

        activeIndex = i;

        while (mostRecent != null && (mostRecent.GetComponent<TowerAI>() != null && !mostRecent.GetComponent<TowerAI>().getPlaced() || mostRecent.GetComponent<Barriers>() != null && !mostRecent.GetComponent<Barriers>().getPlaced()))
        {
            positions[i] = mostRecent.transform.position;
            yield return new WaitForEndOfFrame();
        }

        if (!twoTowersPlacedEventCalled && positions[1] != Vector3.zero)
        {
            twoTowersPlacedEventCalled = true;
            OnTwoTowersPlaced?.Invoke();
        }

        if (mostRecent == null)
            positions[i] = Vector3.zero;
    }

    public bool approvePosition(Vector3 position)
    {
        Vector3 temp = Vector3.zero;
        for (int i = 0; i < activeIndex; i++)
        {
            if (positions[i] == Vector3.zero) { return true; }

            temp.x = position.x - positions[i].x;
            temp.y = position.y - positions[i].y;
            if (temp.magnitude < 2 ) {
                //Debug.Log("Denied - too close to tower " + i);
                return false; 
            }
        }

        return true;
    }

    public Vector3[] getPositions() { return positions; }

    public void removePosition(Vector3 position)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (position == positions[i])
            {
                positions[i] = Vector3.zero;
                return;
            }
        }
    }
    public float getTowerRange() { return towerRange; }
}

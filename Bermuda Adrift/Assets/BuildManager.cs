using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static event Action<Tower> OnTowerPicked;
    public static event Action<BarrierScriptable> OnBarrierPicked;
    public static event Action TooManyBlueprints;

    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;

    private GameManager gameManager;
    private GameObject mostRecent;
    private int mostRecentInt;
    private ScriptableObject mostRecentSO;

    private Vector3[] positions;
    private int activeIndex;
    private float towerRange;

    private List<(int, ScriptableObject)> placeables;

    public void placeTower(Tower scriptable)
    {
        if (gameManager.cost(scriptable.getCost()) && recentWasPlaced())
        {
            OnTowerPicked?.Invoke(scriptable);
            mostRecent = Instantiate(towerPrefabs[0]);
            mostRecentInt = 0;
            mostRecentSO = scriptable;

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
            mostRecentInt = 1;
            mostRecentSO = scriptable;

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
        placeables = new List<(int, ScriptableObject)>();

        clearBuyables();
    }

    private void instantiateBuyables()
    {
        //Dividing + vectors to make the menu consistent with different resolutions
        float xPadding = (2.8f + 21.33f) / (21.33f * 2);
        float yPadding = (3 + 12) / 24f;
        float initialX = (17.1f + 21.33f + transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition.x - 16.09f) / (21.33f * 2);
        float initialY = (5 + 12) / 24f;
        Vector3 initial = Camera.main.ViewportToWorldPoint(new Vector3(initialX, initialY));
        Vector3 padding = Camera.main.ViewportToWorldPoint(new Vector3(xPadding, yPadding));

        int xMult = 0;
        int yMult = 0;
        foreach ((int,ScriptableObject) x in placeables)
        {
            if (x.Item1 == 0)   //0 if a tower
            {
                GameObject tower = new GameObject();
                Button button = tower.AddComponent<Button>();
                Image image = tower.AddComponent<Image>();
                ButtonHover buttonHover = tower.AddComponent<ButtonHover>();
                ButtonDescription buttonDescription = tower.AddComponent<ButtonDescription>();
                buttonDescription.SendMessage("setTower", (Tower) x.Item2);
                buttonHover.tower = (Tower) x.Item2;
                image.sprite = ((Tower)x.Item2).getImage();
                button.targetGraphic = image;
                tower.transform.parent = this.transform;
                button.onClick.AddListener(() => TowerButtonClick(((Tower)x.Item2)));

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
            else if (x.Item1 == 1)  //1 if a barrier
            {
                GameObject barrier = new GameObject();
                Button button = barrier.AddComponent<Button>();
                Image image = barrier.AddComponent<Image>();
                ButtonHover buttonHover = barrier.AddComponent<ButtonHover>();
                buttonHover.barrier = (BarrierScriptable) x.Item2;
                ButtonDescription buttonDescription = barrier.AddComponent<ButtonDescription>();
                buttonDescription.SendMessage("setBarrier", ((BarrierScriptable)x.Item2));
                image.sprite = ((BarrierScriptable)x.Item2).getStartingSprite();
                button.targetGraphic = image;
                barrier.transform.parent = this.transform;
                button.onClick.AddListener(() => TowerButtonClick(((BarrierScriptable)x.Item2)));

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

        /*
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
        */
    }
    private void clearBuyables()
    {
        int max = transform.childCount;
        for (int i = 0; i < max; i++) { Destroy(transform.GetChild(i).gameObject); }
    }
    public void reloadBuyables()
    {
        clearBuyables();
        instantiateBuyables();
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
    public void addToList((int, ScriptableObject) script) 
    {
        if (placeables.Capacity >= 10) return;

        if (placeables.Capacity > 7)
            TooManyBlueprints?.Invoke();

        placeables.Add(script); 
        reloadBuyables();
    }
    public void removeFromList((int, ScriptableObject) script) { placeables.Remove(script); reloadBuyables(); }

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

        if (mostRecent == null)
            positions[i] = Vector3.zero;

        if (mostRecent != null)
        {
            removeFromList((mostRecentInt, mostRecentSO));
        }
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
    public int blueprintNumber() { return placeables.Capacity; }
}

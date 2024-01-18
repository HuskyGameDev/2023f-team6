using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlaceableData
{
    TowerAI towerData;
    Tower towerScriptable;
    Barriers barrierData;
    BarrierScriptable barrierScriptable;
    Vector3 location;
    int upgradeLevel;
    string saveString;

    public PlaceableData(GameObject go)
    {
        //If the component isn't there, it'll return null, which is what we want
        towerData = go.GetComponent<TowerAI>();
        barrierData = go.GetComponent<Barriers>();
        location = go.transform.position;
        upgradeLevel = 0;

        if (towerData != null)
        {
            saveString = towerData.getTower().getSaveString();
        }
        else
        {
            saveString = barrierData.getBarrier().getSaveString();
        }
    }

    public PlaceableData(ScriptableObject go, Vector3 loc, int upgrade)
    {
        location = loc;
        upgradeLevel = upgrade;

        try
        {
            towerScriptable = (Tower) go;
            saveString = towerScriptable.getSaveString();
        }
        catch (Exception e)
        {
            barrierScriptable = (BarrierScriptable) go;
            saveString = barrierScriptable.getSaveString();
        }

    }

    public Type getPlaceableType()
    {
        if (towerScriptable != null)
            return typeof(TowerAI);
        else if (barrierScriptable != null)
            return typeof(Barriers);
        else if (towerData != null)
        {
            upgradeLevel = towerData.getUpgradeLevel();
            return towerData.GetType();
        }
        else
            return barrierData.GetType();
    }

    public bool hasScriptable() { return towerScriptable != null || barrierScriptable != null; }
    public Tower getTowerScriptable() { return towerScriptable; }
    public BarrierScriptable getBarrierScriptable() { return barrierScriptable; }

    public object getPlaceableData()
    {
        if (towerData != null)
            return towerData;
        else
            return barrierData;
    }
    public int getUpgradeLevel() 
    {
        if (towerData == null)
            return 0;

        upgradeLevel = towerData.getUpgradeLevel();
        return upgradeLevel;
    }
    public Vector3 getLocation() { return location; }
    public string getSaveString() { return saveString; }
    //public void setLocation(Vector3 location) { this.location = location; }
}
public class BuildManager : MonoBehaviour
{
    public static event Action<Tower> OnTowerPicked;
    public static event Action<BarrierScriptable> OnBarrierPicked;
    public static event Action TooManyBlueprints;

    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;
    [SerializeField] private BarrierScriptable barrier1;
    [SerializeField] private BarrierScriptable barrier2;

    private GameManager gameManager;
    private GameObject mostRecent;
    private Tower mostRecentTower;

    private float towerRange;

    private List<Tower> placeables;
    private List<PlaceableData> placeableDatas;

    #region Setup stuff
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        placeables = new List<Tower>();
        placeableDatas = new List<PlaceableData>();
    }
    void Start()
    {
        clearBuyables();
        reloadBuyables();
    }
    private void OnEnable()
    {
        TowerAI.OnTowerPlacedBM += addPosition;
        Barriers.OnTowerPlacedBM += addPosition;
    }
    private void OnDisable()
    {
        TowerAI.OnTowerPlacedBM -= addPosition;
        Barriers.OnTowerPlacedBM -= addPosition;
    }
    #endregion

    #region Inventory system
    private void instantiateBuyables()  //Creates the buyable tower buttons
    {
        //Dividing + vectors to make the menu consistent with different resolutions
        float xPadding = (3f + 21.33f) / (21.33f * 2);
        float yPadding = (2.9f + 12) / 24f;
        float initialX = (17.3f + 21.33f + transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition.x - 16.09f) / (21.33f * 2);
        float initialY = (3.6f + 12) / 24f;
        Vector3 initial = Camera.main.ViewportToWorldPoint(new Vector3(initialX, initialY));
        Vector3 padding = Camera.main.ViewportToWorldPoint(new Vector3(xPadding, yPadding));

        int xMult = 0;
        int yMult = 0;

        GameObject barrier1Button = new GameObject();
        Button B1button = barrier1Button.AddComponent<Button>();
        Image B1image = barrier1Button.AddComponent<Image>();
        ButtonHover B1buttonHover = barrier1Button.AddComponent<ButtonHover>();
        B1buttonHover.barrier = barrier1;
        ButtonDescription B1buttonDescription = barrier1Button.AddComponent<ButtonDescription>();
        B1buttonDescription.SendMessage("setBarrier", barrier1); ;
        B1image.sprite = barrier1.getThumbnail();
        B1button.targetGraphic = B1image;
        barrier1Button.transform.parent = transform;
        B1button.onClick.AddListener(() => TowerButtonClick(barrier1));
        barrier1Button.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        barrier1Button.transform.position = new Vector3(initial.x, initial.y + padding.y * 1, transform.position.z);

        if (barrier2 != null)
        {
            GameObject barrier2Button = new GameObject();
            Button B2button = barrier2Button.AddComponent<Button>();
            Image B2image = barrier2Button.AddComponent<Image>();
            ButtonHover B2buttonHover = barrier2Button.AddComponent<ButtonHover>();
            B2buttonHover.barrier = barrier2;
            ButtonDescription B2buttonDescription = barrier2Button.AddComponent<ButtonDescription>();
            B2buttonDescription.SendMessage("setBarrier", barrier2); ;
            B2image.sprite = barrier2.getThumbnail();
            B2button.targetGraphic = B2image;
            barrier2Button.transform.parent = transform;
            B2button.onClick.AddListener(() => TowerButtonClick(barrier2));
            barrier2Button.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            barrier2Button.transform.position = new Vector3(initial.x + padding.x * 1, initial.y + padding.y * 1, transform.position.z);
        }

        foreach (Tower x in placeables)
        {
            GameObject tower = new GameObject();
            Button button = tower.AddComponent<Button>();
            Image image = tower.AddComponent<Image>();
            ButtonHover buttonHover = tower.AddComponent<ButtonHover>();
            ButtonDescription buttonDescription = tower.AddComponent<ButtonDescription>();
            buttonDescription.SendMessage("setTower", x);
            buttonHover.tower = x;
            image.sprite = x.getImage();
            button.targetGraphic = image;
            tower.transform.parent = transform;
            button.onClick.AddListener(() => TowerButtonClick(x));

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
            /*
                GameObject barrier = new GameObject();
                Button button = barrier.AddComponent<Button>();
                Image image = barrier.AddComponent<Image>();
                ButtonHover buttonHover = barrier.AddComponent<ButtonHover>();
                buttonHover.barrier = (BarrierScriptable) x.Item2;
                ButtonDescription buttonDescription = barrier.AddComponent<ButtonDescription>();
                buttonDescription.SendMessage("setBarrier", ((BarrierScriptable)x.Item2));
                image.sprite = ((BarrierScriptable)x.Item2).getThumbnail();
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
            */
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
    private void clearBuyables()    //Deletes all buyable tower buttons
    {
        int max = transform.childCount;
        for (int i = 0; i < max; i++) { Destroy(transform.GetChild(i).gameObject); }
    }
    public void reloadBuyables()    //Deletes all buttons and reinstantiates them
    {
        clearBuyables();
        instantiateBuyables();
    }
    public void addToList(Tower script)   //Add an option to the list available. 0 for tower, 1 for barrier
    {
        if (placeables.Count >= 10) return;

        if (placeables.Count > 7)
            TooManyBlueprints?.Invoke();

        placeables.Add(script);
        reloadBuyables();
    }
    public void removeFromList(Tower script) { placeables.Remove(script); reloadBuyables(); } //Remove a buyable button from the list and reload all buttons
    #endregion

    #region Tower/Barrier placing
    public void placeTower(Tower scriptable)    //Creates a tower with the given tower scriptable
    {
        if (gameManager.cost(scriptable.getCost()) && recentWasPlaced())
        {
            OnTowerPicked?.Invoke(scriptable);

            mostRecent = Instantiate(towerPrefabs[0]);
            mostRecentTower = scriptable;

            mostRecent.SendMessage("place", scriptable);
            if (scriptable.getRange() > 0)
                towerRange = scriptable.getRange();
            else
                towerRange = 1;

            //StartCoroutine(positionTracker());
        }
    }
    public void placeBarrier(BarrierScriptable scriptable)  //Creates a barrier with the given barrier scriptable
    {
        if (gameManager.cost(scriptable.getCost()) && recentWasPlaced())
        {
            OnBarrierPicked?.Invoke(scriptable);

            mostRecent = Instantiate(towerPrefabs[1]);
            mostRecentTower = null;

            mostRecent.SendMessage("setBarrier", scriptable);

            //StartCoroutine(positionTracker());
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

    public bool approvePosition(Vector3 position, int type) //0 for tower, 1 for barrier
    {
        Vector3 temp = Vector3.zero;
        foreach (PlaceableData pd in placeableDatas)
        {
            if (pd.getLocation() == Vector3.zero) { return true; }

            temp.x = position.x - pd.getLocation().x;
            temp.y = position.y - pd.getLocation().y;
            if (temp.magnitude < 2)
            {
                return false;
            }
        }

        if (type == 1)
            return raycastCornersBarrier(position);
        return raycastCornersTower(position);
    }
    public bool approvePosition(GameObject go)  //Goes through the positions of all other towers and checks for overlaps or being out of bounds
    {
        Vector3 temp = Vector3.zero;
        foreach (PlaceableData pd in placeableDatas)
        {
            if (pd.getLocation() == Vector3.zero) { return true; }

            temp.x = go.transform.position.x - pd.getLocation().x;
            temp.y = go.transform.position.y - pd.getLocation().y;
            if (temp.magnitude < 2)
            {
                return false;
            }
        }

        if (go.GetComponent<Barriers>() != null)
            return raycastCornersBarrier(go.transform.position);
        else
            return raycastCornersTower(go.transform.position);
    }
    private bool raycastCornersTower(Vector3 position)   //Checks the corners of the tower to make sure it isn't touching the water or a channel
    {
        int waterMask = 1 << 4;
        int channelMask = 1 << 6;

        if (Physics2D.Raycast(position, Vector3.up + Vector3.right, 1.4f, waterMask).collider != null
            ||
            Physics2D.Raycast(position, Vector3.up + Vector3.right, 1.4f, channelMask).collider != null)    //Top right corner
            return false;

        if (Physics2D.Raycast(position, Vector3.down + Vector3.right, 1.4f, waterMask).collider != null
            ||
            Physics2D.Raycast(position, Vector3.down + Vector3.right, 1.4f, channelMask).collider != null)  //Down right corner
            return false;

        if (Physics2D.Raycast(position, Vector3.up + Vector3.left, 1.4f, waterMask).collider != null
            ||
            Physics2D.Raycast(position, Vector3.up + Vector3.left, 1.4f, channelMask).collider != null)  //Up left corner
            return false;

        if (Physics2D.Raycast(position, Vector3.down + Vector3.left, 1.4f, waterMask).collider != null
            ||
            Physics2D.Raycast(position, Vector3.down + Vector3.left, 1.4f, channelMask).collider != null)  //Down left corner
            return false;


        return true;
    }
    private bool raycastCornersBarrier(Vector3 position) //If in a channel, not touching the raft, and not 
    {
        int channelMask = 1 << 6;
        int raftMask = 1 << 3;
        int waterMask = 1 << 4;

        if (Physics2D.Raycast(position, Vector3.up + Vector3.right, 1.4f, channelMask).collider == null
            ||
            Physics2D.Raycast(position, Vector3.up + Vector3.right, 1.4f, raftMask).collider != null
            ||
            Physics2D.Raycast(position, Vector3.up + Vector3.right, 1.4f, waterMask).collider != null)    //Top right corner
            return false;

        if (Physics2D.Raycast(position, Vector3.down + Vector3.right, 1.4f, channelMask).collider == null
            ||
            Physics2D.Raycast(position, Vector3.down + Vector3.right, 1.4f, raftMask).collider != null
            ||
            Physics2D.Raycast(position, Vector3.down + Vector3.right, 1.4f, waterMask).collider != null)  //Down right corner
            return false;

        if (Physics2D.Raycast(position, Vector3.up + Vector3.left, 1.4f, channelMask).collider == null
            ||
            Physics2D.Raycast(position, Vector3.up + Vector3.left, 1.4f, raftMask).collider != null
            ||
            Physics2D.Raycast(position, Vector3.up + Vector3.left, 1.4f, waterMask).collider != null)  //Up left corner
            return false;

        if (Physics2D.Raycast(position, Vector3.down + Vector3.left, 1.4f, channelMask).collider == null
            ||
            Physics2D.Raycast(position, Vector3.down + Vector3.left, 1.4f, raftMask).collider != null
            ||
            Physics2D.Raycast(position, Vector3.down + Vector3.left, 1.4f, waterMask).collider != null)  //Down left corner
            return false;

        return true;
    }

    #region Loading buildings
    private void clearAllBuildings()    //Deletes all buildings (in preparation for loading in more)
    {
        TowerAI[] towers = FindObjectsOfType<TowerAI>();
        Barriers[] barriers = FindObjectsOfType<Barriers>();

        foreach(TowerAI t in towers)
            Destroy(t.gameObject);

        foreach (Barriers b in barriers)
            Destroy(b.gameObject);
    }
    public void loadPlaceables(List<PlaceableData> list)    //Loads and places all towers/barriers in the given list
    {
        clearAllBuildings();

        placeableDatas = list;
        GameObject currentBuilding;

        foreach (PlaceableData pd in list)
        {
            if (pd.getPlaceableType() == typeof(TowerAI))   //I think this should work
            {
                currentBuilding = Instantiate(towerPrefabs[0], pd.getLocation(), Quaternion.identity);

                if (pd.hasScriptable())
                {
                    currentBuilding.SendMessage("place", pd.getTowerScriptable());
                }
                else
                {
                    currentBuilding.SendMessage("place", ((TowerAI) pd.getPlaceableData()).getTower());
                }

                currentBuilding.SendMessage("placeTower");
                currentBuilding.SendMessage("setUpgrade", pd.getUpgradeLevel());
            }
            else
            {
                currentBuilding = Instantiate(towerPrefabs[1], pd.getLocation(), Quaternion.identity);

                if (pd.hasScriptable())
                {
                    currentBuilding.SendMessage("setBarrier",pd.getBarrierScriptable());
                }
                else
                {
                    currentBuilding.SendMessage("setBarrier", ((Barriers) pd.getPlaceableData()).getBarrier());
                }

                currentBuilding.SendMessage("placeBarrier");
                //No barrier upgrades yet
            }
        }
    }
    #endregion

    #region Randomly placed towers (temp towers)
    public Vector3 randomApprovedTowerPosition()    //Returns a random position that is a valid place for a tower to be put
    {
        Vector3 position = new Vector3(Random.Range(-7, 7), Random.Range(-7, 7));

        while (!approvePosition(position, 0))
        {
            Debug.Log("Rejected (" + position.x + ", " + position.y + ")");
            position.x = Random.Range(-7, 7);
            position.y = Random.Range(-7, 7);
        }
        return position;
    }
    public void placeRandom(Tower tower, float duration)    //Creates a temporary tower at a random position
    {
        Vector3 position = randomApprovedTowerPosition();

        GameObject setPositionTower = Instantiate(towerPrefabs[0], position, Quaternion.identity);
        setPositionTower.SendMessage("place", tower);
        setPositionTower.SendMessage("placeTower");

        if (duration != -1)
            StartCoroutine(tempTower(setPositionTower, duration));
    }
    private IEnumerator tempTower(GameObject g, float duration) //Deletes a temporary tower after a specified duration
    {
        yield return new WaitForSeconds(duration);

        g.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Destroy");
    }
    #endregion

    #endregion

    #region Placeable Data management
    private void printPlaceableDatas()
    {
        if (placeableDatas == null)
        {
            Debug.Log("PlaceableDatas is null");
            return;
        }

        string output = "";
        foreach(PlaceableData pd in placeableDatas)
        {
            output += pd.getPlaceableData() + ", ";
        }
    }
    public List<PlaceableData> GetPlaceableDatas() {
        printPlaceableDatas();
        return placeableDatas;
    }
    public void addPosition(GameObject go)
    {
        placeableDatas.Add(new PlaceableData(go));

        if (mostRecentTower != null)
            removeFromList(mostRecentTower);
    }
    public void removePosition(GameObject go)
    {
        object comparison;
        if (go.GetComponent<TowerAI>() != null)
            comparison = go.GetComponent<TowerAI>();
        else
            comparison = go.GetComponent<Barriers>();

        foreach (PlaceableData pd in placeableDatas)
        {
            if (pd.getPlaceableData() == comparison)
            {
                placeableDatas.Remove(pd);
                return;
            }
        }
    }
    public bool towerPlaced(string towerName)   //Returns true if a tower of the given name exists (only works for towers right now)
    {
        foreach (PlaceableData pd in placeableDatas)
        {
            if (pd.getPlaceableType() == typeof(TowerAI) && ((TowerAI)pd.getPlaceableData()).getTower().getName().CompareTo(towerName) == 0)
                return true;
        }

        return false;
    }
    public TowerAI getTowerOfType(string name)  //Returns a random tower with the given name
    {
        if (!towerPlaced(name)) { return null; } //If the tower doesn't exist, don't bother looking for it

        List<TowerAI> towers = new List<TowerAI>();

        foreach (PlaceableData pd in placeableDatas)
        {
            if (pd.getPlaceableType() == typeof(TowerAI) && ((TowerAI)pd.getPlaceableData()).getTower().getName().CompareTo(name) == 0)
                towers.Add((TowerAI)pd.getPlaceableData());
        }

        return towers[Random.Range(0, towers.Count)];
    }
    #endregion

    #region Get/Set Functions
    public TowerAI getRandomTower()
    {
        List<TowerAI> towers = new List<TowerAI>();

        foreach (PlaceableData pd in placeableDatas)
        {
            if (pd.getPlaceableType() == typeof(TowerAI))
                towers.Add( (TowerAI)pd.getPlaceableData() );
        }

        if (towers.Count <= 0)
            return null;
        else
            return towers[Random.Range(0, towers.Count)];
    }
    public float getTowerRange() { return towerRange; }
    public int blueprintNumber() { return placeables.Count; }
    public void setBarrier1(BarrierScriptable b) { barrier1 = b; }
    public void setBarrier2(BarrierScriptable b) { barrier2 = b; }
    public void clearBarrier2() { barrier2 = null; }
    #endregion
}

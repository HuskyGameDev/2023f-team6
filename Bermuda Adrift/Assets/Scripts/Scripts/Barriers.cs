using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Barriers : MonoBehaviour, IPointerDownHandler
{
    public static event Action OnCancel;
    public static event Action OnTowerPlaced;
    public static event Action<GameObject> OnTowerPlacedBM;
    public static event Action<Barriers> onClicked;

    private BarrierScriptable barrier; //Won't need to be serialized after the placing is set up
    private BuildManager buildManager;
    private int health;
    private float armor = 1;
    private Buffs[] debuffs;
    private Buffs debuffToInflict;
    private Animator animator;

    private bool placed = false;
    
    // Start is called before the first frame update
    void Start()
    {
        health = barrier.getHealth();
        debuffs = new Buffs[5];

        buildManager = FindObjectOfType<BuildManager>();
        AddPhysics2DRaycaster();
    }

    // Update is called once per frame
    void Update()
    {
        if (!placed) {
            if (Input.GetMouseButtonDown(0))
            {
                if ((Mathf.Abs(gameObject.transform.position.x) < 7 && Mathf.Abs(gameObject.transform.position.x) > 1 && (Mathf.Abs(gameObject.transform.position.y) < 1)    // Left/Right channels
                    ||
                    Mathf.Abs(gameObject.transform.position.y) < 7 && Mathf.Abs(gameObject.transform.position.y) > 1 && (Mathf.Abs(gameObject.transform.position.x) < 1))
                    &&
                    buildManager.approvePosition(gameObject))
                {
                    OnTowerPlaced?.Invoke();
                    OnTowerPlacedBM?.Invoke(gameObject);
                    placed = true;
                    GameObject.Find("Managers").GetComponent<GameManager>().spendScrap(barrier.getCost());
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                OnCancel?.Invoke();
                Destroy(gameObject);
            }

            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            gameObject.transform.position = new Vector3(Mathf.Round(mouseWorldPosition.x), Mathf.Round(mouseWorldPosition.y));

            Locate();

            if (GameObject.Find("Managers").GetComponent<GameManager>().getGameState() != GameManager.GameState.Idle)
            {
                OnCancel?.Invoke();
                Destroy(gameObject);
            }

            return;
        }
    }

    private void setBarrier(BarrierScriptable newBarrier)   //Sets up barrier. For now only sets the health and calls Locate()
    {
        barrier = newBarrier;
        health = barrier.getHealth();
        debuffToInflict = barrier.getDebuff();

        if (gameObject.GetComponent<Animator>() != null)
        {
            animator = gameObject.GetComponent<Animator>();
            animator.runtimeAnimatorController = barrier.getAnimator();
        }
        
        gameObject.GetComponent<SpriteRenderer>().sprite = barrier.getStartingSprite();
    }

    private void TakeDamage(int damage)
    {
        health -= (int) (damage * armor);
        if (health <= 0)
        {
            buildManager.removePosition(gameObject);
            Destroy(gameObject);
        }
    }
    private void repair(int damage)
    {
        health += damage;
    }
    public void destroyBarrier()
    {
        //Play destroyed animation
        buildManager.removePosition(gameObject);
        FindObjectOfType<GameManager>().addScrap(barrier.getCost() / 2);
        Destroy(gameObject);
    }

    private void Locate()   //Changes the animation based on which channel the barrier is in
    {
        if (animator == null) { return; }

        Vector2 movementVec = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        movementVec = movementVec.normalized;

        animator.SetFloat("x", movementVec.x);
        animator.SetFloat("y", movementVec.y);
    }

    private void addDebuff(Buffs debuff)    //Add a debuff to the list of debuffs
    {
        for (int i = 0; i < debuffs.Length; i++)
        {
            if (debuffs[i] == null)
            {
                debuffs[i] = debuff;
                return;
            }
        }
    }
    private void removeDebuff(Buffs debuff) //Removes a debuff from the list of debuffs currently applied
    {
        int i = 0;
        for (; i < debuffs.Length; i++)
        {
            if (debuffs[i] == debuff)
            {
                for (; i < debuffs.Length - 1; i++)
                {
                    debuffs[i] = debuffs[i + 1];
                }
                debuffs[i] = null;
            }
        }
    }
    private IEnumerator InflictDebuff(Buffs newDebuff)  //adds a debuff to the list
    {
        addDebuff(newDebuff);

        StartCoroutine(DOT(newDebuff.getDOT(), newDebuff.getDOTSpeed(), newDebuff.getDuration()));

        yield return new WaitForSeconds(newDebuff.getDuration());

        removeDebuff(newDebuff);
    }
    private IEnumerator DOT(int damage, float speed, float duration)    //Does [damage] damage every [speed] seconds for [duration] seconds
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration) //Even if no DOT, still waits until the end of the duration
        {
            if (damage > 0)
                TakeDamage(damage);

            yield return new WaitForSeconds(speed);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (placed && GameObject.FindObjectOfType<GameManager>().getGameState() == GameManager.GameState.Idle && barrier.getName().CompareTo("Barricade") != 0)
            onClicked?.Invoke(this);
    }

    private void AddPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }


    public BarrierScriptable.Effect getEffect() { return barrier.getEffect(); }
    public Buffs getDebuff() { return debuffToInflict; }
    public bool getPlaced() { return placed; }
    public int getHealth() { return health; }
    public int getMaxHealth() { return barrier.getHealth(); }
    public string getName() { return barrier.getName(); }
    public BarrierScriptable getBarrier() { return barrier; }
    private void placeBarrier() { placed = true; Locate(); }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    SpriteRenderer sR;
    int type;
    BuildManager buildManager;
    // Start is called before the first frame update
    void Start()
    {
        sR = gameObject.GetComponent<SpriteRenderer>();
        buildManager = FindObjectOfType<BuildManager>();
    }
    private void Update()
    {
        //Follow mouse in the grid
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        gameObject.transform.position = new Vector3(Mathf.Round(mouseWorldPosition.x), Mathf.Round(mouseWorldPosition.y));

        //Light up in in the right/wrong spot
        if (buildManager.approvePosition(transform.position, type)) { enterCorrect(); }
        else { enterIncorrect(); }

    }
    private void OnDisable()
    {
        //mouseExit();
    }
    private void enterCorrect()
    {
        sR.color = new Vector4(0, 1, 0, 0.5f);
    }

    private void enterIncorrect()
    {
        sR.color = new Vector4(1, 0, 0, 0.75f);
    }

    private void mouseExit()
    {
        sR.color = new Vector4(0, 0, 0, 0.75f);
    }
    public void setType(int type) { this.type = type; }
}

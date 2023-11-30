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
        if (type == 0)
        {
            if ((Mathf.Abs(transform.position.x) <= 6 && Mathf.Abs(transform.position.x) > 1) && (Mathf.Abs(transform.position.y) <= 6 && Mathf.Abs(transform.position.y) > 1) && buildManager.approvePosition(transform.position)) { enterCorrect(); }
            else { enterIncorrect(); }
        } 
        else
        {
            if (((Mathf.Abs(transform.position.x) <= 6 && Mathf.Abs(transform.position.x) > 1 && Mathf.Abs(transform.position.y) < 1)   //Left/Right channels
                || 
                (Mathf.Abs(transform.position.y) <= 6 && Mathf.Abs(transform.position.y) > 1) && Mathf.Abs(transform.position.x) < 1) 
                && 
                buildManager.approvePosition(transform.position)) { enterCorrect(); }
            else { enterIncorrect(); }
        }

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

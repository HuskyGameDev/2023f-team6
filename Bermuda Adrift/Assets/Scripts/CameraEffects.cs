using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    private Transform transform;
    private void Start()
    {
        transform = gameObject.transform;
    }
    

    public void cameraShake(float magnitude) { StartCoroutine(camShake(magnitude)); }   //0.25f seems to be a good magnitude
    private IEnumerator camShake(float magnitude)
    {
        transform.Translate(Vector3.left * magnitude);

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        transform.Translate(Vector3.right * magnitude * 2);

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        transform.Translate((Vector3.left * magnitude) + (Vector3.up * magnitude));

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        transform.Translate(Vector3.down * magnitude * 2);

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        transform.Translate(Vector3.up * magnitude);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public void cameraShake(float magnitude) { StartCoroutine(camShake(magnitude)); }   //0.25f seems to be a good magnitude. More magnitude = stronger shakes
    private IEnumerator camShake(float magnitude)   //Moves the camera left, right, up, and down (in that order) by magnitude then back to original position
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

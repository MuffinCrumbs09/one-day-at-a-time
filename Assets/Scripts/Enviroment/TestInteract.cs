using UnityEngine;

public class TestInteract : MonoBehaviour, IInteractable
{
    public GameObject target;
    bool lerp = false;
    float value = 1f;
    float curTime;
    public float timeToLerp;
    public void Interact()
    {
        if (lerp) return;
        lerp = true;
    }

    void Update()
    {
        if (!lerp) return;
        if (curTime <= timeToLerp)
        {
            curTime += Time.deltaTime;
            value = Mathf.Lerp(value, 0, curTime / timeToLerp);
        }

        target.GetComponent<SpriteRenderer>().material.SetFloat("_Mono", value);

        if (target.GetComponent<SpriteRenderer>().material.GetFloat("_Mono") == 0)
            Destroy(gameObject);
    }
}
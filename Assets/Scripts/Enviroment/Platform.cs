using UnityEngine;

public class Platform : MonoBehaviour
{
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        if (_player == null)
        {
            Debug.LogError("Couldn't Find PLayer");
            Destroy(this);
        }
    }

    private void Update()
    {
        if (_player.transform.position.y <= transform.position.y)
            GetComponent<Collider2D>().enabled = false;
        else
            GetComponent<Collider2D>().enabled = true;
    }
}

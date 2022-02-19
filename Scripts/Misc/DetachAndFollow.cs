using UnityEngine;

public class DetachAndFollow : MonoBehaviour
{
    private GameObject Anchor;

    void Awake()
    {
        Anchor = new GameObject();
        Anchor.name = "Anchor -> " + name;
        Anchor.transform.parent = transform.parent;
        Anchor.transform.position = transform.position;

        transform.parent = null;
    }

    void Update()
    {
        transform.position = Anchor.transform.position;
    }
}
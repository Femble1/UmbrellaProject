using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEffect : MonoBehaviour
{
    public Animated parent;
    // Start is called before the first frame update
    void OnDestroy()
    {
        parent.created_anims.Remove(gameObject);
    }
}

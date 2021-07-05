using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SlimeBomb : SlimeBase
{
    [Space(10f)]
    [Header("Expecific Attributes")]
    [SerializeField] private string _details;

    private void OnMouseDown() {
        Debug.Log("Click");
        CloneItSelf(this.gameObject, true);
    }
    private void OnMouseUp()
    {
        Debug.Log("Click");
        CloneItSelf(this.gameObject, true);

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{

    private static Mouse instance;

    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake()
    {
        instance = this; 
    } 

    private void Update() {
        transform.position = Mouse.GetPosition();
    }
    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
    } 

}

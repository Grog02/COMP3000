using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Crate : MonoBehaviour
{
    public static EventHandler OnAnyCrateDestroy;

    [SerializeField] private Transform destroyedCrate;

    private GridPosition gridPosition;

    private void Start() 
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);    
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    public void Damage()
    {
        // Change crate to destroyed crate 
        Transform crateDestroyedTransform = Instantiate(destroyedCrate, transform.position, transform.rotation);

        // "Explode" crate fragments 
        ExplosionCrateParts(crateDestroyedTransform, 150f, transform.position, 10f);
        Destroy(gameObject);

        OnAnyCrateDestroy?.Invoke(this, EventArgs.Empty);
    }

    private void ExplosionCrateParts(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if(child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }
            // "Explode" each segment of the crate
            ExplosionCrateParts(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}

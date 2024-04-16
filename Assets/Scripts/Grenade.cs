using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class Grenade : MonoBehaviour
{
    private Vector3 targetPosition;

    private Action OnGrenadeImpact;

    [SerializeField] private Transform grenadeExpolosion;

    [SerializeField] private TrailRenderer trailRenderer;

    [SerializeField] private AnimationCurve AnimationCurve;
    public static event EventHandler OnAnyGrenadeExplode;

    private float totalDistance;
    private Vector3 positionXZ;


    private void Update() 
    {
        Vector3 moveDirection = (targetPosition - positionXZ).normalized;
        float moveSpeed = 15f;
        positionXZ += moveDirection * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);



        float distanceNormalized = 1 - distance / totalDistance;
        float reachedTargetDistance = 0.2f;

        float maxHeight = totalDistance / 4f;

        float posY = AnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, posY, positionXZ.z);
        if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            float damageRadius = 4f;
            Collider[] colliders = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach (Collider collider in colliders)
            {
                if(collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(20);
                }
                if(collider.TryGetComponent<Crate>(out Crate crate))
                {
                    crate.Damage();
                }
            }

            OnAnyGrenadeExplode?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;
            Instantiate(grenadeExpolosion, targetPosition + Vector3.up * 1f, quaternion.identity);  
            Destroy(gameObject);

            OnGrenadeImpact();
        }
    }
    public void Setup(GridPosition targetGridPosition, Action OnGrenadeImpact)
    {
        
        this.OnGrenadeImpact = OnGrenadeImpact;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }

}

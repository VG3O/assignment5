using System.Collections;
using System.Collections.Generic;
using static CollisionDetection;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollisionManager : MonoBehaviour
{
    Octree tree;

    public enum CollisionType
    {
        Standard,
        Octree
    }

    public static CollisionType collisionType = CollisionType.Standard;

    [SerializeField]
    public uint nStartingParticles = 100;

    [SerializeField]
    private GameObject particlePrefab;

    [SerializeField]
    private Bounds sceneBox;

    private void Start()
    {
        // TODO: YOUR CODE HERE
        // Create the Octree. Create prefabs within the bounding box of the scene.
        //tree = Octree.Create();

        for (int i = 0; i < nStartingParticles; i++)
        {
            GameObject particle = Instantiate(particlePrefab);
            particle.transform.position = new Vector3(
                Random.Range(sceneBox.min.x, sceneBox.max.x),
                Random.Range(sceneBox.min.y, sceneBox.max.y),
                Random.Range(sceneBox.min.z, sceneBox.max.z)
            );
        }
    }

    private void TreeCollisionResolution()
    {
        // TODO: YOUR CODE HERE
        // Perform sphere-sphere collisions using the Octree
    }

    private void StandardCollisionResolution()
    {
        Sphere[] spheres = FindObjectsOfType<Sphere>();
        PlaneCollider[] planes = FindObjectsOfType<PlaneCollider>();
        for (int i = 0; i < spheres.Length; i++)
        {
            Sphere s1 = spheres[i];
            for (int j = i + 1; j < spheres.Length; j++)
            {
                Sphere s2 = spheres[j];
                ApplyCollisionResolution(s1, s2);
                CollisionChecks++;
            }
            foreach (PlaneCollider plane in planes)
            {
                ApplyCollisionResolution(s1, plane);
                CollisionChecks++;
            }
        }
    }

    private void FixedUpdate()
    {
        CollisionChecks = 0;
        StandardCollisionResolution();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (!keyboard.cKey.wasPressedThisFrame) return;

        collisionType = collisionType == CollisionType.Standard ? CollisionType.Octree : CollisionType.Standard;
    }
}

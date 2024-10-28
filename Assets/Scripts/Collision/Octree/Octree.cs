using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Codice.Client.BaseCommands.Differences;
using System.Linq;

public interface Octree
{
    /// <summary>
    /// Inserts a particle into the octree, descending its children as needed.
    /// </summary>
    /// <param name="particle"></param>
    public void Insert(Sphere particle);

    /// <summary>
    /// Does all necessary collision detection tests.
    /// </summary>
    public void ResolveCollisions();

    /// <summary>
    /// Removes all objects from the Octree.
    /// </summary>
    public void Clear();

    /// <summary>
    /// Creates a new Octree, properly creating children.
    /// </summary>
    /// <param name="pos">The position of this Octree</param>
    /// <param name="halfWidth">The width of this Octree node, from the center to one edge (only needs to be used to calculate children's positions)</param>
    /// <param name="depth">The number of levels beneath this one to create (i.e., depth = 1 means create one node with 8 children. depth = 0 means create only this node. depth = 2 means create one node with 8 children, each of which are Octree's with depth 1.</param>
    /// <returns>The newly created Octree</returns>
    public static Octree Create(Vector3 pos, float halfWidth = 1f, uint depth = 1)
    {
        // TODO: YOUR CODE HERE!
        // Recursively call Create to initialize the Octree
        if (depth == 0)
            return new OctreeObjects();

        // a parent node at this depth
        OctreeNode parent = new OctreeNode();
        parent.position = pos;

        for (int i = 0; i < 8; i++ )
        {
            int xSign = i % 2 == 0 ? -1 : 1;
            int ySign = (i / 2) % 2 == 0 ? -1 : 1;
            int zSign = (i / 4) % 2 == 0 ? -1 : 1;

            parent.children[i] = Create(new Vector3(
               pos.x + (.5f * halfWidth)*(float)xSign,
               pos.y + (.5f * halfWidth)*(float)ySign,
               pos.z + (.5f * halfWidth)*(float)zSign
            ), halfWidth * .5f, depth - 1);
        }        
        
        return parent;
    }
}

/// <summary>
/// An octree that holds 8 children, all of which are Octree's.
/// </summary>
public class OctreeNode : Octree
{
    public Vector3 position;
    public Octree[] children = new Octree[8];

    // TODO: YOUR CODE HERE

    /// <summary>
    /// Inserts the given particle into the appropriate children. The particle
    /// may need to be inserted into more than one child.
    /// </summary>
    /// <param name="sphere">The bounding sphere of the particle to insert.</param>
    public void Insert(Sphere sphere)
    {
        List<int> indices = new List<int>();
        indices.Add(0); // assume we start in bucket 0

        Vector3 distance = sphere.position - position;

        // pickIndex() but matched for multiple buckets for partitioning each collision
       
        if (Mathf.Pow(sphere.Radius,2) > Mathf.Pow(distance.x,2))
        { 
            indices.Add(1);
        }
        else if (sphere.position.x > position.x)
        {
            int count  = indices.Count;
            for (int i=0; i < count; i++)
                indices[i] += 1;
        }

        if (Mathf.Pow(sphere.Radius, 2) > Mathf.Pow(distance.y, 2))
        {
            int count = indices.Count;
            for (int i = 0; i < count; i++)
            {
                indices.Add(i + 2);
            }
        }
        else if (sphere.position.y > position.y)
        {
            int count = indices.Count;
            for (int i = 0; i < count; i++)
            {
                indices[i] += 2;
            }
        }
        if(Mathf.Pow(sphere.Radius, 2) > Mathf.Pow(distance.z, 2))
        {
            int count = indices.Count;
            for (int i = 0; i < count; i++)
            {
                indices.Add(i + 4);
            }
        }
        else if (sphere.position.z > position.z)
        {
            int count = indices.Count;
            for (int i = 0; i < count; i++)
            {
                indices[i] += 4;
            }
        }

        // now recursively call insert to insert on the children
        foreach (int i in indices)
            children[i].Insert(sphere);
    }

    /// <summary>
    /// Resolves collisions in all children, as only leaf nodes can hold particles.
    /// </summary>
    public void ResolveCollisions()
    {
        foreach (Octree octree in children)
        {
            octree.ResolveCollisions();
        }
    }

    /// <summary>
    /// Removes all particles in each child.
    /// </summary>
    public void Clear()
    {
        foreach (Octree octree in children)
        {
            octree.Clear();
        }
    }
}

/// <summary>
/// An octree that holds only particles.
/// </summary>
public class OctreeObjects : Octree
{
    private List<Sphere> objects = new List<Sphere>();

    public ICollection<Sphere> Objects
    {
        get
        {
            return objects;
        }
        
    }

    // TODO: YOUR CODE HERE!

    /// <summary>
    /// Inserts the particle into this node. It will be compared with all other
    /// particles in this node in ResolveCollisions().
    /// </summary>
    /// <param name="particle">The particle to insert.</param>
    public void Insert(Sphere particle)
    {
        Objects.Add(particle);
    }

    /// <summary>
    /// Calls CollisionDetection.ApplyCollisionResolution() on every pair of
    /// spheres in this node.
    /// </summary>
    public void ResolveCollisions()
    {
        Sphere[] arr = Objects.ToArray();
        PlaneCollider[] planes = GameObject.FindObjectsOfType<PlaneCollider>();
        for (int i = 0; i < Objects.Count; i++)
        {
            for (int j = i+1; j < Objects.Count; j++)            
                CollisionDetection.ApplyCollisionResolution(arr[i], arr[j]);
            
            foreach (PlaneCollider plane in planes)
                CollisionDetection.ApplyCollisionResolution(arr[i], plane);
        }
    }

    /// <summary>
    /// Removes all objects from this node.
    /// </summary>
    public void Clear()
    {
        Objects.Clear();
    }
}

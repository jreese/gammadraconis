using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GammaDraconis.Types;

class OctreeLeaf
{
    private const int maxobj = 200;
    private List<GameObject> containedObjects;
    public List<OctreeLeaf> childLeaves;
    private BoundingBox containerBox;
    private int maxDepth;
    private int currentDepth;
    public bool debugOctreeDepth = false;

    /// <summary>
    /// Create a new octree leaf given the specified container bounding box,
    /// maximum depth, and current depth.
    /// </summary>
    /// <param name="bound">The bounding box for this leaf.</param>
    /// <param name="max">The maximum leaf depth.</param>
    /// <param name="myDepth">The current leaf depth.</param>
    public OctreeLeaf(BoundingBox bound, int max, int myDepth)
    {
        containedObjects = new List<GameObject>();
        childLeaves = new List<OctreeLeaf>(8);
        containerBox = bound;
        maxDepth = max;
        currentDepth = myDepth;
    }
    
    /// <summary>
    /// Set the list of objects contained in this leaf.
    /// </summary>
    /// <param name="value">The list of objects to be contained in this leaf.</param>
    public void setContainedObjects(List<GameObject> value)
    {
        containedObjects = value;
        split();
    }

    /// <summary>
    /// Get the list of objects contained in this leaf.
    /// </summary>
    /// <returns>The list of objects contained in this leaf.</returns>
    public List<GameObject> getContainedObjects()
    {
        return containedObjects;
    }

    /// <summary>
    /// Get the list of children to this leaf.
    /// </summary>
    /// <returns>The children of this leaf.</returns>
    public List<OctreeLeaf> getChildLeaves()
    {
        return childLeaves;
    }

    /// <summary>
    /// Get the container box for this leaf.
    /// </summary>
    /// <returns>The container box of this leaf.</returns>
    public BoundingBox getContainerBox()
    {
        return containerBox;
    }
    
    /// <summary>
    /// Set the container box for this leaf.
    /// </summary>
    /// <param name="value">The container box of this leaf.</param>
    public void setContainerBox(BoundingBox value)
    {
        containerBox = value;
    }

    /// <summary>
    /// Split this octree leaf into child leaves.
    /// </summary>
    protected void split()
    {
        Vector3 half = (containerBox.Max - containerBox.Min) / 2;
        Vector3 halfx = Vector3.UnitX * half;
        Vector3 halfy = Vector3.UnitY * half;
        Vector3 halfz = Vector3.UnitZ * half;
        BoundingBox[] boxes = {
            new BoundingBox(containerBox.Min, containerBox.Min + half), 
            new BoundingBox(containerBox.Min + halfx, containerBox.Min + half + halfx),
            new BoundingBox(containerBox.Min + halfy, containerBox.Min + half + halfy),
            new BoundingBox(containerBox.Min + halfz, containerBox.Min + half + halfz),
            new BoundingBox(containerBox.Min + halfx + halfy, containerBox.Min + half + halfx + halfy),
            new BoundingBox(containerBox.Min + halfx + halfz, containerBox.Min + half + halfx + halfz),
            new BoundingBox(containerBox.Min + halfy + halfz, containerBox.Min + half + halfy + halfz),
            new BoundingBox(containerBox.Min + half, containerBox.Max)
        };
        
        childLeaves.Clear();
        foreach( BoundingBox tempBox in boxes)
        {
            OctreeLeaf tempLeaf = new OctreeLeaf(tempBox, maxDepth, currentDepth+1);
            foreach(GameObject obj in containedObjects){
                BoundingSphere objSphere = new BoundingSphere(obj.position.pos(), obj.size);
                if (tempBox.Contains(objSphere) != ContainmentType.Disjoint || objSphere.Contains(tempBox) != ContainmentType.Disjoint)
                {
                    tempLeaf.containedObjects.Add(obj);
                }
            }
            if (currentDepth < maxDepth && tempLeaf.containedObjects.Count != 0){
                
                tempLeaf.split();
            }
            childLeaves.Add(tempLeaf);
        }

        if (debugOctreeDepth)
        {
                Console.WriteLine("Current node depth: " + currentDepth + " Next depth: " + (currentDepth + 1));  
        }
    }

    /// <summary>
    /// List the GameObjects outside this octree leaf.
    /// </summary>
    /// <param name="gameObjects">A list of GameObjects.</param>
    /// <returns>A list of objects from the list that are outside this leaf.</returns>
    public List<GameObject> outsideOctree(List<GameObject> gameObjects)
    {
        List<GameObject> outsideObjects = new List<GameObject>();
        foreach (GameObject obj in gameObjects)
        {
            BoundingSphere objSphere = new BoundingSphere(obj.position.pos(), obj.size);
            if(containerBox.Contains(objSphere) == ContainmentType.Disjoint && objSphere.Contains(containerBox) == ContainmentType.Disjoint)
            {
                //outsideObjects.Add(obj);
            }
        }

        return outsideObjects;
    }

    /// <summary>
    /// List the GameObjects outside visible in this octree leaf.
    /// </summary>
    /// <param name="gameObjects">A list of GameObjects.</param>
    /// <returns>A list of objects from the list that are visible from this leaf.</returns>
    public List<GameObject> visible(out List<GameObject> notVisible, BoundingFrustum viewFrustrum)
    {
        List<GameObject> entirelyVisible = new List<GameObject>();
        notVisible = new List<GameObject>();
        ContainmentType contains = viewFrustrum.Contains(containerBox);
        ContainmentType contains2 = containerBox.Contains(viewFrustrum);
        if(containedObjects.Count != 0)
        {
            if (contains == ContainmentType.Contains || contains2 == ContainmentType.Contains)
            {
                entirelyVisible.AddRange(containedObjects);
            }
            else if (contains == ContainmentType.Intersects || contains2 == ContainmentType.Intersects)
            {
                foreach (OctreeLeaf child in childLeaves)
                {
                    List<GameObject> tempNotVisible;
                    entirelyVisible.AddRange(child.visible(out tempNotVisible, viewFrustrum));
                    notVisible.AddRange(tempNotVisible);
                }
            }
            else if (contains == ContainmentType.Disjoint)
            {
                notVisible.AddRange(containedObjects);
            }
        }
        return entirelyVisible;
    }
}
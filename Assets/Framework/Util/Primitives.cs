using System.Collections.Generic;
using UnityEngine;

public static class Primitives
{
    /** Lookup for various primitive meshes. */
    private static Dictionary<PrimitiveType, Mesh> _primitiveMeshes 
        = new Dictionary<PrimitiveType, Mesh>();

    /** Create a primitive of the given type, optionally without a collider. */
    public static GameObject Create(PrimitiveType type, bool withCollider)
    {
        if (withCollider)
            return GameObject.CreatePrimitive(type);

        GameObject gameObject = new GameObject(type.ToString());
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = GetPrimitiveMesh(type);
        gameObject.AddComponent<MeshRenderer>();
        return gameObject;
    }

    /** Return a primitive mesh of the given type. */
    public static Mesh GetPrimitiveMesh(PrimitiveType type)
    {
        if (!_primitiveMeshes.ContainsKey(type))
            CreatePrimitiveMesh(type);

        return _primitiveMeshes[type];
    }

    /** Create a primitive mesh of the given type. */
    private static Mesh CreatePrimitiveMesh(PrimitiveType type)
    {
        GameObject gameObject = GameObject.CreatePrimitive(type);
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        GameObject.Destroy(gameObject);
        _primitiveMeshes[type] = mesh;
        return mesh;
    }
}
using UnityEngine;

#if UNITY_EDITOR
#endif

public class SubmeshAdder : MonoBehaviour
{
    // Right-click this component in the Inspector and choose "Add Second (Duplicated) Submesh".
    [ContextMenu("Add Second (Duplicated) Submesh")]
    void AddDuplicatedSubmesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("No MeshFilter (or no mesh) found on this GameObject.");
            return;
        }

        // If you want to preserve the original imported mesh asset, 
        // you can clone it here. Otherwise, you're modifying the sharedMesh asset itself:
        //
        // Mesh meshCopy = Instantiate(mf.sharedMesh);
        // mf.sharedMesh = meshCopy;
        Mesh mesh = mf.sharedMesh;

        int originalCount = mesh.subMeshCount;
        int[] tris0 = mesh.GetTriangles(0);

        mesh.subMeshCount = originalCount + 1;
        mesh.SetTriangles(tris0, originalCount);

        Debug.Log($"Added submesh #{originalCount}. Now mesh has {mesh.subMeshCount} submeshes.");
    }
}
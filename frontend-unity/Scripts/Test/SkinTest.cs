using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SkinTest : MonoBehaviour
{

    public GameObject Prop;
    // Start is called before the first frame update
    void Start()
    {
        Apply();
    }

    public void Apply()
    {
        var find = Prop.transform.Find("RProps_01");
        var root = transform.Find("RigPelvis");
        var smr = find.GetComponent<SkinnedMeshRenderer>();

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Material> materials = new List<Material>();
        List<Transform> bones = new List<Transform>();
        Transform[] transforms = transform.GetComponentsInChildren<Transform>();
        
        materials.AddRange(smr.materials);
        bones.AddRange(smr.bones);


        // for (int i = 0; i < smr.sharedMesh.subMeshCount; i++)
        // {
        //     CombineInstance ci = new CombineInstance();
        //     Mesh mesh = new Mesh();
        //     smr.BakeMesh(mesh);
        //     mesh.uv = smr.sharedMesh.uv;
        //     ci.mesh = mesh;
        //     ci.subMeshIndex = i;
        //     combineInstances.Add(ci);
        // }

        GameObject prop = new GameObject("RProps_01_new");

        prop.transform.parent = transform;
        prop.transform.localPosition = Vector3.zero;
        prop.transform.rotation = Quaternion.identity;

        var smr2 = prop.AddComponent<SkinnedMeshRenderer>();

        Mesh mesh2 = new Mesh();
        mesh2.CombineMeshes(combineInstances.ToArray(), false, false);
        smr2.sharedMesh = smr.sharedMesh;
        smr2.bones = FindBones(bones.ToArray(), transforms).ToArray();
        smr2.materials = materials.ToArray();
        smr2.rootBone = root;
    }

    public List<Transform> FindBones(Transform[] b1, Transform[] b2)
    {
        List<Transform> bones = new List<Transform>();
        
        for (int i = 0; i < b1.Length; i++)
        {
            for (int j = 0; j < b2.Length; j++)
            {
                if (b1[i].name== b2[j].name)
                {
                    bones.Add(b2[j]);
                    Debug.Log("Add Bone " + b2[j].name);
                    break;
                }    
            }
        }

        return bones;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}

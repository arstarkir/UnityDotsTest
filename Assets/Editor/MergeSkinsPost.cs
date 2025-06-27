using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class MergeSkinsPost : AssetPostprocessor
{
    void OnPostprocessModel(GameObject go)
    {
        var sources = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (sources.Length <= 1) return;

        // ------------- gather bones & meshes -------------------------------
        var uniqueBones = new List<Transform>();
        var boneIndex = new Dictionary<Transform, int>();

        int MapBone(Transform t)
        {
            if (boneIndex.TryGetValue(t, out int idx)) return idx;
            idx = uniqueBones.Count;
            uniqueBones.Add(t);
            boneIndex[t] = idx;
            return idx;
        }

        var combines = new List<CombineInstance>(sources.Length);

        foreach (var src in sources)
        {
            Mesh copy = Object.Instantiate(src.sharedMesh);
            var bw = copy.boneWeights;

            for (int i = 0; i < bw.Length; i++)
            {
                var w = bw[i];
                w.boneIndex0 = MapBone(src.bones[w.boneIndex0]);
                w.boneIndex1 = MapBone(src.bones[w.boneIndex1]);
                w.boneIndex2 = MapBone(src.bones[w.boneIndex2]);
                w.boneIndex3 = MapBone(src.bones[w.boneIndex3]);
                bw[i] = w;
            }
            copy.boneWeights = bw;

            combines.Add(new CombineInstance { mesh = copy, transform = Matrix4x4.identity });
        }

        // ------------- build combined mesh ---------------------------------
        var merged = new Mesh { name = go.name + "_Merged" };
        merged.CombineMeshes(combines.ToArray(), true, false, false);

        // ------------- create single renderer ------------------------------
        var root = new GameObject("MergedRenderer").transform;
        root.SetParent(go.transform, false);

        var smr = root.gameObject.AddComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = merged;
        smr.sharedMaterial = sources[0].sharedMaterial;
        smr.bones = uniqueBones.ToArray();

        var binds = new Matrix4x4[uniqueBones.Count];
        Matrix4x4 rootToWorld = root.localToWorldMatrix;
        for (int i = 0; i < binds.Length; i++)
            binds[i] = uniqueBones[i].worldToLocalMatrix * rootToWorld;

        merged.bindposes = binds;

        smr.rootBone = uniqueBones[0];
        smr.localBounds = merged.bounds;


        foreach (var s in sources) Object.DestroyImmediate(s.gameObject);
    }
}

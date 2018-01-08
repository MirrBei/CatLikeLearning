﻿using System.Collections.Generic;
using UnityEngine;

namespace T2MeshBasics.S3CubeSphere
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SphereCube : MonoBehaviour
    {
        [SerializeField] private int gridSize;
        [SerializeField] private float radius;

        private Mesh mesh;
        private Vector3[] vertices;
        private Vector3[] normals;
        private Color32[] cubeUv;

        private void Awake()
        {
            Generate();
        }

        private void Generate()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Procedural Cube";
            CreateVertices();
            CreateTriangles();
            CreateColliders();
        }

        private static int SetQuad(IList<int> triangles, int t, int v00, int v10, int v01, int v11)
        {
            triangles[t] = v00;
            triangles[t + 1] = triangles[t + 4] = v01;
            triangles[t + 2] = triangles[t + 3] = v10;
            triangles[t + 5] = v11;
            return t + 6;
        }

        private void CreateColliders()
        {
            gameObject.AddComponent<SphereCollider>();
        }

        private void CreateTriangles()
        {
            var trianglesZ = new int[(gridSize * gridSize) * 12];
            var trianglesX = new int[(gridSize * gridSize) * 12];
            var trianglesY = new int[(gridSize * gridSize) * 12];
            int ring = (gridSize + gridSize) * 2;
            int tZ = 0, tX = 0, tY = 0, v = 0;
            for (var y = 0; y < gridSize; y++, v++)
            {
                for (var q = 0; q < gridSize; q++, v++)
                {
                    tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
                }
                for (var q = 0; q < gridSize; q++, v++)
                {
                    tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
                }
                for (var q = 0; q < gridSize; q++, v++)
                {
                    tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
                }
                for (var q = 1; q < gridSize; q++, v++)
                {
                    tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
                }
                tX = SetQuad(trianglesX, tX, v, v + 1 - ring, v + ring, v + 1);
            }

            tY = CreateTopFace(trianglesY, tY, ring);
            CreateBottomFace(trianglesY, tY, ring);

            mesh.subMeshCount = 3;
            mesh.SetTriangles(trianglesZ, 0);
            mesh.SetTriangles(trianglesX, 1);
            mesh.SetTriangles(trianglesY, 2);
        }

        private int CreateTopFace(int[] triangles, int t, int ring)
        {
            int v = ring * gridSize;
            for (var x = 1; x < gridSize; x++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
            }
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

            int vMin = ring * (gridSize + 1) - 1;
            int vMid = vMin + 1;
            int vMax = v + 2;
            for (var z = 2; z < gridSize; z++, vMin--, vMax++, vMid++)
            {
                t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + gridSize - 1);
                for (var x = 2; x < gridSize; x++, vMid++)
                {
                    t = SetQuad(triangles, t, vMid, vMid + 1, vMid + gridSize - 1, vMid + gridSize);
                }
                t = SetQuad(triangles, t, vMid, vMax, vMid + gridSize - 1, vMax + 1);
            }

            int vTop = vMin - 2;
            t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
            for (var x = 2; x < gridSize; x++, vMid++, vTop--)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
            }
            t = SetQuad(triangles, t, vMid, vMax, vTop, vTop - 1);

            return t;
        }

        private void CreateBottomFace(IList<int> triangles, int t, int ring)
        {
            var v = 1;
            int vMid = vertices.Length - (gridSize - 1) * (gridSize - 1);
            t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
            for (var x = 2; x < gridSize; x++, v++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
            }
            t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

            int vMin = ring - 2;
            ++vMid;
            int vMax = v + 3;
            for (var z = 2; z < gridSize; z++, vMin--, vMax++, vMid++)
            {
                t = SetQuad(triangles, t, vMin, vMid, vMin + 1, vMid - gridSize + 1);
                for (var x = 2; x < gridSize; x++, vMid++)
                {
                    t = SetQuad(triangles, t, vMid, vMid + 1, vMid - gridSize + 1, vMid - gridSize + 2);
                }
                t = SetQuad(triangles, t, vMid, vMax, vMid - gridSize + 1, vMax - 1);
            }

            int vTop = vMin - 1;
            vMid -= gridSize - 1;
            t = SetQuad(triangles, t, vMin, vTop, vMin + 1, vMid);
            for (var x = 2; x < gridSize; x++, vMid++, vTop--)
            {
                t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
            }
            SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
        }

        private void CreateVertices()
        {
            const int cornerVertices = 8;
            int edgeVertices = (gridSize + gridSize + gridSize - 3) * 4;
            int faceVertices = 2 * ((gridSize - 1) * (gridSize - 1) +
                                    (gridSize - 1) * (gridSize - 1) +
                                    (gridSize - 1) * (gridSize - 1));
            vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
            normals = new Vector3[vertices.Length];
            cubeUv = new Color32[vertices.Length];

            var vi = 0;
            for (var y = 0; y <= gridSize; y++)
            {
                for (var x = 0; x < gridSize; x++)
                {
                    SetVertex(vi++, x, y, 0);
                }
                for (var z = 0; z < gridSize; z++)
                {
                    SetVertex(vi++, gridSize, y, z);
                }
                for (int x = gridSize; x > 0; --x)
                {
                    SetVertex(vi++, x, y, gridSize);
                }
                for (int z = gridSize; z > 0; --z)
                {
                    SetVertex(vi++, 0, y, z);
                }
            }

            for (var z = 1; z < gridSize; z++)
            {
                for (var x = 1; x < gridSize; x++)
                {
                    SetVertex(vi++, x, gridSize, z);
                }
            }
            for (var z = 1; z < gridSize; z++)
            {
                for (var x = 1; x < gridSize; x++)
                {
                    SetVertex(vi++, x, 0, z);
                }
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.colors32 = cubeUv;
        }

        private void SetVertex(int i, int x, int y, int z)
        {
            Vector3 v = new Vector3(x, y, z) * 2f / gridSize - Vector3.one;
            float x2 = v.x * v.x;
            float y2 = v.y * v.y;
            float z2 = v.z * v.z;
            Vector3 s;
            s.x = v.x * Mathf.Sqrt(1 - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
            s.y = v.y * Mathf.Sqrt(1 - z2 / 2f - x2 / 2f + x2 * z2 / 3f);
            s.z = v.z * Mathf.Sqrt(1 - y2 / 2f - x2 / 2f + y2 * x2 / 3f);
            normals[i] = s;
            vertices[i] = normals[i] * radius;
            cubeUv[i] = new Color32((byte) x, (byte) y, (byte) z, 0);
        }

        private void OnDrawGizmos()
        {
            if (vertices == null) return;
            Gizmos.color = Color.black;
            for (var i = 0; i < vertices.Length; i++)
            {
                Gizmos.color = Color.black;
                Vector3 vertex = vertices[i];
                Gizmos.DrawSphere(transform.TransformPoint(vertex), 0.1f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.TransformPoint(vertex), transform.TransformDirection(normals[i]));
            }
        }
    }
}
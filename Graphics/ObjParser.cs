using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Graphics.Primitives;

namespace Graphics
{
    public static class ObjParser
    {
        public static Mesh GetMeshFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            List<Vector3> vectors = new List<Vector3>();
            List<Polygon> polygons = new List<Polygon>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            foreach (var line in lines)
            {
                string[] parts = line.Split(' ');
                string type = parts[0];
                string[] values = new string[parts.Length - 1];
                Array.Copy(parts, 1, values, 0, values.Length);

                switch (type)
                {
                    case "v":
                        vectors.Add(ParseVector(values));
                        break;
                    case "vt":
                        Vector3 v = ParseVector(values);
                        uvs.Add(new Vector2(v));
                        break;
                    case "vn":
                        normals.Add(ParseVector(values));
                        break;
                    case "f":
                        polygons.Add(ParsePolygon(values, vectors, normals, uvs));
                        break;
                    default:
                        continue;
                }
            }
            return new Mesh(polygons.ToArray()).Normalize();
        }

        private static Vector3 ParseVector(string[] tuples)
        {            
            var tuples1 = tuples.Where(t => !t.Equals(string.Empty));
            float[] values = tuples1.Select(x => float.Parse(x.Replace('.', ','))).ToArray();
            return new Vector3(values[0], values[1], values[2]);//.Normalized;
        }

        private static Polygon ParsePolygon(string[] tuples, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs)
        {
            List<Vector3> values = new List<Vector3>();
            List<Vector3> normal = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            foreach (var tuple in tuples)
            {
                string[] parts = tuple.Replace("//", "/").Split('/');
                int vertexNumber = int.Parse(parts[0]);
                int normalNumber = -1;
                int uvNumber = -1;
                values.Add(vectors[vertexNumber - 1]);
                if (tuple.Contains("//")) //vertex // normal
                {
                    normalNumber = int.Parse(parts[1]);
                    normal.Add(normals[normalNumber - 1]);
                }
                else //vertex / texture / [normal]
                {   //vertex / texture
                    uvNumber = int.Parse(parts[1]);
                    uv.Add(uvs[uvNumber - 1]);
                    if (parts.Length > 2) //vertex / texture / normal
                    {                        
                        normalNumber = int.Parse(parts[2]);
                        normal.Add(normals[normalNumber - 1]);
                    }                  
                }                                               
            }

            return new Polygon(values.ToArray(), normal.ToArray(), uv.ToArray());
        }
    }
}

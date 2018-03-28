using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Graphics.Primitives;

namespace Graphics
{
    public class MtlParser
    {
        public static List<Material> GetMaterialFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            
            List<Material> materials = new List<Material>();
            Material currentMaterial = null;
            foreach (var line in lines)
            {
                List<String> words = line.Split(' ').ToList();
                
                switch (words[0])
                {
                    case "newmtl":
                        currentMaterial = new Material();
                        currentMaterial.Name = words[1];                        
                        break;
                    case "Ka":
                        currentMaterial.AmbientColor = ParseVector(words.GetRange(1,3));
                        break;
                    case "Kd":
                        currentMaterial.DiffuseColor = ParseVector(words.GetRange(1,3));
                        break;
                    case "Ks":
                        currentMaterial.SpecularColor = ParseVector(words.GetRange(1,3));
                        break;
                    case "illum":
                        currentMaterial.IlluminationModel = Int32.Parse(words[1]);
                        materials.Add(currentMaterial);
                        break;
                    case "Ns":
                        currentMaterial.Shininess = (float) Double.Parse(words[1].Replace('.', ','));
                        break;
                    case "d":
                        currentMaterial.Transparency = (float) Double.Parse(words[1].Replace('.', ','));
                        break;
                    /*case "":
                        materials.Add(currentMaterial);
                        break;*/
                    }
            }
            return materials;
        }
        
        private static Vector3 ParseVector(List<String> tuples)
        {            
            var tuples1 = tuples.Where(t => !t.Equals(string.Empty));
            float[] values = tuples1.Select(x => float.Parse(x.Replace('.', ','))).ToArray();
            return new Vector3(values[0], values[1], values[2]);//.Normalized;
        }
    }
}
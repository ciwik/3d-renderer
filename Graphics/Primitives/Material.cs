using System;
using Graphics.Primitives;

namespace Graphics
{
    public class Material
    {
        public String Name { get; set; }// after newmtl
        
        public Vector3 AmbientColor { get; set; } //Ka
        public Vector3 DiffuseColor { get; set; }  //Kd
        public Vector3 SpecularColor { get; set; }  //Ks

        public int IlluminationModel { get; set; }  //illum

        public float Shininess { get; set; }  //Ns

        public float Transparency { get; set; }  //d or Tr

        //public String TextureMap { get; set; }  //map_Ka
    }
}
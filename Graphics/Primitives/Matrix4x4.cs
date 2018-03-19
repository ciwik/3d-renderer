namespace Graphics.Primitives
{
    public struct Matrix4x4
    {
        private float[,] _values;

        public Matrix4x4(Vector4 diag)
        {
            _values = new float[4, 4];

            _values[0, 0] = diag.X;
            _values[1, 1] = diag.Y;
            _values[2, 2] = diag.Z;
            _values[3, 3] = diag.W;
        }

        public float this[int i, int j]
        {
            get => _values[i, j];
            set => _values[i, j] = value;
        }

        public Vector4 this[int i]
        {
            get
            {
                return new Vector4(_values[i, 0], _values[i, 1], _values[i, 2], _values[i, 3]);
            }
            set
            {
                _values[i, 0] = value.X;
                _values[i, 1] = value.Y;
                _values[i, 2] = value.Z;
                _values[i, 3] = value.W;
            }
        }

        public static Matrix4x4 Identity => new Matrix4x4(new Vector4(1, 1, 1, 1));

        public static Matrix4x4 operator *(Matrix4x4 mat1, Matrix4x4 mat2)
        {
            Matrix4x4 result = new Matrix4x4();
            result._values = new float[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        sum += mat1[i, k] * mat2[k, j];
                    }
                    result[i, j] = sum;
                }
            }

            return result;
        }

        public static Vector4 operator *(Matrix4x4 mat, Vector4 v)
        {
            float[] res = new float[4];
            for (int i = 0; i < 4; i++)
            {
                res[i] = mat[i, 0] * v.X + mat[i, 1] * v.Y + mat[i, 2] * v.Z + mat[i, 3] * v.W;
            }
            return new Vector4(res[0], res[1], res[2], res[3]);
        }
    }
}

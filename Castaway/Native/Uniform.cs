namespace Castaway.Native
{
    public abstract class Uniform
    {
        public abstract void Upload(int uniform);

        public static implicit operator Uniform(float f) => new FloatUniform(f);
    }

    public class FloatUniform : Uniform
    {
        public float Value;

        public FloatUniform(float value)
        {
            Value = value;
        }

        public override unsafe void Upload(int uniform)
        {
            fixed(float* p = &Value) CawNative.cawSetUniformF1(uniform, p, 1);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Castaway.Rendering.Objects;

public abstract class BufferObject : RenderObject, IValidatable, IBindable
{
    protected IEnumerable<byte>? RealData = null;

    public IEnumerable<byte> Data
    {
        get => RealData ?? ImmutableArray<byte>.Empty;
        set => Upload(value);
    }

    public abstract void Bind();
    public abstract void Unbind();

    public abstract bool Valid { get; }
    public abstract void Upload(IEnumerable<byte> bytes);

    public virtual void Upload(IEnumerable<uint> uints)
    {
        Upload(BitConverter.IsLittleEndian
            ? uints.SelectMany(BitConverter.GetBytes)
            : uints.SelectMany(BitConverter.GetBytes).Reverse());
    }

    public virtual void Upload(IEnumerable<int> ints)
    {
        Upload(BitConverter.IsLittleEndian
            ? ints.SelectMany(BitConverter.GetBytes)
            : ints.SelectMany(BitConverter.GetBytes).Reverse());
    }

    public virtual void Upload(IEnumerable<float> floats)
    {
        Upload(BitConverter.IsLittleEndian
            ? floats.SelectMany(BitConverter.GetBytes)
            : floats.SelectMany(BitConverter.GetBytes).Reverse());
    }

    public virtual void Upload(IEnumerable<double> doubles)
    {
        Upload(BitConverter.IsLittleEndian
            ? doubles.SelectMany(BitConverter.GetBytes)
            : doubles.SelectMany(BitConverter.GetBytes).Reverse());
    }

    public virtual void Upload(params int[] ints)
    {
        Upload(ints as IEnumerable<int>);
    }

    public virtual void Upload(params uint[] uints)
    {
        Upload(uints as IEnumerable<uint>);
    }

    public virtual void Upload(params float[] floats)
    {
        Upload(floats as IEnumerable<float>);
    }

    public virtual void Upload(params double[] doubles)
    {
        Upload(doubles as IEnumerable<double>);
    }

    public virtual void Upload(params int[][] intses)
    {
        Upload(intses.SelectMany(a => a));
    }

    public virtual void Upload(params uint[][] uintses)
    {
        Upload(uintses.SelectMany(a => a));
    }

    public virtual void Upload(params float[][] floatses)
    {
        Upload(floatses.SelectMany(a => a));
    }

    public virtual void Upload(params double[][] doubleses)
    {
        Upload(doubleses.SelectMany(a => a));
    }
}
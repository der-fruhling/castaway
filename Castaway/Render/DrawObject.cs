using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Castaway.Native;
using static Castaway.Native.CawNative;
using static Castaway.Native.CawNative.buffer_storage_type;

namespace Castaway.Render
{
    public class DrawObject
    {
        internal readonly buffer Buffer;
        private readonly List<uint> _elements;
        private readonly List<vertex> _vertices = new();
        private readonly Stack<vertex> _stack = new();
        internal uint Count => (uint) _vertices.Count;

        public DrawObject() => Buffer = cawNewBuffer(vert_triangles);
        public DrawObject(buffer b) => Buffer = b;

        public DrawObject Vertex(
            float x, float y, float z = 0,
            float u = 0, float v = 0, float t = 0,
            float nx = 0, float ny = 0, float nz = 0,
            float r = 1, float g = 1, float b = 1, float a = 1)
        {
            _stack.Push(new vertex(x, y, z, u, v, t, nx, ny, nz, r, g, b, a));
            return this;
        }

        internal unsafe void Adjust(shader* s)
        {
            List<float> floats = new();
            foreach (var v in _vertices)
            {
                for (var i = 0; i < s->attrCount; i++)
                {
                    var a = s->attrs[i];
                    switch (a.type)
                    {
                        case vertex_attr_type.position2:
                            floats.AddRange(new[] {v.x, v.y});
                            break;
                        case vertex_attr_type.position3:
                            floats.AddRange(new[] {v.x, v.y, v.z});
                            break;
                        case vertex_attr_type.position4:
                            floats.AddRange(new[] {v.x, v.y, v.z, 1});
                            break;
                        case vertex_attr_type.color3:
                            floats.AddRange(new[] {v.r, v.g, v.b});
                            break;
                        case vertex_attr_type.color4:
                            floats.AddRange(new[] {v.r, v.g, v.b, v.a});
                            break;
                        case vertex_attr_type.normal3:
                            floats.AddRange(new[] {v.nx, v.ny, v.nz});
                            break;
                        case vertex_attr_type.tex_coords2:
                            floats.AddRange(new[] {v.u, v.v});
                            break;
                        case vertex_attr_type.tex_coords3:
                            floats.AddRange(new[] {v.u, v.v, v.t});
                            break;
                        default:
                            throw new InvalidOperationException($"Invalid vertex attribute type: {a.type} in {Marshal.PtrToStringAnsi(new IntPtr(a.name))}");
                    }
                }
            }

            using (Memory.Alloc(out float* ptr, floats.Count))
            {
                Memory.Copy(ptr, floats.ToArray());
                cawUpdateBufferData(Buffer, (ulong) (sizeof(float) * floats.Count), ptr);
            }
        }

        public unsafe void Adjust(Shader s) => Adjust(s.Native);

        public DrawObject Tri()
        {
            _vertices.AddRange(new []{_stack.Pop(), _stack.Pop(), _stack.Pop()});
            return this;
        }
        
        public DrawObject Quad()
        {
            var a = new[] {_stack.Pop(), _stack.Pop(), _stack.Pop(), _stack.Pop()};
            _vertices.AddRange(new [] {
                a[0], a[1], a[2], 
                a[1], a[2], a[3]
            });
            return this;
        }
    }
}
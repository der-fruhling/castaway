using System;
using System.Linq;
using System.Reflection;

namespace Castaway.Components;

public static class Ex
{
	public static Type[] AllExtending(this Type type, Type @base)
	{
		return type.GetNestedTypes().Where(t => t.BaseType == @base).ToArray();
	}

	public static Type[] AllExtending<T>(this Type type)
	{
		return AllExtending(type, typeof(T));
	}

	public static Type[] AllExtending(this Assembly asm, Type @base)
	{
		return asm.GetTypes().Where(t => t.BaseType == @base).ToArray();
	}

	public static Type[] AllExtending<T>(this Assembly asm)
	{
		return AllExtending(asm, typeof(T));
	}
}
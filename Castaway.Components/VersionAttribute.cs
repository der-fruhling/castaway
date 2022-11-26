using System;

namespace Castaway.Components;

[AttributeUsage(AttributeTargets.Assembly)]
public class VersionAttribute : Attribute
{
	public VersionStream Stream;
	public Version Version;

	public VersionAttribute(VersionStream stream, int major, int minor, int build)
	{
		Version = new Version(major, minor, build);
		Stream = stream;
	}

	public override string ToString()
	{
		return Version.ToString();
	}
}
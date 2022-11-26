using System;

namespace Castaway.Rendering.Objects;

public interface IBindable
{
	public void Bind();
	public void Unbind();

	public void Bound(Action a)
	{
		Bind();
		a();
		Unbind();
	}
}
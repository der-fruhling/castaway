using System;
using System.Collections.Generic;
using Castaway.Base;
using Castaway.Input;
using Castaway.Math;
using Castaway.Rendering;
using Castaway.Rendering.Structures;
using GLFW;
using Serilog;

namespace Castaway.UI;

// ReSharper disable once InconsistentNaming
public abstract class UIElement
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();

	private bool _initialized;
	private int _x;
	private int _y;
	public (int X, int Y) Area;
	public List<UIElement> Children = new();
	public UIElement? Parent;
	public Corner Relative;

	protected UIElement(int x, int y, int width, int height, Corner relative = Corner.BottomLeft)
	{
		_x = x;
		_y = y;
		Width = width;
		Height = height;
		Relative = relative;
		Graphics.Current.Window!.GetFramebufferSize(out Area.X, out Area.Y);
	}

	public int Width { get; set; }
	public int Height { get; set; }

	private bool Hovered
	{
		get
		{
			var (a, b) = GetBounds(Area.X, Area.Y);
			return InputSystem.Mouse.IsOver(a, b);
		}
	}

	private bool WasLeftClicked => Hovered && InputSystem.Mouse.IsDown(MouseButton.Left);
	private bool WasRightClicked => Hovered && InputSystem.Mouse.IsDown(MouseButton.Right);
	private bool WasMiddleClicked => Hovered && InputSystem.Mouse.IsDown(MouseButton.Middle);

	public int X
	{
		get => _x + (Parent?.X ?? 0);
		set => _x = value;
	}

	public int Y
	{
		get => _y + (Parent?.Y ?? 0);
		set => _y = value;
	}

	protected internal (Vector2, Vector2) GetBounds(int areaWidth, int areaHeight)
	{
		return Relative switch
		{
			Corner.BottomLeft => (new Vector2(_x, _y), new Vector2(_x + Width, _y + Height)),
			Corner.BottomRight => (new Vector2(-_x + areaWidth, _y),
				new Vector2(-_x + areaWidth - Width, _y + Height)),
			Corner.TopLeft => (new Vector2(_x, -_y + areaHeight),
				new Vector2(_x + Width, -_y + areaHeight - Height)),
			Corner.TopRight => (new Vector2(-_x + areaWidth, -_y + areaHeight),
				new Vector2(-_x + areaWidth - Width, -_y + areaHeight - Height)),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	protected internal Mesh ConstructMesh(Converter<Corner, Vector4> colorChooser)
	{
		var (a, b) = GetBounds(Area.X, Area.Y);
		return Relative switch
		{
			Corner.TopLeft => new Mesh(
				new Mesh.Vertex[]
				{
					new(new Vector3(a.X, a.Y, 0), colorChooser(Corner.TopLeft), new Vector3(0, 1, 0)),
					new(new Vector3(a.X, b.Y, 0), colorChooser(Corner.BottomLeft), new Vector3(0, 0, 0)),
					new(new Vector3(b.X, a.Y, 0), colorChooser(Corner.TopRight), new Vector3(1, 1, 0)),
					new(new Vector3(b.X, b.Y, 0), colorChooser(Corner.BottomRight), new Vector3(1, 0, 0))
				}, new uint[] { 0, 1, 2, 3, 1, 2 }),
			Corner.BottomLeft => new Mesh(
				new Mesh.Vertex[]
				{
					new(new Vector3(a.X, a.Y, 0), colorChooser(Corner.BottomLeft), new Vector3(0, 0, 0)),
					new(new Vector3(a.X, b.Y, 0), colorChooser(Corner.TopLeft), new Vector3(0, 1, 0)),
					new(new Vector3(b.X, a.Y, 0), colorChooser(Corner.BottomRight), new Vector3(1, 0, 0)),
					new(new Vector3(b.X, b.Y, 0), colorChooser(Corner.TopRight), new Vector3(1, 1, 0))
				}, new uint[] { 0, 1, 2, 3, 1, 2 }),
			Corner.TopRight => new Mesh(
				new Mesh.Vertex[]
				{
					new(new Vector3(a.X, a.Y, 0), colorChooser(Corner.TopRight), new Vector3(1, 1, 0)),
					new(new Vector3(a.X, b.Y, 0), colorChooser(Corner.BottomRight), new Vector3(1, 0, 0)),
					new(new Vector3(b.X, a.Y, 0), colorChooser(Corner.TopLeft), new Vector3(0, 1, 0)),
					new(new Vector3(b.X, b.Y, 0), colorChooser(Corner.BottomLeft), new Vector3(0, 0, 0))
				}, new uint[] { 0, 1, 2, 3, 1, 2 }),
			Corner.BottomRight => new Mesh(
				new Mesh.Vertex[]
				{
					new(new Vector3(a.X, a.Y, 0), colorChooser(Corner.BottomRight), new Vector3(1, 0, 0)),
					new(new Vector3(a.X, b.Y, 0), colorChooser(Corner.TopRight), new Vector3(1, 1, 0)),
					new(new Vector3(b.X, a.Y, 0), colorChooser(Corner.BottomLeft), new Vector3(0, 0, 0)),
					new(new Vector3(b.X, b.Y, 0), colorChooser(Corner.TopLeft), new Vector3(0, 1, 0))
				}, new uint[] { 0, 1, 2, 3, 1, 2 }),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	protected internal Mesh ConstructMesh(Vector4 bottomLeft, Vector4 topLeft, Vector4 bottomRight,
		Vector4 topRight)
	{
		return ConstructMesh(corner => corner switch
		{
			Corner.TopLeft => topLeft,
			Corner.BottomLeft => bottomLeft,
			Corner.TopRight => topRight,
			Corner.BottomRight => bottomRight,
			_ => throw new ArgumentOutOfRangeException(nameof(corner), corner, null)
		});
	}

	protected internal Mesh ConstructMesh(Vector4 color)
	{
		return ConstructMesh(_ => color);
	}

	public event EventHandler? LeftClicked, RightClick, MiddleClick;

	public void RenderElement()
	{
		if (Hovered) RenderHovered();
		else Render();
		foreach (var c in Children) c.RenderElement();
	}

	public void UpdateElement()
	{
		Update();

		if (WasLeftClicked)
		{
			Logger.Verbose("Left click detected: {@State}", InputSystem.Mouse);
			ReactLeftClick();
		}

		if (WasRightClicked)
		{
			Logger.Verbose("Right click detected: {@State}", InputSystem.Mouse);
			ReactRightClick();
		}

		if (WasMiddleClicked)
		{
			Logger.Verbose("Middle click detected: {@State}", InputSystem.Mouse);
			ReactMiddleClick();
		}

		foreach (var c in Children) c.UpdateElement();
	}

	internal void Process()
	{
		if (!_initialized)
		{
			Logger.Verbose("Initializing UIElement: {@This}", this);
			Initialize();
			_initialized = true;
		}

		RenderElement();
		UpdateElement();
	}

	protected abstract void Initialize();
	protected abstract void Render();
	protected abstract void Update();

	protected virtual void RenderHovered()
	{
		Render();
	}

	protected virtual void ReactLeftClick()
	{
		LeftClicked?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void ReactRightClick()
	{
		RightClick?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void ReactMiddleClick()
	{
		MiddleClick?.Invoke(this, EventArgs.Empty);
	}
}
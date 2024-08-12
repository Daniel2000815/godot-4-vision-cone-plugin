#if TOOLS
using Godot;
using System;

[Tool]
public partial class VisionCone2DPlugin : EditorPlugin
{
	public override void _EnterTree()
	{
		AddCustomType("VisionCone2D", "Node2D", GD.Load<Script>("res://addons/visioncone2d/VisionCone2DSpawner.cs"), GD.Load<Texture2D>("res://addons/visioncone2d/vision_cone_2d_icon.svg"));
	}

	public override void _ExitTree()
	{
		RemoveCustomType("VisionCone2D");
	}
}
#endif

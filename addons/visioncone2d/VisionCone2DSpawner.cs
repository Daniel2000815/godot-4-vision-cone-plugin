#if TOOLS
using Godot;
using System;

[Tool]
public partial class VisionCone2DSpawner : Node2D
{
	
	public override void _EnterTree()
	{
		GD.Print("amos");
		if(!Engine.IsEditorHint())
			return;

		GD.Print("amos");
		Node2D scene = GD.Load<PackedScene>("res://addons/visioncone2d/vision_cone_2d.tscn").Instantiate<Node2D>();
		scene.Name = "VisionCone2D";
		GetParent().AddChild(scene, true); 
		
		scene.Owner = GetTree().EditedSceneRoot;
		QueueFree();
	}

	public override void _ExitTree()
	{
	}
}
#endif

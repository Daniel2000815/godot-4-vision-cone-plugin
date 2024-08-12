using Godot;
using System;

public partial class detector : Node2D
{
    [Export] public VisionCone2D visionArea;
    [Export] Label label;

    public override void _Ready(){
        base._Ready();

        visionArea.OnEnterFar   += (Node2D n) => label.Text = $"ENTER FAR: {n.Name}";
        visionArea.OnExitFar    += (Node2D n) => label.Text = $"EXIT FAR: {n.Name}";
        visionArea.OnEnterNear  += (Node2D n) => label.Text = $"ENTER NEAR: {n.Name}";
        visionArea.OnExitNear   += (Node2D n) => label.Text = $"EXIT NEAR: {n.Name}";
        visionArea.OnEnterRange += (Node2D n) => label.Text = $"ENTER RANGE: {n.Name}";
        visionArea.OnExitRange  += (Node2D n) => label.Text = $"EXIT RANGE: {n.Name}";
    }
}

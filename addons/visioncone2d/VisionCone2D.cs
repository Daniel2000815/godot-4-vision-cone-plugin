using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class VisionCone2D : Node2D
{
	[ExportGroup("Near Detection")]
	[Export] public Area2D nearArea;
	[Export] public CollisionPolygon2D nearShape;
	[Export] public VisionConeResource nearVisionCone;

	[ExportGroup("Far Detection")]
	[Export] public Area2D farArea;
	[Export] public CollisionPolygon2D farShape;
	[Export] public VisionConeResource farVisionCone;

	[ExportGroup("Range Detection")]
	[Export] public Area2D rangeArea;
	[Export] public CollisionPolygon2D rangeShape;
	[Export] public VisionConeResource rangeVisionCone;

	[ExportGroup("Debug")]
	[Export] bool DrawInGame = false;
	[Export] Color nearColor = new(1,0,0,0.3f), farColor = new(0,1,0,0.3f), rangeColor = new(0,0,1,0.3f);

	public event Action<Node2D> OnEnterNear, OnEnterFar, OnExitNear, OnExitFar, OnEnterRange, OnExitRange, OnExit, OnEnter;
	public HashSet<Node2D> nearNodes, farNodes, rangeNodes;
	private enum AreaType {Near, Far, Range};
	private CharacterBody2D parentCharacter;

    public override void _EnterTree()
    {
        base._EnterTree();
		if(Engine.IsEditorHint()){
			if(nearVisionCone != null)	nearVisionCone.Changed 	+= () => UpdateColliderPoints(nearArea, nearShape, nearVisionCone);
			if(farVisionCone != null)	farVisionCone.Changed 	+= () => UpdateColliderPoints(farArea, farShape, farVisionCone);
			if(rangeVisionCone != null)	rangeVisionCone.Changed += () => UpdateColliderPoints(rangeArea, rangeShape, rangeVisionCone);
		}
    }

    public override void _Ready()
	{
		base._Ready();

		nearNodes = new HashSet<Node2D>();
		farNodes = new HashSet<Node2D>();
		rangeNodes = new HashSet<Node2D>();

		parentCharacter = GetParent() is CharacterBody2D ? GetParent<CharacterBody2D>() : null;

		if(nearArea==null || farArea==null)
			return;

		GD.Print("yes");
		nearArea.BodyEntered 	+= body => HandleEnter(body, AreaType.Near);
		farArea.BodyEntered 	+= body => HandleEnter(body, AreaType.Far);
		rangeArea.BodyEntered 	+= body => HandleEnter(body, AreaType.Range);

		nearArea.BodyExited 	+= body => HandleExit(body, AreaType.Near);
		farArea.BodyExited 		+= body => HandleExit(body, AreaType.Far);
		rangeArea.BodyExited 	+= body => HandleExit(body, AreaType.Range);
	}
	

    public override void _ExitTree()
    {
		// nearVisionCone.Changed 	-= () => UpdateColliderPoints(nearArea, nearShape, nearVisionCone);
		// farVisionCone.Changed 	-= () => UpdateColliderPoints(farArea, farShape, farVisionCone);
		// rangeVisionCone.Changed -= () => UpdateColliderPoints(rangeArea, rangeShape, rangeVisionCone);
        base._ExitTree();
    }

    private void HandleEnter(Node2D body, AreaType type){
		switch(type){
			case AreaType.Range:
				if(!rangeNodes.Contains(body)){
					rangeNodes.Add(body);
				}
				OnEnterRange?.Invoke(body); 
				break;

			case AreaType.Near:
				if(!nearNodes.Contains(body)){
					nearNodes.Add(body);
				}
				OnEnterNear?.Invoke(body); 
				break;

			case AreaType.Far:
				if(!farNodes.Contains(body)){
					farNodes.Add(body);
				}
				OnEnterFar?.Invoke(body);
				break;
		}

		OnEnter?.Invoke(body);
		GD.Print($"{Name} ENTER:\n Range:{rangeNodes.Count}\n Near:{nearNodes.Count}\n Far:{farNodes.Count}");	
	}

	private void HandleExit(Node2D body, AreaType type){
		switch(type){
			case AreaType.Range:
				if(rangeNodes.Contains(body)){
					rangeNodes.Remove(body);
				}
				OnExitRange?.Invoke(body); 
				break;

			case AreaType.Near:
				if(nearNodes.Contains(body)){
					nearNodes.Remove(body);
				}
				OnExitNear?.Invoke(body); 
				break;

			case AreaType.Far:
				if(farNodes.Contains(body)){
					farNodes.Remove(body);
				}
				OnExitFar?.Invoke(body);
				break;
		}

		OnExit?.Invoke(body);
		//GD.Print($"{Name} EXIT:\n Range:{rangeNodes.Count}\n Near:{nearNodes.Count}\n Far:{farNodes.Count}");	
	}

	public bool IsInside(Node2D body){
		return farNodes.Contains(body) || nearNodes.Contains(body) || rangeNodes.Contains(body);
	}

	public T GetNodeInside<T>(){
		if(rangeNodes.Count > 0)	return rangeNodes.OfType<T>().FirstOrDefault();
		if(nearNodes.Count > 0)	return nearNodes.OfType<T>().FirstOrDefault();
		if(farNodes.Count > 0)	return farNodes.OfType<T>().FirstOrDefault();

		return default(T);
	}

	private void UpdateColliderPoints(Area2D area, CollisionPolygon2D shape, VisionConeResource visionCone){
		if(area==null || shape==null)
			return;

		QueueRedraw();
		List<Vector2> colliderPoints = new List<Vector2>{Vector2.Zero};
		for(int i = 0; i < visionCone.Accuracy+1; i++){
			colliderPoints.Add(
				new Vector2(Mathf.Cos(-visionCone.Angle/2 + i*visionCone.Angle/visionCone.Accuracy), Mathf.Sin(-visionCone.Angle/2 + i*visionCone.Angle/visionCone.Accuracy)) * visionCone.Radius
			);
		}

		shape.Polygon = colliderPoints.ToArray();
	}

	public override void _Draw()
	{
		base._Draw();
		if(!Engine.IsEditorHint() && !DrawInGame)
			return;

		DrawCircleArcPoly(farVisionCone.Radius, farVisionCone.Angle, farColor);
		DrawCircleArcPoly(nearVisionCone.Radius, nearVisionCone.Angle, nearColor);
		DrawCircleArcPoly(rangeVisionCone.Radius, rangeVisionCone.Angle, rangeColor);
	}

	public override void _PhysicsProcess(double delta)
	{
		if(!IsInstanceValid(parentCharacter))
			return;

		base._PhysicsProcess(delta);
		
		Rotation = Mathf.LerpAngle(Rotation, parentCharacter.Velocity.Angle(), 0.1f);
	}

	public void DrawCircleArcPoly(float radius, float angleTo, Color color)
	{
		int nbPoints = 32;
		var pointsArc = new Vector2[nbPoints + 2];
		pointsArc[0] = Vector2.Zero;
		var colors = new Color[] { color };

		for (int i = 0; i <= nbPoints; i++)
		{
			float anglePoint = i * angleTo / nbPoints - angleTo/2;
			pointsArc[i + 1] = new Vector2(Mathf.Cos(anglePoint), Mathf.Sin(anglePoint)) * radius;
		}

		DrawPolygon(pointsArc, colors);
	}

	public string Print(){
		return $"RANGE: {String.Join(",", rangeNodes)}\nNEAR: {String.Join(",", nearNodes)}\nFAR: {String.Join(",", farNodes)}";
	}
}

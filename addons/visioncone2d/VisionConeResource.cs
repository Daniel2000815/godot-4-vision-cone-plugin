using System;
using Godot;

[GlobalClass, Tool]
public partial class VisionConeResource : Resource
{
    private ushort _accuracy = 10;
    private float _angle = 3.14f;
    private float _radius = 200;

    [Export(PropertyHint.Range, $"0,100,")] public ushort Accuracy {
		get => _accuracy;
		set{
			_accuracy = value;
			EmitChanged();
        }
	}

    [Export(PropertyHint.Range, $"0,6.28,")] public float Angle {
		get => _angle;
		set{
			_angle = value;
            EmitChanged();
		}
	}

    [Export(PropertyHint.Range, $"0,10000,10")] public float Radius {
		get => _radius;
		set{
			_radius = value;
			EmitChanged();
		}
	}


}
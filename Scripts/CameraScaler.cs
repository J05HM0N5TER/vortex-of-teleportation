using Godot;
using System;

public class CameraScaler : Camera2D
{
    private enum RATIO { HORIZONTAL, VERTICAL };
    [Export] private Vector2 _referenceResolution = new Vector2(480, 270);
    [Export] RATIO _ratioToMaintain;

    public override void _Ready()
    {
        float referenceAspectRatio = _referenceResolution.x / _referenceResolution.y;
        Vector2 currentResolution = OS.WindowSize;

        float horizontalRatio = _referenceResolution.x / currentResolution.x;
        float verticalRatio = _referenceResolution.y / currentResolution.y;

        float ratioToKeep = _ratioToMaintain == RATIO.HORIZONTAL ? horizontalRatio : verticalRatio;
        Zoom = Vector2.One * ratioToKeep;
    }
}
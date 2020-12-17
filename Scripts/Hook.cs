using Godot;
using System;

public class Hook : Node2D
{
    [Export]float MAX_LENGTH = 280f;

    Line2D line;
    Position2D end;
    RayCast2D rayCast;

    Player player;

    bool isClockWise;
    public float rotationSpeed = Mathf.Pi + 1;

    Vector2 castPoint = new Vector2();


    public override void _Ready(){
        line = GetNode<Line2D>("Line");
        end = GetNode<Position2D>("End");
        rayCast = GetNode<RayCast2D>("RayCast2D");

        int rand = (int) GD.RandRange(0,1);

        if(rand == 0){
            isClockWise = true;
        } else if (rand == 1){
            isClockWise = false;
        }

        
    }

    public override void _PhysicsProcess(float delta)
    {
        var maxCastTo = Vector2.Right.Normalized() * MAX_LENGTH;
        rayCast.CastTo = maxCastTo;

        GetNode<CPUParticles2D>("End/CPUParticles").Emitting = rayCast.IsColliding();

        if(rayCast.IsColliding()){
            castPoint = rayCast.GetCollisionPoint();
            end.GlobalPosition = castPoint;
            line.SetPointPosition(1, end.Position);

            GetNode<CPUParticles2D>("End/CPUParticles").Show();
            GetNode<CPUParticles2D>("End/CPUParticles").GlobalRotation = rayCast.GetCollisionNormal().Angle();
        }else{
            end.GlobalPosition = rayCast.CastTo;
            line.SetPointPosition(1, end.GlobalPosition);
            GetNode<CPUParticles2D>("End/CPUParticles").Hide();
        }

        SelfRotate(delta);

        
    }

    void SelfRotate(float delta){
        if(isClockWise){
            Rotation += rotationSpeed * delta;
        } else {
            Rotation -= rotationSpeed * delta;
        }

        if(Rotation >= Mathf.Deg2Rad(70)){
            isClockWise = false;
        } else if(Rotation <= -Mathf.Deg2Rad(70)){
            isClockWise = true;
        }


    }

    


}

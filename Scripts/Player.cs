using Godot;
using System;

public class Player : Area2D
{
    [Signal] 
    public delegate void captured(Area2D area);
    [Signal]
    public delegate void gemCaptured(int additionalScore);

    [Signal]
    public delegate void jumped();
    [Signal]
    public delegate void died();

    [Signal]
    public delegate void hooked();

    public Vector2 velocity = new Vector2(500,0);
    int trailLength = 10;
    bool isFloating = false;
    bool isHooked = false;

    [Export] float jumpSpeed = 1000f;
    [Export] float gravity = -55f;
    [Export] float waitTime = 2f;

    Circle target = null;
    Hook hook;
    Line2D trail;
    AnimatedSprite anim;
    Gem gem = null;
    

    public override void _Input(InputEvent @event){
        if(target != null && !isFloating && @event is InputEventScreenTouch touch && touch.Pressed){
            Jump();
        } else if(target == null && isFloating && @event is InputEventScreenTouch touchHook && touchHook.Pressed && hook.GetNode<RayCast2D>("RayCast2D").IsColliding()){
            EmitSignal(nameof(hooked));
        }
    }

    public override void _Ready(){
        anim = GetNode<AnimatedSprite>("AnimatedSprite");
        trail = GetNode<Line2D>("Trail/Line");
        hook = GetNode<Hook>("Hook");
        

        Connect(nameof(hooked), this, nameof(_on_Player_hooked));
    }

    void Jump(){
        GetNode<Musics>("/root/Music").PlaySound("Jump");
        target.GetNode<CollisionShape2D>("CollisionShape2D").Hide();
        target.Fade();

        EmitSignal(nameof(jumped));

        if(target.HasNode("Timer")){
            target.GetNode<Timer>("Timer").Stop();
        }
        target = null;

        isFloating = true;
        velocity = Transform.x * jumpSpeed;
        anim.Animation = "Jump";

        GetNode<Timer>("Timer").Start();
    }

    public void _on_Player_hooked(){
        isHooked = true;
        
        GetNode<Tween>("Tween").InterpolateProperty(this, "position", Position, hook.GetNode<RayCast2D>("RayCast2D").GetCollisionPoint(), 0.3f, Tween.TransitionType.Linear, Tween.EaseType.In);       
        GetNode<Tween>("Tween").Start();

          
    }

    void  _on_Player_area_entered(Area2D area){
        GetNode<Musics>("/root/Music").PlaySound("Capture");
        target = area as Circle;
        velocity = Vector2.Zero;
        isFloating = false;
        isHooked = false;

        anim.Animation = "Land";
        GetNode<CPUParticles2D>("AnimatedSprite/CPUParticles2D").Emitting = true;

        EmitSignal(nameof(captured), area as Circle);

    }

    void _on_Player_body_entered(Node body){
        if(body.IsInGroup("Gem")){
            gem = body as Gem;
            EmitSignal(nameof(gemCaptured), gem.GetAdditionalScore());
            GD.Print(gem.GetAdditionalScore());
            body.QueueFree();
            GetNode<Musics>("/root/Music").PlaySound("SFX");
        } else {
            return;
        }
    }

    public override void _PhysicsProcess(float delta){
        if(trail.Points.Length > trailLength){
            trail.RemovePoint(0);
        }
        trail.AddPoint(Position);

        if(target != null){
            isFloating = false;
            GetNode<Timer>("Timer").Stop();
            GetNode<Timer>("Timer").WaitTime = waitTime;
            Transform = target.GetNode<Position2D>("Pivot/Orbit").GlobalTransform;
        }
        else{
            isFloating = true;
            if(velocity > Vector2.Zero){
                velocity += Transform.x * gravity;
            }
            Position += velocity * delta;
        }

        if(isFloating){
            GetNode<AnimationPlayer>("AnimationPlayer").Play("Floating");
            GetNode<Hook>("Hook").Show();
        } else{
            GetNode<AnimationPlayer>("AnimationPlayer").Play("Orbiting");
            GetNode<Hook>("Hook").Hide();
        }

        if(isHooked){
            hook.rotationSpeed = 0;
            velocity = Vector2.Zero;
        } else {
            hook.rotationSpeed = Mathf.Pi;
        }

    }

    public void _on_Timer_timeout(){
        Die();
    }

    void _on_VisibilityNotifier2D_screen_exited(){
        Die();
    }

    public void Die(){
        GetNode<Musics>("/root/Music").PlaySound("Death");
        target = null;
        isFloating = false;
        EmitSignal(nameof(died));
        QueueFree();
    }
}

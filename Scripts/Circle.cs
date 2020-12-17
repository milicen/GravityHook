using Godot;
using Godot.Collections;
using System;

public class Circle : Area2D
{
    public enum Mode {
        Normal,
        Timed
    }


    public bool hasGem;

    public Position2D orbitPosition;
    AnimationPlayer animationPlayer;

    float radius = 150f;
    float rotationSpeed = Mathf.Pi;

    public Mode mode {get; set;}
    
    //delete below soon
    float waitTime = 0;
    //delete above soon
    
    Player player;

    PackedScene gemRes = (PackedScene) GD.Load("res://Scenes/Gem.tscn");
    Node2D gemPivot;

    float gemRotationSpeed = Mathf.Pi/3;


    public void Init(Vector2 _position,int score, Mode mode = Mode.Normal){
        player = null;
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        gemPivot = GetNode<Node2D>("GemPivot");

        if(score < 10){
            SetNormalSpawnRate(100, mode, score);
        } else  if(score < 20) {
            SetNormalSpawnRate(70, mode, score);
            radius = 148;
        } else if(score < 25){
            SetNormalSpawnRate(40, mode, score);
            radius = 144;
        } else {
            SetNormalSpawnRate(10, mode, score);
            radius = 140;
        }

        Position = _position;
        var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        
        collisionShape.Shape = (Shape2D) collisionShape.Shape.Duplicate();
        collisionShape.Shape.Set("radius", radius);

        var imgSize = GetNode<Sprite>("Sprite").Texture.GetSize().x / 2;
        GetNode<Sprite>("Sprite").Scale = new Vector2(1,1) * radius / imgSize;

        orbitPosition = GetNode<Position2D>("Pivot/Orbit");
        var pos = new Vector2(radius + 25, orbitPosition.Position.y);
        orbitPosition.Position = pos;
        rotationSpeed *= Mathf.Pow(-1, GD.Randi() % 2);

        SetLuckyGem();


    }

    void SetMode(Mode mode, float waitTime){
        this.mode = mode;
        if(mode == Mode.Timed){
            var timer = new Timer();
            timer.Name = "Timer";
            AddChild(timer);
            this.waitTime = waitTime;
            timer.WaitTime = waitTime;
        }
    }

    void SetNormalSpawnRate(int normalPercentage, Mode mode, int count){
        int rand = (int) GD.RandRange(0, 100);

        if(rand < normalPercentage){
            mode = Mode.Normal;
        } else {
            mode = Mode.Timed;
        }

        if(mode == Mode.Timed && count < 20){
            SetMode(mode, 1.8f);
        } else if(mode == Mode.Timed && count < 25){
            SetMode(mode, 1.7f);
        } else {
            SetMode(mode, 1.6f);
        }
    }

    void SetLuckyGem(){
        float rand = (float) GD.RandRange(0,1);

        if(rand <= 0.1f){            
            gemPivot.Show();

            var gem = (Gem) gemRes.Instance();
            gemPivot.AddChild(gem);
            
            var pos = new Vector2(radius + 25, gem.Position.y);
            gemRotationSpeed *= Mathf.Pow(-1, GD.Randi() % 2);
            gem.Position = pos;
            
            GD.Print("lucky gem spawned");
        }

    }

    public async void CheckTime(){
        await ToSignal(GetNode<Timer>("Timer"), "timeout");
        player.Die();
        Fade();
    }

    public override void _Process(float delta){
        GetNode<Node2D>("Pivot").Rotation += rotationSpeed * delta;

        if(mode == Mode.Timed && player != null){
            CheckTime();
        }

        if(gemPivot.Visible){
            gemPivot.Rotation += gemRotationSpeed * delta;
        }
    }

    public async void Capture(Player _player){
        player = _player;
        player.Connect("jumped", this, nameof(_on_Player_jumped));
        GetNode<Node2D>("Pivot").Rotation = (player.Position - Position).Angle();
        GetNode<Sprite>("Sprite/Sprite2").Show();
        animationPlayer.Play("Capture");
        await ToSignal(animationPlayer, "animation_finished");
        GetNode<Sprite>("Sprite/Sprite2").Hide();

        if(mode == Mode.Timed ){
            animationPlayer.Play("Explode", -1, GetNode<Timer>("Timer").WaitTime);
        }
    }

    void _on_Player_jumped(){
        player = null;
    }

    public async void Fade(){
        animationPlayer.Play("Fade");
        ;
        await ToSignal(GetNode<AnimationPlayer>("AnimationPlayer"), "animation_finished");
        QueueFree();
    }
}

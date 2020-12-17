using Godot;
using Godot.Collections;
using System;


public class Main : Node
{
    //spawn position minmax
    [Export] float maxYTop = -300;
    [Export] float minYBottom = 300;
    [Export] float minXRight = 600;
    [Export] float maxXRight = 700;

    PackedScene Player = (PackedScene)GD.Load("res://Scenes/Player.tscn");
    PackedScene Circle = (PackedScene) GD.Load("res://Scenes/Circle.tscn");

    Player player;
    Camera2D camera;
    Circle currentCircle;
    HUD HUD;
    SaveFile saveFile;

    //set to -1 bcs player gets one point as soon as it's spawned
    int score = -1;
    int count = -1;


    public override void _Ready()
    {
        //save camera node for covenience
        camera = GetNode<Camera2D>("Camera");
        camera.Current = true;

        HUD = GetNode<HUD>("HUD");
        HUD.Connect("Retry", this, nameof(_on_Retry));

        saveFile = GetNode<SaveFile>("/root/SaveFile");

        GD.Randomize();
        NewGame();
    }

    void NewGame(){
        score = -1;
        count = -1;

        HUD.ShowScore();
        HUD.ShowGameover(false);

        GetNode<Camera2D>("Camera").SmoothingEnabled = false;
        GetNode<Camera2D>("Camera").Position = GetNode<Position2D>("StartPosition").Position;

        player = Player.Instance() as Player;
        AddChild(player);
        player.Position = GetNode<Position2D>("StartPosition").Position;

        player.Connect("captured", this, nameof(_on_Player_captured));
        player.Connect("gemCaptured", this, nameof(_on_Player_gemCaptured));
        player.Connect("died", this, nameof(_on_Player_died));

        SpawnCircle(GetNode<Position2D>("StartPosition").Position);
        currentCircle = GetNode<Circle>("Circle");

    }
    
    public void SpawnCircle(Vector2 spawnPos = new Vector2()){
        var circleInstance = Circle.Instance() as Circle;

        if(spawnPos == new Vector2()){
            var x = (float)GD.RandRange(minXRight, maxXRight);
            var y = (float) GD.RandRange(maxYTop, minYBottom);
            spawnPos = currentCircle.Position + new Vector2(x, y);
        }     


        AddChild(circleInstance);
        circleInstance.Init(spawnPos, count);

 
    }

    public void _on_Player_captured(Area2D area){
        if(!camera.SmoothingEnabled){
            camera.SmoothingEnabled = true;
        }
        camera.Position = area.Position;
        currentCircle = area as Circle;
        currentCircle.Capture(player);

        if((currentCircle.HasNode("Timer"))){
            currentCircle.GetNode<Timer>("Timer").Start();

        }
        
        CallDeferred(nameof(SpawnCircle), new Vector2());

        count++;
        score++;
        HUD.SetScore(HUD.scoreDisplay, score);
    }

    void _on_Player_gemCaptured(int additionalScore){
        score += additionalScore;
        HUD.SetScore(HUD.scoreDisplay, score);
        HUD.ShowAdditionalScore("+" + additionalScore.ToString());
    }

    public void _on_Player_died(){
        GetTree().CallGroup("Circles", "Fade");
        HUD.ShowScore(false);
        HUD.ShowGameover();
        HUD.SetScore(HUD.gameoverScoreDisplay, score);
        if(score > saveFile.highscore){
            saveFile.SetHighScore(score);
        }
        HUD.SetScore(HUD.highscoreDisplay, saveFile.highscore);
        
    }


    public void _on_Retry(){
        NewGame();
    }


}

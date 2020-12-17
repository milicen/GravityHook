using Godot;
using System;

public class HUD : Node
{
    [Signal]
    public delegate void Retry();

    Global global;
    [Export] string mainMenuScenePath = "res://Scenes/MainMenu.tscn";

    public string scoreDisplay = "ScoreDisplay/Score";
    public string gameoverScoreDisplay = "GameOver/Container/ScoreContainer/Score";
    public string highscoreDisplay = "GameOver/Container/HighscoreContainer/Highscore";

    public override void _Ready()
    {
        global = GetNode<Global>("/root/Global");
    }

    public void _on_HomeButton_pressed(){
        GetNode<Musics>("/root/Music").PlaySound("Button");
        global.GotoScene("res://Scenes/MainMenu.tscn");
    }

    public void _on_RetryButton_pressed(){
        GetNode<Musics>("/root/Music").PlaySound("Button");
        EmitSignal(nameof(Retry));
        ShowGameover(false);
        ShowScore(true);
    }

    public void ShowScore(bool truthy = true){
        if(truthy == true){
            GetNode<Label>(scoreDisplay).Show();
        }else{
            GetNode<Label>(scoreDisplay).Hide();
        }
    }

    public void ShowGameover(bool truthy = true){
        if(truthy == true){
            GetNode<VBoxContainer>("GameOver/Container").Show();
        }else{
            GetNode<VBoxContainer>("GameOver/Container").Hide();
        }
    }

    public void SetScore(NodePath path, int score){
        GetNode<Label>(path).Text = score.ToString();
    }

    public async void ShowAdditionalScore(string score){
        GetNode<Label>("ScoreDisplay/Label").Text = score;
        GetNode<Label>("ScoreDisplay/Label").Show();
        
        var tween = new Tween();
        AddChild(tween);

        var startOpacity = new Color(1, 1, 1, 0.35f);
        var endOpacity = new Color(1, 1, 1, 0); 
        
        tween.InterpolateProperty(GetNode<Label>("ScoreDisplay/Label"), "modulate", startOpacity, endOpacity, 1, Tween.TransitionType.Linear, Tween.EaseType.Out);
        tween.Start();

        await ToSignal(tween, "tween_completed");
        GetNode<Label>("ScoreDisplay/Label").Hide();
        RemoveChild(tween);
        tween.QueueFree();
    }
}

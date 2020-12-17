using Godot;
using System;

public class MainMenu : CanvasLayer
{
    [Export] string mainScenePath = "res://Scenes/Main.tscn";
    AnimationPlayer animationPlayer;

    SaveFile file;
    Musics musics;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("ZoomIn", -1, -0.1f, true);

        file = GetNode<SaveFile>("/root/SaveFile");
        musics = GetNode<Musics>("/root/Music");

        if(!file.isPlayingBGM){
            GetNode<CheckButton>("Settings/VBoxContainer/Music/MusicToggle").Pressed = false;
            GetNode<Sprite>("Settings/VBoxContainer/Music/MusicToggle/Border/Off").Show();
            GetNode<Sprite>("Settings/VBoxContainer/Music/MusicToggle/Border/On").Hide();
        } else {
            GetNode<CheckButton>("Settings/VBoxContainer/Music/MusicToggle").Pressed = true;
            GetNode<Sprite>("Settings/VBoxContainer/Music/MusicToggle/Border/Off").Hide();
            GetNode<Sprite>("Settings/VBoxContainer/Music/MusicToggle/Border/On").Show();
        }

        if(!file.isPlayingSFX){
            GetNode<CheckButton>("Settings/VBoxContainer/SFX/SFXToggle").Pressed = false;
            GetNode<Sprite>("Settings/VBoxContainer/SFX/SFXToggle/Border/Off").Show();
            GetNode<Sprite>("Settings/VBoxContainer/SFX/SFXToggle/Border/On").Hide();
        } else {
            GetNode<CheckButton>("Settings/VBoxContainer/SFX/SFXToggle").Pressed = true;
            GetNode<Sprite>("Settings/VBoxContainer/SFX/SFXToggle/Border/Off").Hide();
            GetNode<Sprite>("Settings/VBoxContainer/SFX/SFXToggle/Border/On").Show();
        }
    }

    public async void _on_PlayButton_pressed(){
        musics.PlaySound("Button");
        animationPlayer.Play("ZoomIn", -1, 0.5f);
        await ToSignal(animationPlayer, "animation_finished");

        var global =  GetNode<Global>("/root/Global");
        global.GotoScene(mainScenePath);
    }

    void _on_Settings_pressed(){
        musics.PlaySound("Button");
        var tween = GetNode<Tween>("Tween");
        var startOffset = Offset;
        var endOffset = new Vector2(-640, 0);
        tween.InterpolateProperty(this, "offset", startOffset, endOffset, 0.5f);
        tween.Start();
    }

    void _on_SetDoneButton_pressed(){
        musics.PlaySound("Button");
        var tween = GetNode<Tween>("Tween");
        var startOffset = Offset;
        var endOffset = new Vector2(0, 0);
        tween.InterpolateProperty(this, "offset", startOffset, endOffset, 0.5f);
        tween.Start();
    }

    void _on_MusicToggle_pressed(){
        musics.PlaySound("Toggle");

        if(!GetNode<CheckButton>("Settings/VBoxContainer/Music/MusicToggle").Pressed){
            file.isPlayingBGM = false;
        } else {
            file.isPlayingBGM = true;
        }

        CheckBGMPlay();

        file.SaveBGMPlaying();
    }

    void _on_SFXToggle_pressed(){
        musics.PlaySound("Toggle");

        if(!GetNode<CheckButton>("Settings/VBoxContainer/SFX/SFXToggle").Pressed){
            file.isPlayingSFX = false;
        } else {
            file.isPlayingSFX = true;
        }

        CheckSFXPlay();

        file.SaveSFXPlaying();
    }

    void CheckBGMPlay(){
        if(!file.isPlayingBGM){
            musics.PlayBGM();
            GetNode<Sprite>("Settings/VBoxContainer/Music/MusicToggle/Border/Off").Show();
            GetNode<Sprite>("Settings/VBoxContainer/Music/MusicToggle/Border/On").Hide();
        } else {
            musics.PlayBGM();
            GetNode<Sprite>("Settings/VBoxContainer/Music/MusicToggle/Border/Off").Hide();
            GetNode<Sprite>("Settings/VBoxContainer/Music/MusicToggle/Border/On").Show();
        }
    }

    void CheckSFXPlay(){
        if(!file.isPlayingSFX){
            GetNode<Sprite>("Settings/VBoxContainer/SFX/SFXToggle/Border/Off").Show();
            GetNode<Sprite>("Settings/VBoxContainer/SFX/SFXToggle/Border/On").Hide();
        } else {
            GetNode<Sprite>("Settings/VBoxContainer/SFX/SFXToggle/Border/Off").Hide();
            GetNode<Sprite>("Settings/VBoxContainer/SFX/SFXToggle/Border/On").Show();
        }
    }


}

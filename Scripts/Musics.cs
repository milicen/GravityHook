using Godot;
using System;

public class Musics : Node
{

    AudioStream[] audioStream = {
        (AudioStream) GD.Load("res://Musics/multimedia_button_click_001.ogg"),
        (AudioStream) GD.Load("res://Musics/zapsplat_multimedia_button_click_fast_plastic_49161.ogg"),
        (AudioStream) GD.Load("res://Musics/zapsplat_multimedia_game_tone_short_bright_futuristic_beep_with_delay_action_tone_001_59162.ogg"),
        (AudioStream) GD.Load("res://Musics/zapsplat_cartoon_frog_jump_26526.ogg"),
        (AudioStream) GD.Load("res://Musics/water_splash_fist_punch.ogg"),
        (AudioStream) GD.Load("res://Musics/zapsplat_multimedia_game_error_tone_001_24919.ogg")

    };
    bool canClick = true;
    public override void _Ready()
    {
        PlayBGM();
    }

    public void PlayBGM(){
        if(GetNode<SaveFile>("/root/SaveFile").isPlayingBGM){
            GetNode<AudioStreamPlayer>("BGM").Play();
        } else {
            GetNode<AudioStreamPlayer>("BGM").Stop();
        }
    }

    public void PlaySound(string soundName){
        if(GetNode<SaveFile>("/root/SaveFile").isPlayingSFX){
            if(soundName == "SFX"){
                GetNode<AudioStreamPlayer>("SFX").Stream = audioStream[2];
            } else if (soundName == "Button"){
                GetNode<AudioStreamPlayer>("SFX").Stream = audioStream[0];     
            } else if (soundName == "Toggle"){
                GetNode<AudioStreamPlayer>("SFX").Stream = audioStream[1];
            } else if(soundName == "Jump"){
                GetNode<AudioStreamPlayer>("SFX").Stream = audioStream[3];
            } else if(soundName == "Capture"){
                GetNode<AudioStreamPlayer>("SFX").Stream = audioStream[4];
            } else if(soundName == "Death"){
                GetNode<AudioStreamPlayer>("SFX").Stream = audioStream[5];
            }

            GetNode<AudioStreamPlayer>("SFX").Play();
        }
    }


}
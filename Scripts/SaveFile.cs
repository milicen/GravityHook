using Godot;
using System;
using System.Collections.Generic;

public class SaveFile : Node
{
    public int highscore {get; set;}
    public bool isPlayingBGM {get; set;}
    public bool isPlayingSFX {get; set;}
    const string bgmFilePath = "user://bgm.data";
    const string sfxFilePath = "user://sfx.data";
    const string filePath = "user://highscore.data";

    public IDictionary<string, bool> data = new Dictionary<string, bool>(){
        {"bgmPlaying", true},
        {"sfxPlaying", true}
    };
    const string dataPath = "user://user_data.data";
    public override void _Ready()
    {
        LoadHighScore();
        LoadBGMPlaying();
        LoadSFXPlaying();
    }


    public void LoadHighScore(){
        var file = new File();
        if(!file.FileExists(filePath)){
            return;
        }
        file.Open(filePath, File.ModeFlags.Read);
        highscore = (int) file.GetVar();
        file.Close();
    }

    public void SaveHighScore(){
        var file = new File();
        file.Open(filePath, File.ModeFlags.Write);
        file.StoreVar(highscore);
        file.Close();
    }

    public void SetHighScore(int newHighScore){
        highscore = newHighScore;
        SaveHighScore();
    }

    public void LoadBGMPlaying(){
        var file = new File();
        if(!file.FileExists(bgmFilePath)){
            return;
        }
        file.Open(bgmFilePath, File.ModeFlags.Read);
        isPlayingBGM = (bool) file.GetVar();
        file.Close();
    }

    public void SaveBGMPlaying(){
        var file = new File();
        file.Open(bgmFilePath, File.ModeFlags.Write);
        file.StoreVar(isPlayingBGM);
        file.Close();
    }

    public void LoadSFXPlaying(){
        var file = new File();
        if(!file.FileExists(sfxFilePath)){
            return;
        }
        file.Open(sfxFilePath, File.ModeFlags.Read);
        isPlayingSFX = (bool) file.GetVar();
        file.Close();
    }

    public void SaveSFXPlaying(){
        var file = new File();
        file.Open(sfxFilePath, File.ModeFlags.Write);
        file.StoreVar(isPlayingSFX);
        file.Close();
    }


}

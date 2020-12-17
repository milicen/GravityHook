using Godot;
using System;

public class Global : Node
{
    public Node CurrentScene {get; set;}
    Texture texture = (Texture) GD.Load("res://Assets/BG.png");

    public override void _Ready()
    {
        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
    }

    public void GotoScene(string scenePath){
        //called thru signal
        CallDeferred(nameof(DeferredGotoScene), scenePath);
    }

    public void DeferredGotoScene(string scenePath){
        //safe to free scene
        CurrentScene.Free();

        //load next scene
        var nextScene = (PackedScene) GD.Load(scenePath);

        //instance new scene
        CurrentScene = nextScene.Instance();

        //add to active scene as child of root
        GetTree().Root.AddChild(CurrentScene);

    }
}

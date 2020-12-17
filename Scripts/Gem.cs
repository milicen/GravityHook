using Godot;
using System;

public class Gem : KinematicBody2D
{   
    int additionalScore;

    public override void _Ready(){
        int rand = (int) GD.RandRange(0,100);

        if(rand < 85){
            additionalScore = 1;
            Modulate = new Color(0.98f, 1, 0, 1);
        } else if(rand < 95){
            additionalScore = 2;
            Modulate = new Color(0, 1, 0.93f, 1);
        } else {
            additionalScore = 3;
            Modulate = new Color(1, 1, 1, 1);
        }

    }

    public int GetAdditionalScore(){
        return additionalScore;
    }
}

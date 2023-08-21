using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionJSON
{
    public string Genre; 
    public string Category; 
    public string Question;
    public string Answer;
    public string False1; 
    public string False2; 
    public string False3;

    public QuestionJSON(string genre, string category, string question,string answer, string false1, string false2, string false3)
    {
        Genre = genre;
        Category = category;
        Question = question;
        Answer = answer; // 0
        False1 = false1; // 1
        False2 = false2; // 2
        False3 = false3; // 3
    }
}

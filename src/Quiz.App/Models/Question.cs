﻿using System.Collections.Generic;
using System.Linq;

namespace Quiz.App.Models
{
    public class Question : BaseModel
    {
        public Question(string text, List<PossibleAnswer> possibleAnswers, int index) : base()
        {
            Text = text;
            PossibleAnswers = possibleAnswers;
            Index = index;
        }

        public string Text { get; private set; }
        public List<PossibleAnswer> PossibleAnswers { get; private set; }
        public int Index { get; }
        
        public bool IsCorrectAnswer(string answer)
        {
            return GetCorrectAnswer() == answer;
        }

        private string GetCorrectAnswer()
        {
            return PossibleAnswers.FirstOrDefault(x => x.IsAnswer)?.Answer;
        }
    }
}
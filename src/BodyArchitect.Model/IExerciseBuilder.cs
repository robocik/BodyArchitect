using System;
using System.Collections.Generic;


namespace BodyArchitect.Model
{
    public interface IExerciseBuilder
    {
        List<Exercise> Create();
    }
}

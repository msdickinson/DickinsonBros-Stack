﻿namespace DickinsonBros.Infrastructure.Rest.Runner.AspDI.Models.Models
{
    public class Todo
    {
        public int UserId  {get; set;}
        public int Id  {get; set;}
        public string Title { get; set;}
        public bool Completed { get; set; }
    }
}

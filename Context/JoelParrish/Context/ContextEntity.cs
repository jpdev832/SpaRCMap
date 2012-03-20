using System;
using JoelParrish.Context.utils;

namespace JoelParrish.Context
{
    public class ContextEntity
    {
        public string uid { get; set; }
        public string country { get; set; }
        public string region { get; set; }
        public string town { get; set; }
        public DateTime dateTime { get; set; }
        public Vector3 position { get; set; }
        public string environmentType { get; set; }
        public string InteractionType { get; set; }
        public string specificType { get; set; }
        public object data { get; set; }
        public Type dataType { get; set; }
    }
}

﻿using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.Cosmos.Models
{
    [ExcludeFromCodeCoverage]
    public class CosmosEntity
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string _etag { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeCityLight.GoCity
{
    public enum GoCityNodeType {
        PACKAGE     // displayed as district
        , FILE      // displayed as building
        , STRUCT    // displayed as tower on top of a building 
    }

    public class GoCityNodePosition
    {
        [JsonPropertyName("x")]
        public float X { get; set; }

        [JsonPropertyName("y")]
        public float Y { get; set; }
    }

    public class GoCityNode
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonIgnore]
        public GoCityNodeType NType { get; set; }

        [JsonPropertyName("type")]
        public string Type
        {
            get { return NType == null ? "" : NType.ToString("f"); }
            set { if(Enum.TryParse(value, out GoCityNodeType t)) NType = t; }
        }

        [JsonPropertyName("width")]
        public float Width { get; set; }

        [JsonPropertyName("depth")]
        public float Depth { get; set; }

        [JsonPropertyName("position")]
        public GoCityNodePosition Position { get; set; } = new GoCityNodePosition();

        [JsonPropertyName("numberOfLines")]
        public int NumberOfLines { get; set; }

        [JsonPropertyName("numberOfMethods")]
        public int NumberOfMethods { get; set; }

        [JsonPropertyName("numberOfAttributes")]
        public int NumberOfAttributes { get; set; }

        [JsonPropertyName("children")]
        public List<GoCityNode> Children { get; set; } = new List<GoCityNode>();

        public void GenerateChildrenPosition()
        {
            if (Children.Count == 0)
            {
                Width = NumberOfAttributes + 1;
                Depth = NumberOfAttributes + 1;
                return;
            }
            var positionGenerator = new NodePositionGenerator(Children.Count);

            foreach(var child in Children)
            {
                child.GenerateChildrenPosition();
                child.Position = positionGenerator.NextPosition(child.Width, child.Depth);
            }

            var bounds = positionGenerator.GetBounds();

            Width = bounds.X;
            Depth = bounds.Y;

            foreach(var child in Children)
            {
                child.Position.X -= Width / 2;
                child.Position.Y -= Depth / 2;
            }

            if(NType == GoCityNodeType.FILE)
            {
                Width += NumberOfAttributes;
                Depth += NumberOfAttributes;
            }
        }

        public string ToJson(bool writeIndented = false)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = writeIndented
            };
            return JsonSerializer.Serialize(this, options);
        }
    }

    internal class NodePositionGenerator
    {
        private static readonly int defaultMargin = 1;
        private readonly int numberNodes;
        private readonly int dimension;
        private float xReference;
        private float yReference;
        private int currentIndex;
        private float maxWidth;
        private float maxHeight;

        internal NodePositionGenerator(int numberOfNodes)
        {
            numberNodes = numberOfNodes;
            dimension = (int)Math.Ceiling(Math.Sqrt(numberOfNodes));
        }

        internal GoCityNodePosition GetBounds()
        {
            return new GoCityNodePosition
            {
                X = maxWidth + defaultMargin,
                Y = maxHeight + defaultMargin
            };
        }

        internal GoCityNodePosition NextPosition(float width, float height)
        {
            currentIndex++;
            if (currentIndex > dimension && yReference + height >= maxWidth)
            {
                currentIndex = 0;
                yReference = 0;
                xReference = maxWidth + defaultMargin;
            }
            var position = new GoCityNodePosition
            {
                X = xReference + (width + defaultMargin) / 2,
                Y = yReference + (height + defaultMargin) / 2,
            };
            if (xReference + width > maxWidth)
            {
                maxWidth = xReference + width;
            }
            if (yReference + height > maxHeight)
            {
                maxHeight = yReference + height;
            }
            yReference += height + defaultMargin;
            return position;
        }
    }

}

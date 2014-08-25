
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class EntityField
    {
        public EntityField()
        {
        }

        public EntityField(string name, EntityFieldType type, EntityEnumField enumField)
        {
            this.Name = name;
            this.Type = type;
            this.EnumField = enumField;
        }

        public string Name { get; set; }

        internal void SetType(string p)
        {
            switch (p)
            {
                case "string":
                case "text":
                case "html":
                    this.Type = EntityFieldType.String;
                    return;

                case "url":
                    this.Type = EntityFieldType.Url;
                    return;

                case "bool":
                    this.Type = EntityFieldType.Boolean;
                    return;

                case "float":
                    this.Type = EntityFieldType.Float;
                    return;

                case "datetime":
                case "date":
                    this.Type = EntityFieldType.DateTime;
                    return;

                case "integer":
                case "width":
                case "height":
                    this.Type = EntityFieldType.Integer;
                    return;
            }

            if (p.Contains("|"))
            {
                var enumField = new EntityEnumField();
                this.EnumField = enumField;
                var parts = p.Split('|');
                for (int i = 0; i < parts.Length; i++)
                {
                    enumField.Values.Add(parts[i]);
                }

                return;
            }

            throw new NotSupportedException("ENtity field type '" + p + "' is not supported");
        }

        public EntityFieldType Type { get; set; }

        public EntityEnumField EnumField { get; set; }

        public Entity Entity { get; set; }
    }

    public class EntityEnumField : IEnumerable<string>
    {
        public EntityEnumField()
        {
            this.Values = new List<string>();
            this.Names = new List<string>();
        }

        public string Name { get; set; }
        public List<string> Names { get; set; }

        public List<string> Values { get; set; }

        public void Add(string value)
        {
            this.Values.Add(value);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }
    }

    public enum EntityFieldType
    {
        Unknown,
        String,
        Url,
        DateTime,
        Integer,
        Enum,
        Entity,
        Boolean,
        Float,
    }
}

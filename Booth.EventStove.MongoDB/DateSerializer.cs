using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.Attributes;

using Booth.Common;

[assembly: InternalsVisibleToAttribute("Booth.EventStore.MongoDB.Test")]

namespace Booth.EventStore.MongoDB
{
    [BsonSerializer(typeof(DateSerializer))]
    class DateSerializer : SerializerBase<Date>
    {
        public override Date Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.String)
            {
                var value = context.Reader.ReadString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return Date.MinValue;
                }
                if (Date.TryParseExact(value, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out Date date))
                    return date;
                else
                    return Date.MinValue;
            }
            else if (context.Reader.CurrentBsonType == BsonType.DateTime)
            {
                var value = context.Reader.ReadDateTime();
                var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(value).DateTime;

                return new Date(dateTime);
            }
            else if (context.Reader.CurrentBsonType == BsonType.Timestamp)
            {
                var value = context.Reader.ReadTimestamp();
                var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(value).DateTime;

                return new Date(dateTime);
            }
            else
            {
                context.Reader.SkipValue();
                return Date.MinValue;
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Date value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }
            context.Writer.WriteString(value.ToString("yyyy-MM-dd"));
        }
    }
}

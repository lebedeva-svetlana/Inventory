namespace Model
{
    public class Object
    {
        public Object(long objectId, ObjectType objectType, string description)
        {
            ObjectId = objectId;
            ObjectType = objectType;
            Description = description;
        }

        public long ObjectId { get; set; }

        public ObjectType ObjectType { get; set; }

        public string? Description { get; set; }

        public string ObjectName => ObjectType.ObjectTypeName + " " + Description;
    }
}
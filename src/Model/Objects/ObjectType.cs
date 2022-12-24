namespace Model
{
    public class ObjectType
    {
        public ObjectType(long objectTypeId, string objectTypeName)
        {
            ObjectTypeId = objectTypeId;
            ObjectTypeName = objectTypeName;
        }

        public long ObjectTypeId { get; set; }

        public string ObjectTypeName { get; set; }
    }
}
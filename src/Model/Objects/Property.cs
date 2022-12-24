namespace Model
{
    public class Property
    {
        public Property(long propertyId, long placeId, string place, long objectTypeId, string objectType, string description, string inventoryNumber, bool isInStock)
        {
            PropertyId = propertyId;
            PlaceId = placeId;
            Place = place;
            ObjectTypeId = objectTypeId;
            ObjectType = objectType;
            Description = description;
            InventoryNumber = inventoryNumber;
            IsInStock = isInStock;
        }

        public long PropertyId { get; set; }

        public long PlaceId { get; set; }
        public string Place { get; set; }

        public long ObjectTypeId { get; set; }
        public string ObjectType { get; set; }

        public string? Description { get; set; }

        public string InventoryNumber { get; set; }

        public bool IsInStock { get; set; }
    }
}
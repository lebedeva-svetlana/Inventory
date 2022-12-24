namespace Model
{
    public class Place
    {
        public Place(long placeId, string placeName)
        {
            PlaceId = placeId;
            PlaceName = placeName;
        }

        public long PlaceId { get; set; }
        public string PlaceName { get; set; }
    }
}
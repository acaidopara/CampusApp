namespace Rise.Domain.Infrastructure;

public class CampusFacilities : ValueObject
{
    public bool Library { get; private set; }
    public bool RitaHelpdesk { get; private set; }
    public bool RevolteRoom { get; private set; }
    public bool ParkingLot { get; private set; }
    public bool BikeStorage { get; private set; }
    public bool StudentShop { get; private set; }
    public bool Restaurant { get; private set; }
    public bool Cafeteria { get; private set; }
    public bool SportsHall { get; private set; }
    public bool Stuvo { get; private set; }
    public bool Lockers { get; private set; }
    
    private CampusFacilities() { }

    public CampusFacilities(
        bool library,
        bool ritaHelpdesk,
        bool revolteRoom,
        bool parkingLot,
        bool bikeStorage,
        bool studentShop,
        bool restaurant,
        bool cafeteria,
        bool sportsHall,
        bool stuvo,
        bool lockers)
    {
        Library = library;
        RitaHelpdesk = ritaHelpdesk;
        RevolteRoom = revolteRoom;
        ParkingLot = parkingLot;
        BikeStorage = bikeStorage;
        StudentShop = studentShop;
        Restaurant = restaurant;
        Cafeteria = cafeteria;
        SportsHall = sportsHall;
        Stuvo = stuvo;
        Lockers = lockers;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Library;
        yield return RitaHelpdesk;
        yield return RevolteRoom;
        yield return ParkingLot;
        yield return BikeStorage;
        yield return StudentShop;
        yield return Restaurant;
        yield return Cafeteria;
        yield return SportsHall;
        yield return Stuvo;
        yield return Lockers;
    }
}
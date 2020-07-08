using OctoAwesome.Entities;

namespace OctoAwesome.Basics.Entities
{
    public class Dog : ControllableEntity
    {
        public Dog(Coordinate coordinate)
        {
            Position = coordinate;
            Radius = 0.5f;
            Height = 1f;
            Mass = 100;
        }
    }
}   
